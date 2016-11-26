namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Util;
    using Android.Views;
    using Android.Widget;

    using AdMaiora.AppKit.UI;

    using AdMaiora.Bugghy.Model;
    using AdMaiora.Bugghy.Api;

    #pragma warning disable CS4014
    public class IssueFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private int _gimmickId;
        private int _userId;

        private Issue _issue;

        private IssueType _currentIssueType;

        // This flag check if we are already calling the login REST service
        private bool _isSendingIssue;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        #endregion

        #region Widgets

        [Widget]
        private ImageView TypeImage;

        [Widget]
        private TextView TapToChangeTypeLabel;

        [Widget]
        private TextView CodeLabel;

        [Widget]
        private EditText TitleText;

        [Widget]
        private EditText DescriptionText;

        [Widget]
        private ImageButton EditButton;

        #endregion

        #region Constructors

        public IssueFragment()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Fragment Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _gimmickId = this.Arguments.GetInt("GimmickId");
            _userId = AppController.Settings.LastLoginUsernameId;
            _issue = this.Arguments.GetObject<Issue>("Issue");
            _currentIssueType = _issue != null ? _issue.Type : IssueType.Crash;
        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentIssue, inflater, container);

            SlideUpToShowKeyboard();

            this.HasOptionsMenu = true;

            #endregion

            this.Title = _currentIssueType.ToString();                        

            this.TypeImage.Clickable = _issue == null;
            this.TypeImage.Enabled = _issue == null;
            this.TypeImage.SetOnTouchListener(GestureListener.ForSingleTapUp(this.Activity,
                (e) =>
                {
                    _currentIssueType++;
                    if ((int)_currentIssueType == 4)
                        _currentIssueType = IssueType.Crash;

                    this.Title = _currentIssueType.ToString();
                    this.TapToChangeTypeLabel.Visibility = ViewStates.Gone;
                    SetIssueTypeImage(_currentIssueType);

                }));

            this.DescriptionText.FocusChange += DescriptionText_FocusChange;

            this.EditButton.Click += EditButton_Click;

            SetIssueTypeImage(_currentIssueType);

            if (_issue != null)
                LoadIssue();    
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            menu.Clear();

            if(_issue == null || _issue.Status == IssueStatus.Opened)
                menu.Add(0, 1, 0, "Save").SetShowAsAction(ShowAsAction.Always);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {

                case Android.Resource.Id.Home:

                    DismissKeyboard();

                    (new Handler()).PostDelayed(
                        () =>
                        {
                            this.FragmentManager.PopBackStack();

                        }, 100);

                    return true;

                case 1:

                    if (_issue == null)
                        AddIssue();
                    else
                        UpdateIssue();

                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            if (_cts0 != null)
                _cts0.Cancel();

            this.DescriptionText.FocusChange -= DescriptionText_FocusChange;

            this.EditButton.Click -= EditButton_Click;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoadIssue()
        {
            this.TapToChangeTypeLabel.Visibility = ViewStates.Gone;

            this.CodeLabel.Text = _issue.Code;

            this.Title = _issue.Type.ToString();            

            this.TitleText.Text = _issue.Title;
            this.TitleText.Enabled = _issue.Status == IssueStatus.Opened;

            this.DescriptionText.Text = _issue.Description;

            this.EditButton.Visibility = 
                _issue == null || _issue.Status == IssueStatus.Opened ? ViewStates.Visible : ViewStates.Gone;

            SetIssueTypeImage(_issue.Type);
        }

        private void SetIssueTypeImage(IssueType type)
        {
            _currentIssueType = type;

            string[] typeImages = new[] { "image_gear", "image_issue_crash", "image_issue_blocking", "image_issue_nblocking" };
            this.TypeImage.SetImageResource(typeImages[(int)_currentIssueType]);            
        }

        private bool ValidateInput()
        {
            var validator = new WidgetValidator()
                .AddValidator(() => this.TitleText.Text, WidgetValidator.IsNotNullOrEmpty, "Please insert a title!")
                .AddValidator(() => this.DescriptionText.Text, WidgetValidator.IsNotNullOrEmpty, "Please insert a description!");

            string errorMessage;
            if (!validator.Validate(out errorMessage))
            {
                Toast.MakeText(this.Activity.ApplicationContext, errorMessage, ToastLength.Long).Show();

                return false;
            }

            return true;
        }

        private void AddIssue()
        {
            if (_isSendingIssue)
                return;

            if (!ValidateInput())
                return;

            string title = this.TitleText.Text;
            string description = this.DescriptionText.Text;

            _isSendingIssue = true;
            ((MainActivity)this.Activity).BlockUI();

            _cts0 = new CancellationTokenSource();
            AppController.AddIssue(_cts0,
                _gimmickId,
                _userId,
                title,
                description,
                _currentIssueType,
                (todoItem) =>
                {
                    this.FragmentManager.PopBackStack();
                },
                (error) =>
                {
                    Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
                },
                () =>
                {
                    _isSendingIssue = false;
                    ((MainActivity)this.Activity).UnblockUI();
                });
        }

        private void UpdateIssue()
        {
            if (_isSendingIssue)
                return;

            if (!ValidateInput())
                return;

            string title = this.TitleText.Text;
            string description = this.DescriptionText.Text;

            _isSendingIssue = true;
            ((MainActivity)this.Activity).BlockUI();

            _cts0 = new CancellationTokenSource();
            AppController.UpdateIssue(_cts0,
                _issue.IssueId,
                title,
                description,
                (todoItem) =>
                {
                    this.FragmentManager.PopBackStack();
                },
                (error) =>
                {
                    Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
                },
                () =>
                {
                    _isSendingIssue = false;
                    ((MainActivity)this.Activity).UnblockUI();
                });
        }

        #endregion

        #region Event Handlers

        private void DescriptionText_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
                this.DismissKeyboard();
        }

        private void TextInputFragment_TextInputDone(object sender, TextInputDoneEventArgs e)
        {
            if (_issue != null)
                _issue.Description = e.Text;
            else
                this.DescriptionText.Text = e.Text;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            var f = new TextInputFragment();
            f.ContentText = this.DescriptionText.Text;
            f.TextInputDone += TextInputFragment_TextInputDone;
            this.FragmentManager.BeginTransaction()
                .AddToBackStack("BeforeTextInputFragment")
                .Replace(Resource.Id.ContentLayout, f, "TextInputFragment")
                .Commit();
        }

        #endregion
    }
}
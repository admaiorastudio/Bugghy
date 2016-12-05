namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Foundation;
    using UIKit;

    using AdMaiora.AppKit.UI;

    using AdMaiora.Bugghy.Model;
    using AdMaiora.Bugghy.Api;
    using AppKit.UI.App;

    #pragma warning disable CS4014
    public partial class IssueViewController : AdMaiora.AppKit.UI.App.UISubViewController
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

        #region Constructors

        public IssueViewController()
            : base("IssueViewController", null)
        {
        }

        #endregion

        #region Properties
        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _gimmickId = this.Arguments.GetInt("GimmickId");
            _userId = AppController.Settings.LastLoginUserIdUsed;
            _issue = this.Arguments.GetObject<Issue>("Issue");
            _currentIssueType = _issue != null ? _issue.Type : IssueType.Crash;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff

            SlideUpToShowKeyboard();

            this.HasBarButtonItems = true;

            #endregion

            this.Title = _currentIssueType.ToString();

            this.TypeImage.UserInteractionEnabled = true;
            this.TypeImage.AddGestureRecognizer(new UITapGestureRecognizer(
                () =>
                {
                    _currentIssueType++;
                    if ((int)_currentIssueType == 4)
                        _currentIssueType = IssueType.Crash;

                    this.Title = _currentIssueType.ToString();
                    this.TapToChangeTypeLabel.Hidden = true;
                    SetIssueTypeImage(_currentIssueType);
                }));

            this.EditButton.TouchUpInside += EditButton_TouchUpInside;

            SetIssueTypeImage(_currentIssueType);

            if (_issue != null)
                LoadIssue();
        }

        public override bool CreateBarButtonItems(UIBarButtonCreator items)
        {
            base.CreateBarButtonItems(items);

            if (_issue == null || _issue.Status == IssueStatus.Opened)
                items.AddItem("Save", UIBarButtonItemStyle.Plain);

            return true;
        }

        public override bool BarButtonItemSelected(int index)
        {
            switch (index)
            {
                case 0:

                    if (_issue == null)
                        AddIssue();
                    else
                        UpdateIssue();

                    return true;

                default:
                    return base.BarButtonItemSelected(index);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (_cts0 != null)
                _cts0.Cancel();

            this.EditButton.TouchUpInside -= EditButton_TouchUpInside;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoadIssue()
        {
            this.TapToChangeTypeLabel.Hidden = true;

            this.CodeLabel.Text = _issue.Code;

            this.Title = _issue.Type.ToString();

            this.TitleText.Text = _issue.Title;
            this.TitleText.Enabled = _issue.Status == IssueStatus.Opened;

            this.DescriptionText.Text = _issue.Description;

            this.EditButton.Hidden =
                _issue == null || _issue.Status == IssueStatus.Opened ? false : true;

            SetIssueTypeImage(_issue.Type);
        }

        private void SetIssueTypeImage(IssueType type)
        {
            _currentIssueType = type;

            string[] typeImages = new[] { "image_gear", "image_issue_crash", "image_issue_blocking", "image_issue_nblocking" };
            this.TypeImage.Image = UIImage.FromBundle(typeImages[(int)_currentIssueType]);
        }

        private bool ValidateInput()
        {
            var validator = new WidgetValidator()
                .AddValidator(() => this.TitleText.Text, WidgetValidator.IsNotNullOrEmpty, "Please insert a title!")
                .AddValidator(() => this.DescriptionText.Text, WidgetValidator.IsNotNullOrEmpty, "Please insert a description!");

            string errorMessage;
            if (!validator.Validate(out errorMessage))
            {
                UIToast.MakeText(errorMessage, UIToastLength.Long).Show();

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
            ((MainViewController)this.MainViewController).BlockUI();

            _cts0 = new CancellationTokenSource();
            AppController.AddIssue(_cts0,
                _gimmickId,
                _userId,
                title,
                description,
                _currentIssueType,
                (todoItem) =>
                {
                    this.NavigationController.PopViewController(true);
                },
                (error) =>
                {
                    UIToast.MakeText(error, UIToastLength.Long).Show();
                },
                () =>
                {
                    _isSendingIssue = false;
                    ((MainViewController)this.MainViewController).UnblockUI();
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
            ((MainViewController)this.MainViewController).BlockUI();

            _cts0 = new CancellationTokenSource();
            AppController.UpdateIssue(_cts0,
                _issue.IssueId,
                title,
                description,
                (todoItem) =>
                {
                    this.NavigationController.PopViewController(true);
                },
                (error) =>
                {
                    UIToast.MakeText(error, UIToastLength.Long).Show();
                },
                () =>
                {
                    _isSendingIssue = false;
                    ((MainViewController)this.MainViewController).UnblockUI();
                });
        }

        #endregion

        #region Event Handlers

        private void EditButton_TouchUpInside(object sender, EventArgs e)
        {
            var c = new TextInputViewController();
            c.ContentText = this.DescriptionText.Text;
            c.TextInputDone += TextInputViewController_TextInputDone;
            this.NavigationController.PushViewController(c, true);
        }

        private void TextInputViewController_TextInputDone(object sender, TextInputDoneEventArgs e)
        {
            if (_issue != null)
                _issue.Description = e.Text;
            else
                this.DescriptionText.Text = e.Text;
        }

        #endregion
    }
}

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

    #pragma warning disable CS4014
    public class GimmickFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private Gimmick _gimmick;

        // This flag check if we are already calling the login REST service
        private bool _isRefreshingStats;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        #endregion

        #region Widgets

        [Widget]
        private RoundedImageView ThumbImage;

        [Widget]
        private TextView NameLabel;

        [Widget]
        private TextView OwnerLabel;

        [Widget]
        private ImageButton AddButton;

        [Widget]
        private RelativeLayout OpenedLayout;

        [Widget]
        private TextView OpenedNumberLabel;

        [Widget]
        private Button ViewOpenedButton;

        [Widget]
        private RelativeLayout WorkingLayout;

        [Widget]
        private TextView WorkingNumberLabel;

        [Widget]
        private Button ViewWorkingButton;

        [Widget]
        private RelativeLayout ClosedLayout;

        [Widget]
        private TextView ClosedNumberLabel;

        [Widget]
        private Button ViewClosedButton;

        #endregion

        #region Constructors

        public GimmickFragment()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Fragment Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _gimmick = this.Arguments.GetObject<Gimmick>("Gimmick");
        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentGimmick, inflater, container);

            this.HasOptionsMenu = true;
            
            #endregion

            this.Title = "Stats";

            this.AddButton.Click += AddButton_Click;

            this.ViewOpenedButton.Click += ViewIssuesButton_Click;
            this.ViewWorkingButton.Click += ViewIssuesButton_Click;
            this.ViewClosedButton.Click += ViewIssuesButton_Click;

            RefreshThumb();
            RefreshStats();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            menu.Clear();
            menu.Add(0, 1, 0, "Refresh").SetShowAsAction(ShowAsAction.Always);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 1:
                    RefreshStats();
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

            this.AddButton.Click -= AddButton_Click;

            this.ViewOpenedButton.Click -= ViewIssuesButton_Click;
            this.ViewWorkingButton.Click -= ViewIssuesButton_Click;
            this.ViewClosedButton.Click -= ViewIssuesButton_Click;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods
        private void RefreshThumb()
        {
            if (_gimmick.ImageUrl != null)
            {
                // This will load async the image and cache it locally

                var uri = new Uri(_gimmick.ImageUrl);
                AppController.Images.SetImageForView(
                    uri, "image_empty_thumb", this.ThumbImage);
            }
        }

        private void RefreshStats()
        {
            if (_gimmick == null)
                return;

            if (_isRefreshingStats)
                return;

            this.NameLabel.Text = _gimmick.Name;
            this.OwnerLabel.Text = _gimmick.Owner;

            _isRefreshingStats = true;
            ((MainActivity)this.Activity).BlockUI();

            this.ViewOpenedButton.Enabled = false;
            this.ViewWorkingButton.Enabled = false;
            this.ViewClosedButton.Enabled = false;

            Dictionary<string, int> stats = null;

            _cts0 = new CancellationTokenSource();
            AppController.RefreshStats(_cts0,
                _gimmick.GimmickId,
                (newStats) =>
                {
                    stats = newStats;
                },
                (error) =>
                {                    
                    Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
                },
                () =>
                {
                    if(stats != null)
                    {
                        LoadStatistics(stats);

                        _isRefreshingStats = false;
                        ((MainActivity)this.Activity).UnblockUI();
                    }
                    else
                    {
                        AppController.Utility.ExecuteOnAsyncTask(_cts0.Token,
                            () =>
                            {
                                stats = AppController.GetStats(_gimmick.GimmickId);
                            },
                            () =>
                            {
                                LoadStatistics(stats);

                                _isRefreshingStats = false;
                                ((MainActivity)this.Activity).UnblockUI();

                            });
                    }
                });
        }

        private void LoadStatistics(Dictionary<string, int> stats)
        {
            int opened = stats["Opened"];            
            this.OpenedNumberLabel.Text = opened.ToString();
            this.OpenedNumberLabel.SetTextColor(ViewBuilder.ColorFromARGB(opened > 0 ? AppController.Colors.AndroidGreen : AppController.Colors.DarkLiver));
            this.ViewOpenedButton.Enabled = opened > 0;            
            this.ViewOpenedButton.SetTextUnderline(opened > 0);            

            int working = stats["Working"];
            this.WorkingLayout.Clickable = opened > 0;
            this.WorkingNumberLabel.Text = working.ToString();
            this.WorkingNumberLabel.SetTextColor(ViewBuilder.ColorFromARGB(working > 0 ? AppController.Colors.AndroidGreen : AppController.Colors.DarkLiver));
            this.ViewWorkingButton.Enabled = working > 0;            
            this.ViewWorkingButton.SetTextUnderline(working > 0);

            int closed = stats["Closed"];
            this.ClosedLayout.Clickable = opened > 0;
            this.ClosedNumberLabel.Text = closed.ToString();
            this.ClosedNumberLabel.SetTextColor(ViewBuilder.ColorFromARGB(closed > 0 ? AppController.Colors.AndroidGreen : AppController.Colors.DarkLiver));
            this.ViewClosedButton.Enabled = closed > 0;            
            this.ViewClosedButton.SetTextUnderline(closed > 0);
        }

        private void GoToIssues(int filter, bool addNew = false)
        {
            var f = new IssuesFragment();
            f.Arguments = new Bundle();
            f.Arguments.PutInt("GimmickId", _gimmick.GimmickId);
            f.Arguments.PutInt("Filter", addNew ? 0 : filter);
            f.Arguments.PutBoolean("AddNew", addNew);
            this.FragmentManager.BeginTransaction()
                .AddToBackStack("BeforeIssuesFragment")
                .Replace(Resource.Id.ContentLayout, f, "IssuesFragment")
                .Commit();
        }

        #endregion

        #region Event Handlers

        private void AddButton_Click(object sender, EventArgs e)
        {
            GoToIssues(0, true);
        }

        private void ViewIssuesButton_Click(object sender, EventArgs e)
        {
            var buttons = new[] { this.ViewOpenedButton, this.ViewWorkingButton, this.ViewClosedButton };

            int index = Array.IndexOf(buttons, sender);
            GoToIssues(index);  
        }

        #endregion
    }
}
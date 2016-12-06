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
    using AppKit.UI.App;

    #pragma warning disable CS4014
    public partial class GimmickViewController : AdMaiora.AppKit.UI.App.UISubViewController
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

        #region Constructors

        public GimmickViewController()
            : base("GimmickViewController", null)
        {
        }

        #endregion

        #region Properties
        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _gimmick = this.Arguments.GetObject<Gimmick>("Gimmick");
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff

            this.HasBarButtonItems = true;

            #endregion

            this.Title = "Stats";

            this.AddButton.TouchUpInside += AddButton_TouchUpInside;

            this.ViewOpenedButton.TouchUpInside += ViewIssuesButton_TouchUpInside;
            this.ViewWorkingButton.TouchUpInside += ViewIssuesButton_TouchUpInside;
            this.ViewClosedButton.TouchUpInside += ViewIssuesButton_TouchUpInside;

            RefreshThumb();
            RefreshStats();
        }

        public override bool CreateBarButtonItems(UIBarButtonCreator items)
        {
            return base.CreateBarButtonItems(items);
        }

        public override bool BarButtonItemSelected(int index)
        {
            switch (index)
            {
                case 0:
                    RefreshStats();
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

            this.AddButton.TouchUpInside -= AddButton_TouchUpInside;

            this.ViewOpenedButton.TouchUpInside -= ViewIssuesButton_TouchUpInside;
            this.ViewWorkingButton.TouchUpInside -= ViewIssuesButton_TouchUpInside;
            this.ViewClosedButton.TouchUpInside -= ViewIssuesButton_TouchUpInside;
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
            ((MainViewController)this.MainViewController).BlockUI();

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
                    UIToast.MakeText(error, UIToastLength.Long).Show();
                },
                () =>
                {
                    if (stats != null)
                    {
                        LoadStatistics(stats);

                        _isRefreshingStats = false;
                        ((MainViewController)this.MainViewController).UnblockUI();
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
                                ((MainViewController)this.MainViewController).UnblockUI();

                            });
                    }
                });
        }

        private void LoadStatistics(Dictionary<string, int> stats)
        {
            int opened = stats["Opened"];
            this.OpenedNumberLabel.Text = opened.ToString();
            this.OpenedNumberLabel.TextColor = ViewBuilder.ColorFromARGB(opened > 0 ? AppController.Colors.AndroidGreen : AppController.Colors.DarkLiver);
            this.ViewOpenedButton.Enabled = opened > 0;
            this.ViewOpenedButton.SetTextUnderline(opened > 0);

            int working = stats["Working"];            
            this.WorkingNumberLabel.Text = working.ToString();
            this.WorkingNumberLabel.TextColor = ViewBuilder.ColorFromARGB(working > 0 ? AppController.Colors.AndroidGreen : AppController.Colors.DarkLiver);
            this.ViewWorkingButton.Enabled = working > 0;
            this.ViewWorkingButton.SetTextUnderline(working > 0);

            int closed = stats["Closed"];            
            this.ClosedNumberLabel.Text = closed.ToString();
            this.ClosedNumberLabel.TextColor = ViewBuilder.ColorFromARGB(closed > 0 ? AppController.Colors.AndroidGreen : AppController.Colors.DarkLiver);
            this.ViewClosedButton.Enabled = closed > 0;
            this.ViewClosedButton.SetTextUnderline(closed > 0);
        }

        private void GoToIssues(int filter, bool addNew = false)
        {
            var c = new IssuesViewController();
            c.Arguments = new UIBundle();
            c.Arguments.PutInt("GimmickId", _gimmick.GimmickId);
            c.Arguments.PutInt("Filter", addNew ? 0 : filter);
            c.Arguments.PutBoolean("AddNew", addNew);
            this.NavigationController.PushViewController(c, !addNew);
        }

        #endregion

        #region Event Handlers

        private void AddButton_TouchUpInside(object sender, EventArgs e)
        {
            GoToIssues(0, true);
        }

        private void ViewIssuesButton_TouchUpInside(object sender, EventArgs e)
        {
            var buttons = new[] { this.ViewOpenedButton, this.ViewWorkingButton, this.ViewClosedButton };

            int index = Array.IndexOf(buttons, sender);
            GoToIssues(index);
        }

        #endregion
    }
}

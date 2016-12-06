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

    using AdMaiora.Bugghy.Api;
    using AdMaiora.Bugghy.Model;
    using AppKit.UI.App;

#pragma warning disable CS4014
    public partial class IssuesViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        #region Inner Classes

        private class IssueViewSource : UIItemListViewSource<Issue>
        {
            #region Constants and Fields

            private string _currentUser;

            #endregion

            #region Constructors

            public IssueViewSource(UIViewController controller, IEnumerable<Issue> source)
                : base(controller, "IssueViewCell", source)
            {                
            }

            #endregion

            #region Public Methods

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath, UITableViewCell cellView, Issue item)
            {
                var cell = cellView as IssueViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;

                cell.CalloutLayout.Layer.BorderColor = ViewBuilder.ColorFromARGB(AppController.Colors.AndroidGreen).CGColor;

                string[] typeImages = new[] { "image_gear", "image_issue_crash", "image_issue_blocking", "image_issue_nblocking" };
                cell.TypeImage.Image = UIImage.FromBundle(typeImages[(int)item.Type]);

                cell.CodeLabel.Text = String.Format("code: #{0}", item.Code);
                cell.TitleLabel.Text = item.Title;
                cell.SenderLabel.Text = item.Sender.Split('@')[0];
                cell.DescriptionLabel.Text = item.Description;
                cell.CreatedDateLabel.Text = item.CreationDate?.ToString("d");
                cell.StatusDescriptionLabel.Text = item.Status.ToString();

                return cell;
            }

            public void Clear()
            {
                this.SourceItems.Clear();
            }

            public void Refresh(IEnumerable<Issue> items)
            {
                this.SourceItems.Clear();
                this.SourceItems.AddRange(items);
            }

            #endregion
        }

        #endregion

        #region Constants and Fields

        private int _gimmickId;
        private int _filter;
        private bool _addNew;

        private IssueViewSource _source;

        // This flag check if we are already calling the login REST service
        private bool _isRefreshingIssues;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        #endregion

        #region Widgets

        private UIButton[] FilterButtons;

        #endregion

        #region Constructors

        public IssuesViewController()
            : base("IssuesViewController", null)
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
            _filter = this.Arguments.GetInt("Filter");
            _addNew = this.Arguments.GetBoolean("AddNew");
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff

            this.FilterButtons = new[] { this.FilterOpenedButton, this.FilterWorkingButton, this.FilterClosedButton };

            this.HasBarButtonItems = true;

            #endregion

            this.Title = "Issues";

            this.FilterOpenedButton.TouchUpInside += FilterOpenedButton_TouchUpInside;
            this.FilterWorkingButton.TouchUpInside += FilterOpenedButton_TouchUpInside;
            this.FilterClosedButton.TouchUpInside += FilterOpenedButton_TouchUpInside;

            this.IssueList.RowHeight = UITableView.AutomaticDimension;
            this.IssueList.EstimatedRowHeight = 130;
            this.IssueList.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.IssueList.BackgroundColor = ViewBuilder.ColorFromARGB(AppController.Colors.DarkLiver);
            this.IssueList.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
            this.IssueList.ItemSelected += IssueList_ItemSelected;

            if (_addNew)
            {
                _addNew = false;

                var c = new IssueViewController();
                c.Arguments = new UIBundle();
                c.Arguments.PutInt("GimmickId", _gimmickId);
                this.NavigationController.PushViewController(c, true);
            }
            else
            {
                RefreshIssues(_filter);
            }
        }

        public override bool CreateBarButtonItems(UIBarButtonCreator items)
        {
            base.CreateBarButtonItems(items);

            items.AddItem("Add New", UIBarButtonItemStyle.Plain);
            return true;
        }

        public override bool BarButtonItemSelected(int index)
        {
            switch (index)
            {
                case 0:

                    var c = new IssueViewController();
                    c.Arguments = new UIBundle();
                    c.Arguments.PutInt("GimmickId", _gimmickId);
                    this.NavigationController.PushViewController(c, true);

                    return true;

                default:
                    return base.BarButtonItemSelected(index);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            this.FilterOpenedButton.TouchUpInside -= FilterOpenedButton_TouchUpInside;
            this.FilterWorkingButton.TouchUpInside -= FilterOpenedButton_TouchUpInside;
            this.FilterClosedButton.TouchUpInside -= FilterOpenedButton_TouchUpInside;

            this.IssueList.ItemSelected -= IssueList_ItemSelected;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void RefreshIssues(int filter)
        {
            if (_isRefreshingIssues)
                return;

            _filter = filter;

            HilightFilterButton(filter);

            this.IssueList.Hidden = true;

            _isRefreshingIssues = true;
            ((MainViewController)this.MainViewController).BlockUI();

            Issue[] issues = null;

            _cts0 = new CancellationTokenSource();
            AppController.RefreshIssues(_cts0,
                _gimmickId,
                (newIssues) =>
                {
                    issues = newIssues;
                },
                (error) =>
                {
                    UIToast.MakeText(error, UIToastLength.Long).Show();
                },
                () =>
                {
                    if (issues != null)
                    {
                        LoadIssues(issues, filter);

                        if (_source?.Count > 0)
                            this.IssueList.Hidden = false;

                        _isRefreshingIssues = false;
                        ((MainViewController)this.MainViewController).UnblockUI();
                    }
                    else
                    {
                        AppController.Utility.ExecuteOnAsyncTask(_cts0.Token,
                            () =>
                            {
                                issues = AppController.GetIssues(_gimmickId);
                            },
                            () =>
                            {
                                LoadIssues(issues, filter);

                                if (_source?.Count > 0)
                                    this.IssueList.Hidden = false;

                                _isRefreshingIssues = false;
                                ((MainViewController)this.MainViewController).UnblockUI();

                            });
                    }
                });
        }

        private void LoadIssues(IEnumerable<Issue> issues, int filter = -1)
        {
            if (issues == null)
                return;

            if (filter != -1)
            {
                // Filter by tab selection
                switch (filter)
                {
                    case 0:

                        issues = issues.Where(x =>
                              x.Status == IssueStatus.Opened);
                        break;

                    case 1:

                        issues = issues.Where(x =>
                               x.Status == IssueStatus.Evaluating
                            || x.Status == IssueStatus.Working);
                        break;

                    case 2:

                        issues = issues.Where(x =>
                               x.Status == IssueStatus.Resolved
                            || x.Status == IssueStatus.Rejected
                            || x.Status == IssueStatus.Closed);
                        break;
                }
            }

            // Sort desc by creation date
            issues = issues.OrderByDescending(x => x.CreationDate);

            if (_source == null)
            {
                _source = new IssueViewSource(this, issues);
                this.IssueList.Source = _source;
            }
            else
            {
                _source.Refresh(issues);
                this.IssueList.ReloadData();
            }
        }

        private void HilightFilterButton(int filter)
        {
            for (int i = 0; i < this.FilterButtons.Length; i++)
            {
                UIButton b = this.FilterButtons[i];
                b.SetTitleColor(ViewBuilder.ColorFromARGB(i == filter ? AppController.Colors.Jet : AppController.Colors.White), UIControlState.Normal);
                b.BackgroundColor = ViewBuilder.ColorFromARGB(i == filter ? AppController.Colors.White : AppController.Colors.Jet);
            }
        }

        #endregion

        #region Event Handlers

        private void FilterOpenedButton_TouchUpInside(object sender, EventArgs e)
        {
            int filter = Array.IndexOf(this.FilterButtons, sender);
            if (filter == _filter)
                return;

            RefreshIssues(filter);
        }

        private void IssueList_ItemSelected(object sender, UIItemListSelectEventArgs e)
        {
            Issue issue = e.Item as Issue;

            var c = new ChatViewController();
            c.Arguments = new UIBundle();
            c.Arguments.PutInt("GimmickId", _gimmickId);
            c.Arguments.PutObject<Issue>("Issue", issue);
            this.NavigationController.PushViewController(c, true);           
        }

        #endregion
    }
}

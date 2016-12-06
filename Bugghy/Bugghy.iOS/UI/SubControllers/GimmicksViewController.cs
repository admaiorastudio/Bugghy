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

    #pragma warning disable CS4014
    public partial class GimmicksViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        #region Inner Classes

        private class GimmickViewSource : UIItemListViewSource<Gimmick>
        {
            #region Constants and Fields

            private string _currentUser;

            #endregion

            #region Constructors

            public GimmickViewSource(UIViewController controller, IEnumerable<Gimmick> source)
                : base(controller, "GimmickViewCell", source)
            {
                _currentUser = AppController.Settings.LastLoginUsernameUsed;
            }

            #endregion

            #region Public Methods

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath, UITableViewCell cellView, Gimmick item)
            {
                var cell = cellView as GimmickViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;

                cell.CalloutLayout.Layer.BorderColor = ViewBuilder.ColorFromARGB(AppController.Colors.AndroidGreen).CGColor;
                cell.ContentLayout.BackgroundColor = UIColor.White;

                AppController.Images.SetImageForView(
                    new Uri(item.ImageUrl), "image_gear", cell.ThumbImage);

                cell.NameLabel.Text = item.Name;
                cell.OwnerLabel.Text = item.Owner;

                return cell;
            }

            public void Clear()
            {
                this.SourceItems.Clear();
            }

            public void Refresh(IEnumerable<Gimmick> items)
            {
                this.SourceItems.Clear();
                this.SourceItems.AddRange(items);
            }

            #endregion
        }

        #endregion

        #region Constants and Fields

        private GimmickViewSource _source;

        // This flag check if we are already calling the login REST service
        private bool _isRefreshingGimmicks;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        #endregion

        #region Constructors

        public GimmicksViewController()
            : base("GimmicksViewController", null)
        {
        }

        #endregion

        #region Properties
        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff

            this.HasBarButtonItems = true;

            #endregion

            this.Title = "All Gimmicks";

            this.NavigationController.SetNavigationBarHidden(false, true);

            this.GimmickList.RowHeight = UITableView.AutomaticDimension;
            this.GimmickList.EstimatedRowHeight = 88;
            this.GimmickList.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.GimmickList.BackgroundColor = ViewBuilder.ColorFromARGB(AppController.Colors.DarkLiver);
            this.GimmickList.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
            this.GimmickList.ItemSelected += GimmickList_ItemSelected;

            RefreshGimmicks();
        }

        public override bool CreateBarButtonItems(UIBarButtonCreator items)
        {
            return base.CreateBarButtonItems(items);
        }

        public override bool BarButtonItemSelected(int index)
        {
            switch(index)
            {
                case AppKit.UI.App.UISubViewController.BarButtonBack:
                    Logout();
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

            this.GimmickList.ItemSelected -= GimmickList_ItemSelected;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void RefreshGimmicks()
        {
            if (_isRefreshingGimmicks)
                return;

            this.GimmickList.Hidden = true;

            _isRefreshingGimmicks = true;
            ((MainViewController)this.MainViewController).BlockUI();

            Gimmick[] gimmicks = null;

            _cts0 = new CancellationTokenSource();
            AppController.RefreshGimmicks(_cts0,
                (newGimmicks) =>
                {
                    gimmicks = newGimmicks;
                },
                (error) =>
                {
                    UIToast.MakeText(error, UIToastLength.Long).Show();
                },
                () =>
                {
                    if (gimmicks != null)
                    {
                        LoadGimmicks(gimmicks);

                        if (_source?.Count > 0)
                            this.GimmickList.Hidden = false;

                        _isRefreshingGimmicks = false;
                        ((MainViewController)this.MainViewController).UnblockUI();
                    }
                    else
                    {
                        AppController.Utility.ExecuteOnAsyncTask(_cts0.Token,
                            () =>
                            {
                                gimmicks = AppController.GetGimmicks();
                            },
                            () =>
                            {
                                LoadGimmicks(gimmicks);

                                if (_source?.Count > 0)
                                    this.GimmickList.Hidden = false;

                                _isRefreshingGimmicks = false;
                                ((MainViewController)this.MainViewController).UnblockUI();

                            });
                    }
                });
        }

        private void LoadGimmicks(IEnumerable<Gimmick> gimmicks)
        {
            if (gimmicks == null)
                return;

            gimmicks = gimmicks
                .OrderBy(x => x.Name)
                .ToArray();

            if (_source == null)
            {
                _source = new GimmickViewSource(this, gimmicks);
                this.GimmickList.Source = _source;
            }
            else
            {
                _source.Refresh(gimmicks);
                this.GimmickList.ReloadData();
            }
        }

        private void Logout()
        {
            (new UIAlertViewBuilder(new UIAlertView()))
                .SetTitle("Do you want to logout now?")
                .SetMessage("")
                .AddButton("Ok",
                    (s, ea) =>
                    {
                        AppController.Settings.AuthAccessToken = null;
                        AppController.Settings.AuthExpirationDate = null;

                        this.DismissKeyboard();
                        this.NavigationController.PopViewController(true);
                    })
                .AddButton("Not now",
                    (s, ea) =>
                    {
                    })
                .Show();
        }

        #endregion

        #region Event Handlers

        private void GimmickList_ItemSelected(object sender, UIItemListSelectEventArgs e)
        {
            Gimmick gimmick = e.Item as Gimmick;

            var c = new GimmickViewController();
            c.Arguments = new UIBundle();
            c.Arguments.PutObject<Gimmick>("Gimmick", gimmick);
            this.NavigationController.PushViewController(c, true);
        }

        #endregion
    }
}

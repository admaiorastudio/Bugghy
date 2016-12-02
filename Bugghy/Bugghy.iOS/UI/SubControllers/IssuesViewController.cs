namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Foundation;
    using UIKit;

    using AdMaiora.AppKit.UI;

    using AdMaiora.Bugghy.Api;
    using AdMaiora.Bugghy.Model;

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

                string[] typeImages = new[] { "image_gear", "image_issue_crash", "image_issue_blocking", "image_issue_nblocking" };
                holder.TypeImage.SetImageResource(typeImages[(int)item.Type]);

                cell.CodeLabel.Text = String.Format("code: #{0}", item.Code);
                cell.TitleLabel.Text = item.Title;
                cell.SenderLabel.Text = item.Sender.Split('@')[0];
                cell.DescriptionLabel.Text = item.Description;
                cell.CreatedDateLabel.Text = item.CreationDate?.ToString("g");
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
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff
            #endregion
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods
        #endregion

        #region Event Handlers
        #endregion
    }
}

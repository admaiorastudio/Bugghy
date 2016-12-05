namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Foundation;
    using UIKit;
    using AudioToolbox;
    using AVFoundation;

    using AdMaiora.AppKit.UI;
    using AdMaiora.AppKit.UI.App;

    using AdMaiora.Bugghy.Api;
    using AdMaiora.Bugghy.Model;    

    #pragma warning disable CS4014
    public partial class ChatViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        #region Inner Classes

        private class ChatViewSource : UIItemListViewSource<Model.Message>
        {
            #region Constants and Fields

            private string _currentUser;

            #endregion

            #region Constructors

            public ChatViewSource(UIViewController controller, IEnumerable<Model.Message> source)
                : base(controller, "ChatViewCell", source)
            {
                _currentUser = AppController.Settings.LastLoginUsernameUsed;
            }

            #endregion

            #region Public Methods

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath, UITableViewCell cellView, Model.Message item)
            {
                var cell = cellView as ChatViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;

                bool isYours = _currentUser == item.Sender;
                bool isSending = item.PostDate == null;
                bool isSent = item.PostDate.GetValueOrDefault() != DateTime.MinValue;

                var margins = cell.ContentView.LayoutMarginsGuide;
                cell.CalloutLayout.LeadingAnchor.ConstraintEqualTo(margins.LeadingAnchor, 0).Active = !isYours;
                cell.CalloutLayout.TrailingAnchor.ConstraintEqualTo(margins.TrailingAnchor, 0).Active = isYours;                
                cell.CalloutLayout.BackgroundColor = 
                    ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.PapayaWhip : AppController.Colors.AndroidGreen);
                cell.CalloutLayout.Alpha = isSent ? 1 : .35f;

                cell.SenderLabel.Text = String.Concat(isYours ? "YOU" : item.Sender.Split('@')[0], "   ");                
                cell.SenderLabel.TextColor = ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.Jet : AppController.Colors.White);

                cell.MessageLabel.Text = String.Concat(item.Content, "   ");
                cell.MessageLabel.TextColor = ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.Jet : AppController.Colors.White);

                cell.DateLabel.Text = isSent ? String.Format("  sent @ {0:G}", item.PostDate) : String.Empty;
                cell.DateLabel.TextColor = ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.Jet : AppController.Colors.White);

                return cell;
            }

            public void Insert(Model.Message message)
            {
                this.SourceItems.Add(message);
                this.SourceItems = this.SourceItems.OrderBy(x => x.PostDate).ToList();
            }

            public void Refresh(IEnumerable<Model.Message> items)
            {
                this.SourceItems.Clear();
                this.SourceItems.AddRange(items);
            }

            #endregion
        }

        #endregion

        #region Constants and Fields

        private const string ReceiverLock = "ReceiverLock";

        private int _gimmickId;
        private int _userId;

        private Issue _issue;

        private ChatViewSource _source;

        // This flag check if we are already calling the send message REST service
        private bool _isSendingMessage;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        // This flag check if we are already calling the refersh message REST service
        private bool _isRefreshingMessage;
        // This cancellation token is used to cancel the rest refresh messages request
        private CancellationTokenSource _cts1;
        
        #endregion

        #region Constructors

        public ChatViewController()
            : base("ChatViewController", null)
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

            _source = new ChatViewSource(this, new Model.Message[0]);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff

            ResizeToShowKeyboard();

            this.HasBarButtonItems = true;

            #endregion
            
            this.Title = "Chat";

            this.NavigationController.SetNavigationBarHidden(false, true);

            this.HeaderLayout.UserInteractionEnabled = true;
            this.HeaderLayout.AddGestureRecognizer(new UITapGestureRecognizer(
                () =>
                {
                    var c = new IssuesViewController();
                    c.Arguments = new UIBundle();
                    c.Arguments.PutInt("GimmickId", _gimmickId);
                    c.Arguments.PutObject<Issue>("Issue", _issue);
                    this.NavigationController.PushViewController(c, true);
                }));

            this.MessageList.Source = _source;
            this.MessageList.RowHeight = UITableView.AutomaticDimension;
            this.MessageList.EstimatedRowHeight = 74;            
            this.MessageList.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.MessageList.BackgroundColor = ViewBuilder.ColorFromARGB(AppController.Colors.Jet);
            this.MessageList.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            this.SendButton.TouchUpInside += SendButton_TouchUpInside;

            this.MessageText.Constraints.Single(x => x.GetIdentifier() == "Height").Constant = 30f;
            this.MessageText.Changed += MessageText_Changed;

            if (_issue != null)
                LoadIssue();

            RefreshMessages();
        }

        public override bool CreateBarButtonItems(UIBarButtonCreator items)
        {
            base.CreateBarButtonItems(items);

            items.AddItem("Refresh", UIBarButtonItemStyle.Plain);
            return true;
        }

        public override bool BarButtonItemSelected(int index)
        {
            switch(index)
            {
                case 0:
                    RefreshMessages();                    
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

            if (_cts1 != null)
                _cts1.Cancel();
            this.SendButton.TouchUpInside -= SendButton_TouchUpInside;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoadIssue()
        {
            this.TitleLabel.Text = _issue.Title;

            DateTime? statusDate = null;
            switch (_issue.Status)
            {
                case IssueStatus.Opened:
                    statusDate = _issue.CreationDate;
                    break;

                case IssueStatus.Evaluating:
                case IssueStatus.Working:
                    statusDate = _issue.ReplyDate;
                    break;

                case IssueStatus.Resolved:
                case IssueStatus.Rejected:
                case IssueStatus.Closed:
                    statusDate = _issue.ClosedDate;
                    break;
            }

            this.StatusLabel.Text = String.Format("{0} @ {1:G}",
                _issue.Status.ToString(),
                statusDate.GetValueOrDefault());
        }

        private void PostMessage()
        {
            if (_isSendingMessage)
                return;

            string content = this.MessageText.Text;
            if (String.IsNullOrWhiteSpace(content))
                return;

            _isSendingMessage = true;
            ((MainViewController)this.MainViewController).BlockUI();

            _cts0 = new CancellationTokenSource();
            AppController.PostMessage(_cts0,
                _issue.IssueId,
                _userId,
                content,
                (message) =>
                {
                    if (_source != null)
                    {
                        _source.Insert(message);
                        this.MessageList.ReloadData();
                        this.MessageList.Hidden = false;
                        this.MessageText.Text = String.Empty;
                    }
                },
                (error) =>
                {
                    UIToast.MakeText(error, UIToastLength.Long).Show();
                },
                () =>
                {
                    _isSendingMessage = false;
                    ((MainViewController)this.MainViewController).UnblockUI();
                });
        }

        private void RefreshMessages()
        {
            if (_isRefreshingMessage)
                return;

            this.MessageList.Hidden = true;

            _isRefreshingMessage = true;
            ((MainViewController)this.MainViewController).BlockUI();

            Model.Message[] messages = null;

            _cts1 = new CancellationTokenSource();
            AppController.RefreshMessages(_cts1,
                _issue.IssueId,
                (newMessages) =>
                {
                    messages = newMessages;
                },
                (error) =>
                {
                    UIToast.MakeText(error, UIToastLength.Long).Show();
                },
                () =>
                {
                    if (messages != null)
                    {
                        LoadMessages(messages);

                        if (_source?.Count > 0)
                            this.MessageList.Hidden = false;

                        _isRefreshingMessage = false;
                        ((MainViewController)this.MainViewController).UnblockUI();
                    }
                    else
                    {
                        AppController.Utility.ExecuteOnAsyncTask(_cts1.Token,
                            () =>
                            {
                                messages = AppController.GetMessages(_issue.IssueId);
                            },
                            () =>
                            {
                                LoadMessages(messages);

                                if (_source?.Count > 0)
                                    this.MessageList.Hidden = false;

                                _isRefreshingMessage = false;
                                ((MainViewController)this.MainViewController).UnblockUI();

                            });
                    }
                });
        }

        private void LoadMessages(IEnumerable<Model.Message> messages)
        {
            if (messages == null)
                return;

            // Sort desc by creation date
            messages = messages.OrderBy(x => x.PostDate);

            if (_source == null)
            {
                _source = new ChatViewSource(this, messages);
                this.MessageList.Source = _source;
            }
            else
            {
                _source.Refresh(messages);
                this.MessageList.ReloadData();
            }
        }

        private void SetIssueTypeImage(IssueType type)
        {
            string[] typeImages = new[] { "image_gear", "image_issue_crash", "image_issue_blocking", "image_issue_nblocking" };
            this.TypeImage.Image = UIImage.FromBundle(typeImages[(int)type]);
        }

        private void AdjustMessageTextHeight()
        {
            UITextView t = this.MessageText;

            nfloat textWidth = t.TextContainerInset.InsetRect(t.Frame).Width;
            textWidth -= 2.0f * t.TextContainer.LineFragmentPadding;

            var size = (new NSString(t.Text)).GetBoundingRect(
                (new CoreGraphics.CGSize(textWidth, Double.MaxValue)),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes() { Font = t.Font },
                null).Size;

            int numberOfLines = (int)Math.Round(size.Height / t.Font.LineHeight);

            if (numberOfLines > 0 && numberOfLines < 4)
                this.MessageText.Constraints.Single(x => x.GetIdentifier() == "Height").Constant = 30f + (numberOfLines - 1) * t.Font.LineHeight;

            if (numberOfLines == 2)
            {
                this.MessageText.SetContentOffset(CoreGraphics.CGPoint.Empty, true);
                this.MessageText.SetNeedsDisplay();
            }

        }

        #endregion

        #region Event Handlers

        private void MessageText_Changed(object sender, EventArgs e)
        {
            AdjustMessageTextHeight();
        }

        private void SendButton_TouchUpInside(object sender, EventArgs e)
        {
            DismissKeyboard();

            PostMessage();
        }

        #endregion
    }
}



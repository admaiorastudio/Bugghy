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
    using Android.Graphics;
    using Android.Media;

    using AdMaiora.AppKit.UI;
    using AdMaiora.AppKit.UI.App;

    using AdMaiora.Bugghy.Api;
    using AdMaiora.Bugghy.Model;

    #pragma warning disable CS4014
    public class ChatFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes

        class ChatAdapter : ItemRecyclerAdapter<ChatAdapter.ChatViewHolder, Model.Message>
        {
            #region Inner Classes

            public class ChatViewHolder : ItemViewHolder
            {
                [Widget]
                public RelativeLayout CalloutLayout;

                [Widget]
                public TextView SenderLabel;

                [Widget]
                public TextView MessageLabel;

                [Widget]
                public TextView DateLabel;

                public ChatViewHolder(View itemView)
                    : base(itemView)
                {                    
                }
            }

            #endregion

            #region Costants and Fields

            private string _currentUser;

            #endregion

            #region Constructors

            public ChatAdapter(AdMaiora.AppKit.UI.App.Fragment context, IEnumerable<Model.Message> source) 
                : base(context, Resource.Layout.CellChat, source)
            {
                _currentUser = AppController.Settings.LastLoginUsernameUsed;
            }

            #endregion

            #region Public Methods

            public override void GetView(int postion, ChatViewHolder holder, View view, Model.Message item)
            {
                bool isYours = _currentUser == item.Sender;
                bool isSending = item.PostDate == null;
                bool isSent = item.PostDate.GetValueOrDefault() != DateTime.MinValue;

                ((RelativeLayout)view).SetGravity(isYours ? GravityFlags.Right : GravityFlags.Left);
                holder.CalloutLayout.Background.SetColorFilter(
                    ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.PapayaWhip : AppController.Colors.AndroidGreen),
                    PorterDuff.Mode.SrcIn);
                holder.CalloutLayout.Alpha = isSent ? 1 : .35f;

                holder.SenderLabel.Text = String.Concat(isYours ? "YOU" : item.Sender.Split('@')[0], "   ");
                holder.SenderLabel.SetTextColor(ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.Jet : AppController.Colors.White));                

                holder.MessageLabel.Text = String.Concat(item.Content, "   ");
                holder.MessageLabel.SetTextColor(ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.Jet : AppController.Colors.White));

                holder.DateLabel.Text = isSent ? String.Format("  sent @ {0:G}", item.PostDate) : String.Empty;
                holder.DateLabel.SetTextColor(ViewBuilder.ColorFromARGB(isYours ? AppController.Colors.Jet : AppController.Colors.White));
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

        private int _gimmickId;        
        private int _userId;

        private Issue _issue;

        private ChatAdapter _adapter;

        // This flag check if we are already calling the send message REST service
        private bool _isSendingMessage;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        // This flag check if we are already calling the refersh message REST service
        private bool _isRefreshingMessage;
        // This cancellation token is used to cancel the rest refresh messages request
        private CancellationTokenSource _cts1;

        #endregion

        #region Widgets

        [Widget]
        private RelativeLayout HeaderLayout;

        [Widget]
        private ImageView TypeImage;

        [Widget]
        private TextView TitleLabel;

        [Widget]
        private TextView StatusLabel;

        [Widget]
        private ItemRecyclerView MessageList;

        [Widget]
        private RelativeLayout InputLayout;

        [Widget]
        private EditText MessageText;

        [Widget]
        private ImageButton SendButton;

        #endregion

        #region Constructors

        public ChatFragment()
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
            _userId = AppController.Settings.LastLoginUserIdUsed;
            _issue = this.Arguments.GetObject<Issue>("Issue");

            _adapter = new ChatAdapter(this, new Model.Message[0]);            
        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentChat, inflater, container);
            
            ResizeToShowKeyboard();

            this.HasOptionsMenu = true;

            #endregion

            this.Title = "Chat";

            this.ActionBar.Show();

            this.HeaderLayout.Clickable = true;
            this.HeaderLayout.SetOnTouchListener(GestureListener.ForSingleTapUp(this.Activity,
                (e) =>
                {
                    var f = new IssueFragment();
                    f.Arguments = new Bundle();
                    f.Arguments.PutInt("GimmickId", _gimmickId);
                    f.Arguments.PutObject<Issue>("Issue", _issue);
                    this.FragmentManager.BeginTransaction()
                        .AddToBackStack("BeforeIssueFragment")
                        .Replace(Resource.Id.ContentLayout, f, "IssueFragment")
                        .Commit();
                }));

            this.MessageList.SetAdapter(_adapter);

            this.SendButton.Click += SendButton_Click;

            if(_issue != null)
                LoadIssue();

            RefreshMessages();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            menu.Clear();
            menu.Add(0, 1, 0, "Refresh").SetShowAsAction(ShowAsAction.Always);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case 1:
                    RefreshMessages();
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

            if (_cts1 != null)
                _cts1.Cancel();

            this.SendButton.Click -= SendButton_Click;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods        

        private void LoadIssue()
        {
            this.TitleLabel.Text = _issue.Title;

            DateTime? statusDate = null;
            switch(_issue.Status)
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
            ((MainActivity)this.Activity).BlockUI();

            _cts0 = new CancellationTokenSource();
            AppController.PostMessage(_cts0,
                _issue.IssueId,
                _userId,
                content,
                (message) =>
                {
                    if (_adapter != null)
                    {
                        _adapter.Insert(message);
                        this.MessageList.ReloadData();
                        this.MessageList.Visibility = ViewStates.Visible;
                        this.MessageText.Text = String.Empty;
                    }
                },
                (error) =>
                {
                    Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
                },
                () =>
                {
                    _isSendingMessage = false;
                    ((MainActivity)this.Activity).UnblockUI();
                });
        }        

        private void RefreshMessages()
        {
            if (_isRefreshingMessage)
                return;

            this.MessageList.Visibility = ViewStates.Gone;

            _isRefreshingMessage = true;
            ((MainActivity)this.Activity).BlockUI();

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
                    Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
                },
                () =>
                {
                    if (messages != null)
                    {
                        LoadMessages(messages);

                        if (_adapter?.ItemCount > 0)
                            this.MessageList.Visibility = ViewStates.Visible;

                        _isRefreshingMessage = false;
                        ((MainActivity)this.Activity).UnblockUI();
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

                                if (_adapter?.ItemCount > 0)
                                    this.MessageList.Visibility = ViewStates.Visible;

                                _isRefreshingMessage = false;
                                ((MainActivity)this.Activity).UnblockUI();

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

            if (_adapter == null)
            {
                _adapter = new ChatAdapter(this, messages);
                this.MessageList.SetAdapter(_adapter);
            }
            else
            {
                _adapter.Refresh(messages);
                this.MessageList.ReloadData();
            }
        }

        private void SetIssueTypeImage(IssueType type)
        {            
            string[] typeImages = new[] { "image_gear", "image_issue_crash", "image_issue_blocking", "image_issue_nblocking" };
            this.TypeImage.SetImageResource(typeImages[(int)type]);
        }   

        #endregion

        #region Event Handlers

        private void SendButton_Click(object sender, EventArgs e)
        {
            DismissKeyboard();            

            PostMessage();
        }

        #endregion
    }
}
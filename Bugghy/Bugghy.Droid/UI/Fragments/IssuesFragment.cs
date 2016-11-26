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

    using AdMaiora.Bugghy.Api;
    using AdMaiora.Bugghy.Model;

    #pragma warning disable CS4014
    public class IssuesFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes

        class IssueAdapter : ItemRecyclerAdapter<IssueAdapter.ChatViewHolder, Issue>
        {
            #region Inner Classes

            public class ChatViewHolder : ItemViewHolder
            {
                [Widget]
                public ImageView TypeImage;

                [Widget]
                public TextView CodeLabel;

                [Widget]
                public TextView TitleLabel;

                [Widget]
                public TextView SenderLabel;

                [Widget]
                public TextView DescriptionLabel;

                [Widget]
                public TextView CreatedDateLabel;

                [Widget]
                public TextView StatusDescriptionLabel;

                public ChatViewHolder(View itemView)
                    : base(itemView)
                {
                }
            }

            #endregion

            #region Costants and Fields
            #endregion

            #region Constructors

            public IssueAdapter(AdMaiora.AppKit.UI.App.Fragment context, IEnumerable<Issue> source)
                : base(context, Resource.Layout.CellIssue, source)
            {
            }

            #endregion

            #region Public Methods

            public override void GetView(int postion, ChatViewHolder holder, View view, Issue item)
            {
                string[] typeImages = new[] { "image_gear", "image_issue_crash", "image_issue_blocking", "image_issue_nblocking" };
                holder.TypeImage.SetImageResource(typeImages[(int)item.Type]);

                holder.CodeLabel.Text = String.Format("code: #{0}", item.Code);
                holder.TitleLabel.Text = item.Title;
                holder.SenderLabel.Text = item.Sender.Split('@')[0];
                holder.DescriptionLabel.Text = item.Description;
                holder.CreatedDateLabel.Text = item.CreationDate?.ToString("g");
                holder.StatusDescriptionLabel.Text = item.Status.ToString();
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

            #region Methods
            #endregion
        }

        #endregion

        #region Constants and Fields

        private int _gimmickId;
        private int _filter;
        private bool _addNew;    

        private IssueAdapter _adapter;

        // This flag check if we are already calling the login REST service
        private bool _isRefreshingIssues;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        #endregion

        #region Widgets

        private Button[] FilterButtons;

        [Widget]
        private Button FilterOpenedButton;

        [Widget]
        private Button FilterWorkingButton;

        [Widget]
        private Button FilterClosedButton;        

        [Widget]
        private ItemRecyclerView IssueList;

        #endregion

        #region Constructors

        public IssuesFragment()
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
            _filter = this.Arguments.GetInt("Filter");
            _addNew = this.Arguments.GetBoolean("AddNew");     
        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentIssues, inflater, container);

            this.FilterButtons = new[] { this.FilterOpenedButton, this.FilterWorkingButton, this.FilterClosedButton };

            this.HasOptionsMenu = true;

            #endregion

            this.Title = "Issues";

            this.FilterOpenedButton.Click += FilterButton_Click;
            this.FilterWorkingButton.Click += FilterButton_Click;
            this.FilterClosedButton.Click += FilterButton_Click;

            this.IssueList.ItemSelected += IssueList_ItemSelected;

            if (_addNew)
            {
                _addNew = false;

                var f = new IssueFragment();
                f.Arguments = new Bundle();
                f.Arguments.PutInt("GimmickId", _gimmickId);
                this.FragmentManager.BeginTransaction()
                    .AddToBackStack("BeforeIssueFragment")
                    .Replace(Resource.Id.ContentLayout, f, "IssueFragment")
                    .Commit();
            }
            else
            {
                RefreshIssues(_filter);
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            menu.Clear();
            menu.Add(0, 1, 0, "Add New").SetShowAsAction(ShowAsAction.Always);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case 1:

                    var f = new IssueFragment();
                    f.Arguments = new Bundle();
                    f.Arguments.PutInt("GimmickId", _gimmickId);                    
                    this.FragmentManager.BeginTransaction()
                        .AddToBackStack("BeforeIssueFragment")
                        .Replace(Resource.Id.ContentLayout, f, "IssueFragment")
                        .Commit();

                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            this.FilterOpenedButton.Click -= FilterButton_Click;
            this.FilterWorkingButton.Click -= FilterButton_Click;
            this.FilterClosedButton.Click -= FilterButton_Click;

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

            this.IssueList.Visibility = ViewStates.Gone;

            _isRefreshingIssues = true;
            ((MainActivity)this.Activity).BlockUI();

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
                    Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
                },
                () =>
                {
                    if (issues != null)
                    {
                        LoadIssues(issues, filter);

                        if(_adapter?.ItemCount > 0)
                            this.IssueList.Visibility = ViewStates.Visible;

                        _isRefreshingIssues = false;
                        ((MainActivity)this.Activity).UnblockUI();
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

                                if (_adapter?.ItemCount > 0)
                                    this.IssueList.Visibility = ViewStates.Visible;

                                _isRefreshingIssues = false;
                                ((MainActivity)this.Activity).UnblockUI();

                            });
                    }
                });
        }

        private void LoadIssues(IEnumerable<Issue> issues, int filter = -1)
        {
            if (issues == null)
                return;

            if(filter != -1)
            {
                // Filter by tab selection
                switch(filter)
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

            if (_adapter == null)
            {
                _adapter = new IssueAdapter(this, issues);
                this.IssueList.SetAdapter(_adapter);
            }
            else
            {
                _adapter.Refresh(issues);
                this.IssueList.ReloadData();
            }
        }

        private void HilightFilterButton(int filter)
        {
            for(int i = 0; i < this.FilterButtons.Length; i++)
            {
                Button b = this.FilterButtons[i];
                b.SetTextColor(ViewBuilder.ColorFromARGB(i == filter ? AppController.Colors.Jet : AppController.Colors.White));
                b.SetBackgroundColor(ViewBuilder.ColorFromARGB(i == filter ? AppController.Colors.White : AppController.Colors.Jet));
            }
        }

        #endregion

        #region Event Handlers

        private void FilterButton_Click(object sender, EventArgs e)
        {
            int filter = Array.IndexOf(this.FilterButtons, sender);
            if (filter == _filter)
                return;
            
            RefreshIssues(filter);
        }

        private void IssueList_ItemSelected(object sender, ItemListSelectEventArgs e)
        {
            Issue issue = e.Item as Issue;

            var f = new ChatFragment();
            f.Arguments = new Bundle();
            f.Arguments.PutInt("GimmickId", _gimmickId);            
            f.Arguments.PutObject<Issue>("Issue", issue);
            this.FragmentManager.BeginTransaction()
                .AddToBackStack("BeforeChatFragment")
                .Replace(Resource.Id.ContentLayout, f, "ChatFragment")
                .Commit();
        }

        #endregion
    }
}
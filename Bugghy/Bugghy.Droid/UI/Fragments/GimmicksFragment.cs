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
    public class GimmicksFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes

        class GimmickAdapter : ItemRecyclerAdapter<GimmickAdapter.ChatViewHolder, Gimmick>
        {
            #region Inner Classes

            public class ChatViewHolder : ItemViewHolder
            {
                [Widget]
                public ImageView ThumbImage;

                [Widget]
                public TextView NameLabel;

                [Widget]
                public TextView OwnerLabel;

                public ChatViewHolder(View itemView)
                    : base(itemView)
                {
                }
            }

            #endregion

            #region Costants and Fields
            #endregion

            #region Constructors

            public GimmickAdapter(AdMaiora.AppKit.UI.App.Fragment context, IEnumerable<Gimmick> source)
                : base(context, Resource.Layout.CellGimmick, source)
            {
            }

            #endregion

            #region Public Methods

            public override void GetView(int postion, ChatViewHolder holder, View view, Gimmick item)
            {
                AppController.Images.SetImageForView(
                    new Uri(item.ImageUrl), "image_gear", holder.ThumbImage);

                holder.NameLabel.Text = item.Name;
                holder.OwnerLabel.Text = item.Owner;
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

            #region Methods
            #endregion
        }

        #endregion

        #region Constants and Fields

        private GimmickAdapter _adapter;

        // This flag check if we are already calling the login REST service
        private bool _isRefreshingGimmicks;
        // This cancellation token is used to cancel the rest send message request
        private CancellationTokenSource _cts0;

        #endregion

        #region Widgets

        [Widget]
        private ItemRecyclerView GimmickList;

        #endregion

        #region Constructors

        public GimmicksFragment()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Fragment Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentGimmicks, inflater, container);

            this.HasOptionsMenu = true;

            #endregion           

            this.Title = "All Gimmicks";

            this.ActionBar.Show();

            this.GimmickList.ItemSelected += GimmickList_ItemSelected;

            RefreshGimmicks();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            menu.Clear();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Logout();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }

        public override bool OnBackButton()
        {
            Logout();
            return true;                
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

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

            this.GimmickList.Visibility = ViewStates.Gone;

            _isRefreshingGimmicks = true;
            ((MainActivity)this.Activity).BlockUI();

            Gimmick[] gimmicks = null;

            _cts0 = new CancellationTokenSource();
            AppController.RefreshGimmicks(_cts0,                
                (newGimmicks) =>
                {
                    gimmicks = newGimmicks;
                },
                (error) =>
                {
                    Toast.MakeText(this.Activity.ApplicationContext, error, ToastLength.Long).Show();
                },
                () =>
                {
                    if (gimmicks != null)
                    {
                        LoadGimmicks(gimmicks);

                        if(_adapter?.ItemCount > 0)
                            this.GimmickList.Visibility = ViewStates.Visible;

                        _isRefreshingGimmicks = false;
                        ((MainActivity)this.Activity).UnblockUI();
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

                                if (_adapter?.ItemCount > 0)
                                    this.GimmickList.Visibility = ViewStates.Visible;

                                _isRefreshingGimmicks = false;
                                ((MainActivity)this.Activity).UnblockUI();

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

            if (_adapter == null)
            {
                _adapter = new GimmickAdapter(this, gimmicks);
                this.GimmickList.SetAdapter(_adapter);
            }
            else
            {
                _adapter.Refresh(gimmicks);
                this.GimmickList.ReloadData();
            }
        }

        private void Logout()
        {
            (new AlertDialog.Builder(this.Activity))
                .SetTitle("Do you want to logout now?")
                .SetMessage("")
                .SetPositiveButton("Yes please!",
                    (s, ea) =>
                    {
                        AppController.Settings.AuthAccessToken = null;
                        AppController.Settings.AuthExpirationDate = null;

                        this.DismissKeyboard();
                        this.FragmentManager.PopBackStack();
                    })
                .SetNegativeButton("Not now",
                    (s, ea) =>
                    {
                    })
                .Show();
        }


        #endregion

        #region Event Handlers

        private void GimmickList_ItemSelected(object sender, ItemListSelectEventArgs e)
        {
            Gimmick gimmick = e.Item as Gimmick;

            var f = new GimmickFragment();
            f.Arguments = new Bundle();
            f.Arguments.PutObject<Gimmick>("Gimmick", gimmick);
            this.FragmentManager.BeginTransaction()
                .AddToBackStack("BeforeGimmickFragment")
                .Replace(Resource.Id.ContentLayout, f, "GimmickFragment")
                .Commit();
        }

        #endregion
    }
}
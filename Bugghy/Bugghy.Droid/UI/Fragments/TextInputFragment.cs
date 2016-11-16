namespace AdMaiora.Listy
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
    using Android.Views.InputMethods;

    using AdMaiora.AppKit.UI;

    public class TextInputFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields    
        #endregion

        #region Widgets

        [Widget]
        private EditText InputText;

        #endregion

        #region Events

        public event EventHandler<TextInputDoneEventArgs> TextInputDone;

        #endregion

        #region Constructors and Destructors

        public TextInputFragment()
        {
        }

        #endregion

        #region Properties

        public string ContentText
        {
            get
            {
                return this.Arguments.GetString("Content");
            }
            set
            {
                this.Arguments.PutString("Content", value);
            }
        }

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

            SetContentView(Resource.Layout.FragmentTextInput, inflater, container);

            this.HasOptionsMenu = true;

            this.Activity.Window.SetSoftInputMode(SoftInput.StateVisible);

            ResizeToShowKeyboard();

            #endregion                      

            this.Title = "Description";

            this.InputText.Text = this.ContentText;
            this.InputText.RequestUserInput();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            menu.Clear();
            menu.Add(Menu.None, 2, Menu.None, "Ok").SetShowAsAction(ShowAsAction.Always);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {                
                case 2:

                    this.DismissKeyboard();

                    OnInputDone(); 
                                       
                    this.FragmentManager.PopBackStack();

                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        #endregion

        #region Public Methods
        #endregion

        #region Event Raising Methods

        private void OnInputDone()
        {
            if (TextInputDone != null)
                TextInputDone(this, new TextInputDoneEventArgs(this.InputText.Text));
        }

        #endregion

        #region Methods
        #endregion

        #region Event Handlers
        #endregion
    }
}
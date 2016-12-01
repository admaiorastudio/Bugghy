namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Foundation;
    using UIKit;
    using CoreGraphics;

    using AdMaiora.AppKit.UI;    

    public partial class TextInputViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        #region Inner Classes
        #endregion        

        #region Constants and Fields
        #endregion

        #region Events

        public event EventHandler<TextInputDoneEventArgs> TextInputDone;

        #endregion

        #region Constructors

        public TextInputViewController()
            : base("TextInputViewController", null)
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

            ResizeToShowKeyboard();

            #endregion

            this.Title = "Description"; 
            
            this.InputText.Text = this.ContentText;
            this.InputText.RequestUserInput();
        }

        public override bool CreateBarButtonItems(UIBarButtonCreator items)
        {
            items
                .AddItem("Ok", UIBarButtonItemStyle.Plain);

            return true;
        }


        public override bool BarButtonItemSelected(int index)
        {
            switch (index)
            {
                case 0:

                    this.DismissKeyboard();

                    OnInputDone();

                    this.NavigationController.PopViewController(false);

                    return true;

                default:
                    return base.BarButtonItemSelected(index);
            }
        }    

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
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



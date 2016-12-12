namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Foundation;
    using UIKit;
    using CoreGraphics;
    using CoreAnimation;

    using AdMaiora.AppKit.UI;

    using Google.Core;
    using Google.SignIn;

    #pragma warning disable CS4014
    public partial class LoginViewController : AdMaiora.AppKit.UI.App.UISubViewController, ISignInDelegate, ISignInUIDelegate
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private string _email;
        private string _password;

        // This flag check if we are already calling the login REST service
        private bool _isLogginUser;
        // This cancellation token is used to cancel the rest login request
        private CancellationTokenSource _cts0;

        // This flag check if we are already calling the login REST service
        private bool _isConfirmingUser;
        // This cancellation token is used to cancel the rest login request
        private CancellationTokenSource _cts1;

        #endregion
        
        #region Constructors

        public LoginViewController()
            : base("LoginViewController", null)
        {
        }

        #endregion
        
        #region Properties

        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Setup Google API for SignIn
            NSError error;
            Context.SharedInstance.Configure(out error);
            if (error != null)
                SignIn.SharedInstance.ClientID = AppController.Globals.GoogleClientId_iOS;

            SignIn.SharedInstance.Delegate = this;
            SignIn.SharedInstance.UIDelegate = this;          
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff

            AutoShouldReturnTextFields(new[] { this.EmailText, this.PasswordText });

            SlideUpToShowKeyboard();
            
            #endregion

            this.NavigationController.SetNavigationBarHidden(true, true);            

            this.EmailText.Text = AppController.Settings.LastLoginUsernameUsed;

            this.PasswordText.Text = String.Empty;
            this.PasswordText.ShouldReturn += PasswordText_ShouldReturn;

            this.LoginButton.TouchUpInside += LoginButton_TouchUpInside;

            this.GoogleLoginButton.TouchUpInside += GoogleLoginButton_TouchUpInside;
                   
            this.RegisterButton.TouchUpInside += RegisterButton_TouchUpInside;

            this.VerifyButton.Hidden = true;
            this.VerifyButton.TouchUpInside += VerifyButton_TouchUpInside;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (_cts0 != null)
                _cts0.Cancel();

            if (_cts1 != null)
                _cts1.Cancel();
                       
            this.PasswordText.ShouldReturn -= PasswordText_ShouldReturn;
            this.LoginButton.TouchUpInside -= LoginButton_TouchUpInside;
            this.GoogleLoginButton.TouchUpInside -= GoogleLoginButton_TouchUpInside;
            this.RegisterButton.TouchUpInside -= RegisterButton_TouchUpInside;                       
            this.VerifyButton.TouchUpInside -= VerifyButton_TouchUpInside;
        }

        #endregion

        #region Google API Methods

        public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
        {
            try
            {
                string gClientId = AppController.Globals.GoogleClientId_iOS;
                string gEmail = user.Profile.Email;
                string gToken = user.Authentication.IdToken;

                _cts0 = new CancellationTokenSource();
                AppController.LoginUser(_cts0, gClientId, gEmail, gToken,
                    (d) =>
                    {
                        AppController.Settings.LastLoginUserIdUsed = d.UserId;
                        AppController.Settings.LastLoginUsernameUsed = _email;
                        AppController.Settings.AuthAccessToken = d.AuthAccessToken;
                        AppController.Settings.AuthExpirationDate = d.AuthExpirationDate.GetValueOrDefault().ToLocalTime();
                        AppController.Settings.GoogleSignedIn = true;

                        var c = new GimmicksViewController();
                        this.NavigationController.PushViewController(c, true);
                    },
                    (err) =>
                    {
                        UIToast.MakeText(err, UIToastLength.Long).Show();
                    },
                    () =>
                    {
                        ((MainViewController)this.MainViewController).UnblockUI();
                    });
            }
            catch (Exception ex)
            {
                ((MainViewController)this.MainViewController).UnblockUI();

                UIToast.MakeText("Error logging with Google!", UIToastLength.Long).Show();
            }
            finally
            {
                // Do nothing...
            }
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        private void LoginUser()
        {
            if (ValidateInput())
            {
                if (_isLogginUser)
                    return;

                _isLogginUser = true;

                _email = this.EmailText.Text;
                _password = this.PasswordText.Text;

                // Prevent user form tapping views while logging
                ((MainViewController)this.MainViewController).BlockUI();

                // Create a new cancellation token for this request                
                _cts1 = new CancellationTokenSource();
                AppController.LoginUser(_cts1, _email, _password,
                    // Service call success                 
                    (data) =>
                    {
                        AppController.Settings.LastLoginUserIdUsed = data.UserId;
                        AppController.Settings.LastLoginUsernameUsed = _email;                        
                        AppController.Settings.AuthAccessToken = data.AuthAccessToken;
                        AppController.Settings.AuthExpirationDate = data.AuthExpirationDate.GetValueOrDefault().ToLocalTime();
                        AppController.Settings.GoogleSignedIn = false;

                        var c = new GimmicksViewController();
                        this.NavigationController.PushViewController(c, true);
                    },
                    // Service call error
                    (error) =>
                    {
                        if (error.Contains("confirm"))
                            this.VerifyButton.Hidden = false;

                        UIToast.MakeText(error, UIToastLength.Long).Show();
                    },
                    // Service call finished 
                    () =>
                    {
                        _isLogginUser = false;

                        // Allow user to tap views
                        ((MainViewController)this.MainViewController).UnblockUI();
                    });
            }
        }

        private void VerifyUser()
        {
            if (ValidateInput())
            {
                if (_isLogginUser)
                    return;

                _isLogginUser = true;

                _email = this.EmailText.Text;
                _password = this.PasswordText.Text;

                // Prevent user form tapping views while logging
                ((MainViewController)this.MainViewController).BlockUI();

                this.VerifyButton.Hidden = true;

                // Create a new cancellation token for this request                
                _cts1 = new CancellationTokenSource();
                AppController.VerifyUser(_cts1, _email, _password,
                    // Service call success                 
                    () =>
                    {
                        UIToast.MakeText("You should receive a new mail!", UIToastLength.Long).Show();
                    },
                    // Service call error
                    (error) =>
                    {
                        UIToast.MakeText(error, UIToastLength.Long).Show();
                    },
                    // Service call finished 
                    () =>
                    {
                        _isLogginUser = false;

                        // Allow user to tap views
                        ((MainViewController)this.MainViewController).UnblockUI();
                    });
            }
        }

        private bool ValidateInput()
        {
            var validator = new WidgetValidator()
                .AddValidator(() => this.EmailText.Text, WidgetValidator.IsNotNullOrEmpty, "Please insert your email.")
                .AddValidator(() => this.EmailText.Text, WidgetValidator.IsEmail, "Your mail is not valid!")
                .AddValidator(() => this.PasswordText.Text, WidgetValidator.IsNotNullOrEmpty, "Please insert your password.")
                .AddValidator(() => this.PasswordText.Text, WidgetValidator.IsPasswordMin8, "Your passowrd is not valid!");

            string errorMessage;
            if (!validator.Validate(out errorMessage))
            {
                UIToast.MakeText(errorMessage, UIToastLength.Long).Show();

                return false;
            }

            return true;
        }

        private void LoginInGoogle()
        {
            ((MainViewController)this.MainViewController).BlockUI();

            if (AppController.Settings.GoogleSignedIn)
            {
                SignIn.SharedInstance.SignOutUser();
                AppController.Settings.GoogleSignedIn = false;
            }

            SignIn.SharedInstance.SignInUser();            
        }

        #endregion

        #region Event Handlers

        private bool PasswordText_ShouldReturn(UITextField sender)
        {
            LoginUser();

            DismissKeyboard();

            return true;
        }

        private void LoginButton_TouchUpInside(object sender, EventArgs e)
        {
            LoginUser();
            
            DismissKeyboard();
        }

        private void GoogleLoginButton_TouchUpInside(object sender, EventArgs e)
        {
            LoginInGoogle();

            DismissKeyboard();
        }

        private void RegisterButton_TouchUpInside(object sender, EventArgs e)
        {
            var c = new Registration0ViewController();            
            this.NavigationController.PushViewController(c, true);

            DismissKeyboard();
        }

        private void VerifyButton_TouchUpInside(object sender, EventArgs e)
        {
            VerifyUser();

            DismissKeyboard();
        }

        #endregion
    }
}



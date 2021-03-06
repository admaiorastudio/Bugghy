﻿namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Foundation;
    using UIKit;

    using AdMaiora.AppKit;
    using AdMaiora.AppKit.UI;

    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIAppKitApplicationDelegate
    {
        #region Constants and Fields
        #endregion

        #region Constructors

        public AppDelegate()
        {

        }

        #endregion

        #region Properties

        public override UIWindow Window
        {
            get;
            set;
        }

        #endregion

        #region Application Methods

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Setup Application
            AppController.EnableSettings(new AdMaiora.AppKit.Data.UserSettingsPlatformiOS());
            AppController.EnableUtilities(new AdMaiora.AppKit.Utils.ExecutorPlatformiOS());
            AppController.EnableFileSystem(new AdMaiora.AppKit.IO.FileSystemPlatformiOS());
            AppController.EnableImageLoader(new AdMaiora.AppKit.Utils.ImageLoaderPlatofrmiOS());
            AppController.EnableDataStorage(new AdMaiora.AppKit.Data.DataStoragePlatformiOS());
            AppController.EnableServices(new AdMaiora.AppKit.Services.ServiceClientPlatformiOS());

            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            return RegisterMainLauncher(new SplashViewController(), launchOptions);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return Google.SignIn.SignIn.SharedInstance.HandleUrl(url, 
                options["UIApplicationOpenURLOptionsSourceApplicationKey"].ToString(), 
                options["UIApplicationOpenURLOptionsAnnotationKey"]);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return Google.SignIn.SignIn.SharedInstance.HandleUrl(url, sourceApplication, annotation);
        }

        #endregion
    }
}
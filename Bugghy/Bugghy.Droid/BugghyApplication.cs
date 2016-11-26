namespace AdMaiora.Bugghy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Runtime;
    using Android.Widget;
    using Android.OS;
    using Android.Gms.Common;
    using Android.Gms.Gcm.Iid;
    using Android.Gms.Gcm;
    using Android.Support.V4.App;
    
    using AdMaiora.AppKit;
   
#if DEBUG
    [Application(Name = "admaiora.bugghy.ListyApplication")]
#else
    [Application(Name = "admaiora.bugghy.ListyApplication", Debuggable = false)]
#endif

#pragma warning disable CS4014
    public class BugghyApplication : AppKitApplication
    {
        #region Constants and Fields        
        #endregion

        #region Events
        #endregion

        #region Constructors

        public BugghyApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) 
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Application Methods

        public override void OnCreate()
        {
            base.OnCreate();
            
            // Setup Application
            AppController.EnableSettings(new AdMaiora.AppKit.Data.UserSettingsPlatformAndroid());
            AppController.EnableUtilities(new AdMaiora.AppKit.Utils.ExecutorPlatformAndroid());
            AppController.EnableFileSystem(new AdMaiora.AppKit.IO.FileSystemPlatformAndroid());
            AppController.EnableImageLoader(new AdMaiora.AppKit.Utils.ImageLoaderPlatofrmAndroid());
            AppController.EnableDataStorage(new AdMaiora.AppKit.Data.DataStoragePlatformAndroid());
            AppController.EnableServices(new AdMaiora.AppKit.Services.ServiceClientPlatformAndroid());                                    
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Parqueadero.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            SQLitePCL.CurrentPlatform.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

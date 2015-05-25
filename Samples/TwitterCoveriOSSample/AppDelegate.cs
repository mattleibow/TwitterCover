using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace TwitterCoveriOSSample
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = new UINavigationController(new RootViewController());
            Window.MakeKeyAndVisible();

            return true;
        }
    }
}
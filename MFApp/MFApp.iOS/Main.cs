using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace MFApp.iOS
{
    public class Application
    {        
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            CLLocationManager locMgr;
            locMgr = new CLLocationManager();
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // locMgr.RequestAlwaysAuthorization(); // works in background
                locMgr.RequestWhenInUseAuthorization(); // only in foreground
            }

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");

        }
    }
}

using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;
using Android.Support.Design.Widget;
using Android.Util;

namespace MFApp.Droid
{
    [Activity(Label = "MondayFighter", Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            const int REQUEST_LOCATION = 1;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                // Provide an additional rationale to the user if the permission was not granted
                // and the user would benefit from additional context for the use of the permission.
                // For example if the user has previously denied the permission.
                Log.Info(TAG, "Displaying camera permission rationale to provide additional context.");

                var requiredPermissions = new String[] { Manifest.Permission.AccessFineLocation };
                Snackbar.Make(this.CurrentFocus,
                               "Möchte Berechtigung haben!",
                               Snackbar.LengthIndefinite)
                        .SetAction("OK",
                                   new Action<View>(delegate (View obj) {
                                       ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);
                                   }
                        )
                ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_LOCATION);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
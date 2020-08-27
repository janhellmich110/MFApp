using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using MFApp.Services;
using MFApp.Views;

namespace MFApp
{
    public partial class App : Application
    {
        public static ILocationUpdateService LocationUpdateService;

        public App()
        {
            
            InitializeComponent();
            DependencyService.Register<PlayerDataStore>();
            DependencyService.Register<ProfileDataStore>();
            DependencyService.Register<GolfclubDataStore>();
            DependencyService.Register<EventDataStore>();
            DependencyService.Register<TournamentDataStore>();
            DependencyService.Register<CourseDataStore>();
            DependencyService.Register<CourseHandicapTableDataStore>();
            DependencyService.Register<CourseHandicapDataStore>();
            DependencyService.Register<FlightDataStore>();
            DependencyService.Register<Flight2PlayerDataStore>();
            DependencyService.Register<TeeDataStore>();
            DependencyService.Register<ResultDataStore>();
            DependencyService.Register<TeeInfoDataStore>();

            DependencyService.Register<ILocationUpdateService>();

            Device.SetFlags(new string[] { "Expander_Experimental" });

            // sync events
            SyncWebData();

            MainPage = new MainPage();

            ILocationUpdateService LocationUpdateService = DependencyService.Get<ILocationUpdateService>();
            LocationUpdateService.LocationChanged += LocationUpdateService_LocationChanged;

            //try
            //{
            //    //var location = await Geolocation.GetLastKnownLocationAsync();
            //    var request = new GeolocationRequest(GeolocationAccuracy.Medium, new TimeSpan(0, 0, 10));
            //    var tasklocation = Geolocation.GetLocationAsync(request);
            //    var location = tasklocation.Result;

            //    if (location != null)
            //    {
            //        // save location in profile
            //    }
            //}
            //catch (Exception exp)
            //{
            //    // manage exception
            //}

        }

        private async Task<bool> SyncWebData()
        {
            MFWebDataSync DataSync = new MFWebDataSync();
            bool bResult = await DataSync.SyncMFWebSynchron();
            return bResult;
        }

        private void LocationUpdateService_LocationChanged(object sender, ILocationEventArgs e)
        {
            //Here you can get the user's location from "e" -> new Location(e.Latitude, e.Longitude);
            //new Location is from Xamarin.Essentials Location object.
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

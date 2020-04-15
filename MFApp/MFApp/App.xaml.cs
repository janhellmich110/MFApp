using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Services;
using MFApp.Views;

namespace MFApp
{
    public partial class App : Application
    {
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

            // sync events
            SyncWebData();

            MainPage = new MainPage();
        }

        private async Task<bool> SyncWebData()
        {
            MFWebDataSync DataSync = new MFWebDataSync();
            bool bResult = await DataSync.SyncMFWebSynchron();
            return bResult;
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

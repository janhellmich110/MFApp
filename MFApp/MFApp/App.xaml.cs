using System;
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

            MainPage = new MainPage();
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

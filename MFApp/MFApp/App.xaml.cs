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

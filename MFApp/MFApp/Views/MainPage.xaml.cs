using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Services;
using MFApp.Views;


using MFApp.Models;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Home, (NavigationPage)Detail);

            IDataStore<Profile> DataStore = DependencyService.Get<IDataStore<Profile>>();
            var profilesTask = DataStore.GetItemsAsync();
            var profiles = profilesTask.Result;

            bool profileExists = false;
            foreach(Profile p in profiles)
            {
                profileExists = true;
                break;
            }
            if (!profileExists)
                Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Home:
                        MenuPages.Add(id, new NavigationPage(new HomePage()));
                        break;
                    case (int)MenuItemType.Player:
                        MenuPages.Add(id, new NavigationPage(new PlayerPage()));
                        break;
                    case (int)MenuItemType.Configuration:
                        MenuPages.Add(id, new NavigationPage(new ConfigurationPage()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}
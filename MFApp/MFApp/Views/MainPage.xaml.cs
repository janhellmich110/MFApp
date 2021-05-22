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

            ((NavigationPage)Detail).BarBackgroundColor = Color.FromRgb(30, 39, 104);
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
                Navigation.PushModalAsync(new NavigationPage(new LoginPage()) {
                    BarBackgroundColor = Color.FromRgb(30, 39, 104)
                });
            else
            {
                // sync data moved to app start       
            }
        }        

        public async Task NavigateFromMenu(int id)
        {
            if (MenuPages.ContainsKey(id))
            {
                MenuPages.Remove(id);
            }
            int GolfClubId = 1;

            if (true)
            {
                switch (id)
                {
                    case (int)MenuItemType.Home:
                        MenuPages.Add(id, new NavigationPage(new HomePage()) {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.Player:
                        MenuPages.Add(id, new NavigationPage(new PlayerPage())
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.Configuration:
                        MenuPages.Add(id, new NavigationPage(new ConfigurationPage())
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.LogOff:
                        MenuPages.Add(id, new NavigationPage(new MeinProfil())
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.Results:
                        MenuPages.Add(id, new NavigationPage(new ResultPage())
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.Adhoc:
                        MenuPages.Add(id, new NavigationPage(new AdhocTournament())
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.Birdiebook:
                        MenuPages.Add(id, new NavigationPage(new TeeInfoPage(GolfClubId, 1))
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.OOM:
                        MenuPages.Add(id, new NavigationPage(new WebViewPage("OOM", "https://demo.portivity.de/mfweb/oomapp"))
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage())
                        {
                            BarBackgroundColor = Color.FromRgb(30, 39, 104)
                        });
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
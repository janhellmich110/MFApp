using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Models;
using MFApp.Services;

namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeinProfil : ContentPage
    {
        public Profile MyProfile { get; set; }

        public MeinProfil()
        {
            InitializeComponent();

            IDataStore<Profile> DataStore = DependencyService.Get<IDataStore<Profile>>();
            var profilesTask = DataStore.GetItemsAsync();
            var profiles = profilesTask.Result;

            foreach (Profile p in profiles)
            {
                MyProfile = p;
                break;
            }
            this.BindingContext = MyProfile;
        }

        protected override void OnAppearing()
        {
            this.BindingContext = null;
            base.OnAppearing();

            IDataStore<Profile> DataStore = DependencyService.Get<IDataStore<Profile>>();
            var profilesTask = DataStore.GetItemsAsync();
            var profiles = profilesTask.Result;

            foreach (Profile p in profiles)
            {
                MyProfile = p;
                break;
            }
            this.BindingContext = MyProfile;
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {

            IDataStore<Profile> DataStore = DependencyService.Get<IDataStore<Profile>>();
            var profilesTask = DataStore.GetItemsAsync();
            var profiles = profilesTask.Result;
            int profileId = 0;
            bool profileExists = false;
            foreach (Profile p in profiles)
            {
                profileExists = true;
                profileId = p.Id;
                break;
            }

            if (profileExists)
                DataStore.DeleteItemAsync(profileId);

            Navigation.PushModalAsync(new NavigationPage(new LoginPage()));

        }
    }
}
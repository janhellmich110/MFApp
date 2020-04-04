using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Services;
using MFApp.Models;



namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
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

            Navigation.PushModalAsync(new NavigationPage( new LoginPage()));

        }
    }
}
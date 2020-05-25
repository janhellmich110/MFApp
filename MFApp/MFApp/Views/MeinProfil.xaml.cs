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

        IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();

        public MeinProfil()
        {
            InitializeComponent();            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            List<Profile> profiles = (await DataStoreProfile.GetItemsAsync()).ToList();

            if (profiles.Count() > 0)
                MyProfile = profiles[0];

            this.BindingContext = MyProfile;
        }

        private async void Logout_Clicked(object sender, EventArgs e)
        {
            List<Profile> profiles = (await DataStoreProfile.GetItemsAsync()).ToList();

            if (profiles.Count() > 0)
            {
                await DataStoreProfile.DeleteItemAsync(profiles[0].Id);
            }

            await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));

        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            var Player = new Player
            {
                Id = MyProfile.Id,
                Name = MyProfile.Name,
                Initials = MyProfile.Initials,
                UserName = MyProfile.UserName,
                UserPassword = MyProfile.UserPassword,
                Handicap = MyProfile.Handicap,
                Birthday = MyProfile.Birthday,
                Mail = MyProfile.Mail,
                Gender=MyProfile.Gender,
                GroupId=MyProfile.GroupId
            };

            // update player local
            IDataStore<Player> DataStorePlayer= DependencyService.Get<IDataStore<Player>>();
            await DataStorePlayer.UpdateItemAsync(Player);

            // send new player to web
            MFWebDataSync DataSync = new MFWebDataSync();
            await DataSync.SendNewPlayer(Player);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
            this.BindingContext = null;

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
            string inputHdcp = ((Entry)this.FindByName("InputHandicap")).Text;
            inputHdcp = inputHdcp.Replace("-", "");
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
            {
                inputHdcp = inputHdcp.Replace(".", ",");
            }
            else
            {
                inputHdcp = inputHdcp.Replace(",", ".");
            }


            string inputDGVHdcp = ((Entry)this.FindByName("InputDGVHandicap")).Text;
            inputDGVHdcp = inputDGVHdcp.Replace("-", "");
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
            {
                inputDGVHdcp = inputDGVHdcp.Replace(".", ",");
            }
            else
            {
                inputDGVHdcp = inputDGVHdcp.Replace(",", ".");
            }
            Button ButtonSave = (Button)this.FindByName("ButtonSave");

            try
            {
                double decHdcp = 0;
                try
                {
                    decHdcp = Convert.ToDouble(inputHdcp);
                }
                catch (Exception) { }
                double decDGVHdcp = 0;
                try
                {
                    decDGVHdcp = Convert.ToDouble(inputDGVHdcp);
                }
                catch (Exception) { }

                var Player = new Player
                {
                    Id = MyProfile.Id,
                    Name = MyProfile.Name,
                    Initials = MyProfile.Initials,
                    UserName = MyProfile.UserName,
                    UserPassword = MyProfile.UserPassword,
                    Handicap = decHdcp,
                    DGVHandicap = decDGVHdcp,
                    Birthday = MyProfile.Birthday,
                    Mail = MyProfile.Mail,
                    Gender = MyProfile.Gender,
                    GroupId = MyProfile.GroupId
                };

                // update player local
                IDataStore<Player> DataStorePlayer = DependencyService.Get<IDataStore<Player>>();
                await DataStorePlayer.UpdateItemAsync(Player);

                //update profile
                MyProfile.Handicap = decHdcp;
                MyProfile.DGVHandicap = decDGVHdcp;
                IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();
                await DataStoreProfile.UpdateItemAsync(MyProfile);

                // send new player to web
                MFWebDataSync DataSync = new MFWebDataSync();
                await DataSync.SendNewPlayer(Player);
            }
            catch(Exception exp)
            {
                ButtonSave.Text = "Fehler! Nochmal versuchen";
            }
            
            ButtonSave.Text = "Daten wurden gespeichert";
        }
    }
}
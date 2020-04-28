using System;
using System.ComponentModel;
using Xamarin.Forms;


using MFApp.Models;
using MFApp.Services;
using System.Linq;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class NewPlayerPage : ContentPage
    {
        public Player Player { get; set; }

        public NewPlayerPage()
        {
            InitializeComponent();

            Player = new Player
            {
                Name = "",
                Initials = "",
                UserName = "",
                Handicap = 0.0,
                Birthday = new DateTime(2000, 01, 01)
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            // convert gender
            Picker genderPicker = (Picker)this.FindByName("SelectGender");

            if (genderPicker.SelectedItem.ToString().ToLower() == "mann")
                Player.Gender = Gender.Mann;
            else
                Player.Gender = Gender.Frau;

            // set group for new player from profile
            IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();
            var profilesTask = DataStoreProfile.GetItemsAsync();
            var profile = profilesTask.Result.FirstOrDefault();
            if(profile != null)
            {
                Player.GroupId = profile.GroupId;
            }

            MessagingCenter.Send(this, "AddItem", Player);

            // send new player to web
            MFWebDataSync DataSync = new MFWebDataSync();
            await DataSync.SendNewPlayer(Player);

            IDataStore<Player> DataStore = DependencyService.Get<IDataStore<Player>>();
            await DataStore.SyncMFWeb();

            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
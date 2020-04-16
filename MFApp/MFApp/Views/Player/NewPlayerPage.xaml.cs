using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Telerik.XamarinForms;

using MFApp.Models;
using MFApp.Services;
using Telerik.XamarinForms.Input;

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
            MessagingCenter.Send(this, "AddItem", Player);

            // get birthday
            RadDatePicker picker = (RadDatePicker)this.FindByName("PlayerBirthday");
            Player.Birthday = picker.SelectedDate.Value;
            if(Player.Handicap > )

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
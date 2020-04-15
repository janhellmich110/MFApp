using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Services;
using MFApp.Models;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            MFWebDataSync DataSync = new MFWebDataSync();
            bool result = await DataSync.SyncMFWebSynchron();

            Button button = sender as Button;
            if (result)
            {
                button.Text = "Daten wurden erfolgreich synchonisiert";
            }
            else
            {
                button.Text = "Fehler bei Daten-Sync";
            }
            IsBusy = false;
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LogoutPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Button button = (Button) this.FindByName("SyncProgressButton");
            if (button != null)
            {
                button.Text = "Sync Data";
            }

            button = (Button)this.FindByName("ResetDB");
            if (button != null)
            {
                button.Text = "Reset DB";
            }
        }

        private async void ResetDB_Clicked(object sender, EventArgs e)
        {
            MFWebDataSync DataSync = new MFWebDataSync();
            bool result = await DataSync.ResetDB();

            Button button = sender as Button;
            if (result)
            {
                button.Text = "Daten wurden erfolgreich gelöscht";
            }
            else
            {
                button.Text = "Fehler bei Reset DB";
            }
        }
    }
}
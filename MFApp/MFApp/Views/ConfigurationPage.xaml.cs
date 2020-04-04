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
            IDataStore<Player> DataStore = DependencyService.Get<IDataStore<Player>>();

            bool result = await DataStore.SyncMFWeb();

            Button button = sender as Button;
            if (result)
            {
                button.Text = "Spieler erfolgreich synchonisiert";
            }
            else
            {
                button.Text = "Fehler bei Spieler-Sync";
            }

        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LogoutPage());
        }
    }
}
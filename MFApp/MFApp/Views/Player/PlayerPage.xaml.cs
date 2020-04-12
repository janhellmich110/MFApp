using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Models;
using MFApp.Views;
using MFApp.ViewModels;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class PlayerPage : ContentPage
    {
        PlayerViewModel viewModel;

        public PlayerPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new PlayerViewModel();

        }

        async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject)sender;
            var Player = (Player)layout.BindingContext;
            await Navigation.PushAsync(new PlayerDetailPage(new PlayerDetailViewModel(Player)));
        }

        async void AddPlayer_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewPlayerPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.IsBusy = true;
        }
    }
}
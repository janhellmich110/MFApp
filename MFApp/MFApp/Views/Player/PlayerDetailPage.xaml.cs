using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Models;
using MFApp.ViewModels;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class PlayerDetailPage : ContentPage
    {
        PlayerDetailViewModel viewModel;

        public PlayerDetailPage(PlayerDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public PlayerDetailPage()
        {
            InitializeComponent();

            var player = new Player
            {
                Name = "Jan Hellmich",
                Initials = "JH",
                UserName = "janh",
                Handicap = 11.9,
                Birthday = new DateTime(1966, 10, 11)
            };

            viewModel = new PlayerDetailViewModel(player);
            BindingContext = viewModel;
        }
    }
}
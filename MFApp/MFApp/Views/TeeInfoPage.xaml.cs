using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Services;
using MFApp.Models;
using MFApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class TeeInfoPage : ContentPage
    {
        public TeeInfoViewModel teeInfoViewModel { get; set; }

        private int golfClubId;
        private int teeNumber;
        public TeeInfoPage()
        {
            InitializeComponent();
        }

        public TeeInfoPage(int GolfClubId, int TeeNumber)
        {
            InitializeComponent();
            golfClubId = GolfClubId;
            teeNumber = TeeNumber;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            teeInfoViewModel = new TeeInfoViewModel(golfClubId, teeNumber);
            await teeInfoViewModel.LoadAllPlaces();

            Image teeImage = (Image)this.FindByName("TeeImage");
            teeImage.Source = teeInfoViewModel.ImageName;

            this.BindingContext = teeInfoViewModel;
        }

        async void Refresh_Clicked(object sender, EventArgs e)
        {
            CollectionView AllPlaces = (CollectionView)this.FindByName("AllPlacesCollectionView");
            AllPlaces.BindingContext = null;

            await teeInfoViewModel.LoadAllPlaces();
            AllPlaces.BindingContext = teeInfoViewModel;
        }

        async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
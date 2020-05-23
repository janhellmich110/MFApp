using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Services;
using MFApp.Models;
using MFApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MapsPage : ContentPage
    {
        public MapsViewModel mapsViewModel { get; set; }
        public MapsPage()
        {
            InitializeComponent();            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 10));
            var location = await Geolocation.GetLocationAsync(request);

            // set labels
            Label lat = (Label)this.FindByName("Latitude");
            Label lon = (Label)this.FindByName("Longitude");
            lat.Text = location.Latitude.ToString();
            lon.Text = location.Longitude.ToString();

            var map = new Xamarin.Forms.Maps.Map(MapSpan.FromCenterAndRadius(
                new Position(location.Latitude, location.Longitude),
                Distance.FromMeters(500)))
            {
                IsShowingUser = true,
                VerticalOptions=LayoutOptions.FillAndExpand,
                MapType= MapType.Satellite
            };

            MapContainer.Children.Add(map);

        }
    }
}
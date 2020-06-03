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
        private bool ImageZoomed = false;

        public TeeInfoPage()
        {
            InitializeComponent();
            golfClubId = 1;
            teeNumber = 1;
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

            // set teeinfo
            Label Holenumber = (Label)this.FindByName("HoleNumber");
            Label Par = (Label)this.FindByName("Par");
            Label Hdcp = (Label)this.FindByName("Hdcp");
            Label Distance_Yellow = (Label)this.FindByName("Distance_Yellow");
            Label Distance_Red = (Label)this.FindByName("Distance_Red");


            // get tee
            IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
            List<Course> Courses = (await DataStoreCourse.GetItemsAsync()).ToList();

            IDataStore<Tee> DataStoreTee = DependencyService.Get<IDataStore<Tee>>();
            List<Tee> Tees = (await DataStoreTee.GetItemsAsync()).ToList();

            Tee currentTee;
            foreach(Course c in Courses.Where(x=>x.GolfclubId== golfClubId))
            {
                currentTee = Tees.Where(x => x.CourseId == c.Id).Where(y=>y.Name==teeNumber).FirstOrDefault();
                if (currentTee != null)
                {
                    Holenumber.Text = currentTee.Name.ToString();
                    Par.Text = currentTee.Par.ToString();
                    Hdcp.Text = currentTee.Hcp.ToString();
                    Distance_Yellow.Text = currentTee.Length.ToString();
                    Distance_Red.Text = currentTee.LengthRed.ToString();
                    break;
                }
            }

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                // cast to an image
                Image theImage = (Image)sender;

                if(ImageZoomed)
                {
                    ((StackLayout)theImage.Parent).WidthRequest = ((StackLayout)theImage.Parent).Width / 2;
                    ((StackLayout)theImage.Parent).HeightRequest = ((StackLayout)theImage.Parent).Height / 2;

                    theImage.Aspect = Aspect.AspectFit;
                    ImageZoomed = false;
                }
                else
                {
                    double origWidth = ((ScrollView)theImage.Parent.Parent).Width;
                    double origHeight = ((ScrollView)theImage.Parent.Parent).Width;

                    ((StackLayout)theImage.Parent).WidthRequest = ((StackLayout)theImage.Parent).Width * 2;
                    ((StackLayout)theImage.Parent).HeightRequest = ((StackLayout)theImage.Parent).Height * 2;

                    ((ScrollView)theImage.Parent.Parent).ScrollToAsync(0, 0, true);

                    // scrollview should keep width
                    ((ScrollView)theImage.Parent.Parent).WidthRequest = origWidth;

                    theImage.Aspect = Aspect.AspectFit;
                    ImageZoomed = true;
                }
                
            };
            
            teeImage.GestureRecognizers.Add(tapGestureRecognizer);

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
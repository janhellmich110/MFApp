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

        private bool openedModal { get; set; }

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

            InitTeeInfoPage();
        }

        private async void InitTeeInfoPage()
        {
            teeInfoViewModel = new TeeInfoViewModel(golfClubId, teeNumber);
            await teeInfoViewModel.InitTeeInfos();
            teeInfoViewModel.Name = "Birdiebook";
            Init();

            this.BindingContext = teeInfoViewModel;
        }

        protected override void OnAppearing()
        {
            openedModal = (Navigation.ModalStack.Count() > 0) ? true : false;
            InitTeePicker();
            BackButton.IsVisible = openedModal;

            base.OnAppearing();
        }

        private async void InitTeePicker()
        {
            try
            {
                if (teeInfoViewModel == null)
                    return;

                var teeNames = await teeInfoViewModel.GetTeeNameList();
                if (teeNames == null)
                {
                    TeePicker.IsEnabled = false;
                    return;
                }

                TeePicker.ItemsSource = teeNames;

                TeePicker.SelectedIndexChanged += TeePicker_SelectedIndexChanged;

                if (!openedModal)
                {
                    TeePicker.SelectedItem = teeNames.FirstOrDefault();
                    TeePicker.IsEnabled = true;
                }
                else
                {
                    var tee = teeNames.Where(t => t.StartsWith(teeNumber.ToString() + ":")).FirstOrDefault();
                    TeePicker.SelectedItem = tee;
                }

            }
            catch (Exception ex)
            {
                CrashTracker.Track(ex);
                throw;
            }
        }

        private async void Init()
        {
            Image teeImage = (Image)this.FindByName("TeeImage");
            teeImage.Source = teeInfoViewModel.ImageName;

            // set teeinfo
            Label Holenumber = (Label)this.FindByName("HoleNumber");
            Label Par = (Label)this.FindByName("Par");
            Label Hdcp = (Label)this.FindByName("Hdcp");
            Label Distance_Yellow = (Label)this.FindByName("Distance_Yellow");
            Label Distance_Red = (Label)this.FindByName("Distance_Red");
            Label HoleDesciption = (Label)this.FindByName("HoleDescription");

            //Get description from teeinfo
            string holeDescriptionText = "";

            IDataStore<TeeInfo> DataStore = DependencyService.Get<IDataStore<TeeInfo>>();
            List<TeeInfo> TeeInfos = (await DataStore.GetItemsAsync()).ToList();

            TeeInfos = TeeInfos.Where(x => x.GolfClubId == golfClubId).Where(y => y.TeeNummer == teeNumber).OrderBy(z => z.TeeNummer).ToList();

            if (TeeInfos.Count() > 0)
            {
                holeDescriptionText = TeeInfos[0].Description;
            }

            // get tee
            IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
            List<Course> Courses = (await DataStoreCourse.GetItemsAsync()).ToList();

            IDataStore<Tee> DataStoreTee = DependencyService.Get<IDataStore<Tee>>();
            List<Tee> Tees = (await DataStoreTee.GetItemsAsync()).ToList();

            Tee currentTee;
            foreach (Course c in Courses.Where(x => x.GolfclubId == golfClubId))
            {
                currentTee = Tees.Where(x => x.CourseId == c.Id).Where(y => y.Name == teeNumber).FirstOrDefault();
                if (currentTee != null)
                {
                    Holenumber.Text = currentTee.Name.ToString();
                    Par.Text = currentTee.Par.ToString();
                    Hdcp.Text = currentTee.Hcp.ToString();
                    Distance_Yellow.Text = currentTee.Length.ToString();
                    Distance_Red.Text = currentTee.LengthRed.ToString();
                    HoleDesciption.Text = holeDescriptionText;
                    break;
                }
            }

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                // cast to an image
                Image theImage = (Image)sender;

                if (ImageZoomed)
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

            // reset zoom if hole is selected
            if (ImageZoomed)
            {
                ((StackLayout)TeeImage.Parent).WidthRequest = ((StackLayout)TeeImage.Parent).Width / 2;
                ((StackLayout)TeeImage.Parent).HeightRequest = ((StackLayout)TeeImage.Parent).Height / 2;

                TeeImage.Aspect = Aspect.AspectFit;
                ImageZoomed = false;
            }

            if (teeImage.GestureRecognizers.Count() == 0)
                teeImage.GestureRecognizers.Add(tapGestureRecognizer);

            GetLocalisationData();

        }

        private async void GetLocalisationData()
        {
            var result = await teeInfoViewModel.LoadAllPlaces();

            if (result)
            {
                CollectionView AllPlaces = (CollectionView)this.FindByName("AllPlacesCollectionView");
                AllPlaces.BindingContext = null;

                AllPlaces.BindingContext = teeInfoViewModel;
            }
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
            TeePicker.SelectedIndexChanged -= TeePicker_SelectedIndexChanged;
        }

        private async void TeePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            string value = picker.SelectedItem.ToString();
            int i = value.IndexOf(':');
            teeNumber = Convert.ToInt32(value.Substring(0, i));

            teeInfoViewModel.TeeNumber = teeNumber;
            await teeInfoViewModel.InitTeeInfos();
            Init();
        }
    }
}
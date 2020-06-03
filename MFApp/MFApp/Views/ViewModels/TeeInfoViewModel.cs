using System;
using System.Windows.Input;
using Xamarin.Forms;
using MFApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using MFApp.Services;
using System.Linq;
using Xamarin.Essentials;
using System.Runtime.CompilerServices;

namespace MFApp.ViewModels
{
    public class TeeInfoViewModel : INotifyPropertyChanged
    {
        public TeeInfoViewModel()
        {
        }

        public TeeInfoViewModel(int GolfClubId, int TeeNumber)
        {
            GolfclubId = GolfClubId;
            TeeNummer = TeeNumber;
            ImageName = "lochbild" + GolfClubId.ToString() + "_" + TeeNumber.ToString();
            //ImageName = "mf.png";
        }

        bool isRefreshingAllPlaces;

        public bool IsRefreshingAllPlaces
        {
            get { return isRefreshingAllPlaces; }
            set
            {
                isRefreshingAllPlaces = value;
                OnPropertyChanged();
            }
        }

        public int GolfclubId { get; set; }
        public int TeeNummer { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageName { get; set; }

        public ObservableCollection<TeePlace> TeePlaces { get; set; }

        public Command LoadAllPlacesCommand => new Command(async () => await ExecuteLoadAllPlacesCommand());

        async Task ExecuteLoadAllPlacesCommand()
        {
            IsRefreshingAllPlaces = true;
            try
            {
                TeePlaces.Clear();
                await LoadAllPlaces();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsRefreshingAllPlaces = false;
            }
        }

        public async Task<bool> LoadAllPlaces()
        {
            // get infos from db
            IDataStore<TeeInfo> DataStore = DependencyService.Get<IDataStore<TeeInfo>>();
            List<TeeInfo> TeeInfos = (await DataStore.GetItemsAsync()).ToList();

            TeeInfos = TeeInfos.Where(x => x.GolfClubId == GolfclubId).Where(y => y.TeeNummer == TeeNummer).ToList();

            // set text and description
            var FirstTee = TeeInfos.FirstOrDefault();
            if (FirstTee != null)
            {
                Name = FirstTee.TeeNummer.ToString() + ": " + FirstTee.TeeName;
                Description = FirstTee.Description;
            }

            // get current location, 3 times as result is better ???
            var request = new GeolocationRequest(GeolocationAccuracy.High, new TimeSpan(0, 0, 10));
            var location = await Geolocation.GetLocationAsync(request);
            await Task.Delay(1000);
            request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 10));
            location = await Geolocation.GetLocationAsync(request);
            await Task.Delay(1000);
            request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 10));
            location = await Geolocation.GetLocationAsync(request);

            List<TeePlace> TPList = new List<TeePlace>();
            // fill teeplaces list
            foreach (TeeInfo ti in TeeInfos)
            {
                TeePlace tp = new TeePlace();
                tp.Text = ti.TeeInfoName;
                tp.Type = ti.TeeInfoType;
                // calculate distance from current location
                double DistanceKM = Location.CalculateDistance(location, ti.Latitude, ti.Longitude, DistanceUnits.Kilometers);
                tp.Distance = (int)(DistanceKM * 1000);
                TPList.Add(tp);
            }
            TeePlaces = new ObservableCollection<TeePlace>(TPList.OrderBy(x => x.Text));
            return await Task.FromResult(true);
        }
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class TeePlace
    {
        public string Text { get; set; }
        public int Distance { get; set; }
        public TeeInfoTypeEnum Type { get; set; }
    }
}
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

using Plugin.Geolocator;

namespace MFApp.ViewModels
{
    public class TeeInfoViewModel : INotifyPropertyChanged
    {
        public TeeInfoViewModel()
        {
        }

        public TeeInfoViewModel(int golfClubId, int teeNumber)
        {
            GolfclubId = golfClubId;
            TeeNumber = teeNumber;
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
        public int TeeNumber { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageName { get; set; }

        public ObservableCollection<TeePlace> TeePlaces { get; set; }

        public Command LoadAllPlacesCommand => new Command(async () => await ExecuteLoadAllPlacesCommand());

        private List<TeeInfo> TeeInfos { get; set; }

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
                CrashTracker.Track(ex);
                Debug.WriteLine(ex);
            }
            finally
            {
                IsRefreshingAllPlaces = false;
            }
        }

        public async Task InitTeeInfos()
        {
            ImageName = "lochbild" + GolfclubId.ToString() + "_" + TeeNumber.ToString();
            // get infos from db
            IDataStore<TeeInfo> DataStore = DependencyService.Get<IDataStore<TeeInfo>>();
            TeeInfos = (await DataStore.GetItemsAsync()).ToList();

            TeeInfos = TeeInfos.Where(x => x.GolfClubId == GolfclubId).Where(y => y.TeeNummer == TeeNumber).ToList();

            // set text and description
            var FirstTee = TeeInfos.FirstOrDefault();
            if (FirstTee != null)
            {
                Name = FirstTee.TeeNummer.ToString() + ": " + FirstTee.TeeName;
                Description = FirstTee.Description;
            }
        }

        public async Task<bool> LoadAllPlaces()
        {
            // get current location, 2 times as result is better ???
            try
            {
                //var request = new GeolocationRequest(GeolocationAccuracy.High, new TimeSpan(0, 0, 4));
                //var location = await Geolocation.GetLocationAsync(request);
                //await Task.Delay(1000);

                List<TeePlace> TPList = new List<TeePlace>();
                CrossGeolocator.Current.DesiredAccuracy = 1;
                if (IsLocationAvailable())
                {
                    //var request = new GeolocationRequest(GeolocationAccuracy.Best, new TimeSpan(0, 0, 30));
                    //var location = await Geolocation.GetLocationAsync(request);
                    Plugin.Geolocator.Abstractions.Position location = null;
                    try
                    {
                        location = await CrossGeolocator.Current.GetPositionAsync(new TimeSpan(0, 0, 2));
                    }
                    catch(Exception ex)
                    {
                        location = null;
                    }

                    if (location != null)
                    {
                        // fill teeplaces list
                        foreach (TeeInfo ti in TeeInfos)
                        {
                            TeePlace tp = new TeePlace();
                            tp.Text = ti.TeeInfoName;
                            tp.Type = ti.TeeInfoType;
                            // calculate distance from current location
                            double DistanceKM = Location.CalculateDistance(location.Latitude, location.Longitude, ti.Latitude, ti.Longitude, DistanceUnits.Kilometers);
                            tp.Distance = (int)(DistanceKM * 1000);
                            //if (tp.Distance < 600)
                                TPList.Add(tp);
                        }
                    }
                }
                TeePlaces = new ObservableCollection<TeePlace>(TPList.OrderBy(x => x.Text));
            }
            catch (Exception ex)
            {
                CrashTracker.Track(ex);
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        public async Task<List<string>> GetTeeNameList()
        {
            try
            {
                IDataStore<TeeInfo> DataStore = DependencyService.Get<IDataStore<TeeInfo>>();
                List<TeeInfo> teeinfos = (await DataStore.GetItemsAsync()).ToList();

                var tees = teeinfos.Where(x => x.GolfClubId == GolfclubId).OrderBy(n => n.TeeNummer).Select(y => new { y.TeeName, y.TeeNummer }).Distinct();

                if (tees == null || tees?.Count() == 0)
                    return null;

                List<string> result = new List<string>();

                foreach (var t in tees)
                {
                    string teename = string.Concat(t.TeeNummer, ": ", t.TeeName);
                    result.Add(teename);
                }

                return result;
            }
            catch (Exception ex)
            {
                CrashTracker.Track(ex);
                return null;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        public async Task<Plugin.Geolocator.Abstractions.Position> GetGeoLocation()
        {
            return await CrossGeolocator.Current.GetPositionAsync();
        }
    }

    public class TeePlace
    {
        public string Text { get; set; }
        public int Distance { get; set; }
        public TeeInfoTypeEnum Type { get; set; }
    }
}
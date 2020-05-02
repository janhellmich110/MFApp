using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

using Xamarin.Forms;
using MFApp.Services;
using MFApp.Models;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MFApp.ViewModels
{
    public class ResultPageData : INotifyPropertyChanged
    {
        MFWebDataSync DataSync = new MFWebDataSync();

        bool isRefreshing;

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MFAppFullTournamentResult> PlayerResults { get; set; }

        public Command LoadResultsCommand => new Command(async () => await ExecuteLoadResultsCommand());

        public ResultPageData()
        {
            PlayerResults = new ObservableCollection<MFAppFullTournamentResult>();
            LoadPlayerResults();
        }

        async Task ExecuteLoadResultsCommand()
        {
            IsRefreshing = true;
            try
            {
                PlayerResults.Clear();
                await LoadPlayerResults();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        async Task<bool> LoadPlayerResults()
        {
            IEnumerable<MFAppFullTournamentResult> pResults = await DataSync.GetLastResults();
            PlayerResults = new ObservableCollection<MFAppFullTournamentResult>(pResults);
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


}

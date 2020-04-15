using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Models;
using MFApp.Services;

namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage : ContentPage
    {
        ResultPageData ResultData;
        public ResultPage()
        {
            InitializeComponent();
            
            ResultData = new ResultPageData();
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            IsBusy = true;

            MFWebDataSync DataSync = new MFWebDataSync();
            IEnumerable<MFAppFullTournamentResult> pResults = await DataSync.GetLastResults();

            ResultData.PlayerResults = pResults.ToList();

            this.BindingContext = ResultData;

            IsBusy = false;
        }
    }
}
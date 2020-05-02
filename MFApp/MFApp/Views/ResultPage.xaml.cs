using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.ViewModels;
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = ResultData = new ResultPageData();
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            BindingContext = ResultData = new ResultPageData();
            ResultData.IsRefreshing = false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MFApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "Startseite";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://demo.portivity.de/mfweb"));
        }

        public ICommand OpenWebCommand { get; }
    }
}

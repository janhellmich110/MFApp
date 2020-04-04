using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MFApp.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        public ConfigurationViewModel()
        {
            Title = "Einstellungen";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://demo.portivity.de/mfweb"));
        }

        public ICommand OpenWebCommand { get; }
    }
}
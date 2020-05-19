using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace MFApp.ViewModels
{
    public class MapsViewModel : BaseViewModel
    {
        public MapsViewModel()
        {
            Title = "Einstellungen";
        }

        public string Address { get; set; }
        public string PlaceName { get; set; }

        public Location Position { get; set; }        
    }
}
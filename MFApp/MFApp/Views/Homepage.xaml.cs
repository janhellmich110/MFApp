using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Services;
using MFApp.Models;

namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {

        public HomePageData HomePageEvents { get; set; }
        public HomePage()
        {
            InitializeComponent();

            HomePageEvents = new HomePageData();

            IDataStore<Event> DataStore = DependencyService.Get<IDataStore<Event>>();
            var EventTask = DataStore.GetItemsAsync();
            List<Event> Events = EventTask.Result.ToList();

            foreach(Event e in Events)
            {
                HomePageEvents.Events.Add(e);
            }
            BindingContext = HomePageEvents;

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var Event = (Event)layout.BindingContext;

            // get Tournament
            IDataStore<Tournament> DataStore = DependencyService.Get<IDataStore<Tournament>>();
            var TournamentTask = DataStore.GetItemsAsync();
            List<Tournament> Tournaments = TournamentTask.Result.ToList();

            foreach (Tournament t in Tournaments)
            {
                if(t.EventId == Event.Id)
                {
                    Navigation.PushAsync(new TournamentPage(t));
                    break;
                }
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TournamentPage());
        }


    }
}
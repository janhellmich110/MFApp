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

            IDataStore<Golfclub> DataStore = DependencyService.Get<IDataStore<Golfclub>>();
            var GolfclubTask = DataStore.GetItemsAsync();
            List<Golfclub> Golfclubs = GolfclubTask.Result.ToList();

            IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
            var EventTask = DataStoreEvent.GetItemsAsync();
            List<Event> Events = EventTask.Result.ToList();

            IDataStore<Tournament> DataStoreTournament = DependencyService.Get<IDataStore<Tournament>>();
            var TournamentTask = DataStoreTournament.GetItemsAsync();
            List<Tournament> Tournaments = TournamentTask.Result.ToList();

            foreach (Tournament t in Tournaments)
            {
                HomePageEvent hpe = new HomePageEvent();

                Event e = (Event)Events.Where(x => x.Id == t.EventId).FirstOrDefault();

                Golfclub gc = Golfclubs.Where(x => x.Id == e.GolfclubId).FirstOrDefault();

                if (gc != null)
                    hpe.EventClub = Golfclubs.Where(x => x.Id == e.GolfclubId).Select(y => y.Name).First();
                else
                    hpe.EventClub = "";

                hpe.EventDate = t.Datum.ToString("dd.MM.yyy hh:mm");
                hpe.EventName = e.Name;
                hpe.TournamentName = t.Name;
                hpe.EventTournament = t;

                HomePageEvents.Events.Add(hpe);
            }
            BindingContext = HomePageEvents;

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var homepageEvent = (HomePageEvent)layout.BindingContext;
            
            Navigation.PushAsync(new TournamentPage(homepageEvent.EventTournament));
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TournamentPage());
        }


    }
}
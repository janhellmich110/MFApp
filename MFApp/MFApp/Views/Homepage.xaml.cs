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

            

        }


        private void Button_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var homepageEvent = (HomePageEvent)layout.BindingContext;

            if ((homepageEvent != null) && (homepageEvent.EventTournament != null))
                Navigation.PushAsync(new TournamentPage(homepageEvent.EventTournament));
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var homepageEvent = (HomePageEvent)layout.BindingContext;

            if ((homepageEvent != null) && (homepageEvent.EventTournament != null))
                Navigation.PushAsync(new TournamentPage(homepageEvent.EventTournament));
        }

        private void AdHoc_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new AdhocTournament()));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
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

            Tournaments = Tournaments.Where(d => d.Datum >= DateTime.Today).OrderBy(x => x.Datum).ToList();

            foreach (Tournament t in Tournaments)
            {
                HomePageEvent hpe = new HomePageEvent();

                Event ev = (Event)Events.Where(x => x.Id == t.EventId).FirstOrDefault();

                Golfclub gc = Golfclubs.Where(x => x.Id == ev.GolfclubId).FirstOrDefault();

                if (gc != null)
                    hpe.EventClub = Golfclubs.Where(x => x.Id == ev.GolfclubId).Select(y => y.Name).First();
                else
                    hpe.EventClub = "";

                hpe.EventDate = t.Datum.ToString("dd.MM.yyy hh:mm");
                hpe.EventName = ev.Name;
                hpe.TournamentName = t.Name;
                hpe.EventTournament = t;
                hpe.BackColor = "#90EE90";

                //check if already tournament results
                hpe.ButtonText = "Runde starten";
                IDataStore<Result> DataStoreResults = DependencyService.Get<IDataStore<Result>>();
                var ResultTask = DataStoreResults.GetItemsAsync();
                List<Result> Results = ResultTask.Result.ToList();
                List<Result> tResults = Results.Where(x => x.TournamentId == t.Id).ToList();
                if (tResults.Count > 0)
                    hpe.ButtonText = "Runde fortsetzen";

                if (ev.EventType == EventTypeEnum.Event)
                {
                    hpe.BackColor = "#FF9966";
                }
                if (t.Id >= 1000000)
                {
                    hpe.BackColor = "#CCFFFF";
                }

                HomePageEvents.Events.Add(hpe);
            }
            BindingContext = HomePageEvents;
        }
    }
}
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

        IDataStore<Golfclub> DataStoreGolfclub = DependencyService.Get<IDataStore<Golfclub>>();
        IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
        IDataStore<Tournament> DataStoreTournament = DependencyService.Get<IDataStore<Tournament>>();
        IDataStore<Result> DataStoreResults = DependencyService.Get<IDataStore<Result>>();

        public HomePage()
        {
            InitializeComponent();
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            HomePageEvents = new HomePageData();

            List<Tournament> Tournaments = (await DataStoreTournament.GetItemsAsync()).ToList();

            Tournaments = Tournaments.Where(d => d.Datum >= DateTime.Today).OrderBy(x => x.Datum).ToList();

            foreach (Tournament t in Tournaments)
            {
                HomePageEvent hpe = new HomePageEvent();

                Event ev = await DataStoreEvent.GetItemAsync(t.EventId);
                Golfclub gc = await DataStoreGolfclub.GetItemAsync(ev.GolfclubId);

                if (gc != null)
                    hpe.EventClub = gc.Name;
                else
                    hpe.EventClub = "";

                hpe.EventDate = t.Datum.ToString("dd.MM.yyy hh:mm");
                hpe.EventName = ev.Name;
                hpe.TournamentName = t.Name;
                hpe.EventTournament = t;
                hpe.BackColor = "#90EE90";

                //check if already tournament results
                hpe.ButtonText = "Runde starten";

                List<Result> Results = (await DataStoreResults.GetItemsAsync()).ToList();
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

        
    }
}
using MFApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Services;

namespace MFApp.Views
{    

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdhocTournament : ContentPage
    {
        public AdhocView AdhocModel { get; set; }

        IDataStore<Golfclub> DataStoreGolfclub = DependencyService.Get<IDataStore<Golfclub>>();
        IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
        IDataStore<Tournament> DataStoreTournament = DependencyService.Get<IDataStore<Tournament>>();
        IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
        IDataStore<CourseHandicapTable> DataStoreHdcpTable = DependencyService.Get<IDataStore<CourseHandicapTable>>();

        public AdhocTournament()
        {
            InitializeComponent();
            
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            // check for adhoc round today
            List<Tournament> Tournaments = (await DataStoreTournament.GetItemsAsync()).ToList();
            Tournament AdhocToday = Tournaments.Where(d => d.Datum.Date == DateTime.Today).Where(x => x.Name.ToLower().StartsWith("runde:")).FirstOrDefault();

            AdhocModel = new AdhocView();

            AdhocModel.AllClubs = (await DataStoreGolfclub.GetItemsAsync()).ToList();

            if (AdhocToday != null)
            {
                Event ev = await DataStoreEvent.GetItemAsync(AdhocToday.EventId);
                if((ev != null)&&(AdhocModel.AllClubs.Where(x=>x.Id == ev.GolfclubId).FirstOrDefault() != null))
                {
                    AdhocModel.SelectedClub = AdhocModel.AllClubs.Where(x => x.Id == ev.GolfclubId).FirstOrDefault();
                }
            }

            if ((AdhocModel.SelectedClub == null) && (AdhocModel.AllClubs.Count > 0))
                AdhocModel.SelectedClub = AdhocModel.AllClubs[0];

            this.BindingContext = AdhocModel;
        }

        private async void ListClubs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            // get course picker
            Picker coursePicker = (Picker)this.FindByName("ListCourses");
            //coursePicker.Items.Clear();
            coursePicker.ItemsSource = null;

            // update courses
            AdhocModel.AllCourses.Clear();

            List<Course> AllCourses = (await DataStoreCourse.GetItemsAsync()).ToList();

            foreach (Course c in AllCourses)
            {
                if (c.GolfclubId == AdhocModel.SelectedClub.Id)
                {
                    AdhocModel.AllCourses.Add(c);
                }
            }

            // check for adhoc round today
            List<Tournament> Tournaments = (await DataStoreTournament.GetItemsAsync()).ToList();
            Tournament AdhocToday = Tournaments.Where(d => d.Datum.Date == DateTime.Today).Where(x => x.Name.ToLower().StartsWith("runde:")).FirstOrDefault();
            if (AdhocToday != null)
            {
                Course c = AllCourses.Where(x => x.Id == AdhocToday.CourseId).FirstOrDefault();
                if (c != null)
                {
                    AdhocModel.SelectedCourse = c;

                    bool withPutts = AdhocToday.WithPutts;
                    // set also withputts
                    Picker PuttPicker = (Picker)this.FindByName("WithPutts");
                    if (withPutts)
                        PuttPicker.SelectedItem = "Mit Putts";
                    else
                        PuttPicker.SelectedItem = "Ohne Putts";

                }
            }

            coursePicker.ItemsSource = AdhocModel.AllCourses;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if ((AdhocModel.SelectedClub != null) && (AdhocModel.SelectedCourse != null))
            {
                bool WithPutts = false;
                try
                {
                    Picker PuttPicker = (Picker)this.FindByName("WithPutts");
                    if (PuttPicker.SelectedItem.ToString() == "Mit Putts")
                    {
                        WithPutts = true;
                    }
                }
                catch (Exception)
                { }

                string tournamentName = "Runde: " + DateTime.Today.ToShortDateString();
                int courseId = AdhocModel.SelectedCourse.Id;
                int clubId = AdhocModel.SelectedClub.Id;

                // check existing tournament
                var existingTournament = (await DataStoreTournament.GetItemsAsync()).Where(x=>x.Name == tournamentName).Where(d=>d.Datum.Date==DateTime.Today).FirstOrDefault();

                if((existingTournament != null) && (existingTournament.CourseId == courseId))
                {
                    if(existingTournament.WithPutts != WithPutts)
                    {
                        existingTournament.WithPutts = WithPutts;
                        await DataStoreTournament.UpdateItemAsync(existingTournament);
                    }

                    await Navigation.PushAsync(new TournamentPage(existingTournament));
                    return;
                }

                // get handicaptables
                List<CourseHandicapTable> tables = (await DataStoreHdcpTable.GetItemsAsync()).ToList();

                int tableIdMale = 0;
                int tableIdFemale = 0;
                foreach (CourseHandicapTable cht in tables)
                {
                    if ((cht.CourseId == AdhocModel.SelectedCourse.Id) && (cht.TeeGender == Gender.Mann))
                        tableIdMale = cht.Id;
                    if ((cht.CourseId == AdhocModel.SelectedCourse.Id) && (cht.TeeGender == Gender.Frau))
                        tableIdFemale = cht.Id;
                }

                Event ev = new Event
                {
                    Id=0,
                    Name = "Eigene Runde",
                    EventDate = DateTime.Now,
                    EventType = EventTypeEnum.AppEvent,
                    GolfclubId = clubId
                };

                await DataStoreEvent.AddItemAsync(ev);

                int newEventId = (await DataStoreEvent.GetItemsAsync()).OrderByDescending(x => x.Id).First().Id;

                Tournament t = new Tournament
                {
                    Id=0,
                    EventId = newEventId,
                    Name = tournamentName,
                    Datum = DateTime.Now,
                    CourseId = courseId,
                    ManagedFlights = false,
                    WithPutts = WithPutts,
                    HandicapTableFemaleId = tableIdFemale,
                    HandicapTableMaleId = tableIdMale
                };
                
                await DataStoreTournament.AddItemAsync(t);
                int newTournamentId = (await DataStoreTournament.GetItemsAsync()).OrderByDescending(x => x.Id).First().Id;

                t.Id = newTournamentId;

                await Navigation.PushAsync(new TournamentPage(t));
            }
        }

        async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }        
    }

    public class AdhocView
    {
        public AdhocView()
        {
            AllClubs = new List<Golfclub>();
            AllCourses = new List<Course>();
        }
        public List<Golfclub> AllClubs { get; set; }
        public Golfclub SelectedClub { get; set; }
        public List<Course> AllCourses { get; set; }
        public Course SelectedCourse { get; set; }
    }
}
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

        public AdhocTournament()
        {
            InitializeComponent();
            AdhocModel = new AdhocView();
            IDataStore<Golfclub> DataStore = DependencyService.Get<IDataStore<Golfclub>>();
            var GolfclubTask = DataStore.GetItemsAsync();
            AdhocModel.AllClubs = GolfclubTask.Result.ToList();

            foreach(Golfclub gc in AdhocModel.AllClubs)
            {
                AdhocModel.SelectedClub = gc;
                break;
            }            
            
            this.BindingContext = AdhocModel;
        }

        private void ListClubs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update courses
            AdhocModel.AllCourses.Clear();

            IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
            var CourseTask = DataStoreCourse.GetItemsAsync();
            List<Course> AllCourses = CourseTask.Result.ToList();

            foreach (Course c in AllCourses)
            {
                if (c.GolfclubId == AdhocModel.SelectedClub.Id)
                {
                    AdhocModel.AllCourses.Add(c);
                }
            }
            this.BindingContext = AdhocModel;
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
                IDataStore<Tournament> DataStoreTournament = DependencyService.Get<IDataStore<Tournament>>();
                var existingTournament = (await DataStoreTournament.GetItemsAsync()).Where(x=>x.Name == tournamentName).Where(d=>d.Datum.Date==DateTime.Today).FirstOrDefault();

                if((existingTournament != null) && (existingTournament.CourseId == courseId))
                {
                    if(existingTournament.WithPutts != WithPutts)
                    {
                        existingTournament.WithPutts = WithPutts;
                        await DataStoreTournament.UpdateItemAsync(existingTournament);
                    }

                    Navigation.PushAsync(new TournamentPage(existingTournament));
                    return;
                }

                // get handicaptables
                IDataStore<CourseHandicapTable> DataStore = DependencyService.Get<IDataStore<CourseHandicapTable>>();
                var CourseHandicapTableTask = DataStore.GetItemsAsync();
                List<CourseHandicapTable> tables = CourseHandicapTableTask.Result.ToList();

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
                IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
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

                Navigation.PushAsync(new TournamentPage(t));
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
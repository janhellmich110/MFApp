using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;

using MFApp.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MFApp.Models
{
    public class TournamentPageData : INotifyPropertyChanged
    {
        bool isRefreshingAllResults;

        public bool IsRefreshingAllResults
        {
            get { return isRefreshingAllResults; }
            set
            {
                isRefreshingAllResults = value;
                OnPropertyChanged();
            }
        }

        public TournamentPageData()
        {
            AllPlayers = new ObservableCollection<TournamentPlayer>();
            SelectedPlayers = new ObservableCollection<TournamentPlayer>();
        }
        public TournamentPageData(Tournament tournament)
        {
            PlayerResults = new ObservableCollection<TournamentResultSummary>();
            AllPlayerResults = new ObservableCollection<MFAppFullTournamentResult>();

            Tournament = tournament;
            TournamentDate = tournament.Datum.ToString("dd.MM.yyyy hh:mm");
            // get event
            // get Tournament
            IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
            var EventTask = DataStoreEvent.GetItemsAsync();
            List<Event> Events = EventTask.Result.ToList();

            TournamentEvent = null;
            foreach (Event e in Events)
            {
                if (e.Id == tournament.EventId)
                {
                    TournamentEvent = e;
                    break;
                }
            }

            // get club
            IDataStore<Golfclub> DataStoreGolfclub = DependencyService.Get<IDataStore<Golfclub>>();
            var GolfclubTask = DataStoreGolfclub.GetItemsAsync();
            List<Golfclub> Golfclubs = GolfclubTask.Result.ToList();

            TournamentClub = Golfclubs.Where(x => x.Id == TournamentEvent.GolfclubId).FirstOrDefault();

            // get course
            IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
            var CourseTask = DataStoreCourse.GetItemsAsync();
            List<Course> Courses = CourseTask.Result.ToList();

            foreach (Course c in Courses)
            {
                if (Tournament.CourseId == c.Id)
                {
                    Course = c;
                    break;
                }
            }

            // get list of tees
            IDataStore<Tee> DataStoreTee = DependencyService.Get<IDataStore<Tee>>();
            var TeeTask = DataStoreTee.GetItemsAsync();
            List<Tee> Tees = TeeTask.Result.ToList();

            TeeList = Tees.Where(x => x.CourseId == Course.Id).OrderBy(y => y.Name).ToList();

            AllPlayers = new ObservableCollection<TournamentPlayer>();
            SelectedPlayers = new ObservableCollection<TournamentPlayer>();

            // get current player
            Profile CurrentProfile = null;

            IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();
            var profilesTask = DataStoreProfile.GetItemsAsync();
            var profiles = profilesTask.Result;

            foreach (Profile p in profiles)
            {
                CurrentProfile = p;
                break;
            }

            // find existing flight
            List<int> FlightPlayerIds = new List<int>();

            IDataStore<Flight> DataStoreFlight = DependencyService.Get<IDataStore<Flight>>();
            var FlightTask = DataStoreFlight.GetItemsAsync();
            List<Flight> Flights = FlightTask.Result.ToList();

            IDataStore<Flight2Player> DataStoreFlight2Player = DependencyService.Get<IDataStore<Flight2Player>>();
            var Flight2PlayerTask = DataStoreFlight2Player.GetItemsAsync();
            List<Flight2Player> Flight2Players = Flight2PlayerTask.Result.ToList();

            if (!Tournament.ManagedFlights)
            {
                int flightNumber = Tournament.Id * 10000 + CurrentProfile.Id;

                FlightPlayerIds = Flight2Players.Where(x => x.FlightId == flightNumber).Select(x => x.PlayerId).ToList();

                if (FlightPlayerIds.Count() == 0)
                {
                    // no player in flight, add current player
                    FlightPlayerIds.Add(CurrentProfile.Id);
                }
                TournamentFlight = "Mein Flight";
            }
            else
            {
                // get my flight number
                List<Flight> flightList = Flights.Where(x => x.TournamentId == Tournament.Id).ToList();
                foreach(Flight f in flightList)
                {
                    // check if I am in flight
                    var flightPlayer = Flight2Players.Where(x => x.FlightId == f.Id).Where(y => y.PlayerId == CurrentProfile.Id).FirstOrDefault();
                    if(flightPlayer != null)
                    {
                        // flight found
                        TournamentFlight = "Flight " + f.FlightNumber.ToString() + ": " + f.FlightName;
                        FlightPlayerIds = Flight2Players.Where(x => x.FlightId == f.Id).Select(x => x.PlayerId).ToList();
                        break;
                    }
                    else
                    {
                        // no flight found
                        TournamentFlight = "Aktuell noch kein Flight!";
                    }
                }
            }

            // Fill All Players
            IDataStore<Player> DataStore = DependencyService.Get<IDataStore<Player>>();
            var PlayerTask = DataStore.GetItemsAsync();
            List<Player> players = PlayerTask.Result.ToList();

            players = players.OrderBy(x => x.Name).ToList();

            foreach (Player p in players)
            {
                TournamentPlayer tp = new TournamentPlayer
                {
                    Id=p.Id,
                    UserName=p.UserName,
                    Name=p.Name,
                    Initials=p.Initials,
                    Handicap=p.Handicap,
                    DGVHandicap=p.DGVHandicap,
                    Mail=p.Mail,
                    Gender=p.Gender,
                    CourseHandicap = (int)p.Handicap,
                    Selected=false
                };

                tp.CourseHandicap = GetCourseHandicap(tp, Tournament);

                if (TournamentEvent.HandicapType == HandicapTypeEnum.DGVHandicap)
                    tp.Handicap = p.DGVHandicap;

                if(CurrentProfile.UserName.ToLower() == tp.UserName.ToLower())
                {
                    CurrentPlayer = p;
                }

                if (FlightPlayerIds.Count() > 0)
                {
                    foreach(int i in FlightPlayerIds)
                    {
                        if(i==tp.Id)
                        {
                            tp.Selected = true;
                            // add to selected players
                            SelectedPlayers.Add(tp);
                            break;
                        }
                    }
                }

                AllPlayers.Add(tp);
            }

            SelectedPlayers = SortFlightPlayer(SelectedPlayers);
        }

        public Player CurrentPlayer { get; set; }

        public Golfclub TournamentClub { get; set; }

        public Event TournamentEvent { get; set; }

        public Tournament Tournament { get; set; }

        public string TournamentDate { get; set; }

        public string TournamentFlight { get; set; }
        public Course Course { get; set; }

        public List<Tee> TeeList { get; set; }

        public ObservableCollection<TournamentPlayer> AllPlayers{ get; set;}

        public ObservableCollection<TournamentPlayer> SelectedPlayers { get; set;}

        public ObservableCollection<TournamentResultSummary> PlayerResults { get; set; }
        public ObservableCollection<MFAppFullTournamentResult> AllPlayerResults { get; set; }

        private int GetCourseHandicap(TournamentPlayer p, Tournament t)
        {
            int TournamentHandicapId = 0;


            if (p.Gender == Gender.Mann)
            {
                TournamentHandicapId = t.HandicapTableMaleId;
            }
            else
            {
                TournamentHandicapId = t.HandicapTableFemaleId;
            }

            IDataStore<CourseHandicapTable> DataStore = DependencyService.Get<IDataStore<CourseHandicapTable>>();
            var CourseHandicapTask = DataStore.GetItemsAsync();
            List<CourseHandicapTable> CourseHandicaps = CourseHandicapTask.Result.ToList();

            CourseHandicapTable currentTable = CourseHandicaps.Where(x => x.Id == TournamentHandicapId).FirstOrDefault();

            double currentHdcp = p.Handicap;

            try
            {                
                if (TournamentEvent.HandicapType == HandicapTypeEnum.DGVHandicap)
                    currentHdcp = p.DGVHandicap;

                if (currentHdcp <= 36)
                {                    
                    // calculate course handicap, take negativ hdcp to use old formula                    
                    if (currentHdcp > 0)
                        currentHdcp *= -1;

                    int currentCourseHdcp = (int)Math.Round((((double)currentHdcp * currentTable.Slope / ((double)113)) - (double)currentTable.CR + (double)currentTable.Par));
                    if (currentCourseHdcp < 0)
                        currentCourseHdcp *= -1;

                    return currentCourseHdcp;
                }
                else
                {
                    currentHdcp = (int)Math.Round(currentHdcp + currentTable.HandicapStrokes);
                }
            }
            catch (Exception)
            {
                return (int)Math.Round(currentHdcp);
            }


            return (int)Math.Round(currentHdcp);

        }

        public ObservableCollection<TournamentPlayer> SortFlightPlayer(ObservableCollection<TournamentPlayer> FlightPlayers)
        {
            ObservableCollection<TournamentPlayer> SortedPlayers = new ObservableCollection<TournamentPlayer>();

            FlightPlayers = new ObservableCollection<TournamentPlayer>(FlightPlayers.OrderBy(x => x.Id));

            // first add current player
            foreach (TournamentPlayer tp in FlightPlayers)
            {
                if (tp.Id == CurrentPlayer.Id)
                    SortedPlayers.Add(tp);
            }

            // then add other players by id
            foreach (TournamentPlayer tp in FlightPlayers)
            {
                if (tp.Id != CurrentPlayer.Id)
                    SortedPlayers.Add(tp);
            }

            return SortedPlayers;
        }

        public Command LoadAllResultsCommand => new Command(async () => await ExecuteLoadAllResultsCommand());

        async Task ExecuteLoadAllResultsCommand()
        {
            IsRefreshingAllResults = true;
            try
            {
                AllPlayerResults.Clear();
                await LoadAllResults();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsRefreshingAllResults = false;
            }
        }

        async Task<bool> LoadAllResults()
        {
            MFWebDataSync DataSync = new MFWebDataSync();
            IEnumerable<MFAppFullTournamentResult> pResults = await DataSync.GetLastResults();
            AllPlayerResults = new ObservableCollection<MFAppFullTournamentResult>(pResults);

            return await Task.FromResult(true);
        }
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }


}

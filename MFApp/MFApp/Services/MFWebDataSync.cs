using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using MFApp.Models;

using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;

namespace MFApp.Services
{
    public class MFWebDataSync
    {
        private SQLiteConnection conn;
        private string dbPathPlayer => FileAccessHelper.GetLocalFilePath("MFApp.db3");

        public MFWebDataSync()
        {
            conn = new SQLiteConnection(dbPathPlayer);
        }

        public async Task<bool> SyncMFWeb()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"MyAppData/GetMyAppData/");
                    MFWebAPIData item = (MFWebAPIData)await Task.Run(() => JsonConvert.DeserializeObject<MFWebAPIData>(json));

                    #region golfclubs
                    try
                    {
                        conn.Execute("DELETE FROM Golfclub");
                    }
                    catch (Exception) { }

                    IDataStore<Golfclub> DataStore = DependencyService.Get<IDataStore<Golfclub>>();
                    foreach (Golfclub golfclub in item.Golfclubs)
                    {
                        DataStore.AddItemAsync(golfclub);
                    }
                    #endregion

                    #region Event
                    try
                    {
                        conn.Execute("DELETE FROM Event");
                    }
                    catch (Exception) { }

                    IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
                    foreach (Event e in item.Events)
                    {
                        DataStoreEvent.AddItemAsync(e);
                    }
                    #endregion

                    #region Tournaments
                    try
                    {
                        conn.Execute("DELETE FROM Tournament");
                    }
                    catch (Exception) { }

                    IDataStore<Tournament> DataStoreTournament = DependencyService.Get<IDataStore<Tournament>>();
                    foreach (Tournament t in item.Tournaments)
                    {
                        DataStoreTournament.AddItemAsync(t);
                    }
                    #endregion

                    #region Course
                    try
                    {
                        conn.Execute("DELETE FROM Course");
                    }
                    catch (Exception) { }

                    IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
                    foreach (Course c in item.Courses)
                    {
                        DataStoreCourse.AddItemAsync(c);
                    }
                    #endregion

                    #region Tee
                    try
                    {
                        conn.Execute("DELETE FROM Tee");
                    }
                    catch (Exception) { }

                    IDataStore<Tee> DataStoreTee = DependencyService.Get<IDataStore<Tee>>();
                    foreach (Tee t in item.Tees)
                    {
                        DataStoreTee.AddItemAsync(t);
                    }
                    #endregion

                    #region TeeInfo
                    try
                    {
                        conn.Execute("DELETE FROM TeeInfo");
                    }
                    catch (Exception) { }

                    IDataStore<TeeInfo> DataStoreTeeinfo = DependencyService.Get<IDataStore<TeeInfo>>();
                    foreach (TeeInfo t in item.TeeInfos)
                    {
                        DataStoreTeeinfo.AddItemAsync(t);
                    }
                    #endregion

                    #region Flight
                    try
                    {
                        conn.Execute("DELETE FROM Flight2Player Where Id < 100000");
                    }
                    catch (Exception) { }
                    try
                    {
                        conn.Execute("DELETE FROM Flight Where Id < 10000");
                    }
                    catch (Exception) { }

                    IDataStore<Flight> DataStoreFlight = DependencyService.Get<IDataStore<Flight>>();
                    IDataStore<Flight2Player> DataStoreFlight2Player = DependencyService.Get<IDataStore<Flight2Player>>();

                    foreach (MFAppFlight t in item.Flights)
                    {
                        Flight f = new Flight()
                        {
                            Id = t.Id,
                            FlightNumber = t.FlightNumber,
                            FlightName = t.FlightName,
                            TournamentId=t.TournamentId
                        };

                        DataStoreFlight.AddItemAsync(f);

                        foreach (Player p in t.Players)
                        {
                            Flight2Player f2p = new Flight2Player()
                            {
                                Id = (t.Id * 10000) + p.Id,
                                FlightId = t.Id,
                                PlayerId = p.Id
                            };

                            DataStoreFlight2Player.AddItemAsync(f2p);
                        }
                    }
                    #endregion

                    #region CourseHandicapTable
                    try
                    {
                        conn.Execute("DELETE FROM CourseHandicap");
                    }
                    catch (Exception) { }
                    try
                    {
                        conn.Execute("DELETE FROM CourseHandicapTable");
                    }
                    catch (Exception) { }

                    IDataStore<CourseHandicapTable> DataStoreCourseHandicapTable = DependencyService.Get<IDataStore<CourseHandicapTable>>();
                    IDataStore<CourseHandicap> DataStoreCourseHandicap = DependencyService.Get<IDataStore<CourseHandicap>>();

                    foreach (MFAppCourseHandicapTable ct in item.CourseHandicapTables)
                    {
                        CourseHandicapTable cht = new CourseHandicapTable
                        {
                            Id = ct.Id,
                            TeeColour = ct.TeeColour,
                            TeeGender = ct.TeeGender,
                            CourseId = ct.CourseId,
                            Par = ct.Par,
                            CR = ct.CR,
                            Slope = ct.Slope
                        };

                        DataStoreCourseHandicapTable.AddItemAsync(cht);

                        foreach (CourseHandicap ch in ct.CourseHandicaps)
                        {
                            DataStoreCourseHandicap.AddItemAsync(ch);

                        }
                    }
                    #endregion


                    try
                    {
                        conn.Execute("DELETE FROM Player");
                    }
                    catch (Exception) { }

                    IDataStore<Player> DataStorePlayer = DependencyService.Get<IDataStore<Player>>();
                    foreach (Player player in item.AllPlayers)
                    {
                        DataStorePlayer.AddItemAsync(player);
                    }
                }
            }
            catch(Exception exp)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> SyncMFWebSynchron()
        {
            Debug.Print("Start Sync: " + DateTime.Now.ToString("hh:mm:ss.fff"));
            try
            {
                // get current profile
                IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();
                int currentPlayerId = 0;
                Profile currentProfile = null;
                try
                {                    
                    var profilesTask = DataStoreProfile.GetItemsAsync();
                    currentProfile = profilesTask.Result.FirstOrDefault();
                    if (currentProfile != null)
                    {
                        currentPlayerId = currentProfile.Id;
                    }
                }
                catch(Exception) { }

                Debug.Print("Start Sync mit Profil: " + DateTime.Now.ToString("hh:mm:ss.fff"));
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    string webCall = $"MyAppData/GetMyAppData/";
                    if(currentPlayerId > 0)
                    {
                        webCall = $"MyAppData/GetMyAppData?PlayerId=" + currentPlayerId.ToString();
                    }
                    var ResultTask = client.GetStringAsync(webCall);
                    string json = ResultTask.Result.ToString();
                    Debug.Print("Daten vom web: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    MFWebAPIData item = (MFWebAPIData)JsonConvert.DeserializeObject<MFWebAPIData>(json);

                    Debug.Print("Daten aufbereitet: " + DateTime.Now.ToString("hh:mm:ss.fff"));
                    #region golfclubs
                    try
                    {
                        conn.Execute("DELETE FROM Golfclub");
                    }
                    catch (Exception) { }

                    IDataStore<Golfclub> DataStore = DependencyService.Get<IDataStore<Golfclub>>();
                    foreach (Golfclub golfclub in item.Golfclubs)
                    {
                        DataStore.AddItemAsync(golfclub);
                    }
                    #endregion

                    Debug.Print("End Sync Golfclubs: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    #region Event
                    try
                    {
                        conn.Execute("DELETE FROM Event Where Id<1000000");
                        //string sqlString = "DELETE FROM Event where EventDate < '" + DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd") + "'";
                        //conn.Execute(sqlString);
                    }
                    catch (Exception) { }

                    IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
                    foreach (Event e in item.Events)
                    {
                        DataStoreEvent.AddItemAsync(e);
                    }
                    #endregion
                    Debug.Print("End Sync Events: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    #region Tournaments
                    try
                    {
                        conn.Execute("DELETE FROM Tournament Where Id<1000000");
                        //string sqlString = "DELETE FROM Tournament where Datum < '" + DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd") + "'";
                        //conn.Execute(sqlString);
                    }
                    catch (Exception) { }

                    IDataStore<Tournament> DataStoreTournament = DependencyService.Get<IDataStore<Tournament>>();
                    foreach (Tournament t in item.Tournaments)
                    {
                        DataStoreTournament.AddItemAsync(t);
                    }
                    #endregion

                    Debug.Print("End Sync Turniere: " + DateTime.Now.ToString("hh:mm:ss.fff"));
                    
                    #region Course
                    try
                    {
                        conn.Execute("DELETE FROM Course");
                    }
                    catch (Exception) { }

                    IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
                    foreach (Course c in item.Courses)
                    {
                        DataStoreCourse.AddItemAsync(c);
                    }
                    #endregion

                    Debug.Print("End Sync Kurse: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    #region Tee
                    //try
                    //{
                    //    conn.Execute("DELETE FROM Tee");
                    //}
                    //catch (Exception) { }

                    IDataStore<Tee> DataStoreTee = DependencyService.Get<IDataStore<Tee>>();
                    var TeeTask = DataStoreTee.GetItemsAsync();
                    List<Tee> Tees = TeeTask.Result.ToList();

                    foreach (Tee t in item.Tees)
                    {
                        if (Tees.Where(x => x.Id == t.Id).FirstOrDefault() == null)
                        {
                            await DataStoreTee.AddItemAsync(t);
                        }
                        else
                        {
                            Tee currentTee = Tees.Where(x => x.Id == t.Id).FirstOrDefault();
                            currentTee.Length = t.Length;
                            currentTee.LengthRed = t.LengthRed;
                            currentTee.Hcp = t.Hcp;
                            currentTee.Name = t.Name;
                            currentTee.Par = t.Par;
                            currentTee.Textname = t.Textname;

                            await DataStoreTee.UpdateItemAsync(currentTee);
                        }
                    }
                    #endregion

                    Debug.Print("End Sync Tees: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    #region TeeInfo
                    try
                    {
                        conn.Execute("DELETE FROM TeeInfo");
                    }
                    catch (Exception) { }

                    IDataStore<TeeInfo> DataStoreTeeinfo = DependencyService.Get<IDataStore<TeeInfo>>();
                    foreach (TeeInfo t in item.TeeInfos)
                    {
                        DataStoreTeeinfo.AddItemAsync(t);
                    }
                    #endregion

                    #region Flight
                    try
                    {
                        conn.Execute("DELETE FROM Flight2Player Where Id < 100000");
                    }
                    catch (Exception) { }
                    try
                    {
                        conn.Execute("DELETE FROM Flight Where Id < 10000");
                    }
                    catch (Exception) { }

                    IDataStore<Flight> DataStoreFlight = DependencyService.Get<IDataStore<Flight>>();
                    IDataStore<Flight2Player> DataStoreFlight2Player = DependencyService.Get<IDataStore<Flight2Player>>();

                    foreach (MFAppFlight t in item.Flights)
                    {
                        Flight f = new Flight()
                        {
                            Id = t.Id,
                            FlightNumber = t.FlightNumber,
                            FlightName = t.FlightName,
                            TournamentId = t.TournamentId
                        };

                        DataStoreFlight.AddItemAsync(f);

                        foreach (Player p in t.Players)
                        {
                            Flight2Player f2p = new Flight2Player()
                            {
                                Id = (t.Id * 10000) + p.Id,
                                FlightId = t.Id,
                                PlayerId = p.Id
                            };

                            DataStoreFlight2Player.AddItemAsync(f2p);
                        }
                    }
                    #endregion

                    Debug.Print("End Sync Flights: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    #region CourseHandicapTable
                    //try
                    //{
                    //    conn.Execute("DELETE FROM CourseHandicap");
                    //}
                    //catch (Exception) { }
                    //try
                    //{
                    //    conn.Execute("DELETE FROM CourseHandicapTable");
                    //}
                    //catch (Exception) { }

                    IDataStore<CourseHandicapTable> DataStoreCourseHandicapTable = DependencyService.Get<IDataStore<CourseHandicapTable>>();
                    IDataStore<CourseHandicap> DataStoreCourseHandicap = DependencyService.Get<IDataStore<CourseHandicap>>();

                    var CourseHandicapTableTask = DataStoreCourseHandicapTable.GetItemsAsync();
                    List<CourseHandicapTable> CourseHandicapTables = CourseHandicapTableTask.Result.ToList();

                    foreach (MFAppCourseHandicapTable ct in item.CourseHandicapTables)
                    {
                        if (CourseHandicapTables.Where(x => x.Id == ct.Id).FirstOrDefault() == null)
                        {
                            CourseHandicapTable cht = new CourseHandicapTable
                            {
                                Id = ct.Id,
                                TeeColour = ct.TeeColour,
                                TeeGender = ct.TeeGender,
                                CourseId = ct.CourseId,
                                Par = ct.Par,
                                CR = ct.CR,
                                Slope = ct.Slope
                            };

                            DataStoreCourseHandicapTable.AddItemAsync(cht);

                            foreach (CourseHandicap ch in ct.CourseHandicaps)
                            {
                                DataStoreCourseHandicap.AddItemAsync(ch);

                            }
                        }
                    }
                    #endregion
                    
                    Debug.Print("End Sync Vorgaben: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    try
                    {
                        conn.Execute("DELETE FROM Player");
                    }
                    catch (Exception) { }

                    Player currentPlayer = null;
                    IDataStore<Player> DataStorePlayer = DependencyService.Get<IDataStore<Player>>();
                    foreach (Player player in item.AllPlayers)
                    {
                        await DataStorePlayer.AddItemAsync(player);
                        if ((currentProfile != null)&&(currentProfile.Id == player.Id))
                            currentPlayer = player;
                    }

                    Debug.Print("End Sync Spieler: " + DateTime.Now.ToString("hh:mm:ss.fff"));

                    try
                    {
                        // clean old result data
                        //string sqlString = "DELETE FROM Result where LastModified < '" + DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd") + "'";
                        //conn.Execute(sqlString);
                    }
                    catch (Exception) { }

                    // save last sync date
                    if (currentProfile != null)
                    {
                        if(currentPlayer!=null)
                        {
                            currentProfile.Handicap = currentPlayer.Handicap;
                            currentProfile.DGVHandicap = currentPlayer.DGVHandicap;
                            currentProfile.Mail = currentPlayer.Mail;
                            currentProfile.Name = currentPlayer.Name;
                        }
                        currentProfile.LastSync = DateTime.Now;
                        DataStoreProfile.UpdateItemAsync(currentProfile);
                    }
                }
            }
            catch (Exception exp)
            {
                return await Task.FromResult(false);
            }

            Debug.Print("End Sync: " + DateTime.Now.ToString("hh:mm:ss.fff"));

            return await Task.FromResult(true);
        }

        public async Task<bool> SendResults(List<TournamentResult> ResultList)
        {
            // add location info

            try
            {
                //var location = await Geolocation.GetLastKnownLocationAsync();
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, new TimeSpan(0, 0, 10));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    foreach (TournamentResult tr in ResultList)
                    {
                        tr.Latitude = location.Latitude;
                        tr.Longitude = location.Longitude;
                    }
                }
            }
            catch (Exception exp) 
            { 
                // manage exception
            }


            try
            {
               HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(ResultList).ToString(), Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = client.PostAsync($"MyAppResults/SendTournamentResults", content).Result;
                }
            }
            catch(Exception exp)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<MFAppFullTournamentResult>> GetLastResults()
        {
            IEnumerable<MFAppFullTournamentResult> items = null;

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var ResultTask = client.GetStringAsync("MyAppResults/GetLastResults/");
                    string json = ResultTask.Result.ToString();

                    items = (IEnumerable<MFAppFullTournamentResult>)JsonConvert.DeserializeObject<IEnumerable<MFAppFullTournamentResult>>(json);
                }
            }
            catch(Exception)
            {
                return new List<MFAppFullTournamentResult>();
            }
            return items;
        }

        public async Task<bool> SendNewPlayer(Player NewPlayer)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(NewPlayer).ToString(), Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = client.PostAsync($"PlayersAPI/PostPlayer", content).Result;
                }
            }
            catch (Exception exp)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> ResetDB()
        {
            try
            {
                try
                {
                    conn.Execute("DROP TABLE Course;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE CourseHandicap;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE CourseHandicapTable;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Event;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Flight;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Golfclub;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Player;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Profile;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Result;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Tee;");
                }
                catch (Exception) { }

                try
                {
                    conn.Execute("DROP TABLE Tournament;");
                }
                catch (Exception) { }

                // recreate all tables by calling contructor
                IDataStore<Player> DataStorePlayer = DependencyService.Get<IDataStore<Player>>();
                DataStorePlayer = new PlayerDataStore();

                IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();
                DataStoreProfile = new ProfileDataStore();

                IDataStore<Golfclub> DataStoreGolfclub = DependencyService.Get<IDataStore<Golfclub>>();
                DataStoreGolfclub = new GolfclubDataStore();

                IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
                DataStoreEvent = new EventDataStore();

                IDataStore<Tournament> DataStoreTournament = DependencyService.Get<IDataStore<Tournament>>();
                DataStoreTournament = new TournamentDataStore();

                IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
                DataStoreCourse = new CourseDataStore();

                IDataStore<CourseHandicapTable> DataStoreCourseHandicapTable = DependencyService.Get<IDataStore<CourseHandicapTable>>();
                DataStoreCourseHandicapTable = new CourseHandicapTableDataStore();

                IDataStore<CourseHandicap> DataStoreCourseHandicap = DependencyService.Get<IDataStore<CourseHandicap>>();
                DataStoreCourseHandicap = new CourseHandicapDataStore();

                IDataStore<Flight> DataStoreFlight = DependencyService.Get<IDataStore<Flight>>();
                DataStoreFlight = new FlightDataStore();

                IDataStore<Flight2Player> DataStoreFlight2Player = DependencyService.Get<IDataStore<Flight2Player>>();
                DataStoreFlight2Player = new Flight2PlayerDataStore();

                IDataStore<Tee> DataStoreTee = DependencyService.Get<IDataStore<Tee>>();
                DataStoreTee = new TeeDataStore();

            }
            catch (Exception exp)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }
}
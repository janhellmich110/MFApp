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

        public async Task<bool> SendResults(List<TournamentResult> ResultList)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");
                //client.BaseAddress = new Uri($"https://80.228.37.106/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(ResultList).ToString(), Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = client.PostAsync($"MyAppData/SendTournamentResults", content).Result;
                }
            }
            catch(Exception exp)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
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
                    var result = client.PostAsync($"MyAppData/SendNewPlayer", content).Result;
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
                PlayerDataStore playerds = new PlayerDataStore();
                ProfileDataStore profilsDs = new ProfileDataStore();
                GolfclubDataStore clubds = new GolfclubDataStore();
                EventDataStore eventDs = new EventDataStore();
                TournamentDataStore tourDs = new TournamentDataStore();
                CourseDataStore ourseDs = new CourseDataStore();
                CourseHandicapTableDataStore courseht = new CourseHandicapTableDataStore();
                CourseHandicapDataStore courseh = new CourseHandicapDataStore();
                FlightDataStore flightDs = new FlightDataStore();
                Flight2PlayerDataStore f2pDs = new Flight2PlayerDataStore();
                TeeDataStore teeDs = new TeeDataStore();
            }
            catch (Exception exp)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }
}
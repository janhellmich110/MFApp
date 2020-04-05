using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Data;

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
                        conn.Execute("DROP TABLE Golfclub;");
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
                        conn.Execute("DELETE FROM Courses");
                    }
                    catch (Exception) { }

                    IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
                    foreach (Course c in item.Courses)
                    {
                        DataStoreCourse.AddItemAsync(c);
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
            }
            catch (Exception exp)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }
}
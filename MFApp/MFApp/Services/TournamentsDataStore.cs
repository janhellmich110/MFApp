using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using MFApp.Models;

using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;

namespace MFApp.Services
{
    public class TournamentsDataStore : IDataStore<Tournaments>
    {
        private List<Tournaments> TournamentsList;
        private SQLiteConnection conn;
        private string dbPathTournaments => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public TournamentsDataStore()
        {
            conn = new SQLiteConnection(dbPathTournaments);
            if(!SQLiteHelper.TableExists("Tournaments", conn))
                conn.CreateTable<Tournaments>();

            // get all entries from table
            TournamentsList = conn.Table<Tournaments>().ToList();
        }
        public async Task<bool> AddItemAsync(Tournaments Tournaments)
        {
            int result = 0;
            try
            {
                int TournamentsCount = conn.Table<Tournaments>().Count();
                Tournaments.Id = 100000 + TournamentsCount;
                result = conn.Insert(Tournaments);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Tournaments.Name, ex.Message);
            }
            TournamentsList = conn.Table<Tournaments>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Tournaments item)
        {
            var oldItem = TournamentsList.Where((Tournaments arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            TournamentsList = conn.Table<Tournaments>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = TournamentsList.Where((Tournaments arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            TournamentsList = conn.Table<Tournaments>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Tournaments> GetItemAsync(int id)
        {
            return await Task.FromResult(TournamentsList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Tournaments>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(TournamentsList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Tournaments> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"TournamentssAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Tournaments>>(json));

                    //conn.Table<Tournaments>().Delete();
                    conn.Execute("DELETE FROM Tournaments");
                    foreach (Tournaments item in items)
                    {
                        conn.Insert(item);
                    }
                }
            }
            catch(Exception exp)
            {
                return await Task.FromResult(false);
            }
            // get all entries from table
            TournamentsList = conn.Table<Tournaments>().ToList();

            return await Task.FromResult(true);
        }
    }
}
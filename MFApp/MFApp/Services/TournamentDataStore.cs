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
    public class TournamentDataStore : IDataStore<Tournament>
    {
        private List<Tournament> TournamentList;
        private SQLiteConnection conn;
        private string dbPathTournaments => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public TournamentDataStore()
        {
            conn = new SQLiteConnection(dbPathTournaments);
            if(!SQLiteHelper.TableExists("Tournament", conn))
                conn.CreateTable<Tournament>();

            // get all entries from table
            TournamentList = conn.Table<Tournament>().ToList();
        }
        public async Task<bool> AddItemAsync(Tournament Tournament)
        {
            int result = 0;
            int maxId = 1000000;
            try
            {
                if (Tournament.Id == 0)
                {
                    // get max id 
                    if (TournamentList.Count() > 0)
                        maxId = TournamentList.Select(x => x.Id).Max();

                    if (maxId < 1000000)
                    {
                        maxId = 1000000;
                    }
                    else
                    {
                        maxId = maxId + 1;
                    }
                    Tournament.Id = maxId;
                }
                result = conn.Insert(Tournament);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Tournament.Name, ex.Message);
            }
            TournamentList = conn.Table<Tournament>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Tournament item)
        {
            var oldItem = TournamentList.Where((Tournament arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            TournamentList = conn.Table<Tournament>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = TournamentList.Where((Tournament arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            TournamentList = conn.Table<Tournament>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Tournament> GetItemAsync(int id)
        {
            return await Task.FromResult(TournamentList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Tournament>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(TournamentList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Tournament> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"TournamentssAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Tournament>>(json));

                    //conn.Table<Tournaments>().Delete();
                    conn.Execute("DELETE FROM Tournament");
                    foreach (Tournament item in items)
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
            TournamentList = conn.Table<Tournament>().ToList();

            return await Task.FromResult(true);
        }
    }
}
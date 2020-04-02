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
    public class GolfclubDataStore : IDataStore<Golfclub>
    {
        private List<Golfclub> GolfclubList;
        private SQLiteConnection conn;
        private string dbPathGolfclub => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public GolfclubDataStore()
        {
            conn = new SQLiteConnection(dbPathGolfclub);
            if(!SQLiteHelper.TableExists("Golfclub", conn))
                conn.CreateTable<Golfclub>();

            // get all entries from table
            GolfclubList = conn.Table<Golfclub>().ToList();
        }
        public async Task<bool> AddItemAsync(Golfclub Golfclub)
        {
            int result = 0;
            try
            {
                int GolfclubCount = conn.Table<Golfclub>().Count();
                Golfclub.Id = 100000 + GolfclubCount;
                result = conn.Insert(Golfclub);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Golfclub.Name, ex.Message);
            }
            GolfclubList = conn.Table<Golfclub>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Golfclub item)
        {
            var oldItem = GolfclubList.Where((Golfclub arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            GolfclubList = conn.Table<Golfclub>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = GolfclubList.Where((Golfclub arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            GolfclubList = conn.Table<Golfclub>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Golfclub> GetItemAsync(int id)
        {
            return await Task.FromResult(GolfclubList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Golfclub>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(GolfclubList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Golfclub> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"GolfclubsAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Golfclub>>(json));

                    //conn.Table<Golfclub>().Delete();
                    conn.Execute("DELETE FROM Golfclub");
                    foreach (Golfclub item in items)
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
            GolfclubList = conn.Table<Golfclub>().ToList();

            return await Task.FromResult(true);
        }
    }
}
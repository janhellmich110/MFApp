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
    public class Flight2PlayerDataStore : IDataStore<Flight2Player>
    {
        private List<Flight2Player> Flight2PlayerList;
        private SQLiteConnection conn;
        private string dbPathFlight => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public Flight2PlayerDataStore()
        {
            conn = new SQLiteConnection(dbPathFlight);
            if(!SQLiteHelper.TableExists("Flight2Player", conn))
                conn.CreateTable<Flight2Player>();

            // get all entries from table
            Flight2PlayerList = conn.Table<Flight2Player>().ToList();
        }
        public async Task<bool> AddItemAsync(Flight2Player Flight2Player)
        {
            int result = 0;
            try
            {
                result = conn.Insert(Flight2Player);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Flight2Player.FlightId, ex.Message);
            }
            Flight2PlayerList = conn.Table<Flight2Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Flight2Player item)
        {
            Flight2PlayerList = conn.Table<Flight2Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = Flight2PlayerList.Where((Flight2Player arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            Flight2PlayerList = conn.Table<Flight2Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Flight2Player> GetItemAsync(int id)
        {
            return await Task.FromResult(Flight2PlayerList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Flight2Player>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(Flight2PlayerList);
        }

        public async Task<bool> SyncMFWeb()
        {            
            //sync not used

            return await Task.FromResult(true);
        }
    }
}
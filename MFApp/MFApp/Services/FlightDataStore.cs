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
    public class FlightDataStore : IDataStore<Flight>
    {
        private List<Flight> FlightList;
        private SQLiteConnection conn;
        private string dbPathFlight => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public FlightDataStore()
        {
            conn = new SQLiteConnection(dbPathFlight);
            if(!SQLiteHelper.TableExists("Flight", conn))
                conn.CreateTable<Flight>();

            // get all entries from table
            FlightList = conn.Table<Flight>().ToList();
        }
        public async Task<bool> AddItemAsync(Flight Flight)
        {
            int result = 0;
            try
            {
                int FlightCount = conn.Table<Flight>().Count();
                Flight.Id = 100000 + FlightCount;
                result = conn.Insert(Flight);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Flight.FlightNumber, ex.Message);
            }
            FlightList = conn.Table<Flight>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Flight item)
        {
            var oldItem = FlightList.Where((Flight arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            FlightList = conn.Table<Flight>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = FlightList.Where((Flight arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            FlightList = conn.Table<Flight>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Flight> GetItemAsync(int id)
        {
            return await Task.FromResult(FlightList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Flight>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(FlightList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Flight> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"FlightsAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Flight>>(json));

                    //conn.Table<Flight>().Delete();
                    conn.Execute("DELETE FROM Flight");
                    foreach (Flight item in items)
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
            FlightList = conn.Table<Flight>().ToList();

            return await Task.FromResult(true);
        }
    }
}
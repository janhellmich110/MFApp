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
    public class EventDataStore : IDataStore<Event>
    {
        private List<Event> EventList;
        private SQLiteConnection conn;
        private string dbPathEvent => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public EventDataStore()
        {
            conn = new SQLiteConnection(dbPathEvent);
            if(!SQLiteHelper.TableExists("Event", conn))
                conn.CreateTable<Event>();

            // get all entries from table
            EventList = conn.Table<Event>().ToList();
        }
        public async Task<bool> AddItemAsync(Event Event)
        {
            int result = 0;
            int maxId = 1000000;
            try
            {
                if(Event.Id == 0)
                {
                    // get max id 
                    if(EventList.Count() > 0)
                        maxId = EventList.Select(x => x.Id).Max();

                    if(maxId < 1000000)
                    {
                        maxId = 1000000;
                    }
                    else
                    {
                        maxId = maxId + 1;
                    }
                    Event.Id = maxId;
                }
                result = conn.Insert(Event);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("scoretype"))
                {
                    // column scoretype not exists, recreate table and try again
                    conn.DropTable<Event>();

                    conn.CreateTable<Event>();
                    if (Event.Id == 0)
                    {
                        Event.Id = 100000;
                    }
                    result = conn.Insert(Event);
                }
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Event.Name, ex.Message);
            }
            EventList = conn.Table<Event>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Event item)
        {
            var oldItem = EventList.Where((Event arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            EventList = conn.Table<Event>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = EventList.Where((Event arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            EventList = conn.Table<Event>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Event> GetItemAsync(int id)
        {
            return await Task.FromResult(EventList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Event>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(EventList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Event> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"EventsAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Event>>(json));

                    //conn.Table<Event>().Delete();
                    conn.Execute("DELETE FROM Event");
                    foreach (Event item in items)
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
            EventList = conn.Table<Event>().ToList();

            return await Task.FromResult(true);
        }
    }
}
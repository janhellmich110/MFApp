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
    public class TeeDataStore : IDataStore<Tee>
    {
        private List<Tee> TeeList;
        private SQLiteConnection conn;
        private string dbPathTee => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public TeeDataStore()
        {
            conn = new SQLiteConnection(dbPathTee);
            if(!SQLiteHelper.TableExists("Tee", conn))
                conn.CreateTable<Tee>();

            // get all entries from table
            TeeList = conn.Table<Tee>().ToList();
        }
        public async Task<bool> AddItemAsync(Tee Tee)
        {
            int result = 0;
            try
            {
                int TeeCount = conn.Table<Tee>().Count();
                Tee.Id = 100000 + TeeCount;
                result = conn.Insert(Tee);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Tee.Name, ex.Message);
            }
            TeeList = conn.Table<Tee>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Tee item)
        {
            var oldItem = TeeList.Where((Tee arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            TeeList = conn.Table<Tee>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = TeeList.Where((Tee arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            TeeList = conn.Table<Tee>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Tee> GetItemAsync(int id)
        {
            return await Task.FromResult(TeeList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Tee>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(TeeList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Tee> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"TeesAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Tee>>(json));

                    //conn.Table<Tee>().Delete();
                    conn.Execute("DELETE FROM Tee");
                    foreach (Tee item in items)
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
            TeeList = conn.Table<Tee>().ToList();

            return await Task.FromResult(true);
        }
    }
}
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
    public class ResultDataStore : IDataStore<Result>
    {
        private List<Result> ResultList;
        private SQLiteConnection conn;
        private string dbPathResult => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public ResultDataStore()
        {
            conn = new SQLiteConnection(dbPathResult);
            if(!SQLiteHelper.TableExists("Result", conn))
                conn.CreateTable<Result>();

            // get all entries from table
            ResultList = conn.Table<Result>().ToList();
        }
        public async Task<bool> AddItemAsync(Result Result)
        {
            int result = 0;
            try
            {
                if (Result.Id == 0)
                {
                    int ResultCount = ResultList.Count(); // conn.Table<Result>().Count();
                    Result.Id = 100000 + ResultCount;
                }
                result = conn.Insert(Result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Result.PlayerId, ex.Message);
            }
            ResultList = conn.Table<Result>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Result item)
        {
            var oldItem = ResultList.Where((Result arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            ResultList = conn.Table<Result>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = ResultList.Where((Result arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            ResultList = conn.Table<Result>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Result> GetItemAsync(int id)
        {
            return await Task.FromResult(ResultList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Result>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(ResultList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Result> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"ResultsAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Result>>(json));

                    //conn.Table<Result>().Delete();
                    conn.Execute("DELETE FROM Result");
                    foreach (Result item in items)
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
            ResultList = conn.Table<Result>().ToList();

            return await Task.FromResult(true);
        }
    }
}
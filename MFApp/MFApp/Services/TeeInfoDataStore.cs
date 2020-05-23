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
    public class TeeInfoDataStore : IDataStore<TeeInfo>
    {
        private List<TeeInfo> TeeInfoList;
        private SQLiteConnection conn;
        private string dbPathTeeInfo => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public TeeInfoDataStore()
        {
            conn = new SQLiteConnection(dbPathTeeInfo);
            if(!SQLiteHelper.TableExists("TeeInfo", conn))
                conn.CreateTable<TeeInfo>();

            // get all entries from table
            TeeInfoList = conn.Table<TeeInfo>().ToList();
        }
        public async Task<bool> AddItemAsync(TeeInfo TeeInfo)
        {
            int result = 0;
            try
            {
                result = conn.Insert(TeeInfo);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", TeeInfo.TeeName, ex.Message);
            }
            TeeInfoList = conn.Table<TeeInfo>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(TeeInfo item)
        {
            var oldItem = TeeInfoList.Where((TeeInfo arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            TeeInfoList = conn.Table<TeeInfo>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = TeeInfoList.Where((TeeInfo arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            TeeInfoList = conn.Table<TeeInfo>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<TeeInfo> GetItemAsync(int id)
        {
            return await Task.FromResult(TeeInfoList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<TeeInfo>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(TeeInfoList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<TeeInfo> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"TeeInfos");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<TeeInfo>>(json));

                    //conn.Table<Tee>().Delete();
                    conn.Execute("DELETE FROM TeeInfo");
                    foreach (TeeInfo item in items)
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
            TeeInfoList = conn.Table<TeeInfo>().ToList();

            return await Task.FromResult(true);
        }
    }
}
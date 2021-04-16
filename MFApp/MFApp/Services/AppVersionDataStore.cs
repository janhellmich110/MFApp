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
    public class AppVersionDataStore : IDataStore<AppVersion>
    {
        private List<AppVersion> VersionList;
        private SQLiteConnection conn;
        private string dbPathCourse => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public AppVersionDataStore()
        {
            conn = new SQLiteConnection(dbPathCourse);
            if(!SQLiteHelper.TableExists("AppVersion", conn))
                conn.CreateTable<AppVersion>();

            // get all entries from table
            VersionList = conn.Table<AppVersion>().ToList();
        }
        public async Task<bool> AddItemAsync(AppVersion appVersion)
        {
            int result = 0;
            try
            {
                result = conn.Insert(appVersion);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", appVersion.Version.ToString(), ex.Message);
            }
            VersionList = conn.Table<AppVersion>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(AppVersion item)
        {
            var oldItem = VersionList.Where((AppVersion arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            VersionList = conn.Table<AppVersion>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = VersionList.Where((AppVersion arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            VersionList = conn.Table<AppVersion>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<AppVersion> GetItemAsync(int id)
        {
            return await Task.FromResult(VersionList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<AppVersion>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(VersionList);
        }

        public async Task<bool> SyncMFWeb()
        {
            return await Task.FromResult(true);
        }
    }
}
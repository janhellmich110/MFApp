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
    public class ProfileDataStore : IDataStore<Profile>
    {
        private List<Profile> ProfileList;
        private SQLiteConnection conn;
        private string dbPathProfile => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public ProfileDataStore()
        {
            conn = new SQLiteConnection(dbPathProfile);
            if(!SQLiteHelper.TableExists("Profile", conn))
                conn.CreateTable<Profile>();
            // get all entries from table
            ProfileList = conn.Table<Profile>().ToList();
        }
        public async Task<bool> AddItemAsync(Profile Profile)
        {
            int result = 0;
            try
            {
                int ProfileCount = conn.Table<Profile>().Count();
                Profile.Id = 100000 + ProfileCount;
                result = conn.Insert(Profile);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Profile.Name, ex.Message);
            }
            ProfileList = conn.Table<Profile>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Profile item)
        {
            var oldItem = ProfileList.Where((Profile arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            ProfileList = conn.Table<Profile>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                var oldItem = ProfileList.Where((Profile arg) => arg.Id == id).FirstOrDefault();
                conn.Delete(oldItem);
            }
            catch(Exception exp)
            {

            }

            ProfileList = conn.Table<Profile>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Profile> GetItemAsync(int id)
        {
            return await Task.FromResult(ProfileList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Profile>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(ProfileList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Profile> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"ProfilesAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Profile>>(json));

                    //conn.Table<Profile>().Delete();
                    conn.Execute("DELETE FROM Profile");
                    foreach (Profile item in items)
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
            ProfileList = conn.Table<Profile>().ToList();

            return await Task.FromResult(true);
        }
    }
}
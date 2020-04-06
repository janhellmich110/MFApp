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
    public class CourseHandicapDataStore : IDataStore<CourseHandicap>
    {
        private List<CourseHandicap> CourseHandicapList;
        private SQLiteConnection conn;
        private string dbPathCourseHandicap => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public CourseHandicapDataStore()
        {
            conn = new SQLiteConnection(dbPathCourseHandicap);
            if(!SQLiteHelper.TableExists("CourseHandicap", conn))
                conn.CreateTable<CourseHandicap>();

            // get all entries from table
            CourseHandicapList = conn.Table<CourseHandicap>().ToList();
        }
        public async Task<bool> AddItemAsync(CourseHandicap CourseHandicap)
        {
            int result = 0;
            try
            {
                result = conn.Insert(CourseHandicap);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", CourseHandicap.PlayerHandicap, ex.Message);
            }
            CourseHandicapList = conn.Table<CourseHandicap>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(CourseHandicap item)
        {
            var oldItem = CourseHandicapList.Where((CourseHandicap arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            CourseHandicapList = conn.Table<CourseHandicap>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = CourseHandicapList.Where((CourseHandicap arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            CourseHandicapList = conn.Table<CourseHandicap>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<CourseHandicap> GetItemAsync(int id)
        {
            return await Task.FromResult(CourseHandicapList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<CourseHandicap>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(CourseHandicapList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<CourseHandicap> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"CourseHandicapsAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<CourseHandicap>>(json));

                    //conn.Table<CourseHandicap>().Delete();
                    conn.Execute("DELETE FROM CourseHandicap");
                    foreach (CourseHandicap item in items)
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
            CourseHandicapList = conn.Table<CourseHandicap>().ToList();

            return await Task.FromResult(true);
        }
    }
}
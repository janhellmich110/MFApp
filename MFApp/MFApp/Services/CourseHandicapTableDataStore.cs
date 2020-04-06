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
    public class CourseHandicapTableDataStore : IDataStore<CourseHandicapTable>
    {
        private List<CourseHandicapTable> CourseHandicapTableList;
        private SQLiteConnection conn;
        private string dbPathCourseHandicapTable => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public CourseHandicapTableDataStore()
        {
            conn = new SQLiteConnection(dbPathCourseHandicapTable);
            if(!SQLiteHelper.TableExists("CourseHandicapTable", conn))
                conn.CreateTable<CourseHandicapTable>();

            // get all entries from table
            CourseHandicapTableList = conn.Table<CourseHandicapTable>().ToList();
        }
        public async Task<bool> AddItemAsync(CourseHandicapTable CourseHandicapTable)
        {
            int result = 0;
            try
            {                
                result = conn.Insert(CourseHandicapTable);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", CourseHandicapTable.CourseId, ex.Message);
            }
            CourseHandicapTableList = conn.Table<CourseHandicapTable>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(CourseHandicapTable item)
        {
            var oldItem = CourseHandicapTableList.Where((CourseHandicapTable arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            CourseHandicapTableList = conn.Table<CourseHandicapTable>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = CourseHandicapTableList.Where((CourseHandicapTable arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            CourseHandicapTableList = conn.Table<CourseHandicapTable>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<CourseHandicapTable> GetItemAsync(int id)
        {
            return await Task.FromResult(CourseHandicapTableList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<CourseHandicapTable>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(CourseHandicapTableList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<CourseHandicapTable> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"CourseHandicapTablesAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<CourseHandicapTable>>(json));

                    //conn.Table<CourseHandicapTable>().Delete();
                    conn.Execute("DELETE FROM CourseHandicapTable");
                    foreach (CourseHandicapTable item in items)
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
            CourseHandicapTableList = conn.Table<CourseHandicapTable>().ToList();

            return await Task.FromResult(true);
        }
    }
}
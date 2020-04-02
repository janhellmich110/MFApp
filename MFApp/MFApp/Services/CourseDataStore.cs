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
    public class CourseDataStore : IDataStore<Course>
    {
        private List<Course> CourseList;
        private SQLiteConnection conn;
        private string dbPathCourse => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public CourseDataStore()
        {
            conn = new SQLiteConnection(dbPathCourse);
            if(!SQLiteHelper.TableExists("Course", conn))
                conn.CreateTable<Course>();

            // get all entries from table
            CourseList = conn.Table<Course>().ToList();
        }
        public async Task<bool> AddItemAsync(Course Course)
        {
            int result = 0;
            try
            {
                int CourseCount = conn.Table<Course>().Count();
                Course.Id = 100000 + CourseCount;
                result = conn.Insert(Course);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Course.Name, ex.Message);
            }
            CourseList = conn.Table<Course>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Course item)
        {
            var oldItem = CourseList.Where((Course arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            CourseList = conn.Table<Course>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = CourseList.Where((Course arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            CourseList = conn.Table<Course>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Course> GetItemAsync(int id)
        {
            return await Task.FromResult(CourseList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Course>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(CourseList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Course> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"CoursesAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Course>>(json));

                    //conn.Table<Course>().Delete();
                    conn.Execute("DELETE FROM Course");
                    foreach (Course item in items)
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
            CourseList = conn.Table<Course>().ToList();

            return await Task.FromResult(true);
        }
    }
}
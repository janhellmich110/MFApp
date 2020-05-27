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
    public class PlayerDataStore : IDataStore<Player>
    {
        private List<Player> PlayerList;
        private SQLiteConnection conn;
        private string dbPathPlayer => FileAccessHelper.GetLocalFilePath("MFApp.db3");
        public string StatusMessage { get; set; }

        public PlayerDataStore()
        {
            conn = new SQLiteConnection(dbPathPlayer);
            if(!SQLiteHelper.TableExists("Player", conn))
                conn.CreateTable<Player>();

            // get all entries from table
            PlayerList = conn.Table<Player>().ToList();
        }
        public async Task<bool> AddItemAsync(Player Player)
        {
            int result = 0;
            try
            {
                if (Player.Id == 0)
                {
                    int PlayerId = conn.Table<Player>().Select(x=>x.Id).Max() +1;
                    if (PlayerId < 100000)
                        PlayerId = 100000;
                    Player.Id = PlayerId;
                }
                result = conn.Insert(Player);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("dgvhandicap"))
                {
                    // column finl not exists, recreate table and try again
                    conn.DropTable<Player>();

                    conn.CreateTable<Player>();
                    if (Player.Id == 0)
                    {                        
                        Player.Id = 100000;
                    }
                    result = conn.Insert(Player);
                }

                StatusMessage = string.Format("Failed to add {0}. Error: {1}", Player.Name, ex.Message);
            }
            PlayerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Player item)
        {
            var oldItem = PlayerList.Where((Player arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            PlayerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = PlayerList.Where((Player arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            PlayerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Player> GetItemAsync(int id)
        {
            return await Task.FromResult(PlayerList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Player>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(PlayerList);
        }

        public async Task<bool> SyncMFWeb()
        {
            IEnumerable<Player> items;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://demo.portivity.de/mfweb/api/");

                bool IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
                if (IsConnected)
                {
                    var json = await client.GetStringAsync($"PlayersAPI");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Player>>(json));

                    //conn.Table<Player>().Delete();
                    conn.Execute("DELETE FROM Player");
                    foreach (Player item in items)
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
            PlayerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }
    }
}
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
        private List<Player> playerList;
        private SQLiteConnection conn;
        private string dbPathPlayer => FileAccessHelper.GetLocalFilePath("player.db3");
        public string StatusMessage { get; set; }

        //public PlayerDataStore()
        //{
        //    playerList = new List<Player>()
        //    {
        //        new Player { Id = Guid.NewGuid().ToString(), Initials = "JH", UserName="janh", Name="Jan Hellmich", Handicap=1.0, Birthday=new DateTime(1966, 10, 11) },
        //        new Player { Id = Guid.NewGuid().ToString(), Initials = "TH", UserName="tobiash", Name="Tobias Hellmich", Handicap=2.6, Birthday=new DateTime(2001, 10, 1) },
        //        new Player { Id = Guid.NewGuid().ToString(), Initials = "SH", UserName="saskiah", Name="Saskia Hellmich", Handicap=3.5, Birthday=new DateTime(1999, 10, 18) }
        //    };
        //}

        public PlayerDataStore()
        {
            conn = new SQLiteConnection(dbPathPlayer);
            if(!SQLiteHelper.TableExists("Player", conn))
                conn.CreateTable<Player>();

            // get all entries from table
            playerList = conn.Table<Player>().ToList();
        }
        public async Task<bool> AddItemAsync(Player player)
        {
            int result = 0;
            try
            {
                int playerCount = conn.Table<Player>().Count();
                player.Id = 100000 + playerCount;
                result = conn.Insert(player);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", player.Name, ex.Message);
            }
            playerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Player item)
        {
            var oldItem = playerList.Where((Player arg) => arg.Id == item.Id).FirstOrDefault();
            conn.Delete(oldItem);
            conn.Insert(item);

            playerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = playerList.Where((Player arg) => arg.Id == id).FirstOrDefault();
            conn.Delete(oldItem);

            playerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }

        public async Task<Player> GetItemAsync(int id)
        {
            return await Task.FromResult(playerList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Player>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(playerList);
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
                    var json = await client.GetStringAsync($"MFPlayersAPI");
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
            playerList = conn.Table<Player>().ToList();

            return await Task.FromResult(true);
        }
    }
}
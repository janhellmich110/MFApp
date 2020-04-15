using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

using Xamarin.Forms;

using MFApp.Models;
using MFApp.Views;

namespace MFApp.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        public ObservableCollection<Player> Player { get; set; }
        public Command LoadPlayerCommand { get; set; }

        public PlayerViewModel()
        {
            Title = "Spieler";
            Player = new ObservableCollection<Player>();
            LoadPlayerCommand = new Command(async () => await ExecuteLoadPlayerCommand());

            MessagingCenter.Subscribe<NewPlayerPage, Player>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Player;
                Player.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
        }

        async Task ExecuteLoadPlayerCommand()
        {
            IsBusy = true;

            try
            {
                Player.Clear();
                var items = await DataStore.GetItemsAsync(true);
                items = items.OrderBy(x => x.Name);
                foreach (var item in items)
                {
                    Player.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
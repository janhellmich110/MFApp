using System;

using MFApp.Models;

namespace MFApp.ViewModels
{
    public class PlayerDetailViewModel : BaseViewModel
    {
        public Player Player { get; set; }
        public PlayerDetailViewModel(Player player = null)
        {
            Title = player.Name;
            Player = player;
        }
    }
}

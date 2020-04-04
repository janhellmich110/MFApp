using System;

using MFApp.Models;

namespace MFApp.ViewModels
{
    public class PlayerDetailViewModel : BaseViewModel
    {
        public Player Player { get; set; }
        public PlayerDetailViewModel(Player Player = null)
        {
            Title = Player.Name;
            Player = Player;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;


namespace MFApp.Models
{
    public class TournamentPageData
    {
        public TournamentPageData()
        {
            AllPlayers = new ObservableCollection<Player>();
            SelectedPlayers = new ObservableCollection<Player>();
        }
        public Event TournamentEvent { get; set; }

        public ObservableCollection<Player> AllPlayers{ get; set;}

        public ObservableCollection<Player> SelectedPlayers { get; set;}
    }
}

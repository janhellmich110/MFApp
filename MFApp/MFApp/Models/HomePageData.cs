using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MFApp.Models
{
    public class HomePageData
    {
        public ObservableCollection<HomePageEvent> Events { get; set; }

        public HomePageData()
        {
            Events = new ObservableCollection<HomePageEvent>();
        }
    }

    public class HomePageEvent
    {
        public string EventClub { get; set; }
        public string EventDate { get; set; }
        public string EventName { get; set; }
        public string TournamentName { get; set; }

        public string BackColor { get; set; }

        public string ButtonText { get; set; }

        public Tournament EventTournament { get; set; }

    }

}

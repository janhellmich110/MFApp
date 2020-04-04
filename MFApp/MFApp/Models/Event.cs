using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Models
{
    public class Event
    {
        public int Id;
        public string Name;

        public List<Tournament> TournamentsList;
    }
}

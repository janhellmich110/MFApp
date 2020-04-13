using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class TournamentResultSummary
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int ScoreBrutto { get; set; }
        public int ScoreNetto { get; set; }
        public int BruttoPoints { get; set; }
        public int NettoPoints { get; set; }
        public int Putts { get; set; }
    }
}

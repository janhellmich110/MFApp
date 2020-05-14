using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class TournamentResult
    {
        public int PlayerId { get; set; }
        public int TournamentId { get; set; }
        public int TeeId { get; set; }
        public int Score { get; set; }
        public int Putts { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

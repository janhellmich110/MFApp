using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class Result
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int Score { get; set; }
        public int Putts { get; set; }
        public int BruttoZ { get; set; }
        public int BruttoS { get; set; }
        public int NettoZ { get; set; }
        public int NettoS { get; set; }

        public int PlayerId { get; set; }

        public int TeeId { get; set; }

        public int TournamentId { get; set; }
    }
}

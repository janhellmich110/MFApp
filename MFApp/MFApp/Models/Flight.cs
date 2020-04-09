using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class Flight
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int FlightNumber { get; set; }

        public int TournamentId { get; set; }
    }
}

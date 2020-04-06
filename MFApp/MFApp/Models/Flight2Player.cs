using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class Flight2Player
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int PlayerId { get; set; }
    }
}

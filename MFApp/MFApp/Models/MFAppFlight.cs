﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class MFAppFlight
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int FlightNumber { get; set; }
        public List<Player> Players { get; set; }
    }
}

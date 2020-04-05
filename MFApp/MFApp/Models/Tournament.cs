﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class Tournament
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Datum { get; set; }

        public int CourseId { get; set; }

        public int CourseHandicapTableId { get; set; }
    }
}

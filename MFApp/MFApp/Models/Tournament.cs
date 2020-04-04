using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Models
{
    public class Tournament
    {
        public int Id;
        public string name;

        public DateTime Datum;

        public int CourseId;
        public List<Flight> Flightlist;
    }
}

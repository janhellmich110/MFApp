using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SQLite;

namespace MFApp.Models
{
    public class Profile
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string Mail { get; set; }
        public double Handicap { get; set; }
        public DateTime Birthday { get; set; }

        public Gender Gender;

        public int GroupId { get; set; }

        public DateTime LastSync { get; set; }
    }
}

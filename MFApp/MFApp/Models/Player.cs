using System;
using SQLite;

namespace MFApp.Models
{
    public enum Gender
    {
        Mann = 0,
        Frau = 1
    }
    public class Player
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string Mail { get; set; }
        public double Handicap { get; set; }
        public double DGVHandicap { get; set; }
        public DateTime Birthday { get; set; }

        public Gender Gender { get; set; }

        public int GroupId { get; set; }
    }
}
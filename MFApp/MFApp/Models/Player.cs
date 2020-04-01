using System;

namespace MFApp.Models
{
    public enum Gender
    {
        Male = 0,
        Female = 1
    }
    public class Player
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string Mail { get; set; }
        public double Handicap { get; set; }
        public DateTime Birthday { get; set; }

        public Gender Gender;
    }
}
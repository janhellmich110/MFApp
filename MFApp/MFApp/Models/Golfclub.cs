using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class Golfclub
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

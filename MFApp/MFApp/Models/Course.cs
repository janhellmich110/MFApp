using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace MFApp.Models
{
    public class Course
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }

        public int GolfclubId { get; set; }
    }
}

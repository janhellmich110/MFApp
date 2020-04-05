using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class CourseHandicapTable
    {
        [PrimaryKey]
        public int Id { get; set; }

        List<CourseHandicap> CourseHandicapList { get; set; }
        public int CourseId { get; set; }
        public string TeeColour { get; set; }
        public Gender TeeGender { get; set; }

        public int Par { get; set; }
        public double CR { get; set; }
        public int Slope { get; set; }
    }
}

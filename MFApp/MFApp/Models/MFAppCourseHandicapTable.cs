using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class MFAppCourseHandicapTable
    {
        [PrimaryKey]
        public int Id { get; set; }

        public List<CourseHandicap> CourseHandicaps;
        public int CourseId { get; set; }
        public string TeeColour { get; set; }
        public Gender TeeGender { get; set; }

        public int Par { get; set; }
        public double CR { get; set; }
        public int Slope { get; set; }
    }
}

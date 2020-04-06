using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class CourseHandicap
    {
        [PrimaryKey]
        public int Id { get; set; }
        public double HandicapFrom { get; set; }
        public double HandicapTo { get; set; }
        public int PlayerHandicap { get; set; }
        public int CourseHandicapTableId { get; set; }

    }
}

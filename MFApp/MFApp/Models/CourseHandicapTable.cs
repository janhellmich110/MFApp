﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Models
{
    public class CourseHandicapTable
    {
        public int Id;

        List<CourseHandicap> CourseHandicapList;
        public int CourseId;
        public string TeeColour;
        public Gender TeeGender;

        public int Par;
        public double CR;
        public int Slope;
    }
}

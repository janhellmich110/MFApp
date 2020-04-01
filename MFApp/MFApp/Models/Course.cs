﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Models
{
    public class Course
    {
        public int Id;
        public string Name;

        public int GolfclubId;

        public List<Tee> TeeList;
    }
}

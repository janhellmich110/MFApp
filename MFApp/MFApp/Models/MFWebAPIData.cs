using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Models
{
    public class MFWebAPIData
    {
        public List<Golfclub> Golfclubs { get; set; }

        public List<Event> Events;

        public List<Tournament> Tournaments;

        public List<Course> Courses;

        public List<Tee> Tees;

        public List<TeeInfo> TeeInfos { get; set; }

        public List<MFAppFlight> Flights;

        public List<MFAppCourseHandicapTable> CourseHandicapTables;

        public List<Player> AllPlayers { get; set; }
    }
}

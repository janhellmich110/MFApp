using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class Event
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime EventDate { get; set; }

        public EventTypeEnum EventType { get; set; }
    }

    public enum EventTypeEnum
    {
        Tournament = 0,
        Event = 1
    }
}

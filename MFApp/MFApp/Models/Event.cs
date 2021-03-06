﻿using System;
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

        public ScoreTypeEnum ScoreType { get; set; }

        public HandicapTypeEnum HandicapType { get; set; }

        public int GolfclubId { get; set; }
    }

    public enum EventTypeEnum
    {
        Tournament = 0,
        Event = 1,
        AppEvent = 99
    }

    public enum ScoreTypeEnum
    {
        Netto = 0,
        Brutto = 1
    }
    public enum HandicapTypeEnum
    {
        MFHandicap = 0,
        DGVHandicap = 1
    }
}

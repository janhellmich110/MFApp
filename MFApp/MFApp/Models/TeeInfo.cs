using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class TeeInfo
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int GolfClubId { get; set; }
        public int TeeNummer { get; set; }
        public string TeeName { get; set; }
        public string TeeInfoName { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TeeInfoTypeEnum TeeInfoType { get; set; }
    }

    public enum TeeInfoTypeEnum
    {
        Sonstiges = 0,
        Abschlag = 1,
        Loch = 2,
        Bunker = 3,
        Wasser = 4,
        Landezone = 5
    }
}

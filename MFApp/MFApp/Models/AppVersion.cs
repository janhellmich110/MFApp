using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace MFApp.Models
{
    public class AppVersion
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int Version { get; set; }
        
        public string DBVersion { get; set; }

    }
}

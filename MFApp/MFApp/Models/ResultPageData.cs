using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MFApp.Models
{
    public class ResultPageData
    {
        public List<MFAppFullTournamentResult> PlayerResults { get; set; }

        public ResultPageData()
        {
            PlayerResults = new List<MFAppFullTournamentResult>();
        }
    }

    
}

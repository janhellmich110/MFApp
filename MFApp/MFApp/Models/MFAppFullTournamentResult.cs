using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MFApp.Models
{
    public class MFAppFullTournamentResult
    {
        public string PlayerName { get; set; }

        public int HolesPlayed { get; set; }
        public int Putts { get; set; }
        public int ScoreBrutto { get; set; }
        public int ScoreNetto { get; set; }

        public int ScoreBruttoRelativ { get; set; }
        public int ScoreNettoRelativ { get; set; }

        public int Tee1Score { get; set; }
        public int Tee2Score { get; set; }
        public int Tee3Score { get; set; }
        public int Tee4Score { get; set; }
        public int Tee5Score { get; set; }
        public int Tee6Score { get; set; }
        public int Tee7Score { get; set; }
        public int Tee8Score { get; set; }
        public int Tee9Score { get; set; }
        public int Tee10Score { get; set; }
        public int Tee11Score { get; set; }
        public int Tee12Score { get; set; }
        public int Tee13Score { get; set; }
        public int Tee14Score { get; set; }
        public int Tee15Score { get; set; }
        public int Tee16Score { get; set; }
        public int Tee17Score { get; set; }
        public int Tee18Score { get; set; }
    }
}

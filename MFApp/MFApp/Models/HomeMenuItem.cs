using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Models
{
    public enum MenuItemType
    {
        Home,
        Player,
        Birdiebook,
        Configuration,
        LogOff,
        Results,
        About,
        Adhoc,
        OOM
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}

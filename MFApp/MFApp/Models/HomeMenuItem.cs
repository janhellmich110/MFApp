using System;
using System.Collections.Generic;
using System.Text;

namespace MFApp.Models
{
    public enum MenuItemType
    {
        Home,
        Player,
        Configuration,
        LogOff,
        Results,
        About,
        Adhoc,
        Maps
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}

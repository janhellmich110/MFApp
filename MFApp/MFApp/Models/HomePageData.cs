using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MFApp.Models
{
    public class HomePageData
    {
        public ObservableCollection<Event> Events { get; set; }

        public HomePageData()
        {
            Events = new ObservableCollection<Event>();
        }
    }
}

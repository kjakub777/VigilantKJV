using System;
using System.Collections.Generic;
using System.Text;

namespace VigilantKJV.Models
{
    public enum MenuItemType
    { 
        About,
        Bible,
        Memorized,
        DbTools,
        LastRecited
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
        public string Image { get; set; }
    }
}

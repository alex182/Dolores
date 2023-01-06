using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response
{
    public class Launch
    {
        public int? Id { get; set; }
        public string Cospar_Id { get; set; }
        public string Sort_Date { get; set; }
        public string Name { get; set; }
        public Provider? Provider { get; set; }
        public Vehicle? Vehicle { get; set; }
        public Pad? Pad { get; set; }
        public List<Mission> Missions { get; set; } = new List<Mission>();
        public string Mission_Description { get; set; }
        public string Launch_Description { get; set; }
        public DateTime? Win_Open { get; set; }
        public string T0 { get; set; }
        public DateTime? Win_Close { get; set; }
        public EstDate? Est_Date { get; set; }
        public string Date_Str { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag> { };
        public string Slug { get; set; }
        public string Weather_Summary { get; set; }
        public double? Weather_Temp { get; set; }
        public string Weather_Condition { get; set; }
        public double? Weather_Wind_MPH { get; set; }
        public string Weather_Icon { get; set; }
        public DateTime? Weather_Updated { get; set; }
        public string Quicktext { get; set; }
        public List<Medium> Media { get; set; } = new List<Medium> { };
        public int? Result { get; set; }
        public bool? Suborbital { get; set; }
        public DateTime? Modified { get; set; }
    }
}

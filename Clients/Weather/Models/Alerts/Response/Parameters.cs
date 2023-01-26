using System.Collections.Generic; 
using System; 
namespace Dolores.Clients.Weather.Models.Alerts{ 

    public class Parameters
    {
        public List<string> AWIPSidentifier { get; set; }
        public List<string> WMOidentifier { get; set; }
        public List<string> NWSheadline { get; set; }
        public List<string> BLOCKCHANNEL { get; set; }
        public List<string> VTEC { get; set; }
        public List<DateTime> eventEndingTime { get; set; }
    }

}
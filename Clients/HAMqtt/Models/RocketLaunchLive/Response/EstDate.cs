using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dolores.Clients.HAMqtt.Models.RocketLaunchLive.Response
{
    public class EstDate
    {
        public int? Month { get; set; }
        public int? Day { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }
    }
}

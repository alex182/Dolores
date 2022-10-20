using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt.Models.RocketLaunchLive.Response
{
    public class Location
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string Slug { get; set; }
    }
}

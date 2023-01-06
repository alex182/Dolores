using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response
{
    public class Vehicle
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Company_id { get; set; }
        public string Slug { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response
{
    public class Pad
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public Location? Location { get; set; }
    }
}

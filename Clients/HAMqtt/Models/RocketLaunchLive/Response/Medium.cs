using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt.Models.RocketLaunchLive.Response
{
    public class Medium
    {
        public int? Id { get; set; }
        public string Media_Url { get; set; }
        public string Youtube_VidId { get; set; }
        public bool? Featured { get; set; }
        public bool? LDFeatured { get; set; }
        public bool? Approved { get; set; }
    }
}

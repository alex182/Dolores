using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response
{
    public class ResponseBody
    {
        public bool? Valid_Auth { get; set; }
        public int? Count { get; set; }
        public int? Limit { get; set; }
        public int? Total { get; set; }
        public int? Last_Page { get; set; }
        public List<Launch> Result { get; set; }
    }
}

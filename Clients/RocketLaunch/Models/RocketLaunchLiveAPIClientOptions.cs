using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.RocketLaunch.Models
{
    public class RocketLaunchLiveAPIClientOptions : IRocketLaunchLiveAPIClientOptions
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }
}

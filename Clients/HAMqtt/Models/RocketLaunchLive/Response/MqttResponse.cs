using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt.Models.RocketLaunchLive.Response
{
    public class MqttResponse
    {
        public int StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public ResponseBody Result { get; set; }
    }
}

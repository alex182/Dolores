using Dolores.Clients.HAMqtt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt
{
    public class LaunchMqttOptions : IMqttOptions
    {
        public string BaseAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Topic { get; set; }
    }
}

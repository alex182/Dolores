using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt.Models
{
    public class HomeAssistantMqttOptions : IMqttOptions
    {
        public string BaseAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Topic { get; set; }
    }
}

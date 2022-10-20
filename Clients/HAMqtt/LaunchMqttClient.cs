using Dolores.Clients.HAMqtt.Models;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt
{
    public class LaunchMqttClient : MqttClient
    {
        public LaunchMqttClient(LaunchMqttOptions mqttOptions, MqttFactory mqttFactory) 
            : base(mqttOptions, mqttFactory)
        {
        }
    }
}

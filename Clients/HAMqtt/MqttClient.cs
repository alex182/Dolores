using Dolores.Clients.HAMqtt.Models;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt
{
    public class MqttClient : IMqttClient
    {
        private readonly IMqttOptions _mqttOptions;
        private readonly MqttFactory _mqttFactory;

        public MqttClient(IMqttOptions mqttOptions, MqttFactory mqttFactory)
        {
            _mqttOptions = mqttOptions;
            _mqttFactory = mqttFactory;
        }

        public async Task<MQTTnet.Client.IMqttClient> SubscribeToTopic()
        {
            var client = _mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_mqttOptions.BaseAddress)
                .WithCredentials(_mqttOptions.Username, _mqttOptions.Password)
                .Build();

            await client.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(_mqttOptions.Topic);
                   })
               .Build();

            await client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            return client;
        }

    }
}


namespace Dolores.Clients.HAMqtt
{
    public interface IMqttClient
    {
        Task<MQTTnet.Client.IMqttClient> SubscribeToTopic();
    }
}
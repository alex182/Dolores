using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt.Models
{
    public class MqttMessage
    {
        public ApplicationMessage ApplicationMessage { get; set; }
        public bool AutoAcknowledge { get; set; }
        public string ClientId { get; set; }
        public bool IsHandled { get; set; }
        public int PacketIdentifier { get; set; }
        public bool ProcessingFailed { get; set; }
        public int ReasonCode { get; set; }
        public string ResponseReasonString { get; set; }
        public List<string> ResponseUserProperties { get; set; }
        public string Tag { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.HAMqtt.Models
{
    public class ApplicationMessage
    {
        public string ContentType { get; set; }
        public string CorrelationData { get; set; }
        public bool Dup { get; set; }
        public int MessageExpiryInterval { get; set; }
        public string Payload { get; set; }
        public int PayloadFormatIndicator { get; set; }
        public int QualityOfServiceLevel { get; set; }
        public string ResponseTopic { get; set; }
        public bool Retain { get; set; }
        public string SubscriptionIdentifiers { get; set; }
        public string Topic { get; set; }
        public int TopicAlias { get; set; }
        public string UserProperties { get; set; }
    }
}

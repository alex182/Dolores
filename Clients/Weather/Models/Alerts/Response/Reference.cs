using Newtonsoft.Json; 
using System; 
namespace Dolores.Clients.Weather.Models.Alerts{ 

    public class Reference
    {
        [JsonProperty("@id")]
        public string id { get; set; }
        public string identifier { get; set; }
        public string sender { get; set; }
        public DateTime sent { get; set; }
    }

}
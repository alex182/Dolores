using Newtonsoft.Json; 
using System.Collections.Generic; 
using System; 
namespace Dolores.Clients.Weather.Models.Alerts{ 

    public class Root
    {
        [JsonProperty("@context")]
        public List<object> context { get; set; }
        public string type { get; set; }
        public List<Feature> features { get; set; }
        public string title { get; set; }
        public DateTime updated { get; set; }
    }

}
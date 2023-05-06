using Newtonsoft.Json; 
using System.Collections.Generic; 
namespace Dolores.Clients.Noaa.Models.Forecast{ 

    public class Response
    {
        [JsonProperty("@context")]
        public List<object> context { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
    }

}
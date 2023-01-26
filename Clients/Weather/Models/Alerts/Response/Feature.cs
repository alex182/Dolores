namespace Dolores.Clients.Weather.Models.Alerts{ 

    public class Feature
    {
        public string id { get; set; }
        public string type { get; set; }
        public object geometry { get; set; }
        public Properties properties { get; set; }
    }

}
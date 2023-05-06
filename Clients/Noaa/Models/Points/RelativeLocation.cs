namespace Dolores.Clients.Noaa.Models.Points
{

    public class RelativeLocation
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
    }

}
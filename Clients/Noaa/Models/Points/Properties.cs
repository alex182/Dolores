using Newtonsoft.Json;
namespace Dolores.Clients.Noaa.Models.Points
{

    public class Properties
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public string cwa { get; set; }
        public string forecastOffice { get; set; }
        public string gridId { get; set; }
        public int gridX { get; set; }
        public int gridY { get; set; }
        public string forecast { get; set; }
        public string forecastHourly { get; set; }
        public string forecastGridData { get; set; }
        public string observationStations { get; set; }
        public RelativeLocation relativeLocation { get; set; }
        public string forecastZone { get; set; }
        public string county { get; set; }
        public string fireWeatherZone { get; set; }
        public string timeZone { get; set; }
        public string radarStation { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public Distance distance { get; set; }
        public Bearing bearing { get; set; }
    }

}
namespace Dolores.Clients.Noaa.Models
{
    public class NoaaOptions : INoaaOptions
    {
        public string BaseUrl { get; set; } = "https://api.weather.gov";

    }
}

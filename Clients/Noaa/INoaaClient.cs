using Dolores.Clients.Noaa.Models.Points;

namespace Dolores.Clients.Noaa
{
    public interface INoaaClient
    {
        Task<Models.Forecast.Response> GetForecast(Models.Forecast.Request request);
        Task<RelativeLocation> GetPoints(Request request);
    }
}
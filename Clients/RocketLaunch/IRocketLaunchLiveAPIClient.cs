using Dolores.Clients.Models;
using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;

namespace Dolores.Clients.RocketLaunch
{
    public interface IRocketLaunchLiveAPIClient
    {
        Task<APIResultsWrapper<ResponseBody>> GetLaunchesBetweenDates(string afterDate, string beforeDate);
    }
}
using Dolores.Clients.Models;
using Dolores.Clients.Nasa.Models;
using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;
using Dolores.Commands.Yarn.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Dolores
{
    public interface IUtility
    {
        Task<APIResultsWrapper<ResponseBody>> GetLaunches(DateTime? startDate, DateTime? endDate);
        Task SendLaunchNotification(ResponseBody launchInfo);
        Task SendApod(APODResponse message);
    }
}
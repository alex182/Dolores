using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;

namespace Dolores.Clients.Weather.Models.Alerts.Response
{
    public class ResponseBody
    {
        public bool? Valid_Auth { get; set; }
        public int? Count { get; set; }
        public int? Limit { get; set; }
        public int? Total { get; set; }
        public int? Last_Page { get; set; }
        public List<Properties> Result { get; set; }
    }
}

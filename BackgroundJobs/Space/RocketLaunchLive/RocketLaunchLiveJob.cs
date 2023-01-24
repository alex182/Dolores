using Dolores.BackgroundJobs.Space.RocketLaunchLive.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.BackgroundJobs.Space.RocketLaunchLive
{
    public class RocketLaunchLiveJob : BaseJob
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
        private readonly IUtility _utility;
        private readonly RocketLaunchLiveJobOptions _rocketLaunchLiveJobOptions;

        public RocketLaunchLiveJob(IUtility utility, RocketLaunchLiveJobOptions rocketLaunchLiveJobOptions)
        {
            _utility = utility;
            _rocketLaunchLiveJobOptions = rocketLaunchLiveJobOptions;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                if(currentTime.TimeOfDay.Hours == _rocketLaunchLiveJobOptions.RunTime.Hours && currentTime.TimeOfDay.Minutes == _rocketLaunchLiveJobOptions.RunTime.Minutes)
                {
                    var message = await _utility.GetLaunches(null, null);
                    await _utility.SendLaunchNotification(message.Result);
                }
               
            }
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);

        }
    }
}

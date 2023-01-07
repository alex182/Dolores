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
        private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(24));
        private readonly IUtility _utility;

        public RocketLaunchLiveJob(IUtility utility)
        {
            _utility = utility;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var message = await _utility.GetLaunches(null,null);
                await _utility.SendLaunchNotification(message.Result);
            }
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);

        }
    }
}

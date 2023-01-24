using Dolores.BackgroundJobs.Space.NasasAPOD.Model;
using Dolores.BackgroundJobs.Space.RocketLaunchLive.Models;
using Dolores.Clients.Nasa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.BackgroundJobs.Space.NasasAPOD
{
    public class APODJob : BaseJob
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
        private readonly INasaClient _nasaClient;
        private readonly IUtility _utility;
        private readonly APODJobOptions _apodOptions;

        public APODJob(INasaClient nasaClient, IUtility utility, APODJobOptions aPODJobOptions)
        {
            _nasaClient = nasaClient;
            _utility = utility;
            _apodOptions = aPODJobOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                if (currentTime.TimeOfDay.Hours == _apodOptions.RunTime.Hours && currentTime.TimeOfDay.Minutes == _apodOptions.RunTime.Minutes)
                {
                    var message = await _nasaClient.GetApod();
                    await _utility.SendApod(message);
                }

            }
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);

        }

    }
}

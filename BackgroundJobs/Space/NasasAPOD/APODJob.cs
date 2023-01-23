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
        private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(24));
        private readonly INasaClient _nasaClient;
        private readonly IUtility _utility;

        public APODJob(INasaClient nasaClient, IUtility utility)
        {
            _nasaClient = nasaClient;
            _utility = utility;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var message = await _nasaClient.GetApod();
                await _utility.SendApod(message);
            }
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);

        }

    }
}

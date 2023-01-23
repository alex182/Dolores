using Dolores.Clients.Nasa;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.BackgroundJobs.Space.NasasAPOD
{

    public class SpaceJob : IJob
    {
        private readonly INasaClient _nasaClient;
        private readonly IUtility _utility;
        public SpaceJob(INasaClient nasaClient, IUtility utility)
        {
            _nasaClient = nasaClient;
            _utility = utility;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var message = await _nasaClient.GetApod();
            await _utility.SendApod(message);
        }
    }

}

namespace Dolores.BackgroundJobs.Weather.NWSAlerts
{
    public class NWSAlertActiveJob : BaseJob
    {
        private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        private readonly IUtility _utility;

        public NWSAlertActiveJob(IUtility utility)
        {
            _utility = utility; 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var message = await _utility.GetWeatherAlerts();
                //await _utility.SendWeatherAlertNotification(message.Result);
            }
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
        }
    }
}

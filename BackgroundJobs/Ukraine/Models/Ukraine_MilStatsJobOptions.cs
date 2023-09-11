namespace Dolores.BackgroundJobs.Ukraine.Models
{
    public class Ukraine_MilStatsJobOptions
    {
        public TimeSpan RunTime { get; set; } = new TimeSpan(7,2, 0);
        public string WebookUrl{ get; set; }
        public string ThreadId { get; set; } = "1065387852189409321";
    }
}

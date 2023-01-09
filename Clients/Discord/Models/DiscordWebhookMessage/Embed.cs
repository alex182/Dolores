using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Discord.Models.DiscordWebhookMessage
{
    public class Embed
    {
        public string title { get; set; }
        public string description { get; set; }
        public int color { get; set; }
        public Image image { get; set; }
        public List<Field> fields { get; set; } = new List<Field>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Discord.Models.DiscordWebhookMessage
{
    public class DiscordWebhookMessage
    {
        public string content { get; set; }
        public List<Embed> embeds { get; set; } = new List<Embed>(); 
        public List<string> attatchments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Discord.Models
{
    public class DiscordClientOptions : IDiscordClientOptions
    {
        public string WebhookUrl { get; set; }
    }
}

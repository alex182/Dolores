using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Discord.Models
{
    public class DiscordClientOptions : IDiscordClientOptions
    {
        public string DiscordKey { get; set; }

        //prod is .
        //alex is !
        //denny is a twat
        public string DiscordCommandPrefix { get; set; }
    }
}

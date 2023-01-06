using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.NytSpeaker
{
    public class NytSpeakerOptions : INytSpeakerOptions
    {
        public string BaseUrl { get; set; } = @"https://int.nyt.com/newsgraphics/2023/congress/tally_by_party.json";
    }
}

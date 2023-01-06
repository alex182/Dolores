using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.NytSpeaker.Model
{
    public class NytSpeakerResponse
    {
        public string key { get; set; }
        public string vote_round { get; set; }
        public bool has_votes { get; set; }
        public List<Value> values { get; set; }
    }
}

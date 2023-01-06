using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.NytSpeaker.Model
{
    public class VoteCount
    {
        public string NomineeName { get; set; }
        public float TotalVotes { get; set; } = 0;
        public float PercentageOfTotal { get; set; } = 0;
    }
}

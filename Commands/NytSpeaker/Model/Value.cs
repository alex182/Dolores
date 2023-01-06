using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.NytSpeaker.Model
{
    public class Value
    {
        public string key { get; set; }
        public List<Value> values { get; set; }
        public int total { get; set; }
        public bool other { get; set; }
        public int value { get; set; }
    }
}

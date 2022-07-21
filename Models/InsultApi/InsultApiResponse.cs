using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Models.InsultApi
{
    public class InsultApiResponse
    {
        public string number { get; set; }
        public string language { get; set; }
        public string insult { get; set; }
        public string created { get; set; }
        public string shown { get; set; }
        public string createdby { get; set; }
        public string active { get; set; }
        public string comment { get; set; }
    }
}

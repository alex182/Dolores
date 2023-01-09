using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Nasa.Models
{
    public class NasaOptions : INasaOptions
    {
        public string BaseUrl { get; set; } = "https://api.nasa.gov";
        public string ApiKey { get; set; }
    }
}

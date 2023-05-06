using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Noaa
{
    public class NoaaCommandOptions : INoaaCommandOptions
    {
        public string BaseUrl { get; set; } = "https://api.weather.gov";

    }
}

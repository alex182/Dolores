using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Yarn
{
    public class YarnCommandOptions : IYarnCommandOptions
    {
        public string BaseUrl { get; set; } = "https://getyarn.io/";

    }
}

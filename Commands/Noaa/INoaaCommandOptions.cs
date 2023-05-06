using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Noaa
{
    public interface INoaaCommandOptions
    {
        string BaseUrl { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Nasa.Models
{
    public interface INasaOptions
    {
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Models
{
    public class APIResultsWrapper<T> : IAPIResultsWrapper<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public T Result { get; set; }
    }
}

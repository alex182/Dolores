using System.Net;

namespace Dolores.Clients.Models
{
    public interface IAPIResultsWrapper<T>
    {
        bool IsSuccessStatusCode { get; set; }
        T Result { get; set; }
        HttpStatusCode StatusCode { get; set; }
    }
}
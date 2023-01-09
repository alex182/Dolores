using Dolores.Clients.Nasa.Models;

namespace Dolores.Clients.Nasa
{
    public interface INasaClient
    {
        Task<APODResponse> GetApod();
    }
}
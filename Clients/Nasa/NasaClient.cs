using Dolores.Clients.Nasa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.Nasa
{
    public class NasaClient : INasaClient
    {
        private readonly INasaOptions _nasaOptions;
        private HttpClient _httpClient;

        public NasaClient(INasaOptions nasaOptions, HttpClient httpClient)
        {
            _nasaOptions = nasaOptions;
            _httpClient = httpClient;
        }

        public async Task<APODResponse> GetApod()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_nasaOptions.BaseUrl}/planetary/apod?api_key={_nasaOptions.ApiKey}"));
            request.Headers.Accept.Clear();

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var apod = JsonConvert.DeserializeObject<APODResponse>(body);

            return apod;
        }
    }
}

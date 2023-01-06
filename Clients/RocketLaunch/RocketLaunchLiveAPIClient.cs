using Dolores.Clients.Models;
using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Clients.RocketLaunch
{
    public class RocketLaunchLiveAPIClient : IRocketLaunchLiveAPIClient
    {
        private readonly HttpClient _httpClient;
        private readonly IRocketLaunchLiveAPIClientOptions _rocketLaunchLiveAPIClientOptions;

        public RocketLaunchLiveAPIClient(HttpClient httpClient, IRocketLaunchLiveAPIClientOptions rocketLaunchLiveAPIClientOptions)
        {
            _httpClient = httpClient;
            _rocketLaunchLiveAPIClientOptions = rocketLaunchLiveAPIClientOptions;
        }

        public async Task<APIResultsWrapper<ResponseBody>> GetLaunchesBetweenDates(string afterDate, string beforeDate)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            var result = await _httpClient.GetAsync($"{_rocketLaunchLiveAPIClientOptions.BaseUrl}/json/launches?key={_rocketLaunchLiveAPIClientOptions.ApiKey}" +
                $"&after_date={afterDate}&before_date{beforeDate}");

            if (!result.IsSuccessStatusCode)
            {
                return new APIResultsWrapper<ResponseBody>
                {
                    IsSuccessStatusCode = result.IsSuccessStatusCode,
                    StatusCode = result.StatusCode
                };
            }

            var body = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseBody>(body);

            return new APIResultsWrapper<ResponseBody>
            {
                Result = response,
                IsSuccessStatusCode = result.IsSuccessStatusCode,
                StatusCode = result.StatusCode
            };
        }


    }
}

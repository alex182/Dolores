using Dolores.Services.UkraineStats.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using RandomUserAgent;

namespace Dolores.Services.UkraineStats
{
    public class UkraineStats_Service
    {
        private readonly HttpClient _httpClient; 
        private readonly ILogger _logger;
        private readonly UkraineStats_Service_Options _options;

        public UkraineStats_Service(HttpClient httpClient, ILogger logger, UkraineStats_Service_Options options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;
        }

        public async Task<List<AssetStat>> GetAssetStats(DateTime date)
        {
            var results = new List<AssetStat>();

            var url = $"{_options.BaseUrl}/en/news/{date.Year}/{date.Month.ToString("00")}/{date.Day.ToString("00")}/" +
                $"the-total-combat-losses-of-the-enemy-from-24-02-2022-to-{date.Day.ToString("00")}-{date.Month.ToString("00")}-{date.Year}/";

            var userAgent = RandomUa.RandomUserAgent;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://www.google.com/");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Cookie", "name=value; Path=/; cf_clearance=_ZIpi71l.1UgKYx16Ay8w6_S.K61xxXoSS9Vde15tzY-1694290961-0-1-f9936a2a.69e58009.ae9235ca-0.2.1694290961; PHPSESSID=6rmjicsa1ldc5n18i9dlira2k4; __cf_bm=Yq0jC4c43OiKRQx75wb4g8kMw_7283KOF.f2K9vjll0-1694290840-0-AZEk+lAdLhG8y0itIJPxEPHCimBpjYdpoMFK156Wd0O/qcjJTF+2BOG8NvTRVVNDmsz4oUKXm5bgUHSgL7HpOUA=");
            _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
            _httpClient.DefaultRequestHeaders.Add("TE", "trailers");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return results;

            HttpContent content = response.Content;

            string html = await content.ReadAsStringAsync();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var combatLossesParagraph = doc.DocumentNode.SelectSingleNode("//p[contains(text(),'The total combat losses of the enemy from')]");
            string[] lines = combatLossesParagraph.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines.ToList().GetRange(1, 12))
            {
                var asset = new AssetStat(); 
                string text = HtmlEntity.DeEntitize(HtmlEntity.Entitize(line)).Trim();
                var splitText = Regex.Split(text, @" – ");

                if (splitText.Length == 1)
                {
                    splitText = Regex.Split(text, @" ‒ ");
                }

                var assetCategory = splitText[0];
                var counts = splitText[1].Split();

                asset.Total = Convert.ToInt32(counts[0]);
                asset.DailyDiff = Convert.ToInt32(counts[1]?.Replace("(+", "").Replace("),", ""));


                switch (assetCategory)
                {
                    case string a when assetCategory.ToLower().Contains("personnel"):
                        asset.AssetCategory = AssetCategory.Personnel;
                        break;
                    case string a when assetCategory.ToLower().Contains("taks"):
                        asset.AssetCategory = AssetCategory.Tanks;
                        break;
                    case string a when assetCategory.ToLower().Contains("apv"):
                        asset.AssetCategory = AssetCategory.APV;
                        break;
                    case string a when assetCategory.ToLower().Contains("artillery "):
                        asset.AssetCategory = AssetCategory.Artillery;
                        break;
                    case string a when assetCategory.ToLower().Contains("mlrs"):
                        asset.AssetCategory = AssetCategory.MLRS;
                        break;
                    case string a when assetCategory.ToLower().Contains("anti-aircraft"): //anti-airfract and aircraft need to stay in this order
                        asset.AssetCategory = AssetCategory.AntiAircraft;
                        break;
                    case string a when assetCategory.ToLower().Contains("aircraft"): //anti-airfract and aircraft need to stay in this order
                        asset.AssetCategory = AssetCategory.Aircraft;
                        break;
                    case string a when assetCategory.ToLower().Contains("helicopters"):
                        asset.AssetCategory = AssetCategory.Helicopters;
                        break;
                    case string a when assetCategory.ToLower().Contains("uav"):
                        asset.AssetCategory = AssetCategory.UAV;
                        break;
                    case string a when assetCategory.ToLower().Contains("missiles"):
                        asset.AssetCategory = AssetCategory.CruiseMissiles;
                        break;
                    case string a when assetCategory.ToLower().Contains("warships"):
                        asset.AssetCategory = AssetCategory.WarshipsBoats;
                        break;
                    case string a when assetCategory.ToLower().Contains("vehicles"):
                        asset.AssetCategory = AssetCategory.VehiclesAndFuelTanks;
                        break;
                    case string a when assetCategory.ToLower().Contains("special"):
                        asset.AssetCategory = AssetCategory.SpecialEquipment;
                        break;
                    default:
                        asset.AssetCategory = AssetCategory.Uncategorized;
                        break;
                }                
            }


            return results;
        }
    }
}

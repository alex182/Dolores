using Dolores.Services.UkraineStats.Models;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V114.Profiler;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;



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

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(url);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string pageSource = driver.PageSource;

            if (string.IsNullOrEmpty(pageSource))
                return results;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageSource);

            var combatLossesParagraph = doc.DocumentNode.SelectSingleNode("//p[contains(text(),'The total combat losses of the enemy from')]");
            string[] lines = combatLossesParagraph.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

            if(lines.Length == 1)
            {
                return results; 
            }

            foreach (string line in lines.ToList().GetRange(1, 12))
            {
                _logger.LogInformation("{@line}",line);

                var asset = new AssetStat();
                var splitText = Regex.Split(line, @" – ");

                _logger.LogInformation("{@splitText}", splitText);


                if (splitText.Length == 1)
                {
                    splitText = Regex.Split(line, @" ‒ ");

                    _logger.LogInformation("Regex Match: {@splitText}", splitText);
                }

                var assetCategory = splitText[0];
                _logger.LogInformation("{@assetCategory}", assetCategory);

                var counts = splitText[1].Split();

                _logger.LogInformation("{@counts}", counts);

                string firstCount = counts.FirstOrDefault(s => int.TryParse(s, out _));

                _logger.LogInformation("{@firstCount}", firstCount);


                if (firstCount != null)
                {
                    asset.Total = int.Parse(firstCount);
                }

                string diffString = counts.FirstOrDefault(s => s.StartsWith("(+"));

                _logger.LogInformation("{@diffString}", diffString);

                if (diffString != null)
                {
                    asset.DailyDiff = Convert.ToInt32(diffString.Replace("(+", "").Replace(")", "").Replace(",", ""));
                }

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
                        _logger.LogInformation("Uncategorized Asset: {@AssetCategory}", asset.AssetCategory);
                        break;
                }

                _logger.LogInformation("{@asset}", asset);

                results.Add(asset);
            }


            return results;
        }
    }
}

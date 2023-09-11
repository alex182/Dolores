using Dolores.Services.UkraineStats.Models;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V114.Profiler;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Text.RegularExpressions;



namespace Dolores.Services.UkraineStats
{
    public class UkraineStats_Service : IUkraineStats_Service
    {
        private readonly UkraineStats_Service_Options _options;

        public UkraineStats_Service(UkraineStats_Service_Options options)
        {
            _options = options;
        }

        internal string BuildUrl(DateTime date)
        {
            return $"{_options.BaseUrl}/en/news/{date.Year}/{date.Month.ToString("00")}/{date.Day.ToString("00")}/" +
                $"the-total-combat-losses-of-the-enemy-from-24-02-2022-to-{date.Day.ToString("00")}-{date.Month.ToString("00")}-{date.Year}/";
        }

        internal string GetPageSource(string url) 
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(url);

            string pageSource = driver.PageSource;

            return pageSource;
        }

        public string GetInfographicUrl(DateTime date)
        {
            var imageUrl = ""; 

            var url = BuildUrl(date);

            var pageSource = GetPageSource(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageSource);

            HtmlNode imageNode = doc.DocumentNode.SelectSingleNode("//img[@class='mainImage']");

            if (imageNode != null)
            {
               imageUrl = $"{_options.BaseUrl}{imageNode.GetAttributeValue("src", "")}";
            }

            return imageUrl;
        }

        public async Task<List<AssetStat>> GetAssetStats(DateTime date)
        {
            var results = new List<AssetStat>();
            var url = BuildUrl(date);

            var pageSource = GetPageSource(url);

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
                Log.Information("{@line}",line);

                var asset = new AssetStat();
                var splitText = Regex.Split(line, @" – ");

                Log.Information("{@splitText}", splitText);


                if (splitText.Length == 1)
                {
                    splitText = Regex.Split(line, @" ‒ ");

                    Log.Information("Regex Match: {@splitText}", splitText);
                }

                var assetCategory = splitText[0];
                Log.Information("{@assetCategory}", assetCategory);

                var counts = splitText[1].Split();

                Log.Information("{@counts}", counts);

                string firstCount = counts.FirstOrDefault(s => int.TryParse(s, out _));

                Log.Information("{@firstCount}", firstCount);


                if (firstCount != null)
                {
                    asset.Total = int.Parse(firstCount);
                }

                string diffString = counts.FirstOrDefault(s => s.StartsWith("(+"));

                Log.Information("{@diffString}", diffString);

                if (diffString != null)
                {
                    asset.DailyDiff = Convert.ToInt32(diffString.Replace("(+", "").Replace(")", "").Replace(",", ""));
                }

                switch (assetCategory)
                {
                    case string a when assetCategory.ToLower().Contains("personnel"):
                        asset.AssetCategory = AssetCategory.Personnel;
                        break;
                    case string a when assetCategory.ToLower().Contains("vehicles"): //vehicles and tanks need to stay in this order
                        asset.AssetCategory = AssetCategory.VehiclesAndFuelTanks;
                        break;
                    case string a when assetCategory.ToLower().Contains("tanks"): //vehicles and tanks need to stay in this order
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
                    case string a when assetCategory.ToLower().Contains("special"):
                        asset.AssetCategory = AssetCategory.SpecialEquipment;
                        break;
                    default:
                        asset.AssetCategory = AssetCategory.Uncategorized;
                        Log.Information("Uncategorized Asset: {@AssetCategory}", asset.AssetCategory);
                        break;
                }

                Log.Information("{@asset}", asset);

                results.Add(asset);
            }


            return results;
        }
    }
}

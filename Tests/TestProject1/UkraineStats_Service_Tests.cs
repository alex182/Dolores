using Dolores.Services.UkraineStats;
using Dolores.Services.UkraineStats.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace TestProject1
{
    [TestClass]
    public class UkraineStats_Service_Tests
    {
        [TestClass]
        public class UkraineStat_Service_Tests
        {

            private static UkraineStats_Service _ukraineStatService;

            [ClassInitialize]
            public static void Init(TestContext context)
            {
                var options = new UkraineStats_Service_Options();

                _ukraineStatService = new UkraineStats_Service(options);
            }

            [TestMethod]
            public async Task UkrainStat_ValidDate_ShouldGetAssets()
            {
                var date = DateTime.UtcNow.AddDays(-1);

                var stats = await _ukraineStatService.GetAssetStats(date);

                stats.Count.Should().BeGreaterThan(0);
            }


            [TestMethod]
            public async Task UkrainStat_DateInFuture_AssetsShouldBeZero()
            {
                var date = DateTime.UtcNow.AddDays(1);

                var stats = await _ukraineStatService.GetAssetStats(date);

                stats.Count().Should().Be(0);
            }


            [TestMethod]
            public void UkrainStat_ValidDate_ShouldGetImageUrl()
            {
                var date = DateTime.UtcNow.AddDays(-1);

                var url = _ukraineStatService.GetInfographicUrl(date);

                url.Should().NotBe("");
            }
        }
    }
}
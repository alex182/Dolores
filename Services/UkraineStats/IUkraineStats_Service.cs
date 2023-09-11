using Dolores.Services.UkraineStats.Models;

namespace Dolores.Services.UkraineStats
{
    public interface IUkraineStats_Service
    {
        Task<List<AssetStat>> GetAssetStats(DateTime date);

        string GetInfographicUrl(DateTime date);

    }
}
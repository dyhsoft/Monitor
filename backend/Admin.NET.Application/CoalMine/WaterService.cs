using Admin.NET.Core;
using SqlSugar;
using Furion;
using Furion.DynamicApiController;

namespace Admin.NET.Application;

/// <summary>
/// 水害监测服务
/// </summary>
public class WaterService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public WaterService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<SqlSugarPagedList<WaterRealtime>> GetPage(BasePageInput input)
    {
        return await _db.Queryable<WaterRealtime>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    public async Task<dynamic> GetStatistics()
    {
        var list = await _db.Queryable<WaterRealtime>().ToListAsync();
        return new { Total = list.Count };
    }
}

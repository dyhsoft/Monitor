using Admin.NET.Core;
using SqlSugar;
using Furion;
using Furion.DynamicApiController;

namespace Admin.NET.Application;

/// <summary>
/// 安全监测服务
/// </summary>
public class SafetyService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public SafetyService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<SqlSugarPagedList<SafetyRealtime>> GetPage(BasePageInput input)
    {
        return await _db.Queryable<SafetyRealtime>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    public async Task<dynamic> GetStatistics()
    {
        var list = await _db.Queryable<SafetyRealtime>().ToListAsync();
        return new { Total = list.Count };
    }
}

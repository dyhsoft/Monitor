using Admin.NET.Core;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 安全监测服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Safety", Order = 100)]
public class SafetyService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public SafetyService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取安全监测实时数据
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<SafetyRealtime>> GetPage( BasePageInput input)
    {
        return await _db.Queryable<SafetyRealtime>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取安全监测统计
    /// </summary>
    public async Task<dynamic> GetStatistics()
    {
        var list = await _db.Queryable<SafetyRealtime>().ToListAsync();
        return new
        {
            Total = list.Count,
            Normal = list.Count(it => it.Status == 0),
            Alarm = list.Count(it => it.Status == 1),
            Fault = list.Count(it => it.Status == 2)
        };
    }
}

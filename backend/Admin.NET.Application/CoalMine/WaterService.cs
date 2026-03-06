using Admin.NET.Core;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 水害监测服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Water", Order = 100)]
public class WaterService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public WaterService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取水害监测实时数据
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<WaterRealtime>> GetPage([FromBody] PageInputBase input)
    {
        return await _db.Queryable<WaterRealtime>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取水害监测统计
    /// </summary>
    public async Task<dynamic> GetStatistics()
    {
        var list = await _db.Queryable<WaterRealtime>().ToListAsync();
        return new
        {
            Total = list.Count,
            Normal = list.Count(it => it.Status == 0),
            Alarm = list.Count(it => it.Status == 1)
        };
    }
}

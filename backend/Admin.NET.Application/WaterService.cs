using Admin.NET.Core;
using Admin.NET.Core.Entity.CoalMine;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<SqlSugarPagedList<WaterRealtime>> GetRealtimePage([FromBody] WaterInput input)
    {
        return await _db.Queryable<WaterRealtime>()
            .LeftJoin<CoalMine>((w, c) => w.MineId == c.Id)
            .WhereIF(input.MineId > 0, (w, c) => w.MineId == input.MineId)
            .Select((w, c) => new WaterRealtime
            {
                Id = w.Id,
                MineId = w.MineId,
                SensorCode = w.SensorCode,
                SensorName = w.SensorName,
                Status = w.Status,
                WaterLevel = w.WaterLevel,
                FlowRate = w.FlowRate,
                Temperature = w.Temperature,
                UpdateTime = w.UpdateTime
            })
            .OrderBy(w => w.UpdateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取水害监测统计
    /// </summary>
    public async Task<WaterStatistics> GetStatistics(long? mineId)
    {
        var query = _db.Queryable<WaterRealtime>();
        if (mineId > 0)
            query = query.Where(it => it.MineId == mineId);

        var list = await query.ToListAsync();

        return new WaterStatistics
        {
            Total = list.Count,
            Normal = list.Count(it => it.Status == 0),
            Alarm = list.Count(it => it.Status == 1)
        };
    }
}

/// <summary>
/// 水害监测输入
/// </summary>
public class WaterInput
{
    public int Current { get; set; } = 1;
    public int Size { get; set; } = 10;
    public long? MineId { get; set; }
}

/// <summary>
/// 水害监测统计
/// </summary>
public class WaterStatistics
{
    public int Total { get; set; }
    public int Normal { get; set; }
    public int Alarm { get; set; }
}

using Admin.NET.Core;
using Admin.NET.Core.Entity.CoalMine;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<SqlSugarPagedList<SafetyRealtime>> GetRealtimePage([FromBody] SafetyInput input)
    {
        return await _db.Queryable<SafetyRealtime>()
            .LeftJoin<CoalMine>((s, c) => s.MineId == c.Id)
            .WhereIF(input.MineId > 0, (s, c) => s.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.SensorCode), (s, c) => s.SensorCode.Contains(input.SensorCode))
            .Select((s, c) => new SafetyRealtime
            {
                Id = s.Id,
                MineId = s.MineId,
                SensorCode = s.SensorCode,
                Value = s.Value,
                Unit = s.Unit,
                Status = s.Status,
                UpdateTime = s.UpdateTime
            })
            .OrderBy(s => s.UpdateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取安全监测统计
    /// </summary>
    public async Task<SafetyStatistics> GetStatistics(long? mineId)
    {
        var query = _db.Queryable<SafetyRealtime>();
        if (mineId > 0)
            query = query.Where(it => it.MineId == mineId);

        var list = await query.ToListAsync();

        return new SafetyStatistics
        {
            Total = list.Count,
            Normal = list.Count(it => it.Status == 0),
            Alarm = list.Count(it => it.Status == 1),
            Fault = list.Count(it => it.Status == 2)
        };
    }
}

/// <summary>
/// 安全监测输入
/// </summary>
public class SafetyInput
{
    public int Current { get; set; } = 1;
    public int Size { get; set; } = 10;
    public long? MineId { get; set; }
    public string SensorCode { get; set; }
}

/// <summary>
/// 安全监测统计
/// </summary>
public class SafetyStatistics
{
    public int Total { get; set; }
    public int Normal { get; set; }
    public int Alarm { get; set; }
    public int Fault { get; set; }
}

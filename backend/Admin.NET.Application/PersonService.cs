using Admin.NET.Core;
using Admin.NET.Core.Entity.CoalMine;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 人员定位服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Person", Order = 100)]
public class PersonService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public PersonService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取人员定位实时数据
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<PersonLocation>> GetRealtimePage([FromBody] PersonInput input)
    {
        return await _db.Queryable<PersonLocation>()
            .LeftJoin<CoalMine>((p, c) => p.MineId == c.Id)
            .WhereIF(input.MineId > 0, (p, c) => p.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.CardId), (p, c) => p.CardId.Contains(input.CardId))
            .Select((p, c) => new PersonLocation
            {
                Id = p.Id,
                MineId = p.MineId,
                CardId = p.CardId,
                PersonName = p.PersonName,
                StationId = p.StationId,
                StationName = p.StationName,
                AreaCode = p.AreaCode,
                AreaName = p.AreaName,
                InTime = p.InTime,
                UpdateTime = p.UpdateTime
            })
            .OrderBy(p => p.UpdateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取区域人员统计
    /// </summary>
    public async Task<List<AreaStatistics>> GetAreaStatistics(long? mineId)
    {
        var query = _db.Queryable<PersonLocation>();
        if (mineId > 0)
            query = query.Where(it => it.MineId == mineId);

        var list = await query.ToListAsync();

        return list.GroupBy(it => it.AreaName)
            .Select(g => new AreaStatistics
            {
                AreaName = g.Key ?? "未知区域",
                PersonCount = g.Count()
            })
            .ToList();
    }

    /// <summary>
    /// 获取人员统计
    /// </summary>
    public async Task<PersonStatistics> GetStatistics(long? mineId)
    {
        var query = _db.Queryable<PersonLocation>();
        if (mineId > 0)
            query = query.Where(it => it.MineId == mineId);

        var count = await query.CountAsync();

        return new PersonStatistics { Total = (int)count };
    }
}

/// <summary>
/// 人员定位输入
/// </summary>
public class PersonInput
{
    public int Current { get; set; } = 1;
    public int Size { get; set; } = 10;
    public long? MineId { get; set; }
    public string CardId { get; set; }
}

/// <summary>
/// 区域统计
/// </summary>
public class AreaStatistics
{
    public string AreaName { get; set; }
    public int PersonCount { get; set; }
}

/// <summary>
/// 人员统计
/// </summary>
public class PersonStatistics
{
    public int Total { get; set; }
}

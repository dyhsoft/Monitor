using Admin.NET.Application.CoalMine.SafetyMonitoring.Dtos;
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.SafetyMonitoring.Services;

/// <summary>
/// 安全监测服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Safety", Freeze = true)]
public class SafetyService : ISafetyService, ITransient
{
    private readonly ISqlSugarClient _db;

    public SafetyService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询实时数据
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<SafetyRealtimeOutput>> GetRealtimePage(SafetyRealtimePageInput input)
    {
        return await _db.Queryable<SafetyRealtime>()
            .LeftJoin<CoalMine>((s, m) => s.MineId == m.Id)
            .WhereIF(input.MineId.HasValue, (s, m) => s.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.SensorCode), (s, m) => s.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrEmpty(input.SensorName), (s, m) => s.SensorCode.Contains(input.SensorName))
            .WhereIF(!string.IsNullOrEmpty(input.SensorType), (s, m) => s.SensorCode.Contains(input.SensorType))
            .WhereIF(input.Status.HasValue, (s, m) => s.Status == input.Status)
            .Select((s, m) => new SafetyRealtimeOutput
            {
                Id = s.Id,
                MineId = s.MineId,
                MineName = m.Name,
                SensorCode = s.SensorCode,
                SensorName = SqlFunc.Substring(s.SensorCode, 15, 100), // 从传感器编号截取名称
                Value = s.Value,
                Unit = s.Unit,
                Status = s.Status,
                StatusName = GetStatusName(s.Status),
                UpdateTime = s.UpdateTime,
                ReceivedTime = s.ReceivedTime
            })
            .OrderBy(s => s.UpdateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 分页查询历史数据
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<SafetyHistory>> GetHistoryPage(SafetyHistoryPageInput input)
    {
        return await _db.Queryable<SafetyHistory>()
            .WhereIF(input.MineId.HasValue, s => s.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.SensorCode), s => s.SensorCode.Contains(input.SensorCode))
            .WhereIF(input.StartTime.HasValue, s => s.UpdateTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, s => s.UpdateTime <= input.EndTime)
            .OrderBy(s => s.UpdateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取实时数据详情
    /// </summary>
    public async Task<SafetyRealtime> GetRealtime(long id)
    {
        return await _db.Queryable<SafetyRealtime>()
            .Where(s => s.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 获取煤矿监测统计
    /// </summary>
    public async Task<List<SafetyStatisticsOutput>> GetStatistics(long? mineId)
    {
        var list = await _db.Queryable<CoalMine>()
            .WhereIF(mineId.HasValue, c => c.Id == mineId)
            .Select(c => new SafetyStatisticsOutput
            {
                MineId = c.Id,
                MineName = c.Name
            })
            .ToListAsync();

        foreach (var item in list)
        {
            var stats = await _db.Queryable<SafetyRealtime>()
                .Where(s => s.MineId == item.MineId)
                .GroupBy(s => s.Status)
                .Select(s => new { Status = s.Status, Count = SqlFunc.Count(s.Id) })
                .ToListAsync();

            item.TotalCount = stats.Sum(s => s.Count);
            item.NormalCount = stats.FirstOrDefault(s => s.Status == 0)?.Count ?? 0;
            item.AlarmCount = stats.FirstOrDefault(s => s.Status == 1)?.Count ?? 0;
            item.PowerOffCount = stats.FirstOrDefault(s => s.Status == 2)?.Count ?? 0;
            item.FaultCount = stats.FirstOrDefault(s => s.Status == 3)?.Count ?? 0;
        }

        return list;
    }

    /// <summary>
    /// 获取传感器类型列表
    /// </summary>
    public async Task<List<string>> GetSensorTypes()
    {
        // 从传感器编号中提取类型码
        var list = await _db.Queryable<SafetyRealtime>()
            .Select(s => SqlFunc.Substring(s.SensorCode, 13, 3))
            .Distinct()
            .ToListAsync();
        return list;
    }

    /// <summary>
    /// 获取某煤矿某类型传感器的最新数据
    /// </summary>
    public async Task<List<SafetyRealtimeOutput>> GetRealtimeByType(long mineId, string sensorType)
    {
        return await _db.Queryable<SafetyRealtime>()
            .LeftJoin<CoalMine>((s, m) => s.MineId == m.Id)
            .Where((s, m) => s.MineId == mineId)
            .Where((s, m) => SqlFunc.Substring(s.SensorCode, 13, 3) == sensorType)
            .Select((s, m) => new SafetyRealtimeOutput
            {
                Id = s.Id,
                MineId = s.MineId,
                MineName = m.Name,
                SensorCode = s.SensorCode,
                Value = s.Value,
                Unit = s.Unit,
                Status = s.Status,
                StatusName = GetStatusName(s.Status),
                UpdateTime = s.UpdateTime
            })
            .ToListAsync();
    }

    /// <summary>
    /// 手动保存实时数据
    /// </summary>
    public async Task<long> SaveRealtime(SafetyRealtime realtime)
    {
        realtime.ReceivedTime = DateTime.Now;
        return await _db.Insertable(realtime).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 批量保存实时数据
    /// </summary>
    public async Task<int> BatchSaveRealtime(List<SafetyRealtime> list)
    {
        if (list == null || list.Count == 0) return 0;

        // 按测点编号分组，存在则更新，不存在则插入
        int count = 0;
        foreach (var item in list)
        {
            item.ReceivedTime = DateTime.Now;
            var existing = await _db.Queryable<SafetyRealtime>()
                .Where(s => s.MineId == item.MineId && s.SensorCode == item.SensorCode)
                .FirstAsync();

            if (existing != null)
            {
                existing.Value = item.Value;
                existing.Unit = item.Unit;
                existing.Status = item.Status;
                existing.UpdateTime = item.UpdateTime;
                existing.ReceivedTime = DateTime.Now;
                await _db.Updateable(existing).ExecuteCommandAsync();
            }
            else
            {
                await _db.Insertable(item).ExecuteReturnIdentityAsync();
            }
            count++;
        }

        return count;
    }

    /// <summary>
    /// 删除实时数据
    /// </summary>
    public async Task DeleteRealtime(long id)
    {
        await _db.Deleteable<SafetyRealtime>(id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取报警中的传感器列表
    /// </summary>
    public async Task<List<SafetyRealtimeOutput>> GetAlarmList(long? mineId)
    {
        return await _db.Queryable<SafetyRealtime>()
            .LeftJoin<CoalMine>((s, m) => s.MineId == m.Id)
            .Where((s, m) => s.Status == 1 || s.Status == 2) // 报警或断电
            .WhereIF(mineId.HasValue, (s, m) => s.MineId == mineId)
            .Select((s, m) => new SafetyRealtimeOutput
            {
                Id = s.Id,
                MineId = s.MineId,
                MineName = m.Name,
                SensorCode = s.SensorCode,
                Value = s.Value,
                Unit = s.Unit,
                Status = s.Status,
                StatusName = GetStatusName(s.Status),
                UpdateTime = s.UpdateTime
            })
            .OrderBy(s => s.UpdateTime, OrderByType.Desc)
            .ToListAsync();
    }

    /// <summary>
    /// 获取实时数据趋势
    /// </summary>
    public async Task<List<Dictionary<string, object>>> GetTrendData(long mineId, string sensorCode, int hours = 24)
    {
        var startTime = DateTime.Now.AddHours(-hours);
        var list = await _db.Queryable<SafetyHistory>()
            .Where(s => s.MineId == mineId && s.SensorCode == sensorCode)
            .Where(s => s.UpdateTime >= startTime)
            .OrderBy(s => s.UpdateTime)
            .Select(s => new { s.UpdateTime, s.Value })
            .ToListAsync();

        return list.Select(s => new Dictionary<string, object>
        {
            { "time", s.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss") },
            { "value", s.Value ?? 0 }
        }).ToList();
    }

    private string GetStatusName(int status)
    {
        return status switch
        {
            0 => "正常",
            1 => "报警",
            2 => "断电",
            3 => "故障",
            4 => "离线",
            _ => "未知"
        };
    }
}

using Admin.NET.Core;
using Admin.NET.EntityFramework.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 报警配置服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "AlarmConfig", Order = 100)]
public class AlarmConfigService : IAlarmConfigService, ITransient
{
    private readonly ISqlSugarClient _db;

    public AlarmConfigService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询报警配置
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<AlarmConfig>> GetPage(AlarmConfigPageInput input)
    {
        return await _db.Queryable<AlarmConfig>()
            .LeftJoin<CoalMine>((a, m) => a.MineId == m.Id)
            .WhereIF(input.MineId.HasValue, (a, m) => a.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.SensorTypeCode), (a, m) => a.SensorTypeCode.Contains(input.SensorTypeCode))
            .WhereIF(input.AlarmType.HasValue, (a, m) => a.AlarmType == input.AlarmType)
            .WhereIF(input.AlarmEnabled.HasValue, (a, m) => a.AlarmEnabled == input.AlarmEnabled)
            .Select((a, m) => new AlarmConfig
            {
                Id = a.Id,
                MineId = a.MineId,
                MineName = m.Name,
                SensorTypeCode = a.SensorTypeCode,
                SensorTypeName = a.SensorTypeName,
                AlarmType = a.AlarmType,
                AlarmLevel = a.AlarmLevel,
                ThresholdValue = a.ThresholdValue,
                ThresholdValue2 = a.ThresholdValue2,
                DelaySeconds = a.DelaySeconds,
                AlarmEnabled = a.AlarmEnabled,
                Remark = a.Remark,
                CreateTime = a.CreateTime
            })
            .OrderBy(a => a.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取报警配置详情
    /// </summary>
    public async Task<AlarmConfig> Get(long id)
    {
        return await _db.Queryable<AlarmConfig>()
            .Where(a => a.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 新增报警配置
    /// </summary>
    public async Task<long> Add(AlarmConfig input)
    {
        // 检查是否已存在相同配置
        var exists = await _db.Queryable<AlarmConfig>()
            .Where(a => a.MineId == input.MineId && a.SensorTypeCode == input.SensorTypeCode && a.AlarmType == input.AlarmType)
            .FirstAsync();

        if (exists != null)
        {
            throw Oops.Oh("该报警配置已存在");
        }

        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新报警配置
    /// </summary>
    public async Task Update(AlarmConfig input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除报警配置
    /// </summary>
    public async Task Delete(long id)
    {
        await _db.Deleteable<AlarmConfig>(id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取传感器类型列表
    /// </summary>
    public async Task<List<Dictionary<string, string>>> GetSensorTypes()
    {
        var types = new List<Dictionary<string, string>>
        {
            new() { { "code", "CDSS" }, { "name", "安全监测-模拟量" } },
            new() { { "code", "CDDY" }, { "name", "安全监测-开关量" } },
            new() { { "code", "FZSS" }, { "name", "安全监测-分站" } },
            new() { { "code", "KGBH" }, { "name", "安全监测-馈电" } },
            new() { { "code", "TJSJ" }, { "name", "安全监测-提升" } },
            new() { { "code", "YCBJ" }, { "name", "安全监测-远程" } },
            new() { { "code", "CGKCDSS" }, { "name", "水害-水仓水位" } },
            new() { { "code", "PSLCDSS" }, { "name", "水害-排水流量" } },
            new() { { "code", "JSLCDSS" }, { "name", "水害-井下水仓" } }
        };
        return types;
    }

    /// <summary>
    /// 获取报警类型列表
    /// </summary>
    public async Task<List<Dictionary<string, int>>> GetAlarmTypes()
    {
        var types = new List<Dictionary<string, int>>
        {
            new() { { "name", "超过阈值" }, { "value", 1 } },
            new() { { "name", "低于阈值" }, { "value", 2 } },
            new() { { "name", "异常报警" }, { "value", 3 } },
            new() { { "name", "设备故障" }, { "value", 4 } },
            new() { { "name", "通讯中断" }, { "value", 5 } }
        };
        return types;
    }

    /// <summary>
    /// 获取报警级别列表
    /// </summary>
    public async Task<List<Dictionary<string, int>>> GetAlarmLevels()
    {
        var levels = new List<Dictionary<string, int>>
        {
            new() { { "name", "一般" }, { "value", 1 } },
            new() { { "name", "重要" }, { "value", 2 } },
            new() { { "name", "紧急" }, { "value", 3 } },
            new() { { "name", "严重" }, { "value", 4 } }
        };
        return levels;
    }
}

/// <summary>
/// 报警记录服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "AlarmRecord", Order = 100)]
public class AlarmRecordService : IAlarmRecordService, ITransient
{
    private readonly ISqlSugarClient _db;

    public AlarmRecordService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询报警记录
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<AlarmRecord>> GetPage(AlarmRecordPageInput input)
    {
        return await _db.Queryable<AlarmRecord>()
            .LeftJoin<CoalMine>((a, m) => a.MineId == m.Id)
            .LeftJoin<SysUser>((a, m, u) => a.ConfirmUserId == u.Id)
            .WhereIF(input.MineId.HasValue, (a, m, u) => a.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.SensorCode), (a, m, u) => a.SensorCode.Contains(input.SensorCode))
            .WhereIF(input.AlarmLevel.HasValue, (a, m, u) => a.AlarmLevel == input.AlarmLevel)
            .WhereIF(input.Status.HasValue, (a, m, u) => a.Status == input.Status)
            .WhereIF(input.StartTime.HasValue, (a, m, u) => a.AlarmTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, (a, m, u) => a.AlarmTime <= input.EndTime)
            .Select((a, m, u) => new AlarmRecord
            {
                Id = a.Id,
                MineId = a.MineId,
                MineName = m.Name,
                SensorCode = a.SensorCode,
                SensorName = a.SensorName,
                SensorTypeName = a.SensorTypeName,
                AlarmType = a.AlarmType,
                AlarmLevel = a.AlarmLevel,
                AlarmValue = a.AlarmValue,
                ThresholdValue = a.ThresholdValue,
                AlarmTime = a.AlarmTime,
                ConfirmTime = a.ConfirmTime,
                ConfirmUserName = u.RealName,
                ResolveTime = a.ResolveTime,
                Status = a.Status,
                Remark = a.Remark
            })
            .OrderBy(a => a.AlarmTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取报警记录详情
    /// </summary>
    public async Task<AlarmRecord> Get(long id)
    {
        return await _db.Queryable<AlarmRecord>()
            .Where(a => a.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 确认报警
    /// </summary>
    public async Task Confirm(AlarmConfirmInput input)
    {
        var alarm = await _db.Queryable<AlarmRecord>()
            .Where(a => a.Id == input.Id)
            .FirstAsync();

        if (alarm == null)
        {
            throw Oops.Oh("报警记录不存在");
        }

        if (alarm.Status != 0)
        {
            throw Oops.Oh("该报警已被处理");
        }

        alarm.ConfirmTime = DateTime.Now;
        alarm.ConfirmUserId = App.User?.FindFirst(ClaimConst.UserId)?.Value.ParseToLong();
        alarm.ConfirmUserName = App.User?.FindFirst(ClaimConst.RealName)?.Value;
        alarm.Status = 1; // 已确认

        await _db.Updateable(alarm).ExecuteCommandAsync();
    }

    /// <summary>
    /// 解除报警
    /// </summary>
    public async Task Resolve(AlarmResolveInput input)
    {
        var alarm = await _db.Queryable<AlarmRecord>()
            .Where(a => a.Id == input.Id)
            .FirstAsync();

        if (alarm == null)
        {
            throw Oops.Oh("报警记录不存在");
        }

        alarm.ResolveTime = DateTime.Now;
        alarm.ResolveUserId = App.User?.FindFirst(ClaimConst.UserId)?.Value.ParseToLong();
        alarm.ResolveUserName = App.User?.FindFirst(ClaimConst.RealName)?.Value;
        alarm.Status = 2; // 已解除
        alarm.Remark = input.Remark;

        await _db.Updateable(alarm).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取未处理报警数量
    /// </summary>
    public async Task<int> GetUnprocessedCount(long? mineId)
    {
        return await _db.Queryable<AlarmRecord>()
            .WhereIF(mineId.HasValue, a => a.MineId == mineId)
            .Where(a => a.Status == 0)
            .CountAsync();
    }

    /// <summary>
    /// 获取今日报警统计
    /// </summary>
    public async Task<Dictionary<string, int>> GetTodayStatistics(long? mineId)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var list = await _db.Queryable<AlarmRecord>()
            .WhereIF(mineId.HasValue, a => a.MineId == mineId)
            .Where(a => a.AlarmTime >= today && a.AlarmTime < tomorrow)
            .GroupBy(a => a.AlarmLevel)
            .Select(a => new { AlarmLevel = a.AlarmLevel, Count = SqlFunc.Count(a.Id) })
            .ToListAsync();

        var result = new Dictionary<string, int>
        {
            { "total", list.Sum(a => a.Count) },
            { "level1", list.FirstOrDefault(a => a.AlarmLevel == 1)?.Count ?? 0 },
            { "level2", list.FirstOrDefault(a => a.AlarmLevel == 2)?.Count ?? 0 },
            { "level3", list.FirstOrDefault(a => a.AlarmLevel == 3)?.Count ?? 0 },
            { "level4", list.FirstOrDefault(a => a.AlarmLevel == 4)?.Count ?? 0 }
        };

        return result;
    }
}

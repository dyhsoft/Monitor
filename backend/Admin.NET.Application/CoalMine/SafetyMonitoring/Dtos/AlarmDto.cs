using Admin.NET.EntityFramework.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Admin.NET.Core;
using SqlSugar;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Furion.DynamicApiController;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application;

/// <summary>
/// 报警配置分页输入
/// </summary>
public class AlarmConfigPageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 传感器类型码
    /// </summary>
    public string? SensorTypeCode { get; set; }

    /// <summary>
    /// 报警类型
    /// </summary>
    public int? AlarmType { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int? AlarmEnabled { get; set; }
}

/// <summary>
/// 报警记录分页输入
/// </summary>
public class AlarmRecordPageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    public string? SensorCode { get; set; }

    /// <summary>
    /// 报警级别
    /// </summary>
    public int? AlarmLevel { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 报警确认输入
/// </summary>
public class AlarmConfirmInput
{
    /// <summary>
    /// 报警记录Id
    /// </summary>
    [Required]
    public long Id { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 报警解除输入
/// </summary>
public class AlarmResolveInput
{
    /// <summary>
    /// 报警记录Id
    /// </summary>
    [Required]
    public long Id { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 报警配置服务接口
/// </summary>
public interface IAlarmConfigService
{
    Task<SqlSugarPagedList<AlarmConfig>> GetPage(AlarmConfigPageInput input);
    Task<AlarmConfig> Get(long id);
    Task<long> Add(AlarmConfig input);
    Task Update(AlarmConfig input);
    Task Delete(long id);
    Task<List<Dictionary<string, string>>> GetSensorTypes();
    Task<List<Dictionary<string, int>>> GetAlarmTypes();
    Task<List<Dictionary<string, int>>> GetAlarmLevels();
}

/// <summary>
/// 报警记录服务接口
/// </summary>
public interface IAlarmRecordService
{
    Task<SqlSugarPagedList<AlarmRecord>> GetPage(AlarmRecordPageInput input);
    Task<AlarmRecord> Get(long id);
    Task Confirm(AlarmConfirmInput input);
    Task Resolve(AlarmResolveInput input);
    Task<int> GetUnprocessedCount(long? mineId);
    Task<Dictionary<string, int>> GetTodayStatistics(long? mineId);
}

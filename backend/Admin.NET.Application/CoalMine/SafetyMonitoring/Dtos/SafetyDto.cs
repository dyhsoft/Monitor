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
namespace Admin.NET.Application.CoalMine.SafetyMonitoring.Dtos;

/// <summary>
/// 安全监测实时数据分页查询
/// </summary>
public class SafetyRealtimePageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 测点名称
    /// </summary>
    public string SensorName { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 安全监测历史数据分页查询
/// </summary>
public class SafetyHistoryPageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }

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
/// 实时数据DTO（带煤矿信息）
/// </summary>
public class SafetyRealtimeOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 测点名称
    /// </summary>
    public string SensorName { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    public decimal? Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// 采集时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 接收时间
    /// </summary>
    public DateTime ReceivedTime { get; set; }
}

/// <summary>
/// 安全监测统计数据
/// </summary>
public class SafetyStatisticsOutput
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 传感器总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 正常数量
    /// </summary>
    public int NormalCount { get; set; }

    /// <summary>
    /// 报警数量
    /// </summary>
    public int AlarmCount { get; set; }

    /// <summary>
    /// 断电数量
    /// </summary>
    public int PowerOffCount { get; set; }

    /// <summary>
    /// 故障数量
    /// </summary>
    public int FaultCount { get; set; }

    /// <summary>
    /// 离线数量
    /// </summary>
    public int OfflineCount { get; set; }
}

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
using Admin.NET.ApplicationSafetyMonitoring.Dtos;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.SafetyMonitoring.Services;

/// <summary>
/// 安全监测服务接口
/// </summary>
public interface ISafetyService
{
    /// <summary>
    /// 分页查询实时数据
    /// </summary>
    Task<SqlSugarPagedList<SafetyRealtimeOutput>> GetRealtimePage(SafetyRealtimePageInput input);

    /// <summary>
    /// 分页查询历史数据
    /// </summary>
    Task<SqlSugarPagedList<SafetyHistory>> GetHistoryPage(SafetyHistoryPageInput input);

    /// <summary>
    /// 获取实时数据详情
    /// </summary>
    Task<SafetyRealtime> GetRealtime(long id);

    /// <summary>
    /// 获取煤矿监测统计
    /// </summary>
    Task<List<SafetyStatisticsOutput>> GetStatistics(long? mineId);

    /// <summary>
    /// 获取传感器类型列表
    /// </summary>
    Task<List<string>> GetSensorTypes();

    /// <summary>
    /// 获取某煤矿某类型传感器的最新数据
    /// </summary>
    Task<List<SafetyRealtimeOutput>> GetRealtimeByType(long mineId, string sensorType);

    /// <summary>
    /// 手动保存实时数据
    /// </summary>
    Task<long> SaveRealtime(SafetyRealtime realtime);

    /// <summary>
    /// 批量保存实时数据（用于解析服务调用）
    /// </summary>
    Task<int> BatchSaveRealtime(List<SafetyRealtime> list);

    /// <summary>
    /// 删除实时数据
    /// </summary>
    Task DeleteRealtime(long id);

    /// <summary>
    /// 获取报警中的传感器列表
    /// </summary>
    Task<List<SafetyRealtimeOutput>> GetAlarmList(long? mineId);

    /// <summary>
    /// 获取实时数据趋势（用于图表）
    /// </summary>
    Task<List<Dictionary<string, object>>> GetTrendData(long mineId, string sensorCode, int hours = 24);
}

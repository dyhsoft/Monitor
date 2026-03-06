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
namespace Admin.NET.ApplicationCoalDataAccess;

/// <summary>
/// 数据文件类型枚举
/// </summary>
public enum DataFileType
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,

    // ========== 安全监测 ==========
    /// <summary>
    /// 传感器定时数据
    /// </summary>
    CDSS = 1,
    /// <summary>
    /// 超限断电数据
    /// </summary>
    CDDY = 2,
    /// <summary>
    /// 分站实时数据
    /// </summary>
    FZSS = 3,
    /// <summary>
    /// 开关报警数据
    /// </summary>
    KGBH = 4,
    /// <summary>
    /// 调校数据
    /// </summary>
    TJSJ = 5,
    /// <summary>
    /// 异常报警数据
    /// </summary>
    YCBJ = 6,

    // ========== 人员定位 ==========
    /// <summary>
    /// 人员实时数据
    /// </summary>
    RYSS = 10,
    /// <summary>
    /// 人员初始化
    /// </summary>
    RYCS = 11,
    /// <summary>
    /// 人员出勤
    /// </summary>
    RYCY = 12,
    /// <summary>
    /// 人员区域
    /// </summary>
    RYQJ = 13,
    /// <summary>
    /// 基站实时
    /// </summary>
    JZSS = 14,

    // ========== 水害防治 ==========
    /// <summary>
    /// 水仓传感器定时
    /// </summary>
    CGKCDSS = 20,
    /// <summary>
    /// 水仓超限断电
    /// </summary>
    CGKCDDY = 21,
    /// <summary>
    /// 井下水仓流量
    /// </summary>
    JSLCDSS = 22,
    /// <summary>
    /// 排水流量
    /// </summary>
    PSLCDSS = 23
}

/// <summary>
/// 数据分类
/// </summary>
public enum DataCategory
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// 安全监测
    /// </summary>
    Safety = 1,
    /// <summary>
    /// 人员定位
    /// </summary>
    Person = 2,
    /// <summary>
    /// 水害防治
    /// </summary>
    Water = 3,
    /// <summary>
    /// 视频监控
    /// </summary>
    Video = 4
}

/// <summary>
/// 解析结果
/// </summary>
public class ParseResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public DataFileType FileType { get; set; }

    /// <summary>
    /// 煤矿编码
    /// </summary>
    public string MineCode { get; set; }

    /// <summary>
    /// 数据时间
    /// </summary>
    public DateTime DataTime { get; set; }

    /// <summary>
    /// 解析记录
    /// </summary>
    public List<Dictionary<string, object>> Records { get; set; } = new();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 解析耗时(毫秒)
    /// </summary>
    public int ParseTimeMs { get; set; }
}

/// <summary>
/// 解析配置
/// </summary>
public class ParseOptions
{
    /// <summary>
    /// 文件编码
    /// </summary>
    public string Encoding { get; set; } = "UTF-8";

    /// <summary>
    /// 字段分隔符
    /// </summary>
    public string FieldSeparator { get; set; } = ";";

    /// <summary>
    /// 记录分隔符
    /// </summary>
    public string RecordSeparator { get; set; } = "~";
}

/// <summary>
/// 传感器类型映射
/// </summary>
public static class SensorTypeMap
{
    private static readonly Dictionary<string, (string Name, string Unit)> _map = new()
    {
        // 气体传感器
        { "A01", ("甲烷", "%CH4") },
        { "A02", ("一氧化碳", "ppm") },
        { "A03", ("一氧化氮", "ppm") },
        { "A04", ("二氧化氮", "ppm") },
        { "A05", ("二氧化硫", "ppm") },
        { "A06", ("硫化氢", "ppm") },
        { "A07", ("氨气", "ppm") },
        { "A08", ("氯气", "ppm") },
        { "A09", ("氢气", "%") },
        { "A10", ("氧气", "%") },
        { "A11", ("二氧化碳", "%") },
        // 环境传感器
        { "A12", ("粉尘", "mg/m³") },
        { "A13", ("温度", "℃") },
        { "A14", ("湿度", "%RH") },
        { "A15", ("风速", "m/s") },
        { "A16", ("风向", "") },
        { "A17", ("风压", "kPa") },
        // 水害传感器
        { "A20", ("水位", "m") },
        { "A21", ("流量", "m³/min") },
        // 抽采传感器
        { "A25", ("管道压力", "kPa") },
        { "A26", ("管道温度", "℃") },
        { "A27", ("管道流量", "m³/min") },
        { "A38", ("管道一氧化碳", "ppm") },
        { "A40", ("管道瓦斯", "%CH4") },
        // 开关量
        { "B01", ("烟雾", "") },
        { "B02", ("风门", "") },
        { "B03", ("设备开停", "") },
        { "B04", ("馈电状态", "") },
        { "B05", ("断电器", "") }
    };

    public static (string Name, string Unit)? GetSensorInfo(string typeCode)
    {
        if (string.IsNullOrEmpty(typeCode)) return null;
        return _map.TryGetValue(typeCode.ToUpper(), out var info) ? info : null;
    }

    public static string GetSensorName(string typeCode)
    {
        return GetSensorInfo(typeCode)?.Name ?? typeCode;
    }

    public static string GetSensorUnit(string typeCode)
    {
        return GetSensorInfo(typeCode)?.Unit ?? "";
    }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 分站状态数据（FZSS）
/// </summary>
[SugarTable("StationData", "分站状态数据表")]
public class StationData : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 分站编号
    /// </summary>
    [SugarColumn(ColumnDescription = "分站编号", Length = 50)]
    public string StationCode { get; set; }

    /// <summary>
    /// 分站名称
    /// </summary>
    [SugarColumn(ColumnDescription = "分站名称", Length = 100, IsNullable = true)]
    public string StationName { get; set; }

    /// <summary>
    /// 负压值(Pa)
    /// </summary>
    [SugarColumn(ColumnDescription = "负压值(Pa)", IsNullable = true)]
    public decimal? Pressure { get; set; }

    /// <summary>
    /// 通风量(m³/min)
    /// </summary>
    [SugarColumn(ColumnDescription = "通风量(m³/min)", IsNullable = true)]
    public decimal? AirFlow { get; set; }

    /// <summary>
    /// 状态:0-正常,1-报警,2-故障
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "0")]
    public int Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 开关量报警数据（KGBH）
/// </summary>
[SugarTable("SwitchAlarmRecords", "开关量报警记录表")]
public class SwitchAlarmRecord : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器编号", Length = 50)]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器名称", Length = 100, IsNullable = true)]
    public string SensorName { get; set; }

    /// <summary>
    /// 开关状态:0-断开,1-闭合
    /// </summary>
    [SugarColumn(ColumnDescription = "开关状态")]
    public int SwitchStatus { get; set; }

    /// <summary>
    /// 报警类型
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型", Length = 50, IsNullable = true)]
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警时间
    /// </summary>
    [SugarColumn(ColumnDescription = "报警时间")]
    public DateTime AlarmTime { get; set; }

    /// <summary>
    /// 状态:0-未处理,1-已处理
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "0")]
    public int Status { get; set; }
}

/// <summary>
/// 统计汇总数据（TJSJ）
/// </summary>
[SugarTable("StatisticsData", "统计汇总数据表")]
public class StatisticsData : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 统计类型
    /// </summary>
    [SugarColumn(ColumnDescription = "统计类型", Length = 50)]
    public string StatType { get; set; }

    /// <summary>
    /// 统计值
    /// </summary>
    [SugarColumn(ColumnDescription = "统计值")]
    public decimal Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [SugarColumn(ColumnDescription = "单位", Length = 20, IsNullable = true)]
    public string Unit { get; set; }

    /// <summary>
    /// 统计时间
    /// </summary>
    [SugarColumn(ColumnDescription = "统计时间")]
    public DateTime StatTime { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}

/// <summary>
/// 异常报警数据（YCBJ）
/// </summary>
[SugarTable("AbnormalAlarms", "异常报警记录表")]
public class AbnormalAlarm : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器编号", Length = 50, IsNullable = true)]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器名称", Length = 100, IsNullable = true)]
    public string SensorName { get; set; }

    /// <summary>
    /// 报警类型
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型", Length = 50)]
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警内容
    /// </summary>
    [SugarColumn(ColumnDescription = "报警内容", Length = 500)]
    public string AlarmMessage { get; set; }

    /// <summary>
    /// 报警级别:1-一般,2-重要,3-紧急
    /// </summary>
    [SugarColumn(ColumnDescription = "报警级别", DefaultValue = "1")]
    public int AlarmLevel { get; set; } = 1;

    /// <summary>
    /// 报警时间
    /// </summary>
    [SugarColumn(ColumnDescription = "报警时间")]
    public DateTime AlarmTime { get; set; }

    /// <summary>
    /// 状态:0-未处理,1-已处理,2-已确认
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "0")]
    public int Status { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    [SugarColumn(ColumnDescription = "处理时间", IsNullable = true)]
    public DateTime? HandleTime { get; set; }

    /// <summary>
    /// 处理人
    /// </summary>
    [SugarColumn(ColumnDescription = "处理人", Length = 50, IsNullable = true)]
    public string Handler { get; set; }
}

/// <summary>
/// 人员区域统计（RYQJ）
/// </summary>
[SugarTable("PersonAreaStatistics", "人员区域统计表")]
public class PersonAreaStatistic : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    [SugarColumn(ColumnDescription = "区域编码", Length = 50)]
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [SugarColumn(ColumnDescription = "区域名称", Length = 100)]
    public string AreaName { get; set; }

    /// <summary>
    /// 区域类型:1-采煤面,2-掘进面,3-巷道,4-硐室
    /// </summary>
    [SugarColumn(ColumnDescription = "区域类型")]
    public int AreaType { get; set; }

    /// <summary>
    /// 当前人数
    /// </summary>
    [SugarColumn(ColumnDescription = "当前人数")]
    public int PersonCount { get; set; }

    /// <summary>
    /// 限定人数
    /// </summary>
    [SugarColumn(ColumnDescription = "限定人数")]
    public int LimitCount { get; set; }

    /// <summary>
    /// 统计时间
    /// </summary>
    [SugarColumn(ColumnDescription = "统计时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 皮带秤数据（JSLCDSS）
/// </summary>
[SugarTable("BeltScaleData", "皮带秤数据表")]
public class BeltScaleData : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 皮带编号
    /// </summary>
    [SugarColumn(ColumnDescription = "皮带编号", Length = 50)]
    public string BeltCode { get; set; }

    /// <summary>
    /// 皮带名称
    /// </summary>
    [SugarColumn(ColumnDescription = "皮带名称", Length = 100, IsNullable = true)]
    public string BeltName { get; set; }

    /// <summary>
    /// 瞬时流量(t/h)
    /// </summary>
    [SugarColumn(ColumnDescription = "瞬时流量(t/h)", IsNullable = true)]
    public decimal? InstantFlow { get; set; }

    /// <summary>
    /// 累计流量(t)
    /// </summary>
    [SugarColumn(ColumnDescription = "累计流量(t)", IsNullable = true)]
    public decimal? TotalFlow { get; set; }

    /// <summary>
    /// 皮带速度(m/s)
    /// </summary>
    [SugarColumn(ColumnDescription = "皮带速度(m/s)", IsNullable = true)]
    public decimal? BeltSpeed { get; set; }

    /// <summary>
    /// 状态:0-正常,1-故障
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "0")]
    public int Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 排水量数据（PSLCDSS）
/// </summary>
[SugarTable("DrainageData", "排水量数据表")]
public class DrainageData : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 水泵编号
    /// </summary>
    [SugarColumn(ColumnDescription = "水泵编号", Length = 50)]
    public string PumpCode { get; set; }

    /// <summary>
    /// 水泵名称
    /// </summary>
    [SugarColumn(ColumnDescription = "水泵名称", Length = 100, IsNullable = true)]
    public string PumpName { get; set; }

    /// <summary>
    /// 瞬时流量(m³/h)
    /// </summary>
    [SugarColumn(ColumnDescription = "瞬时流量(m³/h)", IsNullable = true)]
    public decimal? InstantFlow { get; set; }

    /// <summary>
    /// 累计流量(m³)
    /// </summary>
    [SugarColumn(ColumnDescription = "累计流量(m³)", IsNullable = true)]
    public decimal? TotalFlow { get; set; }

    /// <summary>
    /// 运行状态:0-停止,1-运行
    /// </summary>
    [SugarColumn(ColumnDescription = "运行状态", DefaultValue = "0")]
    public int RunStatus { get; set; }

    /// <summary>
    /// 水位(m)
    /// </summary>
    [SugarColumn(ColumnDescription = "水位(m)", IsNullable = true)]
    public decimal? WaterLevel { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

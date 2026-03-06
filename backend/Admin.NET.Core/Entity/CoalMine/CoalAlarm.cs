using SqlSugar;

namespace Admin.NET.Core;

/// <summary>
/// 报警消息配置实体
/// </summary>
[SugarTable("CoalAlarmConfig", "报警消息配置表")]
public class CoalAlarmConfig : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 报警类型（1-安全监测 2-人员定位 3-水害监测 4-视频监控）
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型")]
    public int AlarmType { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 接收人员Id列表（逗号分隔）
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "接收人员Id列表")]
    public string UserIds { get; set; }

    /// <summary>
    /// 接收人员姓名列表（逗号分隔）
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "接收人员姓名列表")]
    public string UserNames { get; set; }

    /// <summary>
    /// 通知方式（1-系统消息 2-短信 3-邮件）
    /// </summary>
    [SugarColumn(ColumnDescription = "通知方式")]
    public int NotifyType { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

/// <summary>
/// 报警记录实体
/// </summary>
[SugarTable("CoalAlarmRecord", "报警记录表")]
public class CoalAlarmRecord : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "煤矿名称")]
    public string MineName { get; set; }

    /// <summary>
    /// 报警类型（1-安全监测 2-人员定位 3-水害监测 4-视频监控）
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型")]
    public int AlarmType { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "传感器名称")]
    public string SensorName { get; set; }

    /// <summary>
    /// 报警值
    /// </summary>
    [SugarColumn(ColumnDescription = "报警值", DecimalDigits = 2)]
    public decimal? AlarmValue { get; set; }

    /// <summary>
    /// 报警阈值
    /// </summary>
    [SugarColumn(ColumnDescription = "报警阈值", DecimalDigits = 2)]
    public decimal? ThresholdValue { get; set; }

    /// <summary>
    /// 报警等级（1-低 2-中 3-高 4-紧急）
    /// </summary>
    [SugarColumn(ColumnDescription = "报警等级")]
    public int AlarmLevel { get; set; } = 1;

    /// <summary>
    /// 报警描述
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "报警描述")]
    public string Description { get; set; }

    /// <summary>
    /// 报警时间
    /// </summary>
    [SugarColumn(ColumnDescription = "报警时间")]
    public DateTime AlarmTime { get; set; }

    /// <summary>
    /// 处理状态（0-未处理 1-已处理 2-已忽略）
    /// </summary>
    [SugarColumn(ColumnDescription = "处理状态")]
    public int Status { get; set; } = 0;

    /// <summary>
    /// 处理人
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "处理人")]
    public string Handler { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    [SugarColumn(ColumnDescription = "处理时间")]
    public DateTime? HandleTime { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "处理备注")]
    public string HandleRemark { get; set; }
}

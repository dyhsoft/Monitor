namespace Admin.NET.Core;

/// <summary>
/// 通用报警配置表
/// </summary>
[SugarTable("AlarmConfigs", "报警配置表")]
public class AlarmConfig : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 报警名称
    /// </summary>
    [SugarColumn(ColumnDescription = "报警名称", Length = 100)]
    public string AlarmName { get; set; }

    /// <summary>
    /// 报警编码（唯一标识）
    /// </summary>
    [SugarColumn(ColumnDescription = "报警编码", Length = 50)]
    public string AlarmCode { get; set; }

    /// <summary>
    /// 报警类别：传感器报警、设备监控、数据监控、人员行为、智能预测
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类别", Length = 20)]
    public string AlarmCategory { get; set; }

    /// <summary>
    /// 报警级别：1-一般,2-重要,3-紧急,4-预警
    /// </summary>
    [SugarColumn(ColumnDescription = "报警级别", DefaultValue = "1")]
    public int AlarmLevel { get; set; } = 1;

    /// <summary>
    /// 报警条件（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDescription = "报警条件", Length = 2000, IsNullable = true)]
    public string Condition { get; set; }

    /// <summary>
    /// 描述说明
    /// </summary>
    [SugarColumn(ColumnDescription = "描述说明", Length = 500, IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 时间阈值(秒)
    /// </summary>
    [SugarColumn(ColumnDescription = "时间阈值(秒)", DefaultValue = "0")]
    public int TimeThreshold { get; set; } = 0;

    /// <summary>
    /// 通知人员ID列表(JSON): [1,2,3]
    /// </summary>
    [SugarColumn(ColumnDescription = "通知人员ID列表", Length = 1000, IsNullable = true)]
    public string NotifyUserIds { get; set; }

    /// <summary>
    /// 通知人员名称列表(JSON): ["张三","李四"]
    /// </summary>
    [SugarColumn(ColumnDescription = "通知人员名称列表", Length = 1000, IsNullable = true)]
    public string NotifyUserNames { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int Enabled { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}

/// <summary>
/// 报警通知记录表
/// </summary>
[SugarTable("AlarmNotifyRecords", "报警通知记录表")]
public class AlarmNotifyRecord : EntityBase
{
    /// <summary>
    /// 报警记录ID
    /// </summary>
    [SugarColumn(ColumnDescription = "报警记录ID")]
    public long AlarmRecordId { get; set; }

    /// <summary>
    /// 通知人员ID
    /// </summary>
    [SugarColumn(ColumnDescription = "通知人员ID")]
    public long NotifyUserId { get; set; }

    /// <summary>
    /// 通知人员名称
    /// </summary>
    [SugarColumn(ColumnDescription = "通知人员名称", Length = 50, IsNullable = true)]
    public string NotifyUserName { get; set; }

    /// <summary>
    /// 通知方式：1-系统消息,2-短信,3-微信
    /// </summary>
    [SugarColumn(ColumnDescription = "通知方式", DefaultValue = "1")]
    public int NotifyType { get; set; } = 1;

    /// <summary>
    /// 通知状态：0-待发送,1-已发送,2-发送失败
    /// </summary>
    [SugarColumn(ColumnDescription = "通知状态", DefaultValue = "0")]
    public int Status { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [SugarColumn(ColumnDescription = "发送时间", IsNullable = true)]
    public DateTime? SendTime { get; set; }

    /// <summary>
    /// 失败原因
    /// </summary>
    [SugarColumn(ColumnDescription = "失败原因", Length = 500, IsNullable = true)]
    public string FailReason { get; set; }
}

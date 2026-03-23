// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 报警配置表
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
    /// 传感器类型代码
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器类型代码", Length = 20)]
    public string SensorTypeCode { get; set; }

    /// <summary>
    /// 传感器类型名称
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器类型名称", Length = 100, IsNullable = true)]
    public string SensorTypeName { get; set; }

    /// <summary>
    /// 报警类型:上限/下限/突变/断线
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型", Length = 20)]
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警级别:1-一般,2-重要,3-紧急
    /// </summary>
    [SugarColumn(ColumnDescription = "报警级别", DefaultValue = "1")]
    public int AlarmLevel { get; set; } = 1;

    /// <summary>
    /// 阈值
    /// </summary>
    [SugarColumn(ColumnDescription = "阈值", DecimalDigits = 2)]
    public decimal ThresholdValue { get; set; }

    /// <summary>
    /// 延迟时间(秒)
    /// </summary>
    [SugarColumn(ColumnDescription = "延迟时间(秒)", DefaultValue = "0")]
    public int DelaySeconds { get; set; } = 0;

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int AlarmEnabled { get; set; } = 1;

    /// <summary>
    /// 通知人员
    /// </summary>
    [SugarColumn(ColumnDescription = "通知人员", Length = 500, IsNullable = true)]
    public string NotifyUsers { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}

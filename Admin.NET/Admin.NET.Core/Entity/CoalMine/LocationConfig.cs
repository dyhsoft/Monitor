// 人员定位配置实体
namespace Admin.NET.Core;

/// <summary>
/// 矿领导配置表
/// </summary>
[SugarTable("LocationLeaderConfigs", "矿领导配置表")]
public class LocationLeaderConfig : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 员工姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "员工姓名", Length = 50)]
    public string PersonName { get; set; }

    /// <summary>
    /// 卡号
    /// </summary>
    [SugarColumn(ColumnDescription = "卡号", Length = 50)]
    public string CardId { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(ColumnDescription = "部门", Length = 100, IsNullable = true)]
    public string DeptName { get; set; }

    /// <summary>
    /// 职务
    /// </summary>
    [SugarColumn(ColumnDescription = "职务", Length = 50, IsNullable = true)]
    public string Position { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int Enabled { get; set; } = 1;
}

/// <summary>
/// 限定人数配置表
/// </summary>
[SugarTable("LocationLimitConfigs", "限定人数配置表")]
public class LocationLimitConfig : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    [SugarColumn(ColumnDescription = "区域编码", Length = 20, IsNullable = true)]
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [SugarColumn(ColumnDescription = "区域名称", Length = 100, IsNullable = true)]
    public string AreaName { get; set; }

    /// <summary>
    /// 限定人数
    /// </summary>
    [SugarColumn(ColumnDescription = "限定人数")]
    public int LimitCount { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int Enabled { get; set; } = 1;
}

/// <summary>
/// 定位报警记录表
/// </summary>
[SugarTable("LocationAlarms", "定位报警记录表")]
public class LocationAlarm : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 报警类型:1-超时,2-超员,3-基站报警
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型")]
    public int AlarmType { get; set; }

    /// <summary>
    /// 卡号
    /// </summary>
    [SugarColumn(ColumnDescription = "卡号", Length = 50, IsNullable = true)]
    public string CardId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "姓名", Length = 50, IsNullable = true)]
    public string PersonName { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    [SugarColumn(ColumnDescription = "区域", Length = 100, IsNullable = true)]
    public string AreaName { get; set; }

    /// <summary>
    /// 报警内容
    /// </summary>
    [SugarColumn(ColumnDescription = "报警内容", Length = 500)]
    public string AlarmMessage { get; set; }

    /// <summary>
    /// 状态:0-未处理,1-已处理
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "0")]
    public int Status { get; set; }

    /// <summary>
    /// 报警时间
    /// </summary>
    [SugarColumn(ColumnDescription = "报警时间")]
    public DateTime AlarmTime { get; set; }
}

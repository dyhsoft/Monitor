// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 煤矿表
/// </summary>
[SugarTable("CoalMines", "煤矿表")]
public class CoalMine : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿编码
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿编码", Length = 20)]
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿名称", Length = 100)]
    public string Name { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", Length = 200, IsNullable = true)]
    public string Address { get; set; }

    /// <summary>
    /// FTP根目录
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP根目录", Length = 500, IsNullable = true)]
    public string FtpPath { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据类型", Length = 20, IsNullable = true)]
    public string DataType { get; set; }

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
/// 区域表
/// </summary>
[SugarTable("CoalMineAreas", "区域表")]
public class CoalMineArea : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(MineId))]
    public CoalMine CoalMine { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    [SugarColumn(ColumnDescription = "区域编码", Length = 20)]
    public string Code { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [SugarColumn(ColumnDescription = "区域名称", Length = 100)]
    public string Name { get; set; }

    /// <summary>
    /// 父区域ID
    /// </summary>
    [SugarColumn(ColumnDescription = "父区域ID", IsNullable = true)]
    public long? ParentId { get; set; }

    /// <summary>
    /// 区域类型:1-大巷,2-工作面,3-硐室
    /// </summary>
    [SugarColumn(ColumnDescription = "区域类型", DefaultValue = "1")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 大地坐标X
    /// </summary>
    [SugarColumn(ColumnDescription = "大地坐标X", IsNullable = true)]
    public decimal? X { get; set; }

    /// <summary>
    /// 大地坐标Y
    /// </summary>
    [SugarColumn(ColumnDescription = "大地坐标Y", IsNullable = true)]
    public decimal? Y { get; set; }

    /// <summary>
    /// 深度
    /// </summary>
    [SugarColumn(ColumnDescription = "深度", IsNullable = true)]
    public decimal? Z { get; set; }

    /// <summary>
    /// 容纳人数
    /// </summary>
    [SugarColumn(ColumnDescription = "容纳人数", IsNullable = true)]
    public int? Capacity { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int Enabled { get; set; } = 1;
}

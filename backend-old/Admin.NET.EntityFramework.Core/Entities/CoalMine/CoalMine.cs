using Furion.DatabaseAccessor;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.EntityFramework.Core;

/// <summary>
/// 煤矿实体
/// </summary>
[SugarTable("CoalMine", "煤矿表")]
[Tenant(Seven = true)]
public class CoalMine : EntityBase
{
    /// <summary>
    /// 煤矿编码（10位）
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "煤矿编码")]
    [Required]
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "煤矿名称")]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 所属集团
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "所属集团")]
    public string GroupName { get; set; }

    /// <summary>
    /// 省份编码
    /// </summary>
    [SugarColumn(Length = 2, ColumnDescription = "省份编码")]
    public string Province { get; set; }

    /// <summary>
    /// 城市编码
    /// </summary>
    [SugarColumn(Length = 2, ColumnDescription = "城市编码")]
    public string City { get; set; }

    /// <summary>
    /// 县区编码
    /// </summary>
    [SugarColumn(Length = 2, ColumnDescription = "县区编码")]
    public string County { get; set; }

    /// <summary>
    /// 矿井类型（生产/建设）
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "矿井类型")]
    public string MineType { get; set; }

    /// <summary>
    /// 设计产能（万吨/年）
    /// </summary>
    [SugarColumn(ColumnDescription = "设计产能")]
    public int? DesignCapacity { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "联系人")]
    public string Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "详细地址")]
    public string Address { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    [SugarColumn(ColumnDescription = "经度", DecimalDigits = 6)]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    [SugarColumn(ColumnDescription = "纬度", DecimalDigits = 6)]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 状态（0=停用,1=启用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

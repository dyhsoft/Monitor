using SqlSugar;

namespace Admin.NET.Core.Entity.CoalMine;

/// <summary>
/// 煤矿表
/// </summary>
[SugarTable("CoalMine", "煤矿表")]
public class CoalMine : EntityBase
{
    /// <summary>
    /// 煤矿编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "煤矿编号")]
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "煤矿名称")]
    public string Name { get; set; }

    /// <summary>
    /// 所属集团
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "所属集团")]
    public string GroupName { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "省份")]
    public string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "城市")]
    public string City { get; set; }

    /// <summary>
    /// 县区
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "县区")]
    public string County { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "地址")]
    public string Address { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    [SugarColumn(ColumnDescription = "经度")]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    [SugarColumn(ColumnDescription = "纬度")]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 矿井类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "矿井类型")]
    public string MineType { get; set; }

    /// <summary>
    /// 生产能力
    /// </summary>
    [SugarColumn(ColumnDescription = "生产能力")]
    public decimal? ProductionCapacity { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

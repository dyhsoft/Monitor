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
namespace Admin.NET.Application.CoalMine.Dtos;

/// <summary>
/// 煤矿分页查询
/// </summary>
public class CoalMinePageInput : PageInputBase
{
    /// <summary>
    /// 煤矿编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 所属集团
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// 矿井类型
    /// </summary>
    public string MineType { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 添加煤矿
/// </summary>
public class AddCoalMineInput
{
    /// <summary>
    /// 煤矿编码（10位）
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// 所属集团
    /// </summary>
    [StringLength(100)]
    public string GroupName { get; set; }

    /// <summary>
    /// 省份编码
    /// </summary>
    [StringLength(2)]
    public string Province { get; set; }

    /// <summary>
    /// 城市编码
    /// </summary>
    [StringLength(2)]
    public string City { get; set; }

    /// <summary>
    /// 县区编码
    /// </summary>
    [StringLength(2)]
    public string County { get; set; }

    /// <summary>
    /// 矿井类型
    /// </summary>
    [StringLength(20)]
    public string MineType { get; set; }

    /// <summary>
    /// 设计产能
    /// </summary>
    public int? DesignCapacity { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [StringLength(50)]
    public string Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(20)]
    public string Phone { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [StringLength(200)]
    public string Address { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string Remark { get; set; }
}

/// <summary>
/// 更新煤矿
/// </summary>
public class UpdateCoalMineInput : AddCoalMineInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// 删除煤矿
/// </summary>
public class DeleteCoalMineInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

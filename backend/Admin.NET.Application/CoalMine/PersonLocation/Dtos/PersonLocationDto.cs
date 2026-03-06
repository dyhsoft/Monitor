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
namespace Admin.NET.ApplicationPersonLocation.Dtos;

/// <summary>
/// 人员定位分页查询
/// </summary>
public class PersonLocationPageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 基站编号
    /// </summary>
    public string StationId { get; set; }
}

/// <summary>
/// 人员实时位置输出
/// </summary>
public class PersonLocationOutput
{
    public long Id { get; set; }
    public long MineId { get; set; }
    public string MineName { get; set; }
    public string CardId { get; set; }
    public string PersonName { get; set; }
    public string StationId { get; set; }
    public string StationName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public decimal? X { get; set; }
    public decimal? Y { get; set; }
    public decimal? Z { get; set; }
    public DateTime? InTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 区域人员统计
/// </summary>
public class AreaPersonStatisticsOutput
{
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public int PersonCount { get; set; }
}

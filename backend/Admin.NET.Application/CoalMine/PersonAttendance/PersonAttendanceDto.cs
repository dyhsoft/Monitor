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
using SqlSugar;

namespace Admin.NET.ApplicationPersonAttendance.Dtos;

/// <summary>
/// 人员出勤分页输入
/// </summary>
public class PersonAttendancePageInput : PageInputBase
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
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 出勤日期
    /// </summary>
    public DateTime? AttendanceDate { get; set; }
}

/// <summary>
/// 人员出勤输出
/// </summary>
public class PersonAttendanceOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 人员信息Id
    /// </summary>
    public long PersonInfoId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    public string WorkType { get; set; }

    /// <summary>
    /// 入井时间
    /// </summary>
    public DateTime? InTime { get; set; }

    /// <summary>
    /// 出井时间
    /// </summary>
    public DateTime? OutTime { get; set; }

    /// <summary>
    /// 工作时长
    /// </summary>
    public string WorkDuration { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 出勤日期
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 考勤统计输出
/// </summary>
public class AttendanceStatisticsOutput
{
    /// <summary>
    /// 今日下井人数
    /// </summary>
    public int TodayCount { get; set; }

    /// <summary>
    /// 今日出勤率
    /// </summary>
    public decimal AttendanceRate { get; set; }

    /// <summary>
    /// 在岗人数
    /// </summary>
    public int OnDuty { get; set; }

    /// <summary>
    /// 离岗人数
    /// </summary>
    public int OffDuty { get; set; }
}

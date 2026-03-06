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

namespace Admin.NET.ApplicationPersonInfo.Dtos;

/// <summary>
/// 人员信息输入
/// </summary>
public class PersonInfoInput
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long MineId { get; set; }

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
    /// 工种
    /// </summary>
    public string WorkType { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    public string IdCard { get; set; }

    /// <summary>
    /// 照片路径
    /// </summary>
    public string PhotoPath { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

/// <summary>
/// 人员信息分页输入
/// </summary>
public class PersonInfoPageInput : PageInputBase
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
}

/// <summary>
/// 人员信息输出
/// </summary>
public class PersonInfoOutput
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
    /// 工种
    /// </summary>
    public string WorkType { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    public string IdCard { get; set; }

    /// <summary>
    /// 照片路径
    /// </summary>
    public string PhotoPath { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}

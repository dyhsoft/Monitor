// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 人员信息表
/// </summary>
[SugarTable("PersonInfos", "人员信息表")]
public class PersonInfo : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    [SugarColumn(ColumnDescription = "定位卡号", Length = 50)]
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "人员姓名", Length = 50)]
    public string PersonName { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(ColumnDescription = "性别", Length = 10, IsNullable = true)]
    public string Gender { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(ColumnDescription = "部门", Length = 100, IsNullable = true)]
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    [SugarColumn(ColumnDescription = "工种", Length = 50, IsNullable = true)]
    public string WorkType { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 20, IsNullable = true)]
    public string Phone { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    [SugarColumn(ColumnDescription = "身份证号", Length = 20, IsNullable = true)]
    public string IdCard { get; set; }

    /// <summary>
    /// 照片
    /// </summary>
    [SugarColumn(ColumnDescription = "照片", Length = 500, IsNullable = true)]
    public string Photo { get; set; }

    /// <summary>
    /// 岗位状态:1-在岗,2-请假,3-离职
    /// </summary>
    [SugarColumn(ColumnDescription = "岗位状态", DefaultValue = "1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 入职日期
    /// </summary>
    [SugarColumn(ColumnDescription = "入职日期", IsNullable = true)]
    public DateTime? JoinDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}

/// <summary>
/// 人员出勤表
/// </summary>
[SugarTable("PersonAttendances", "人员出勤表")]
public class PersonAttendance : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 人员ID
    /// </summary>
    [SugarColumn(ColumnDescription = "人员ID")]
    public long PersonId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    [SugarColumn(ColumnDescription = "定位卡号", Length = 50)]
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "人员姓名", Length = 50)]
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(ColumnDescription = "部门", Length = 100, IsNullable = true)]
    public string Department { get; set; }

    /// <summary>
    /// 出勤日期
    /// </summary>
    [SugarColumn(ColumnDescription = "出勤日期")]
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// 上班时间
    /// </summary>
    [SugarColumn(ColumnDescription = "上班时间", IsNullable = true)]
    public DateTime? OnDutyTime { get; set; }

    /// <summary>
    /// 下班时间
    /// </summary>
    [SugarColumn(ColumnDescription = "下班时间", IsNullable = true)]
    public DateTime? OffDutyTime { get; set; }

    /// <summary>
    /// 工作时长(小时)
    /// </summary>
    [SugarColumn(ColumnDescription = "工作时长(小时)", IsNullable = true)]
    public decimal? WorkHours { get; set; }

    /// <summary>
    /// 出勤状态:1-出勤,2-请假,3-迟到,4-早退,5-旷工
    /// </summary>
    [SugarColumn(ColumnDescription = "出勤状态", DefaultValue = "1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}

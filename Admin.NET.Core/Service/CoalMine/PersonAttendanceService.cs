// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 人员出勤服务
/// </summary>
[ApiDescriptionSettings("PersonAttendance", Description = "人员出勤")]
public class PersonAttendanceService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<PersonAttendance> _attendanceRep;

    public PersonAttendanceService(SqlSugarRepository<PersonAttendance> attendanceRep)
    {
        _attendanceRep = attendanceRep;
    }

    /// <summary>
    /// 分页查询出勤记录
    /// </summary>
    [DisplayName("分页查询出勤记录")]
    public async Task<SqlSugarPagedList<PersonAttendanceOutput>> GetPage([FromQuery] PageAttendanceInput input)
    {
        return await _attendanceRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(input.PersonId > 0, u => u.PersonId == input.PersonId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.PersonName), u => u.PersonName.Contains(input.PersonName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Department), u => u.Department.Contains(input.Department))
            .WhereIF(input.Status.HasValue, u => u.Status == input.Status)
            .WhereIF(input.AttendanceDate.HasValue, u => u.AttendanceDate == input.AttendanceDate.Value.Date)
            .WhereIF(input.StartDate.HasValue, u => u.AttendanceDate >= input.StartDate.Value.Date)
            .WhereIF(input.EndDate.HasValue, u => u.AttendanceDate <= input.EndDate.Value.Date)
            .OrderByDescending(u => u.AttendanceDate)
            .OrderBy(u => u.PersonName)
            .Select<PersonAttendanceOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取考勤统计
    /// </summary>
    [DisplayName("获取考勤统计")]
    public async Task<AttendanceStatisticsOutput> GetStatistics([FromQuery] long mineId, [FromQuery] DateTime date)
    {
        var dateStart = date.Date;
        var dateEnd = date.Date.AddDays(1);

        var list = await _attendanceRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .Where(u => u.AttendanceDate >= dateStart && u.AttendanceDate < dateEnd)
            .ToListAsync();

        return new AttendanceStatisticsOutput
        {
            TotalCount = list.Count,
            PresentCount = list.Count(u => u.Status == 1),
            LeaveCount = list.Count(u => u.Status == 2),
            LateCount = list.Count(u => u.Status == 3),
            EarlyLeaveCount = list.Count(u => u.Status == 4),
            AbsentCount = list.Count(u => u.Status == 5),
            WorkHours = list.Where(u => u.WorkHours.HasValue).Sum(u => u.WorkHours.Value)
        };
    }

    /// <summary>
    /// 获取人员考勤详情
    /// </summary>
    [DisplayName("获取人员考勤详情")]
    public async Task<List<PersonAttendanceOutput>> GetPersonAttendance([FromQuery] long personId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        return await _attendanceRep.AsQueryable()
            .Where(u => u.PersonId == personId)
            .Where(u => u.AttendanceDate >= startDate.Date && u.AttendanceDate <= endDate.Date)
            .OrderByDescending(u => u.AttendanceDate)
            .Select<PersonAttendanceOutput>()
            .ToListAsync();
    }

    /// <summary>
    /// 批量生成出勤记录（从人员定位数据）
    /// </summary>
    [DisplayName("批量生成出勤记录")]
    public Task GenerateAttendance([FromQuery] long mineId, [FromQuery] DateTime date)
    {
        // TODO: 根据实际业务逻辑，从人员定位数据生成出勤记录
        // 这里需要结合煤矿的上下班时间来判断
        return Task.CompletedTask;
    }
}

/// <summary>
/// 出勤分页输入
/// </summary>
public class PageAttendanceInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 人员ID
    /// </summary>
    public long PersonId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 出勤状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 出勤日期
    /// </summary>
    public DateTime? AttendanceDate { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 考勤统计输出
/// </summary>
public class AttendanceStatisticsOutput
{
    /// <summary>
    /// 总人数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 出勤人数
    /// </summary>
    public int PresentCount { get; set; }

    /// <summary>
    /// 请假人数
    /// </summary>
    public int LeaveCount { get; set; }

    /// <summary>
    /// 迟到人数
    /// </summary>
    public int LateCount { get; set; }

    /// <summary>
    /// 早退人数
    /// </summary>
    public int EarlyLeaveCount { get; set; }

    /// <summary>
    /// 旷工人数
    /// </summary>
    public int AbsentCount { get; set; }

    /// <summary>
    /// 总工时
    /// </summary>
    public decimal WorkHours { get; set; }
}

/// <summary>
/// 出勤记录输出
/// </summary>
public class PersonAttendanceOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 人员ID
    /// </summary>
    public long PersonId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 出勤日期
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// 上班时间
    /// </summary>
    public DateTime? OnDutyTime { get; set; }

    /// <summary>
    /// 下班时间
    /// </summary>
    public DateTime? OffDutyTime { get; set; }

    /// <summary>
    /// 工作时长(小时)
    /// </summary>
    public decimal? WorkHours { get; set; }

    /// <summary>
    /// 出勤状态:1-出勤,2-请假,3-迟到,4-早退,5-旷工
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName => Status switch
    {
        1 => "出勤",
        2 => "请假",
        3 => "迟到",
        4 => "早退",
        5 => "旷工",
        _ => "未知"
    };

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

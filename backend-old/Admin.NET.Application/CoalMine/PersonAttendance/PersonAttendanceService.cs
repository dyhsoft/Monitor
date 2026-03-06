using Admin.NET.Application.CoalMine.PersonAttendance.Dtos;
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.PersonAttendance.Services;

/// <summary>
/// 人员出勤服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "PersonAttendance")]
public class PersonAttendanceService : IPersonAttendanceService, ITransient
{
    private readonly ISqlSugarClient _db;

    public PersonAttendanceService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<PersonAttendanceOutput>> GetPage(PersonAttendancePageInput input)
    {
        var date = input.AttendanceDate ?? DateTime.Today;
        var startDate = date.Date;
        var endDate = date.Date.AddDays(1);

        return await _db.Queryable<PersonAttendance>()
            .LeftJoin<CoalMine>((p, c) => p.MineId == c.Id)
            .Where((p, c) => input.MineId == null || p.MineId == input.MineId)
            .Where((p, c) => string.IsNullOrEmpty(input.PersonName) || p.PersonName.Contains(input.PersonName))
            .Where((p, c) => string.IsNullOrEmpty(input.CardId) || p.CardId.Contains(input.CardId))
            .Where((p, c) => string.IsNullOrEmpty(input.Department) || p.Department.Contains(input.Department))
            .Where((p, c) => input.Status == null || p.Status == input.Status)
            .Where((p, c) => p.AttendanceDate >= startDate && p.AttendanceDate < endDate)
            .Select((p, c) => new PersonAttendanceOutput
            {
                Id = p.Id,
                MineId = p.MineId,
                MineName = c.Name,
                PersonInfoId = p.PersonInfoId,
                CardId = p.CardId,
                PersonName = p.PersonName,
                Department = p.Department,
                WorkType = p.WorkType,
                InTime = p.InTime,
                OutTime = p.OutTime,
                WorkDuration = p.WorkDuration != null ? FormatDuration(p.WorkDuration.Value) : "",
                Status = p.Status,
                AttendanceDate = p.AttendanceDate,
                CreateTime = p.CreateTime
            })
            .OrderBy(p => p.InTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    [HttpGet]
    public async Task<PersonAttendance> Get(long id)
    {
        return await _db.Queryable<PersonAttendance>().Where(p => p.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    public async Task<long> Add(PersonAttendanceInput input)
    {
        var entity = new PersonAttendance
        {
            MineId = input.MineId,
            PersonInfoId = input.PersonInfoId,
            CardId = input.CardId,
            PersonName = input.PersonName,
            Department = input.Department,
            WorkType = input.WorkType,
            InTime = input.InTime,
            OutTime = input.OutTime,
            WorkDuration = input.WorkDuration,
            Status = input.Status,
            AttendanceDate = input.AttendanceDate
        };
        return await _db.Insertable(entity).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新
    /// </summary>
    [HttpPost]
    public async Task Update(PersonAttendanceInput input)
    {
        var entity = await _db.Queryable<PersonAttendance>().Where(p => p.Id == input.Id).FirstAsync();
        if (entity == null) throw new Exception("记录不存在");

        entity.OutTime = input.OutTime;
        entity.WorkDuration = input.WorkDuration;
        entity.Status = input.Status;

        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取考勤统计
    /// </summary>
    [HttpGet]
    public async Task<AttendanceStatisticsOutput> GetStatistics(long? mineId, DateTime? date)
    {
        var queryDate = date ?? DateTime.Today;
        var startDate = queryDate.Date;
        var endDate = queryDate.Date.AddDays(1);

        // 查询今日出勤记录
        var query = _db.Queryable<PersonAttendance>()
            .Where(p => p.AttendanceDate >= startDate && p.AttendanceDate < endDate);
        
        if (mineId.HasValue)
            query = query.Where(p => p.MineId == mineId.Value);

        var list = await query.ToListAsync();

        // 获取总人员数
        var totalQuery = _db.Queryable<PersonInfo>();
        if (mineId.HasValue)
            totalQuery = totalQuery.Where(p => p.MineId == mineId.Value);
        var totalCount = await totalQuery.CountAsync();

        var result = new AttendanceStatisticsOutput
        {
            TodayCount = list.Count,
            OnDuty = list.Count(p => p.Status == 0),
            OffDuty = list.Count(p => p.Status == 1),
            AttendanceRate = totalCount > 0 ? Math.Round((decimal)list.Count / totalCount * 100, 2) : 0
        };

        return result;
    }

    /// <summary>
    /// 格式化时长
    /// </summary>
    private string FormatDuration(int seconds)
    {
        var hours = seconds / 3600;
        var minutes = (seconds % 3600) / 60;
        return $"{hours}小时{minutes}分";
    }
}

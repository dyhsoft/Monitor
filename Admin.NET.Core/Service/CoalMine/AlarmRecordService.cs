using Admin.NET.Core;
using Microsoft.AspNetCore.Mvc;

namespace Admin.NET.Core.Service;

[ApiDescriptionSettings("AlarmRecord", Description = "报警记录")]
public class AlarmRecordService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<AlarmRecord> _alarmRep;

    public AlarmRecordService(SqlSugarRepository<AlarmRecord> alarmRep)
    {
        _alarmRep = alarmRep;
    }

    [DisplayName("分页查询报警记录")]
    public async Task<SqlSugarPagedList<AlarmRecordOutput>> GetPage([FromQuery] PageAlarmRecordInput input)
    {
        return await _alarmRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.AlarmType), u => u.AlarmType.Contains(input.AlarmType))
            .WhereIF(input.Status.HasValue, u => u.Status == input.Status)
            .WhereIF(input.StartTime.HasValue, u => u.AlarmTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, u => u.AlarmTime <= input.EndTime)
            .OrderByDescending(u => u.AlarmTime)
            .Select<AlarmRecordOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [DisplayName("获取未处理报警数")]
    public async Task<int> GetUnHandleCount([FromQuery] long mineId)
    {
        return await _alarmRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .Where(u => u.Status == 1)
            .CountAsync();
    }

    [DisplayName("确认报警")]
    public async Task Confirm(ConfirmAlarmInput input)
    {
        var entity = await _alarmRep.GetByIdAsync(input.Id);
        if (entity != null)
        {
            entity.Status = 2;
            entity.HandleTime = DateTime.Now;
            entity.Handler = "Admin";
            entity.Remark = input.Remark;
            await _alarmRep.AsUpdateable(entity).ExecuteCommandAsync();
        }
    }

    [DisplayName("解决报警")]
    public async Task Resolve(ResolveAlarmInput input)
    {
        var entity = await _alarmRep.GetByIdAsync(input.Id);
        if (entity != null)
        {
            entity.Status = 3;
            entity.HandleTime = DateTime.Now;
            entity.Handler = "Admin";
            entity.Remark = input.Remark;
            await _alarmRep.AsUpdateable(entity).ExecuteCommandAsync();
        }
    }

    [DisplayName("获取报警详情")]
    public async Task<AlarmRecordOutput> Get(long id)
    {
        return await _alarmRep.AsQueryable()
            .Where(u => u.Id == id)
            .Select<AlarmRecordOutput>()
            .FirstAsync();
    }
}

public class PageAlarmRecordInput : BasePageInput
{
    public long MineId { get; set; }
    public string SensorCode { get; set; }
    public string AlarmType { get; set; }
    public int? Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class ConfirmAlarmInput
{
    public long Id { get; set; }
    public string Remark { get; set; }
}

public class ResolveAlarmInput
{
    public long Id { get; set; }
    public string Remark { get; set; }
}

public class AlarmRecordOutput
{
    public long Id { get; set; }
    public long MineId { get; set; }
    public string SensorCode { get; set; }
    public string AlarmType { get; set; }
    public string AlarmValue { get; set; }
    public string Threshold { get; set; }
    public DateTime AlarmTime { get; set; }
    public int Status { get; set; }
    public string StatusName => Status switch { 1 => "未处理", 2 => "已处理", 3 => "已忽略", _ => "未知" };
    public DateTime? HandleTime { get; set; }
    public string Handler { get; set; }
    public string Remark { get; set; }
}

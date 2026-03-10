namespace Admin.NET.Core.Service;

/// <summary>
/// 矿领导配置服务
/// </summary>
[ApiDescriptionSettings("LocationLeaderConfig", Description = "矿领导配置")]
public class LocationLeaderConfigService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<LocationLeaderConfig> _rep;

    public LocationLeaderConfigService(SqlSugarRepository<LocationLeaderConfig> rep)
    {
        _rep = rep;
    }

    [DisplayName("获取矿领导配置分页列表")]
    public async Task<SqlSugarPagedList<LocationLeaderConfigOutput>> GetPage([FromQuery] PageLocationLeaderConfigInput input)
    {
        return await _rep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.PersonName), u => u.PersonName.Contains(input.PersonName))
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new LocationLeaderConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                PersonName = u.PersonName,
                CardId = u.CardId,
                DeptName = u.DeptName,
                Position = u.Position,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [DisplayName("新增矿领导配置")]
    public async Task<long> Add(AddLocationLeaderConfigInput input)
    {
        return await _rep.InsertReturnIdentityAsync(input.Adapt<LocationLeaderConfig>());
    }

    [DisplayName("更新矿领导配置")]
    public async Task Update(UpdateLocationLeaderConfigInput input)
    {
        await _rep.AsUpdateable(input.Adapt<LocationLeaderConfig>()).ExecuteCommandAsync();
    }

    [DisplayName("删除矿领导配置")]
    public async Task Delete(long id)
    {
        await _rep.DeleteByIdAsync(id);
    }
}

/// <summary>
/// 限定人数配置服务
/// </summary>
[ApiDescriptionSettings("LocationLimitConfig", Description = "限定人数配置")]
public class LocationLimitConfigService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<LocationLimitConfig> _rep;

    public LocationLimitConfigService(SqlSugarRepository<LocationLimitConfig> rep)
    {
        _rep = rep;
    }

    [DisplayName("获取限定人数配置分页列表")]
    public async Task<SqlSugarPagedList<LocationLimitConfigOutput>> GetPage([FromQuery] PageLocationLimitConfigInput input)
    {
        return await _rep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new LocationLimitConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                AreaCode = u.AreaCode,
                AreaName = u.AreaName,
                LimitCount = u.LimitCount,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [DisplayName("新增限定人数配置")]
    public async Task<long> Add(AddLocationLimitConfigInput input)
    {
        return await _rep.InsertReturnIdentityAsync(input.Adapt<LocationLimitConfig>());
    }

    [DisplayName("更新限定人数配置")]
    public async Task Update(UpdateLocationLimitConfigInput input)
    {
        await _rep.AsUpdateable(input.Adapt<LocationLimitConfig>()).ExecuteCommandAsync();
    }

    [DisplayName("删除限定人数配置")]
    public async Task Delete(long id)
    {
        await _rep.DeleteByIdAsync(id);
    }
}

/// <summary>
/// 定位报警记录服务
/// </summary>
[ApiDescriptionSettings("LocationAlarm", Description = "定位报警记录")]
public class LocationAlarmService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<LocationAlarm> _rep;

    public LocationAlarmService(SqlSugarRepository<LocationAlarm> rep)
    {
        _rep = rep;
    }

    [DisplayName("获取定位报警记录分页列表")]
    public async Task<SqlSugarPagedList<LocationAlarmOutput>> GetPage([FromQuery] PageLocationAlarmInput input)
    {
        return await _rep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(input.AlarmType.HasValue, u => u.AlarmType == input.AlarmType)
            .WhereIF(input.Status.HasValue, u => u.Status == input.Status)
            .WhereIF(input.StartTime.HasValue, u => u.AlarmTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, u => u.AlarmTime <= input.EndTime)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.AlarmTime, OrderByType.Desc)
            .Select((u, m) => new LocationAlarmOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                AlarmType = u.AlarmType,
                CardId = u.CardId,
                PersonName = u.PersonName,
                AreaName = u.AreaName,
                AlarmMessage = u.AlarmMessage,
                Status = u.Status,
                AlarmTime = u.AlarmTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    [DisplayName("获取实时报警")]
    public async Task<List<LocationAlarmOutput>> GetRealTime([FromQuery] long mineId)
    {
        return await _rep.AsQueryable()
            .Where(u => u.MineId == mineId && u.Status == 0)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.AlarmTime, OrderByType.Desc)
            .Select((u, m) => new LocationAlarmOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                AlarmType = u.AlarmType,
                CardId = u.CardId,
                PersonName = u.PersonName,
                AreaName = u.AreaName,
                AlarmMessage = u.AlarmMessage,
                Status = u.Status,
                AlarmTime = u.AlarmTime
            })
            .ToListAsync();
    }

    [DisplayName("处理报警")]
    public async Task Handle(long id)
    {
        await _rep.AsUpdateable()
            .SetColumns(u => u.Status == 1)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }
}

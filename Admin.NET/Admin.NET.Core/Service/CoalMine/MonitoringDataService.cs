// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 安全监测数据服务
/// </summary>
[ApiDescriptionSettings("SafetyData", Description = "安全监测数据")]
public class SafetyDataService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SafetyData> _safetyDataRep;

    public SafetyDataService(SqlSugarRepository<SafetyData> safetyDataRep)
    {
        _safetyDataRep = safetyDataRep;
    }

    /// <summary>
    /// 获取安全监测数据分页列表
    /// </summary>
    [DisplayName("获取安全监测数据分页列表")]
    public async Task<SqlSugarPagedList<SafetyDataOutput>> GetPage([FromQuery] PageSafetyDataInput input)
    {
        return await _safetyDataRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorType), u => u.SensorType == input.SensorType)
            .WhereIF(input.StartTime.HasValue, u => u.UpdateTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, u => u.UpdateTime <= input.EndTime)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.UpdateTime, OrderByType.Desc)
            .Select((u, m) => new SafetyDataOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                SensorCode = u.SensorCode,
                SensorName = u.SensorName,
                SensorType = u.SensorType,
                Value = u.Value,
                Unit = u.Unit,
                Status = u.Status,
                UpdateTime = u.UpdateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取安全监测实时数据
    /// </summary>
    [DisplayName("获取安全监测实时数据")]
    public async Task<List<SafetyDataOutput>> GetRealTime([FromQuery] long mineId)
    {
        var latestData = await _safetyDataRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .GroupBy(u => u.SensorCode)
            .Select(u => new { SensorCode = u.SensorCode, Id = SqlFunc.AggregateMax(u.Id) })
            .ToListAsync();

        var latestIds = latestData.Select(x => x.Id).ToList();

        return await _safetyDataRep.AsQueryable()
            .Where(u => latestIds.Contains(u.Id))
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .Select((u, m) => new SafetyDataOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                SensorCode = u.SensorCode,
                SensorName = u.SensorName,
                SensorType = u.SensorType,
                Value = u.Value,
                Unit = u.Unit,
                Status = u.Status,
                UpdateTime = u.UpdateTime
            })
            .ToListAsync();
    }
}

/// <summary>
/// 人员定位数据服务
/// </summary>
[ApiDescriptionSettings("PersonLocation", Description = "人员定位数据")]
public class PersonLocationService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<PersonLocation> _personLocationRep;

    public PersonLocationService(SqlSugarRepository<PersonLocation> personLocationRep)
    {
        _personLocationRep = personLocationRep;
    }

    /// <summary>
    /// 获取人员定位数据分页列表
    /// </summary>
    [DisplayName("获取人员定位数据分页列表")]
    public async Task<SqlSugarPagedList<PersonLocationOutput>> GetPage([FromQuery] PagePersonLocationInput input)
    {
        return await _personLocationRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.CardId), u => u.CardId.Contains(input.CardId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PersonName), u => u.PersonName.Contains(input.PersonName))
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.UpdateTime, OrderByType.Desc)
            .Select((u, m) => new PersonLocationOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                CardId = u.CardId,
                PersonName = u.PersonName,
                DeptName = u.DeptName,
                StationId = u.StationId,
                AreaCode = u.AreaCode,
                AreaName = u.AreaName,
                X = u.X,
                Y = u.Y,
                Z = u.Z,
                Status = u.Status,
                UpdateTime = u.UpdateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取实时人员位置
    /// </summary>
    [DisplayName("获取实时人员位置")]
    public async Task<List<PersonLocationOutput>> GetRealTime([FromQuery] long mineId)
    {
        var latestData = await _personLocationRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .GroupBy(u => u.CardId)
            .Select(u => new { CardId = u.CardId, Id = SqlFunc.AggregateMax(u.Id) })
            .ToListAsync();

        var latestIds = latestData.Select(x => x.Id).ToList();

        return await _personLocationRep.AsQueryable()
            .Where(u => latestIds.Contains(u.Id))
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .Select((u, m) => new PersonLocationOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                CardId = u.CardId,
                PersonName = u.PersonName,
                DeptName = u.DeptName,
                StationId = u.StationId,
                AreaCode = u.AreaCode,
                AreaName = u.AreaName,
                X = u.X,
                Y = u.Y,
                Z = u.Z,
                Status = u.Status,
                UpdateTime = u.UpdateTime
            })
            .ToListAsync();
    }
}

/// <summary>
/// 人员进出记录服务
/// </summary>
[ApiDescriptionSettings("PersonRecord", Description = "人员进出记录")]
public class PersonRecordService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<PersonRecord> _personRecordRep;

    public PersonRecordService(SqlSugarRepository<PersonRecord> personRecordRep)
    {
        _personRecordRep = personRecordRep;
    }

    /// <summary>
    /// 获取人员进出记录分页列表
    /// </summary>
    [DisplayName("获取人员进出记录分页列表")]
    public async Task<SqlSugarPagedList<PersonRecordOutput>> GetPage([FromQuery] PagePersonRecordInput input)
    {
        var query = _personRecordRep.AsQueryable();
        if (input.MineId != null && input.MineId > 0)
            query = query.Where(u => u.MineId == input.MineId);
        if (!string.IsNullOrWhiteSpace(input.CardId))
            query = query.Where(u => u.CardId.Contains(input.CardId));
        if (input.RecordType != null && input.RecordType > 0)
            query = query.Where(u => u.RecordType == input.RecordType);
        if (input.StartTime != null)
            query = query.Where(u => u.RecordTime >= input.StartTime);
        if (input.EndTime != null)
            query = query.Where(u => u.RecordTime <= input.EndTime);
        
        return await query
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.RecordTime, OrderByType.Desc)
            .Select((u, m) => new PersonRecordOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                CardId = u.CardId,
                PersonName = u.PersonName,
                DeptName = u.DeptName,
                AreaCode = u.AreaCode,
                AreaName = u.AreaName,
                Status = u.RecordType,
                UpdateTime = u.RecordTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }
}

/// <summary>
/// 矿压监测数据服务
/// </summary>
[ApiDescriptionSettings("PressureData", Description = "矿压监测数据")]
public class PressureDataService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<PressureData> _pressureDataRep;

    public PressureDataService(SqlSugarRepository<PressureData> pressureDataRep)
    {
        _pressureDataRep = pressureDataRep;
    }

    /// <summary>
    /// 获取矿压监测数据分页列表
    /// </summary>
    [DisplayName("获取矿压监测数据分页列表")]
    public async Task<SqlSugarPagedList<SafetyDataOutput>> GetPage([FromQuery] PagePressureDataInput input)
    {
        return await _pressureDataRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorType), u => u.SensorType == input.SensorType)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.UpdateTime, OrderByType.Desc)
            .Select((u, m) => new SafetyDataOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                SensorCode = u.SensorCode,
                SensorName = u.SensorName,
                SensorType = u.SensorType,
                Value = u.Value,
                Unit = u.Unit,
                Status = 0,
                UpdateTime = u.UpdateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }
}

/// <summary>
/// 水文监测数据服务
/// </summary>
[ApiDescriptionSettings("WaterData", Description = "水文监测数据")]
public class WaterDataService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<WaterData> _waterDataRep;

    public WaterDataService(SqlSugarRepository<WaterData> waterDataRep)
    {
        _waterDataRep = waterDataRep;
    }

    /// <summary>
    /// 获取水文监测数据分页列表
    /// </summary>
    [DisplayName("获取水文监测数据分页列表")]
    public async Task<SqlSugarPagedList<SafetyDataOutput>> GetPage([FromQuery] PageWaterDataInput input)
    {
        return await _waterDataRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorType), u => u.SensorType == input.SensorType)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.UpdateTime, OrderByType.Desc)
            .Select((u, m) => new SafetyDataOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                SensorCode = u.SensorCode,
                SensorName = u.SensorName,
                SensorType = u.SensorType,
                Value = u.Value,
                Unit = u.Unit,
                Status = 0,
                UpdateTime = u.UpdateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }
}

/// <summary>
/// 解析日志服务
/// </summary>
[ApiDescriptionSettings("ParseLog", Description = "解析日志")]
public class ParseLogService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ParseLog> _parseLogRep;

    public ParseLogService(SqlSugarRepository<ParseLog> parseLogRep)
    {
        _parseLogRep = parseLogRep;
    }

    /// <summary>
    /// 获取解析日志分页列表
    /// </summary>
    [DisplayName("获取解析日志分页列表")]
    public async Task<SqlSugarPagedList<ParseLogOutput>> GetPage([FromQuery] PageParseLogInput input)
    {
        return await _parseLogRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.DataType), u => u.DataType == input.DataType)
            .WhereIF(input.Status.HasValue, u => u.Status == input.Status)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .Select((u, m) => new ParseLogOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                FileName = u.FileName,
                DataType = u.DataType,
                BindSystem = u.BindSystem,
                RecordCount = u.RecordCount,
                Status = u.Status,
                ErrorMessage = u.ErrorMessage,
                Duration = u.Duration,
                CreateTime = u.CreateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }
}

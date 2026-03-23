// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 煤矿数据保存服务
/// </summary>
[ApiDescriptionSettings("CoalData", Description = "煤矿数据保存")]
public class CoalDataSaveService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<CoalMine> _coalMineRep;
    private readonly SqlSugarRepository<SafetyData> _safetyDataRep;
    private readonly SqlSugarRepository<AlarmRecord> _alarmRecordRep;
    private readonly SqlSugarRepository<PersonLocation> _personLocationRep;
    private readonly SqlSugarRepository<PersonRecord> _personRecordRep;
    private readonly SqlSugarRepository<StationStatus> _stationStatusRep;
    private readonly SqlSugarRepository<WaterData> _waterDataRep;
    private readonly SqlSugarRepository<PressureData> _pressureDataRep;
    private readonly SqlSugarRepository<ParseLog> _parseLogRep;
    private readonly ILogger<CoalDataSaveService> _logger;

    public CoalDataSaveService(
        SqlSugarRepository<CoalMine> coalMineRep,
        SqlSugarRepository<SafetyData> safetyDataRep,
        SqlSugarRepository<AlarmRecord> alarmRecordRep,
        SqlSugarRepository<PersonLocation> personLocationRep,
        SqlSugarRepository<PersonRecord> personRecordRep,
        SqlSugarRepository<StationStatus> stationStatusRep,
        SqlSugarRepository<WaterData> waterDataRep,
        SqlSugarRepository<PressureData> pressureDataRep,
        SqlSugarRepository<ParseLog> parseLogRep,
        ILogger<CoalDataSaveService> logger)
    {
        _coalMineRep = coalMineRep;
        _safetyDataRep = safetyDataRep;
        _alarmRecordRep = alarmRecordRep;
        _personLocationRep = personLocationRep;
        _personRecordRep = personRecordRep;
        _stationStatusRep = stationStatusRep;
        _waterDataRep = waterDataRep;
        _pressureDataRep = pressureDataRep;
        _parseLogRep = parseLogRep;
        _logger = logger;
    }

    /// <summary>
    /// 解析并保存文件数据
    /// </summary>
    [DisplayName("解析并保存文件数据")]
    public async Task<ParseResult> ParseAndSaveAsync(string filePath)
    {
        var startTime = DateTime.Now;
        var result = CoalMineDataParser.ParseFile(filePath);
        
        try
        {
            if (!result.Success)
            {
                await SaveParseLog(filePath, result.DataType, 0, false, result.ErrorMessage, startTime);
                return result;
            }

            // 获取煤矿ID
            var mine = await _coalMineRep.AsQueryable()
                .Where(m => m.Code == result.MineCode)
                .FirstAsync();

            if (mine == null)
            {
                await SaveParseLog(filePath, result.DataType, 0, false, $"煤矿不存在: {result.MineCode}", startTime);
                result.Success = false;
                result.ErrorMessage = $"煤矿不存在: {result.MineCode}";
                return result;
            }

            // 根据数据类型保存数据
            var saveResult = result.DataType switch
            {
                "CDSS" => await SaveSafetyDataAsync(mine.Id, result.DataRecords),
                "CDDY" => await SaveAlarmDataAsync(mine.Id, result.DataRecords),
                "RYSS" => await SavePersonLocationAsync(mine.Id, result.DataRecords),
                "RYCS" => await SavePersonInfoAsync(mine.Id, result.DataRecords),
                "RYCY" => await SavePersonRecordAsync(mine.Id, result.DataRecords),
                "JZSS" => await SaveStationStatusAsync(mine.Id, result.DataRecords),
                "CGKCDSS" => await SaveWaterDataAsync(mine.Id, result.DataRecords),
                "CGKCDDY" => await SaveWaterAlarmAsync(mine.Id, result.DataRecords),
                "KYCDSS" => await SavePressureDataAsync(mine.Id, result.DataRecords),
                "KYCDDY" => await SavePressureAlarmAsync(mine.Id, result.DataRecords),
                _ => true
            };

            await SaveParseLog(filePath, result.DataType, result.RecordCount, saveResult, null, startTime);
            _logger.LogInformation("数据保存完成: {DataType}, 记录数: {Count}", result.DataType, result.RecordCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存数据失败: {FilePath}", filePath);
            await SaveParseLog(filePath, result.DataType, 0, false, ex.Message, startTime);
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 保存解析日志
    /// </summary>
    private async Task SaveParseLog(string fileName, string dataType, int recordCount, bool success, string errorMessage, DateTime startTime)
    {
        var log = new ParseLog
        {
            FileName = fileName,
            DataType = dataType,
            RecordCount = recordCount,
            Status = success ? 1 : 0,
            ErrorMessage = errorMessage,
            Duration = (int)(DateTime.Now - startTime).TotalMilliseconds,
            CreateTime = DateTime.Now
        };
        await _parseLogRep.InsertAsync(log);
    }

    /// <summary>
    /// 保存安全监测数据
    /// </summary>
    private async Task<bool> SaveSafetyDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new SafetyData
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("SensorCode")?.ToString(),
            SensorName = r.GetValueOrDefault("SensorName")?.ToString(),
            SensorType = "CDSS",
            Value = Convert.ToDecimal(r.GetValueOrDefault("Value") ?? "0"),
            Unit = r.GetValueOrDefault("Unit")?.ToString(),
            Status = Convert.ToInt32(r.GetValueOrDefault("Status") ?? "0"),
            UpdateTime = DateTime.Now
        }).ToList();

        await _safetyDataRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存报警数据
    /// </summary>
    private async Task<bool> SaveAlarmDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new AlarmRecord
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("SensorCode")?.ToString(),
            AlarmType = r.GetValueOrDefault("AlarmType")?.ToString(),
            AlarmValue = r.GetValueOrDefault("Value")?.ToString(),
            Threshold = r.GetValueOrDefault("Threshold")?.ToString(),
            AlarmTime = DateTime.Now,
            Status = 1 // 未处理
        }).ToList();

        await _alarmRecordRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存人员定位数据
    /// </summary>
    private async Task<bool> SavePersonLocationAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new PersonLocation
        {
            MineId = mineId,
            CardId = r.GetValueOrDefault("CardId")?.ToString(),
            PersonName = r.GetValueOrDefault("PersonName")?.ToString(),
            DeptName = r.GetValueOrDefault("DeptName")?.ToString(),
            AreaCode = r.GetValueOrDefault("AreaCode")?.ToString(),
            AreaName = r.GetValueOrDefault("AreaName")?.ToString(),
            StationId = r.GetValueOrDefault("StationId")?.ToString(),
            X = Convert.ToDecimal(r.GetValueOrDefault("X") ?? "0"),
            Y = Convert.ToDecimal(r.GetValueOrDefault("Y") ?? "0"),
            Z = Convert.ToDecimal(r.GetValueOrDefault("Z") ?? "0"),
            Status = 1,
            UpdateTime = DateTime.Now
        }).ToList();

        await _personLocationRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存人员信息（RYCS）- 实际应该更新人员表
    /// </summary>
    private async Task<bool> SavePersonInfoAsync(long mineId, List<Dictionary<string, object>> records)
    {
        // 这里简化处理，实际应该更新人员表
        _logger.LogInformation("收到人员初始化数据: {Count} 条", records.Count);
        return true;
    }

    /// <summary>
    /// 保存人员进出记录
    /// </summary>
    private async Task<bool> SavePersonRecordAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new PersonRecord
        {
            MineId = mineId,
            CardId = r.GetValueOrDefault("CardId")?.ToString(),
            PersonName = r.GetValueOrDefault("PersonName")?.ToString(),
            DeptName = r.GetValueOrDefault("DeptName")?.ToString(),
            AreaCode = r.GetValueOrDefault("AreaCode")?.ToString(),
            AreaName = r.GetValueOrDefault("AreaName")?.ToString(),
            RecordType = 1,
            RecordTime = DateTime.Now
        }).ToList();

        await _personRecordRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存基站状态
    /// </summary>
    private async Task<bool> SaveStationStatusAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new StationStatus
        {
            MineId = mineId,
            StationId = r.GetValueOrDefault("StationId")?.ToString(),
            StationName = r.GetValueOrDefault("StationName")?.ToString(),
            Status = Convert.ToInt32(r.GetValueOrDefault("Status") ?? "0"),
            Power = Convert.ToDecimal(r.GetValueOrDefault("Power") ?? "0"),
            Signal = Convert.ToInt32(r.GetValueOrDefault("Signal") ?? "0"),
            UpdateTime = DateTime.Now
        }).ToList();

        await _stationStatusRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存水文监测数据
    /// </summary>
    private async Task<bool> SaveWaterDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new WaterData
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("SensorCode")?.ToString(),
            SensorName = r.GetValueOrDefault("SensorName")?.ToString(),
            SensorType = "CGKCDSS",
            Value = Convert.ToDecimal(r.GetValueOrDefault("WaterLevel") ?? "0"),
            Unit = "m",
            Status = r.GetValueOrDefault("Status")?.ToString() ?? "0",
            UpdateTime = DateTime.Now
        }).ToList();

        await _waterDataRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存水害报警
    /// </summary>
    private async Task<bool> SaveWaterAlarmAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new AlarmRecord
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("SensorCode")?.ToString(),
            AlarmType = "水害报警",
            AlarmValue = r.GetValueOrDefault("WaterLevel")?.ToString(),
            AlarmTime = DateTime.Now,
            Status = 1
        }).ToList();

        await _alarmRecordRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存矿压监测数据
    /// </summary>
    private async Task<bool> SavePressureDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new PressureData
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("SensorCode")?.ToString(),
            SensorName = r.GetValueOrDefault("SensorName")?.ToString(),
            SensorType = "KYCDSS",
            Value = Convert.ToDecimal(r.GetValueOrDefault("Pressure") ?? r.GetValueOrDefault("Value") ?? "0"),
            Unit = "MPa",
            Status = r.GetValueOrDefault("Status")?.ToString() ?? "0",
            UpdateTime = DateTime.Now
        }).ToList();

        await _pressureDataRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存矿压报警
    /// </summary>
    private async Task<bool> SavePressureAlarmAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new AlarmRecord
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("SensorCode")?.ToString(),
            AlarmType = "矿压报警",
            AlarmValue = r.GetValueOrDefault("Pressure")?.ToString(),
            AlarmTime = DateTime.Now,
            Status = 1
        }).ToList();

        await _alarmRecordRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }
}

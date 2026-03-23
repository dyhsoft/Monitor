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
    private readonly SqlSugarRepository<PersonInfo> _personInfoRep;
    private readonly SqlSugarRepository<LocationAlarm> _locationAlarmRep;
    private readonly SqlSugarRepository<StationData> _stationDataRep;
    private readonly SqlSugarRepository<SwitchAlarmRecord> _switchAlarmRep;
    private readonly SqlSugarRepository<StatisticsData> _statisticsRep;
    private readonly SqlSugarRepository<AbnormalAlarm> _abnormalAlarmRep;
    private readonly SqlSugarRepository<PersonAreaStatistic> _personAreaStatRep;
    private readonly SqlSugarRepository<BeltScaleData> _beltScaleRep;
    private readonly SqlSugarRepository<DrainageData> _drainageRep;
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
        SqlSugarRepository<PersonInfo> personInfoRep,
        SqlSugarRepository<LocationAlarm> locationAlarmRep,
        SqlSugarRepository<StationData> stationDataRep,
        SqlSugarRepository<SwitchAlarmRecord> switchAlarmRep,
        SqlSugarRepository<StatisticsData> statisticsRep,
        SqlSugarRepository<AbnormalAlarm> abnormalAlarmRep,
        SqlSugarRepository<PersonAreaStatistic> personAreaStatRep,
        SqlSugarRepository<BeltScaleData> beltScaleRep,
        SqlSugarRepository<DrainageData> drainageRep,
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
        _personInfoRep = personInfoRep;
        _locationAlarmRep = locationAlarmRep;
        _stationDataRep = stationDataRep;
        _switchAlarmRep = switchAlarmRep;
        _statisticsRep = statisticsRep;
        _abnormalAlarmRep = abnormalAlarmRep;
        _personAreaStatRep = personAreaStatRep;
        _beltScaleRep = beltScaleRep;
        _drainageRep = drainageRep;
        _logger = logger;
    }

    /// <summary>
    /// 解析并保存文件数据（使用默认分隔符）
    /// </summary>
    [DisplayName("解析并保存文件数据")]
    public async Task<ParseResult> ParseAndSaveAsync(string filePath)
    {
        return await ParseAndSaveAsync(filePath, ";", "~");
    }

    /// <summary>
    /// 解析并保存文件数据（自定义分隔符）
    /// </summary>
    [DisplayName("解析并保存文件数据")]
    public async Task<ParseResult> ParseAndSaveAsync(string filePath, string fieldSeparator, string recordSeparator)
    {
        var startTime = DateTime.Now;
        var result = CoalMineDataParser.ParseFile(filePath, fieldSeparator, recordSeparator);
        
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
                "FZSS" => await SaveStationDataAsync(mine.Id, result.DataRecords),
                "KGBH" => await SaveSwitchAlarmAsync(mine.Id, result.DataRecords),
                "TJSJ" => await SaveStatisticsDataAsync(mine.Id, result.DataRecords),
                "YCBJ" => await SaveAbnormalAlarmAsync(mine.Id, result.DataRecords),
                "RYSS" => await SavePersonLocationAsync(mine.Id, result.DataRecords),
                "RYCS" => await SavePersonInfoAsync(mine.Id, result.DataRecords),
                "RYCY" => await SavePersonRecordAsync(mine.Id, result.DataRecords),
                "RYQJ" => await SavePersonAreaStatisticsAsync(mine.Id, result.DataRecords),
                "JZSS" => await SaveStationStatusAsync(mine.Id, result.DataRecords),
                "CGKCDSS" => await SaveWaterDataAsync(mine.Id, result.DataRecords),
                "CGKCDDY" => await SaveWaterAlarmAsync(mine.Id, result.DataRecords),
                "JSLCDSS" => await SaveBeltScaleDataAsync(mine.Id, result.DataRecords),
                "PSLCDSS" => await SaveDrainageDataAsync(mine.Id, result.DataRecords),
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
        // 实际数据格式: Field1=传感器编号, Field2=类型, Field3=位置, Field4=值, Field5=单位, Field6=状态, Field7=时间
        var entities = records.Select(r => new SafetyData
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            SensorName = r.GetValueOrDefault("Field3")?.ToString(), // 安装位置作为名称
            SensorType = r.GetValueOrDefault("Field2")?.ToString(), // 传感器类型
            Value = Convert.ToDecimal(r.GetValueOrDefault("Field4") ?? "0"),
            Unit = r.GetValueOrDefault("Field5")?.ToString(),
            Status = Convert.ToInt32(r.GetValueOrDefault("Field6") ?? "0"),
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
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            AlarmType = r.GetValueOrDefault("Field2")?.ToString(),
            AlarmValue = r.GetValueOrDefault("Field4")?.ToString(),
            AlarmTime = DateTime.Now,
            Status = 1
        }).ToList();

        await _alarmRecordRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存人员定位数据
    /// </summary>
    private async Task<bool> SavePersonLocationAsync(long mineId, List<Dictionary<string, object>> records)
    {
        // 实际数据格式: Field1=卡号, Field2=姓名, Field3=状态, Field4=入井时间, Field5=?, Field6=区域, Field7=识别时间
        var entities = records.Select(r => new PersonLocation
        {
            MineId = mineId,
            CardId = r.GetValueOrDefault("Field1")?.ToString(),
            PersonName = r.GetValueOrDefault("Field2")?.ToString(),
            Status = Convert.ToInt32(r.GetValueOrDefault("Field3") ?? "1"),
            UpdateTime = DateTime.Now
        }).ToList();

        await _personLocationRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存人员信息（RYCS）
    /// </summary>
    private async Task<bool> SavePersonInfoAsync(long mineId, List<Dictionary<string, object>> records)
    {
        foreach (var r in records)
        {
            var cardId = r.GetValueOrDefault("CardId")?.ToString();
            if (string.IsNullOrEmpty(cardId)) continue;

            // 检查是否已存在
            var existing = await _personInfoRep.AsQueryable()
                .Where(u => u.MineId == mineId && u.CardId == cardId)
                .FirstAsync();

            if (existing != null)
            {
                // 更新已有人员信息
                existing.PersonName = r.GetValueOrDefault("Field2")?.ToString() ?? existing.PersonName;
                existing.Department = r.GetValueOrDefault("Field3")?.ToString() ?? existing.Department;
                await _personInfoRep.AsUpdateable(existing).ExecuteCommandAsync();
            }
            else
            {
                var entity = new PersonInfo
                {
                    MineId = mineId,
                    CardId = cardId,
                    PersonName = r.GetValueOrDefault("Field2")?.ToString(),
                    Department = r.GetValueOrDefault("Field3")?.ToString(),
                    Status = 1
                };
                await _personInfoRep.InsertAsync(entity);
            }
        }
        _logger.LogInformation("人员初始化数据保存完成: {Count} 条", records.Count);
        return true;
    }

    /// <summary>
    /// 保存人员进出记录
    /// </summary>
    private async Task<bool> SavePersonRecordAsync(long mineId, List<Dictionary<string, object>> records)
    {
        // RYCY格式: Field1=区域, Field2=人数, Field3=区域编号, Field4=区域名称, Field5=时间, Field6=卡号列表(&分隔)
        var entities = records.Select(r => new PersonRecord
        {
            MineId = mineId,
            CardId = r.GetValueOrDefault("Field6")?.ToString(),
            AreaCode = r.GetValueOrDefault("Field3")?.ToString(),
            AreaName = r.GetValueOrDefault("Field4")?.ToString(),
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
        // JZSS格式: Field1=基站编号, Field2=状态, Field3=在线人数, Field4=时间
        var entities = records.Select(r => new StationStatus
        {
            MineId = mineId,
            StationId = r.GetValueOrDefault("Field1")?.ToString(),
            Status = Convert.ToInt32(r.GetValueOrDefault("Field2") ?? "0"),
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
        // CGKCDSS格式: Field1=传感器编号, Field2=状态, Field3=水位, Field4=温度, Field5=时间
        var entities = records.Select(r => new WaterData
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            SensorType = "CGKCDSS",
            Value = Convert.ToDecimal(r.GetValueOrDefault("Field3") ?? "0"),
            Unit = "m",
            Status = r.GetValueOrDefault("Field2")?.ToString() ?? "0",
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
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            AlarmType = "水害报警",
            AlarmValue = r.GetValueOrDefault("Field3")?.ToString(),
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
        // KYCDSS格式: Field1=传感器编号, Field2=类型, Field3=位置, Field4=值, Field5=单位, Field6=状态, Field7=时间
        var entities = records.Select(r => new PressureData
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            SensorName = r.GetValueOrDefault("Field3")?.ToString(),
            SensorType = "KYCDSS",
            Value = Convert.ToDecimal(r.GetValueOrDefault("Field4") ?? "0"),
            Unit = r.GetValueOrDefault("Field5")?.ToString() ?? "MPa",
            Status = r.GetValueOrDefault("Field6")?.ToString() ?? "0",
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
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            AlarmType = "矿压报警",
            AlarmValue = r.GetValueOrDefault("Field4")?.ToString(),
            AlarmTime = DateTime.Now,
            Status = 1
        }).ToList();

        await _alarmRecordRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存分站状态数据（FZSS）
    /// </summary>
    private async Task<bool> SaveStationDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new StationData
        {
            MineId = mineId,
            StationCode = r.GetValueOrDefault("Field1")?.ToString(),
            StationName = r.GetValueOrDefault("Field3")?.ToString(),
            Pressure = Convert.ToDecimal(r.GetValueOrDefault("Field4") ?? "0"),
            AirFlow = Convert.ToDecimal(r.GetValueOrDefault("Field5") ?? "0"),
            Status = Convert.ToInt32(r.GetValueOrDefault("Field6") ?? "0"),
            UpdateTime = DateTime.Now
        }).ToList();

        await _stationDataRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存开关量报警数据（KGBH）
    /// </summary>
    private async Task<bool> SaveSwitchAlarmAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new SwitchAlarmRecord
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            SensorName = r.GetValueOrDefault("Field3")?.ToString(),
            SwitchStatus = Convert.ToInt32(r.GetValueOrDefault("Field2") ?? "0"),
            AlarmType = r.GetValueOrDefault("Field2")?.ToString(),
            AlarmTime = DateTime.Now,
            Status = 0
        }).ToList();

        await _switchAlarmRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存统计汇总数据（TJSJ）
    /// </summary>
    private async Task<bool> SaveStatisticsDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new StatisticsData
        {
            MineId = mineId,
            StatType = r.GetValueOrDefault("Field1")?.ToString(),
            Value = Convert.ToDecimal(r.GetValueOrDefault("Field2") ?? "0"),
            Unit = r.GetValueOrDefault("Field3")?.ToString(),
            StatTime = DateTime.Now
        }).ToList();

        await _statisticsRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存异常报警数据（YCBJ）
    /// </summary>
    private async Task<bool> SaveAbnormalAlarmAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new AbnormalAlarm
        {
            MineId = mineId,
            SensorCode = r.GetValueOrDefault("Field1")?.ToString(),
            AlarmType = r.GetValueOrDefault("Field2")?.ToString() ?? "异常报警",
            AlarmMessage = r.GetValueOrDefault("Field3")?.ToString(),
            AlarmLevel = Convert.ToInt32(r.GetValueOrDefault("Field4") ?? "1"),
            AlarmTime = DateTime.Now,
            Status = 0
        }).ToList();

        await _abnormalAlarmRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存人员区域统计（RYQJ）
    /// </summary>
    private async Task<bool> SavePersonAreaStatisticsAsync(long mineId, List<Dictionary<string, object>> records)
    {
        // RYQJ格式: Field1=区域编号, Field2=区域名称, Field3=人数
        var entities = records.Select(r => new PersonAreaStatistic
        {
            MineId = mineId,
            AreaCode = r.GetValueOrDefault("Field1")?.ToString(),
            AreaName = r.GetValueOrDefault("Field2")?.ToString(),
            PersonCount = Convert.ToInt32(r.GetValueOrDefault("Field3") ?? "0"),
            UpdateTime = DateTime.Now
        }).ToList();

        await _personAreaStatRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存皮带秤数据（JSLCDSS）
    /// </summary>
    private async Task<bool> SaveBeltScaleDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new BeltScaleData
        {
            MineId = mineId,
            BeltCode = r.GetValueOrDefault("Field1")?.ToString(),
            BeltName = r.GetValueOrDefault("Field3")?.ToString(),
            InstantFlow = Convert.ToDecimal(r.GetValueOrDefault("Field4") ?? "0"),
            TotalFlow = Convert.ToDecimal(r.GetValueOrDefault("Field5") ?? "0"),
            Status = Convert.ToInt32(r.GetValueOrDefault("Field2") ?? "0"),
            UpdateTime = DateTime.Now
        }).ToList();

        await _beltScaleRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 保存排水量数据（PSLCDSS）
    /// </summary>
    private async Task<bool> SaveDrainageDataAsync(long mineId, List<Dictionary<string, object>> records)
    {
        var entities = records.Select(r => new DrainageData
        {
            MineId = mineId,
            PumpCode = r.GetValueOrDefault("Field1")?.ToString(),
            PumpName = r.GetValueOrDefault("Field3")?.ToString(),
            InstantFlow = Convert.ToDecimal(r.GetValueOrDefault("Field4") ?? "0"),
            TotalFlow = Convert.ToDecimal(r.GetValueOrDefault("Field5") ?? "0"),
            RunStatus = Convert.ToInt32(r.GetValueOrDefault("Field2") ?? "0"),
            WaterLevel = Convert.ToDecimal(r.GetValueOrDefault("Field6") ?? "0"),
            UpdateTime = DateTime.Now
        }).ToList();

        await _drainageRep.AsInsertable(entities).ExecuteCommandAsync();
        return true;
    }
}

using Admin.NET.Core;
using SqlSugar;
using Furion;
using Furion.DynamicApiController;

namespace Admin.NET.Application;

/// <summary>
/// 数据解析服务
/// </summary>
public class DataParserService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public DataParserService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 解析安全监测数据
    /// </summary>
    public async Task<bool> ParseSafetyData(string fileContent, long mineId)
    {
        try
        {
            // 按行解析
            var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var list = new List<SafetyRealtime>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 4)
                {
                    list.Add(new SafetyRealtime
                    {
                        MineId = mineId,
                        SensorCode = parts[0].Trim(),
                        Value = decimal.TryParse(parts[1].Trim(), out var v) ? v : null,
                        Unit = parts[2].Trim(),
                        Status = int.TryParse(parts[3].Trim(), out var s) ? s : 0,
                        ReceivedTime = DateTime.Now
                    });
                }
            }

            if (list.Count > 0)
            {
                await _db.Insertable(list).ExecuteCommandAsync();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 解析人员定位数据
    /// </summary>
    public async Task<bool> ParsePersonData(string fileContent, long mineId)
    {
        try
        {
            var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var list = new List<PersonLocation>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 4)
                {
                    list.Add(new PersonLocation
                    {
                        MineId = mineId,
                        CardId = parts[0].Trim(),
                        PersonName = parts[1].Trim(),
                        StationId = parts[2].Trim(),
                        StationName = parts.Length > 3 ? parts[3].Trim() : ""
                    });
                }
            }

            if (list.Count > 0)
            {
                await _db.Insertable(list).ExecuteCommandAsync();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 解析水害监测数据
    /// </summary>
    public async Task<bool> ParseWaterData(string fileContent, long mineId)
    {
        try
        {
            var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var list = new List<WaterRealtime>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 4)
                {
                    list.Add(new WaterRealtime
                    {
                        MineId = mineId,
                        SensorCode = parts[0].Trim(),
                        SensorName = parts[1].Trim(),
                        WaterLevel = decimal.TryParse(parts[2].Trim(), out var w) ? w : null,
                        FlowRate = parts.Length > 3 && decimal.TryParse(parts[3].Trim(), out var f) ? f : null,
                        Status = 0
                    });
                }
            }

            if (list.Count > 0)
            {
                await _db.Insertable(list).ExecuteCommandAsync();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 煤矿数据解析结果
/// </summary>
public class ParseResult
{
    /// <summary>
    /// 煤矿标识
    /// </summary>
    public string MineCode { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 数据记录
    /// </summary>
    public List<Dictionary<string, object>> DataRecords { get; set; } = new();

    /// <summary>
    /// 记录数
    /// </summary>
    public int RecordCount => DataRecords?.Count ?? 0;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; }
}

/// <summary>
/// 煤矿数据解析服务
/// </summary>
public class CoalMineDataParser
{
    /// <summary>
    /// 支持的数据类型
    /// </summary>
    public static string[] SupportedDataTypes => new[] 
    { 
        "CDSS", "CDDY", "FZSS", "KGBH", "TJSJ", "YCBJ",    // 安全监测 (6)
        "RYSS", "RYCS", "RYCY", "RYQJ", "JZSS",             // 人员定位 (5)
        "CGKCDSS", "CGKCDDY",                               // 水害监测 (2)
        "JSLCDSS", "PSLCDSS",                               // 皮带秤/排水 (2)
        "KYCDSS", "KYCDDY"                                  // 矿压监测 (2)
    };                                                       // 共17种

    /// <summary>
    /// 解析文件
    /// </summary>
    public static ParseResult ParseFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new ParseResult { Success = false, ErrorMessage = "文件不存在" };
            }

            var fileName = Path.GetFileName(filePath);
            
            // 从文件名解析数据类型: 1234567890_CDSS_20240115.txt
            var dataType = ParseDataTypeFromFileName(fileName);
            if (string.IsNullOrEmpty(dataType))
            {
                return new ParseResult { Success = false, ErrorMessage = "无法从文件名解析数据类型" };
            }

            // 从文件名解析煤矿标识
            var mineCode = ParseMineCodeFromFileName(fileName);

            var lines = File.ReadAllLines(filePath);
            if (lines.Length < 2)
            {
                return new ParseResult { Success = false, ErrorMessage = "文件数据为空" };
            }

            // 解析数据
            var result = dataType switch
            {
                "CDSS" => ParseSafetyData(lines, mineCode),
                "CDDY" => ParseAlarmData(lines, mineCode),
                "FZSS" => ParsePressureData(lines, mineCode),
                "KGBH" => ParseSwitchAlarm(lines, mineCode),
                "RYSS" => ParsePersonLocation(lines, mineCode),
                "RYCS" => ParsePersonInfo(lines, mineCode),
                "RYCY" => ParsePersonRecord(lines, mineCode),
                "RYQJ" => ParseAreaCount(lines, mineCode),
                "JZSS" => ParseStationStatus(lines, mineCode),
                "CGKCDSS" => ParseWaterLevel(lines, mineCode),
                "CGKCDDY" => ParseWaterAlarm(lines, mineCode),
                "KYCDSS" => ParsePressure(lines, mineCode),
                "KYCDDY" => ParsePressureAlarm(lines, mineCode),
                _ => new ParseResult { Success = false, ErrorMessage = $"不支持的数据类型: {dataType}" }
            };

            result.FileName = fileName;
            return result;
        }
        catch (Exception ex)
        {
            return new ParseResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    /// <summary>
    /// 从文件名解析数据类型
    /// </summary>
    private static string ParseDataTypeFromFileName(string fileName)
    {
        // 文件名格式: 1234567890_CDSS_20240115.txt
        var parts = fileName.Split('_');
        if (parts.Length >= 2)
        {
            var dataType = parts[1].ToUpper();
            if (SupportedDataTypes.Contains(dataType))
            {
                return dataType;
            }
        }
        return null;
    }

    /// <summary>
    /// 从文件名解析煤矿标识
    /// </summary>
    private static string ParseMineCodeFromFileName(string fileName)
    {
        var parts = fileName.Split('_');
        return parts.Length >= 1 ? parts[0] : null;
    }

    /// <summary>
    /// 解析安全监测实时数据 CDSS
    /// 格式: SensorCode~Value~Unit~Status
    /// 示例: CD001~0.15~%CH4~0
    /// </summary>
    private static ParseResult ParseSafetyData(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "CDSS",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析报警数据 CDDY
    /// 格式: SensorCode~Value~Threshold~AlarmType~Time
    /// </summary>
    private static ParseResult ParseAlarmData(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "CDDY",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析负压数据 FZSS
    /// </summary>
    private static ParseResult ParsePressureData(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "FZSS",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析开关报警 KGBH
    /// </summary>
    private static ParseResult ParseSwitchAlarm(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "KGBH",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析人员实时定位 RYSS
    /// 格式: CardId~PersonName~AreaCode~AreaName~StationId~StationName~X~Y~Z~Time
    /// </summary>
    private static ParseResult ParsePersonLocation(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "RYSS",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析人员初始化 RYCS
    /// 格式: CardId~PersonName~DeptName~EmployeeNo~Phone~Position~Status
    /// </summary>
    private static ParseResult ParsePersonInfo(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "RYCS",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析人员出勤 RYCY
    /// 格式: CardId~PersonName~DeptName~AreaCode~AreaName~InTime~OutTime~Duration
    /// </summary>
    private static ParseResult ParsePersonRecord(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "RYCY",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析区域人数 RYQJ
    /// 格式: AreaCode~AreaName~PersonCount~Time
    /// </summary>
    private static ParseResult ParseAreaCount(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "RYQJ",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析基站状态 JZSS
    /// 格式: StationId~StationName~Status~Power~Signal~Time
    /// </summary>
    private static ParseResult ParseStationStatus(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "JZSS",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析水仓水位 CGKCDSS
    /// 格式: SensorCode~SensorName~WaterLevel~FlowRate~Temperature~Status
    /// </summary>
    private static ParseResult ParseWaterLevel(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "CGKCDSS",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析水害报警 CGKCDDY
    /// </summary>
    private static ParseResult ParseWaterAlarm(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "CGKCDDY",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析矿压监测 KYCDSS
    /// 格式: SensorCode~SensorName~Pressure~Depth~Position~Status
    /// </summary>
    private static ParseResult ParsePressure(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "KYCDSS",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析矿压报警 KYCDDY
    /// </summary>
    private static ParseResult ParsePressureAlarm(string[] lines, string mineCode)
    {
        var result = new ParseResult
        {
            DataType = "KYCDDY",
            MineCode = mineCode,
            Success = true
        };

        var headers = lines[0].Split('~');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split('~');
            var record = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            result.DataRecords.Add(record);
        }
        return result;
    }
}

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
    /// 数据时间
    /// </summary>
    public DateTime? DataTime { get; set; }

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
    /// 解析文件（使用默认分隔符）
    /// </summary>
    public static ParseResult ParseFile(string filePath)
    {
        return ParseFile(filePath, ";", "~");
    }

    /// <summary>
    /// 解析文件（自定义分隔符）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="fieldSeparator">字段分隔符</param>
    /// <param name="recordSeparator">记录分隔符</param>
    public static ParseResult ParseFile(string filePath, string fieldSeparator, string recordSeparator)
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

            // 解析文件头: <煤矿编号>;<煤矿名称>;<数据时间> (使用字段分隔符)
            var fileHeader = ParseFileHeader(lines[0], fieldSeparator);
            if (!string.IsNullOrEmpty(fileHeader.MineCode))
                mineCode = fileHeader.MineCode;
            var mineName = fileHeader.MineName;
            var dataTime = fileHeader.DataTime;

            // 解析数据（传入自定义分隔符）
            var result = dataType switch
            {
                "CDSS" => ParseSafetyData(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "CDDY" => ParseAlarmData(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "FZSS" => ParsePressureData(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "KGBH" => ParseSwitchAlarm(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "RYSS" => ParsePersonLocation(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "RYCS" => ParsePersonInfo(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "RYCY" => ParsePersonRecord(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "RYQJ" => ParseAreaCount(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "JZSS" => ParseStationStatus(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "CGKCDSS" => ParseWaterLevel(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "CGKCDDY" => ParseWaterAlarm(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "TJSJ" => ParseStatisticsData(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "YCBJ" => ParseAbnormalAlarm(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "JSLCDSS" => ParseBeltScaleData(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "PSLCDSS" => ParseDrainageData(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "KYCDSS" => ParsePressure(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
                "KYCDDY" => ParsePressureAlarm(lines, mineCode, mineName, dataTime, fieldSeparator, recordSeparator),
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
    /// 文件头信息
    /// </summary>
    private class FileHeader
    {
        public string MineCode { get; set; }
        public string MineName { get; set; }
        public DateTime? DataTime { get; set; }
    }

    /// <summary>
    /// 解析文件头
    /// 格式: <煤矿编号>;<煤矿名称>;<数据时间>
    /// 示例: 1234567890;测试煤矿;2024-01-15 10:00:00
    /// </summary>
    private static FileHeader ParseFileHeader(string headerLine, string fieldSeparator)
    {
        var header = new FileHeader();
        if (string.IsNullOrWhiteSpace(headerLine)) return header;

        var parts = headerLine.Split(fieldSeparator);
        if (parts.Length >= 1)
            header.MineCode = parts[0].Trim();
        if (parts.Length >= 2)
            header.MineName = parts[1].Trim();
        if (parts.Length >= 3 && DateTime.TryParse(parts[2].Trim(), out var dt))
            header.DataTime = dt;

        return header;
    }

    /// <summary>
    /// 解析安全监测实时数据 CDSS
    /// 格式: SensorCode~Value~Unit~Status
    /// 示例: CD001~0.15~%CH4~0
    /// </summary>
    private static ParseResult ParseSafetyData(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        var result = new ParseResult
        {
            DataType = "CDSS",
            MineCode = mineCode,
            MineName = mineName,
            DataTime = dataTime,
            Success = true
        };

        // 使用记录分隔符分割数据行
        var dataContent = string.Join("", lines.Skip(1));
        var records = dataContent.Split(recordSeparator, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var recordStr in records)
        {
            if (string.IsNullOrWhiteSpace(recordStr)) continue;
            var values = recordStr.Split(fieldSeparator);
            var record = new Dictionary<string, object>();
            // 字段按顺序命名
            for (int j = 0; j < values.Length; j++)
            {
                record[$"Field{j + 1}"] = values[j].Trim();
            }
            // 如果数据中没有时间戳，使用文件头的时间
            if (!record.ContainsKey("Time") && dataTime.HasValue)
                record["Time"] = dataTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            result.DataRecords.Add(record);
        }
        return result;
    }

    /// <summary>
    /// 解析报警数据 CDDY
    /// </summary>
    private static ParseResult ParseAlarmData(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "CDDY", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析负压数据 FZSS
    /// </summary>
    private static ParseResult ParsePressureData(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "FZSS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析开关报警 KGBH
    /// </summary>
    private static ParseResult ParseSwitchAlarm(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "KGBH", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析人员实时定位 RYSS
    /// </summary>
    private static ParseResult ParsePersonLocation(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "RYSS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析人员初始化 RYCS
    /// </summary>
    private static ParseResult ParsePersonInfo(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "RYCS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析人员出勤 RYCY
    /// </summary>
    private static ParseResult ParsePersonRecord(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "RYCY", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析区域人数 RYQJ
    /// </summary>
    private static ParseResult ParseAreaCount(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "RYQJ", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析基站状态 JZSS
    /// </summary>
    private static ParseResult ParseStationStatus(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "JZSS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析水仓水位 CGKCDSS
    /// </summary>
    private static ParseResult ParseWaterLevel(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "CGKCDSS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析水害报警 CGKCDDY
    /// </summary>
    private static ParseResult ParseWaterAlarm(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "CGKCDDY", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析统计汇总数据 TJSJ
    /// </summary>
    private static ParseResult ParseStatisticsData(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "TJSJ", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析异常报警数据 YCBJ
    /// </summary>
    private static ParseResult ParseAbnormalAlarm(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "YCBJ", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析皮带秤数据 JSLCDSS
    /// </summary>
    private static ParseResult ParseBeltScaleData(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "JSLCDSS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析排水量数据 PSLCDSS
    /// </summary>
    private static ParseResult ParseDrainageData(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "PSLCDSS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析矿压监测 KYCDSS
    /// </summary>
    private static ParseResult ParsePressure(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "KYCDSS", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 解析矿压报警 KYCDDY
    /// </summary>
    private static ParseResult ParsePressureAlarm(string[] lines, string mineCode, string mineName, DateTime? dataTime, string fieldSeparator, string recordSeparator)
    {
        return ParseWithSeparators(lines, mineCode, mineName, dataTime, "KYCDDY", fieldSeparator, recordSeparator);
    }

    /// <summary>
    /// 通用的分隔符解析方法
    /// </summary>
    private static ParseResult ParseWithSeparators(string[] lines, string mineCode, string mineName, DateTime? dataTime, string dataType, string fieldSeparator, string recordSeparator)
    {
        var result = new ParseResult
        {
            DataType = dataType,
            MineCode = mineCode,
            MineName = mineName,
            DataTime = dataTime,
            Success = true
        };

        // 使用记录分隔符分割数据行（跳过第一行文件头）
        var dataContent = string.Join("", lines.Skip(1));
        var records = dataContent.Split(recordSeparator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var recordStr in records)
        {
            if (string.IsNullOrWhiteSpace(recordStr)) continue;
            var values = recordStr.Split(fieldSeparator);
            var record = new Dictionary<string, object>();
            for (int j = 0; j < values.Length; j++)
            {
                record[$"Field{j + 1}"] = values[j].Trim();
            }
            // 如果数据中没有时间戳，使用文件头的时间
            if (!record.ContainsKey("Time") && dataTime.HasValue)
                record["Time"] = dataTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            result.DataRecords.Add(record);
        }
        return result;
    }
}

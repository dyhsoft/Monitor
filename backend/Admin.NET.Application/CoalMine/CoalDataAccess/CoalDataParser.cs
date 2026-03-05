using System.Text;
using System.Text.RegularExpressions;

namespace Admin.NET.Application.CoalMine.CoalDataAccess;

/// <summary>
/// 煤矿数据解析器
/// </summary>
public class CoalDataParser
{
    private readonly ParseOptions _options;

    public CoalDataParser(ParseOptions options = null)
    {
        _options = options ?? new ParseOptions();
    }

    /// <summary>
    /// 解析数据文件
    /// </summary>
    public ParseResult Parse(string content)
    {
        var result = new ParseResult();
        var startTime = DateTime.Now;

        try
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                result.Success = false;
                result.ErrorMessage = "文件内容为空";
                return result;
            }

            // 解析文件头信息（第一行）
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                result.Success = false;
                result.ErrorMessage = "文件无有效数据";
                return result;
            }

            // 解析文件头: 煤矿编号;煤矿名称;数据时间
            var header = ParseHeader(lines[0]);
            if (header == null)
            {
                result.Success = false;
                result.ErrorMessage = "文件头格式错误";
                return result;
            }

            result.MineCode = header.Value.mineCode;
            result.DataTime = header.Value.dataTime;
            result.FileType = GetFileType(header.Value.dataType);

            // 解析数据记录
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var record = ParseRecord(line, result.FileType);
                if (record != null)
                {
                    result.Records.Add(record);
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            result.ParseTimeMs = (int)(DateTime.Now - startTime).TotalMilliseconds;
        }

        return result;
    }

    /// <summary>
    /// 解析文件头
    /// </summary>
    private (string mineCode, string mineName, string dataType, DateTime dataTime)? ParseHeader(string line)
    {
        // 格式: 煤矿编号;煤矿名称;数据时间
        var parts = line.Split(_options.FieldSeparator.ToCharArray());
        if (parts.Length < 3) return null;

        var mineCode =Trim();
        var mineName = parts parts[0].[1].Trim();

        // 从文件名获取数据类型 (在文件头之前)
        var dataTypeMatch = Regex.Match(line, @"^(\d{10})_(\w+)_");
        if (!dataTypeMatch.Success)
        {
            // 尝试从完整路径解析
            dataTypeMatch = Regex.Match(mineCode, @"(\d{10})_(\w+)_");
        }

        string dataType = "";
        if (dataTypeMatch.Success)
        {
            mineCode = dataTypeMatch.Groups[1].Value;
            dataType = dataTypeMatch.Groups[2].Value;
        }

        // 解析时间
        DateTime dataTime;
        var timeStr = parts[2].Trim();
        if (!DateTime.TryParse(timeStr, out dataTime))
        {
            dataTime = DateTime.Now;
        }

        return (mineCode, mineName, dataType, dataTime);
    }

    /// <summary>
    /// 解析数据记录
    /// </summary>
    private Dictionary<string, object> ParseRecord(string line, DataFileType fileType)
    {
        var record = new Dictionary<string, object>();

        // 按~分割记录
        var parts = line.Split(_options.RecordSeparator.ToCharArray());
        if (parts.Length == 0) return null;

        // 根据数据类型解析
        return fileType switch
        {
            DataFileType.CDSS or DataFileType.CGKCDSS => ParseSafetyRecord(parts),
            DataFileType.RYSS => ParsePersonRecord(parts),
            DataFileType.CDDY or DataFileType.CGKCDDY => ParseAlarmRecord(parts),
            DataFileType.FZSS => ParseStationRecord(parts),
            _ => ParseGenericRecord(parts)
        };
    }

    /// <summary>
    /// 解析安全监测数据
    /// 格式: 测点编号;类型;名称;值;单位;状态;采集时间
    /// </summary>
    private Dictionary<string, object> ParseSafetyRecord(string[] parts)
    {
        var record = new Dictionary<string, object>();

        if (parts.Length >= 7)
        {
            record["SensorCode"] = parts[0].Trim();           // 测点编号
            record["SensorType"] = parts[1].Trim();          // 类型
            record["SensorName"] = parts[2].Trim();          // 名称
            record["Value"] = parts[3].Trim();              // 值
            record["Unit"] = parts[4].Trim();                // 单位
            record["Status"] = parts[5].Trim();              // 状态
            record["UpdateTime"] = parts[6].Trim();         // 采集时间
        }
        else if (parts.Length >= 5)
        {
            // 水害数据格式: 测点编号;状态;液位;温度;采集时间
            record["SensorCode"] = parts[0].Trim();
            record["Status"] = parts[1].Trim();
            record["WaterLevel"] = parts[2].Trim();
            record["Temperature"] = parts[3].Trim();
            if (parts.Length > 4)
                record["UpdateTime"] = parts[4].Trim();
        }

        return record;
    }

    /// <summary>
    /// 解析人员定位数据
    /// 格式: 人员卡号;姓名;区域编号;进入时间;最后更新时间
    /// </summary>
    private Dictionary<string, object> ParsePersonRecord(string[] parts)
    {
        var record = new Dictionary<string, object>();

        if (parts.Length >= 5)
        {
            record["CardId"] = parts[0].Trim();              // 人员卡号
            record["PersonName"] = parts[1].Trim();         // 姓名
            record["AreaCode"] = parts[2].Trim();           // 区域编号
            record["InTime"] = parts[3].Trim();             // 进入时间
            record["UpdateTime"] = parts[4].Trim();         // 最后更新时间
        }

        return record;
    }

    /// <summary>
    /// 解析报警数据
    /// </summary>
    private Dictionary<string, object> ParseAlarmRecord(string[] parts)
    {
        var record = new Dictionary<string, object>();

        if (parts.Length >= 7)
        {
            record["SensorCode"] = parts[0].Trim();
            record["SensorType"] = parts[1].Trim();
            record["SensorName"] = parts[2].Trim();
            record["Value"] = parts[3].Trim();
            record["Unit"] = parts[4].Trim();
            record["AlarmType"] = parts[5].Trim();
            record["AlarmTime"] = parts[6].Trim();
        }

        return record;
    }

    /// <summary>
    /// 解析分站数据
    /// </summary>
    private Dictionary<string, object> ParseStationRecord(string[] parts)
    {
        var record = new Dictionary<string, object>();

        if (parts.Length >= 4)
        {
            record["StationCode"] = parts[0].Trim();
            record["StationName"] = parts[1].Trim();
            record["Status"] = parts[2].Trim();
            record["UpdateTime"] = parts[3].Trim();
        }

        return record;
    }

    /// <summary>
    /// 通用解析
    /// </summary>
    private Dictionary<string, object> ParseGenericRecord(string[] parts)
    {
        var record = new Dictionary<string, object>();
        for (int i = 0; i < parts.Length; i++)
        {
            record[$"Field{i}"] = parts[i].Trim();
        }
        return record;
    }

    /// <summary>
    /// 获取文件类型
    /// </summary>
    private DataFileType GetFileType(string dataType)
    {
        return dataType?.ToUpper() switch
        {
            "CDSS" => DataFileType.CDSS,
            "CDDY" => DataFileType.CDDY,
            "FZSS" => DataFileType.FZSS,
            "KGBH" => DataFileType.KGBH,
            "TJSJ" => DataFileType.TJSJ,
            "YCBJ" => DataFileType.YCBJ,
            "RYSS" => DataFileType.RYSS,
            "RYCS" => DataFileType.RYCS,
            "RYCY" => DataFileType.RYCY,
            "RYQJ" => DataFileType.RYQJ,
            "JZSS" => DataFileType.JZSS,
            "CGKCDSS" => DataFileType.CGKCDSS,
            "CGKCDDY" => DataFileType.CGKCDDY,
            "JSLCDSS" => DataFileType.JSLCDSS,
            "PSLCDSS" => DataFileType.PSLCDSS,
            _ => DataFileType.Unknown
        };
    }

    /// <summary>
    /// 检测文件编码
    /// </summary>
    public static Encoding DetectEncoding(byte[] buffer)
    {
        // 检查BOM
        if (buffer.Length >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
            return Encoding.UTF8;

        if (buffer.Length >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE)
            return Encoding.Unicode;

        if (buffer.Length >= 2 && buffer[0] == 0xFE && buffer[1] == 0xFF)
            return Encoding.BigEndianUnicode;

        // 尝试解码
        var encodings = new[] { Encoding.UTF8, Encoding.GetEncoding("GBK"), Encoding.GetEncoding("GB2312"), Encoding.GetEncoding("GB18030") };

        foreach (var encoding in encodings)
        {
            try
            {
                var text = encoding.GetString(buffer);
                // 检查是否包含无效字符
                if (!text.Contains('\0') && text.Length > 10)
                {
                    // 检查是否包含中文
                    if (Regex.IsMatch(text, @"[\u4e00-\u9fa5]"))
                        return encoding;
                }
            }
            catch { }
        }

        return Encoding.UTF8;
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Admin.NET.Core;
using SqlSugar;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using System.Text;
using System.Text.RegularExpressions;
using Admin.NET.EntityFramework.Core;

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

        var mineCode = parts[0].Trim();
        var mineName = parts[1].Trim();

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
    /// 解析人员定位实时数据 (RYSS)
    /// 返回 PersonLocation 实体列表
    /// </summary>
    public static List<PersonLocation> ParseRYSS(string content)
    {
        var list = new List<PersonLocation>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            // 跳过文件头，从第一行数据开始
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 5)
                {
                    var location = new PersonLocation
                    {
                        CardId = parts[0].Trim(),
                        PersonName = parts[1].Trim(),
                        AreaCode = parts[2].Trim(),
                        AreaName = parts[2].Trim(), // TODO: 区域名称映射
                        StationId = parts.Length > 5 ? parts[5].Trim() : "",
                        StationName = parts.Length > 6 ? parts[6].Trim() : "",
                        UpdateTime = DateTime.Now
                    };

                    // 解析时间
                    if (DateTime.TryParse(parts[3].Trim(), out var inTime))
                        location.InTime = inTime;
                    if (DateTime.TryParse(parts[4].Trim(), out var updateTime))
                        location.UpdateTime = updateTime;

                    list.Add(location);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析RYSS数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析水仓水位数据 (CGKCDSS)
    /// 返回 WaterRealtime 实体列表
    /// </summary>
    public static List<WaterRealtime> ParseCGKCDSS(string content)
    {
        var list = new List<WaterRealtime>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 5)
                {
                    var water = new WaterRealtime
                    {
                        SensorCode = parts[0].Trim(),
                        SensorName = parts.Length > 1 ? parts[1].Trim() : "",
                        Status = parts.Length > 2 && int.TryParse(parts[2].Trim(), out var status) ? status : 0,
                        WaterLevel = parts.Length > 3 && decimal.TryParse(parts[3].Trim(), out var wl) ? wl : null,
                        Temperature = parts.Length > 4 && decimal.TryParse(parts[4].Trim(), out var temp) ? temp : null,
                        UpdateTime = parts.Length > 5 && DateTime.TryParse(parts[5].Trim(), out var ut) ? ut : DateTime.Now
                    };
                    list.Add(water);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析CGKCDSS数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析排水流量数据 (PSLCDSS)
    /// 返回 WaterRealtime 实体列表
    /// </summary>
    public static List<WaterRealtime> ParsePSLCDSS(string content)
    {
        var list = new List<WaterRealtime>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 5)
                {
                    var water = new WaterRealtime
                    {
                        SensorCode = parts[0].Trim(),
                        SensorName = parts.Length > 1 ? parts[1].Trim() : "",
                        Status = parts.Length > 2 && int.TryParse(parts[2].Trim(), out var status) ? status : 0,
                        FlowRate = parts.Length > 3 && decimal.TryParse(parts[3].Trim(), out var flow) ? flow : null,
                        UpdateTime = parts.Length > 4 && DateTime.TryParse(parts[4].Trim(), out var ut) ? ut : DateTime.Now
                    };
                    list.Add(water);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析PSLCDSS数据失败: {ex.Message}");
        }
        return list;
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

    /// <summary>
    /// 解析人员初始化数据 (RYCS)
    /// 返回人员卡号信息列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 卡号;姓名;部门;工种;职位;电话;照片标识
    /// </remarks>
    public static List<Dictionary<string, object>> ParseRYCS(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["CardId"] = parts[0].Trim(),
                        ["PersonName"] = parts.Length > 1 ? parts[1].Trim() : "",
                        ["Department"] = parts.Length > 2 ? parts[2].Trim() : "",
                        ["WorkType"] = parts.Length > 3 ? parts[3].Trim() : "",
                        ["Position"] = parts.Length > 4 ? parts[4].Trim() : "",
                        ["Phone"] = parts.Length > 5 ? parts[5].Trim() : "",
                        ["PhotoFlag"] = parts.Length > 6 ? parts[6].Trim() : ""
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析RYCS数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析人员出勤数据 (RYCY)
    /// 返回人员出勤记录列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 卡号;姓名;部门;入井时间;出井时间;工作时长;状态
    /// </remarks>
    public static List<Dictionary<string, object>> ParseRYCY(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["CardId"] = parts[0].Trim(),
                        ["PersonName"] = parts.Length > 1 ? parts[1].Trim() : "",
                        ["Department"] = parts.Length > 2 ? parts[2].Trim() : "",
                        ["InTime"] = parts.Length > 3 ? parts[3].Trim() : "",
                        ["OutTime"] = parts.Length > 4 ? parts[4].Trim() : "",
                        ["WorkDuration"] = parts.Length > 5 ? parts[5].Trim() : "",
                        ["Status"] = parts.Length > 6 ? parts[6].Trim() : "0"
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析RYCY数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析人员区域数据 (RYQJ)
    /// 返回区域人员统计列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 区域编号;区域名称;人数;进入时间
    /// </remarks>
    public static List<Dictionary<string, object>> ParseRYQJ(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["AreaCode"] = parts[0].Trim(),
                        ["AreaName"] = parts.Length > 1 ? parts[1].Trim() : "",
                        ["PersonCount"] = parts.Length > 2 && int.TryParse(parts[2].Trim(), out var count) ? count : 0,
                        ["UpdateTime"] = parts.Length > 3 ? parts[3].Trim() : ""
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析RYQJ数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析基站实时数据 (JZSS)
    /// 返回基站状态列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 基站编号;基站名称;状态;在线人数;设备状态;最后更新时间
    /// 状态: 0=正常, 1=故障, 2=离线
    /// </remarks>
    public static List<Dictionary<string, object>> ParseJZSS(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["StationCode"] = parts[0].Trim(),
                        ["StationName"] = parts.Length > 1 ? parts[1].Trim() : "",
                        ["Status"] = parts.Length > 2 && int.TryParse(parts[2].Trim(), out var status) ? status : 0,
                        ["OnlineCount"] = parts.Length > 3 && int.TryParse(parts[3].Trim(), out var count) ? count : 0,
                        ["DeviceStatus"] = parts.Length > 4 ? parts[4].Trim() : "",
                        ["UpdateTime"] = parts.Length > 5 ? parts[5].Trim() : ""
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析JZSS数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析开关报警数据 (KGBH)
    /// 返回开关量报警列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 测点编号;类型;名称;状态;报警时间;...
    /// 状态: 0=正常, 1=报警, 2=故障
    /// </remarks>
    public static List<Dictionary<string, object>> ParseKGBH(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["SensorCode"] = parts[0].Trim(),
                        ["SensorType"] = parts.Length > 1 ? parts[1].Trim() : "",
                        ["SensorName"] = parts.Length > 2 ? parts[2].Trim() : "",
                        ["Status"] = parts.Length > 3 && int.TryParse(parts[3].Trim(), out var status) ? status : 0,
                        ["AlarmTime"] = parts.Length > 4 ? parts[4].Trim() : "",
                        ["Value"] = parts.Length > 5 ? parts[5].Trim() : "",
                        ["Unit"] = parts.Length > 6 ? parts[6].Trim() : ""
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析KGBH数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析调校数据 (TJSJ)
    /// 返回设备校准记录列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 测点编号;类型;名称;调校前值;调校后值;调校人;调校时间
    /// </remarks>
    public static List<Dictionary<string, object>> ParseTJSJ(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["SensorCode"] = parts[0].Trim(),
                        ["SensorType"] = parts.Length > 1 ? parts[1].Trim() : "",
                        ["SensorName"] = parts.Length > 2 ? parts[2].Trim() : "",
                        ["BeforeValue"] = parts.Length > 3 ? parts[3].Trim() : "",
                        ["AfterValue"] = parts.Length > 4 ? parts[4].Trim() : "",
                        ["Calibrator"] = parts.Length > 5 ? parts[5].Trim() : "",
                        ["CalibrateTime"] = parts.Length > 6 ? parts[6].Trim() : ""
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析TJSJ数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析异常报警数据 (YCBJ)
    /// 返回设备异常报警列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 测点编号;类型;名称;异常类型;描述;时间
    /// 异常类型: 1=传感器故障, 2=通讯故障, 3=设备异常
    /// </remarks>
    public static List<Dictionary<string, object>> ParseYCBJ(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["SensorCode"] = parts[0].Trim(),
                        ["SensorType"] = parts.Length > 1 ? parts[1].Trim() : "",
                        ["SensorName"] = parts.Length > 2 ? parts[2].Trim() : "",
                        ["ExceptionType"] = parts.Length > 3 && int.TryParse(parts[3].Trim(), out var type) ? type : 0,
                        ["Description"] = parts.Length > 4 ? parts[4].Trim() : "",
                        ["AlarmTime"] = parts.Length > 5 ? parts[5].Trim() : ""
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析YCBJ数据失败: {ex.Message}");
        }
        return list;
    }

    /// <summary>
    /// 解析井下水仓流量数据 (JSLCDSS)
    /// 返回井下水仓监测数据列表
    /// </summary>
    /// <remarks>
    /// 数据格式: 测点编号;状态;流量;累计量;采集时间
    /// </remarks>
    public static List<Dictionary<string, object>> ParseJSLCDSS(string content)
    {
        var list = new List<Dictionary<string, object>>();
        try
        {
            if (string.IsNullOrWhiteSpace(content)) return list;

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var record = new Dictionary<string, object>
                    {
                        ["SensorCode"] = parts[0].Trim(),
                        ["Status"] = parts.Length > 1 && int.TryParse(parts[1].Trim(), out var status) ? status : 0,
                        ["FlowRate"] = parts.Length > 2 && decimal.TryParse(parts[2].Trim(), out var flow) ? flow : null,
                        ["TotalFlow"] = parts.Length > 3 && decimal.TryParse(parts[3].Trim(), out var total) ? total : null,
                        ["UpdateTime"] = parts.Length > 4 ? parts[4].Trim() : ""
                    };
                    list.Add(record);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析JSLCDSS数据失败: {ex.Message}");
        }
        return list;
    }
}

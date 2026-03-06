using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Admin.NET.Core;
using SqlSugar;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Admin.NET.ApplicationCoalDataAccess;
using Admin.NET.Core;
using Admin.NET.EntityFramework.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 文件监听服务 - 支持多目录监听
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "FileWatcher", Order = 100)]
public class FileWatcherService : IFileWatcherService, ITransient
{
    private readonly ISqlSugarClient _db;
    private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
    private readonly ILogger<FileWatcherService> _logger;

    // 各监测类型配置
    private readonly WatchPathConfig _safetyConfig;
    private readonly WatchPathConfig _personConfig;
    private readonly WatchPathConfig _waterConfig;
    private readonly WatchPathConfig _pressureConfig;
    private readonly WatchPathConfig _commonConfig;

    public FileWatcherService(ISqlSugarClient db)
    {
        _db = db;
        _logger = App.GetService<ILogger<FileWatcherService>>();

        // 安全监测目录配置
        _safetyConfig = new WatchPathConfig
        {
            Name = "安全监测",
            Code = "safety",
            WatchPath = App.Settings.GetValue<string>("CoalMine:Safety:WatchPath") ?? @"C:\CoalMineData\Safety",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Safety:BackupPath") ?? @"C:\CoalMineData\Safety\Backup",
            ProcessedPath = App.Settings.GetValue<string>("CoalMine:Safety:ProcessedPath") ?? @"C:\CoalMineData\Safety\Processed",
            ErrorPath = App.Settings.GetValue<string>("CoalMine:Safety:ErrorPath") ?? @"C:\CoalMineData\Safety\Error",
            FileTypes = new[] { "CDSS", "CDDY", "FZSS", "KGBH", "TJSJ", "YCBJ" }
        };

        // 人员定位目录配置
        _personConfig = new WatchPathConfig
        {
            Name = "人员定位",
            Code = "person",
            WatchPath = App.Settings.GetValue<string>("CoalMine:Person:WatchPath") ?? @"C:\CoalMineData\Person",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Person:BackupPath") ?? @"C:\CoalMineData\Person\Backup",
            ProcessedPath = App.Settings.GetValue<string>("CoalMine:Person:ProcessedPath") ?? @"C:\CoalMineData\Person\Processed",
            ErrorPath = App.Settings.GetValue<string>("CoalMine:Person:ErrorPath") ?? @"C:\CoalMineData\Person\Error",
            FileTypes = new[] { "RYSS", "RYCS", "RYCY", "RYQJ", "JZSS" }
        };

        // 水害监测目录配置
        _waterConfig = new WatchPathConfig
        {
            Name = "水害监测",
            Code = "water",
            WatchPath = App.Settings.GetValue<string>("CoalMine:Water:WatchPath") ?? @"C:\CoalMineData\Water",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Water:BackupPath") ?? @"C:\CoalMineData\Water\Backup",
            ProcessedPath = App.Settings.GetValue<string>("CoalMine:Water:ProcessedPath") ?? @"C:\CoalMineData\Water\Processed",
            ErrorPath = App.Settings.GetValue<string>("CoalMine:Water:ErrorPath") ?? @"C:\CoalMineData\Water\Error",
            FileTypes = new[] { "CGKCDSS", "CGKCDDY", "JSLCDSS", "PSLCDSS" }
        };

        // 矿压监测目录配置
        _pressureConfig = new WatchPathConfig
        {
            Name = "矿压监测",
            Code = "pressure",
            WatchPath = App.Settings.GetValue<string>("CoalMine:Pressure:WatchPath") ?? @"C:\CoalMineData\Pressure",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Pressure:BackupPath") ?? @"C:\CoalMineData\Pressure\Backup",
            ProcessedPath = App.Settings.GetValue<string>("CoalMine:Pressure:ProcessedPath") ?? @"C:\CoalMineData\Pressure\Processed",
            ErrorPath = App.Settings.GetValue<string>("CoalMine:Pressure:ErrorPath") ?? @"C:\CoalMineData\Pressure\Error",
            FileTypes = new[] { "KYCDSS", "KYCDDY" }
        };

        // 通用目录配置（备用）
        _commonConfig = new WatchPathConfig
        {
            Name = "通用",
            Code = "common",
            WatchPath = App.Settings.GetValue<string>("CoalMine:Common:WatchPath") ?? @"C:\CoalMineData\Receive",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Common:BackupPath") ?? @"C:\CoalMineData\Backup",
            ProcessedPath = App.Settings.GetValue<string>("CoalMine:Common:ProcessedPath") ?? @"C:\CoalMineData\Processed",
            ErrorPath = App.Settings.GetValue<string>("CoalMine:Common:ErrorPath") ?? @"C:\CoalMineData\Error"
        };

        // 确保所有目录存在
        EnsureDirectoriesExist(_safetyConfig);
        EnsureDirectoriesExist(_personConfig);
        EnsureDirectoriesExist(_waterConfig);
        EnsureDirectoriesExist(_pressureConfig);
        EnsureDirectoriesExist(_commonConfig);
    }

    /// <summary>
    /// 启动所有监听
    /// </summary>
    [HttpPost]
    public async Task<string> Start()
    {
        var results = new List<string>();

        // 启动各监测类型监听
        results.Add(await StartWatcher(_safetyConfig));
        results.Add(await StartWatcher(_personConfig));
        results.Add(await StartWatcher(_waterConfig));
        results.Add(await StartWatcher(_pressureConfig));

        return string.Join("; ", results);
    }

    /// <summary>
    /// 停止所有监听
    /// </summary>
    [HttpPost]
    public string Stop()
    {
        foreach (var watcher in _watchers.Values)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        _watchers.Clear();
        _logger.LogInformation("所有文件监听服务已停止");
        return "所有监听已停止";
    }

    /// <summary>
    /// 启动指定监测类型监听
    /// </summary>
    [HttpPost]
    public async Task<string> StartType(string type)
    {
        var config = GetConfig(type);
        if (config == null) return "未知的监测类型";
        return await StartWatcher(config);
    }

    /// <summary>
    /// 停止指定监测类型监听
    /// </summary>
    [HttpPost]
    public string StopType(string type)
    {
        var config = GetConfig(type);
        if (config == null) return "未知的监测类型";

        if (_watchers.TryGetValue(config.Code, out var watcher))
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            _watchers.Remove(config.Code);
        }

        return $"{config.Name}监听已停止";
    }

    /// <summary>
    /// 获取监听状态
    /// </summary>
    public async Task<Dictionary<string, object>> GetStatus()
    {
        return new Dictionary<string, object>
        {
            { "safety", new { name = "安全监测", path = _safetyConfig.WatchPath, running = _watchers.ContainsKey("safety") } },
            { "person", new { name = "人员定位", path = _personConfig.WatchPath, running = _watchers.ContainsKey("person") } },
            { "water", new { name = "水害监测", path = _waterConfig.WatchPath, running = _watchers.ContainsKey("water") } },
            { "pressure", new { name = "矿压监测", path = _pressureConfig.WatchPath, running = _watchers.ContainsKey("pressure") } }
        };
    }

    /// <summary>
    /// 手动触发文件处理
    /// </summary>
    [HttpPost]
    public async Task<Dictionary<string, object>> ProcessFile(string filePath, string type = "common")
    {
        var config = GetConfig(type) ?? _commonConfig;
        return await ProcessFileInternal(filePath, config);
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    private WatchPathConfig GetConfig(string type)
    {
        return type?.ToLower() switch
        {
            "safety" or "安全监测" => _safetyConfig,
            "person" or "人员定位" => _personConfig,
            "water" or "水害监测" => _waterConfig,
            "pressure" or "矿压监测" => _pressureConfig,
            _ => null
        };
    }

    /// <summary>
    /// 启动监听器
    /// </summary>
    private async Task<string> StartWatcher(WatchPathConfig config)
    {
        if (_watchers.ContainsKey(config.Code))
        {
            return $"{config.Name}监听已启动";
        }

        try
        {
            var watcher = new FileSystemWatcher(config.WatchPath)
            {
                Filter = "*.txt",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                IncludeSubdirectories = false
            };

            watcher.Created += async (sender, e) => await OnFileCreated(e.FullPath, config);
            watcher.Error += OnWatcherError;

            watcher.EnableRaisingEvents = true;
            _watchers[config.Code] = watcher;

            _logger.LogInformation($"{config.Name}文件监听服务已启动，监听路径: {config.WatchPath}");
            return $"{config.Name}监听启动成功";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"启动{config.Name}文件监听失败");
            return $"{config.Name}启动失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 处理新创建的文件
    /// </summary>
    private async Task OnFileCreated(string filePath, WatchPathConfig config)
    {
        try
        {
            await Task.Delay(1000);

            if (!IsFileReady(filePath))
            {
                _logger.LogWarning($"文件被占用，等待重试: {filePath}");
                await Task.Delay(2000);
            }

            await ProcessFileInternal(filePath, config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"处理文件失败: {filePath}");
        }
    }

    /// <summary>
    /// 处理文件内部方法
    /// </summary>
    private async Task<Dictionary<string, object>> ProcessFileInternal(string filePath, WatchPathConfig config)
    {
        var result = new Dictionary<string, object>
        {
            { "filePath", filePath },
            { "monitorType", config.Name },
            { "success", false },
            { "recordCount", 0 },
            { "error", "" }
        };

        try
        {
            if (!File.Exists(filePath))
            {
                result["error"] = "文件不存在";
                return result;
            }

            // 备份原始文件
            await BackupFile(filePath, config);

            // 读取并解析
            var bytes = await File.ReadAllBytesAsync(filePath);
            var encoding = CoalDataParser.DetectEncoding(bytes);
            var content = encoding.GetString(bytes);

            var parser = new CoalDataParser();
            var parseResult = parser.Parse(content);

            if (!parseResult.Success)
            {
                result["error"] = parseResult.ErrorMessage;
                MoveToError(filePath, config);
                return result;
            }

            // 根据监测类型处理数据
            int recordCount = 0;
            if (config.Code == "safety")
                recordCount = await SaveSafetyData(parseResult);
            else if (config.Code == "person")
                recordCount = await SavePersonData(parseResult);
            else if (config.Code == "water")
                recordCount = await SaveWaterData(parseResult);
            else if (config.Code == "pressure")
                recordCount = await SavePressureData(parseResult);

            // 保存解析日志
            await SaveParseLog(parseResult, content, bytes.Length, recordCount, 0, null, config);

            // 移动到已处理目录
            MoveToProcessed(filePath, config);

            result["success"] = true;
            result["recordCount"] = recordCount;
            result["dataType"] = parseResult.FileType.ToString();

            _logger.LogInformation($"文件处理成功: {filePath}, 类型: {config.Name}, 记录数: {recordCount}");
        }
        catch (Exception ex)
        {
            // 保存错误日志
            try
            {
                await SaveParseLog(null, "", 0, 0, 0, ex.Message, config);
            }
            catch { }

            result["error"] = ex.Message;
            _logger.LogError(ex, $"处理文件异常: {filePath}");
            MoveToError(filePath, config);
        }

        return result;
    }

    /// <summary>
    /// 保存解析日志
    /// </summary>
    private async Task SaveParseLog(ParseResult parseResult, string content, long fileSize, int recordCount, int errorCount, string errorMessage, WatchPathConfig config)
    {
        try
        {
            var mine = parseResult != null ? await _db.Queryable<CoalMine>().Where(c => c.Code == parseResult.MineCode).FirstAsync() : null;

            var log = new ParseLog
            {
                MineId = mine?.Id ?? 0,
                MineCode = parseResult?.MineCode ?? "",
                FileName = "",
                FilePath = "",
                FileType = parseResult?.FileType.ToString() ?? "",
                Encoding = "UTF-8",
                FileSize = fileSize,
                SourceContent = content.Length > 64000 ? content.Substring(0, 64000) : content, // 限制内容长度
                RecordCount = recordCount,
                SuccessCount = recordCount - errorCount,
                ErrorCount = errorCount,
                ParseTime = parseResult?.ParseTimeMs,
                ErrorMessage = errorMessage,
                Status = string.IsNullOrEmpty(errorMessage) ? 1 : 2,
                CreateTime = DateTime.Now
            };

            await _db.Insertable(log).ExecuteReturnIdentityAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存解析日志失败");
        }
    }
    /// 备份原始文件
    /// </summary>
    private async Task BackupFile(string filePath, WatchPathConfig config)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var backupPath = Path.Combine(config.BackupPath, $"{timestamp}_{fileName}");
            
            await Task.Run(() => File.Copy(filePath, backupPath, true));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"备份文件失败: {filePath}");
        }
    }

    /// <summary>
    /// 保存安全监测数据
    /// </summary>
    private async Task<int> SaveSafetyData(ParseResult parseResult)
    {
        var mine = await _db.Queryable<CoalMine>()
            .Where(c => c.Code == parseResult.MineCode)
            .FirstAsync();

        if (mine == null) return 0;

        int count = 0;
        foreach (var record in parseResult.Records)
        {
            try
            {
                var sensorCode = record.GetValueOrDefault("SensorCode")?.ToString();
                if (string.IsNullOrEmpty(sensorCode)) continue;

                var existing = await _db.Queryable<SafetyRealtime>()
                    .Where(s => s.MineId == mine.Id && s.SensorCode == sensorCode)
                    .FirstAsync();

                var realtime = new SafetyRealtime
                {
                    MineId = mine.Id,
                    SensorCode = sensorCode,
                    Value = record.GetValueOrDefault("Value")?.ToString()?.ParseToDecimal(),
                    Unit = record.GetValueOrDefault("Unit")?.ToString(),
                    Status = record.GetValueOrDefault("Status")?.ToString()?.ParseToInt() ?? 0,
                    UpdateTime = parseResult.DataTime,
                    ReceivedTime = DateTime.Now
                };

                if (existing != null)
                {
                    existing.Value = realtime.Value;
                    existing.Unit = realtime.Unit;
                    existing.Status = realtime.Status;
                    existing.UpdateTime = realtime.UpdateTime;
                    existing.ReceivedTime = DateTime.Now;
                    await _db.Updateable(existing).ExecuteCommandAsync();
                }
                else
                {
                    await _db.Insertable(realtime).ExecuteReturnIdentityAsync();
                }

                var history = new SafetyHistory
                {
                    MineId = mine.Id,
                    SensorCode = sensorCode,
                    Value = realtime.Value,
                    Unit = realtime.Unit,
                    Status = realtime.Status,
                    UpdateTime = realtime.UpdateTime,
                    ReceivedTime = DateTime.Now
                };
                await _db.Insertable(history).ExecuteReturnIdentityAsync();
                count++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存安全监测数据失败");
            }
        }
        return count;
    }

    /// <summary>
    /// 保存人员定位数据
    /// </summary>
    private async Task<int> SavePersonData(ParseResult parseResult)
    {
        var mine = await _db.Queryable<CoalMine>()
            .Where(c => c.Code == parseResult.MineCode)
            .FirstAsync();

        if (mine == null) return 0;

        int count = 0;
        foreach (var record in parseResult.Records)
        {
            try
            {
                var cardId = record.GetValueOrDefault("CardId")?.ToString();
                if (string.IsNullOrEmpty(cardId)) continue;

                var existing = await _db.Queryable<PersonLocation>()
                    .Where(p => p.MineId == mine.Id && p.CardId == cardId)
                    .FirstAsync();

                if (existing != null)
                {
                    existing.StationId = record.GetValueOrDefault("StationId")?.ToString();
                    existing.StationName = record.GetValueOrDefault("StationName")?.ToString();
                    existing.AreaCode = record.GetValueOrDefault("AreaCode")?.ToString();
                    existing.AreaName = record.GetValueOrDefault("AreaName")?.ToString();
                    existing.UpdateTime = DateTime.Now;
                    await _db.Updateable(existing).ExecuteCommandAsync();
                }
                else
                {
                    var location = new PersonLocation
                    {
                        MineId = mine.Id,
                        CardId = cardId,
                        PersonName = record.GetValueOrDefault("PersonName")?.ToString(),
                        StationId = record.GetValueOrDefault("StationId")?.ToString(),
                        StationName = record.GetValueOrDefault("StationName")?.ToString(),
                        AreaCode = record.GetValueOrDefault("AreaCode")?.ToString(),
                        AreaName = record.GetValueOrDefault("AreaName")?.ToString(),
                        InTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    await _db.Insertable(location).ExecuteReturnIdentityAsync();
                }
                count++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存人员定位数据失败");
            }
        }
        return count;
    }

    /// <summary>
    /// 保存水害监测数据
    /// </summary>
    private async Task<int> SaveWaterData(ParseResult parseResult)
    {
        var mine = await _db.Queryable<CoalMine>()
            .Where(c => c.Code == parseResult.MineCode)
            .FirstAsync();

        if (mine == null) return 0;

        int count = 0;
        foreach (var record in parseResult.Records)
        {
            try
            {
                var sensorCode = record.GetValueOrDefault("SensorCode")?.ToString();
                if (string.IsNullOrEmpty(sensorCode)) continue;

                var existing = await _db.Queryable<WaterRealtime>()
                    .Where(w => w.MineId == mine.Id && w.SensorCode == sensorCode)
                    .FirstAsync();

                var water = new WaterRealtime
                {
                    MineId = mine.Id,
                    SensorCode = sensorCode,
                    SensorName = record.GetValueOrDefault("SensorName")?.ToString(),
                    Status = record.GetValueOrDefault("Status")?.ToString()?.ParseToInt() ?? 0,
                    WaterLevel = record.GetValueOrDefault("WaterLevel")?.ToString()?.ParseToDecimal(),
                    FlowRate = record.GetValueOrDefault("FlowRate")?.ToString()?.ParseToDecimal(),
                    Temperature = record.GetValueOrDefault("Temperature")?.ToString()?.ParseToDecimal(),
                    UpdateTime = parseResult.DataTime
                };

                if (existing != null)
                {
                    existing.Status = water.Status;
                    existing.WaterLevel = water.WaterLevel;
                    existing.FlowRate = water.FlowRate;
                    existing.Temperature = water.Temperature;
                    existing.UpdateTime = water.UpdateTime;
                    await _db.Updateable(existing).ExecuteCommandAsync();
                }
                else
                {
                    await _db.Insertable(water).ExecuteReturnIdentityAsync();
                }
                count++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存水害监测数据失败");
            }
        }
        return count;
    }

    /// <summary>
    /// 保存矿压监测数据
    /// </summary>
    private async Task<int> SavePressureData(ParseResult parseResult)
    {
        // 矿压监测目前复用安全监测表存储
        return await SaveSafetyData(parseResult);
    }

    /// <summary>
    /// 检查文件是否准备好
    /// </summary>
    private bool IsFileReady(string filePath)
    {
        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// 移动到已处理目录
    /// </summary>
    private void MoveToProcessed(string filePath, WatchPathConfig config)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var newPath = Path.Combine(config.ProcessedPath, $"{timestamp}_{fileName}");
            File.Move(filePath, newPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"移动文件到已处理目录失败: {filePath}");
        }
    }

    /// <summary>
    /// 移动到错误目录
    /// </summary>
    private void MoveToError(string filePath, WatchPathConfig config)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var newPath = Path.Combine(config.ErrorPath, $"{timestamp}_{fileName}");
            File.Move(filePath, newPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"移动文件到错误目录失败: {filePath}");
        }
    }

    /// <summary>
    /// 确保目录存在
    /// </summary>
    private void EnsureDirectoriesExist(WatchPathConfig config)
    {
        if (!Directory.Exists(config.WatchPath)) Directory.CreateDirectory(config.WatchPath);
        if (!Directory.Exists(config.BackupPath)) Directory.CreateDirectory(config.BackupPath);
        if (!Directory.Exists(config.ProcessedPath)) Directory.CreateDirectory(config.ProcessedPath);
        if (!Directory.Exists(config.ErrorPath)) Directory.CreateDirectory(config.ErrorPath);
    }

    /// <summary>
    /// 监听错误事件
    /// </summary>
    private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException(), "文件监听发生错误");
    }
}

/// <summary>
/// 目录配置
/// </summary>
public class WatchPathConfig
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string WatchPath { get; set; }
    public string BackupPath { get; set; }
    public string ProcessedPath { get; set; }
    public string ErrorPath { get; set; }
    public string[] FileTypes { get; set; }
}

/// <summary>
/// 文件监听服务接口
/// </summary>
public interface IFileWatcherService
{
    Task<string> Start();
    Task<string> Stop();
    Task<string> StartType(string type);
    Task<string> StopType(string type);
    Task<Dictionary<string, object>> GetStatus();
    Task<Dictionary<string, object>> ProcessFile(string filePath, string type = "common");
}

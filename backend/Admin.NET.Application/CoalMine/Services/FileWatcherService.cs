using Admin.NET.Application.CoalMine.CoalDataAccess;
using Admin.NET.Core;
using Admin.NET.EntityFramework.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 文件监听服务 - 监听共享文件夹
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "FileWatcher", Order = 100)]
public class FileWatcherService : IFileWatcherService, ITransient
{
    private readonly ISqlSugarClient _db;
    private FileSystemWatcher _watcher;
    private readonly string _watchPath;
    private readonly string _backupPath;
    private readonly string _processedPath;
    private readonly string _errorPath;
    private readonly ILogger<FileWatcherService> _logger;

    public FileWatcherService(ISqlSugarClient db)
    {
        _db = db;
        _logger = App.GetService<ILogger<FileWatcherService>>();

        // 配置路径（从数据库或配置读取）
        _watchPath = App.Settings.GetValue<string>("CoalMine:WatchPath") ?? @"C:\CoalMineData\Receive";
        _backupPath = App.Settings.GetValue<string>("CoalMine:BackupPath") ?? @"C:\CoalMineData\Backup";
        _processedPath = App.Settings.GetValue<string>("CoalMine:ProcessedPath") ?? @"C:\CoalMineData\Processed";
        _errorPath = App.Settings.GetValue<string>("CoalMine:ErrorPath") ?? @"C:\CoalMineData\Error";

        // 确保目录存在
        EnsureDirectoryExists(_watchPath);
        EnsureDirectoryExists(_backupPath);
        EnsureDirectoryExists(_processedPath);
        EnsureDirectoryExists(_errorPath);
    }

    /// <summary>
    /// 启动文件监听
    /// </summary>
    [HttpPost]
    public async Task<string> Start()
    {
        if (_watcher != null && _watcher.EnableRaisingEvents)
        {
            return "监听已启动";
        }

        try
        {
            _watcher = new FileSystemWatcher(_watchPath)
            {
                Filter = "*.txt",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                IncludeSubdirectories = false
            };

            _watcher.Created += async (sender, e) => await OnFileCreated(e.FullPath);
            _watcher.Error += OnWatcherError;

            _watcher.EnableRaisingEvents = true;

            _logger.LogInformation($"文件监听服务已启动，监听路径: {_watchPath}");

            return "监听启动成功";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动文件监听失败");
            return $"启动失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 停止文件监听
    /// </summary>
    [HttpPost]
    public string Stop()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _watcher = null;
            _logger.LogInformation("文件监听服务已停止");
        }
        return "监听已停止";
    }

    /// <summary>
    /// 获取监听状态
    /// </summary>
    public async Task<Dictionary<string, object>> GetStatus()
    {
        return new Dictionary<string, object>
        {
            { "isRunning", _watcher?.EnableRaisingEvents ?? false },
            { "watchPath", _watchPath },
            { "backupPath", _backupPath },
            { "processedPath", _processedPath },
            { "errorPath", _errorPath }
        };
    }

    /// <summary>
    /// 手动触发文件处理
    /// </summary>
    [HttpPost]
    public async Task<Dictionary<string, object>> ProcessFile(string filePath)
    {
        return await ProcessFileInternal(filePath);
    }

    /// <summary>
    /// 处理新创建的文件
    /// </summary>
    private async Task OnFileCreated(string filePath)
    {
        try
        {
            // 等待文件写入完成
            await Task.Delay(1000);

            // 检查文件是否被占用
            if (!IsFileReady(filePath))
            {
                _logger.LogWarning($"文件被占用，等待重试: {filePath}");
                await Task.Delay(2000);
            }

            await ProcessFileInternal(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"处理文件失败: {filePath}");
        }
    }

    /// <summary>
    /// 处理文件内部方法
    /// </summary>
    private async Task<Dictionary<string, object>> ProcessFileInternal(string filePath)
    {
        var result = new Dictionary<string, object>
        {
            { "filePath", filePath },
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

            // 读取文件内容
            var content = await File.ReadAllTextAsync(filePath);

            // 检测编码
            var bytes = await File.ReadAllBytesAsync(filePath);
            var encoding = CoalDataParser.DetectEncoding(bytes);
            content = encoding.GetString(bytes);

            // 解析数据
            var parser = new CoalDataParser();
            var parseResult = parser.Parse(content);

            if (!parseResult.Success)
            {
                result["error"] = parseResult.ErrorMessage;
                MoveToError(filePath);
                return result;
            }

            // 根据数据类型处理
            int recordCount = 0;
            switch (parseResult.FileType)
            {
                case DataFileType.CDSS:
                case DataFileType.CDDY:
                case DataFileType.FZSS:
                case DataFileType.KGBH:
                case DataFileType.TJSJ:
                case DataFileType.YCBJ:
                    recordCount = await SaveSafetyData(parseResult);
                    break;

                case DataFileType.RYSS:
                case DataFileType.RYCS:
                case DataFileType.RYCY:
                case DataFileType.RYQJ:
                case DataFileType.JZSS:
                    recordCount = await SavePersonData(parseResult);
                    break;

                case DataFileType.CGKCDSS:
                case DataFileType.CGKCDDY:
                case DataFileType.JSLCDSS:
                case DataFileType.PSLCDSS:
                    recordCount = await SaveWaterData(parseResult);
                    break;

                default:
                    result["error"] = "未知数据类型";
                    MoveToError(filePath);
                    return result;
            }

            // 记录处理日志
            await SaveProcessLog(parseResult, recordCount);

            // 移动到已处理目录
            MoveToProcessed(filePath);

            result["success"] = true;
            result["recordCount"] = recordCount;
            result["dataType"] = parseResult.FileType.ToString();
            result["mineCode"] = parseResult.MineCode;

            _logger.LogInformation($"文件处理成功: {filePath}, 记录数: {recordCount}");
        }
        catch (Exception ex)
        {
            result["error"] = ex.Message;
            _logger.LogError(ex, $"处理文件异常: {filePath}");
            MoveToError(filePath);
        }

        return result;
    }

    /// <summary>
    /// 保存安全监测数据
    /// </summary>
    private async Task<int> SaveSafetyData(ParseResult parseResult)
    {
        // 根据煤矿编号获取煤矿ID
        var mine = await _db.Queryable<CoalMine>()
            .Where(c => c.Code == parseResult.MineCode)
            .FirstAsync();

        if (mine == null)
        {
            _logger.LogWarning($"煤矿编号不存在: {parseResult.MineCode}");
            return 0;
        }

        int count = 0;
        foreach (var record in parseResult.Records)
        {
            try
            {
                var sensorCode = record.GetValueOrDefault("SensorCode")?.ToString();
                if (string.IsNullOrEmpty(sensorCode)) continue;

                // 查询是否已存在
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

                // 同时存入历史表
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
        // 根据煤矿编号获取煤矿ID
        var mine = await _db.Queryable<CoalMine>()
            .Where(c => c.Code == parseResult.MineCode)
            .FirstAsync();

        if (mine == null)
        {
            _logger.LogWarning($"煤矿编号不存在: {parseResult.MineCode}");
            return 0;
        }

        int count = 0;

        // 根据文件类型分别处理
        switch (parseResult.FileType)
        {
            case DataFileType.RYSS:
                // 人员实时位置
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
                        _logger.LogError(ex, "保存人员实时位置失败");
                    }
                }
                break;

            case DataFileType.RYCS:
                // 人员初始化信息 - 可以存入人员信息表或更新现有记录
                foreach (var record in parseResult.Records)
                {
                    var cardId = record.GetValueOrDefault("CardId")?.ToString();
                    if (string.IsNullOrEmpty(cardId)) continue;

                    // TODO: 存入人员信息表 PersonInfo
                    _logger.LogInformation($"人员初始化: {cardId}, 姓名: {record.GetValueOrDefault("PersonName")}");
                    count++;
                }
                break;

            case DataFileType.RYCY:
                // 人员出勤记录 - 存入历史表
                foreach (var record in parseResult.Records)
                {
                    var cardId = record.GetValueOrDefault("CardId")?.ToString();
                    if (string.IsNullOrEmpty(cardId)) continue;

                    // TODO: 存入人员出勤历史表 PersonAttendance
                    _logger.LogInformation($"人员出勤: {cardId}, 入井: {record.GetValueOrDefault("InTime")}");
                    count++;
                }
                break;

            case DataFileType.RYQJ:
                // 区域人员统计 - 可用于展示
                foreach (var record in parseResult.Records)
                {
                    _logger.LogInformation($"区域: {record.GetValueOrDefault("AreaCode")}, 人数: {record.GetValueOrDefault("PersonCount")}");
                    count++;
                }
                break;

            case DataFileType.JZSS:
                // 基站状态
                foreach (var record in parseResult.Records)
                {
                    _logger.LogInformation($"基站: {record.GetValueOrDefault("StationCode")}, 状态: {record.GetValueOrDefault("Status")}");
                    count++;
                }
                break;
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
    /// 保存处理日志
    /// </summary>
    private async Task SaveProcessLog(ParseResult parseResult, int recordCount)
    {
        try
        {
            // 可以存入数据库或日志文件
            _logger.LogInformation($"数据处理完成: 类型={parseResult.FileType}, 煤矿={parseResult.MineCode}, 记录数={recordCount}, 耗时={parseResult.ParseTimeMs}ms");
        }
        catch { }
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
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 移动到已处理目录
    /// </summary>
    private void MoveToProcessed(string filePath)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var newPath = Path.Combine(_processedPath, $"{timestamp}_{fileName}");
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
    private void MoveToError(string filePath)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var newPath = Path.Combine(_errorPath, $"{timestamp}_{fileName}");
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
    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
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
/// 文件监听服务接口
/// </summary>
public interface IFileWatcherService
{
    Task<string> Start();
    Task<string> Stop();
    Task<Dictionary<string, object>> GetStatus();
    Task<Dictionary<string, object>> ProcessFile(string filePath);
}

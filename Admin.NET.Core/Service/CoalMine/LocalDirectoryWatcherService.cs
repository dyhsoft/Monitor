// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何法律责任！

using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 本地目录监听服务
/// 监听配置的本地目录，自动读取并解析新文件
/// </summary>
public class LocalDirectoryWatcherService : ITransient
{
    private readonly SqlSugarRepository<ListenerConfig> _listenerRep;
    private readonly CoalDataSaveService _dataSaveService;
    private readonly ILogger<LocalDirectoryWatcherService> _logger;
    private readonly Dictionary<long, FileSystemWatcher> _watchers = new();
    private readonly Dictionary<long, DateTime> _lastProcessed = new();

    public LocalDirectoryWatcherService(
        SqlSugarRepository<ListenerConfig> listenerRep,
        CoalDataSaveService dataSaveService,
        ILogger<LocalDirectoryWatcherService> logger)
    {
        _listenerRep = listenerRep;
        _dataSaveService = dataSaveService;
        _logger = logger;
    }

    /// <summary>
    /// 启动所有启用的目录监听
    /// </summary>
    public async Task StartAllWatchersAsync()
    {
        // 只启动监听类型为"本地目录(1)"的配置
        var configs = await _listenerRep.AsQueryable()
            .Where(c => c.Enabled == 1 && c.ListenerType == 1)
            .ToListAsync();

        foreach (var config in configs)
        {
            try
            {
                StartWatcher(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动目录监听失败: {ConfigId}, 路径: {Path}", config.Id, config.ListenPath);
            }
        }

        _logger.LogInformation("目录监听服务启动完成，共 {Count} 个监听", configs.Count);
    }

    /// <summary>
    /// 启动单个目录监听
    /// </summary>
    public void StartWatcher(ListenerConfig config)
    {
        if (_watchers.ContainsKey(config.Id))
        {
            _logger.LogWarning("目录监听已存在: {ConfigId}", config.Id);
            return;
        }

        if (string.IsNullOrEmpty(config.ListenPath) || !Directory.Exists(config.ListenPath))
        {
            throw new Exception($"监听目录不存在: {config.ListenPath}");
        }

        var watcher = new FileSystemWatcher(config.ListenPath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };

        // 设置文件过滤
        if (!string.IsNullOrEmpty(config.FilePattern))
        {
            watcher.Filter = config.FilePattern;
        }
        else
        {
            watcher.Filter = "*.txt";
        }

        // 文件创建事件
        watcher.Created += async (s, e) => await OnFileCreated(config, e.FullPath);
        
        // 复制事件（某些FTP客户端使用复制）
        watcher.Changed += async (s, e) => await OnFileChanged(config, e.FullPath);

        _watchers[config.Id] = watcher;
        _logger.LogInformation("已启动目录监听: {Path}, 绑定系统: {BindSystem}", config.ListenPath, config.BindSystem);
    }

    /// <summary>
    /// 停止单个目录监听
    /// </summary>
    public void StopWatcher(long configId)
    {
        if (_watchers.TryGetValue(configId, out var watcher))
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            _watchers.Remove(configId);
            _logger.LogInformation("已停止目录监听: {ConfigId}", configId);
        }
    }

    /// <summary>
    /// 停止所有监听
    /// </summary>
    public void StopAllWatchers()
    {
        foreach (var watcher in _watchers.Values)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        _watchers.Clear();
        _logger.LogInformation("已停止所有目录监听");
    }

    /// <summary>
    /// 文件创建事件处理
    /// </summary>
    private async Task OnFileCreated(ListenerConfig config, string filePath)
    {
        // 防止重复处理
        if (_lastProcessed.TryGetValue(config.Id, out var lastTime) && 
            (DateTime.Now - lastTime).TotalSeconds < 5)
        {
            return;
        }

        // 等待文件写入完成
        await Task.Delay(1000);

        // 检查文件是否可访问
        if (!await WaitForFileAsync(filePath, TimeSpan.FromSeconds(10)))
        {
            _logger.LogWarning("文件无法访问: {FilePath}", filePath);
            return;
        }

        await ProcessFileAsync(config, filePath);
    }

    /// <summary>
    /// 文件变更事件处理
    /// </summary>
    private async Task OnFileChanged(ListenerConfig config, string filePath)
    {
        await OnFileCreated(config, filePath);
    }

    /// <summary>
    /// 处理文件
    /// </summary>
    private async Task ProcessFileAsync(ListenerConfig config, string filePath)
    {
        try
        {
            _lastProcessed[config.Id] = DateTime.Now;
            _logger.LogInformation("开始处理文件: {FilePath}", filePath);

            // 解析并保存数据（使用配置的分隔符）
            var fieldSep = !string.IsNullOrEmpty(config.FieldSeparator) ? config.FieldSeparator : ";";
            var recordSep = !string.IsNullOrEmpty(config.RecordSeparator) ? config.RecordSeparator : "~";
            var result = await _dataSaveService.ParseAndSaveAsync(filePath, fieldSep, recordSep);

            if (result.Success)
            {
                _logger.LogInformation("文件处理成功: {FilePath}, 记录数: {Count}", filePath, result.RecordCount);

                // 处理完成后的文件移动
                if (!string.IsNullOrEmpty(config.ProcessedPath))
                {
                    if (!Directory.Exists(config.ProcessedPath))
                    {
                        Directory.CreateDirectory(config.ProcessedPath);
                    }

                    var fileName = Path.GetFileName(filePath);
                    var destPath = Path.Combine(config.ProcessedPath, fileName);
                    
                    // 避免文件覆盖，添加时间戳
                    if (File.Exists(destPath))
                    {
                        destPath = Path.Combine(config.ProcessedPath, 
                            $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");
                    }

                    File.Move(filePath, destPath);
                    _logger.LogInformation("文件已移动到: {DestPath}", destPath);
                }
                else
                {
                    // 删除原文件
                    File.Delete(filePath);
                    _logger.LogInformation("原文件已删除: {FilePath}", filePath);
                }
            }
            else
            {
                _logger.LogWarning("文件处理失败: {FilePath}, 错误: {Error}", filePath, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理文件异常: {FilePath}", filePath);
        }
    }

    /// <summary>
    /// 等待文件可访问
    /// </summary>
    private async Task<bool> WaitForFileAsync(string filePath, TimeSpan timeout)
    {
        var endTime = DateTime.Now + timeout;
        while (DateTime.Now < endTime)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                return true;
            }
            catch (IOException)
            {
                await Task.Delay(500);
            }
        }
        return false;
    }

    /// <summary>
    /// 获取监听状态
    /// </summary>
    public Dictionary<long, bool> GetWatcherStatus()
    {
        return _watchers.ToDictionary(w => w.Key, w => w.Value.EnableRaisingEvents);
    }
}

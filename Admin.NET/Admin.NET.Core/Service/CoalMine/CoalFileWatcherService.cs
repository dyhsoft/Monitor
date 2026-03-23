// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MAGE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 文件监听服务
/// </summary>
[ApiDescriptionSettings("FileWatcher", Description = "文件监听服务")]
public class CoalFileWatcherService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ListenerConfig> _listenerConfigRep;
    private readonly CoalDataSaveService _dataSaveService;
    private readonly ILogger<CoalFileWatcherService> _logger;
    private Dictionary<long, FileSystemWatcher> _watchers = new();

    public CoalFileWatcherService(
        SqlSugarRepository<ListenerConfig> listenerConfigRep,
        CoalDataSaveService dataSaveService,
        ILogger<CoalFileWatcherService> logger)
    {
        _listenerConfigRep = listenerConfigRep;
        _dataSaveService = dataSaveService;
        _logger = logger;
    }

    /// <summary>
    /// 启动所有监听
    /// </summary>
    [DisplayName("启动所有文件监听")]
    public async Task StartAllWatchersAsync()
    {
        var configs = await _listenerConfigRep.AsQueryable()
            .Where(c => c.Enabled == 1)
            .ToListAsync();

        foreach (var config in configs)
        {
            await StartWatcherAsync(config);
        }

        _logger.LogInformation("已启动 {Count} 个文件监听", configs.Count);
    }

    /// <summary>
    /// 启动单个监听
    /// </summary>
    [DisplayName("启动文件监听")]
    public async Task StartWatcherAsync(long configId)
    {
        var config = await _listenerConfigRep.AsQueryable()
            .Where(c => c.Id == configId)
            .FirstAsync();

        if (config == null)
        {
            throw new Exception($"监听配置不存在: {configId}");
        }

        await StartWatcherAsync(config);
    }

    /// <summary>
    /// 启动监听（内部方法）
    /// </summary>
    private async Task StartWatcherAsync(ListenerConfig config)
    {
        if (_watchers.ContainsKey(config.Id))
        {
            _logger.LogWarning("监听已存在: {ListenPath}", config.ListenPath);
            return;
        }

        if (!Directory.Exists(config.ListenPath))
        {
            _logger.LogError("监听目录不存在: {ListenPath}", config.ListenPath);
            return;
        }

        var watcher = new FileSystemWatcher(config.ListenPath)
        {
            Filter = config.FilePattern ?? "*.*",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

        watcher.Created += async (sender, e) =>
        {
            _logger.LogInformation("检测到新文件: {FileName}", e.Name);
            // 等待文件写入完成
            await Task.Delay(1000);
            await ProcessFileAsync(e.FullPath);
        };

        _watchers[config.Id] = watcher;
        _logger.LogInformation("已启动监听: {ListenPath}", config.ListenPath);
    }

    /// <summary>
    /// 停止监听
    /// </summary>
    [DisplayName("停止文件监听")]
    public async Task StopWatcherAsync(long configId)
    {
        if (_watchers.TryGetValue(configId, out var watcher))
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            _watchers.Remove(configId);
            _logger.LogInformation("已停止监听: {ConfigId}", configId);
        }
    }

    /// <summary>
    /// 停止所有监听
    /// </summary>
    [DisplayName("停止所有文件监听")]
    public Task StopAllWatchersAsync()
    {
        foreach (var watcher in _watchers.Values)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        _watchers.Clear();
        _logger.LogInformation("已停止所有文件监听");
        return Task.CompletedTask;
    }

    /// <summary>
    /// 处理单个文件
    /// </summary>
    [DisplayName("处理文件")]
    public async Task<ParseResult> ProcessFileAsync(string filePath)
    {
        try
        {
            _logger.LogInformation("开始处理文件: {FilePath}", filePath);
            var result = await _dataSaveService.ParseAndSaveAsync(filePath);
            
            if (result.Success)
            {
                _logger.LogInformation("文件处理成功: {FilePath}, 记录数: {Count}", filePath, result.RecordCount);
            }
            else
            {
                _logger.LogError("文件处理失败: {FilePath}, 错误: {Error}", filePath, result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理文件异常: {FilePath}", filePath);
            return new ParseResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    /// <summary>
    /// 批量处理目录
    /// </summary>
    [DisplayName("批量处理目录")]
    public async Task<int> ProcessDirectoryAsync(string directoryPath, string filePattern = "*.txt")
    {
        if (!Directory.Exists(directoryPath))
        {
            _logger.LogError("目录不存在: {DirectoryPath}", directoryPath);
            return 0;
        }

        var files = Directory.GetFiles(directoryPath, filePattern);
        var successCount = 0;

        foreach (var file in files)
        {
            try
            {
                var result = await ProcessFileAsync(file);
                if (result.Success) successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理文件失败: {File}", file);
            }
        }

        _logger.LogInformation("批量处理完成: 成功 {Success}/{Total}", successCount, files.Length);
        return successCount;
    }

    /// <summary>
    /// 获取监听状态
    /// </summary>
    [DisplayName("获取监听状态")]
    public Dictionary<long, bool> GetWatcherStatus()
    {
        return _watchers.ToDictionary(w => w.Key, w => w.Value.EnableRaisingEvents);
    }
}

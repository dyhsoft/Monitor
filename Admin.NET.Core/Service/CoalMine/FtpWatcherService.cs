// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

using FluentFTP;

namespace Admin.NET.Core.Service;

/// <summary>
/// FTP监听服务
/// </summary>
[ApiDescriptionSettings("FtpWatcher", Description = "FTP监听服务")]
public class FtpWatcherService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<FtpConfig> _ftpConfigRep;
    private readonly CoalDataSaveService _dataSaveService;
    private readonly ILogger<FtpWatcherService> _logger;
    private readonly Dictionary<long, FtpClient> _ftpClients = new();
    private readonly Dictionary<long, Timer> _timers = new();

    public FtpWatcherService(
        SqlSugarRepository<FtpConfig> ftpConfigRep,
        CoalDataSaveService dataSaveService,
        ILogger<FtpWatcherService> logger)
    {
        _ftpConfigRep = ftpConfigRep;
        _dataSaveService = dataSaveService;
        _logger = logger;
    }

    /// <summary>
    /// 启动FTP监听
    /// </summary>
    [DisplayName("启动FTP监听")]
    public async Task StartFtpWatcherAsync(long configId)
    {
        var config = await _ftpConfigRep.AsQueryable()
            .Where(c => c.Id == configId && c.Enabled == 1)
            .FirstAsync();

        if (config == null)
        {
            throw new Exception($"FTP配置不存在或未启用: {configId}");
        }

        await StartFtpClientAsync(config);
    }

    /// <summary>
    /// 启动所有启用的FTP监听
    /// </summary>
    [DisplayName("启动所有FTP监听")]
    public async Task StartAllFtpWatchersAsync()
    {
        var configs = await _ftpConfigRep.AsQueryable()
            .Where(c => c.Enabled == 1)
            .ToListAsync();

        foreach (var config in configs)
        {
            try
            {
                await StartFtpClientAsync(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动FTP监听失败: {ConfigId}", config.Id);
            }
        }
    }

    /// <summary>
    /// 启动FTP客户端
    /// </summary>
    private Task StartFtpClientAsync(FtpConfig config)
    {
        if (_ftpClients.ContainsKey(config.Id))
        {
            _logger.LogWarning("FTP客户端已存在: {ConfigId}", config.Id);
            return Task.CompletedTask;
        }

        // 创建临时目录用于下载文件
        var tempPath = Path.Combine(Path.GetTempPath(), "CoalMineFtp", config.Id.ToString());
        if (!Directory.Exists(tempPath))
        {
            Directory.CreateDirectory(tempPath);
        }

        // 创建FTP客户端
        var client = new FtpClient
        {
            Host = config.Host,
            Port = config.Port,
            Credentials = new System.Net.NetworkCredential(config.Username, config.Password)
        };
        client.AutoConnect();

        _ftpClients[config.Id] = client;

        // 创建定时器，每5分钟同步一次
        var timer = new Timer(async _ => await SyncFtpFilesAsync(config), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        _timers[config.Id] = timer;

        _logger.LogInformation("已启动FTP监听: {BindSystem}", config.BindSystem);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 同步FTP文件
    /// </summary>
    private async Task SyncFtpFilesAsync(FtpConfig config)
    {
        try
        {
            if (!_ftpClients.TryGetValue(config.Id, out var client))
            {
                return;
            }

            if (!client.IsConnected)
            {
                client.AutoConnect();
            }

            var tempPath = Path.Combine(Path.GetTempPath(), "CoalMineFtp", config.Id.ToString());
            var files = client.GetListing(config.RootDirectory);

            foreach (var file in files)
            {
                if (file.Type == FtpObjectType.File)
                {
                    var localPath = Path.Combine(tempPath, file.Name);

                    // 下载文件
                    client.DownloadFile(localPath, file.FullName);

                    // 处理文件
                    var result = await _dataSaveService.ParseAndSaveAsync(localPath);

                    if (result.Success)
                    {
                        // 删除远程文件或移动到备份目录
                        client.DeleteFile(file.FullName);
                        _logger.LogInformation("FTP文件处理成功: {FileName}, 记录数: {Count}", file.Name, result.RecordCount);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FTP文件同步失败: {ConfigId}", config.Id);
        }
    }

    /// <summary>
    /// 停止FTP监听
    /// </summary>
    [DisplayName("停止FTP监听")]
    public Task StopFtpWatcherAsync(long configId)
    {
        if (_timers.TryGetValue(configId, out var timer))
        {
            timer.Dispose();
            _timers.Remove(configId);
        }

        if (_ftpClients.TryGetValue(configId, out var client))
        {
            client.Disconnect();
            client.Dispose();
            _ftpClients.Remove(configId);
        }

        _logger.LogInformation("已停止FTP监听: {ConfigId}", configId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 停止所有FTP监听
    /// </summary>
    [DisplayName("停止所有FTP监听")]
    public Task StopAllFtpWatchersAsync()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Dispose();
        }
        _timers.Clear();

        foreach (var client in _ftpClients.Values)
        {
            client.Disconnect();
            client.Dispose();
        }
        _ftpClients.Clear();

        _logger.LogInformation("已停止所有FTP监听");
        return Task.CompletedTask;
    }

    /// <summary>
    /// 手动同步一次
    /// </summary>
    [DisplayName("手动同步一次")]
    public async Task SyncOnceAsync(long configId)
    {
        var config = await _ftpConfigRep.AsQueryable()
            .Where(c => c.Id == configId)
            .FirstAsync();

        if (config == null)
        {
            throw new Exception($"FTP配置不存在: {configId}");
        }

        await SyncFtpFilesAsync(config);
    }

    /// <summary>
    /// 测试FTP连接
    /// </summary>
    [DisplayName("测试FTP连接")]
    public Task<FtpTestResult> TestConnectionAsync(TestFtpConnectionInput input)
    {
        try
        {
            using var client = new FtpClient
            {
                Host = input.Host,
                Port = input.Port,
                Credentials = new System.Net.NetworkCredential(input.Username, input.Password)
            };
            client.AutoConnect();

            var isConnected = client.IsConnected;

            return Task.FromResult(new FtpTestResult
            {
                Success = true,
                Message = "连接成功",
                IsConnected = isConnected
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new FtpTestResult
            {
                Success = false,
                Message = $"连接失败: {ex.Message}",
                IsConnected = false
            });
        }
    }

    /// <summary>
    /// 获取FTP监听状态
    /// </summary>
    [DisplayName("获取FTP监听状态")]
    public Dictionary<long, FtpWatcherStatus> GetFtpWatcherStatus()
    {
        var status = new Dictionary<long, FtpWatcherStatus>();

        foreach (var kvp in _ftpClients)
        {
            var client = kvp.Value;
            status[kvp.Key] = new FtpWatcherStatus
            {
                ConfigId = kvp.Key,
                IsConnected = client.IsConnected,
                Status = client.IsConnected ? "运行中" : "已断开"
            };
        }

        return status;
    }
}

/// <summary>
/// 测试FTP连接输入
/// </summary>
public class TestFtpConnectionInput
{
    /// <summary>
    /// 主机地址
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; } = 21;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
}

/// <summary>
/// FTP测试结果
/// </summary>
public class FtpTestResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否已连接
    /// </summary>
    public bool IsConnected { get; set; }
}

/// <summary>
/// FTP监听状态
/// </summary>
public class FtpWatcherStatus
{
    /// <summary>
    /// 配置ID
    /// </summary>
    public long ConfigId { get; set; }

    /// <summary>
    /// 是否连接
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; }
}

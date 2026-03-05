using Admin.NET.Application.CoalMine.CoalDataAccess;
using Admin.NET.Core;
using Admin.NET.EntityFramework.Core;
using FluentFTP;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// FTP监听服务 - 监听FTP目录
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "FtpWatcher", Order = 100)]
public class FtpWatcherService : IFtpWatcherService, ITransient
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger<FtpWatcherService> _logger;
    private CancellationTokenSource _cancellationTokenSource;
    private Task _monitorTask;
    private bool _isRunning;

    // FTP配置
    private string _ftpHost = "";
    private int _ftpPort = 21;
    private string _ftpUsername = "";
    private string _ftpPassword = "";
    private string _ftpRemotePath = "/";
    private string _localDownloadPath = "";
    private string _backupPath = "";
    private int _pollIntervalSeconds = 10;

    public FtpWatcherService(ISqlSugarClient db)
    {
        _db = db;
        _logger = App.GetService<ILogger<FtpWatcherService>>();

        // 从配置读取FTP设置
        _ftpHost = App.Settings.GetValue<string>("CoalMine:Ftp:Host") ?? "127.0.0.1";
        _ftpPort = App.Settings.GetValue<int>("CoalMine:Ftp:Port", 21);
        _ftpUsername = App.Settings.GetValue<string>("CoalMine:Ftp:Username") ?? "ftpuser";
        _ftpPassword = App.Settings.GetValue<string>("CoalMine:Ftp:Password") ?? "";
        _ftpRemotePath = App.Settings.GetValue<string>("CoalMine:Ftp:RemotePath") ?? "/";
        _localDownloadPath = App.Settings.GetValue<string>("CoalMine:Ftp:DownloadPath") ?? @"C:\CoalMineData\FtpDownload";
        _backupPath = App.Settings.GetValue<string>("CoalMine:Ftp:BackupPath") ?? @"C:\CoalMineData\FtpBackup";
        _pollIntervalSeconds = App.Settings.GetValue<int>("CoalMine:Ftp:PollInterval", 10);

        // 确保目录存在
        if (!Directory.Exists(_localDownloadPath))
            Directory.CreateDirectory(_localDownloadPath);
        if (!Directory.Exists(_backupPath))
            Directory.CreateDirectory(_backupPath);
    }

    /// <summary>
    /// 启动FTP监听
    /// </summary>
    [HttpPost]
    public async Task<string> Start()
    {
        if (_isRunning)
        {
            return "FTP监听已在运行";
        }

        try
        {
            // 测试FTP连接
            using var ftpClient = CreateFtpClient();
            await ftpClient.ConnectAsync();

            _cancellationTokenSource = new CancellationTokenSource();
            _monitorTask = Task.Run(() => MonitorFtpDirectory(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

            _isRunning = true;
            _logger.LogInformation($"FTP监听服务已启动，FTP地址: {_ftpHost}:{_ftpPort}{_ftpRemotePath}");

            return "FTP监听启动成功";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动FTP监听失败");
            return $"启动失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 停止FTP监听
    /// </summary>
    [HttpPost]
    public async Task<string> Stop()
    {
        if (!_isRunning)
        {
            return "FTP监听未运行";
        }

        try
        {
            _cancellationTokenSource?.Cancel();
            await (_monitorTask ?? Task.CompletedTask);

            _isRunning = false;
            _logger.LogInformation("FTP监听服务已停止");

            return "FTP监听已停止";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止FTP监听失败");
            return $"停止失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 获取监听状态
    /// </summary>
    public async Task<Dictionary<string, object>> GetStatus()
    {
        return new Dictionary<string, object>
        {
            { "isRunning", _isRunning },
            { "ftpHost", _ftpHost },
            { "ftpPort", _ftpPort },
            { "ftpRemotePath", _ftpRemotePath },
            { "localDownloadPath", _localDownloadPath },
            { "pollIntervalSeconds", _pollIntervalSeconds }
        };
    }

    /// <summary>
    /// 配置FTP参数
    /// </summary>
    [HttpPost]
    public async Task<string> Configure(FtpConfigInput input)
    {
        _ftpHost = input.Host;
        _ftpPort = input.Port;
        _ftpUsername = input.Username;
        _ftpPassword = input.Password;
        _ftpRemotePath = input.RemotePath;
        _pollIntervalSeconds = input.PollInterval;

        return "FTP配置已更新，重启监听后生效";
    }

    /// <summary>
    /// 测试FTP连接
    /// </summary>
    [HttpPost]
    public async Task<bool> TestConnection(FtpConfigInput input)
    {
        try
        {
            using var client = new FtpClient(input.Host, input.Port, input.Username, input.Password);
            client.ConnectTimeout = 5000;
            await client.ConnectAsync();
            return client.IsConnected;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 手动同步一次
    /// </summary>
    [HttpPost]
    public async Task<Dictionary<string, object>> SyncOnce()
    {
        var result = new Dictionary<string, object>
        {
            { "success", false },
            { "filesDownloaded", 0 },
            { "filesProcessed", 0 },
            { "errors", "" }
        };

        try
        {
            using var ftpClient = CreateFtpClient();
            await ftpClient.ConnectAsync();

            // 获取远程文件列表
            var files = await ftpClient.GetFileList(_ftpRemotePath, false);
            var txtFiles = files.Where(f => f.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToList();

            result["filesDownloaded"] = txtFiles.Count;

            foreach (var file in txtFiles)
            {
                try
                {
                    var localPath = Path.Combine(_localDownloadPath, file.Name);
                    var remotePath = $"{_ftpRemotePath}/{file.Name}";

                    // 下载文件
                    await ftpClient.DownloadFileAsync(localPath, remotePath);

                    // 处理文件（调用文件处理服务）
                    var fileWatcherService = App.GetService<IFileWatcherService>();
                    var processResult = await fileWatcherService.ProcessFile(localPath);

                    // 删除远程文件
                    await ftpClient.DeleteFile(remotePath);

                    // 备份到本地
                    var backupPath = Path.Combine(_backupPath, $"{DateTime.Now:yyyyMMdd}_{file.Name}");
                    File.Move(localPath, backupPath);

                    result["filesProcessed"] = (int)result["filesProcessed"] + 1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"处理FTP文件失败: {file.Name}");
                }
            }

            result["success"] = true;
        }
        catch (Exception ex)
        {
            result["errors"] = ex.Message;
            _logger.LogError(ex, "FTP同步失败");
        }

        return result;
    }

    /// <summary>
    /// 监控FTP目录
    /// </summary>
    private async Task MonitorFtpDirectory(CancellationToken cancellationToken)
    {
        var processedFiles = new HashSet<string>();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var ftpClient = CreateFtpClient();
                await ftpClient.ConnectAsync();

                // 获取文件列表
                var files = await ftpClient.GetFileList(_ftpRemotePath, false);
                var txtFiles = files.Where(f => f.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var file in txtFiles)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    if (processedFiles.Contains(file.Name)) continue;

                    try
                    {
                        var localPath = Path.Combine(_localDownloadPath, file.Name);
                        var remotePath = $"{_ftpRemotePath}/{file.Name}";

                        // 下载文件
                        await ftpClient.DownloadFileAsync(localPath, remotePath);

                        // 处理文件
                        var fileWatcherService = App.GetService<IFileWatcherService>();
                        await fileWatcherService.ProcessFile(localPath);

                        // 删除远程文件
                        await ftpClient.DeleteFile(remotePath);

                        processedFiles.Add(file.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"处理FTP文件失败: {file.Name}");
                    }
                }

                // 定期清理已处理文件记录
                if (processedFiles.Count > 1000)
                {
                    processedFiles.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FTP监控异常");
            }

            // 等待下一次轮询
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_pollIntervalSeconds), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 创建FTP客户端
    /// </summary>
    private FtpClient CreateFtpClient()
    {
        var client = new FtpClient(_ftpHost, _ftpPort, _ftpUsername, _ftpPassword)
        {
            ConnectTimeout = 10000,
            ReadTimeout = 30000,
            DataConnectionConnectTimeout = 10000,
            DataConnectionReadTimeout = 30000
        };
        return client;
    }
}

/// <summary>
/// FTP配置输入
/// </summary>
public class FtpConfigInput
{
    /// <summary>
    /// FTP主机
    /// </summary>
    public string Host { get; set; } = "";

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; } = 21;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = "";

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = "";

    /// <summary>
    /// 远程目录
    /// </summary>
    public string RemotePath { get; set; } = "/";

    /// <summary>
    /// 轮询间隔（秒）
    /// </summary>
    public int PollInterval { get; set; } = 10;
}

/// <summary>
/// FTP监听服务接口
/// </summary>
public interface IFtpWatcherService
{
    Task<string> Start();
    Task<string> Stop();
    Task<Dictionary<string, object>> GetStatus();
    Task<string> Configure(FtpConfigInput input);
    Task<bool> TestConnection(FtpConfigInput input);
    Task<Dictionary<string, object>> SyncOnce();
}

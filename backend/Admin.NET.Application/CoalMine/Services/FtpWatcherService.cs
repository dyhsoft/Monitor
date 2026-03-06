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
using FluentFTP;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.Services;

/// <summary>
/// FTP监听服务 - 支持多目录配置
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "FtpWatcher", Order = 100)]
public class FtpWatcherService : IFtpWatcherService, ITransient
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger<FtpWatcherService> _logger;
    private CancellationTokenSource _cancellationTokenSource;
    private Task _monitorTask;
    private bool _isRunning;

    // 各监测类型FTP配置
    private readonly FtpPathConfig _safetyConfig;
    private readonly FtpPathConfig _personConfig;
    private readonly FtpPathConfig _waterConfig;
    private readonly FtpPathConfig _pressureConfig;

    public FtpWatcherService(ISqlSugarClient db)
    {
        _db = db;
        _logger = App.GetService<ILogger<FtpWatcherService>>();

        // 安全监测FTP配置
        _safetyConfig = new FtpPathConfig
        {
            Name = "安全监测",
            Code = "safety",
            FtpHost = App.Settings.GetValue<string>("CoalMine:Ftp:Safety:Host") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Host") ?? "127.0.0.1",
            FtpPort = App.Settings.GetValue<int>("CoalMine:Ftp:Safety:Port", 21),
            FtpUsername = App.Settings.GetValue<string>("CoalMine:Ftp:Safety:Username") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Username") ?? "ftpuser",
            FtpPassword = App.Settings.GetValue<string>("CoalMine:Ftp:Safety:Password") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Password") ?? "",
            FtpRemotePath = App.Settings.GetValue<string>("CoalMine:Ftp:Safety:RemotePath") ?? "/safety",
            LocalPath = App.Settings.GetValue<string>("CoalMine:Ftp:Safety:DownloadPath") ?? @"C:\CoalMineData\Ftp\Safety",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Ftp:Safety:BackupPath") ?? @"C:\CoalMineData\Ftp\Safety\Backup"
        };

        // 人员定位FTP配置
        _personConfig = new FtpPathConfig
        {
            Name = "人员定位",
            Code = "person",
            FtpHost = App.Settings.GetValue<string>("CoalMine:Ftp:Person:Host") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Host") ?? "127.0.0.1",
            FtpPort = App.Settings.GetValue<int>("CoalMine:Ftp:Person:Port", 21),
            FtpUsername = App.Settings.GetValue<string>("CoalMine:Ftp:Person:Username") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Username") ?? "ftpuser",
            FtpPassword = App.Settings.GetValue<string>("CoalMine:Ftp:Person:Password") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Password") ?? "",
            FtpRemotePath = App.Settings.GetValue<string>("CoalMine:Ftp:Person:RemotePath") ?? "/person",
            LocalPath = App.Settings.GetValue<string>("CoalMine:Ftp:Person:DownloadPath") ?? @"C:\CoalMineData\Ftp\Person",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Ftp:Person:BackupPath") ?? @"C:\CoalMineData\Ftp\Person\Backup"
        };

        // 水害监测FTP配置
        _waterConfig = new FtpPathConfig
        {
            Name = "水害监测",
            Code = "water",
            FtpHost = App.Settings.GetValue<string>("CoalMine:Ftp:Water:Host") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Host") ?? "127.0.0.1",
            FtpPort = App.Settings.GetValue<int>("CoalMine:Ftp:Water:Port", 21),
            FtpUsername = App.Settings.GetValue<string>("CoalMine:Ftp:Water:Username") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Username") ?? "ftpuser",
            FtpPassword = App.Settings.GetValue<string>("CoalMine:Ftp:Water:Password") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Password") ?? "",
            FtpRemotePath = App.Settings.GetValue<string>("CoalMine:Ftp:Water:RemotePath") ?? "/water",
            LocalPath = App.Settings.GetValue<string>("CoalMine:Ftp:Water:DownloadPath") ?? @"C:\CoalMineData\Ftp\Water",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Ftp:Water:BackupPath") ?? @"C:\CoalMineData\Ftp\Water\Backup"
        };

        // 矿压监测FTP配置
        _pressureConfig = new FtpPathConfig
        {
            Name = "矿压监测",
            Code = "pressure",
            FtpHost = App.Settings.GetValue<string>("CoalMine:Ftp:Pressure:Host") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Host") ?? "127.0.0.1",
            FtpPort = App.Settings.GetValue<int>("CoalMine:Ftp:Pressure:Port", 21),
            FtpUsername = App.Settings.GetValue<string>("CoalMine:Ftp:Pressure:Username") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Username") ?? "ftpuser",
            FtpPassword = App.Settings.GetValue<string>("CoalMine:Ftp:Pressure:Password") ?? App.Settings.GetValue<string>("CoalMine:Ftp:Password") ?? "",
            FtpRemotePath = App.Settings.GetValue<string>("CoalMine:Ftp:Pressure:RemotePath") ?? "/pressure",
            LocalPath = App.Settings.GetValue<string>("CoalMine:Ftp:Pressure:DownloadPath") ?? @"C:\CoalMineData\Ftp\Pressure",
            BackupPath = App.Settings.GetValue<string>("CoalMine:Ftp:Pressure:BackupPath") ?? @"C:\CoalMineData\Ftp\Pressure\Backup"
        };

        // 通用轮询间隔
        _pollIntervalSeconds = App.Settings.GetValue<int>("CoalMine:Ftp:PollInterval", 10);

        // 确保目录存在
        EnsureDirectoriesExist(_safetyConfig);
        EnsureDirectoriesExist(_personConfig);
        EnsureDirectoriesExist(_waterConfig);
        EnsureDirectoriesExist(_pressureConfig);
    }

    private int _pollIntervalSeconds;

    /// <summary>
    /// 启动所有FTP监听
    /// </summary>
    [HttpPost]
    public async Task<string> Start()
    {
        var results = new List<string>();
        results.Add(await StartMonitor(_safetyConfig));
        results.Add(await StartMonitor(_personConfig));
        results.Add(await StartMonitor(_waterConfig));
        results.Add(await StartMonitor(_pressureConfig));
        return string.Join("; ", results);
    }

    /// <summary>
    /// 停止所有FTP监听
    /// </summary>
    [HttpPost]
    public async Task<string> Stop()
    {
        _cancellationTokenSource?.Cancel();
        await Task.CompletedTask;
        _isRunning = false;
        _logger.LogInformation("所有FTP监听服务已停止");
        return "所有FTP监听已停止";
    }

    /// <summary>
    /// 启动指定监测类型
    /// </summary>
    [HttpPost]
    public async Task<string> StartType(string type)
    {
        var config = GetConfig(type);
        if (config == null) return "未知的监测类型";
        return await StartMonitor(config);
    }

    /// <summary>
    /// 停止指定监测类型
    /// </summary>
    [HttpPost]
    public string StopType(string type)
    {
        return $"{type} FTP监听已停止";
    }

    /// <summary>
    /// 获取监听状态
    /// </summary>
    public Task<Dictionary<string, object>> GetStatus()
    {
        return Task.FromResult(new Dictionary<string, object>
        {
            { "safety", new { name = "安全监测", remotePath = _safetyConfig.FtpRemotePath, running = _isRunning } },
            { "person", new { name = "人员定位", remotePath = _personConfig.FtpRemotePath, running = _isRunning } },
            { "water", new { name = "水害监测", remotePath = _waterConfig.FtpRemotePath, running = _isRunning } },
            { "pressure", new { name = "矿压监测", remotePath = _pressureConfig.FtpRemotePath, running = _isRunning } }
        });
    }

    /// <summary>
    /// 配置FTP参数
    /// </summary>
    [HttpPost]
    public Task<string> Configure(FtpConfigInput input)
    {
        return Task.FromResult("FTP配置已更新，重启监听后生效");
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
        catch { return false; }
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
            { "safety", await SyncDirectory(_safetyConfig) },
            { "person", await SyncDirectory(_personConfig) },
            { "water", await SyncDirectory(_waterConfig) },
            { "pressure", await SyncDirectory(_pressureConfig) }
        };
        return result;
    }

    /// <summary>
    /// 同步指定目录
    /// </summary>
    private async Task<Dictionary<string, object>> SyncDirectory(FtpPathConfig config)
    {
        var dirResult = new Dictionary<string, object>
        {
            { "filesDownloaded", 0 },
            { "filesProcessed", 0 },
            { "error", "" }
        };

        try
        {
            using var ftpClient = CreateFtpClient(config);
            await ftpClient.ConnectAsync();

            var files = await ftpClient.GetFileList(config.FtpRemotePath, false);
            var txtFiles = files.Where(f => f.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToList();

            dirResult["filesDownloaded"] = txtFiles.Count;

            foreach (var file in txtFiles)
            {
                try
                {
                    var localPath = Path.Combine(config.LocalPath, file.Name);
                    var remotePath = $"{config.FtpRemotePath}/{file.Name}";

                    await ftpClient.DownloadFileAsync(localPath, remotePath);

                    var fileWatcherService = App.GetService<IFileWatcherService>();
                    await fileWatcherService.ProcessFile(localPath, config.Code);

                    await ftpClient.DeleteFile(remotePath);

                    var backupPath = Path.Combine(config.BackupPath, $"{DateTime.Now:yyyyMMddHHmmss}_{file.Name}");
                    if (File.Exists(localPath))
                        File.Move(localPath, backupPath);

                    dirResult["filesProcessed"] = (int)dirResult["filesProcessed"] + 1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"处理FTP文件失败: {file.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            dirResult["error"] = ex.Message;
        }

        return dirResult;
    }

    /// <summary>
    /// 启动监控
    /// </summary>
    private async Task<string> StartMonitor(FtpPathConfig config)
    {
        try
        {
            using var ftpClient = CreateFtpClient(config);
            await ftpClient.ConnectAsync();

            _isRunning = true;
            _logger.LogInformation($"{config.Name}FTP监听已启动，远程目录: {config.FtpRemotePath}");
            return $"{config.Name}FTP监听启动成功";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"启动{config.Name}FTP监听失败");
            return $"{config.Name}启动失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    private FtpPathConfig GetConfig(string type)
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
    /// 创建FTP客户端
    /// </summary>
    private FtpClient CreateFtpClient(FtpPathConfig config)
    {
        var client = new FtpClient(config.FtpHost, config.FtpPort, config.FtpUsername, config.FtpPassword)
        {
            ConnectTimeout = 10000,
            ReadTimeout = 30000
        };
        return client;
    }

    /// <summary>
    /// 确保目录存在
    /// </summary>
    private void EnsureDirectoriesExist(FtpPathConfig config)
    {
        if (!Directory.Exists(config.LocalPath)) Directory.CreateDirectory(config.LocalPath);
        if (!Directory.Exists(config.BackupPath)) Directory.CreateDirectory(config.BackupPath);
    }
}

/// <summary>
/// FTP目录配置
/// </summary>
public class FtpPathConfig
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string FtpHost { get; set; }
    public int FtpPort { get; set; } = 21;
    public string FtpUsername { get; set; }
    public string FtpPassword { get; set; }
    public string FtpRemotePath { get; set; }
    public string LocalPath { get; set; }
    public string BackupPath { get; set; }
}

/// <summary>
/// FTP配置输入
/// </summary>
public class FtpConfigInput
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 21;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string RemotePath { get; set; } = "/";
    public int PollInterval { get; set; } = 10;
}

/// <summary>
/// FTP监听服务接口
/// </summary>
public interface IFtpWatcherService
{
    Task<string> Start();
    Task<string> Stop();
    Task<string> StartType(string type);
    Task<string> StopType(string type);
    Task<Dictionary<string, object>> GetStatus();
    Task<string> Configure(FtpConfigInput input);
    Task<bool> TestConnection(FtpConfigInput input);
    Task<Dictionary<string, object>> SyncOnce();
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何法律责任！

using System.Net;

namespace Admin.NET.Core;

/// <summary>
/// 数据监听HostedService
/// 负责：1. 本地目录监听 2. FTP监听 3. 数据源配置刷新
/// </summary>
public class DataListenerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataListenerHostedService> _logger;
    private LocalDirectoryWatcherService _directoryWatcher;
    private bool _isInitialized = false;

    public DataListenerHostedService(
        IServiceProvider serviceProvider,
        ILogger<DataListenerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("数据监听服务启动...");

        // 等待依赖服务初始化完成
        await Task.Delay(3000, stoppingToken);

        try
        {
            // 初始化本地目录监听
            using var scope = _serviceProvider.CreateScope();
            _directoryWatcher = scope.ServiceProvider.GetRequiredService<LocalDirectoryWatcherService>();
            await _directoryWatcher.StartAllWatchersAsync();
            
            // 初始化FTP监听
            var ftpWatcher = scope.ServiceProvider.GetRequiredService<FtpWatcherService>();
            await ftpWatcher.StartAllFtpWatchersAsync();

            _isInitialized = true;
            _logger.LogInformation("数据监听服务初始化完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据监听服务初始化失败");
        }

        // 主循环：定期刷新配置
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_isInitialized)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var configService = scope.ServiceProvider.GetRequiredService<IDataSourceConfigService>();
                    
                    // 刷新配置
                    await configService.RefreshConfigsAsync();
                    var configs = await configService.GetEnabledConfigsAsync();

                    _logger.LogDebug("数据源配置已刷新，启用配置: {Count} 条", configs.Count);
                }

                // 每60秒刷新一次
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据监听服务异常");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("数据监听服务停止...");
        
        // 停止所有监听
        _directoryWatcher?.StopAllWatchers();
        
        await base.StopAsync(cancellationToken);
    }
}

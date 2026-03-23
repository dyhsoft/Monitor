// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
using System.Net;

namespace Admin.NET.Core;

/// <summary>
/// 数据监听HostedService
/// 负责：1. 定期刷新数据源配置 2. TCP/UDP端口监听(预留)
/// </summary>
public class DataListenerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataListenerHostedService> _logger;

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

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<IDataSourceConfigService>();
                
                // 刷新配置
                await configService.RefreshConfigsAsync();
                var configs = await configService.GetEnabledConfigsAsync();

                _logger.LogInformation("数据源配置已刷新，启用配置: {Count} 条", configs.Count);

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
        await base.StopAsync(cancellationToken);
    }
}

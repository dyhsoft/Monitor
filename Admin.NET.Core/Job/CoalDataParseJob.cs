// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Furion.Logging.Extensions;

namespace Admin.NET.Core;

/// <summary>
/// 煤矿数据定时解析作业
/// 根据 DB14_1226 安全监测与定位联网上传规范
/// 定时扫描指定目录，解析数据文件并保存到数据库
/// </summary>
[JobDetail("job_coalDataParse", Description = "煤矿数据定时解析", GroupName = "CoalMine", Concurrent = false)]
[PeriodSeconds(300, TriggerId = "trigger_coalDataParse", Description = "煤矿数据定时解析(每5分钟)", MaxNumberOfRuns = 0, RunOnStart = true)]
public class CoalDataParseJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public CoalDataParseJob(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = loggerFactory.CreateLogger(CommonConst.SysLogCategoryName);
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();

        try
        {
            // 获取监听配置服务
            var listenerConfigRep = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<ListenerConfig>>();
            var coalDataSaveService = serviceScope.ServiceProvider.GetRequiredService<CoalDataSaveService>();

            // 获取所有启用的监听配置
            List<ListenerConfig> configs;
            try
            {
                configs = await listenerConfigRep.AsQueryable()
                    .Where(c => c.Enabled == 1)
                    .ToListAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "获取监听配置失败");
                configs = new List<ListenerConfig>();
            }

            var successCount = 0;
            var failCount = 0;

            foreach (var config in configs)
            {
                try
                {
                    if (!Directory.Exists(config.ListenPath))
                    {
                        _logger.LogWarning("监听目录不存在: {ListenPath}", config.ListenPath);
                        continue;
                    }

                    // 获取匹配的文件
                    var files = Directory.GetFiles(config.ListenPath, config.FilePattern ?? "*.txt");
                    
                    foreach (var file in files)
                    {
                        try
                        {
                            // 检查文件是否被占用（正在写入）
                            if (IsFileLocked(file))
                            {
                                continue;
                            }

                            var result = await coalDataSaveService.ParseAndSaveAsync(file);
                            if (result.Success)
                            {
                                successCount++;
                                // 解析成功后移动或删除文件（可选）
                                // MoveProcessedFile(file);
                            }
                            else
                            {
                                failCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "处理文件失败: {File}", file);
                            failCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "处理监听配置失败: {ConfigId}", config.Id);
                    failCount++;
                }
            }

            if (configs.Count > 0)
            {
                _logger.LogInformation("【{Time}】煤矿数据解析完成: 成功 {Success} 个, 失败 {Fail} 个", 
                    DateTime.Now, successCount, failCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "煤矿数据解析作业执行异常");
        }
    }

    /// <summary>
    /// 检查文件是否被锁定
    /// </summary>
    private bool IsFileLocked(string filePath)
    {
        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }
}

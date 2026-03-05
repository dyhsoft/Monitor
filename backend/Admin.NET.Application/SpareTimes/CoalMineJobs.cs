using Admin.NET.Core;
using Admin.NET.EntityFramework.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Furion.TaskScheduler;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 人员定位数据采集定时任务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "PersonLocationJob", Order = 100)]
public class PersonLocationCollectJob : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public PersonLocationCollectJob(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 启动人员定位数据采集任务
    /// </summary>
    /// <param name="interval">采集间隔(秒)</param>
    /// <param name="jobName">任务名称</param>
    /// <returns></returns>
    public string Start(int interval = 60, string jobName = "PersonLocationCollect")
    {
        Console.WriteLine($"启动人员定位数据采集任务，间隔: {interval}秒");

        SpareTime.Do(interval * 1000, (timer, count) =>
        {
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                CollectPersonLocationData(db);
            });
        }, jobName, "人员定位数据采集任务");

        return $"任务 {jobName} 已启动，采集间隔: {interval}秒";
    }

    /// <summary>
    /// 停止任务
    /// </summary>
    public string Stop(string jobName = "PersonLocationCollect")
    {
        SpareTime.Stop(jobName);
        return $"任务 {jobName} 已停止";
    }

    /// <summary>
    /// 执行一次采集
    /// </summary>
    public async Task<int> ExecuteOnce()
    {
        return await Task.Run(() =>
        {
            int count = 0;
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                count = CollectPersonLocationData(db);
            });
            return count;
        });
    }

    /// <summary>
    /// 采集人员定位数据
    /// </summary>
    private int CollectPersonLocationData(ISqlSugarClient db)
    {
        int count = 0;
        try
        {
            // 获取所有启用的网关配置
            var gateways = db.Queryable<CoalGatewayConfig>()
                .Where(g => g.Status == 1)
                .ToList();

            foreach (var gateway in gateways)
            {
                try
                {
                    // 获取人员定位实时数据文件 (RYSS)
                    var files = FileHelper.GetDataFiles(gateway, "RYSS");
                    foreach (var file in files)
                    {
                        var records = CoalDataParser.ParseRYSS(file.Content);
                        foreach (var record in records)
                        {
                            record.MineId = gateway.MineId;
                            record.UpdateTime = DateTime.Now;

                            // 存在则更新，不存在则插入
                            var existing = db.Queryable<PersonLocation>()
                                .Where(p => p.MineId == gateway.MineId && p.CardId == record.CardId)
                                .First();

                            if (existing != null)
                            {
                                existing.StationId = record.StationId;
                                existing.StationName = record.StationName;
                                existing.AreaCode = record.AreaCode;
                                existing.AreaName = record.AreaName;
                                existing.X = record.X;
                                existing.Y = record.Y;
                                existing.Z = record.Z;
                                existing.UpdateTime = DateTime.Now;
                                db.Updateable(existing).ExecuteCommand();
                            }
                            else
                            {
                                record.InTime = DateTime.Now;
                                db.Insertable(record).ExecuteReturnIdentity();
                            }
                            count++;
                        }

                        // 标记文件已处理
                        FileHelper.MarkFileProcessed(file.FilePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"采集人员定位数据失败: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"人员定位数据采集任务异常: {ex.Message}");
        }

        Console.WriteLine($"人员定位数据采集完成，共处理 {count} 条记录");
        return count;
    }
}

/// <summary>
/// 水害监测数据采集定时任务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "WaterMonitorJob", Order = 100)]
public class WaterMonitorCollectJob : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public WaterMonitorCollectJob(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 启动水害监测数据采集任务
    /// </summary>
    public string Start(int interval = 60, string jobName = "WaterMonitorCollect")
    {
        Console.WriteLine($"启动水害监测数据采集任务，间隔: {interval}秒");

        SpareTime.Do(interval * 1000, (timer, count) =>
        {
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                CollectWaterMonitorData(db);
            });
        }, jobName, "水害监测数据采集任务");

        return $"任务 {jobName} 已启动，采集间隔: {interval}秒";
    }

    /// <summary>
    /// 停止任务
    /// </summary>
    public string Stop(string jobName = "WaterMonitorCollect")
    {
        SpareTime.Stop(jobName);
        return $"任务 {jobName} 已停止";
    }

    /// <summary>
    /// 执行一次采集
    /// </summary>
    public async Task<int> ExecuteOnce()
    {
        return await Task.Run(() =>
        {
            int count = 0;
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                count = CollectWaterMonitorData(db);
            });
            return count;
        });
    }

    /// <summary>
    /// 采集水害监测数据
    /// </summary>
    private int CollectWaterMonitorData(ISqlSugarClient db)
    {
        int count = 0;
        try
        {
            var gateways = db.Queryable<CoalGatewayConfig>()
                .Where(g => g.Status == 1)
                .ToList();

            foreach (var gateway in gateways)
            {
                try
                {
                    // 获取水仓水位数据 (CGKCDSS)
                    var waterLevelFiles = FileHelper.GetDataFiles(gateway, "CGKCDSS");
                    foreach (var file in waterLevelFiles)
                    {
                        var records = CoalDataParser.ParseCGKCDSS(file.Content);
                        foreach (var record in records)
                        {
                            record.MineId = gateway.MineId;
                            record.ReceivedTime = DateTime.Now;

                            var existing = db.Queryable<WaterRealtime>()
                                .Where(w => w.MineId == gateway.MineId && w.SensorCode == record.SensorCode)
                                .First();

                            if (existing != null)
                            {
                                existing.WaterLevel = record.WaterLevel;
                                existing.Status = record.Status;
                                existing.UpdateTime = record.UpdateTime;
                                existing.ReceivedTime = DateTime.Now;
                                db.Updateable(existing).ExecuteCommand();
                            }
                            else
                            {
                                db.Insertable(record).ExecuteReturnIdentity();
                            }
                            count++;
                        }
                        FileHelper.MarkFileProcessed(file.FilePath);
                    }

                    // 获取排水流量数据 (PSLCDSS)
                    var flowFiles = FileHelper.GetDataFiles(gateway, "PSLCDSS");
                    foreach (var file in flowFiles)
                    {
                        var records = CoalDataParser.ParsePSLCDSS(file.Content);
                        foreach (var record in records)
                        {
                            record.MineId = gateway.MineId;
                            record.ReceivedTime = DateTime.Now;

                            var existing = db.Queryable<WaterRealtime>()
                                .Where(w => w.MineId == gateway.MineId && w.SensorCode == record.SensorCode)
                                .First();

                            if (existing != null)
                            {
                                existing.FlowRate = record.FlowRate;
                                existing.Status = record.Status;
                                existing.UpdateTime = record.UpdateTime;
                                existing.ReceivedTime = DateTime.Now;
                                db.Updateable(existing).ExecuteCommand();
                            }
                            else
                            {
                                db.Insertable(record).ExecuteReturnIdentity();
                            }
                            count++;
                        }
                        FileHelper.MarkFileProcessed(file.FilePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"采集水害监测数据失败: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"水害监测数据采集任务异常: {ex.Message}");
        }

        Console.WriteLine($"水害监测数据采集完成，共处理 {count} 条记录");
        return count;
    }
}

/// <summary>
/// 报警检测定时任务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "AlarmCheckJob", Order = 100)]
public class AlarmCheckJob : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public AlarmCheckJob(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 启动报警检测任务
    /// </summary>
    public string Start(int interval = 30, string jobName = "AlarmCheck")
    {
        Console.WriteLine($"启动报警检测任务，间隔: {interval}秒");

        SpareTime.Do(interval * 1000, (timer, count) =>
        {
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                CheckAlarms(db);
            });
        }, jobName, "报警检测任务");

        return $"任务 {jobName} 已启动，检测间隔: {interval}秒";
    }

    /// <summary>
    /// 停止任务
    /// </summary>
    public string Stop(string jobName = "AlarmCheck")
    {
        SpareTime.Stop(jobName);
        return $"任务 {jobName} 已停止";
    }

    /// <summary>
    /// 执行一次报警检测
    /// </summary>
    public async Task<int> ExecuteOnce()
    {
        return await Task.Run(() =>
        {
            int count = 0;
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                count = CheckAlarms(db);
            });
            return count;
        });
    }

    /// <summary>
    /// 检测报警
    /// </summary>
    private int CheckAlarms(ISqlSugarClient db)
    {
        int count = 0;
        try
        {
            // 获取所有启用的报警配置
            var alarmConfigs = db.Queryable<AlarmConfig>()
                .Where(a => a.AlarmEnabled == 1)
                .ToList();

            foreach (var config in alarmConfigs)
            {
                try
                {
                    // 获取对应类型的实时数据
                    var realtimeData = db.Queryable<SafetyRealtime>()
                        .Where(s => s.MineId == config.MineId)
                        .Where(s => SqlFunc.Substring(s.SensorCode, 13, 3) == config.SensorTypeCode)
                        .ToList();

                    foreach (var data in realtimeData)
                    {
                        bool isAlarm = false;
                        string alarmType = "";

                        // 根据报警类型判断
                        switch (config.AlarmType)
                        {
                            case 1: // 超过阈值
                                if (data.Value.HasValue && config.ThresholdValue.HasValue &&
                                    data.Value > config.ThresholdValue)
                                {
                                    isAlarm = true;
                                    alarmType = "超限报警";
                                }
                                break;
                            case 2: // 低于阈值
                                if (data.Value.HasValue && config.ThresholdValue.HasValue &&
                                    data.Value < config.ThresholdValue)
                                {
                                    isAlarm = true;
                                    alarmType = "低值报警";
                                }
                                break;
                            case 3: // 异常报警
                                if (data.Status == 3) // 故障状态
                                {
                                    isAlarm = true;
                                    alarmType = "故障报警";
                                }
                                break;
                        }

                        if (isAlarm)
                        {
                            // 检查是否已存在未处理的报警
                            var existingAlarm = db.Queryable<AlarmRecord>()
                                .Where(a => a.MineId == data.MineId)
                                .Where(a => a.SensorCode == data.SensorCode)
                                .Where(a => a.Status == 0) // 未处理
                                .Where(a => a.AlarmTime >= DateTime.Now.AddMinutes(-5)) // 5分钟内
                                .First();

                            if (existingAlarm == null)
                            {
                                // 创建新报警记录
                                var alarmRecord = new AlarmRecord
                                {
                                    MineId = data.MineId,
                                    SensorCode = data.SensorCode,
                                    SensorName = data.SensorCode, // TODO: 获取传感器名称
                                    SensorTypeName = config.SensorTypeName,
                                    AlarmType = alarmType,
                                    AlarmLevel = config.AlarmLevel,
                                    AlarmValue = data.Value,
                                    ThresholdValue = config.ThresholdValue,
                                    AlarmTime = DateTime.Now,
                                    Status = 0, // 未处理
                                    Remark = $"自动检测报警 - {config.SensorTypeName}"
                                };
                                db.Insertable(alarmRecord).ExecuteReturnIdentity();
                                count++;
                                Console.WriteLine($"检测到报警: {data.SensorCode} - {alarmType}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"检测报警失败: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"报警检测任务异常: {ex.Message}");
        }

        if (count > 0)
            Console.WriteLine($"报警检测完成，共生成 {count} 条报警记录");
        return count;
    }
}

/// <summary>
/// 历史数据归档定时任务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "HistoryArchiveJob", Order = 100)]
public class HistoryArchiveJob : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public HistoryArchiveJob(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 启动历史数据归档任务
    /// </summary>
    public string Start(string cron = "0 0 * * * ?", string jobName = "HistoryArchive")
    {
        Console.WriteLine($"启动历史数据归档任务");

        SpareTime.Do(cron, (timer, count) =>
        {
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                ArchiveHistoryData(db);
            });
        }, jobName, "历史数据归档任务");

        return $"任务 {jobName} 已启动";
    }

    /// <summary>
    /// 停止任务
    /// </summary>
    public string Stop(string jobName = "HistoryArchive")
    {
        SpareTime.Stop(jobName);
        return $"任务 {jobName} 已停止";
    }

    /// <summary>
    /// 执行一次归档
    /// </summary>
    public async Task<int> ExecuteOnce()
    {
        return await Task.Run(() =>
        {
            int count = 0;
            Scoped.Create((_, scope) =>
            {
                var db = scope.ServiceProvider.GetService<ISqlSugarClient>();
                count = ArchiveHistoryData(db);
            });
            return count;
        });
    }

    /// <summary>
    /// 归档历史数据
    /// </summary>
    private int ArchiveHistoryData(ISqlSugarClient db)
    {
        int count = 0;
        try
        {
            // 归档安全监测历史数据 (保留超过24小时的实时数据)
            var cutoffTime = DateTime.Now.AddHours(-24);
            var safetyRealtime = db.Queryable<SafetyRealtime>()
                .Where(s => s.UpdateTime < cutoffTime)
                .ToList();

            foreach (var item in safetyRealtime)
            {
                var history = new SafetyHistory
                {
                    MineId = item.MineId,
                    SensorCode = item.SensorCode,
                    Value = item.Value,
                    Unit = item.Unit,
                    Status = item.Status,
                    UpdateTime = item.UpdateTime,
                    ReceivedTime = item.ReceivedTime
                };
                db.Insertable(history).ExecuteReturnIdentity();
                count++;
            }

            // 删除已归档的实时数据
            if (count > 0)
            {
                db.Deleteable<SafetyRealtime>(safetyRealtime).ExecuteCommand();
            }

            Console.WriteLine($"历史数据归档完成，共归档 {count} 条安全监测记录");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"历史数据归档任务异常: {ex.Message}");
        }

        return count;
    }
}

/// <summary>
/// 文件帮助类
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 获取数据文件列表
    /// </summary>
    public static List<DataFileInfo> GetDataFiles(CoalGatewayConfig gateway, string fileType)
    {
        var files = new List<DataFileInfo>();
        try
        {
            string searchPattern = $"{gateway.MineId}_{fileType}_*.txt";
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", gateway.MineId.ToString());

            if (Directory.Exists(basePath))
            {
                var filePaths = Directory.GetFiles(basePath, searchPattern);
                foreach (var filePath in filePaths)
                {
                    // 跳过已处理的文件
                    if (IsFileProcessed(filePath)) continue;

                    var content = File.ReadAllText(filePath, System.Text.Encoding.GetEncoding(gateway.FileEncoding ?? "UTF-8"));
                    files.Add(new DataFileInfo
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        Content = content
                    });
                }
            }

            // TODO: 从FTP获取文件
            if (!string.IsNullOrEmpty(gateway.FtpHost))
            {
                // FTP文件获取逻辑
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取数据文件失败: {ex.Message}");
        }
        return files;
    }

    /// <summary>
    /// 标记文件已处理
    /// </summary>
    public static void MarkFileProcessed(string filePath)
    {
        try
        {
            string processedPath = filePath + ".processed";
            File.Move(filePath, processedPath);
        }
        catch
        {
            // 忽略错误
        }
    }

    /// <summary>
    /// 检查文件是否已处理
    /// </summary>
    public static bool IsFileProcessed(string filePath)
    {
        return File.Exists(filePath + ".processed");
    }
}

/// <summary>
/// 数据文件信息
/// </summary>
public class DataFileInfo
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string Content { get; set; }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 水害监测服务
/// </summary>
[ApiDescriptionSettings("Water", Description = "水害监测")]
public class WaterService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<WaterData> _waterRep;

    public WaterService(SqlSugarRepository<WaterData> waterRep)
    {
        _waterRep = waterRep;
    }

    /// <summary>
    /// 分页查询实时数据
    /// </summary>
    [DisplayName("分页查询实时数据")]
    public async Task<SqlSugarPagedList<WaterDataOutput>> GetRealtimePage([FromQuery] PageWaterInput input)
    {
        return await _waterRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorType), u => u.SensorType == input.SensorType)
            .OrderByDescending(u => u.UpdateTime)
            .Select<WaterDataOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取报警列表
    /// </summary>
    [DisplayName("获取报警列表")]
    public async Task<List<WaterDataOutput>> GetAlarmList([FromQuery] long mineId)
    {
        // 获取最新的水文数据，根据传感器类型判断是否异常
        var latestData = await _waterRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .OrderByDescending(u => u.UpdateTime)
            .Select<WaterDataOutput>()
            .ToListAsync();

        // 简单处理：返回所有数据，由前端判断是否报警
        // 实际可以根据阈值配置来判断
        return latestData;
    }

    /// <summary>
    /// 查询历史数据
    /// </summary>
    [DisplayName("查询历史数据")]
    public async Task<SqlSugarPagedList<WaterDataOutput>> GetHistoryPage([FromQuery] PageWaterHistoryInput input)
    {
        return await _waterRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorType), u => u.SensorType == input.SensorType)
            .WhereIF(input.StartTime.HasValue, u => u.UpdateTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, u => u.UpdateTime <= input.EndTime)
            .OrderByDescending(u => u.UpdateTime)
            .Select<WaterDataOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取传感器类型列表
    /// </summary>
    [DisplayName("获取传感器类型列表")]
    public async Task<List<string>> GetSensorTypes()
    {
        return await _waterRep.AsQueryable()
            .Select(u => u.SensorType)
            .Distinct()
            .ToListAsync();
    }
}

/// <summary>
/// 水害监测分页输入
/// </summary>
public class PageWaterInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }
}

/// <summary>
/// 水害监测历史分页输入
/// </summary>
public class PageWaterHistoryInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 水文监测数据输出
/// </summary>
public class WaterDataOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    public string SensorName { get; set; }

    /// <summary>
    /// 类型:水位/流量/排水量
    /// </summary>
    public string SensorType { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

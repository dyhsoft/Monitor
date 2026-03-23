// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会顺序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 矿压监测服务
/// </summary>
[ApiDescriptionSettings("Pressure", Description = "矿压监测")]
public class PressureService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<PressureData> _pressureRep;

    public PressureService(SqlSugarRepository<PressureData> pressureRep)
    {
        _pressureRep = pressureRep;
    }

    /// <summary>
    /// 分页查询实时数据
    /// </summary>
    [DisplayName("分页查询实时数据")]
    public async Task<SqlSugarPagedList<PressureDataOutput>> GetRealtimePage([FromQuery] PagePressureInput input)
    {
        return await _pressureRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorType), u => u.SensorType == input.SensorType)
            .OrderByDescending(u => u.UpdateTime)
            .Select<PressureDataOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 查询历史数据
    /// </summary>
    [DisplayName("查询历史数据")]
    public async Task<SqlSugarPagedList<PressureDataOutput>> GetHistoryPage([FromQuery] PagePressureHistoryInput input)
    {
        return await _pressureRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorCode), u => u.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorType), u => u.SensorType == input.SensorType)
            .WhereIF(input.StartTime.HasValue, u => u.UpdateTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, u => u.UpdateTime <= input.EndTime)
            .OrderByDescending(u => u.UpdateTime)
            .Select<PressureDataOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取传感器类型列表
    /// </summary>
    [DisplayName("获取传感器类型列表")]
    public async Task<List<string>> GetSensorTypes()
    {
        return await _pressureRep.AsQueryable()
            .Select(u => u.SensorType)
            .Distinct()
            .ToListAsync();
    }
}

/// <summary>
/// 矿压监测分页输入
/// </summary>
public class PagePressureInput : BasePageInput
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
/// 矿压监测历史分页输入
/// </summary>
public class PagePressureHistoryInput : BasePageInput
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
/// 矿压监测数据输出
/// </summary>
public class PressureDataOutput
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
    /// 类型:压力/位移/锚杆应力
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

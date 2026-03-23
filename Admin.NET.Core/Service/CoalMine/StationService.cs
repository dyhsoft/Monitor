// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 基站状态服务
/// </summary>
[ApiDescriptionSettings("Station", Description = "基站管理")]
public class StationService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<StationStatus> _stationRep;

    public StationService(SqlSugarRepository<StationStatus> stationRep)
    {
        _stationRep = stationRep;
    }

    /// <summary>
    /// 分页查询基站状态
    /// </summary>
    [DisplayName("分页查询基站状态")]
    public async Task<SqlSugarPagedList<StationStatusOutput>> GetPage([FromQuery] PageStationInput input)
    {
        return await _stationRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.StationId), u => u.StationId.Contains(input.StationId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.StationName), u => u.StationName.Contains(input.StationName))
            .WhereIF(input.Status.HasValue, u => u.Status == input.Status)
            .OrderBy(u => u.StationId)
            .Select<StationStatusOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取基站状态列表
    /// </summary>
    [DisplayName("获取基站状态列表")]
    public async Task<List<StationStatusOutput>> GetList([FromQuery] long mineId)
    {
        return await _stationRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .OrderBy(u => u.StationId)
            .Select<StationStatusOutput>()
            .ToListAsync();
    }

    /// <summary>
    /// 获取基站详情
    /// </summary>
    [DisplayName("获取基站详情")]
    public async Task<StationStatusOutput> Get(long id)
    {
        return await _stationRep.AsQueryable()
            .Where(u => u.Id == id)
            .Select<StationStatusOutput>()
            .FirstAsync();
    }

    /// <summary>
    /// 新增基站
    /// </summary>
    [DisplayName("新增基站")]
    public async Task<long> Add(AddStationInput input)
    {
        var entity = input.Adapt<StationStatus>();
        return await _stationRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新基站
    /// </summary>
    [DisplayName("更新基站")]
    public async Task Update(UpdateStationInput input)
    {
        var entity = input.Adapt<StationStatus>();
        await _stationRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除基站
    /// </summary>
    [DisplayName("删除基站")]
    public async Task Delete(long id)
    {
        await _stationRep.DeleteByIdAsync(id);
    }

    /// <summary>
    /// 获取在线基站统计
    /// </summary>
    [DisplayName("获取在线基站统计")]
    public async Task<StationStatisticsOutput> GetStatistics([FromQuery] long mineId)
    {
        var list = await _stationRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .ToListAsync();

        return new StationStatisticsOutput
        {
            TotalCount = list.Count,
            OnlineCount = list.Count(u => u.Status == 1),
            OfflineCount = list.Count(u => u.Status == 0)
        };
    }

    /// <summary>
    /// 批量更新基站状态
    /// </summary>
    [DisplayName("批量更新基站状态")]
    public async Task UpdateStatus(List<UpdateStationStatusInput> stations)
    {
        foreach (var station in stations)
        {
            var existing = await _stationRep.AsQueryable()
                .Where(u => u.MineId == station.MineId && u.StationId == station.StationId)
                .FirstAsync();

            if (existing != null)
            {
                existing.Status = station.Status;
                existing.Power = station.Power;
                existing.Signal = station.Signal;
                existing.UpdateTime = DateTime.Now;
                await _stationRep.AsUpdateable(existing).ExecuteCommandAsync();
            }
            else
            {
                var newStation = new StationStatus
                {
                    MineId = station.MineId,
                    StationId = station.StationId,
                    StationName = station.StationName,
                    Status = station.Status,
                    Power = station.Power,
                    Signal = station.Signal,
                    UpdateTime = DateTime.Now
                };
                await _stationRep.InsertAsync(newStation);
            }
        }
    }
}

/// <summary>
/// 基站分页输入
/// </summary>
public class PageStationInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 基站编号
    /// </summary>
    public string StationId { get; set; }

    /// <summary>
    /// 基站名称
    /// </summary>
    public string StationName { get; set; }

    /// <summary>
    /// 状态:0-离线,1-在线
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 新增基站输入
/// </summary>
public class AddStationInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 基站编号
    /// </summary>
    public string StationId { get; set; }

    /// <summary>
    /// 基站名称
    /// </summary>
    public string StationName { get; set; }
}

/// <summary>
/// 更新基站输入
/// </summary>
public class UpdateStationInput : AddStationInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// 更新基站状态输入
/// </summary>
public class UpdateStationStatusInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 基站编号
    /// </summary>
    public string StationId { get; set; }

    /// <summary>
    /// 基站名称
    /// </summary>
    public string StationName { get; set; }

    /// <summary>
    /// 状态:0-离线,1-在线
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 电量
    /// </summary>
    public decimal? Power { get; set; }

    /// <summary>
    /// 信号强度
    /// </summary>
    public int? Signal { get; set; }
}

/// <summary>
/// 基站状态输出
/// </summary>
public class StationStatusOutput
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
    /// 基站编号
    /// </summary>
    public string StationId { get; set; }

    /// <summary>
    /// 基站名称
    /// </summary>
    public string StationName { get; set; }

    /// <summary>
    /// 状态:0-离线,1-在线
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName => Status == 1 ? "在线" : "离线";

    /// <summary>
    /// 电量
    /// </summary>
    public decimal? Power { get; set; }

    /// <summary>
    /// 信号强度
    /// </summary>
    public int? Signal { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 基站统计输出
/// </summary>
public class StationStatisticsOutput
{
    /// <summary>
    /// 总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 在线数
    /// </summary>
    public int OnlineCount { get; set; }

    /// <summary>
    /// 离线数
    /// </summary>
    public int OfflineCount { get; set; }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 人员定位服务
/// </summary>
[ApiDescriptionSettings("Person", Description = "人员定位")]
public class PersonService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<PersonLocation> _personRep;
    private readonly SqlSugarRepository<PersonRecord> _personRecordRep;

    public PersonService(SqlSugarRepository<PersonLocation> personRep, SqlSugarRepository<PersonRecord> personRecordRep)
    {
        _personRep = personRep;
        _personRecordRep = personRecordRep;
    }

    /// <summary>
    /// 分页查询实时位置
    /// </summary>
    [DisplayName("分页查询实时位置")]
    public async Task<SqlSugarPagedList<PersonLocationOutput>> GetRealtimePage([FromQuery] PagePersonInput input)
    {
        return await _personRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.CardId), u => u.CardId.Contains(input.CardId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PersonName), u => u.PersonName.Contains(input.PersonName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.AreaCode), u => u.AreaCode == input.AreaCode)
            .OrderByDescending(u => u.UpdateTime)
            .Select<PersonLocationOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取区域统计
    /// </summary>
    [DisplayName("获取区域统计")]
    public async Task<List<AreaStatisticsOutput>> GetAreaStatistics([FromQuery] long mineId)
    {
        var list = await _personRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .Select(u => new { u.AreaCode, u.AreaName })
            .ToListAsync();

        return list
            .GroupBy(u => new { u.AreaCode, u.AreaName })
            .Select(g => new AreaStatisticsOutput
            {
                AreaCode = g.Key.AreaCode,
                AreaName = g.Key.AreaName,
                PersonCount = g.Count()
            })
            .ToList();
    }

    /// <summary>
    /// 获取历史轨迹
    /// </summary>
    [DisplayName("获取历史轨迹")]
    public async Task<List<PersonRecordOutput>> GetTrackHistory([FromQuery] TrackHistoryInput input)
    {
        return await _personRecordRep.AsQueryable()
            .Where(u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.CardId), u => u.CardId == input.CardId)
            .WhereIF(input.StartTime.HasValue, u => u.RecordTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, u => u.RecordTime <= input.EndTime)
            .OrderBy(u => u.RecordTime)
            .Select<PersonRecordOutput>()
            .ToListAsync();
    }

    /// <summary>
    /// 获取当前在线人数
    /// </summary>
    [DisplayName("获取当前在线人数")]
    public async Task<int> GetOnlineCount([FromQuery] long mineId)
    {
        return await _personRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .CountAsync();
    }

    /// <summary>
    /// 获取进出记录分页
    /// </summary>
    [DisplayName("获取进出记录分页")]
    public async Task<SqlSugarPagedList<PersonRecordOutput>> GetRecordPage([FromQuery] PagePersonRecordInput input)
    {
        return await _personRecordRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.CardId), u => u.CardId.Contains(input.CardId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PersonName), u => u.PersonName.Contains(input.PersonName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.AreaCode), u => u.AreaCode == input.AreaCode)
            .WhereIF(input.RecordType.HasValue, u => u.RecordType == input.RecordType)
            .WhereIF(input.StartTime.HasValue, u => u.RecordTime >= input.StartTime)
            .WhereIF(input.EndTime.HasValue, u => u.RecordTime <= input.EndTime)
            .OrderByDescending(u => u.RecordTime)
            .Select<PersonRecordOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }
}

/// <summary>
/// 人员定位分页输入
/// </summary>
public class PagePersonInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    public string AreaCode { get; set; }
}

/// <summary>
/// 人员进出记录分页输入
/// </summary>
public class PagePersonRecordInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    public string AreaCode { get; set; }

    /// <summary>
    /// 记录类型:1-进入,2-离开
    /// </summary>
    public int? RecordType { get; set; }

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
/// 轨迹历史输入
/// </summary>
public class TrackHistoryInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

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
/// 区域统计输出
/// </summary>
public class AreaStatisticsOutput
{
    /// <summary>
    /// 区域编码
    /// </summary>
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int PersonCount { get; set; }
}

/// <summary>
/// 人员定位输出
/// </summary>
public class PersonLocationOutput
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
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string DeptName { get; set; }

    /// <summary>
    /// 定位分站ID
    /// </summary>
    public string StationId { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 大地坐标X
    /// </summary>
    public decimal? X { get; set; }

    /// <summary>
    /// 大地坐标Y
    /// </summary>
    public decimal? Y { get; set; }

    /// <summary>
    /// 深度/高程
    /// </summary>
    public decimal? Z { get; set; }

    /// <summary>
    /// 状态:1-进入,2-离开
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName => Status == 1 ? "进入" : "离开";

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 人员进出记录输出
/// </summary>
public class PersonRecordOutput
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
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string DeptName { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 状态:0-正常,1-报警
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 类型:1-进入,2-离开
    /// </summary>
    public int RecordType { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    public string RecordTypeName => RecordType == 1 ? "进入" : "离开";

    /// <summary>
    /// 记录时间
    /// </summary>
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

using Admin.NET.Application.CoalMine.PersonLocation.Dtos;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.PersonLocation.Services;

/// <summary>
/// 人员定位服务接口
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// 分页查询实时位置
    /// </summary>
    Task<SqlSugarPagedList<PersonLocationOutput>> GetRealtimePage(PersonLocationPageInput input);

    /// <summary>
    /// 获取实时位置详情
    /// </summary>
    Task<PersonLocation> GetRealtime(long id);

    /// <summary>
    /// 获取井下人数统计
    /// </summary>
    Task<List<AreaPersonStatisticsOutput>> GetAreaStatistics(long? mineId);

    /// <summary>
    /// 获取某区域人员列表
    /// </summary>
    Task<List<PersonLocationOutput>> GetPersonListByArea(long mineId, string areaCode);

    /// <summary>
    /// 获取历史轨迹
    /// </summary>
    Task<List<Dictionary<string, object>>> GetTrackHistory(long mineId, string cardId, DateTime startTime, DateTime endTime);

    /// <summary>
    /// 批量保存实时位置
    /// </summary>
    Task<int> BatchSaveRealtime(List<PersonLocation> list);
}

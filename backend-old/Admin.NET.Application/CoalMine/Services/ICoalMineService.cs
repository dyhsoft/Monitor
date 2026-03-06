using Admin.NET.Application.CoalMine.Dtos;

namespace Admin.NET.Application.CoalMine.Services;

/// <summary>
/// 煤矿管理服务接口
/// </summary>
public interface ICoalMineService
{
    /// <summary>
    /// 分页查询煤矿
    /// </summary>
    Task<SqlSugarPagedList<CoalMine>> GetPage(CoalMinePageInput input);

    /// <summary>
    /// 获取煤矿详情
    /// </summary>
    Task<CoalMine> Get(long id);

    /// <summary>
    /// 添加煤矿
    /// </summary>
    Task<long> Add(AddCoalMineInput input);

    /// <summary>
    /// 更新煤矿
    /// </summary>
    Task Update(UpdateCoalMineInput input);

    /// <summary>
    /// 删除煤矿
    /// </summary>
    Task Delete(DeleteCoalMineInput input);

    /// <summary>
    /// 获取煤矿列表（下拉选项）
    /// </summary>
    Task<List<SelectOption>> GetSelect();
}

using Admin.NET.Core;
using SqlSugar;
using Furion;
using Furion.DynamicApiController;

namespace Admin.NET.Application;

/// <summary>
/// 煤矿管理服务
/// </summary>
public class CoalMineService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public CoalMineService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取煤矿列表
    /// </summary>
    public async Task<SqlSugarPagedList<CoalMine>> GetPage(BasePageInput input)
    {
        return await _db.Queryable<CoalMine>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取煤矿详情
    /// </summary>
    public async Task<CoalMine> Get(long id)
    {
        return await _db.Queryable<CoalMine>().Where(it => it.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增煤矿
    /// </summary>
    public async Task<long> Add(CoalMine input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新煤矿
    /// </summary>
    public async Task Update(CoalMine input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除煤矿
    /// </summary>
    public async Task Delete(long id)
    {
        await _db.Deleteable<CoalMine>().Where(it => it.Id == id).ExecuteCommandAsync();
    }
}

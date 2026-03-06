using Admin.NET.Core;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 网关配置服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Gateway", Order = 100)]
public class GatewayConfigService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public GatewayConfigService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取网关配置列表
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<CoalGatewayConfig>> GetPage([FromBody] PageInputBase input)
    {
        return await _db.Queryable<CoalGatewayConfig>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取网关配置详情
    /// </summary>
    public async Task<CoalGatewayConfig> Get(long id)
    {
        return await _db.Queryable<CoalGatewayConfig>().Where(it => it.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增网关配置
    /// </summary>
    public async Task<long> Add([FromBody] CoalGatewayConfig input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新网关配置
    /// </summary>
    public async Task Update([FromBody] CoalGatewayConfig input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除网关配置
    /// </summary>
    public async Task Delete(long id)
    {
        await _db.Deleteable<CoalGatewayConfig>().Where(it => it.Id == id).ExecuteCommandAsync();
    }
}

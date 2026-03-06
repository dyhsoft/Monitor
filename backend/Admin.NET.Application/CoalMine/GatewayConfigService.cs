namespace Admin.NET.Application;

/// <summary>
/// 网关配置服务
/// </summary>
public class GatewayConfigService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public GatewayConfigService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<SqlSugarPagedList<CoalGatewayConfig>> GetPage(BasePageInput input)
    {
        return await _db.Queryable<CoalGatewayConfig>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    public async Task<CoalGatewayConfig> Get(long id)
    {
        return await _db.Queryable<CoalGatewayConfig>().Where(it => it.Id == id).FirstAsync();
    }

    public async Task<long> Add(CoalGatewayConfig input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    public async Task Update(CoalGatewayConfig input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    public async Task Delete(long id)
    {
        await _db.Deleteable<CoalGatewayConfig>().Where(it => it.Id == id).ExecuteCommandAsync();
    }
}

using Admin.NET.Core;
using SqlSugar;
using Furion;
using Furion.DynamicApiController;

namespace Admin.NET.Application;

/// <summary>
/// 传感器服务
/// </summary>
public class SensorService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public SensorService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<SqlSugarPagedList<CoalSensor>> GetPage(BasePageInput input)
    {
        return await _db.Queryable<CoalSensor>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    public async Task<CoalSensor> Get(long id)
    {
        return await _db.Queryable<CoalSensor>().Where(it => it.Id == id).FirstAsync();
    }

    public async Task<long> Add(CoalSensor input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    public async Task Update(CoalSensor input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    public async Task Delete(long id)
    {
        await _db.Deleteable<CoalSensor>().Where(it => it.Id == id).ExecuteCommandAsync();
    }
}

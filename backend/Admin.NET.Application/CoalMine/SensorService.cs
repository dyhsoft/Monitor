using Admin.NET.Core;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 传感器服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Sensor", Order = 100)]
public class SensorService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public SensorService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取传感器列表
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<CoalSensor>> GetPage([FromBody] PageInputBase input)
    {
        return await _db.Queryable<CoalSensor>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取传感器详情
    /// </summary>
    public async Task<CoalSensor> Get(long id)
    {
        return await _db.Queryable<CoalSensor>().Where(it => it.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增传感器
    /// </summary>
    public async Task<long> Add([FromBody] CoalSensor input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新传感器
    /// </summary>
    public async Task Update([FromBody] CoalSensor input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除传感器
    /// </summary>
    public async Task Delete(long id)
    {
        await _db.Deleteable<CoalSensor>().Where(it => it.Id == id).ExecuteCommandAsync();
    }
}

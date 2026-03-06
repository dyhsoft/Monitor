using Admin.NET.Core;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 人员定位服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Person", Order = 100)]
public class PersonService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public PersonService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取人员定位实时数据
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<PersonLocation>> GetPage( BasePageInput input)
    {
        return await _db.Queryable<PersonLocation>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取人员统计
    /// </summary>
    public async Task<dynamic> GetStatistics()
    {
        var count = await _db.Queryable<PersonLocation>().CountAsync();
        return new { Total = count };
    }
}

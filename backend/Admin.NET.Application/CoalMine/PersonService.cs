namespace Admin.NET.Application;

/// <summary>
/// 人员定位服务
/// </summary>
public class PersonService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public PersonService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<SqlSugarPagedList<PersonLocation>> GetPage(BasePageInput input)
    {
        return await _db.Queryable<PersonLocation>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    public async Task<dynamic> GetStatistics()
    {
        var count = await _db.Queryable<PersonLocation>().CountAsync();
        return new { Total = count };
    }
}

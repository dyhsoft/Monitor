namespace Admin.NET.Application;

/// <summary>
/// 视频监控服务
/// </summary>
public class VideoService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public VideoService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<SqlSugarPagedList<VideoDevice>> GetPage(BasePageInput input)
    {
        return await _db.Queryable<VideoDevice>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    public async Task<VideoDevice> Get(long id)
    {
        return await _db.Queryable<VideoDevice>().Where(it => it.Id == id).FirstAsync();
    }

    public async Task<long> Add(VideoDevice input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    public async Task Update(VideoDevice input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    public async Task Delete(long id)
    {
        await _db.Deleteable<VideoDevice>().Where(it => it.Id == id).ExecuteCommandAsync();
    }
}

using Admin.NET.Core;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 视频监控服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Video", Order = 100)]
public class VideoService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;

    public VideoService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取视频设备列表
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<VideoDevice>> GetPage( BasePageInput input)
    {
        return await _db.Queryable<VideoDevice>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取视频设备详情
    /// </summary>
    public async Task<VideoDevice> Get(long id)
    {
        return await _db.Queryable<VideoDevice>().Where(it => it.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增视频设备
    /// </summary>
    public async Task<long> Add( VideoDevice input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新视频设备
    /// </summary>
    public async Task Update( VideoDevice input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除视频设备
    /// </summary>
    public async Task Delete(long id)
    {
        await _db.Deleteable<VideoDevice>().Where(it => it.Id == id).ExecuteCommandAsync();
    }
}

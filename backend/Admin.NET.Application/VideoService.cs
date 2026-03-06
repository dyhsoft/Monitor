using Admin.NET.Core;
using Admin.NET.Core.Entity.CoalMine;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<SqlSugarPagedList<VideoDevice>>> GetPage([FromBody] VideoInput input)
    {
        return await _db.Queryable<VideoDevice>()
            .WhereIF(input.MineId > 0, it => it.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.DeviceName), it => it.DeviceName.Contains(input.DeviceName))
            .OrderBy(it => it.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
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
    public async Task<long> Add([FromBody] VideoDevice input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新视频设备
    /// </summary>
    public async Task Update([FromBody] VideoDevice input)
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

/// <summary>
/// 视频设备输入
/// </summary>
public class VideoInput
{
    public int Current { get; set; } = 1;
    public int Size { get; set; } = 10;
    public long? MineId { get; set; }
    public string DeviceName { get; set; }
}

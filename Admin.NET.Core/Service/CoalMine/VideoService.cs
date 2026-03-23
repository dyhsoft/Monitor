// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 视频监控服务
/// </summary>
[ApiDescriptionSettings("Video", Description = "视频监控")]
public class VideoService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<VideoConfig> _videoRep;

    public VideoService(SqlSugarRepository<VideoConfig> videoRep)
    {
        _videoRep = videoRep;
    }

    /// <summary>
    /// 分页查询摄像头列表
    /// </summary>
    [DisplayName("分页查询摄像头列表")]
    public async Task<SqlSugarPagedList<VideoConfig>> GetPage([FromQuery] PageVideoInput input)
    {
        return await _videoRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.CameraCode), u => u.CameraCode.Contains(input.CameraCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.CameraName), u => u.CameraName.Contains(input.CameraName))
            .WhereIF(input.CameraType.HasValue, u => u.CameraType == input.CameraType)
            .WhereIF(input.Enabled.HasValue, u => u.Enabled == input.Enabled)
            .OrderBy(u => u.CameraCode)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取摄像头详情
    /// </summary>
    [DisplayName("获取摄像头详情")]
    public async Task<VideoConfig> Get(long id)
    {
        return await _videoRep.AsQueryable()
            .Where(u => u.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 新增摄像头
    /// </summary>
    [DisplayName("新增摄像头")]
    public async Task<long> Add([FromBody] AddVideoInput input)
    {
        var entity = new VideoConfig
        {
            MineId = input.MineId,
            CameraCode = input.CameraCode,
            CameraName = input.CameraName,
            Location = input.Location,
            CameraType = input.CameraType,
            Protocol = input.Protocol,
            StreamUrl = input.StreamUrl,
            AreaName = input.AreaName,
            Longitude = input.Longitude,
            Latitude = input.Latitude,
            Manufacturer = input.Manufacturer,
            Model = input.Model,
            IpAddress = input.IpAddress,
            Port = input.Port,
            Enabled = input.Enabled,
            Remark = input.Remark
        };
        return await _videoRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新摄像头
    /// </summary>
    [DisplayName("更新摄像头")]
    public async Task Update([FromBody] UpdateVideoInput input)
    {
        var entity = await _videoRep.AsQueryable()
            .Where(u => u.Id == input.Id)
            .FirstAsync();

        if (entity == null)
            throw new Exception("摄像头不存在");

        entity.CameraName = input.CameraName;
        entity.Location = input.Location;
        entity.CameraType = input.CameraType;
        entity.Protocol = input.Protocol;
        entity.StreamUrl = input.StreamUrl;
        entity.AreaName = input.AreaName;
        entity.Longitude = input.Longitude;
        entity.Latitude = input.Latitude;
        entity.Manufacturer = input.Manufacturer;
        entity.Model = input.Model;
        entity.IpAddress = input.IpAddress;
        entity.Port = input.Port;
        entity.Enabled = input.Enabled;
        entity.Remark = input.Remark;

        await _videoRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除摄像头
    /// </summary>
    [DisplayName("删除摄像头")]
    public async Task Delete(long id)
    {
        await _videoRep.AsDeleteable()
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 设置启用状态
    /// </summary>
    [DisplayName("设置启用状态")]
    public async Task SetEnabled(long id, int enabled)
    {
        await _videoRep.AsUpdateable()
            .SetColumns(u => u.Enabled == enabled)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取摄像头下拉列表
    /// </summary>
    [DisplayName("获取摄像头下拉列表")]
    public async Task<List<VideoSelectOutput>> GetSelectList([FromQuery] long mineId)
    {
        return await _videoRep.AsQueryable()
            .Where(u => u.MineId == mineId && u.Enabled == 1)
            .Select(u => new VideoSelectOutput
            {
                Id = u.Id,
                CameraCode = u.CameraCode,
                CameraName = u.CameraName,
                Location = u.Location,
                StreamUrl = u.StreamUrl
            })
            .ToListAsync();
    }
}

/// <summary>
/// 视频分页输入
/// </summary>
public class PageVideoInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 摄像头编号
    /// </summary>
    public string CameraCode { get; set; }

    /// <summary>
    /// 摄像头名称
    /// </summary>
    public string CameraName { get; set; }

    /// <summary>
    /// 摄像头类型
    /// </summary>
    public int? CameraType { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int? Enabled { get; set; }
}

/// <summary>
/// 新增视频输入
/// </summary>
public class AddVideoInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 摄像头编号
    /// </summary>
    public string CameraCode { get; set; }

    /// <summary>
    /// 摄像头名称
    /// </summary>
    public string CameraName { get; set; }

    /// <summary>
    /// 安装位置
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// 摄像头类型
    /// </summary>
    public int CameraType { get; set; } = 1;

    /// <summary>
    /// 视频协议
    /// </summary>
    public int Protocol { get; set; } = 1;

    /// <summary>
    /// 流媒体地址
    /// </summary>
    public string StreamUrl { get; set; }

    /// <summary>
    /// 所属区域
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 生产厂家
    /// </summary>
    public string Manufacturer { get; set; }

    /// <summary>
    /// 型号
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

/// <summary>
/// 更新视频输入
/// </summary>
public class UpdateVideoInput : AddVideoInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// 视频下拉输出
/// </summary>
public class VideoSelectOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 摄像头编号
    /// </summary>
    public string CameraCode { get; set; }

    /// <summary>
    /// 摄像头名称
    /// </summary>
    public string CameraName { get; set; }

    /// <summary>
    /// 安装位置
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// 流媒体地址
    /// </summary>
    public string StreamUrl { get; set; }
}

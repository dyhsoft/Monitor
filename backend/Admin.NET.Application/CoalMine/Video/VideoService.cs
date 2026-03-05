using Admin.NET.Core;
using Admin.NET.EntityFramework.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application;

/// <summary>
/// 视频监控服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Video", Order = 100)]
public class VideoService : IVideoService, ITransient
{
    private readonly ISqlSugarClient _db;

    public VideoService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询视频设备
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<VideoDevice>> GetPage(VideoDevicePageInput input)
    {
        return await _db.Queryable<VideoDevice>()
            .LeftJoin<CoalMine>((v, m) => v.MineId == m.Id)
            .WhereIF(input.MineId.HasValue, (v, m) => v.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.DeviceCode), (v, m) => v.DeviceCode.Contains(input.DeviceCode))
            .WhereIF(!string.IsNullOrEmpty(input.DeviceName), (v, m) => v.DeviceName.Contains(input.DeviceName))
            .WhereIF(!string.IsNullOrEmpty(input.DeviceType), (v, m) => v.DeviceType.Contains(input.DeviceType))
            .Select((v, m) => new VideoDevice
            {
                Id = v.Id,
                MineId = v.MineId,
                MineName = m.Name,
                DeviceCode = v.DeviceCode,
                DeviceName = v.DeviceName,
                DeviceType = v.DeviceType,
                IpAddress = v.IpAddress,
                Port = v.Port,
                Channel = v.Channel,
                StreamType = v.StreamType,
                Username = v.Username,
                // Password 不返回前端
                Status = v.Status,
                InstallLocation = v.InstallLocation,
                Remark = v.Remark,
                CreateTime = v.CreateTime
            })
            .OrderBy(v => v.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取视频设备详情
    /// </summary>
    public async Task<VideoDevice> Get(long id)
    {
        return await _db.Queryable<VideoDevice>()
            .Where(v => v.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 新增视频设备
    /// </summary>
    public async Task<long> Add(VideoDevice input)
    {
        // 检查设备编号是否已存在
        var exists = await _db.Queryable<VideoDevice>()
            .Where(v => v.MineId == input.MineId && v.DeviceCode == input.DeviceCode)
            .FirstAsync();

        if (exists != null)
        {
            throw Oops.Oh("该设备编号已存在");
        }

        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新视频设备
    /// </summary>
    public async Task Update(VideoDevice input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除视频设备
    /// </summary>
    public async Task Delete(long id)
    {
        await _db.Deleteable<VideoDevice>(id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取视频设备类型列表
    /// </summary>
    public async Task<List<Dictionary<string, string>>> GetDeviceTypes()
    {
        var types = new List<Dictionary<string, string>>
        {
            new() { { "code", "HIKVISION" }, { "name", "海康威视" } },
            new() { { "code", "DH" }, { "name", "大华" } },
            new() { { "code", "UNIVIEW" }, { "name", "宇视" } },
            new() { { "code", "ONVIF" }, { "name", "ONVIF" } },
            new() { { "code", "RTSP" }, { "name", "RTSP" } }
        };
        return types;
    }

    /// <summary>
    /// 获取码流类型列表
    /// </summary>
    public async Task<List<Dictionary<string, int>>> GetStreamTypes()
    {
        var types = new List<Dictionary<string, int>>
        {
            new() { { "name", "主码流" }, { "value", 1 } },
            new() { { "name", "子码流" }, { "value", 2 } }
        };
        return types;
    }

    /// <summary>
    /// 测试视频设备连接
    /// </summary>
    public async Task<bool> TestConnection(VideoDevice input)
    {
        try
        {
            // 海康威视 SDK 连接测试
            // 实际项目中需要调用海康威视 SDK 或 HTTP API 进行测试
            // 这里简化为模拟测试
            if (string.IsNullOrEmpty(input.IpAddress))
            {
                throw Oops.Oh("IP地址不能为空");
            }

            // TODO: 实际对接海康威视 SDK
            // CHCNetSDK.NETDll.NET_DVR_Init();
            // var loginInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            // var userId = CHCNetSDK.NETDll.NET_DVR_Login_V30(input.IpAddress, (short)input.Port, input.Username, input.Password, ref loginInfo);
            
            return await Task.FromResult(true);
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }

    /// <summary>
    /// 获取视频播放地址
    /// </summary>
    public async Task<string> GetPlayUrl(long id)
    {
        var device = await _db.Queryable<VideoDevice>()
            .Where(v => v.Id == id)
            .FirstAsync();

        if (device == null)
        {
            throw Oops.Oh("设备不存在");
        }

        // 根据不同设备类型生成播放地址
        return device.DeviceType?.ToUpper() switch
        {
            "HIKVISION" => GenerateHikvisionUrl(device),
            "DH" => GenerateDHUrl(device),
            "RTSP" => $"rtsp://{device.Username}:{device.Password}@{device.IpAddress}:{device.Port}/ch{device.Channel}/{device.StreamType}",
            _ => $"rtsp://{device.IpAddress}:{device.Port}/ch{device.Channel}/{device.StreamType}"
        };
    }

    private string GenerateHikvisionUrl(VideoDevice device)
    {
        // 海康威视 HTTP API 获取播放地址
        return $"/video/stream/{device.DeviceCode}";
    }

    private string GenerateDHUrl(VideoDevice device)
    {
        // 大华 HTTP API 获取播放地址
        return $"/video/dahua/stream/{device.DeviceCode}";
    }
}

/// <summary>
/// 视频设备分页输入
/// </summary>
public class VideoDevicePageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 设备编号
    /// </summary>
    public string? DeviceCode { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    public string? DeviceType { get; set; }
}

/// <summary>
/// 视频设备服务接口
/// </summary>
public interface IVideoService
{
    Task<SqlSugarPagedList<VideoDevice>> GetPage(VideoDevicePageInput input);
    Task<VideoDevice> Get(long id);
    Task<long> Add(VideoDevice input);
    Task Update(VideoDevice input);
    Task Delete(long id);
    Task<List<Dictionary<string, string>>> GetDeviceTypes();
    Task<List<Dictionary<string, int>>> GetStreamTypes();
    Task<bool> TestConnection(VideoDevice input);
    Task<string> GetPlayUrl(long id);
}

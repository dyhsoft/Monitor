// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 视频监控配置表
/// </summary>
[SugarTable("VideoConfigs", "视频监控配置表")]
public class VideoConfig : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 摄像头编号
    /// </summary>
    [SugarColumn(ColumnDescription = "摄像头编号", Length = 50)]
    public string CameraCode { get; set; }

    /// <summary>
    /// 摄像头名称
    /// </summary>
    [SugarColumn(ColumnDescription = "摄像头名称", Length = 100)]
    public string CameraName { get; set; }

    /// <summary>
    /// 安装位置
    /// </summary>
    [SugarColumn(ColumnDescription = "安装位置", Length = 200, IsNullable = true)]
    public string Location { get; set; }

    /// <summary>
    /// 摄像头类型:1-球机,2-枪机,3-半球
    /// </summary>
    [SugarColumn(ColumnDescription = "摄像头类型", DefaultValue = "1")]
    public int CameraType { get; set; } = 1;

    /// <summary>
    /// 视频协议:1-RTSP,2-GB28181,3-ONVIF,4-HTTP
    /// </summary>
    [SugarColumn(ColumnDescription = "视频协议", DefaultValue = "1")]
    public int Protocol { get; set; } = 1;

    /// <summary>
    /// 流媒体地址
    /// </summary>
    [SugarColumn(ColumnDescription = "流媒体地址", Length = 500, IsNullable = true)]
    public string StreamUrl { get; set; }

    /// <summary>
    /// 所属区域
    /// </summary>
    [SugarColumn(ColumnDescription = "所属区域", Length = 100, IsNullable = true)]
    public string AreaName { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    [SugarColumn(ColumnDescription = "经度", IsNullable = true)]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    [SugarColumn(ColumnDescription = "纬度", IsNullable = true)]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 生产厂家
    /// </summary>
    [SugarColumn(ColumnDescription = "生产厂家", Length = 100, IsNullable = true)]
    public string Manufacturer { get; set; }

    /// <summary>
    /// 型号
    /// </summary>
    [SugarColumn(ColumnDescription = "型号", Length = 100, IsNullable = true)]
    public string Model { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(ColumnDescription = "IP地址", Length = 50, IsNullable = true)]
    public string IpAddress { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    [SugarColumn(ColumnDescription = "端口", IsNullable = true)]
    public int? Port { get; set; }

    /// <summary>
    /// 是否启用:0-禁用,1-启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int Enabled { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}

/// <summary>
/// 视频监控记录表
/// </summary>
[SugarTable("VideoRecords", "视频监控记录表")]
public class VideoRecord : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 摄像头编号
    /// </summary>
    [SugarColumn(ColumnDescription = "摄像头编号", Length = 50)]
    public string CameraCode { get; set; }

    /// <summary>
    /// 摄像头名称
    /// </summary>
    [SugarColumn(ColumnDescription = "摄像头名称", Length = 100, IsNullable = true)]
    public string CameraName { get; set; }

    /// <summary>
    /// 录像类型:1-实时,2-录像回放,3-告警录像
    /// </summary>
    [SugarColumn(ColumnDescription = "录像类型", DefaultValue = "1")]
    public int RecordType { get; set; } = 1;

    /// <summary>
    /// 录像地址
    /// </summary>
    [SugarColumn(ColumnDescription = "录像地址", Length = 500, IsNullable = true)]
    public string RecordUrl { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(ColumnDescription = "开始时间")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(ColumnDescription = "结束时间", IsNullable = true)]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 文件大小(字节)
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小(字节)", IsNullable = true)]
    public long? FileSize { get; set; }

    /// <summary>
    /// 状态:0-录制中,1-已完成,2-失败
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "0")]
    public int Status { get; set; } = 0;
}

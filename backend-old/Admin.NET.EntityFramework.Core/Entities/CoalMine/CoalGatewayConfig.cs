using Furion.DatabaseAccessor;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.EntityFramework.Core;

/// <summary>
/// 网关配置实体
/// </summary>
[SugarTable("CoalGatewayConfig", "煤矿网关配置表")]
[Tenant(Seven = true)]
public class CoalGatewayConfig : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 网关编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "网关编号")]
    [Required]
    public string GatewayCode { get; set; }

    /// <summary>
    /// 网关名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "网关名称")]
    public string GatewayName { get; set; }

    /// <summary>
    /// 网关类型（安全监测/人员定位/水害防治/视频监控）
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "网关类型")]
    [Required]
    public string GatewayType { get; set; }

    /// <summary>
    /// FTP主机地址
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "FTP主机地址")]
    public string FtpHost { get; set; }

    /// <summary>
    /// FTP端口
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP端口")]
    public int FtpPort { get; set; } = 21;

    /// <summary>
    /// FTP用户名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "FTP用户名")]
    public string FtpUsername { get; set; }

    /// <summary>
    /// FTP密码（加密存储）
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "FTP密码")]
    public string FtpPassword { get; set; }

    /// <summary>
    /// FTP根目录
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "FTP根目录")]
    public string FtpRootPath { get; set; }

    /// <summary>
    /// 数据目录（相对于根目录）
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "数据目录")]
    public string DataPath { get; set; }

    /// <summary>
    /// 文件编码（UTF-8/GBK/GB2312/GB18030）
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "文件编码")]
    public string FileEncoding { get; set; } = "UTF-8";

    /// <summary>
    /// 数据推送频率（秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "数据推送频率(秒)")]
    public int PushFrequency { get; set; } = 60;

    /// <summary>
    /// IP地址限制
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "IP地址限制")]
    public string AllowedIp { get; set; }

    /// <summary>
    /// 状态（0=停用,1=启用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

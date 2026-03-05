namespace Admin.NET.Application.CoalMine.Dtos;

/// <summary>
/// 网关配置分页查询
/// </summary>
public class GatewayConfigPageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 网关编号
    /// </summary>
    public string GatewayCode { get; set; }

    /// <summary>
    /// 网关名称
    /// </summary>
    public string GatewayName { get; set; }

    /// <summary>
    /// 网关类型
    /// </summary>
    public string GatewayType { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 添加网关配置
/// </summary>
public class AddGatewayConfigInput
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 网关编号
    /// </summary>
    [Required]
    [StringLength(50)]
    public string GatewayCode { get; set; }

    /// <summary>
    /// 网关名称
    /// </summary>
    [StringLength(100)]
    public string GatewayName { get; set; }

    /// <summary>
    /// 网关类型
    /// </summary>
    [Required]
    [StringLength(50)]
    public string GatewayType { get; set; }

    /// <summary>
    /// FTP主机地址
    /// </summary>
    [StringLength(100)]
    public string FtpHost { get; set; }

    /// <summary>
    /// FTP端口
    /// </summary>
    public int FtpPort { get; set; } = 21;

    /// <summary>
    /// FTP用户名
    /// </summary>
    [StringLength(50)]
    public string FtpUsername { get; set; }

    /// <summary>
    /// FTP密码
    /// </summary>
    [StringLength(200)]
    public string FtpPassword { get; set; }

    /// <summary>
    /// FTP根目录
    /// </summary>
    [StringLength(200)]
    public string FtpRootPath { get; set; }

    /// <summary>
    /// 数据目录
    /// </summary>
    [StringLength(200)]
    public string DataPath { get; set; }

    /// <summary>
    /// 文件编码
    /// </summary>
    [StringLength(20)]
    public string FileEncoding { get; set; } = "UTF-8";

    /// <summary>
    /// 数据推送频率
    /// </summary>
    public int PushFrequency { get; set; } = 60;

    /// <summary>
    /// IP地址限制
    /// </summary>
    [StringLength(100)]
    public string AllowedIp { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string Remark { get; set; }
}

/// <summary>
/// 更新网关配置
/// </summary>
public class UpdateGatewayConfigInput : AddGatewayConfigInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// 删除网关配置
/// </summary>
public class DeleteGatewayConfigInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

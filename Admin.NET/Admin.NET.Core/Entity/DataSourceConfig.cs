// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
namespace Admin.NET.Core;

/// <summary>
/// 数据源配置表
/// </summary>
[SugarTable(null, "数据源配置表")]
[SysTable]
public partial class DataSourceConfig : EntityBaseId
{
    /// <summary>
    /// 关联FTP配置Id
    /// </summary>
    [SugarColumn(ColumnDescription = "关联FTP配置Id")]
    public long FtpConfigId { get; set; }

    /// <summary>
    /// 系统类型
    /// </summary>
    [SugarColumn(ColumnDescription = "系统类型", Length = 20)]
    public string SystemType { get; set; }

    /// <summary>
    /// 解析协议
    /// </summary>
    [SugarColumn(ColumnDescription = "解析协议", Length = 20)]
    public string Protocol { get; set; }

    /// <summary>
    /// 远程读取路径
    /// </summary>
    [SugarColumn(ColumnDescription = "远程读取路径", Length = 200)]
    public string RemotePath { get; set; }

    /// <summary>
    /// 文件匹配规则
    /// </summary>
    [SugarColumn(ColumnDescription = "文件匹配规则", Length = 50)]
    public string FilePattern { get; set; }

    /// <summary>
    /// 是否启用解析
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用解析")]
    public bool ParseEnabled { get; set; }

    /// <summary>
    /// 解析间隔(秒)
    /// </summary>
    [SugarColumn(ColumnDescription = "解析间隔(秒)")]
    public int ParseInterval { get; set; } = 30;

    /// <summary>
    /// 最后解析时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后解析时间")]
    public DateTime? LastParseTime { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id")]
    public long TenantId { get; set; }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// FTP配置表
/// </summary>
[SugarTable("FtpConfigs", "FTP配置表")]
public class FtpConfig : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(MineId))]
    public CoalMine CoalMine { get; set; }

    /// <summary>
    /// FTP主机地址
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP主机地址", Length = 100)]
    public string Host { get; set; }

    /// <summary>
    /// FTP端口
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP端口", DefaultValue = "21")]
    public int Port { get; set; } = 21;

    /// <summary>
    /// FTP用户名
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP用户名", Length = 50)]
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnDescription = "密码", Length = 100)]
    public string Password { get; set; }

    /// <summary>
    /// 根目录
    /// </summary>
    [SugarColumn(ColumnDescription = "根目录", Length = 500)]
    public string RootDirectory { get; set; }

    /// <summary>
    /// 绑定系统:安全监测/人员定位/矿压监测/水文监测
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定系统", Length = 50)]
    public string BindSystem { get; set; }

    /// <summary>
    /// 允许IP
    /// </summary>
    [SugarColumn(ColumnDescription = "允许IP", Length = 200, IsNullable = true)]
    public string AllowedIp { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int Enabled { get; set; } = 1;
}

/// <summary>
/// 文件监听配置表
/// </summary>
[SugarTable("ListenerConfigs", "文件监听配置表")]
public class ListenerConfig : EntityBaseTenantDel
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(MineId))]
    public CoalMine CoalMine { get; set; }

    /// <summary>
    /// 监听类型: 1-本地目录, 2-FTP
    /// </summary>
    [SugarColumn(ColumnDescription = "监听类型", DefaultValue = "1")]
    public int ListenerType { get; set; } = 1;

    /// <summary>
    /// 监听路径/FTP根目录
    /// </summary>
    [SugarColumn(ColumnDescription = "监听路径", Length = 500)]
    public string ListenPath { get; set; }

    /// <summary>
    /// FTP主机地址(监听类型为2时使用)
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP主机地址", Length = 100, IsNullable = true)]
    public string Host { get; set; }

    /// <summary>
    /// FTP端口
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP端口", DefaultValue = "21")]
    public int Port { get; set; } = 21;

    /// <summary>
    /// FTP用户名
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP用户名", Length = 50, IsNullable = true)]
    public string Username { get; set; }

    /// <summary>
    /// FTP密码
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP密码", Length = 100, IsNullable = true)]
    public string Password { get; set; }

    /// <summary>
    /// 数据类型:CDSS/RYSS/KYCDSS/CGKCDSS等
    /// </summary>
    [SugarColumn(ColumnDescription = "数据类型", Length = 50)]
    public string DataType { get; set; }

    /// <summary>
    /// 绑定系统:安全监测/人员定位/矿压监测/水文监测
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定系统", Length = 50)]
    public string BindSystem { get; set; }

    /// <summary>
    /// 文件匹配模式
    /// </summary>
    [SugarColumn(ColumnDescription = "文件匹配模式", Length = 100, IsNullable = true)]
    public string FilePattern { get; set; }

    /// <summary>
    /// 字段分隔符(默认 ;)
    /// </summary>
    [SugarColumn(ColumnDescription = "字段分隔符", Length = 10, DefaultValue = ";")]
    public string FieldSeparator { get; set; } = ";";

    /// <summary>
    /// 记录分隔符(默认 ~)
    /// </summary>
    [SugarColumn(ColumnDescription = "记录分隔符", Length = 10, DefaultValue = "~")]
    public string RecordSeparator { get; set; } = "~";

    /// <summary>
    /// 处理后文件移动目录(为空则删除)
    /// </summary>
    [SugarColumn(ColumnDescription = "处理后文件移动目录", Length = 500, IsNullable = true)]
    public string ProcessedPath { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用", DefaultValue = "1")]
    public int Enabled { get; set; } = 1;
}

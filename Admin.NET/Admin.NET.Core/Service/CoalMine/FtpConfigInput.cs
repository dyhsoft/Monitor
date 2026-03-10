namespace Admin.NET.Core;

/// <summary>
/// FTP配置输入参数
/// </summary>
public class AddFtpConfigInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [Required]
    public long MineId { get; set; }

    /// <summary>
    /// FTP用户名
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// 根目录
    /// </summary>
    [Required]
    public string RootDirectory { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    [Required]
    public string BindSystem { get; set; }

    /// <summary>
    /// 允许IP
    /// </summary>
    public string AllowedIp { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; } = 1;
}

/// <summary>
/// FTP配置更新输入参数
/// </summary>
public class UpdateFtpConfigInput : AddFtpConfigInput
{
    /// <summary>
    /// Id
    /// </summary>
    [Required]
    public long Id { get; set; }
}

/// <summary>
/// FTP配置分页查询参数
/// </summary>
public class PageFtpConfigInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    public string BindSystem { get; set; }
}

/// <summary>
/// FTP配置输出参数
/// </summary>
public class FtpConfigOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// FTP用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 根目录
    /// </summary>
    public string RootDirectory { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    public string BindSystem { get; set; }

    /// <summary>
    /// 允许IP
    /// </summary>
    public string AllowedIp { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
}

/// <summary>
/// 文件监听配置输入参数
/// </summary>
public class AddListenerConfigInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [Required]
    public long MineId { get; set; }

    /// <summary>
    /// 监听路径
    /// </summary>
    [Required]
    public string ListenPath { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    [Required]
    public string DataType { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    [Required]
    public string BindSystem { get; set; }

    /// <summary>
    /// 文件匹配模式
    /// </summary>
    public string FilePattern { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; } = 1;
}

/// <summary>
/// 文件监听配置更新输入参数
/// </summary>
public class UpdateListenerConfigInput : AddListenerConfigInput
{
    /// <summary>
    /// Id
    /// </summary>
    [Required]
    public long Id { get; set; }
}

/// <summary>
/// 文件监听配置分页查询参数
/// </summary>
public class PageListenerConfigInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    public string BindSystem { get; set; }
}

/// <summary>
/// 文件监听配置输出参数
/// </summary>
public class ListenerConfigOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 监听路径
    /// </summary>
    public string ListenPath { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    public string BindSystem { get; set; }

    /// <summary>
    /// 文件匹配模式
    /// </summary>
    public string FilePattern { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
}

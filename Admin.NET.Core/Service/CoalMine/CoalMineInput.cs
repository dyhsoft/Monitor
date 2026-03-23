// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core;

/// <summary>
/// 煤矿输入参数
/// </summary>
public class AddCoalMineInput
{
    /// <summary>
    /// 煤矿编码
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "煤矿编码", Length = 20)]
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    [Required]
    [SugarColumn(ColumnDescription = "煤矿名称", Length = 100)]
    public string Name { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", Length = 200, IsNullable = true)]
    public string Address { get; set; }

    /// <summary>
    /// FTP根目录
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP根目录", Length = 500, IsNullable = true)]
    public string FtpPath { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据类型", Length = 20, IsNullable = true)]
    public string DataType { get; set; }

    /// <summary>
    /// 是否启用
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
/// 煤矿更新输入参数
/// </summary>
public class UpdateCoalMineInput : AddCoalMineInput
{
    /// <summary>
    /// Id
    /// </summary>
    [Required]
    public long Id { get; set; }
}

/// <summary>
/// 煤矿删除输入参数
/// </summary>
public class DeleteCoalMineInput
{
    /// <summary>
    /// Id
    /// </summary>
    [Required]
    public long Id { get; set; }
}

/// <summary>
/// 煤矿分页查询参数
/// </summary>
public class PageCoalMineInput : BasePageInput
{
    /// <summary>
    /// 煤矿编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// 煤矿输出参数
/// </summary>
public class CoalMineOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// FTP根目录
    /// </summary>
    public string FtpPath { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
}

/// <summary>
/// 区域输入参数
/// </summary>
public class AddCoalMineAreaInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [Required]
    public long MineId { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    [Required]
    public string Code { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 父区域ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 区域类型:1-大巷,2-工作面,3-硐室
    /// </summary>
    public int Type { get; set; } = 1;

    /// <summary>
    /// 大地坐标X
    /// </summary>
    public decimal? X { get; set; }

    /// <summary>
    /// 大地坐标Y
    /// </summary>
    public decimal? Y { get; set; }

    /// <summary>
    /// 深度
    /// </summary>
    public decimal? Z { get; set; }

    /// <summary>
    /// 容纳人数
    /// </summary>
    public int? Capacity { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; } = 1;
}

/// <summary>
/// 区域更新输入参数
/// </summary>
public class UpdateCoalMineAreaInput : AddCoalMineAreaInput
{
    /// <summary>
    /// Id
    /// </summary>
    [Required]
    public long Id { get; set; }
}

/// <summary>
/// 区域删除输入参数
/// </summary>
public class DeleteCoalMineAreaInput
{
    /// <summary>
    /// Id
    /// </summary>
    [Required]
    public long Id { get; set; }
}

/// <summary>
/// 区域分页查询参数
/// </summary>
public class PageCoalMineAreaInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// 区域输出参数
/// </summary>
public class CoalMineAreaOutput
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
    /// 区域编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 父区域ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 区域类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 大地坐标X
    /// </summary>
    public decimal? X { get; set; }

    /// <summary>
    /// 大地坐标Y
    /// </summary>
    public decimal? Y { get; set; }

    /// <summary>
    /// 深度
    /// </summary>
    public decimal? Z { get; set; }

    /// <summary>
    /// 容纳人数
    /// </summary>
    public int? Capacity { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
}

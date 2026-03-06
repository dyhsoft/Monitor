using Admin.NET.EntityFramework.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Admin.NET.Core;
using SqlSugar;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using SqlSugar;

namespace Admin.NET.ApplicationParseLog.Dtos;

/// <summary>
/// 解析日志分页输入
/// </summary>
public class ParseLogPageInput : PageInputBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 解析日志输出
/// </summary>
public class ParseLogOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿Id
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 煤矿编号
    /// </summary>
    public string MineCode { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 源文件路径
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// 文件类型名称
    /// </summary>
    public string FileTypeName { get; set; }

    /// <summary>
    /// 文件编码
    /// </summary>
    public string Encoding { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 源文件内容
    /// </summary>
    public string SourceContent { get; set; }

    /// <summary>
    /// 解析记录数
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// 成功数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 错误数
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// 解析耗时(ms)
    /// </summary>
    public int? ParseTime { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 验证结果
    /// </summary>
    public List<ValidationResult> ValidationResults { get; set; }
}

/// <summary>
/// 验证结果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 行号
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// 验证项目
    /// </summary>
    public string Item { get; set; }

    /// <summary>
    /// 验证结果
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; }
}

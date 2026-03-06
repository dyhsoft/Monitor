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
using Admin.NET.ApplicationParseLog.Dtos;
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.ApplicationParseLog.Services;

public class ParseLogService : IParseLogService, ITransient
{
    private readonly ISqlSugarClient _db;

    public ParseLogService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<ParseLogOutput>> GetPage(ParseLogPageInput input)
    {
        return await _db.Queryable<ParseLog>()
            .LeftJoin<CoalMine>((p, c) => p.MineId == c.Id)
            .Where((p, c) => input.MineId == null || p.MineId == input.MineId)
            .Where((p, c) => string.IsNullOrEmpty(input.FileName) || p.FileName.Contains(input.FileName))
            .Where((p, c) => string.IsNullOrEmpty(input.FileType) || p.FileType == input.FileType)
            .Where((p, c) => input.Status == null || p.Status == input.Status)
            .Where((p, c) => input.StartTime == null || p.CreateTime >= input.StartTime)
            .Where((p, c) => input.EndTime == null || p.CreateTime <= input.EndTime)
            .Select((p, c) => new ParseLogOutput
            {
                Id = p.Id,
                MineId = p.MineId,
                MineName = c.Name,
                MineCode = p.MineCode,
                FileName = p.FileName,
                FilePath = p.FilePath,
                FileType = p.FileType,
                FileTypeName = GetFileTypeName(p.FileType),
                Encoding = p.Encoding,
                FileSize = p.FileSize,
                SourceContent = p.SourceContent,
                RecordCount = p.RecordCount,
                SuccessCount = p.SuccessCount,
                ErrorCount = p.ErrorCount,
                ParseTime = p.ParseTime,
                ErrorMessage = p.ErrorMessage,
                Status = p.Status,
                CreateTime = p.CreateTime
            })
            .OrderBy(p => p.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 分页查询错误记录
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<ParseLogOutput>> GetErrorPage(ParseLogPageInput input)
    {
        return await _db.Queryable<ParseLog>()
            .LeftJoin<CoalMine>((p, c) => p.MineId == c.Id)
            .Where((p, c) => p.Status == 2) // 只查询失败的
            .Where((p, c) => input.MineId == null || p.MineId == input.MineId)
            .Where((p, c) => string.IsNullOrEmpty(input.FileName) || p.FileName.Contains(input.FileName))
            .Where((p, c) => string.IsNullOrEmpty(input.FileType) || p.FileType == input.FileType)
            .Where((p, c) => input.StartTime == null || p.CreateTime >= input.StartTime)
            .Where((p, c) => input.EndTime == null || p.CreateTime <= input.EndTime)
            .Select((p, c) => new ParseLogOutput
            {
                Id = p.Id,
                MineId = p.MineId,
                MineName = c.Name,
                MineCode = p.MineCode,
                FileName = p.FileName,
                FilePath = p.FilePath,
                FileType = p.FileType,
                FileTypeName = GetFileTypeName(p.FileType),
                Encoding = p.Encoding,
                FileSize = p.FileSize,
                SourceContent = p.SourceContent,
                RecordCount = p.RecordCount,
                SuccessCount = p.SuccessCount,
                ErrorCount = p.ErrorCount,
                ParseTime = p.ParseTime,
                ErrorMessage = p.ErrorMessage,
                Status = p.Status,
                CreateTime = p.CreateTime
            })
            .OrderBy(p => p.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    [HttpGet]
    public async Task<ParseLogOutput> Get(long id)
    {
        var log = await _db.Queryable<ParseLog>()
            .LeftJoin<CoalMine>((p, c) => p.MineId == c.Id)
            .Where((p, c) => p.Id == id)
            .Select((p, c) => new ParseLogOutput
            {
                Id = p.Id,
                MineId = p.MineId,
                MineName = c.Name,
                MineCode = p.MineCode,
                FileName = p.FileName,
                FilePath = p.FilePath,
                FileType = p.FileType,
                FileTypeName = GetFileTypeName(p.FileType),
                Encoding = p.Encoding,
                FileSize = p.FileSize,
                SourceContent = p.SourceContent,
                RecordCount = p.RecordCount,
                SuccessCount = p.SuccessCount,
                ErrorCount = p.ErrorCount,
                ParseTime = p.ParseTime,
                ErrorMessage = p.ErrorMessage,
                Status = p.Status,
                CreateTime = p.CreateTime
            })
            .FirstAsync();

        // 进行协议验证
        if (log != null && !string.IsNullOrEmpty(log.SourceContent))
        {
            log.ValidationResults = ValidateContent(log.FileType, log.SourceContent, log.MineCode);
        }

        return log;
    }

    /// <summary>
    /// 验证文件是否符合协议
    /// </summary>
    [HttpGet]
    public async Task<List<ValidationResult>> ValidateFile(long id)
    {
        var log = await _db.Queryable<ParseLog>().Where(p => p.Id == id).FirstAsync();
        if (log == null || string.IsNullOrEmpty(log.SourceContent))
            return new List<ValidationResult>();

        return ValidateContent(log.FileType, log.SourceContent, log.MineCode);
    }

    /// <summary>
    /// 验证文件内容是否符合协议规范
    /// </summary>
    private List<ValidationResult> ValidateContent(string fileType, string content, string mineCode)
    {
        var results = new List<ValidationResult>();
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 验证第1行：文件头
        if (lines.Length == 0)
        {
            results.Add(new ValidationResult { LineNumber = 0, Item = "文件头", IsValid = false, Message = "文件为空" });
            return results;
        }

        var lineNum = 1;
        var headerParts = lines[0].Split(new[] { ';', '~' });
        
        // 验证煤矿编号（10位）
        if (headerParts.Length > 0)
        {
            var code = headerParts[0].Trim();
            var isValidCode = !string.IsNullOrEmpty(code) && code.Length == 10 && code.All(char.IsDigit);
            results.Add(new ValidationResult
            {
                LineNumber = lineNum,
                Item = "煤矿编号",
                IsValid = isValidCode,
                Message = isValidCode ? $"格式正确: {code}" : $"格式错误: 应为10位数字，当前为 '{code}'"
            });
        }

        // 验证煤矿名称
        if (headerParts.Length > 1)
        {
            var name = headerParts[1].Trim();
            results.Add(new ValidationResult
            {
                LineNumber = lineNum,
                Item = "煤矿名称",
                IsValid = !string.IsNullOrEmpty(name),
                Message = string.IsNullOrEmpty(name) ? "煤矿名称不能为空" : $"格式正确: {name}"
            });
        }

        // 验证数据时间
        if (headerParts.Length > 2)
        {
            var timeStr = headerParts[2].Trim();
            var isValidTime = DateTime.TryParse(timeStr, out var dataTime);
            results.Add(new ValidationResult
            {
                LineNumber = lineNum,
                Item = "数据时间",
                IsValid = isValidTime,
                Message = isValidTime ? $"格式正确: {timeStr}" : $"时间格式错误: {timeStr}"
            });
        }

        // 验证数据类型
        var validFileTypes = new[] { "CDSS", "CDDY", "FZSS", "KGBH", "TJSJ", "YCBJ", "RYSS", "RYCS", "RYCY", "RYQJ", "JZSS", "CGKCDSS", "CGKCDDY", "JSLCDSS", "PSLCDSS", "KYCDSS", "KYCDDY" };
        results.Add(new ValidationResult
        {
            LineNumber = lineNum,
            Item = "数据类型",
            IsValid = validFileTypes.Contains(fileType),
            Message = validFileTypes.Contains(fileType) ? $"正确: {fileType}" : $"未知类型: {fileType}"
        });

        // 验证数据行
        if (lines.Length > 1)
        {
            // 检查第2行是否是字段定义
            var fieldLine = lines[1];
            var isFieldDefinition = fieldLine.Contains("~");
            
            results.Add(new ValidationResult
            {
                LineNumber = 2,
                Item = "字段定义",
                IsValid = isFieldDefinition,
                Message = isFieldDefinition ? "字段定义存在" : "缺少字段定义行"
            });

            // 验证数据行
            var minFields = 0;
            if (fileType == "CDSS" || fileType == "CDDY" || fileType == "KYCDSS") minFields = 4;
            else if (fileType == "RYSS" || fileType == "RYCS") minFields = 6;
            else if (fileType == "CGKCDSS" || fileType == "CGKCDDY") minFields = 6;

            for (int i = 2; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                
                var fields = lines[i].Split('~');
                results.Add(new ValidationResult
                {
                    LineNumber = i + 1,
                    Item = $"数据行{i-1}",
                    IsValid = fields.Length >= minFields,
                    Message = fields.Length >= minFields ? $"字段数正确: {fields.Length}" : $"字段数不足: 期望{minFields}个，实际{fields.Length}个"
                });
            }
        }

        // 验证记录数
        results.Add(new ValidationResult
        {
            LineNumber = lines.Length,
            Item = "数据统计",
            IsValid = true,
            Message = $"共 {lines.Length - 2} 行数据"
        });

        return results;
    }

    /// <summary>
    /// 获取文件类型名称
    /// </summary>
    private string GetFileTypeName(string fileType)
    {
        var dict = new Dictionary<string, string>
        {
            { "CDSS", "安全监测实时数据" },
            { "CDDY", "报警数据" },
            { "FZSS", "负压数据" },
            { "KGBH", "开关报警" },
            { "TJSJ", "提升数据" },
            { "YCBJ", "远程预警" },
            { "RYSS", "人员实时定位" },
            { "RYCS", "人员初始化" },
            { "RYCY", "人员出勤" },
            { "RYQJ", "区域人数" },
            { "JZSS", "基站状态" },
            { "CGKCDSS", "水仓水位监测" },
            { "CGKCDDY", "水害报警" },
            { "JSLCDSS", "胶带流水量" },
            { "PSLCDSS", "排水量监测" },
            { "KYCDSS", "矿压监测数据" },
            { "KYCDDY", "矿压报警" }
        };
        return dict.GetValueOrDefault(fileType, fileType);
    }

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete]
    public async Task Delete(long id)
    {
        await _db.Deleteable<ParseLog>().Where(p => p.Id == id).ExecuteCommandAsync();
    }
}

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
using SqlSugar;

namespace Admin.NET.ApplicationParseLog.Services;

public interface IParseLogService
{
    Task<SqlSugarPagedList<ParseLogOutput>> GetPage(ParseLogPageInput input);
    Task<SqlSugarPagedList<ParseLogOutput>> GetErrorPage(ParseLogPageInput input);
    Task<ParseLogOutput> Get(long id);
    Task<List<ValidationResult>> ValidateFile(long id);
    Task Delete(long id);
}

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
using Admin.NET.ApplicationPersonInfo.Dtos;
using SqlSugar;

namespace Admin.NET.ApplicationPersonInfo.Services;

public interface IPersonInfoService
{
    Task<SqlSugarPagedList<PersonInfoOutput>> GetPage(PersonInfoPageInput input);
    Task<PersonInfo> Get(long id);
    Task<long> Add(PersonInfoInput input);
    Task Update(PersonInfoInput input);
    Task Delete(long id);
}

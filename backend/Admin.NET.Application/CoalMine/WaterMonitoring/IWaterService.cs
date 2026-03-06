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
using Admin.NET.ApplicationWaterMonitoring.Dtos;
using SqlSugar;

namespace Admin.NET.ApplicationWaterMonitoring.Services;

public interface IWaterService
{
    Task<SqlSugarPagedList<WaterOutput>> GetRealtimePage(WaterPageInput input);
    Task<WaterRealtime> GetRealtime(long id);
    Task<List<WaterOutput>> GetAlarmList(long? mineId);
    Task<int> BatchSaveRealtime(List<WaterRealtime> list);
}

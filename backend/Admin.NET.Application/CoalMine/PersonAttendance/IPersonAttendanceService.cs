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
using Admin.NET.ApplicationPersonAttendance.Dtos;
using SqlSugar;

namespace Admin.NET.ApplicationPersonAttendance.Services;

public interface IPersonAttendanceService
{
    Task<SqlSugarPagedList<PersonAttendanceOutput>> GetPage(PersonAttendancePageInput input);
    Task<PersonAttendance> Get(long id);
    Task<long> Add(PersonAttendanceInput input);
    Task Update(PersonAttendanceInput input);
    Task<AttendanceStatisticsOutput> GetStatistics(long? mineId, DateTime? date);
}

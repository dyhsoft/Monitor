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
namespace Admin.NET.ApplicationPersonAttendance.Dtos;

public class PersonAttendanceInput
{
    public long MineId { get; set; }
    public long PersonInfoId { get; set; }
    public string CardId { get; set; }
    public string PersonName { get; set; }
    public string Department { get; set; }
    public string WorkType { get; set; }
    public DateTime? InTime { get; set; }
    public DateTime? OutTime { get; set; }
    public int? WorkDuration { get; set; }
    public int Status { get; set; }
    public DateTime AttendanceDate { get; set; }
}

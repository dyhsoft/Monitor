using Admin.NET.Application.CoalMine.PersonAttendance.Dtos;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.PersonAttendance.Services;

public interface IPersonAttendanceService
{
    Task<SqlSugarPagedList<PersonAttendanceOutput>> GetPage(PersonAttendancePageInput input);
    Task<PersonAttendance> Get(long id);
    Task<long> Add(PersonAttendanceInput input);
    Task Update(PersonAttendanceInput input);
    Task<AttendanceStatisticsOutput> GetStatistics(long? mineId, DateTime? date);
}

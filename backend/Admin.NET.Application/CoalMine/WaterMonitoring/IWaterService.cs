using Admin.NET.Application.CoalMine.WaterMonitoring.Dtos;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.WaterMonitoring.Services;

public interface IWaterService
{
    Task<SqlSugarPagedList<WaterOutput>> GetRealtimePage(WaterPageInput input);
    Task<WaterRealtime> GetRealtime(long id);
    Task<List<WaterOutput>> GetAlarmList(long? mineId);
    Task<int> BatchSaveRealtime(List<WaterRealtime> list);
}

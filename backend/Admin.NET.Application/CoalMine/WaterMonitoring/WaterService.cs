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
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.WaterMonitoring.Services;

[ApiDescriptionSettings("CoalMine", Name = "Water", Freeze = true)]
public class WaterService : IWaterService, ITransient
{
    private readonly ISqlSugarClient _db;

    public WaterService(ISqlSugarClient db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<SqlSugarPagedList<WaterOutput>> GetRealtimePage(WaterPageInput input)
    {
        return await _db.Queryable<WaterRealtime>()
            .LeftJoin<CoalMine>((w, m) => w.MineId == m.Id)
            .WhereIF(input.MineId.HasValue, (w, m) => w.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.SensorCode), (w, m) => w.SensorCode.Contains(input.SensorCode))
            .WhereIF(!string.IsNullOrEmpty(input.SensorName), (w, m) => w.SensorName.Contains(input.SensorName))
            .WhereIF(input.Status.HasValue, (w, m) => w.Status == input.Status)
            .Select((w, m) => new WaterOutput
            {
                Id = w.Id,
                MineId = w.MineId,
                MineName = m.Name,
                SensorCode = w.SensorCode,
                SensorName = w.SensorName,
                WaterLevel = w.WaterLevel,
                FlowRate = w.FlowRate,
                Temperature = w.Temperature,
                Status = w.Status,
                StatusName = w.Status == 0 ? "正常" : "报警",
                UpdateTime = w.UpdateTime
            })
            .OrderBy(w => w.UpdateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    public async Task<WaterRealtime> GetRealtime(long id)
    {
        return await _db.Queryable<WaterRealtime>().Where(w => w.Id == id).FirstAsync();
    }

    public async Task<List<WaterOutput>> GetAlarmList(long? mineId)
    {
        return await _db.Queryable<WaterRealtime>()
            .LeftJoin<CoalMine>((w, m) => w.MineId == m.Id)
            .Where((w, m) => w.Status == 1)
            .WhereIF(mineId.HasValue, (w, m) => w.MineId == mineId)
            .Select((w, m) => new WaterOutput
            {
                Id = w.Id,
                MineId = w.MineId,
                MineName = m.Name,
                SensorCode = w.SensorCode,
                SensorName = w.SensorName,
                WaterLevel = w.WaterLevel,
                Status = w.Status,
                StatusName = "报警",
                UpdateTime = w.UpdateTime
            })
            .ToListAsync();
    }

    public async Task<int> BatchSaveRealtime(List<WaterRealtime> list)
    {
        if (list == null || list.Count == 0) return 0;
        int count = 0;
        foreach (var item in list)
        {
            var existing = await _db.Queryable<WaterRealtime>()
                .Where(w => w.MineId == item.MineId && w.SensorCode == item.SensorCode)
                .FirstAsync();
            if (existing != null)
            {
                existing.WaterLevel = item.WaterLevel;
                existing.FlowRate = item.FlowRate;
                existing.Temperature = item.Temperature;
                existing.Status = item.Status;
                existing.UpdateTime = item.UpdateTime;
                await _db.Updateable(existing).ExecuteCommandAsync();
            }
            else
            {
                await _db.Insertable(item).ExecuteReturnIdentityAsync();
            }
            count++;
        }
        return count;
    }
}

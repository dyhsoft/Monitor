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
using Admin.NET.ApplicationPersonLocation.Dtos;
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.PersonLocation.Services;

/// <summary>
/// 人员定位服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "Person", Freeze = true)]
public class PersonService : IPersonService, ITransient
{
    private readonly ISqlSugarClient _db;

    public PersonService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询实时位置
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<PersonLocationOutput>> GetRealtimePage(PersonLocationPageInput input)
    {
        return await _db.Queryable<PersonLocation>()
            .LeftJoin<CoalMine>((p, m) => p.MineId == m.Id)
            .WhereIF(input.MineId.HasValue, (p, m) => p.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.PersonName), (p, m) => p.PersonName.Contains(input.PersonName))
            .WhereIF(!string.IsNullOrEmpty(input.CardId), (p, m) => p.CardId.Contains(input.CardId))
            .WhereIF(!string.IsNullOrEmpty(input.StationId), (p, m) => p.StationId.Contains(input.StationId))
            .Select((p, m) => new PersonLocationOutput
            {
                Id = p.Id,
                MineId = p.MineId,
                MineName = m.Name,
                CardId = p.CardId,
                PersonName = p.PersonName,
                StationId = p.StationId,
                StationName = p.StationName,
                AreaCode = p.AreaCode,
                AreaName = p.AreaName,
                X = p.X,
                Y = p.Y,
                Z = p.Z,
                InTime = p.InTime,
                UpdateTime = p.UpdateTime
            })
            .OrderBy(p => p.UpdateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取实时位置详情
    /// </summary>
    public async Task<PersonLocation> GetRealtime(long id)
    {
        return await _db.Queryable<PersonLocation>()
            .Where(p => p.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 获取区域人员统计
    /// </summary>
    public async Task<List<AreaPersonStatisticsOutput>> GetAreaStatistics(long? mineId)
    {
        var list = await _db.Queryable<PersonLocation>()
            .WhereIF(mineId.HasValue, p => p.MineId == mineId)
            .GroupBy(p => new { p.AreaCode, p.AreaName })
            .Select(p => new AreaPersonStatisticsOutput
            {
                AreaCode = p.AreaCode,
                AreaName = p.AreaName,
                PersonCount = SqlFunc.Count(p.Id)
            })
            .ToListAsync();

        return list;
    }

    /// <summary>
    /// 获取某区域人员列表
    /// </summary>
    public async Task<List<PersonLocationOutput>> GetPersonListByArea(long mineId, string areaCode)
    {
        return await _db.Queryable<PersonLocation>()
            .LeftJoin<CoalMine>((p, m) => p.MineId == m.Id)
            .Where((p, m) => p.MineId == mineId && p.AreaCode == areaCode)
            .Select((p, m) => new PersonLocationOutput
            {
                Id = p.Id,
                MineId = p.MineId,
                MineName = m.Name,
                CardId = p.CardId,
                PersonName = p.PersonName,
                StationId = p.StationId,
                StationName = p.StationName,
                AreaCode = p.AreaCode,
                AreaName = p.AreaName,
                X = p.X,
                Y = p.Y,
                Z = p.Z,
                InTime = p.InTime,
                UpdateTime = p.UpdateTime
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取历史轨迹
    /// </summary>
    public async Task<List<Dictionary<string, object>>> GetTrackHistory(long mineId, string cardId, DateTime startTime, DateTime endTime)
    {
        var list = await _db.Queryable<PersonHistory>()
            .Where(p => p.MineId == mineId && p.CardId == cardId)
            .Where(p => p.InTime >= startTime && p.InTime <= endTime)
            .OrderBy(p => p.InTime)
            .Select(p => new { p.StationId, p.StationName, p.AreaName, p.InTime, p.OutTime, p.Duration })
            .ToListAsync();

        return list.Select(s => new Dictionary<string, object>
        {
            { "stationId", s.StationId },
            { "stationName", s.StationName },
            { "areaName", s.AreaName },
            { "inTime", s.InTime?.ToString("yyyy-MM-dd HH:mm:ss") },
            { "outTime", s.OutTime?.ToString("yyyy-MM-dd HH:mm:ss") },
            { "duration", s.Duration ?? 0 }
        }).ToList();
    }

    /// <summary>
    /// 批量保存实时位置
    /// </summary>
    public async Task<int> BatchSaveRealtime(List<PersonLocation> list)
    {
        if (list == null || list.Count == 0) return 0;

        int count = 0;
        foreach (var item in list)
        {
            var existing = await _db.Queryable<PersonLocation>()
                .Where(p => p.MineId == item.MineId && p.CardId == item.CardId)
                .FirstAsync();

            if (existing != null)
            {
                existing.StationId = item.StationId;
                existing.StationName = item.StationName;
                existing.AreaCode = item.AreaCode;
                existing.AreaName = item.AreaName;
                existing.X = item.X;
                existing.Y = item.Y;
                existing.Z = item.Z;
                existing.UpdateTime = item.UpdateTime;
                await _db.Updateable(existing).ExecuteCommandAsync();
            }
            else
            {
                item.InTime = item.InTime ?? DateTime.Now;
                await _db.Insertable(item).ExecuteReturnIdentityAsync();
            }
            count++;
        }

        return count;
    }
}

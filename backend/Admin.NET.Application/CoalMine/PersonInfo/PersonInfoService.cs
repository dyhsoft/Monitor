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
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.PersonInfo.Services;

/// <summary>
/// 人员信息服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "PersonInfo")]
public class PersonInfoService : IPersonInfoService, ITransient
{
    private readonly ISqlSugarClient _db;

    public PersonInfoService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<PersonInfoOutput>> GetPage(PersonInfoPageInput input)
    {
        return await _db.Queryable<PersonInfo>()
            .LeftJoin<CoalMine>((p, c) => p.MineId == c.Id)
            .Where((p, c) => input.MineId == null || p.MineId == input.MineId)
            .Where((p, c) => string.IsNullOrEmpty(input.PersonName) || p.PersonName.Contains(input.PersonName))
            .Where((p, c) => string.IsNullOrEmpty(input.CardId) || p.CardId.Contains(input.CardId))
            .Where((p, c) => string.IsNullOrEmpty(input.Department) || p.Department.Contains(input.Department))
            .Where((p, c) => input.Status == null || p.Status == input.Status)
            .Select((p, c) => new PersonInfoOutput
            {
                Id = p.Id,
                MineId = p.MineId,
                MineName = c.Name,
                PersonName = p.PersonName,
                CardId = p.CardId,
                Department = p.Department,
                WorkType = p.WorkType,
                Position = p.Position,
                Phone = p.Phone,
                IdCard = p.IdCard,
                PhotoPath = p.PhotoPath,
                Status = p.Status,
                Remark = p.Remark,
                CreateTime = p.CreateTime
            })
            .OrderBy(p => p.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    [HttpGet]
    public async Task<PersonInfo> Get(long id)
    {
        return await _db.Queryable<PersonInfo>().Where(p => p.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    public async Task<long> Add(PersonInfoInput input)
    {
        var entity = new PersonInfo
        {
            MineId = input.MineId,
            PersonName = input.PersonName,
            CardId = input.CardId,
            Department = input.Department,
            WorkType = input.WorkType,
            Position = input.Position,
            Phone = input.Phone,
            IdCard = input.IdCard,
            PhotoPath = input.PhotoPath,
            Status = input.Status,
            Remark = input.Remark
        };
        return await _db.Insertable(entity).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新
    /// </summary>
    [HttpPost]
    public async Task Update(PersonInfoInput input)
    {
        var entity = await _db.Queryable<PersonInfo>().Where(p => p.Id == input.Id).FirstAsync();
        if (entity == null) throw new Exception("记录不存在");

        entity.MineId = input.MineId;
        entity.PersonName = input.PersonName;
        entity.CardId = input.CardId;
        entity.Department = input.Department;
        entity.WorkType = input.WorkType;
        entity.Position = input.Position;
        entity.Phone = input.Phone;
        entity.IdCard = input.IdCard;
        entity.PhotoPath = input.PhotoPath;
        entity.Status = input.Status;
        entity.Remark = input.Remark;

        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete]
    public async Task Delete(long id)
    {
        await _db.Deleteable<PersonInfo>().Where(p => p.Id == id).ExecuteCommandAsync();
    }
}

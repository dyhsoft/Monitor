using Admin.NET.Application.CoalMine.Dtos;
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.Services;

/// <summary>
/// 煤矿管理服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "CoalMine", Freeze = true)]
public class CoalMineService : ICoalMineService, ITransient
{
    private readonly ISqlSugarClient _db;

    public CoalMineService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询煤矿
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<CoalMine>> GetPage(CoalMinePageInput input)
    {
        return await _db.Queryable<CoalMine>()
            .WhereIF(!string.IsNullOrEmpty(input.Code), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrEmpty(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrEmpty(input.GroupName), u => u.GroupName.Contains(input.GroupName))
            .WhereIF(!string.IsNullOrEmpty(input.MineType), u => u.MineType == input.MineType)
            .WhereIF(input.Status.HasValue, u => u.Status == input.Status)
            .OrderBy(u => u.Code)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取煤矿详情
    /// </summary>
    public async Task<CoalMine> Get(long id)
    {
        return await _db.Queryable<CoalMine>()
            .Where(u => u.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 添加煤矿
    /// </summary>
    public async Task<long> Add(AddCoalMineInput input)
    {
        var entity = new CoalMine
        {
            Code = input.Code,
            Name = input.Name,
            GroupName = input.GroupName,
            Province = input.Province,
            City = input.City,
            County = input.County,
            MineType = input.MineType,
            DesignCapacity = input.DesignCapacity,
            Contact = input.Contact,
            Phone = input.Phone,
            Address = input.Address,
            Longitude = input.Longitude,
            Latitude = input.Latitude,
            Status = input.Status,
            Remark = input.Remark,
            CreatorId = App.User?.FindFirst(ClaimConst.CLAIM_USERID)?.Value.ParseToLong(),
            CreateTime = DateTimeOffset.Now
        };

        // 检查编码是否已存在
        var exists = await _db.Queryable<CoalMine>()
            .Where(u => u.Code == input.Code)
            .AnyAsync();
        if (exists)
            throw Oops.Bah($"煤矿编码 {input.Code} 已存在");

        return await _db.Insertable(entity).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新煤矿
    /// </summary>
    public async Task Update(UpdateCoalMineInput input)
    {
        var entity = await _db.Queryable<CoalMine>()
            .Where(u => u.Id == input.Id)
            .FirstAsync();

        if (entity == null)
            throw Oops.Bah("煤矿不存在");

        // 检查编码是否与其他记录冲突
        if (input.Code != entity.Code)
        {
            var exists = await _db.Queryable<CoalMine>()
                .Where(u => u.Code == input.Code && u.Id != input.Id)
                .AnyAsync();
            if (exists)
                throw Oops.Bah($"煤矿编码 {input.Code} 已存在");
        }

        entity.Code = input.Code;
        entity.Name = input.Name;
        entity.GroupName = input.GroupName;
        entity.Province = input.Province;
        entity.City = input.City;
        entity.County = input.County;
        entity.MineType = input.MineType;
        entity.DesignCapacity = input.DesignCapacity;
        entity.Contact = input.Contact;
        entity.Phone = input.Phone;
        entity.Address = input.Address;
        entity.Longitude = input.Longitude;
        entity.Latitude = input.Latitude;
        entity.Status = input.Status;
        entity.Remark = input.Remark;
        entity.ModifierId = App.User?.FindFirst(ClaimConst.CLAIM_USERID)?.Value.ParseToLong();
        entity.ModifyTime = DateTimeOffset.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除煤矿
    /// </summary>
    public async Task Delete(DeleteCoalMineInput input)
    {
        // 检查是否有关联的网关配置
        var hasGateway = await _db.Queryable<CoalGatewayConfig>()
            .Where(u => u.MineId == input.Id)
            .AnyAsync();
        if (hasGateway)
            throw Oops.Bah("该煤矿下存在网关配置，无法删除");

        await _db.Deleteable<CoalMine>(input.Id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取煤矿列表（下拉选项）
    /// </summary>
    public async Task<List<SelectOption>> GetSelect()
    {
        return await _db.Queryable<CoalMine>()
            .Where(u => u.Status == 1)
            .OrderBy(u => u.Code)
            .Select(u => new SelectOption
            {
                Label = u.Name,
                Value = u.Id.ToString()
            })
            .ToListAsync();
    }
}

/// <summary>
/// 下拉选项
/// </summary>
public class SelectOption
{
    /// <summary>
    /// 显示文本
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }
}

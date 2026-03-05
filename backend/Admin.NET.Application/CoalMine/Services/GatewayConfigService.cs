using Admin.NET.Application.CoalMine.Dtos;
using Admin.NET.Core;
using Furion.DependencyInjection;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Admin.NET.Application.CoalMine.Services;

/// <summary>
/// 网关配置服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Name = "GatewayConfig", Freeze = true)]
public class GatewayConfigService : IGatewayConfigService, ITransient
{
    private readonly ISqlSugarClient _db;

    public GatewayConfigService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 分页查询网关配置
    /// </summary>
    [HttpPost]
    public async Task<SqlSugarPagedList<CoalGatewayConfig>> GetPage(GatewayConfigPageInput input)
    {
        return await _db.Queryable<CoalGatewayConfig>()
            .LeftJoin<CoalMine>((c, m) => c.MineId == m.Id)
            .WhereIF(input.MineId.HasValue, (c, m) => c.MineId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.GatewayCode), (c, m) => c.GatewayCode.Contains(input.GatewayCode))
            .WhereIF(!string.IsNullOrEmpty(input.GatewayName), (c, m) => c.GatewayName.Contains(input.GatewayName))
            .WhereIF(!string.IsNullOrEmpty(input.GatewayType), (c, m) => c.GatewayType == input.GatewayType)
            .WhereIF(input.Status.HasValue, (c, m) => c.Status == input.Status)
            .Select((c, m) => new CoalGatewayConfig
            {
                Id = c.Id,
                MineId = c.MineId,
                GatewayCode = c.GatewayCode,
                GatewayName = c.GatewayName,
                GatewayType = c.GatewayType,
                FtpHost = c.FtpHost,
                FtpPort = c.FtpPort,
                FtpUsername = c.FtpUsername,
                FtpPassword = c.FtpPassword,
                FtpRootPath = c.FtpRootPath,
                DataPath = c.DataPath,
                FileEncoding = c.FileEncoding,
                PushFrequency = c.PushFrequency,
                AllowedIp = c.AllowedIp,
                Status = c.Status,
                Remark = c.Remark,
                CreateTime = c.CreateTime
            })
            .OrderBy(c => c.MineId)
            .ToPagedListAsync(input.Current, input.Size);
    }

    /// <summary>
    /// 获取网关配置详情
    /// </summary>
    public async Task<CoalGatewayConfig> Get(long id)
    {
        return await _db.Queryable<CoalGatewayConfig>()
            .Where(u => u.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 添加网关配置
    /// </summary>
    public async Task<long> Add(AddGatewayConfigInput input)
    {
        // 检查煤矿是否存在
        var mine = await _db.Queryable<CoalMine>()
            .Where(u => u.Id == input.MineId)
            .FirstAsync();
        if (mine == null)
            throw Oops.Bah("煤矿不存在");

        var entity = new CoalGatewayConfig
        {
            MineId = input.MineId,
            GatewayCode = input.GatewayCode,
            GatewayName = input.GatewayName ?? input.GatewayCode,
            GatewayType = input.GatewayType,
            FtpHost = input.FtpHost,
            FtpPort = input.FtpPort,
            FtpUsername = input.FtpUsername,
            FtpPassword = input.FtpPassword,
            FtpRootPath = input.FtpRootPath,
            DataPath = input.DataPath,
            FileEncoding = input.FileEncoding,
            PushFrequency = input.PushFrequency,
            AllowedIp = input.AllowedIp,
            Status = input.Status,
            Remark = input.Remark,
            CreatorId = App.User?.FindFirst(ClaimConst.CLAIM_USERID)?.Value.ParseToLong(),
            CreateTime = DateTimeOffset.Now
        };

        // 检查网关编号是否已存在
        var exists = await _db.Queryable<CoalGatewayConfig>()
            .Where(u => u.GatewayCode == input.GatewayCode)
            .AnyAsync();
        if (exists)
            throw Oops.Bah($"网关编号 {input.GatewayCode} 已存在");

        return await _db.Insertable(entity).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新网关配置
    /// </summary>
    public async Task Update(UpdateGatewayConfigInput input)
    {
        var entity = await _db.Queryable<CoalGatewayConfig>()
            .Where(u => u.Id == input.Id)
            .FirstAsync();

        if (entity == null)
            throw Oops.Bah("网关配置不存在");

        // 检查煤矿是否存在
        var mine = await _db.Queryable<CoalMine>()
            .Where(u => u.Id == input.MineId)
            .FirstAsync();
        if (mine == null)
            throw Oops.Bah("煤矿不存在");

        // 检查网关编号是否与其他记录冲突
        if (input.GatewayCode != entity.GatewayCode)
        {
            var exists = await _db.Queryable<CoalGatewayConfig>()
                .Where(u => u.GatewayCode == input.GatewayCode && u.Id != input.Id)
                .AnyAsync();
            if (exists)
                throw Oops.Bah($"网关编号 {input.GatewayCode} 已存在");
        }

        entity.MineId = input.MineId;
        entity.GatewayCode = input.GatewayCode;
        entity.GatewayName = input.GatewayName;
        entity.GatewayType = input.GatewayType;
        entity.FtpHost = input.FtpHost;
        entity.FtpPort = input.FtpPort;
        entity.FtpUsername = input.FtpUsername;
        entity.FtpPassword = input.FtpPassword;
        entity.FtpRootPath = input.FtpRootPath;
        entity.DataPath = input.DataPath;
        entity.FileEncoding = input.FileEncoding;
        entity.PushFrequency = input.PushFrequency;
        entity.AllowedIp = input.AllowedIp;
        entity.Status = input.Status;
        entity.Remark = input.Remark;
        entity.ModifierId = App.User?.FindFirst(ClaimConst.CLAIM_USERID)?.Value.ParseToLong();
        entity.ModifyTime = DateTimeOffset.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除网关配置
    /// </summary>
    public async Task Delete(DeleteGatewayConfigInput input)
    {
        await _db.Deleteable<CoalGatewayConfig>(input.Id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取网关列表（下拉选项）
    /// </summary>
    public async Task<List<SelectOption>> GetSelect(long mineId)
    {
        return await _db.Queryable<CoalGatewayConfig>()
            .Where(u => u.MineId == mineId && u.Status == 1)
            .OrderBy(u => u.GatewayCode)
            .Select(u => new SelectOption
            {
                Label = u.GatewayName,
                Value = u.Id.ToString()
            })
            .ToListAsync();
    }
}

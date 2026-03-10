// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 煤矿管理服务
/// </summary>
[ApiDescriptionSettings("CoalMine", Description = "煤矿管理")]
public class CoalMineService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<CoalMine> _coalMineRep;

    public CoalMineService(SqlSugarRepository<CoalMine> coalMineRep)
    {
        _coalMineRep = coalMineRep;
    }

    /// <summary>
    /// 获取煤矿列表
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取煤矿列表")]
    public async Task<List<CoalMineOutput>> GetList([FromQuery] PageCoalMineInput input)
    {
        return await _coalMineRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .OrderBy(u => u.Id)
            .Select<CoalMineOutput>()
            .ToListAsync();
    }

    /// <summary>
    /// 获取煤矿分页列表
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取煤矿分页列表")]
    public async Task<SqlSugarPagedList<CoalMineOutput>> GetPage([FromQuery] PageCoalMineInput input)
    {
        return await _coalMineRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .OrderBy(u => u.Id)
            .Select<CoalMineOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取煤矿详情
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取煤矿详情")]
    public async Task<CoalMineOutput> Get(long id)
    {
        return await _coalMineRep.AsQueryable()
            .Where(u => u.Id == id)
            .Select<CoalMineOutput>()
            .FirstAsync();
    }

    /// <summary>
    /// 新增煤矿
    /// </summary>
    /// <returns></returns>
    [DisplayName("新增煤矿")]
    public async Task<long> Add(AddCoalMineInput input)
    {
        var entity = input.Adapt<CoalMine>();
        entity.Code = entity.Code.ToUpper();
        return await _coalMineRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新煤矿
    /// </summary>
    /// <returns></returns>
    [DisplayName("更新煤矿")]
    public async Task Update(UpdateCoalMineInput input)
    {
        var entity = input.Adapt<CoalMine>();
        entity.Code = entity.Code.ToUpper();
        await _coalMineRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除煤矿
    /// </summary>
    /// <returns></returns>
    [DisplayName("删除煤矿")]
    public async Task Delete(long id)
    {
        await _coalMineRep.DeleteByIdAsync(id);
    }

    /// <summary>
    /// 设置煤矿启用状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="enabled">启用状态</param>
    /// <returns></returns>
    [DisplayName("设置煤矿启用状态")]
    public async Task SetEnabled(long id, int enabled)
    {
        await _coalMineRep.AsUpdateable()
            .SetColumns(u => u.Enabled == enabled)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }
}

/// <summary>
/// 区域管理服务
/// </summary>
[ApiDescriptionSettings("CoalMineArea", Description = "区域管理")]
public class CoalMineAreaService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<CoalMineArea> _coalMineAreaRep;

    public CoalMineAreaService(SqlSugarRepository<CoalMineArea> coalMineAreaRep)
    {
        _coalMineAreaRep = coalMineAreaRep;
    }

    /// <summary>
    /// 获取区域列表
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取区域列表")]
    public async Task<List<CoalMineAreaOutput>> GetList([FromQuery] PageCoalMineAreaInput input)
    {
        return await _coalMineAreaRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new CoalMineAreaOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                Code = u.Code,
                Name = u.Name,
                ParentId = u.ParentId,
                Type = u.Type,
                X = u.X,
                Y = u.Y,
                Z = u.Z,
                Capacity = u.Capacity,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取区域分页列表
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取区域分页列表")]
    public async Task<SqlSugarPagedList<CoalMineAreaOutput>> GetPage([FromQuery] PageCoalMineAreaInput input)
    {
        return await _coalMineAreaRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new CoalMineAreaOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                Code = u.Code,
                Name = u.Name,
                ParentId = u.ParentId,
                Type = u.Type,
                X = u.X,
                Y = u.Y,
                Z = u.Z,
                Capacity = u.Capacity,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取区域详情
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取区域详情")]
    public async Task<CoalMineAreaOutput> Get(long id)
    {
        return await _coalMineAreaRep.AsQueryable()
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .Where(u => u.Id == id)
            .Select((u, m) => new CoalMineAreaOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                Code = u.Code,
                Name = u.Name,
                ParentId = u.ParentId,
                Type = u.Type,
                X = u.X,
                Y = u.Y,
                Z = u.Z,
                Capacity = u.Capacity,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .FirstAsync();
    }

    /// <summary>
    /// 新增区域
    /// </summary>
    /// <returns></returns>
    [DisplayName("新增区域")]
    public async Task<long> Add(AddCoalMineAreaInput input)
    {
        var entity = input.Adapt<CoalMineArea>();
        return await _coalMineAreaRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新区域
    /// </summary>
    /// <returns></returns>
    [DisplayName("更新区域")]
    public async Task Update(UpdateCoalMineAreaInput input)
    {
        var entity = input.Adapt<CoalMineArea>();
        await _coalMineAreaRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除区域
    /// </summary>
    /// <returns></returns>
    [DisplayName("删除区域")]
    public async Task Delete(long id)
    {
        await _coalMineAreaRep.DeleteByIdAsync(id);
    }

    /// <summary>
    /// 设置区域启用状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="enabled">启用状态</param>
    /// <returns></returns>
    [DisplayName("设置区域启用状态")]
    public async Task SetEnabled(long id, int enabled)
    {
        await _coalMineAreaRep.AsUpdateable()
            .SetColumns(u => u.Enabled == enabled)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// FTP配置服务
/// </summary>
[ApiDescriptionSettings("FtpConfig", Description = "FTP配置")]
public class FtpConfigService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<FtpConfig> _ftpConfigRep;

    public FtpConfigService(SqlSugarRepository<FtpConfig> ftpConfigRep)
    {
        _ftpConfigRep = ftpConfigRep;
    }

    /// <summary>
    /// 获取FTP配置列表
    /// </summary>
    [DisplayName("获取FTP配置列表")]
    public async Task<List<FtpConfigOutput>> GetList([FromQuery] PageFtpConfigInput input)
    {
        return await _ftpConfigRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Username), u => u.Username.Contains(input.Username))
            .WhereIF(!string.IsNullOrWhiteSpace(input.BindSystem), u => u.BindSystem == input.BindSystem)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new FtpConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                Host = u.Host,
                Port = u.Port,
                Username = u.Username,
                RootDirectory = u.RootDirectory,
                BindSystem = u.BindSystem,
                AllowedIp = u.AllowedIp,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取FTP配置分页列表
    /// </summary>
    [DisplayName("获取FTP配置分页列表")]
    public async Task<SqlSugarPagedList<FtpConfigOutput>> GetPage([FromQuery] PageFtpConfigInput input)
    {
        return await _ftpConfigRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Username), u => u.Username.Contains(input.Username))
            .WhereIF(!string.IsNullOrWhiteSpace(input.BindSystem), u => u.BindSystem == input.BindSystem)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new FtpConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                Host = u.Host,
                Port = u.Port,
                Username = u.Username,
                RootDirectory = u.RootDirectory,
                BindSystem = u.BindSystem,
                AllowedIp = u.AllowedIp,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取FTP配置详情
    /// </summary>
    [DisplayName("获取FTP配置详情")]
    public async Task<FtpConfigOutput> Get(long id)
    {
        return await _ftpConfigRep.AsQueryable()
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .Where(u => u.Id == id)
            .Select((u, m) => new FtpConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                Host = u.Host,
                Port = u.Port,
                Username = u.Username,
                RootDirectory = u.RootDirectory,
                BindSystem = u.BindSystem,
                AllowedIp = u.AllowedIp,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .FirstAsync();
    }

    /// <summary>
    /// 新增FTP配置
    /// </summary>
    [DisplayName("新增FTP配置")]
    public async Task<long> Add(AddFtpConfigInput input)
    {
        var entity = input.Adapt<FtpConfig>();
        return await _ftpConfigRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新FTP配置
    /// </summary>
    [DisplayName("更新FTP配置")]
    public async Task Update(UpdateFtpConfigInput input)
    {
        var entity = input.Adapt<FtpConfig>();
        await _ftpConfigRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除FTP配置
    /// </summary>
    [DisplayName("删除FTP配置")]
    public async Task Delete(long id)
    {
        await _ftpConfigRep.DeleteByIdAsync(id);
    }

    /// <summary>
    /// 设置FTP配置启用状态
    /// </summary>
    [DisplayName("设置FTP配置启用状态")]
    public async Task SetEnabled(long id, int enabled)
    {
        await _ftpConfigRep.AsUpdateable()
            .SetColumns(u => u.Enabled == enabled)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }
}

/// <summary>
/// 文件监听配置服务
/// </summary>
[ApiDescriptionSettings("ListenerConfig", Description = "文件监听配置")]
public class ListenerConfigService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ListenerConfig> _listenerConfigRep;

    public ListenerConfigService(SqlSugarRepository<ListenerConfig> listenerConfigRep)
    {
        _listenerConfigRep = listenerConfigRep;
    }

    /// <summary>
    /// 获取文件监听配置列表
    /// </summary>
    [DisplayName("获取文件监听配置列表")]
    public async Task<List<ListenerConfigOutput>> GetList([FromQuery] PageListenerConfigInput input)
    {
        return await _listenerConfigRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.DataType), u => u.DataType == input.DataType)
            .WhereIF(!string.IsNullOrWhiteSpace(input.BindSystem), u => u.BindSystem == input.BindSystem)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new ListenerConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                ListenPath = u.ListenPath,
                DataType = u.DataType,
                BindSystem = u.BindSystem,
                FilePattern = u.FilePattern,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取文件监听配置分页列表
    /// </summary>
    [DisplayName("获取文件监听配置分页列表")]
    public async Task<SqlSugarPagedList<ListenerConfigOutput>> GetPage([FromQuery] PageListenerConfigInput input)
    {
        return await _listenerConfigRep.AsQueryable()
            .WhereIF(input.MineId.HasValue, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.DataType), u => u.DataType == input.DataType)
            .WhereIF(!string.IsNullOrWhiteSpace(input.BindSystem), u => u.BindSystem == input.BindSystem)
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .OrderBy(u => u.Id)
            .Select((u, m) => new ListenerConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                ListenPath = u.ListenPath,
                DataType = u.DataType,
                BindSystem = u.BindSystem,
                FilePattern = u.FilePattern,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取文件监听配置详情
    /// </summary>
    [DisplayName("获取文件监听配置详情")]
    public async Task<ListenerConfigOutput> Get(long id)
    {
        return await _listenerConfigRep.AsQueryable()
            .LeftJoin<CoalMine>((u, m) => u.MineId == m.Id)
            .Where(u => u.Id == id)
            .Select((u, m) => new ListenerConfigOutput
            {
                Id = u.Id,
                MineId = u.MineId,
                MineName = m.Name,
                ListenPath = u.ListenPath,
                DataType = u.DataType,
                BindSystem = u.BindSystem,
                FilePattern = u.FilePattern,
                Enabled = u.Enabled,
                CreateTime = u.CreateTime
            })
            .FirstAsync();
    }

    /// <summary>
    /// 新增文件监听配置
    /// </summary>
    [DisplayName("新增文件监听配置")]
    public async Task<long> Add(AddListenerConfigInput input)
    {
        var entity = input.Adapt<ListenerConfig>();
        entity.FilePattern = entity.FilePattern ?? "*.txt";
        return await _listenerConfigRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新文件监听配置
    /// </summary>
    [DisplayName("更新文件监听配置")]
    public async Task Update(UpdateListenerConfigInput input)
    {
        var entity = input.Adapt<ListenerConfig>();
        entity.FilePattern = entity.FilePattern ?? "*.txt";
        await _listenerConfigRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除文件监听配置
    /// </summary>
    [DisplayName("删除文件监听配置")]
    public async Task Delete(long id)
    {
        await _listenerConfigRep.DeleteByIdAsync(id);
    }

    /// <summary>
    /// 设置文件监听配置启用状态
    /// </summary>
    [DisplayName("设置文件监听配置启用状态")]
    public async Task SetEnabled(long id, int enabled)
    {
        await _listenerConfigRep.AsUpdateable()
            .SetColumns(u => u.Enabled == enabled)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }
}

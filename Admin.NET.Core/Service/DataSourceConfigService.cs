// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
namespace Admin.NET.Core;

public interface IDataSourceConfigService
{
    Task<List<DataSourceConfig>> GetEnabledConfigsAsync();
    Task<SqlSugarPagedList<DataSourceConfig>> GetPageAsync(PageDataSourceConfigInput input);
    Task<DataSourceConfig> GetAsync(long id);
    Task AddAsync(DataSourceConfig entity);
    Task UpdateAsync(DataSourceConfig entity);
    Task DeleteAsync(long id);
    Task RefreshConfigsAsync();
}

public class DataSourceConfigService : IDataSourceConfigService
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger<DataSourceConfigService> _logger;
    private List<DataSourceConfig> _cachedConfigs = new();
    private DateTime _lastRefresh = DateTime.MinValue;

    public DataSourceConfigService(
        ISqlSugarClient db,
        ILogger<DataSourceConfigService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    public async Task<SqlSugarPagedList<DataSourceConfig>> GetPageAsync(PageDataSourceConfigInput input)
    {
        return await _db.Queryable<DataSourceConfig>()
            .WhereIF(input.MineId > 0, c => c.FtpConfigId == input.MineId)
            .WhereIF(!string.IsNullOrEmpty(input.SystemType), c => c.SystemType == input.SystemType)
            .OrderBy(c => c.Id)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    public async Task<DataSourceConfig> GetAsync(long id)
    {
        return await _db.Queryable<DataSourceConfig>().Where(c => c.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task AddAsync(DataSourceConfig entity)
    {
        await _db.Insertable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 修改
    /// </summary>
    public async Task UpdateAsync(DataSourceConfig entity)
    {
        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        await _db.Deleteable<DataSourceConfig>().Where(c => c.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取启用的配置（带缓存）
    /// </summary>
    public async Task<List<DataSourceConfig>> GetEnabledConfigsAsync()
    {
        if ((DateTime.Now - _lastRefresh).TotalSeconds > 60)
        {
            await RefreshConfigsAsync();
        }
        return _cachedConfigs.Where(c => c.ParseEnabled).ToList();
    }

    /// <summary>
    /// 刷新配置缓存
    /// </summary>
    public async Task RefreshConfigsAsync()
    {
        _cachedConfigs = await _db.Queryable<DataSourceConfig>()
            .Where(c => c.ParseEnabled)
            .ToListAsync();
        _lastRefresh = DateTime.Now;
        _logger.LogInformation("数据源配置已刷新，共 {Count} 条启用配置", _cachedConfigs.Count);
    }
}

/// <summary>
/// 分页输入
/// </summary>
public class PageDataSourceConfigInput : BasePageInput
{
    public long MineId { get; set; }
    public string? SystemType { get; set; }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规和许可证的要求。
namespace Admin.NET.Core;

/// <summary>
/// 数据源配置管理控制器
/// </summary>
[ApiDescriptionSettings("煤矿管理", Order = 200)]
public class DataSourceConfigController : IDynamicApiController
{
    private readonly IDataSourceConfigService _service;

    public DataSourceConfigController(IDataSourceConfigService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取数据源配置分页列表
    /// </summary>
    [HttpGet]
    public async Task<SqlSugarPagedList<DataSourceConfig>> GetPage([FromQuery] PageDataSourceConfigInput input)
    {
        return await _service.GetPageAsync(input);
    }

    /// <summary>
    /// 获取数据源配置详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<DataSourceConfig> Get(long id)
    {
        return await _service.GetAsync(id);
    }

    /// <summary>
    /// 新增数据源配置
    /// </summary>
    [HttpPost]
    public async Task Add([FromBody] DataSourceConfig entity)
    {
        await _service.AddAsync(entity);
    }

    /// <summary>
    /// 修改数据源配置
    /// </summary>
    [HttpPut]
    public async Task Update([FromBody] DataSourceConfig entity)
    {
        await _service.UpdateAsync(entity);
    }

    /// <summary>
    /// 删除数据源配置
    /// </summary>
    [HttpDelete("{id}")]
    public async Task Delete(long id)
    {
        await _service.DeleteAsync(id);
    }
}

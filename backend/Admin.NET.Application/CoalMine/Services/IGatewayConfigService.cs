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
using Admin.NET.ApplicationDtos;

namespace Admin.NET.ApplicationServices;

/// <summary>
/// 网关配置服务接口
/// </summary>
public interface IGatewayConfigService
{
    /// <summary>
    /// 分页查询网关配置
    /// </summary>
    Task<SqlSugarPagedList<CoalGatewayConfig>> GetPage(GatewayConfigPageInput input);

    /// <summary>
    /// 获取网关配置详情
    /// </summary>
    Task<CoalGatewayConfig> Get(long id);

    /// <summary>
    /// 添加网关配置
    /// </summary>
    Task<long> Add(AddGatewayConfigInput input);

    /// <summary>
    /// 更新网关配置
    /// </summary>
    Task Update(UpdateGatewayConfigInput input);

    /// <summary>
    /// 删除网关配置
    /// </summary>
    Task Delete(DeleteGatewayConfigInput input);

    /// <summary>
    /// 获取网关列表（下拉选项）
    /// </summary>
    Task<List<SelectOption>> GetSelect(long mineId);
}

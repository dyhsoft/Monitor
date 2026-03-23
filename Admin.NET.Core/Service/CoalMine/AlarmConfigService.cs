// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！
using Admin.NET.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 报警配置服务
/// </summary>
[ApiDescriptionSettings("AlarmConfig", Description = "报警配置")]
public class AlarmConfigService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<AlarmConfig> _alarmConfigRep;

    public AlarmConfigService(SqlSugarRepository<AlarmConfig> alarmConfigRep)
    {
        _alarmConfigRep = alarmConfigRep;
    }

    /// <summary>
    /// 分页查询报警配置
    /// </summary>
    [DisplayName("分页查询报警配置")]
    public async Task<SqlSugarPagedList<AlarmConfigOutput>> GetPage([FromQuery] PageAlarmConfigInput input)
    {
        return await _alarmConfigRep.AsQueryable()
            .WhereIF(input.MineId > 0, u => u.MineId == input.MineId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SensorTypeCode), u => u.SensorTypeCode.Contains(input.SensorTypeCode))
            .WhereIF(!string.IsNullOrWhiteSpace(input.AlarmType), u => u.AlarmType.Contains(input.AlarmType))
            .WhereIF(input.AlarmLevel.HasValue, u => u.AlarmLevel == input.AlarmLevel)
            .WhereIF(input.AlarmEnabled.HasValue, u => u.AlarmEnabled == input.AlarmEnabled)
            .OrderBy(u => u.SensorTypeCode)
            .OrderBy(u => u.AlarmType)
            .Select<AlarmConfigOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取报警配置列表
    /// </summary>
    [DisplayName("获取报警配置列表")]
    public async Task<List<AlarmConfigOutput>> GetList([FromQuery] long mineId)
    {
        return await _alarmConfigRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .Where(u => u.AlarmEnabled == 1)
            .OrderBy(u => u.SensorTypeCode)
            .Select<AlarmConfigOutput>()
            .ToListAsync();
    }

    /// <summary>
    /// 获取报警配置详情
    /// </summary>
    [DisplayName("获取报警配置详情")]
    public async Task<AlarmConfigOutput> Get(long id)
    {
        return await _alarmConfigRep.AsQueryable()
            .Where(u => u.Id == id)
            .Select<AlarmConfigOutput>()
            .FirstAsync();
    }

    /// <summary>
    /// 新增报警配置
    /// </summary>
    [DisplayName("新增报警配置")]
    public async Task<long> Add(AddAlarmConfigInput input)
    {
        var entity = input.Adapt<AlarmConfig>();
        return await _alarmConfigRep.InsertReturnIdentityAsync(entity);
    }

    /// <summary>
    /// 更新报警配置
    /// </summary>
    [DisplayName("更新报警配置")]
    public async Task Update(UpdateAlarmConfigInput input)
    {
        var entity = input.Adapt<AlarmConfig>();
        await _alarmConfigRep.AsUpdateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除报警配置
    /// </summary>
    [DisplayName("删除报警配置")]
    public async Task Delete(long id)
    {
        await _alarmConfigRep.DeleteByIdAsync(id);
    }

    /// <summary>
    /// 设置报警配置启用状态
    /// </summary>
    [DisplayName("设置报警配置启用状态")]
    public async Task SetEnabled(long id, int enabled)
    {
        await _alarmConfigRep.AsUpdateable()
            .SetColumns(u => u.AlarmEnabled == enabled)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 复制报警配置到其他煤矿
    /// </summary>
    [DisplayName("复制报警配置")]
    public async Task CopyToMines(long configId, List<long> targetMineIds)
    {
        var sourceConfig = await _alarmConfigRep.AsQueryable()
            .Where(u => u.Id == configId)
            .FirstAsync();

        if (sourceConfig == null)
        {
            throw new Exception("报警配置不存在");
        }

        foreach (var mineId in targetMineIds)
        {
            var newConfig = new AlarmConfig
            {
                MineId = mineId,
                SensorTypeCode = sourceConfig.SensorTypeCode,
                SensorTypeName = sourceConfig.SensorTypeName,
                AlarmType = sourceConfig.AlarmType,
                AlarmLevel = sourceConfig.AlarmLevel,
                ThresholdValue = sourceConfig.ThresholdValue,
                DelaySeconds = sourceConfig.DelaySeconds,
                AlarmEnabled = sourceConfig.AlarmEnabled,
                NotifyUsers = sourceConfig.NotifyUsers,
                Remark = sourceConfig.Remark
            };
            await _alarmConfigRep.InsertAsync(newConfig);
        }
    }
}

/// <summary>
/// 报警配置分页输入
/// </summary>
public class PageAlarmConfigInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 传感器类型代码
    /// </summary>
    public string SensorTypeCode { get; set; }

    /// <summary>
    /// 报警类型
    /// </summary>
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警级别
    /// </summary>
    public int? AlarmLevel { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int? AlarmEnabled { get; set; }
}

/// <summary>
/// 新增报警配置输入
/// </summary>
public class AddAlarmConfigInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 传感器类型代码
    /// </summary>
    public string SensorTypeCode { get; set; }

    /// <summary>
    /// 传感器类型名称
    /// </summary>
    public string SensorTypeName { get; set; }

    /// <summary>
    /// 报警类型:上限/下限/突变/断线
    /// </summary>
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警级别:1-一般,2-重要,3-紧急
    /// </summary>
    public int AlarmLevel { get; set; } = 1;

    /// <summary>
    /// 阈值
    /// </summary>
    public decimal ThresholdValue { get; set; }

    /// <summary>
    /// 延迟时间(秒)
    /// </summary>
    public int DelaySeconds { get; set; } = 0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public int AlarmEnabled { get; set; } = 1;

    /// <summary>
    /// 通知人员
    /// </summary>
    public string NotifyUsers { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

/// <summary>
/// 更新报警配置输入
/// </summary>
public class UpdateAlarmConfigInput : AddAlarmConfigInput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// 报警配置输出
/// </summary>
public class AlarmConfigOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long MineId { get; set; }

    /// <summary>
    /// 传感器类型代码
    /// </summary>
    public string SensorTypeCode { get; set; }

    /// <summary>
    /// 传感器类型名称
    /// </summary>
    public string SensorTypeName { get; set; }

    /// <summary>
    /// 报警类型:上限/下限/突变/断线
    /// </summary>
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警级别:1-一般,2-重要,3-紧急
    /// </summary>
    public int AlarmLevel { get; set; }

    /// <summary>
    /// 报警级别名称
    /// </summary>
    public string AlarmLevelName => AlarmLevel switch
    {
        1 => "一般",
        2 => "重要",
        3 => "紧急",
        _ => "未知"
    };

    /// <summary>
    /// 阈值
    /// </summary>
    public decimal ThresholdValue { get; set; }

    /// <summary>
    /// 延迟时间(秒)
    /// </summary>
    public int DelaySeconds { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public int AlarmEnabled { get; set; }

    /// <summary>
    /// 是否启用名称
    /// </summary>
    public string AlarmEnabledName => AlarmEnabled == 1 ? "启用" : "禁用";

    /// <summary>
    /// 通知人员
    /// </summary>
    public string NotifyUsers { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}

// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
using Newtonsoft.Json;

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
            .WhereIF(!string.IsNullOrWhiteSpace(input.AlarmCategory), u => u.AlarmCategory == input.AlarmCategory)
            .WhereIF(!string.IsNullOrWhiteSpace(input.AlarmName), u => u.AlarmName.Contains(input.AlarmName))
            .WhereIF(input.AlarmLevel.HasValue, u => u.AlarmLevel == input.AlarmLevel)
            .WhereIF(input.Enabled.HasValue, u => u.Enabled == input.Enabled)
            .OrderBy(u => u.AlarmCategory)
            .OrderBy(u => u.AlarmLevel)
            .Select<AlarmConfigOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取报警配置列表
    /// </summary>
    [DisplayName("获取报警配置列表")]
    public async Task<List<AlarmConfigOutput>> GetList([FromQuery] long mineId, [FromQuery] string? alarmCategory = null)
    {
        return await _alarmConfigRep.AsQueryable()
            .Where(u => u.MineId == mineId)
            .WhereIF(!string.IsNullOrWhiteSpace(alarmCategory), u => u.AlarmCategory == alarmCategory)
            .Where(u => u.Enabled == 1)
            .OrderBy(u => u.AlarmCategory)
            .OrderBy(u => u.AlarmLevel)
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
            .SetColumns(u => u.Enabled == enabled)
            .Where(u => u.Id == id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 复制报警配置到其他煤矿
    /// </summary>
    [DisplayName("复制报警配置")]
    public async Task CopyToMines(CopyAlarmConfigInput input)
    {
        var sourceConfig = await _alarmConfigRep.AsQueryable()
            .Where(u => u.Id == input.ConfigId)
            .FirstAsync();

        if (sourceConfig == null)
        {
            throw new Exception("报警配置不存在");
        }

        foreach (var mineId in input.TargetMineIds)
        {
            var newConfig = new AlarmConfig
            {
                MineId = mineId,
                AlarmName = sourceConfig.AlarmName,
                AlarmCode = sourceConfig.AlarmCode,
                AlarmCategory = sourceConfig.AlarmCategory,
                AlarmLevel = sourceConfig.AlarmLevel,
                Condition = sourceConfig.Condition,
                Description = sourceConfig.Description,
                TimeThreshold = sourceConfig.TimeThreshold,
                NotifyUserIds = sourceConfig.NotifyUserIds,
                NotifyUserNames = sourceConfig.NotifyUserNames,
                Enabled = sourceConfig.Enabled,
                Remark = sourceConfig.Remark
            };
            await _alarmConfigRep.InsertAsync(newConfig);
        }
    }

    /// <summary>
    /// 获取报警类别列表
    /// </summary>
    [DisplayName("获取报警类别列表")]
    public List<OptionOutput> GetAlarmCategoryList()
    {
        return new List<OptionOutput>
        {
            new() { Label = "传感器报警", Value = "传感器报警" },
            new() { Label = "设备监控", Value = "设备监控" },
            new() { Label = "数据监控", Value = "数据监控" },
            new() { Label = "人员行为", Value = "人员行为" },
            new() { Label = "智能预测", Value = "智能预测" }
        };
    }

    /// <summary>
    /// 获取预定义报警配置列表
    /// </summary>
    [DisplayName("获取预定义报警配置列表")]
    public List<PresetAlarmConfigOutput> GetPresetAlarmConfigs()
    {
        var presets = new List<PresetAlarmConfigOutput>
        {
            // 传感器报警 - CO
            new() { AlarmCategory = "传感器报警", AlarmName = "Ⅰ级CO超限报警", AlarmCode = "CO_Level1", Description = "CO气体监测浓度达到500ppm及以上", AlarmLevel = 3, Condition = JsonConvert.SerializeObject(new { sensorType = "CO", threshold = 500, unit = "ppm" }) },
            new() { AlarmCategory = "传感器报警", AlarmName = "Ⅱ级CO超限报警", AlarmCode = "CO_Level2", Description = "CO传感器浓度大于100ppm，持续时间超过30分钟", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { sensorType = "CO", threshold = 100, duration = 1800, unit = "ppm" }) },
            new() { AlarmCategory = "传感器报警", AlarmName = "Ⅲ级CO超限报警", AlarmCode = "CO_Level3", Description = "CO传感器浓度大于24ppm，持续时间超过10分钟", AlarmLevel = 1, Condition = JsonConvert.SerializeObject(new { sensorType = "CO", threshold = 24, duration = 600, unit = "ppm" }) },
            
            // 传感器报警 - 甲烷
            new() { AlarmCategory = "传感器报警", AlarmName = "Ⅰ级瓦斯超限报警", AlarmCode = "CH4_Level1", Description = "甲烷传感器浓度达到1.0%及以上", AlarmLevel = 3, Condition = JsonConvert.SerializeObject(new { sensorType = "CH4", threshold = 1.0, unit = "%" }) },
            new() { AlarmCategory = "传感器报警", AlarmName = "Ⅱ级瓦斯超限报警", AlarmCode = "CH4_Level2", Description = "甲烷传感器浓度达到1.0%及以上、小于1.5%", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { sensorType = "CH4", threshold = 1.0, threshold2 = 1.5, unit = "%" }) },
            new() { AlarmCategory = "传感器报警", AlarmName = "Ⅲ级瓦斯超限报警", AlarmCode = "CH4_Level3", Description = "甲烷传感器浓度达到0.5%及以上、小于1.0%", AlarmLevel = 1, Condition = JsonConvert.SerializeObject(new { sensorType = "CH4", threshold = 0.5, threshold2 = 1.0, unit = "%" }) },
            
            // 传感器报警 - 温度
            new() { AlarmCategory = "传感器报警", AlarmName = "井口温度低于2度报警", AlarmCode = "Temp_Level1", Description = "安全监测中，井口的温度低于2度报警", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { sensorType = "温度", location = "井口", threshold = 2, comparison = "lt", unit = "℃" }) },
            new() { AlarmCategory = "传感器报警", AlarmName = "井口温度低于4度预警", AlarmCode = "Temp_Level2", Description = "安全监测中，井口的温度低于4度预警", AlarmLevel = 1, Condition = JsonConvert.SerializeObject(new { sensorType = "温度", location = "井口", threshold = 4, comparison = "lt", unit = "℃" }) },
            
            // 设备监控 - 风机
            new() { AlarmCategory = "设备监控", AlarmName = "主通风机双停5分钟", AlarmCode = "Fan_Stop_5min", Description = "两台风机均停机持续时间≥5分钟", AlarmLevel = 3, Condition = JsonConvert.SerializeObject(new { deviceType = "风机", status = "双停", duration = 300 }), TimeThreshold = 300 },
            new() { AlarmCategory = "设备监控", AlarmName = "主通风机双停2分钟", AlarmCode = "Fan_Stop_2min", Description = "两台风机均停机持续时间≥2分钟", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { deviceType = "风机", status = "双停", duration = 120 }), TimeThreshold = 120 },
            
            // 设备监控 - 电力
            new() { AlarmCategory = "设备监控", AlarmName = "矿井单回路电源失电", AlarmCode = "Power_Single_8h", Description = "I段或II段所有的进线断路器为分闸状态≥8小时", AlarmLevel = 3, Condition = JsonConvert.SerializeObject(new { deviceType = "电力", status = "分闸", duration = 28800 }), TimeThreshold = 28800 },
            new() { AlarmCategory = "设备监控", AlarmName = "全矿井电源失电", AlarmCode = "Power_All_1min", Description = "所有进线柜断路器状态为分闸状态大于1分钟", AlarmLevel = 3, Condition = JsonConvert.SerializeObject(new { deviceType = "电力", status = "全失电", duration = 60 }), TimeThreshold = 60 },
            
            // 数据监控
            new() { AlarmCategory = "数据监控", AlarmName = "安全监控数据未更新", AlarmCode = "Data_Safety_NotUpdate", Description = "安全监控数据未更新", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { dataType = "安全监控", timeout = 3600 }), TimeThreshold = 3600 },
            new() { AlarmCategory = "数据监控", AlarmName = "人员定位数据未更新", AlarmCode = "Data_Location_NotUpdate", Description = "人员定位数据未更新", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { dataType = "人员定位", timeout = 3600 }), TimeThreshold = 3600 },
            new() { AlarmCategory = "数据监控", AlarmName = "水文监测数据未更新", AlarmCode = "Data_Water_NotUpdate", Description = "水文监测数据未更新", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { dataType = "水文监测", timeout = 3600 }), TimeThreshold = 3600 },
            
            // 人员行为
            new() { AlarmCategory = "人员行为", AlarmName = "井下人数阈值报警", AlarmCode = "Person_Count_Alarm", Description = "矿井井下人员总数超过或少于设定阈值报警", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { behaviorType = "人数阈值", thresholdType = "over/under" }) },
            
            // 智能预测
            new() { AlarmCategory = "智能预测", AlarmName = "瓦斯浓度趋势预测预警", AlarmCode = "Predict_CH4", Description = "瓦斯预测浓度大于0.3%预警", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { model = "瓦斯预测", threshold = 0.3, unit = "%" }) },
            new() { AlarmCategory = "智能预测", AlarmName = "CO浓度预测模型预警", AlarmCode = "Predict_CO", Description = "CO浓度大于0.8%预警", AlarmLevel = 2, Condition = JsonConvert.SerializeObject(new { model = "CO预测", threshold = 0.8, unit = "%" }) },
        };
        
        return presets;
    }
}

/// <summary>
/// 报警配置分页输入
/// </summary>
public class PageAlarmConfigInput : BasePageInput
{
    public long MineId { get; set; }
    public string AlarmCategory { get; set; }
    public string AlarmName { get; set; }
    public int? AlarmLevel { get; set; }
    public int? Enabled { get; set; }
}

/// <summary>
/// 新增报警配置输入
/// </summary>
public class AddAlarmConfigInput
{
    public long MineId { get; set; }
    public string AlarmName { get; set; }
    public string AlarmCode { get; set; }
    public string AlarmCategory { get; set; }
    public int AlarmLevel { get; set; } = 1;
    public string Condition { get; set; }
    public string Description { get; set; }
    public int TimeThreshold { get; set; }
    public string NotifyUserIds { get; set; }
    public string NotifyUserNames { get; set; }
    public int Enabled { get; set; } = 1;
    public string Remark { get; set; }
}

/// <summary>
/// 更新报警配置输入
/// </summary>
public class UpdateAlarmConfigInput : AddAlarmConfigInput
{
    public long Id { get; set; }
}

/// <summary>
/// 复制报警配置输入
/// </summary>
public class CopyAlarmConfigInput
{
    public long ConfigId { get; set; }
    public List<long> TargetMineIds { get; set; }
}

/// <summary>
/// 报警配置输出
/// </summary>
public class AlarmConfigOutput
{
    public long Id { get; set; }
    public long MineId { get; set; }
    public string MineName { get; set; }
    public string AlarmName { get; set; }
    public string AlarmCode { get; set; }
    public string AlarmCategory { get; set; }
    public int AlarmLevel { get; set; }
    public string AlarmLevelName => AlarmLevel switch { 1 => "一般", 2 => "重要", 3 => "紧急", 4 => "预警", _ => "未知" };
    public string Condition { get; set; }
    public string Description { get; set; }
    public int TimeThreshold { get; set; }
    public string NotifyUserIds { get; set; }
    public string NotifyUserNames { get; set; }
    public int Enabled { get; set; }
    public string EnabledName => Enabled == 1 ? "启用" : "禁用";
    public string Remark { get; set; }
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 预定义报警配置输出
/// </summary>
public class PresetAlarmConfigOutput
{
    public string AlarmCategory { get; set; }
    public string AlarmName { get; set; }
    public string AlarmCode { get; set; }
    public string Description { get; set; }
    public int AlarmLevel { get; set; }
    public string Condition { get; set; }
    public int TimeThreshold { get; set; }
}

/// <summary>
/// 下拉选项输出
/// </summary>
public class OptionOutput
{
    public string Label { get; set; }
    public string Value { get; set; }
}

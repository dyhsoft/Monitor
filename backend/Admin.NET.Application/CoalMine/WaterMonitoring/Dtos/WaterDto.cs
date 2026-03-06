namespace Admin.NET.Application.CoalMine.WaterMonitoring.Dtos;

/// <summary>
/// 水害监测分页查询
/// </summary>
public class WaterPageInput : PageInputBase
{
    public long? MineId { get; set; }
    public string SensorCode { get; set; }
    public string SensorName { get; set; }
    public int? Status { get; set; }
}

/// <summary>
/// 水害监测输出
/// </summary>
public class WaterOutput
{
    public long Id { get; set; }
    public long MineId { get; set; }
    public string MineName { get; set; }
    public string SensorCode { get; set; }
    public string SensorName { get; set; }
    public decimal? WaterLevel { get; set; }
    public decimal? FlowRate { get; set; }
    public decimal? Temperature { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; }
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 水害报警配置
/// </summary>
public class WaterAlarmConfigInput
{
    public long MineId { get; set; }
    public string SensorTypeCode { get; set; }
    public decimal? MaxWaterLevel { get; set; }
    public decimal? MaxFlowRate { get; set; }
    public int AlarmEnabled { get; set; } = 1;
}

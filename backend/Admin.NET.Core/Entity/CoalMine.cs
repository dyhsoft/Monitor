using SqlSugar;

namespace Admin.NET.Core.Entity.CoalMine;

/// <summary>
/// 煤矿表
/// </summary>
[SugarTable("CoalMine", "煤矿表")]
public class CoalMine : EntityBase
{
    [SugarColumn(Length = 20, ColumnDescription = "煤矿编号")]
    public string Code { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "煤矿名称")]
    public string Name { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "所属集团")]
    public string GroupName { get; set; }

    [SugarColumn(Length = 10, ColumnDescription = "省份")]
    public string Province { get; set; }

    [SugarColumn(Length = 10, ColumnDescription = "城市")]
    public string City { get; set; }

    [SugarColumn(Length = 10, ColumnDescription = "县区")]
    public string County { get; set; }

    [SugarColumn(Length = 200, ColumnDescription = "地址")]
    public string Address { get; set; }

    [SugarColumn(ColumnDescription = "经度")]
    public decimal? Longitude { get; set; }

    [SugarColumn(ColumnDescription = "纬度")]
    public decimal? Latitude { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "矿井类型")]
    public string MineType { get; set; }

    [SugarColumn(ColumnDescription = "生产能力")]
    public decimal? ProductionCapacity { get; set; }

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 网关配置表
/// </summary>
[SugarTable("CoalGatewayConfig", "网关配置表")]
public class CoalGatewayConfig : EntityBase
{
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "网关编号")]
    public string GatewayCode { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "网关名称")]
    public string GatewayName { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "IP地址")]
    public string IpAddress { get; set; }

    [SugarColumn(ColumnDescription = "端口")]
    public int Port { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "通讯协议")]
    public string Protocol { get; set; }

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 传感器表
/// </summary>
[SugarTable("CoalSensor", "传感器表")]
public class CoalSensor : EntityBase
{
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "传感器名称")]
    public string SensorName { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "传感器类型")]
    public string SensorType { get; set; }

    [SugarColumn(Length = 200, ColumnDescription = "安装位置")]
    public string Location { get; set; }

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 安全监测实时数据
/// </summary>
[SugarTable("SafetyRealtime", "安全监测实时数据")]
public class SafetyRealtime : EntityBase
{
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    [SugarColumn(ColumnDescription = "监测值", DecimalDigits = 2)]
    public decimal? Value { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "单位")]
    public string Unit { get; set; }

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }

    [SugarColumn(ColumnDescription = "接收时间")]
    public DateTime ReceivedTime { get; set; }
}

/// <summary>
/// 人员定位实时表
/// </summary>
[SugarTable("PersonLocation", "人员定位实时表")]
public class PersonLocation : EntityBase
{
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "定位卡号")]
    public string CardId { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "姓名")]
    public string PersonName { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "基站编号")]
    public string StationId { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "基站名称")]
    public string StationName { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "区域编号")]
    public string AreaCode { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "区域名称")]
    public string AreaName { get; set; }

    [SugarColumn(ColumnDescription = "入井时间")]
    public DateTime? InTime { get; set; }

    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 水害监测实时表
/// </summary>
[SugarTable("WaterRealtime", "水害监测实时表")]
public class WaterRealtime : EntityBase
{
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "传感器名称")]
    public string SensorName { get; set; }

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    [SugarColumn(ColumnDescription = "水位(m)", DecimalDigits = 2)]
    public decimal? WaterLevel { get; set; }

    [SugarColumn(ColumnDescription = "流量(m³/h)", DecimalDigits = 2)]
    public decimal? FlowRate { get; set; }

    [SugarColumn(ColumnDescription = "温度(℃)", DecimalDigits = 1)]
    public decimal? Temperature { get; set; }

    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 视频设备表
/// </summary>
[SugarTable("VideoDevice", "视频设备表")]
public class VideoDevice : EntityBase
{
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    [SugarColumn(Length = 50, ColumnDescription = "设备编号")]
    public string DeviceCode { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "设备名称")]
    public string DeviceName { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "设备类型")]
    public string DeviceType { get; set; }

    [SugarColumn(Length = 200, ColumnDescription = "安装位置")]
    public string Location { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "RTSP地址")]
    public string RtspUrl { get; set; }

    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

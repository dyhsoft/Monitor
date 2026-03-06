using SqlSugar;

namespace Admin.NET.Core.Entity.CoalMine;

/// <summary>
/// 煤矿表
/// </summary>
[SugarTable("CoalMine", "煤矿表")]
public class CoalMine : EntityBase
{
    /// <summary>
    /// 煤矿编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "煤矿编号")]
    public string Code { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "煤矿名称")]
    public string Name { get; set; }

    /// <summary>
    /// 所属集团
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "所属集团")]
    public string GroupName { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "省份")]
    public string Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "城市")]
    public string City { get; set; }

    /// <summary>
    /// 县区
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "县区")]
    public string County { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "地址")]
    public string Address { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    [SugarColumn(ColumnDescription = "经度")]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    [SugarColumn(ColumnDescription = "纬度")]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 矿井类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "矿井类型")]
    public string MineType { get; set; }

    /// <summary>
    /// 生产能力
    /// </summary>
    [SugarColumn(ColumnDescription = "生产能力")]
    public decimal? ProductionCapacity { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 网关配置表
/// </summary>
[SugarTable("CoalGatewayConfig", "网关配置表")]
public class CoalGatewayConfig : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 网关编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "网关编号")]
    public string GatewayCode { get; set; }

    /// <summary>
    /// 网关名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "网关名称")]
    public string GatewayName { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "IP地址")]
    public string IpAddress { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    [SugarColumn(ColumnDescription = "端口")]
    public int Port { get; set; }

    /// <summary>
    /// 通讯协议
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "通讯协议")]
    public string Protocol { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 传感器表
/// </summary>
[SugarTable("CoalSensor", "传感器表")]
public class CoalSensor : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "传感器名称")]
    public string SensorName { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "传感器类型")]
    public string SensorType { get; set; }

    /// <summary>
    /// 安装位置
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "安装位置")]
    public string Location { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 安全监测实时数据
/// </summary>
[SugarTable("SafetyRealtime", "安全监测实时数据")]
public class SafetyRealtime : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    [SugarColumn(ColumnDescription = "监测值", DecimalDigits = 2)]
    public decimal? Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "单位")]
    public string Unit { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 接收时间
    /// </summary>
    [SugarColumn(ColumnDescription = "接收时间")]
    public DateTime ReceivedTime { get; set; }
}

/// <summary>
/// 人员定位实时表
/// </summary>
[SugarTable("PersonLocation", "人员定位实时表")]
public class PersonLocation : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "定位卡号")]
    public string CardId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "姓名")]
    public string PersonName { get; set; }

    /// <summary>
    /// 基站编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "基站编号")]
    public string StationId { get; set; }

    /// <summary>
    /// 基站名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "基站名称")]
    public string StationName { get; set; }

    /// <summary>
    /// 区域编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "区域编号")]
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "区域名称")]
    public string AreaName { get; set; }

    /// <summary>
    /// 入井时间
    /// </summary>
    [SugarColumn(ColumnDescription = "入井时间")]
    public DateTime? InTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 水害监测实时表
/// </summary>
[SugarTable("WaterRealtime", "水害监测实时表")]
public class WaterRealtime : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "传感器名称")]
    public string SensorName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 水位(m)
    /// </summary>
    [SugarColumn(ColumnDescription = "水位(m)", DecimalDigits = 2)]
    public decimal? WaterLevel { get; set; }

    /// <summary>
    /// 流量(m³/h)
    /// </summary>
    [SugarColumn(ColumnDescription = "流量(m³/h)", DecimalDigits = 2)]
    public decimal? FlowRate { get; set; }

    /// <summary>
    /// 温度(℃)
    /// </summary>
    [SugarColumn(ColumnDescription = "温度(℃)", DecimalDigits = 1)]
    public decimal? Temperature { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 视频设备表
/// </summary>
[SugarTable("VideoDevice", "视频设备表")]
public class VideoDevice : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 设备编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "设备编号")]
    public string DeviceCode { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "设备名称")]
    public string DeviceName { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "设备类型")]
    public string DeviceType { get; set; }

    /// <summary>
    /// 安装位置
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "安装位置")]
    public string Location { get; set; }

    /// <summary>
    /// RTSP地址
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "RTSP地址")]
    public string RtspUrl { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

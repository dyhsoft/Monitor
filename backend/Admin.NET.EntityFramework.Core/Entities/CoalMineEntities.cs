using Furion.DatabaseAccessor;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.EntityFramework.Core;

/// <summary>
/// 煤矿实体
/// </summary>
[SugarTable("CoalMine", "煤矿表")]
[Tenant(Seven = true)]
public class CoalMine : EntityBase
{
    /// <summary>
    /// 煤矿编码（10位）
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "煤矿编码")]
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
    /// 省份编码
    /// </summary>
    [SugarColumn(Length = 2, ColumnDescription = "省份编码")]
    public string Province { get; set; }

    /// <summary>
    /// 城市编码
    /// </summary>
    [SugarColumn(Length = 2, ColumnDescription = "城市编码")]
    public string City { get; set; }

    /// <summary>
    /// 县区编码
    /// </summary>
    [SugarColumn(Length = 2, ColumnDescription = "县区编码")]
    public string County { get; set; }

    /// <summary>
    /// 矿井类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "矿井类型")]
    public string MineType { get; set; }

    /// <summary>
    /// 设计产能
    /// </summary>
    [SugarColumn(ColumnDescription = "设计产能")]
    public int? DesignCapacity { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "联系人")]
    public string Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "详细地址")]
    public string Address { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    [SugarColumn(ColumnDescription = "经度", DecimalDigits = 6)]
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 纬度
    /// </summary>
    [SugarColumn(ColumnDescription = "纬度", DecimalDigits = 6)]
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

/// <summary>
/// 网关配置实体
/// </summary>
[SugarTable("CoalGatewayConfig", "煤矿网关配置表")]
[Tenant(Seven = true)]
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
    /// 网关类型
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "网关类型")]
    public string GatewayType { get; set; }

    /// <summary>
    /// FTP主机
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "FTP主机")]
    public string FtpHost { get; set; }

    /// <summary>
    /// FTP端口
    /// </summary>
    [SugarColumn(ColumnDescription = "FTP端口")]
    public int FtpPort { get; set; } = 21;

    /// <summary>
    /// FTP用户名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "FTP用户名")]
    public string FtpUsername { get; set; }

    /// <summary>
    /// FTP密码
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "FTP密码")]
    public string FtpPassword { get; set; }

    /// <summary>
    /// FTP根目录
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "FTP根目录")]
    public string FtpRootPath { get; set; }

    /// <summary>
    /// 数据目录
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "数据目录")]
    public string DataPath { get; set; }

    /// <summary>
    /// 文件编码
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "文件编码")]
    public string FileEncoding { get; set; } = "UTF-8";

    /// <summary>
    /// 推送频率
    /// </summary>
    [SugarColumn(ColumnDescription = "推送频率(秒)")]
    public int PushFrequency { get; set; } = 60;

    /// <summary>
    /// IP限制
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "IP限制")]
    public string AllowedIp { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

/// <summary>
/// 传感器实体
/// </summary>
[SugarTable("CoalSensor", "传感器表")]
[Tenant(Seven = true)]
public class CoalSensor : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "测点编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 测点名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "测点名称")]
    public string SensorName { get; set; }

    /// <summary>
    /// 传感器类型码
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "传感器类型码")]
    public string SensorTypeCode { get; set; }

    /// <summary>
    /// 传感器类型名称
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "传感器类型名称")]
    public string SensorTypeName { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "单位")]
    public string Unit { get; set; }

    /// <summary>
    /// 安装位置编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "安装位置编号")]
    public string LocationCode { get; set; }

    /// <summary>
    /// 安装位置名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "安装位置名称")]
    public string LocationName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 安全监测实时数据
/// </summary>
[SugarTable("SafetyRealtime", "安全监测实时数据表")]
[Tenant(Seven = true)]
public class SafetyRealtime : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "测点编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    [SugarColumn(ColumnDescription = "监测值", DecimalDigits = 4)]
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
    /// 采集时间
    /// </summary>
    [SugarColumn(ColumnDescription = "采集时间")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 接收时间
    /// </summary>
    [SugarColumn(ColumnDescription = "接收时间")]
    public DateTime ReceivedTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 安全监测历史数据
/// </summary>
[SugarTable("SafetyHistory", "安全监测历史数据表")]
[Tenant(Seven = true)]
public class SafetyHistory : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "测点编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    [SugarColumn(ColumnDescription = "监测值", DecimalDigits = 4)]
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
    /// 采集时间
    /// </summary>
    [SugarColumn(ColumnDescription = "采集时间")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 接收时间
    /// </summary>
    [SugarColumn(ColumnDescription = "接收时间")]
    public DateTime ReceivedTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 人员信息
/// </summary>
[SugarTable("CoalPerson", "人员信息表")]
[Tenant(Seven = true)]
public class CoalPerson : EntityBase
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
    /// 部门
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "部门")]
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "工种")]
    public string JobType { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    [SugarColumn(Length = 18, ColumnDescription = "身份证号")]
    public string IdCard { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}

/// <summary>
/// 人员定位实时数据
/// </summary>
[SugarTable("PersonLocation", "人员定位实时表")]
[Tenant(Seven = true)]
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
    /// X坐标
    /// </summary>
    [SugarColumn(ColumnDescription = "X坐标", DecimalDigits = 2)]
    public decimal? X { get; set; }

    /// <summary>
    /// Y坐标
    /// </summary>
    [SugarColumn(ColumnDescription = "Y坐标", DecimalDigits = 2)]
    public decimal? Y { get; set; }

    /// <summary>
    /// Z坐标
    /// </summary>
    [SugarColumn(ColumnDescription = "Z坐标", DecimalDigits = 2)]
    public decimal? Z { get; set; }

    /// <summary>
    /// 进入时间
    /// </summary>
    [SugarColumn(ColumnDescription = "进入时间")]
    public DateTime? InTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 人员定位历史记录
/// </summary>
[SugarTable("PersonHistory", "人员定位历史表")]
[Tenant(Seven = true)]
public class PersonHistory : EntityBase
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
    /// 进入时间
    /// </summary>
    [SugarColumn(ColumnDescription = "进入时间")]
    public DateTime InTime { get; set; }

    /// <summary>
    /// 离开时间
    /// </summary>
    [SugarColumn(ColumnDescription = "离开时间")]
    public DateTime? OutTime { get; set; }

    /// <summary>
    /// 停留时长(秒)
    /// </summary>
    [SugarColumn(ColumnDescription = "停留时长")]
    public int? Duration { get; set; }
}

/// <summary>
/// 水害监测实时数据
/// </summary>
[SugarTable("WaterRealtime", "水害监测实时表")]
[Tenant(Seven = true)]
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
    [SugarColumn(Length = 20, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "传感器名称")]
    public string SensorName { get; set; }

    /// <summary>
    /// 水位
    /// </summary>
    [SugarColumn(ColumnDescription = "水位", DecimalDigits = 2)]
    public decimal? WaterLevel { get; set; }

    /// <summary>
    /// 流量
    /// </summary>
    [SugarColumn(ColumnDescription = "流量", DecimalDigits = 2)]
    public decimal? FlowRate { get; set; }

    /// <summary>
    /// 温度
    /// </summary>
    [SugarColumn(ColumnDescription = "温度", DecimalDigits = 2)]
    public decimal? Temperature { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 采集时间
    /// </summary>
    [SugarColumn(ColumnDescription = "采集时间")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 接收时间
    /// </summary>
    [SugarColumn(ColumnDescription = "接收时间")]
    public DateTime ReceivedTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 水害监测历史数据
/// </summary>
[SugarTable("WaterHistory", "水害监测历史表")]
[Tenant(Seven = true)]
public class WaterHistory : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "传感器编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 水位
    /// </summary>
    [SugarColumn(ColumnDescription = "水位", DecimalDigits = 2)]
    public decimal? WaterLevel { get; set; }

    /// <summary>
    /// 流量
    /// </summary>
    [SugarColumn(ColumnDescription = "流量", DecimalDigits = 2)]
    public decimal? FlowRate { get; set; }

    /// <summary>
    /// 温度
    /// </summary>
    [SugarColumn(ColumnDescription = "温度", DecimalDigits = 2)]
    public decimal? Temperature { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 采集时间
    /// </summary>
    [SugarColumn(ColumnDescription = "采集时间")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 接收时间
    /// </summary>
    [SugarColumn(ColumnDescription = "接收时间")]
    public DateTime ReceivedTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 报警配置
/// </summary>
[SugarTable("AlarmConfig", "报警配置表")]
[Tenant(Seven = true)]
public class AlarmConfig : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器类型码
    /// </summary>
    [SugarColumn(Length = 10, ColumnDescription = "传感器类型码")]
    public string SensorTypeCode { get; set; }

    /// <summary>
    /// 传感器类型名称
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "传感器类型名称")]
    public string SensorTypeName { get; set; }

    /// <summary>
    /// 报警类型
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型")]
    public int AlarmType { get; set; } = 1;

    /// <summary>
    /// 报警级别
    /// </summary>
    [SugarColumn(ColumnDescription = "报警级别")]
    public int AlarmLevel { get; set; } = 1;

    /// <summary>
    /// 阈值
    /// </summary>
    [SugarColumn(ColumnDescription = "阈值", DecimalDigits = 4)]
    public decimal? ThresholdValue { get; set; }

    /// <summary>
    /// 阈值2
    /// </summary>
    [SugarColumn(ColumnDescription = "阈值2", DecimalDigits = 4)]
    public decimal? ThresholdValue2 { get; set; }

    /// <summary>
    /// 报警延时(秒)
    /// </summary>
    [SugarColumn(ColumnDescription = "报警延时")]
    public int DelaySeconds { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public int AlarmEnabled { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

/// <summary>
/// 报警记录
/// </summary>
[SugarTable("AlarmRecord", "报警记录表")]
[Tenant(Seven = true)]
public class AlarmRecord : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 测点编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "测点编号")]
    public string SensorCode { get; set; }

    /// <summary>
    /// 测点名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "测点名称")]
    public string SensorName { get; set; }

    /// <summary>
    /// 传感器类型名称
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "传感器类型名称")]
    public string SensorTypeName { get; set; }

    /// <summary>
    /// 报警类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "报警类型")]
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警级别
    /// </summary>
    [SugarColumn(ColumnDescription = "报警级别")]
    public int AlarmLevel { get; set; }

    /// <summary>
    /// 报警值
    /// </summary>
    [SugarColumn(ColumnDescription = "报警值", DecimalDigits = 4)]
    public decimal? AlarmValue { get; set; }

    /// <summary>
    /// 阈值
    /// </summary>
    [SugarColumn(ColumnDescription = "阈值", DecimalDigits = 4)]
    public decimal? ThresholdValue { get; set; }

    /// <summary>
    /// 报警时间
    /// </summary>
    [SugarColumn(ColumnDescription = "报警时间")]
    public DateTime AlarmTime { get; set; }

    /// <summary>
    /// 确认时间
    /// </summary>
    [SugarColumn(ColumnDescription = "确认时间")]
    public DateTime? ConfirmTime { get; set; }

    /// <summary>
    /// 确认人Id
    /// </summary>
    [SugarColumn(ColumnDescription = "确认人Id")]
    public long? ConfirmUserId { get; set; }

    /// <summary>
    /// 确认人姓名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "确认人姓名")]
    public string ConfirmUserName { get; set; }

    /// <summary>
    /// 解除时间
    /// </summary>
    [SugarColumn(ColumnDescription = "解除时间")]
    public DateTime? ResolveTime { get; set; }

    /// <summary>
    /// 解除人Id
    /// </summary>
    [SugarColumn(ColumnDescription = "解除人Id")]
    public long? ResolveUserId { get; set; }

    /// <summary>
    /// 解除人姓名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "解除人姓名")]
    public string ResolveUserName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

/// <summary>
/// 视频设备
/// </summary>
[SugarTable("VideoDevice", "视频设备表")]
[Tenant(Seven = true)]
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
    /// IP地址
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "IP地址")]
    public string IpAddress { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    [SugarColumn(ColumnDescription = "端口")]
    public int Port { get; set; } = 8000;

    /// <summary>
    /// 通道号
    /// </summary>
    [SugarColumn(ColumnDescription = "通道号")]
    public int Channel { get; set; } = 1;

    /// <summary>
    /// 码流类型
    /// </summary>
    [SugarColumn(ColumnDescription = "码流类型")]
    public int StreamType { get; set; } = 1;

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "用户名")]
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "密码")]
    public string Password { get; set; }

    /// <summary>
    /// 协议类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "协议类型")]
    public string ProtocolType { get; set; } = "GB28181";

    /// <summary>
    /// 视频流地址
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "视频流地址")]
    public string StreamUrl { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 安装位置
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "安装位置")]
    public string InstallLocation { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

/// <summary>
/// 解析日志
/// </summary>
[SugarTable("ParseLog", "解析日志表")]
[Tenant(Seven = true)]
public class ParseLog : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 煤矿编号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "煤矿编号")]
    public string MineCode { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "文件名")]
    public string FileName { get; set; }

    /// <summary>
    /// 源文件路径
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "源文件路径")]
    public string FilePath { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "文件类型")]
    public string FileType { get; set; }

    /// <summary>
    /// 文件编码
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "文件编码")]
    public string Encoding { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小")]
    public long? FileSize { get; set; }

    /// <summary>
    /// 源文件内容
    /// </summary>
    [SugarColumn(ColumnDescription = "源文件内容", ColumnDataType = "ntext")]
    public string SourceContent { get; set; }

    /// <summary>
    /// 解析记录数
    /// </summary>
    [SugarColumn(ColumnDescription = "解析记录数")]
    public int RecordCount { get; set; }

    /// <summary>
    /// 成功数
    /// </summary>
    [SugarColumn(ColumnDescription = "成功数")]
    public int SuccessCount { get; set; }

    /// <summary>
    /// 错误数
    /// </summary>
    [SugarColumn(ColumnDescription = "错误数")]
    public int ErrorCount { get; set; }

    /// <summary>
    /// 解析耗时
    /// </summary>
    [SugarColumn(ColumnDescription = "解析耗时")]
    public int? ParseTime { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(Length = 1000, ColumnDescription = "错误信息")]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }
}

/// <summary>
/// 人员信息
/// </summary>
[SugarTable("PersonInfo", "人员信息表")]
[Tenant(Seven = true)]
public class PersonInfo : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "姓名")]
    public string PersonName { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "定位卡号")]
    public string CardId { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "部门")]
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "工种")]
    public string WorkType { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "职位")]
    public string Position { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    [SugarColumn(Length = 18, ColumnDescription = "身份证号")]
    public string IdCard { get; set; }

    /// <summary>
    /// 照片路径
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "照片路径")]
    public string PhotoPath { get; set; }

    /// <summary>
    /// 状态（1=在岗，0=离岗）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "备注")]
    public string Remark { get; set; }
}

/// <summary>
/// 人员出勤记录
/// </summary>
[SugarTable("PersonAttendance", "人员出勤记录表")]
[Tenant(Seven = true)]
public class PersonAttendance : EntityBase
{
    /// <summary>
    /// 煤矿Id
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿Id")]
    public long MineId { get; set; }

    /// <summary>
    /// 人员信息Id
    /// </summary>
    [SugarColumn(ColumnDescription = "人员信息Id")]
    public long PersonInfoId { get; set; }

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
    /// 部门
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "部门")]
    public string Department { get; set; }

    /// <summary>
    /// 工种
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "工种")]
    public string WorkType { get; set; }

    /// <summary>
    /// 入井时间
    /// </summary>
    [SugarColumn(ColumnDescription = "入井时间")]
    public DateTime? InTime { get; set; }

    /// <summary>
    /// 出井时间
    /// </summary>
    [SugarColumn(ColumnDescription = "出井时间")]
    public DateTime? OutTime { get; set; }

    /// <summary>
    /// 工作时长（秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "工作时长")]
    public int? WorkDuration { get; set; }

    /// <summary>
    /// 状态（0=在井中，1=已出井）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 出勤日期
    /// </summary>
    [SugarColumn(ColumnDescription = "出勤日期")]
    public DateTime AttendanceDate { get; set; }
}

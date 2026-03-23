// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 安全监测数据
/// </summary>
[SugarTable("SafetyData", "安全监测数据表")]
public class SafetyData : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器编号", Length = 50)]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器名称", Length = 100, IsNullable = true)]
    public string SensorName { get; set; }

    /// <summary>
    /// 传感器类型代码
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器类型代码", Length = 20)]
    public string SensorType { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    [SugarColumn(ColumnDescription = "监测值")]
    public decimal Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [SugarColumn(ColumnDescription = "单位", Length = 20, IsNullable = true)]
    public string Unit { get; set; }

    /// <summary>
    /// 状态:0-正常,1-报警,2-断电,3-故障,4-离线
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "0")]
    public int Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 人员定位数据
/// </summary>
[SugarTable("PersonLocations", "人员定位数据表")]
public class PersonLocation : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    [SugarColumn(ColumnDescription = "定位卡号", Length = 50)]
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "人员姓名", Length = 50, IsNullable = true)]
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(ColumnDescription = "部门", Length = 100, IsNullable = true)]
    public string DeptName { get; set; }

    /// <summary>
    /// 定位分站ID
    /// </summary>
    [SugarColumn(ColumnDescription = "定位分站ID", Length = 50, IsNullable = true)]
    public string StationId { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    [SugarColumn(ColumnDescription = "区域编码", Length = 20, IsNullable = true)]
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [SugarColumn(ColumnDescription = "区域名称", Length = 100, IsNullable = true)]
    public string AreaName { get; set; }

    /// <summary>
    /// 大地坐标X
    /// </summary>
    [SugarColumn(ColumnDescription = "大地坐标X", IsNullable = true)]
    public decimal? X { get; set; }

    /// <summary>
    /// 大地坐标Y
    /// </summary>
    [SugarColumn(ColumnDescription = "大地坐标Y", IsNullable = true)]
    public decimal? Y { get; set; }

    /// <summary>
    /// 深度/高程
    /// </summary>
    [SugarColumn(ColumnDescription = "深度/高程", IsNullable = true)]
    public decimal? Z { get; set; }

    /// <summary>
    /// 状态:1-进入,2-离开
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "1")]
    public int Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 人员进出记录
/// </summary>
[SugarTable("PersonRecords", "人员进出记录表")]
public class PersonRecord : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    [SugarColumn(ColumnDescription = "定位卡号", Length = 50)]
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "人员姓名", Length = 50, IsNullable = true)]
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(ColumnDescription = "部门", Length = 100, IsNullable = true)]
    public string DeptName { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    [SugarColumn(ColumnDescription = "区域编码", Length = 20, IsNullable = true)]
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    [SugarColumn(ColumnDescription = "区域名称", Length = 100, IsNullable = true)]
    public string AreaName { get; set; }

    /// <summary>
    /// 类型:1-进入,2-离开
    /// </summary>
    [SugarColumn(ColumnDescription = "类型")]
    public int RecordType { get; set; }

    /// <summary>
    /// 记录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "记录时间")]
    public DateTime RecordTime { get; set; }
}

/// <summary>
/// 矿压监测数据
/// </summary>
[SugarTable("PressureData", "矿压监测数据表")]
public class PressureData : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器编号", Length = 50)]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器名称", Length = 100, IsNullable = true)]
    public string SensorName { get; set; }

    /// <summary>
    /// 类型:压力/位移/锚杆应力
    /// </summary>
    [SugarColumn(ColumnDescription = "类型", Length = 20)]
    public string SensorType { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    [SugarColumn(ColumnDescription = "监测值")]
    public decimal Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [SugarColumn(ColumnDescription = "单位", Length = 20, IsNullable = true)]
    public string Unit { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", Length = 20, IsNullable = true)]
    public string Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 水文监测数据
/// </summary>
[SugarTable("WaterData", "水文监测数据表")]
public class WaterData : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器编号", Length = 50)]
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器名称", Length = 100, IsNullable = true)]
    public string SensorName { get; set; }

    /// <summary>
    /// 类型:水位/流量/排水量
    /// </summary>
    [SugarColumn(ColumnDescription = "类型", Length = 20)]
    public string SensorType { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    [SugarColumn(ColumnDescription = "监测值")]
    public decimal Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [SugarColumn(ColumnDescription = "单位", Length = 20, IsNullable = true)]
    public string Unit { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", Length = 20, IsNullable = true)]
    public string Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 报警记录
/// </summary>
[SugarTable("AlarmRecords", "报警记录表")]
public class AlarmRecord : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    [SugarColumn(ColumnDescription = "传感器编号", Length = 50, IsNullable = true)]
    public string SensorCode { get; set; }

    /// <summary>
    /// 报警类型
    /// </summary>
    [SugarColumn(ColumnDescription = "报警类型", Length = 50)]
    public string AlarmType { get; set; }

    /// <summary>
    /// 报警值
    /// </summary>
    [SugarColumn(ColumnDescription = "报警值", Length = 50, IsNullable = true)]
    public string AlarmValue { get; set; }

    /// <summary>
    /// 阈值
    /// </summary>
    [SugarColumn(ColumnDescription = "阈值", Length = 50, IsNullable = true)]
    public string Threshold { get; set; }

    /// <summary>
    /// 报警时间
    /// </summary>
    [SugarColumn(ColumnDescription = "报警时间")]
    public DateTime AlarmTime { get; set; }

    /// <summary>
    /// 状态:1-未处理,2-已处理,3-已忽略
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "1")]
    public int Status { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    [SugarColumn(ColumnDescription = "处理时间", IsNullable = true)]
    public DateTime? HandleTime { get; set; }

    /// <summary>
    /// 处理人
    /// </summary>
    [SugarColumn(ColumnDescription = "处理人", Length = 50, IsNullable = true)]
    public string Handler { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    [SugarColumn(ColumnDescription = "处理备注", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}

/// <summary>
/// 基站状态
/// </summary>
[SugarTable("StationStatus", "基站状态表")]
public class StationStatus : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID")]
    public long MineId { get; set; }

    /// <summary>
    /// 基站编号
    /// </summary>
    [SugarColumn(ColumnDescription = "基站编号", Length = 50)]
    public string StationId { get; set; }

    /// <summary>
    /// 基站名称
    /// </summary>
    [SugarColumn(ColumnDescription = "基站名称", Length = 100, IsNullable = true)]
    public string StationName { get; set; }

    /// <summary>
    /// 状态:0-离线,1-在线
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "1")]
    public int Status { get; set; }

    /// <summary>
    /// 电量
    /// </summary>
    [SugarColumn(ColumnDescription = "电量", IsNullable = true)]
    public decimal? Power { get; set; }

    /// <summary>
    /// 信号强度
    /// </summary>
    [SugarColumn(ColumnDescription = "信号强度", IsNullable = true)]
    public int? Signal { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public new DateTime UpdateTime { get; set; }
}

/// <summary>
/// 解析日志
/// </summary>
[SugarTable("ParseLogs", "解析日志表")]
public class ParseLog : EntityBase
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    [SugarColumn(ColumnDescription = "煤矿ID", IsNullable = true)]
    public long? MineId { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    [SugarColumn(ColumnDescription = "文件名", Length = 200)]
    public string FileName { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    [SugarColumn(ColumnDescription = "数据类型", Length = 20, IsNullable = true)]
    public string DataType { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定系统", Length = 50, IsNullable = true)]
    public string BindSystem { get; set; }

    /// <summary>
    /// 记录数
    /// </summary>
    [SugarColumn(ColumnDescription = "记录数", DefaultValue = "0")]
    public int RecordCount { get; set; }

    /// <summary>
    /// 状态:1-成功,0-失败
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", DefaultValue = "1")]
    public int Status { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(ColumnDescription = "错误信息", Length = 1000, IsNullable = true)]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 耗时(毫秒)
    /// </summary>
    [SugarColumn(ColumnDescription = "耗时(毫秒)", IsNullable = true)]
    public int? Duration { get; set; }
}

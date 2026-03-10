namespace Admin.NET.Core;

/// <summary>
/// 安全监测数据查询输入参数
/// </summary>
public class PageSafetyDataInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 安全监测数据输出参数
/// </summary>
public class SafetyDataOutput
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
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器名称
    /// </summary>
    public string SensorName { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }

    /// <summary>
    /// 监测值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 人员定位数据查询输入参数
/// </summary>
public class PagePersonLocationInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string PersonName { get; set; }
}

/// <summary>
/// 人员定位数据输出参数
/// </summary>
public class PersonLocationOutput
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
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string DeptName { get; set; }

    /// <summary>
    /// 定位分站ID
    /// </summary>
    public string StationId { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 大地坐标X
    /// </summary>
    public decimal? X { get; set; }

    /// <summary>
    /// 大地坐标Y
    /// </summary>
    public decimal? Y { get; set; }

    /// <summary>
    /// 深度/高程
    /// </summary>
    public decimal? Z { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 人员进出记录查询输入参数
/// </summary>
public class PagePersonRecordInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 记录类型
    /// </summary>
    public int? RecordType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 人员进出记录输出参数
/// </summary>
public class PersonRecordOutput
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
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 定位卡号
    /// </summary>
    public string CardId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    public string PersonName { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public string DeptName { get; set; }

    /// <summary>
    /// 区域编码
    /// </summary>
    public string AreaCode { get; set; }

    /// <summary>
    /// 区域名称
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 状态/类型
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 记录时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 矿压监测数据查询输入参数
/// </summary>
public class PagePressureDataInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }
}

/// <summary>
/// 水文监测数据查询输入参数
/// </summary>
public class PageWaterDataInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 传感器编号
    /// </summary>
    public string SensorCode { get; set; }

    /// <summary>
    /// 传感器类型
    /// </summary>
    public string SensorType { get; set; }
}

/// <summary>
/// 解析日志查询输入参数
/// </summary>
public class PageParseLogInput : BasePageInput
{
    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 解析日志输出参数
/// </summary>
public class ParseLogOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 煤矿ID
    /// </summary>
    public long? MineId { get; set; }

    /// <summary>
    /// 煤矿名称
    /// </summary>
    public string MineName { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 绑定系统
    /// </summary>
    public string BindSystem { get; set; }

    /// <summary>
    /// 记录数
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 耗时(毫秒)
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}

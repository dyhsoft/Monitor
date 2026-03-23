using Admin.NET.Core;

namespace Admin.NET.Core;

/// <summary>
/// 矿领导配置输入参数
/// </summary>
public class AddLocationLeaderConfigInput
{
    public long MineId { get; set; }
    public string PersonName { get; set; }
    public string CardId { get; set; }
    public string DeptName { get; set; }
    public string Position { get; set; }
    public int Enabled { get; set; } = 1;
}

public class UpdateLocationLeaderConfigInput : AddLocationLeaderConfigInput
{
    public long Id { get; set; }
}

public class PageLocationLeaderConfigInput : BasePageInput
{
    public long? MineId { get; set; }
    public string PersonName { get; set; }
}

public class LocationLeaderConfigOutput
{
    public long Id { get; set; }
    public long MineId { get; set; }
    public string MineName { get; set; }
    public string PersonName { get; set; }
    public string CardId { get; set; }
    public string DeptName { get; set; }
    public string Position { get; set; }
    public int Enabled { get; set; }
    public DateTime? CreateTime { get; set; }
}

/// <summary>
/// 限定人数配置输入参数
/// </summary>
public class AddLocationLimitConfigInput
{
    public long MineId { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public int LimitCount { get; set; }
    public int Enabled { get; set; } = 1;
}

public class UpdateLocationLimitConfigInput : AddLocationLimitConfigInput
{
    public long Id { get; set; }
}

public class PageLocationLimitConfigInput : BasePageInput
{
    public long? MineId { get; set; }
}

public class LocationLimitConfigOutput
{
    public long Id { get; set; }
    public long MineId { get; set; }
    public string MineName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public int LimitCount { get; set; }
    public int Enabled { get; set; }
    public DateTime? CreateTime { get; set; }
}

/// <summary>
/// 定位报警记录查询输入参数
/// </summary>
public class PageLocationAlarmInput : BasePageInput
{
    public long? MineId { get; set; }
    public int? AlarmType { get; set; }
    public int? Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class LocationAlarmOutput
{
    public long Id { get; set; }
    public long MineId { get; set; }
    public string MineName { get; set; }
    public int AlarmType { get; set; }
    public string CardId { get; set; }
    public string PersonName { get; set; }
    public string AreaName { get; set; }
    public string AlarmMessage { get; set; }
    public int Status { get; set; }
    public DateTime AlarmTime { get; set; }
}

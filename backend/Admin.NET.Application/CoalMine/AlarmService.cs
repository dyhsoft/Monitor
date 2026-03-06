using Admin.NET.Core;
using SqlSugar;
using Furion;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Admin.NET.Application;

/// <summary>
/// 报警消息服务
/// </summary>
public class AlarmService : IDynamicApiController
{
    private readonly ISqlSugarClient _db;
    private readonly IHubContext<OnlineUserHub, IOnlineUserHub> _hubContext;

    public AlarmService(ISqlSugarClient db, IHubContext<OnlineUserHub, IOnlineUserHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    #region 报警配置

    /// <summary>
    /// 获取报警配置列表
    /// </summary>
    public async Task<SqlSugarPagedList<CoalAlarmConfig>> GetConfigPage(BasePageInput input)
    {
        return await _db.Queryable<CoalAlarmConfig>()
            .OrderBy(it => it.Id, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取报警配置详情
    /// </summary>
    public async Task<CoalAlarmConfig> GetConfig(long id)
    {
        return await _db.Queryable<CoalAlarmConfig>().Where(it => it.Id == id).FirstAsync();
    }

    /// <summary>
    /// 新增报警配置
    /// </summary>
    public async Task<long> AddConfig(CoalAlarmConfig input)
    {
        return await _db.Insertable(input).ExecuteReturnIdentityAsync();
    }

    /// <summary>
    /// 更新报警配置
    /// </summary>
    public async Task UpdateConfig(CoalAlarmConfig input)
    {
        await _db.Updateable(input).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除报警配置
    /// </summary>
    public async Task DeleteConfig(long id)
    {
        await _db.Deleteable<CoalAlarmConfig>().Where(it => it.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取煤矿的报警配置
    /// </summary>
    public async Task<List<CoalAlarmConfig>> GetConfigsByMine(long mineId)
    {
        return await _db.Queryable<CoalAlarmConfig>()
            .Where(it => it.MineId == mineId && it.Enabled)
            .ToListAsync();
    }

    #endregion

    #region 报警记录

    /// <summary>
    /// 获取报警记录列表
    /// </summary>
    public async Task<SqlSugarPagedList<CoalAlarmRecord>> GetRecordPage(BasePageInput input)
    {
        return await _db.Queryable<CoalAlarmRecord>()
            .OrderBy(it => it.AlarmTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取报警记录详情
    /// </summary>
    public async Task<CoalAlarmRecord> GetRecord(long id)
    {
        return await _db.Queryable<CoalAlarmRecord>().Where(it => it.Id == id).FirstAsync();
    }

    /// <summary>
    /// 处理报警
    /// </summary>
    public async Task HandleAlarm(HandleAlarmInput input)
    {
        var alarm = await _db.Queryable<CoalAlarmRecord>().Where(it => it.Id == input.Id).FirstAsync();
        if (alarm != null)
        {
            alarm.Status = input.Status;
            alarm.Handler = App.User?.FindFirst(ClaimConst.RealName)?.Value ?? "系统";
            alarm.HandleTime = DateTime.Now;
            alarm.HandleRemark = input.Remark;
            await _db.Updateable(alarm).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 获取未处理报警数
    /// </summary>
    public async Task<dynamic> GetUnhandleCount()
    {
        var count = await _db.Queryable<CoalAlarmRecord>().Where(it => it.Status == 0).CountAsync();
        return new { UnhandleCount = count };
    }

    #endregion

    #region 报警消息推送

    /// <summary>
    /// 发送报警消息
    /// </summary>
    public async Task<bool> SendAlarmMessage(CoalAlarmRecord alarm)
    {
        try
        {
            // 获取报警配置
            var configs = await _db.Queryable<CoalAlarmConfig>()
                .Where(it => it.MineId == alarm.MineId && it.AlarmType == alarm.AlarmType && it.Enabled)
                .ToListAsync();

            if (configs.Count == 0) return false;

            // 收集所有接收用户
            var userIds = new List<long>();
            foreach (var config in configs)
            {
                if (!string.IsNullOrEmpty(config.UserIds))
                {
                    var ids = config.UserIds.Split(',').Select(long.Parse).ToList();
                    userIds.AddRange(ids);
                }
            }

            userIds = userIds.Distinct().ToList();
            if (userIds.Count == 0) return false;

            // 构建消息
            var message = new MessageInput
            {
                Title = $"【{GetAlarmTypeName(alarm.AlarmType)}报警】{alarm.MineName}",
                Message = alarm.Description,
                MessageType = MessageTypeEnum.Warning,
                SendUserId = "system",
                SendUserName = "系统",
                SendTime = DateTime.Now,
                UserIds = userIds
            };

            // 通过 SignalR 发送消息
            var hashKey = App.Cache.HashGetAll<string, SysOnlineUser>(CacheConst.KeyUserOnline);
            var receiveUsers = hashKey.Where(u => userIds.Any(a => a == u.Value.UserId)).Select(u => u.Value).ToList();
            
            await receiveUsers.ForEachAsync(u => 
                _hubContext.Clients.Client(u.ConnectionId ?? "").ReceiveMessage(message));

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 触发报警（安全监测）
    /// </summary>
    public async Task TriggerSafetyAlarm(long mineId, string sensorCode, string sensorName, decimal value, decimal threshold, string description)
    {
        var alarm = new CoalAlarmRecord
        {
            MineId = mineId,
            AlarmType = 1, // 安全监测
            SensorCode = sensorCode,
            SensorName = sensorName,
            AlarmValue = value,
            ThresholdValue = threshold,
            Description = description,
            AlarmTime = DateTime.Now,
            Status = 0
        };

        // 保存报警记录
        await _db.Insertable(alarm).ExecuteReturnIdentityAsync();

        // 发送消息
        await SendAlarmMessage(alarm);
    }

    private string GetAlarmTypeName(int type)
    {
        return type switch
        {
            1 => "安全监测",
            2 => "人员定位",
            3 => "水害监测",
            4 => "视频监控",
            _ => "未知"
        };
    }

    #endregion
}

/// <summary>
/// 处理报警输入
/// </summary>
public class HandleAlarmInput
{
    /// <summary>
    /// 报警Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 处理状态（1-已处理 2-已忽略）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    public string Remark { get; set; }
}

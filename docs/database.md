-- =============================================
-- 煤矿安全监测系统 - 数据库设计文档
-- 版本: V1.0
-- 日期: 2026-03-05
-- 数据库: SQL Server / SQLite
-- =============================================

-- =============================================
-- 1. 煤矿管理模块
-- =============================================

-- 煤矿信息表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CoalMine]') AND type in (N'U'))
CREATE TABLE [dbo].[CoalMine] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [Code] NVARCHAR(10) NOT NULL UNIQUE,          -- 煤矿编码(10位)
    [Name] NVARCHAR(100) NOT NULL,                -- 煤矿名称
    [GroupName] NVARCHAR(100) NULL,               -- 所属集团
    [Province] NVARCHAR(2) NULL,                  -- 省份编码
    [City] NVARCHAR(2) NULL,                      -- 城市编码
    [County] NVARCHAR(2) NULL,                     -- 县区编码
    [MineType] NVARCHAR(20) NULL,                  -- 矿井类型(生产/建设)
    [DesignCapacity] INT NULL,                     -- 设计产能(万吨/年)
    [Contact] NVARCHAR(50) NULL,                   -- 联系人
    [Phone] NVARCHAR(20) NULL,                     -- 联系电话
    [Address] NVARCHAR(200) NULL,                  -- 详细地址
    [Longitude] DECIMAL(10,6) NULL,                -- 经度
    [Latitude] DECIMAL(10,6) NULL,                 -- 纬度
    [Status] INT DEFAULT 1,                        -- 状态(0=停用,1=启用)
    [Remark] NVARCHAR(500) NULL,                   -- 备注
    [CreateTime] DATETIME DEFAULT GETDATE(),        -- 创建时间
    [CreatorId] BIGINT NULL,                       -- 创建人Id
    [ModifyTime] DATETIME NULL,                    -- 修改时间
    [ModifierId] BIGINT NULL                       -- 修改人Id
);

-- 网关配置表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CoalGatewayConfig]') AND type in (N'U'))
CREATE TABLE [dbo].[CoalGatewayConfig] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,                     -- 煤矿Id
    [GatewayCode] NVARCHAR(50) NOT NULL UNIQUE,   -- 网关编号
    [GatewayName] NVARCHAR(100) NULL,             -- 网关名称
    [GatewayType] NVARCHAR(50) NOT NULL,          -- 网关类型
    [FtpHost] NVARCHAR(100) NULL,                -- FTP主机地址
    [FtpPort] INT DEFAULT 21,                     -- FTP端口
    [FtpUsername] NVARCHAR(50) NULL,              -- FTP用户名
    [FtpPassword] NVARCHAR(200) NULL,             -- FTP密码(加密)
    [FtpRootPath] NVARCHAR(200) NULL,             -- FTP根目录
    [DataPath] NVARCHAR(200) NULL,                -- 数据目录
    [FileEncoding] NVARCHAR(20) DEFAULT 'UTF-8', -- 文件编码
    [PushFrequency] INT DEFAULT 60,                -- 推送频率(秒)
    [AllowedIp] NVARCHAR(100) NULL,               -- IP限制
    [Status] INT DEFAULT 1,                        -- 状态
    [Remark] NVARCHAR(500) NULL,                  -- 备注
    [CreateTime] DATETIME DEFAULT GETDATE(),
    [CreatorId] BIGINT NULL,
    [ModifyTime] DATETIME NULL,
    [ModifierId] BIGINT NULL,
    CONSTRAINT [FK_CoalGatewayConfig_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- =============================================
-- 2. 传感器/测点管理
-- =============================================

-- 传感器表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CoalSensor]') AND type in (N'U'))
CREATE TABLE [dbo].[CoalSensor] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,                     -- 煤矿Id
    [SensorCode] NVARCHAR(20) NOT NULL UNIQUE,   -- 测点编号
    [SensorName] NVARCHAR(100) NULL,              -- 测点名称
    [SensorTypeCode] NVARCHAR(10) NULL,          -- 传感器类型码(A01/A02...)
    [SensorTypeName] NVARCHAR(50) NULL,           -- 传感器类型名称(甲烷/一氧化碳...)
    [Unit] NVARCHAR(20) NULL,                     -- 单位
    [LocationCode] NVARCHAR(20) NULL,             -- 安装位置编号
    [LocationName] NVARCHAR(100) NULL,            -- 安装位置名称
    [Status] INT DEFAULT 1,                        -- 状态(0=停用,1=启用)
    [CreateTime] DATETIME DEFAULT GETDATE(),
    [ModifyTime] DATETIME NULL,
    CONSTRAINT [FK_CoalSensor_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- =============================================
-- 3. 安全监测数据
-- =============================================

-- 安全监测实时数据表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SafetyRealtime]') AND type in (N'U'))
CREATE TABLE [dbo].[SafetyRealtime] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,                     -- 煤矿Id
    [SensorCode] NVARCHAR(20) NOT NULL,           -- 测点编号
    [Value] DECIMAL(18,4) NULL,                    -- 监测值
    [Unit] NVARCHAR(20) NULL,                     -- 单位
    [Status] INT DEFAULT 0,                        -- 状态(0=正常,1=报警,2=断电,3=故障)
    [UpdateTime] DATETIME NOT NULL,                -- 采集时间
    [ReceivedTime] DATETIME DEFAULT GETDATE(),     -- 接收时间
    CONSTRAINT [FK_SafetyRealtime_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- 安全监测历史数据表(按月分区)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SafetyHistory]') AND type in (N'U'))
CREATE TABLE [dbo].[SafetyHistory] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [SensorCode] NVARCHAR(20) NOT NULL,
    [Value] DECIMAL(18,4) NULL,
    [Unit] NVARCHAR(20) NULL,
    [Status] INT DEFAULT 0,
    [UpdateTime] DATETIME NOT NULL,
    [ReceivedTime] DATETIME DEFAULT GETDATE()
);

-- 创建索引
CREATE INDEX [IX_SafetyHistory_MineId_UpdateTime] ON [SafetyHistory]([MineId], [UpdateTime]);
CREATE INDEX [IX_SafetyHistory_SensorCode] ON [SafetyHistory]([SensorCode]);

-- =============================================
-- 4. 人员定位数据
-- =============================================

-- 人员信息表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CoalPerson]') AND type in (N'U'))
CREATE TABLE [dbo].[CoalPerson] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [CardId] NVARCHAR(20) NOT NULL UNIQUE,       -- 定位卡号
    [PersonName] NVARCHAR(50) NULL,               -- 姓名
    [Department] NVARCHAR(100) NULL,              -- 部门/班组
    [JobType] NVARCHAR(50) NULL,                  -- 工种
    [Phone] NVARCHAR(20) NULL,                    -- 联系电话
    [IdCard] NVARCHAR(18) NULL,                   -- 身份证号
    [Status] INT DEFAULT 1,                        -- 状态(0=离职,1=在职)
    [CreateTime] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK_CoalPerson_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- 人员定位实时表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonLocation]') AND type in (N'U'))
CREATE TABLE [dbo].[PersonLocation] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [CardId] NVARCHAR(20) NOT NULL,
    [PersonName] NVARCHAR(50) NULL,
    [StationId] NVARCHAR(50) NULL,                -- 基站/分站编号
    [StationName] NVARCHAR(100) NULL,             -- 基站名称
    [AreaCode] NVARCHAR(20) NULL,                 -- 区域编号
    [AreaName] NVARCHAR(100) NULL,                -- 区域名称
    [X] DECIMAL(10,2) NULL,                       -- X坐标
    [Y] DECIMAL(10,2) NULL,                       -- Y坐标
    [Z] DECIMAL(10,2) NULL,                       -- Z坐标(高度)
    [InTime] DATETIME NULL,                       -- 进入时间
    [UpdateTime] DATETIME NOT NULL,                -- 更新时间
    CONSTRAINT [FK_PersonLocation_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- 人员定位历史表(进出记录)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersonHistory]') AND type in (N'U'))
CREATE TABLE [dbo].[PersonHistory] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [CardId] NVARCHAR(20) NOT NULL,
    [PersonName] NVARCHAR(50) NULL,
    [StationId] NVARCHAR(50) NULL,
    [StationName] NVARCHAR(100) NULL,
    [AreaCode] NVARCHAR(20) NULL,
    [AreaName] NVARCHAR(100) NULL,
    [InTime] DATETIME NOT NULL,                    -- 进入时间
    [OutTime] DATETIME NULL,                       -- 离开时间
    [Duration] INT NULL,                           -- 停留时长(秒)
    CONSTRAINT [FK_PersonHistory_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

CREATE INDEX [IX_PersonHistory_CardId_InTime] ON [PersonHistory]([CardId], [InTime]);

-- =============================================
-- 5. 水害防治数据
-- =============================================

-- 水害监测实时数据
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterRealtime]') AND type in (N'U'))
CREATE TABLE [dbo].[WaterRealtime] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [SensorCode] NVARCHAR(20) NOT NULL,
    [SensorName] NVARCHAR(100) NULL,              -- 传感器名称
    [WaterLevel] DECIMAL(10,2) NULL,               -- 水位(米)
    [FlowRate] DECIMAL(10,2) NULL,                 -- 流量(立方米/分)
    [Temperature] DECIMAL(10,2) NULL,             -- 温度(℃)
    [Status] INT DEFAULT 0,                        -- 状态
    [UpdateTime] DATETIME NOT NULL,
    [ReceivedTime] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK_WaterRealtime_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- 水害监测历史数据
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterHistory]') AND type in (N'U'))
CREATE TABLE [dbo].[WaterHistory] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [SensorCode] NVARCHAR(20) NOT NULL,
    [WaterLevel] DECIMAL(10,2) NULL,
    [FlowRate] DECIMAL(10,2) NULL,
    [Temperature] DECIMAL(10,2) NULL,
    [Status] INT DEFAULT 0,
    [UpdateTime] DATETIME NOT NULL,
    [ReceivedTime] DATETIME DEFAULT GETDATE()
);

CREATE INDEX [IX_WaterHistory_MineId_UpdateTime] ON [WaterHistory]([MineId], [UpdateTime]);

-- =============================================
-- 6. 报警管理
-- =============================================

-- 报警配置表(按煤矿+传感器类型)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmConfig]') AND type in (N'U'))
CREATE TABLE [dbo].[AlarmConfig] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,                     -- 煤矿Id(0=全局配置)
    [SensorTypeCode] NVARCHAR(10) NULL,           -- 传感器类型码(为空表示全部类型)
    [SensorTypeName] NVARCHAR(50) NULL,           -- 传感器类型名称
    [AlarmType] INT DEFAULT 1,                     -- 报警类型(1=上限,2=下限,3=差值,4=变化率)
    [AlarmLevel] INT DEFAULT 1,                   -- 报警级别(1=一般,2=重要,3=紧急)
    [ThresholdValue] DECIMAL(18,4) NULL,           -- 阈值
    [ThresholdValue2] DECIMAL(18,4) NULL,          -- 阈值2(用于差值/变化率)
    [DelaySeconds] INT DEFAULT 0,                  -- 报警延时(秒)
    [AlarmEnabled] INT DEFAULT 1,                  -- 报警启用
    [Remark] NVARCHAR(500) NULL,
    [CreateTime] DATETIME DEFAULT GETDATE(),
    [ModifyTime] DATETIME NULL,
    CONSTRAINT [FK_AlarmConfig_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- 报警记录表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlarmRecord]') AND type in (N'U'))
CREATE TABLE [dbo].[AlarmRecord] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [SensorCode] NVARCHAR(20) NULL,
    [SensorName] NVARCHAR(100) NULL,
    [SensorTypeName] NVARCHAR(50) NULL,
    [AlarmType] NVARCHAR(20) NULL,                -- 报警类型描述
    [AlarmLevel] INT DEFAULT 1,                   -- 报警级别
    [AlarmValue] DECIMAL(18,4) NULL,              -- 报警值
    [ThresholdValue] DECIMAL(18,4) NULL,          -- 阈值
    [AlarmTime] DATETIME NOT NULL,                -- 报警时间
    [ConfirmTime] DATETIME NULL,                  -- 确认时间
    [ConfirmUserId] BIGINT NULL,                   -- 确认人
    [ConfirmUserName] NVARCHAR(50) NULL,
    [ResolveTime] DATETIME NULL,                   -- 解除时间
    [ResolveUserId] BIGINT NULL,                   -- 解除人
    [ResolveUserName] NVARCHAR(50) NULL,
    [Status] INT DEFAULT 0,                        -- 状态(0=未确认,1=已确认,2=已解除,3=已恢复)
    [Remark] NVARCHAR(500) NULL,
    CONSTRAINT [FK_AlarmRecord_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

CREATE INDEX [IX_AlarmRecord_MineId_AlarmTime] ON [AlarmRecord]([MineId], [AlarmTime]);
CREATE INDEX [IX_AlarmRecord_Status] ON [AlarmRecord]([Status]);

-- =============================================
-- 7. 视频监控
-- =============================================

-- 视频设备表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VideoDevice]') AND type in (N'U'))
CREATE TABLE [dbo].[VideoDevice] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [DeviceCode] NVARCHAR(50) NOT NULL UNIQUE,    -- 设备编号
    [DeviceName] NVARCHAR(100) NULL,              -- 设备名称
    [DeviceType] NVARCHAR(20) NULL,               -- 设备类型(摄像机/NVR/DVR)
    [IpAddress] NVARCHAR(50) NULL,                -- IP地址
    [Port] INT DEFAULT 8000,                      -- 端口
    [Channel] INT DEFAULT 1,                       -- 通道号
    [StreamType] INT DEFAULT 1,                    -- 码流类型(1=主码流,2=子码流)
    [Username] NVARCHAR(50) NULL,                 -- 用户名
    [Password] NVARCHAR(200) NULL,                -- 密码(加密)
    [ProtocolType] NVARCHAR(20) DEFAULT 'GB28181',-- 协议类型
    [StreamUrl] NVARCHAR(500) NULL,               -- 视频流地址
    [Status] INT DEFAULT 0,                        -- 状态(0=离线,1=在线)
    [InstallLocation] NVARCHAR(200) NULL,         -- 安装位置
    [Remark] NVARCHAR(500) NULL,
    [CreateTime] DATETIME DEFAULT GETDATE(),
    [ModifyTime] DATETIME NULL,
    CONSTRAINT [FK_VideoDevice_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- =============================================
-- 8. 系统配置
-- =============================================

-- 系统配置表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SysConfig]') AND type in (N'U'))
CREATE TABLE [dbo].[SysConfig] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [ConfigKey] NVARCHAR(100) NOT NULL UNIQUE,
    [ConfigValue] NVARCHAR(500) NULL,
    [ConfigType] NVARCHAR(20) NULL,               -- 配置类型
    [Remark] NVARCHAR(500) NULL,
    [CreateTime] DATETIME DEFAULT GETDATE(),
    [ModifyTime] DATETIME NULL
);

-- =============================================
-- 9. 数据解析日志
-- =============================================

-- 文件解析日志表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ParseLog]') AND type in (N'U'))
CREATE TABLE [dbo].[ParseLog] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [MineId] BIGINT NOT NULL,
    [FileName] NVARCHAR(200) NOT NULL,
    [FileType] NVARCHAR(20) NOT NULL,             -- 数据类型(CDSS/RYSS等)
    [Encoding] NVARCHAR(20) NULL,                  -- 文件编码
    [FileSize] BIGINT NULL,                        -- 文件大小(字节)
    [RecordCount] INT DEFAULT 0,                   -- 解析记录数
    [SuccessCount] INT DEFAULT 0,                  -- 成功数
    [ErrorCount] INT DEFAULT 0,                    -- 错误数
    [ParseTime] INT NULL,                          -- 解析耗时(毫秒)
    [ErrorMessage] NVARCHAR(1000) NULL,
    [Status] INT DEFAULT 0,                        -- 0=成功,1=部分成功,2=失败
    [ParseTime2] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK_ParseLog_CoalMine] FOREIGN KEY ([MineId]) REFERENCES [CoalMine]([Id])
);

-- =============================================
-- 初始化数据
-- =============================================

-- 传感器类型配置
INSERT INTO [SysConfig] ([ConfigKey], [ConfigValue], [ConfigType], [Remark]) VALUES
('SensorType_A01', '甲烷;%CH4;气体', 'SensorType', '瓦斯/甲烷传感器'),
('SensorType_A02', '一氧化碳;ppm;气体', 'SensorType', '一氧化碳传感器'),
('SensorType_A03', '一氧化氮;ppm;气体', 'SensorType', '一氧化氮传感器'),
('SensorType_A04', '二氧化氮;ppm;气体', 'SensorType', '二氧化氮传感器'),
('SensorType_A05', '二氧化硫;ppm;气体', 'SensorType', '二氧化硫传感器'),
('SensorType_A06', '硫化氢;ppm;气体', 'SensorType', '硫化氢传感器'),
('SensorType_A07', '氨气;ppm;气体', 'SensorType', '氨气传感器'),
('SensorType_A08', '氯气;ppm;气体', 'SensorType', '氯气传感器'),
('SensorType_A09', '氢气;%;气体', 'SensorType', '氢气传感器'),
('SensorType_A10', '氧气;%;气体', 'SensorType', '氧气传感器'),
('SensorType_A11', '二氧化碳;%;气体', 'SensorType', '二氧化碳传感器'),
('SensorType_A12', '粉尘;mg/m3;粉尘', 'SensorType', '粉尘传感器'),
('SensorType_A13', '温度;℃;环境', 'SensorType', '温度传感器'),
('SensorType_A14', '湿度;%RH;环境', 'SensorType', '湿度传感器'),
('SensorType_A15', '风速;m/s;通风', 'SensorType', '风速传感器'),
('SensorType_A20', '水位;m;水害', 'SensorType', '水位传感器'),
('SensorType_A21', '流量;m3/min;水害', 'SensorType', '流量传感器');

-- 网关类型配置
INSERT INTO [SysConfig] ([ConfigKey], [ConfigValue], [ConfigType], [Remark]) VALUES
('GatewayType_Safety', '安全监测', 'GatewayType', '安全监测系统网关'),
('GatewayType_Person', '人员定位', 'GatewayType', '人员定位系统网关'),
('GatewayType_Water', '水害防治', 'GatewayType', '水害防治系统网关'),
('GatewayType_Video', '视频监控', 'GatewayType', '视频监控系统网关');

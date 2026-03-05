# 煤矿安全监测与定位系统

<div align="center">

[![star](https://gitee.com/dyhsoft/Monitor/badge/star.svg?theme=gvp)](https://gitee.com/dyhsoft/Monitor/stargazers)
[![GitHub stars](https://img.shields.io/github/stars/dyhsoft/Monitor?logo=github)](https://github.com/dyhsoft/Monitor/stargazers)
[![GitHub license](https://img.shields.io/badge/license-MIT-yellow)](https://github.com/dyhsoft/Monitor/blob/main/LICENSE)

</div>

## 📋 概述

煤矿安全监测与定位系统是基于 Admin.NET (Furion + Vue3) 开发的工业级应用平台，实现煤矿安全监控数据的实时采集、解析、存储与可视化。

### 技术栈

- **后端**: .NET 8 + Furion + EF Core
- **前端**: Vue3 + Vben-Admin
- **数据库**: SQL Server
- **协议**: 煤矿安全监控系统数据接入协议

## 🏗️ 系统架构

```
┌─────────────────────────────────────────────────────────────┐
│                    数据采集层 (Gateway)                      │
├─────────────────────────────────────────────────────────────┤
│  安全监测 │ 人员定位 │ 水害防治 │ 视频监控                   │
├─────────────────────────────────────────────────────────────┤
│                    数据解析服务 (Parser)                     │
├─────────────────────────────────────────────────────────────┤
│                    业务应用层 (Admin.NET)                    │
├─────────────────────────────────────────────────────────────┤
│  煤矿管理 │ 网关配置 │ 实时监测 │ 报警管理 │ 报表统计           │
└─────────────────────────────────────────────────────────────┘
```

## 📊 数据类型

| 类别 | 文件类型 | 说明 |
|------|----------|------|
| 安全监测 | CDSS/CDDY/FZSS/KGBH/TJSJ/YCBJ | 模拟量/开关量/断电器/馈电/提升/远程 |
| 人员定位 | RYSS/RYCS/RYCY/JZSS | 实时/初始化/出勤/基站 |
| 水害防治 | CGKCDSS/CGKCDDY/JSLCDSS/PSLCDSS | 水仓/流量/排水 |

### 数据文件格式

- **命名规则**: `{煤矿编号}_{数据类型}_{时间戳}.txt`
- **字段分隔符**: `;`
- **记录分隔符**: `~`
- **煤矿编号**: 10位 (省2+市2+县2+序号4)
- **编码**: UTF-8/GBK/GB2312

## 🚀 功能模块

- [x] 煤矿管理 - 煤矿基础信息维护
- [x] 网关配置 - 数据网关参数设置
- [x] 安全监测 - 实时数据监控
- [ ] 人员定位 - 人员位置与轨迹
- [ ] 水害防治 - 水位/流量监控
- [ ] 报警管理 - 阈值配置与报警记录
- [ ] 视频监控 - 海康威视接入

## 📁 项目结构

```
MonitorProject/
├── backend/          # .NET 8 后端
├── backend-mixed/    # 混合架构后端
├── frontend-vue3/    # Vue3 前端
├── doc/              # 接口文档
└── docs/             # 项目文档
```

## 🔗 相关资源

- GitHub: https://github.com/dyhsoft/Monitor
- 协议文档: /coal-mine-docs/

## 📄 协议参考

- DB14_1226-2019 煤矿安全监测系统数据采集传输规范
- 煤安监办〔2019〕42号 煤矿安全监控数据接入细则

# MaiziWPF

> 基于 .NET 10 + WPF 的企业级后台管理系统，采用 ABP 模块化架构与 Prism MVVM 框架构建。

## 项目状态

> ⏸️ **当前阶段：暂停更新**
>
> 本项目已完成核心功能开发，暂时进入维护阶段。后续将作为 **AI 数字人** 的后台管理系统进行深度集成，届时将根据实际需求继续迭代。

### 已完成功能

| 模块 | 状态 | 说明 |
|------|------|------|
| 用户管理 | ✅ | 用户列表、新增/编辑、角色分配、数据权限 |
| 角色管理 | ✅ | 角色列表、菜单权限分配、数据权限分配、用户授权 |
| 菜单管理 | ✅ | 菜单树管理、按钮权限 |
| 部门管理 | ✅ | 组织架构树、部门选择 |
| 岗位管理 | ✅ | 岗位列表、岗位分配 |
| 字典管理 | ✅ | 字典类型与字典数据维护 |
| 参数配置 | ✅ | 系统参数配置管理 |
| 通知公告 | ✅ | 公告发布、维护与详情查看 |
| OSS 文件管理 | ✅ | 文件上传（S3 兼容）、文件列表管理 |
| 个人中心 | ✅ | 用户个人信息编辑 |
| 仪表盘 | ✅ | 系统首页概览 |
| 图标选择 | ✅ | Material Design 图标选择器 |

### 待完成功能

| 模块 | 状态 | 说明 |
|------|------|------|
| 登录日志 |  | 登录历史记录查询（UI 已实现，数据对接待完善） |
| 操作日志 | 🚧 | 系统操作日志记录与详情查看（UI 已实现，数据对接待完善） |
| 系统设置 | 🚧 | 系统全局配置管理（待实现） |

### 未来规划

- **AI 数字人集成**：本项目将作为 AI 数字人的后台管理系统，提供用户管理、权限控制、消息推送、文件存储等基础设施支持
- **实时交互**：通过 MQTT 协议实现数字人与后台系统的实时通信
- **数据联动**：数字人可调用后台 API 获取用户信息、角色权限、组织架构等数据

## 项目简介

**MaiziWPF** 是一个使用 **WPF（.NET 10）** 技术栈构建的桌面端后台管理系统，参考了 **若依（RuoYi-Vue-Plus）** 管理系统的功能设计理念，并借鉴了 **ABP（Volo.Abp）** 框架的模块化分层架构思想。项目采用 **Prism** 实现 MVVM 模式与模块化加载，使用 **Material Design Themes** 提供现代化的 Material Design 风格 UI 界面，集成 **MQTTnet** 实现实时消息推送，使用 **SQLite** 作为本地数据库缓存离线消息。

## 技术栈

| 技术 | 说明 |
|------|------|
| **.NET 10.0** | 目标运行时 |
| **WPF** | 桌面客户端框架 |
| **Prism 9.0** | MVVM 框架，模块化、导航、对话框、区域管理 |
| **DryIoc** | Prism 集成 DI 容器 |
| **Volo.Abp 10.1.0** | ABP 框架核心，模块化与依赖注入 |
| **Material Design Themes 5.3.0** | Material Design 风格 UI 组件库 |
| **FreeSql 3.5.306** | 国产 ORM，支持 MySQL + SQLite 双数据库 |
| **MQTTnet 4.3.7** | MQTT 协议客户端，实时消息推送与在线状态管理 |
| **AWSSDK.S3 4.0+** | S3 兼容对象存储客户端（支持 RustFS/MinIO 等） |
| **SQLite** | 本地嵌入式数据库，缓存离线消息与同步状态 |
| **Serilog** | 结构化日志记录（按天滚动） |
| **H.NotifyIcon** | 系统托盘图标支持 |
| **BCrypt.Net** | 密码哈希加密 |
| **Newtonsoft.Json** | JSON 序列化（MQTT 消息编解码） |
| **Fody/Rougamo** | AOP 面向切面编程 |

## 项目架构

项目采用 **分层 + 模块化** 架构，各层职责清晰：

```
MaiziWPF.slnx
├── MaiziWPF                              # WPF 主应用（Shell）
├── MaiziWPF.Core                         # 核心 UI 层（公共控件、转换器、模型、附加属性）
├── MaiziWPF.Common                       # 通用工具层（安全、树构建、字符串扩展）
├── MaiziWPF.Modules.Sys                  # 系统管理模块（Prism Module）
│
├── Services/
│   ├── MaiziWPF.Services.Application           # 应用服务层（业务逻辑、MQTT、本地DB）
│   ├── MaiziWPF.Services.Application.Contracts  # 应用服务接口/DTO
│   ├── MaiziWPF.Services.Domain                # 领域层（实体定义、仓储接口）
│   ├── MaiziWPF.Services.Domain.Shared         # 领域共享类型（查询输入、枚举、审计）
│   └── MaiziWPF.Services.MySql                 # MySQL 数据库仓储实现
```

### 各层详细说明

| 项目 | 职责 | 关键内容 |
|------|------|----------|
| **MaiziWPF** | 主应用入口 | Shell 窗口、ABP 初始化、Prism 模块加载、Serilog 配置、TabControl Region 适配器 |
| **MaiziWPF.Core** | 核心 UI 层 | 自定义控件、值转换器、附加属性（权限/字典绑定）、基础 ViewModel、MQTT 配置、TabControl Region 适配器 |
| **MaiziWPF.Common** | 通用工具层 | SecurityUtils（超级管理员判断）、TreeListHelper（树形构建）、StringExtensions |
| **MaiziWPF.Modules.Sys** | 系统管理模块 | 用户/角色/菜单/部门/岗位/字典/配置/通知/OSS/日志等全部业务视图与 ViewModel、DataScopeItem（数据范围选项） |
| **Services.Application** | 应用服务层 | 业务逻辑实现、MqttService、LocalDbService、MessagePublisher、PermissionService、CurrentUserService |
| **Services.Application.Contracts** | 服务契约层 | 全部服务接口定义（ICurrentUserService、IMqttService、ILocalDbService、IMessagePublisher、IPermissionService、ISysUserService、ISysRoleService、ISysMenuService、ISysDeptService、ISysPostService、ISysDictService、ISysConfigService、ISysNoticeService、ISysOssService、ISysOperLogService、ISysLogininforService、ISysMessageService） |
| **Services.Domain** | 领域层 | 实体定义（SysUser/SysRole/SysMenu/SysDept/SysPost/SysDict/SysConfig/SysNotice/SysOss/SysOperLog/SysLogininfor/SysMessage 等）、仓储接口 |
| **Services.Domain.Shared** | 领域共享层 | 查询输入 DTO、UserStatus 枚举、AuditUserContext（审计用户静态上下文）、DataPermissionManager、DataScopeType 枚举、TransactionalAttribute、IPagingInfo |
| **Services.MySql** | MySQL 仓储层 | 全部 FreeSql 仓储实现，MySQL 数据库访问，AuditValueHandler（AOP 审计处理） |

### 依赖关系

```
MaiziWPF ──────────────► MaiziWPF.Core
     │                     MaiziWPF.Modules.Sys
     │                     Services.Application
     │                     Services.MySql
     │
     ├── MaiziWPF.Core ──► MaiziWPF.Common
     │                     Services.Application.Contracts
     │
     ├── MaiziWPF.Modules.Sys ──► MaiziWPF.Core
     │                             Services.Application.Contracts
     │                             Services.Domain
     │
     ├── Services.Application ──► Services.Application.Contracts
     │                            Services.Domain
     │                            MaiziWPF.Common
     │
     ├── Services.Application.Contracts ──► Services.Domain
     │
     ├── Services.Domain ──► Services.Domain.Shared
     │
     └── Services.MySql ──► Services.Domain
```

## 功能模块

### 系统管理模块（Sys Module）

| 功能 | 视图 | 说明 |
|------|------|------|
| **仪表盘** | `DashboardView` | 系统首页概览 |
| **用户管理** | `UserListView` / `UserFormView` | 用户列表、新增/编辑、角色分配、数据权限 |
| **角色管理** | `RoleListView` / `RoleFormView` / `RolePermissionView` / `RoleAuthUserView` / `AuthRoleView` | 角色列表、菜单权限分配、数据权限分配（全部/本部门/本部门及以下/自定义）、用户授权 |
| **菜单管理** | `MenuListView` / `MenuFormView` | 菜单树管理、按钮权限 |
| **部门管理** | `DeptListView` / `DeptFormView` | 组织架构树、部门选择 |
| **岗位管理** | `PostListView` / `PostFormView` | 岗位列表、岗位分配 |
| **字典管理** | `DictListView` / `DictTypeFormView` / `DictDataFormView` | 字典类型与字典数据维护 |
| **参数配置** | `ConfigListView` / `ConfigFormView` | 系统参数配置管理 |
| **通知公告** | `NoticeListView` / `NoticeFormView` / `NoticeDetailView` | 公告发布、维护与详情查看 |
| **OSS 文件管理** | `OssListView` | 文件上传与管理 |
| **操作日志** | `OperLogListView` / `OperLogDetailView` | 系统操作日志记录与详情查看 |
| **登录日志** | `LoginInfoListView` | 登录历史记录查询 |
| **个人中心** | `ProfileView` | 用户个人信息编辑（昵称、手机、邮箱、性别、密码） |
| **图标选择** | `IconPickerView` | Material Design 图标选择器 |

### 实时消息系统

| 组件 | 说明 |
|------|------|
| `MqttService` | MQTT 客户端服务，支持全局广播（`message/all`）、用户点对点（`message/user/{id}`）、控制指令（`user/{id}/control`） |
| `LocalDbService` | SQLite 本地数据库，缓存消息已读状态（`local_msg_status`）与同步位点（`local_sync_state`） |
| `MessagePublisher` | 消息发布器，支持通知公告推送、系统消息推送、工作流消息推送，自动触发 MQTT 广播 |
| `ICurrentUserService` | 当前用户服务，维护登录用户信息、菜单树、权限集合、角色标识 |

**MQTT 消息流程：**

```
发布公告/系统消息 → MessagePublisher → 写入 sys_message 表 → MQTT Publish
                                                              ↓
客户端 MqttService 订阅 ← message/all / message/user/{id} ← MQTT Broker
         ↓
收到消息 → LocalDbService 缓存 → UI 通知（OnNewMessages 事件）
```

**MQTT 控制指令：**

- **踢下线**：`user/{id}/control` → `action=kick` → 触发 `OnKicked` 事件
- **禁用账号**：`user/{id}/control` → `action=disable` → 触发 `OnKicked` 事件
- **离线消息同步**：连接成功后自动拉取未读消息（`PullOfflineMessagesAsync`）

### 文件存储系统

| 组件 | 说明 |
|------|------|
| `S3ClientService` | S3 兼容对象存储客户端，封装上传/下载/删除/列举操作 |
| `OssUploadViewModel` | 文件上传 UI 逻辑，支持批量选择、进度显示、结果统计 |
| `OssOptions` | OSS 配置项（端点、密钥、桶名、路径前缀等） |

**核心特性：**

- **S3 兼容协议**：基于 AWSSDK.S3，支持 RustFS、MinIO、阿里云 OSS 等任何 S3 兼容服务
- **批量上传**：支持多文件同时选择，逐个上传并显示进度
- **进度反馈**：通过 `ProgressStream` 包装实现实时上传进度百分比
- **自动路径**：按 `upload/yyyy/MM/dd/{uuid}.{ext}` 格式生成文件 Key
- **Content-Type 识别**：自动识别 15+ 种文件类型（图片、文档、表格等）
- **桶自动创建**：启动时自动检查并创建存储桶
- **记录持久化**：上传成功后写入 `sys_oss` 表，记录文件名、路径、服务类型

**配置方式**（`appsettings.json`）：

```json
{
  "Oss": {
    "Endpoint": "http://127.0.0.1:9000",
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key",
    "BucketName": "maizi-wpf",
    "ForcePathStyle": true,
    "UseHttps": false,
    "Prefix": "upload",
    "TimeoutSeconds": 300,
    "MaxErrorRetry": 1
  }
}
```

### 自定义控件

| 控件 | 说明 |
|------|------|
| `DateRangeControl` | 日期范围选择器 |
| `DeptTreePanelControl` | 部门树面板 |
| `DeptTreeSelectControl` | 部门树下拉选择 |
| `ExpanderMenuControl` | 可展开菜单 |
| `MultiSelectComboBox` | 多选下拉框 |
| `SearchableComboBox` | 可搜索下拉框 |
| `PageControl` | 分页控件 |
| `UserSelectControl` | 用户选择器 |
| `BorderlessDialogWindow` | 无边框对话框 |
| `TabControlRegionAdapter` | TabControl 区域适配器，支持动态 Tab 页管理 |

### 附加属性（Assists）

| 附加属性 | 说明 |
|----------|------|
| `PermsAssist.Perms` | 按钮级权限控制，无权限自动隐藏元素 |
| `DataDictAssist.Type` | 字典数据自动绑定，ComboBox 自动加载字典项 |
| `DbAssist.Bind` | 数据库字段绑定辅助，支持初始选中项 |
| `GridViewAssist` | DataGrid 视图辅助 |

### 核心模型

| 模型 | 说明 |
|------|------|
| `Checked` | 可勾选项基类，支持选中状态管理 |
| `FormBindableBase` | 表单绑定基类，支持验证与错误提示 |
| `PageBindableBase` | 分页绑定基类，支持分页参数管理 |
| `DataScopeItem` | 数据范围选项（全部/本部门/本部门及以下/自定义/仅本人） |

### 值转换器

| 转换器 | 说明 |
|--------|------|
| `BooleanConverter` | 布尔值转换 |
| `InverseBooleanConverter` | 布尔值取反 |
| `VisibilityConverter` | 布尔→可见性转换 |
| `EditModeVisibilityConverter` | 编辑模式可见性 |
| `DateTimeConverter` | 日期时间格式化 |
| `FirstCharConverter` | 取首字符（头像占位） |
| `ListToStringConverter` | 列表→字符串拼接 |
| `StringToPackIconKindConverter` | 字符串→Material Design 图标 |

## 截图预览

![登录界面](https://github.com/mymaizi/MaiziWPF/blob/master/temp/login.png "登录界面")
![菜单管理](https://github.com/mymaizi/MaiziWPF/blob/master/temp/menu.png "菜单管理")
![部门管理](https://github.com/mymaizi/MaiziWPF/blob/master/temp/dept.png "部门管理")
![岗位管理](https://github.com/mymaizi/MaiziWPF/blob/master/temp/post.png "岗位管理")
![字典管理](https://github.com/mymaizi/MaiziWPF/blob/master/temp/dict.png "字典管理")
![参数配置](https://github.com/mymaizi/MaiziWPF/blob/master/temp/user.png "参数配置")
![角色管理](https://github.com/mymaizi/MaiziWPF/blob/master/temp/role.png "角色管理")

## 开发环境

- **IDE:** Visual Studio 2022+ / JetBrains Rider
- **SDK:** .NET 10.0 SDK
- **数据库:** MySQL 8.0+（主数据库）、SQLite（本地缓存，自动创建）
- **MQTT Broker:** 需部署 MQTT 消息中间件（如 EMQX / Mosquitto）
- **操作系统:** Windows 10/11

## 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/mymaizi/MaiziWPF.git
cd MaiziWPF
```

### 2. 配置 MQTT

在 `MaiziWPF/appsettings.json` 中配置 MQTT 服务器地址：

```json
{
  "Mqtt": {
    "ServerIp": "127.0.0.1",
    "ServerPort": 1883
  }
}
```

### 3. 配置数据库与对象存储

MySQL 连接字符串在 ABP 模块初始化时配置，确保 MySQL 数据库已创建并可访问。SQLite 本地数据库在应用启动时自动创建于 `%LocalAppData%/MaiziWPF/local.db`。

如需使用文件上传功能，在 `appsettings.json` 中配置 OSS 信息（见上方"文件存储系统"章节）。

### 4. 还原依赖并运行

```bash
dotnet restore
dotnet run --project MaiziWPF
```

## 项目特点

- **模块化架构**：基于 ABP 模块化思想，业务模块可插拔
- **MVVM 模式**：Prism 驱动的 ViewModel-First 开发模式
- **Material Design**：现代化 Material Design 风格界面
- **权限控制**：RBAC 角色-菜单-按钮级权限控制（`PermsAssist` 附加属性）
- **数据权限**：支持 5 种数据范围（全部/本部门/本部门及以下/自定义/仅本人），通过 `DataPermissionManager` 与 FreeSql `GlobalFilter` 实现；角色权限分配界面支持菜单权限与数据权限的可视化配置（展开/折叠、全选、父子联动）
- **审计上下文**：`AuditUserContext` 静态类为 FreeSql AOP 审计提供用户信息，解耦 ORM 层与应用服务层
- **实时消息**：MQTT 协议实现消息推送、在线状态管理、踢下线/禁用控制
- **离线缓存**：SQLite 本地数据库缓存消息已读状态与同步位点
- **双数据库**：MySQL（主库）+ SQLite（本地缓存），FreeSql 统一 ORM
- **日志系统**：Serilog 结构化日志，按天滚动保存，操作日志完整记录
- **Fody 编织**：Rougamo AOP 实现横切关注点分离
- **字典绑定**：`DataDictAssist` 附加属性实现 ComboBox 字典数据自动加载
- **系统托盘**：H.NotifyIcon 支持最小化到托盘
- **分页控件**：`PageControl` 支持分页参数管理与数据绑定
- **表单验证**：`FormBindableBase` 基类支持表单验证与错误提示
- **Tab 管理**：`TabControlRegionAdapter` 支持动态 Tab 页管理与区域导航

## License

MIT License
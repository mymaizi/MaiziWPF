# MaiziWPF

> 基于 .NET 10 + WPF 的企业级后台管理系统，采用 ABP 模块化架构与 Prism MVVM 框架构建。

## 项目简介

**MaiziWPF** 是一个使用 **WPF（.NET 10）** 技术栈构建的桌面端后台管理系统，参考了 **若依（RuoYi-Vue-Plus）** 管理系统的功能设计理念，并借鉴了 **ABP（Volo.Abp）** 框架的模块化分层架构思想。项目采用 **Prism** 实现 MVVM 模式与模块化加载，使用 **Material Design Themes** 提供现代化的 Material Design 风格 UI 界面。

## 技术栈

| 技术 | 说明 |
|------|------|
| **.NET 10.0** | 目标运行时 |
| **WPF** | 桌面客户端框架 |
| **Prism 9.0** | MVVM 框架，模块化、导航、对话框、区域管理 |
| **DryIoc** | Prism 集成 DI 容器 |
| **Volo.Abp 10.1.0** | ABP 框架核心，模块化与依赖注入 |
| **Material Design Themes 5.3.0** | Material Design 风格 UI 组件库 |
| **FreeSql** | 国产 ORM，支持 MySQL 数据库 |
| **Serilog** | 结构化日志记录（按天滚动） |
| **H.NotifyIcon** | 系统托盘图标支持 |
| **BCrypt.Net** | 密码哈希加密 |
| **Fody/Rougamo** | AOP 面向切面编程 |

## 项目架构

项目采用 **分层 + 模块化** 架构，各层职责清晰：

```
MaiziWPF.slnx
├── MaiziWPF                              # WPF 主应用（Shell）
├── MaiziWPF.Core                         # 核心 UI 层（公共控件、转换器、模型）
├── MaiziWPF.Common                       # 通用工具层
├── MaiziWPF.Modules.Sys                  # 系统管理模块（Prism Module）
│
├── Services/
│   ├── MaiziWPF.Services.Application           # 应用服务层（业务逻辑）
│   ├── MaiziWPF.Services.Application.Contracts  # 应用服务接口/DTO
│   ├── MaiziWPF.Services.Domain                # 领域层（实体定义）
│   ├── MaiziWPF.Services.Domain.Shared         # 领域共享类型
│   └── MaiziWPF.Services.MySql                 # MySQL 数据库实现
```

### 依赖关系

```
MaiziWPF ────────────► MaiziWPF.Core
     │                  MaiziWPF.Modules.Sys ──► MaiziWPF.Core
     │                  Services.Application ──► Services.Domain
     │                  Services.MySql ────────► Services.Domain
     │
     └──► MaiziWPF.Modules.Sys
     └──► Services.Application
     └──► Services.MySql
```

## 功能模块

### 系统管理模块（Sys Module）

| 功能 | 说明 |
|------|------|
| **用户管理** | 用户列表、新增/编辑、角色分配、数据权限 |
| **角色管理** | 角色列表、权限分配、用户授权 |
| **菜单管理** | 菜单树管理、按钮权限 |
| **部门管理** | 组织架构树、部门选择 |
| **岗位管理** | 岗位列表、岗位分配 |
| **字典管理** | 字典类型与字典数据维护 |
| **参数配置** | 系统参数配置管理 |
| **通知公告** | 公告发布与维护 |
| **OSS 文件管理** | 文件上传与管理 |
| **操作日志** | 系统操作日志记录与查看 |
| **登录日志** | 登录历史记录查询 |

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

## 截图预览

![登录界面](https://raw.githubusercontent.com/mymaizi/wpfdemo/refs/heads/master/login.png "登录界面")
![菜单管理](https://raw.githubusercontent.com/mymaizi/wpfdemo/refs/heads/master/menu.png "菜单管理")
![部门管理](https://raw.githubusercontent.com/mymaizi/wpfdemo/refs/heads/master/dept.png "部门管理")
![岗位管理](https://raw.githubusercontent.com/mymaizi/wpfdemo/refs/heads/master/post.png "岗位管理")

## 开发环境

- **IDE:** Visual Studio 2022+ / JetBrains Rider
- **SDK:** .NET 10.0 SDK
- **数据库:** MySQL 8.0+
- **操作系统:** Windows 10/11

## 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/mymaizi/MaiziWPF.git
cd MaiziWPF
```

### 2. 配置数据库

在 `MaiziWPF/appsettings.json` 中配置 MySQL 连接字符串：

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=maizi_wpf;User=root;Password=your_password;"
  }
}
```

### 3. 还原依赖并运行

```bash
dotnet restore
dotnet run --project MaiziWPF
```

## 项目特点

- **模块化架构**：基于 ABP 模块化思想，业务模块可插拔
- **MVVM 模式**：Prism 驱动的 ViewModel-First 开发模式
- **Material Design**：现代化 Material Design 风格界面
- **权限控制**：RBAC 角色-菜单-按钮级权限控制
- **数据权限**：支持按部门的数据范围权限
- **日志系统**：Serilog 结构化日志，操作日志完整记录
- **Fody 编织**：Rougamo AOP 实现横切关注点分离

## License

MIT License
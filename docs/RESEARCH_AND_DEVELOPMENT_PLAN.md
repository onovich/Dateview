# Dateview 研发方案

具体执行文档见：

- [Dateview P0-P7 Goal 模式执行指南](P0_P7_GOAL_MODE_EXECUTION_GUIDE.md)
- [Dateview 分阶段研发计划](PHASED_RESEARCH_AND_DEVELOPMENT_PLAN.md)

## 项目理解

Dateview 是一个面向 Windows 的轻量托盘日历工具。核心体验不是“独立日历应用”，而是从任务栏自然弹出的时间感知增强层：普通用户权限运行、离线优先、无遥测、不修改 Explorer、不注入任务栏，使用托盘图标承载入口。

MVP 聚焦三个闭环：托盘生命周期、6 x 7 月历网格、中国节假日/调休离线标注。视觉与交互要贴近 Windows 11 原生日历弹层，包括任务栏贴边定位、多屏/DPI、轻量动画、无 Alt+Tab、无任务栏窗口按钮。

## 技术路线

- 平台：C#、.NET 10、WPF。
- UI 入口：`System.Windows.Forms.NotifyIcon`，只有遇到明确限制才引入 Win32 `Shell_NotifyIcon`。
- 架构：`Desktop -> Application -> Domain`，`Desktop -> Infrastructure -> Application -> Domain`。
- 数据：`System.Text.Json` 读取打包的 `assets/holidays/cn/{year}.json`。
- 测试：xUnit，所有日期逻辑使用 fake clock，禁止依赖真实当前日期。
- 质量门禁：`dotnet restore`、`dotnet build --configuration Release`、`dotnet test --configuration Release`、`dotnet format --verify-no-changes`。

## 阶段计划

### P0 仓库与工程骨架

- 建立 solution、四层项目、测试项目、`.editorconfig`、`Directory.Build.props`。
- 固定 nullable、warnings-as-errors、基础 git/ops workflow。
- 输出 `AGENTS.md`、holiday 数据说明和研发方案。

验收：Release build 和测试命令可运行，仓库可从远端 clone 后恢复。

### P1 Domain 日历引擎

- 实现 `CalendarMonth`、`CalendarDay`、`DayMarker`、`MonthGrid`。
- 实现 42 格月历生成、首日规则、跨月 filler、today/weekend 分类。
- 覆盖周一开头、周日开头、闰年二月、today marker 测试。

验收：Domain 无 UI/IO/时间系统依赖。

### P2 假期数据管线

- 定义 JSON schema 与验证器。
- 导入 2025、2026 官方来源数据，保留 source metadata。
- 实现 `JsonHolidayRepository` 与缓存。
- 测试无效日期、重复日期、未知类型、年份不匹配、调休覆盖周末。

验收：缺失年份有明确用户态信息，不虚构未来调休。

### P3 Application 用例层

- 实现 `GetMonthCalendarUseCase`、settings use cases、startup toggle use case。
- 定义 `IClock`、`IHolidayRepository`、`ISettingsStore`、`IAutoStartService`。
- 输出 UI 无关 DTO，避免 view model 直接计算日历。

验收：Application 不引用 WPF、WinForms、Registry、文件系统实现。

### P4 WPF 弹层与托盘生命周期

- 实现托盘图标、左键切换、右键菜单、退出。
- 实现 Calendar popup：贴近任务栏/当前屏幕、Escape 关闭、失焦收起。
- 建立“任务栏延伸”动画：fade + translate，控制 120–180ms。
- 单实例守卫，第二次启动安全退出或唤起现有实例。

验收：不出现在 Alt+Tab，不显示任务栏按钮，多屏和 PerMonitorV2 DPI 行为正确。

### P5 设置、自启动与打包

- 实现 `%APPDATA%/ChinaTrayCalendar/settings.json` 原子保存。
- 实现 HKCU 当前用户自启动注册，路径安全加引号。
- 增加 release publish profile、图标、README 和故障排查文档。

验收：普通用户权限完成安装后使用路径，设置跨重启持久化。

## 风险与对策

- 节假日数据每年变化：将数据导入做成文档化流程，只接受官方来源。
- 多屏/DPI 定位复杂：把几何计算抽成可测试服务，WPF 只做窗口应用。
- UI 逻辑泄漏：所有日历计算和状态转换先进入 Domain/Application，code-behind 只保留窗口桥接。
- 过度系统融合：MVP 只做托盘弹窗 Level 1，禁止 Shell hook、Explorer 修改和管理员权限。

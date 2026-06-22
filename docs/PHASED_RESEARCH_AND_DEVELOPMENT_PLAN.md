# Dateview 分阶段研发计划

日期：2026-06-22

状态：P0 仓库初始化已完成。本文把现有研发方案拆成可逐阶段执行、可验证、可提交推送的研发计划。

执行程序员使用的完整 Goal 指南见：

- [Dateview P0-P7 Goal 模式执行指南](P0_P7_GOAL_MODE_EXECUTION_GUIDE.md)

## 0. 会话轮定义

本文的“会话轮”指一次可独立交付的 Codex 工作轮，而不是单条聊天消息。一轮应包含：

- 明确目标和影响范围。
- 代码或文档变更。
- Debug 自检和架构自检。
- 相关验证命令及结果。
- commit hash 和 push 结果。

每轮未验证通过不得提交；提交后未推送成功不得进入下一轮。

## 1. 总体预算

P0 已完成，不计入剩余预算。MVP 剩余预计 30 轮；若 Windows 托盘、DPI、多屏或假期数据来源出现额外问题，预留 3 到 5 轮弹性。

| Phase | 状态 | 预估会话轮数 | 主体实现 | 缓冲修复 | 最终验证 | 核心产物 |
| --- | --- | ---: | ---: | ---: | ---: | --- |
| P0 仓库与工程骨架 | 已完成 | 1 | 1 | 0 | 0 | solution、四层项目、测试项目、workflow |
| P1 Domain 日历引擎 | 待做 | 3 | 2 | 0 | 1 | 42 格月历、日期模型、基础分类测试 |
| P2 假期数据管线 | 待做 | 5 | 4 | 0 | 1 | JSON schema、parser、repository、官方数据导入流程 |
| P3 Application 用例层 | 待做 | 4 | 3 | 0 | 1 | ports、DTO、use cases、跨层测试 |
| P4 WPF 月历弹层 | 待做 | 6 | 4 | 1 | 1 | popup、月历 UI、导航、状态呈现 |
| P5 托盘与 UX Fusion | 待做 | 5 | 3 | 1 | 1 | NotifyIcon、单实例、贴边定位、多屏/DPI/动画 |
| P6 设置与自启动 | 待做 | 4 | 3 | 0 | 1 | settings store、HKCU startup、设置入口 |
| P7 打包与发布整理 | 待做 | 3 | 2 | 0 | 1 | icon、publish profile、README、troubleshooting |

## 2. 直接给执行者的 Goal Prompt

```text
请在 D:\ToolProjects\Dateview 中执行 Dateview 下一阶段研发。

必读：
- AGENTS.md
- docs/PHASED_RESEARCH_AND_DEVELOPMENT_PLAN.md
- docs/DEVELOPMENT_PLAN.md
- docs/UX_FUSION_ADDENDUM.md
- docs/HOLIDAY_DATA.md

当前状态：P0 已完成，下一阶段从 P1 Domain 日历引擎开始。

执行规则：
- 每轮先确认 git status，避免纳入无关文件。
- 每轮只做当前 phase 的最小闭环。
- 每轮必须包含 Debug 自检、架构自检、验证命令与结果。
- 验证通过后提交并推送；推送成功后才能进入下一轮。
- 不要引入第三方包，除非先在 docs/adr/ 写短 ADR。
- 严守依赖方向：Desktop -> Application -> Domain；Desktop -> Infrastructure -> Application -> Domain。
```

## 3. 每轮固定工作流

1. 读取当前 phase 目标、AGENTS 边界和相关 docs。
2. 执行 `git status --short --branch`，确认没有无关改动。
3. 做最小可交付变更。
4. 补测试或文档。
5. 运行本轮相关验证；源代码轮默认运行：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

6. 本轮总结必须报告：

- 本轮目标。
- 完成内容。
- Debug 自检。
- 架构自检。
- 验证命令和结果。
- commit hash 和 push 结果。
- 下一轮目标。
- 是否消耗缓冲轮。

## 4. P1 Domain 日历引擎

预估：3 轮。

目标：在 `ChinaTrayCalendar.Domain` 中建立纯业务日历模型与 42 格月历生成，不引入 UI、IO、JSON、系统时间依赖。

### P1.1 模型和验证

- 实现 `CalendarMonth`、`CalendarDay`、`DayMarker`、`MonthGrid`。
- 定义年份范围 1900-2100、月份范围 1-12。
- 添加参数验证和异常语义。
- 测试无效 year/month、基础构造。

验证：

```powershell
dotnet test tests\ChinaTrayCalendar.Domain.Tests\ChinaTrayCalendar.Domain.Tests.csproj --configuration Release
```

### P1.2 42 格 MonthGridBuilder

- 实现 Monday 默认首日。
- 支持自定义 `DayOfWeek firstDayOfWeek`。
- 固定输出 42 cells，包含前后月 filler。
- 覆盖周一开头、周日开头、月中开头、闰年二月测试。

### P1.3 today/weekend 分类与最终验证

- 支持 `DateOnly today` 输入。
- 输出 `IsToday`、`IsWeekend`、`IsInCurrentMonth`。
- 清理 API 命名和测试结构。
- 运行完整 `Validate.cmd`，提交推送。

PASS：

- Domain 无项目引用。
- 没有 `DateTime.Now`、`DateTime.UtcNow`、`TimeZoneInfo.Local`。
- 所有日期测试 deterministic。

## 5. P2 假期数据管线

预估：5 轮。

目标：建立中国节假日离线数据格式、验证器、repository 和 day marker 分类能力。

### P2.1 假期领域模型和 schema 草案

- 定义 `HolidayDay`、`HolidayDayType`、source metadata 模型。
- 明确 `dayOff`、`adjustedWorkday`、预留 `festivalOnly`。
- 更新 `docs/HOLIDAY_DATA.md`。

### P2.2 JSON parser 和验证器

- 使用 `System.Text.Json`。
- 拒绝 invalid date、duplicate date、unknown type、mismatched year、unsupported jurisdiction。
- 覆盖 parser 单元测试。

### P2.3 JsonHolidayRepository

- 从 `assets/holidays/cn/{year}.json` 或输出目录读取。
- 进程内缓存 per year。
- 对缺失 future data 给出可被 Application/UI 呈现的结果或异常策略。

### P2.4 官方数据导入

- 验证官方来源后导入 2025、2026 数据。
- 不推断未来调休。
- 每年 JSON 保留 source title、publishedDate、source URL 或来源说明。

注意：此轮需要查验官方来源，执行时必须使用权威来源，并在文档中记录来源。

### P2.5 分类集成与最终验证

- 将 day-off、adjusted workday 应用到月历分类。
- adjusted workday 必须覆盖 weekend 视觉语义。
- 运行完整 `Validate.cmd`，提交推送。

PASS：

- 无效 JSON 有明确错误。
- 缺失年份不会虚构数据。
- Domain/Application 仍不承担 JSON IO 职责。

## 6. P3 Application 用例层

预估：4 轮。

目标：把 Domain 和 Infrastructure 之间的业务入口稳定下来，让 UI 只消费用例输出。

### P3.1 Ports、settings 模型和 DTO

- 定义 `IClock`、`IHolidayRepository`、`ISettingsStore`、`IAutoStartService`。
- 定义 `AppSettings`、calendar DTO。
- 确认所有 async IO 端口带 `CancellationToken`。

### P3.2 GetMonthCalendarUseCase

- 计算 42 格可见年份集合。
- 加载相关年份假期。
- 组合 Domain 月历和假期分类。
- 覆盖跨年 filler、缺失数据、fake clock 测试。

### P3.3 Settings 和 startup use cases

- 实现 load/save settings 用例。
- 实现 startup toggle 用例。
- 验证默认值、无效设置回退或错误呈现策略。

### P3.4 跨层验证

- Application 不引用 WPF、WinForms、Registry、文件系统。
- 运行完整 `Validate.cmd`，提交推送。

PASS：

- UI 可通过 use case 拿到完整月历 DTO。
- 所有 wall-clock 访问都通过 `IClock`。

## 7. P4 WPF 月历弹层

预估：6 轮。

目标：实现可用的 WPF 月历 popup，但暂不把托盘生命周期做复杂化。

### P4.1 ViewModel 和命令骨架

- 实现 `CalendarViewModel`。
- 状态包含 `DisplayedMonth`、`MonthTitle`、`WeekdayHeaders`、cells、loading、error。
- 命令包含 previous、next、today、close。

### P4.2 月历视觉布局

- 实现 6 x 7 grid。
- today、weekend、dayOff、adjustedWorkday 不只靠颜色区分。
- 使用中文默认 UI 文案，逐步迁移到 central string provider。

### P4.3 导航和状态

- 连接 use case。
- 支持月份切换、回到今天、loading/error 状态。
- 避免 UI 线程执行文件 IO 或解析。

### P4.4 Windows 11 视觉基础

- Segoe UI Variable。
- 8-12px 圆角、弱阴影、清爽固定主题或主题探测占位。
- 不做过度装饰。

### P4.5 缓冲修复

- 修复布局、绑定、测试或 warning 问题。
- 保证 code-behind 只做 `InitializeComponent` 和必要视觉桥接。

### P4.6 最终验证

- 运行完整 `Validate.cmd`。
- 做一次手动 WPF smoke 记录：打开窗口、切换月份、回到今天、错误状态路径可呈现。
- 提交推送。

PASS：

- ViewModel 不引用 WPF 控件。
- XAML converter 不计算日历业务逻辑。
- Popup 具备可继续接入托盘的边界。

## 8. P5 托盘与 UX Fusion

预估：5 轮。

目标：把 Dateview 变成真正从通知区启动和交互的 Windows 托盘体验。

### P5.1 NotifyIcon lifecycle

- 实现 tray icon service。
- 创建、显示、dispose 生命周期明确。
- 右键菜单包含 Today、Settings、Start with Windows、Exit 占位或可用项。

### P5.2 左键切换 popup

- 左键打开/关闭 Calendar popup。
- App 启动不显示普通 main window。
- popup 不显示任务栏按钮。

### P5.3 单实例和干净退出

- 实现 single-instance guard。
- 第二次启动安全退出或唤起现有实例。
- App shutdown dispose tray icon。

### P5.4 任务栏贴边、多屏和动画

- 基于鼠标点击点确定屏幕。
- popup 贴近任务栏边缘，禁止默认居中。
- 打开/关闭动画遵循 UX Fusion：fade + translate，约 120-180ms。
- 支持 PerMonitorV2 DPI 策略。

### P5.5 最终验证

- 手动 smoke：托盘显示、左键切换、右键菜单、ESC、外部点击、退出。
- 运行完整 `Validate.cmd`，提交推送。

PASS：

- 不 hook Shell，不修改 Explorer，不需要管理员权限。
- 不出现在 Alt+Tab，不显示任务栏窗口按钮。

## 9. P6 设置与自启动

预估：4 轮。

目标：让用户设置可持久化，并实现当前用户自启动。

### P6.1 JsonSettingsStore

- 保存到 `%APPDATA%/ChinaTrayCalendar/settings.json`。
- 目录不存在时创建。
- 原子保存：temp file + replace/move。
- 覆盖 round trip、missing file、corrupt JSON 测试。

### P6.2 WindowsAutoStartService

- 使用 HKCU 当前用户 startup location。
- 安全引用 executable path。
- 不写 HKLM，不要求管理员权限。
- 将 registry 访问封装在 Infrastructure。

### P6.3 Settings UI 和菜单绑定

- 连接 Start with Windows toggle。
- 支持 first day of week。
- theme setting 可作为 placeholder，避免拉大 scope。

### P6.4 最终验证

- 运行完整 `Validate.cmd`。
- 手动 smoke：设置保存、重启后读取、自启动开关读写。
- 提交推送。

PASS：

- 设置失败有安全用户态错误。
- UI 不直接访问 registry/file system。

## 10. P7 打包与发布整理

预估：3 轮。

目标：形成可交付的 Release build 和使用文档。

### P7.1 图标和 publish profile

- 添加 icon assets。
- 配置 Windows desktop publish。
- 验证 release 输出目录。

### P7.2 README 和 troubleshooting

- 写 README：功能、运行、构建、测试、隐私/权限、假期数据更新。
- 写 troubleshooting：托盘图标隐藏、DPI、多屏、设置路径、启动项。

### P7.3 最终 release dry-run

- 运行完整 `Validate.cmd`。
- 运行 package/publish。
- 从 fresh output 做 smoke。
- 提交推送。

PASS：

- 用户可按 README 从源码构建并运行。
- Release artifact 生成路径清晰。

## 11. Debug 自检模板

每轮至少回答：

- 当前变更能否用最小测试 fixture 或用户工作流解释？
- 失败能否定位到具体层：Domain、Application、Infrastructure、Desktop、tooling？
- 成功、失败、空数据、缺失数据、无效数据状态是否覆盖？
- UI 改动是否有可重复 smoke 验证？
- 状态或配置改动是否覆盖 load、save、validate、migration 边界？

## 12. 架构自检模板

每轮至少回答：

- 当前 source-of-truth 层是否仍是 source of truth？
- UI/host 是否避免复制 Domain/Application 语义？
- JSON、registry、file IO 是否只在 Infrastructure？
- 是否避免把未来 scope 拉进当前 phase？
- 是否保留无关文件和用户改动？

## 13. Phase 完成报告模板

```text
Phase:
预估轮数:
实际轮数:
完成内容:
未完成内容:
验证:
已推送 commit:
风险:
下一 phase:
```

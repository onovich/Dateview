# Dateview P0-P7 Goal 模式执行指南

日期：2026-06-22

状态：交给专门执行程序员使用的完整 MVP 研发目标指南。P0 已完成并推送，P1-P7 需要按本文继续执行。

## 0. 直接给执行程序员的 Goal Prompt

```text
你是 Dateview 的执行程序员。请在 D:\ToolProjects\Dateview 中，把 P0-P7 作为一个完整 MVP Goal 执行到可交付状态。

目标：交付一个 Windows .NET 10 WPF 托盘日历应用。它通过通知区托盘图标打开贴近任务栏的月历 popup，离线显示中国节假日和调休，支持设置、自启动、打包与基础发布文档。不得修改 Explorer，不得 hook Shell，不得要求管理员权限，不得引入遥测。

必读：
- AGENTS.md
- docs/P0_P7_GOAL_MODE_EXECUTION_GUIDE.md
- docs/PHASED_RESEARCH_AND_DEVELOPMENT_PLAN.md
- docs/RESEARCH_AND_DEVELOPMENT_PLAN.md
- docs/DEVELOPMENT_PLAN.md
- docs/UX_FUSION_ADDENDUM.md
- docs/HOLIDAY_DATA.md

执行规则：
- 每一轮先运行 git status，确认没有无关文件。
- 每一轮只做本文指定的最小可交付闭环。
- 每一轮必须包含 Debug 自检、架构自检、验证命令与结果。
- 每一轮验证通过后提交并推送；推送成功后才能进入下一轮。
- 每一轮汇报 commit hash、push 结果、下一轮目标、是否消耗 buffer。
- 遇到架构边界、scope 扩张、第三方依赖、官方假期数据争议、Windows Shell/DPI/托盘行为不确定时，暂停并向架构/项目负责人请求决策。
```

## 1. 角色分工

执行程序员负责：

- 按轮次实现代码、测试、文档。
- 保持每轮变更小而可验证。
- 运行本轮验证、提交、推送。
- 在轮次总结中暴露风险、阻塞和下一步。

架构设计、项目管理和技术指导负责：

- 守住分层架构和 MVP scope。
- 审核 ADR、第三方依赖、官方数据来源和 Windows 行为取舍。
- 在每个 phase 完成后做阶段验收。
- 处理跨 phase 优先级、轮次预算和风险缓冲分配。
- 对 UI/UX Fusion、DPI、多屏、托盘行为提出验收口径。

## 2. 当前基线与下一步

P0 已完成：

- `.NET 10` solution 和四层项目已创建。
- xUnit 测试项目已创建。
- Codex git/ops workflow 已配置。
- `origin/main` 已推送过初始提交。

下一步从 P1 开始。执行程序员第一轮仍需做 P0 接收检查，确认本机 clone/build/test 状态可靠。

## 3. 总轮次预算

总 Goal 预算：36 轮。

- P0 接收与基线确认：1 轮。
- P1-P7 主线研发：30 轮。
- 跨阶段 buffer：4 轮。
- 最终总验收与交付报告：1 轮。

| 轮次 | Phase | 类型 | 目标 |
| --- | --- | --- | --- |
| R1 | P0 | 基线确认 | 确认仓库、远端、workflow、build/test/format 可运行 |
| R2-R4 | P1 | 主线 | Domain 日历引擎 |
| R5-R9 | P2 | 主线 | 假期数据管线 |
| R10-R13 | P3 | 主线 | Application 用例层 |
| R14-R19 | P4 | 主线 | WPF 月历弹层 |
| R20-R24 | P5 | 主线 | 托盘与 UX Fusion |
| R25-R28 | P6 | 主线 | 设置与自启动 |
| R29-R31 | P7 | 主线 | 打包与发布整理 |
| R32-R35 | Buffer | 缓冲 | 修复跨阶段缺陷、Windows 行为问题、数据问题 |
| R36 | Final | 总验收 | 完整验证、release dry-run、最终报告 |

Buffer 只能用于已经暴露的具体问题，不能用来引入新功能。

## 4. 本 Goal 要完成什么

MVP 完成时必须具备：

- 普通用户权限启动。
- 单实例 Windows desktop app。
- 通知区 tray icon。
- 左键打开/关闭月历 popup。
- 右键菜单：Today、Settings、Start with Windows、Exit。
- 6 x 7 月历网格，默认周一为一周首日。
- today、weekend、中国 day-off、adjusted workday 标注。
- 离线假期 JSON 数据和验证。
- 设置持久化到当前用户 profile。
- HKCU 当前用户自启动开关。
- Windows 11 风格基础视觉、任务栏贴边、多屏/DPI 策略。
- Release publish 输出、README、troubleshooting 文档。

## 5. 本 Goal 不做什么

- 不 hook Windows Shell。
- 不修改 Explorer 行为。
- 不注入任务栏 UI。
- 不替换或隐藏系统时钟。
- 不要求管理员权限。
- 不接入在线假期 API。
- 不做日历账号同步。
- 不做 lunar calendar，除非 MVP 完成后另开 goal。
- 不引入第三方包，除非先写 ADR 并经架构负责人确认。

## 6. 架构边界

依赖方向必须保持：

```text
Desktop -> Application -> Domain
Desktop -> Infrastructure -> Application -> Domain
Infrastructure -> Application -> Domain
Domain -> no project dependencies
```

Domain：

- 只放日期模型、月历生成、day classification。
- 禁止 WPF、WinForms、Win32、JSON、文件系统、Registry、网络、真实系统时间。

Application：

- 只放 use cases、ports、UI 无关 DTO。
- 禁止 WPF/WinForms、Registry/file-system 实现、JSON parsing 细节。

Infrastructure：

- 实现 JSON holiday repository、settings store、HKCU startup adapter。
- 禁止 UI 行为和业务规则。

Desktop：

- WPF、ViewModels、tray lifecycle、composition root。
- code-behind 只做 `InitializeComponent`、必要视觉事件桥接和窗口 placement plumbing。

## 7. 每轮固定工作流

每轮开始：

```powershell
git status --short --branch
```

每轮实现：

- 只改本轮相关文件。
- 新行为必须有测试或手动 smoke 记录。
- 文档和行为不一致时，同时更新文档。
- 不处理无关 TODO，不做无关重构。

每轮验证：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

若本轮只改文档，可运行：

```powershell
git diff --check
```

但如果文档改动影响 build/test 命令、项目结构或 public behavior，仍需运行完整 `Validate.cmd`。

每轮提交推送：

```powershell
git status --short --branch
git diff --stat
git add <本轮相关文件>
git commit -m "<phase>: <round summary>"
git push
git status --short --branch
```

不要 stage 无关 untracked files。验证失败不得提交；提交失败不得推送；推送失败不得进入下一轮。

## 8. 每轮汇报模板

```text
Round:
Phase:
本轮目标:
完成内容:
Debug 自检:
架构自检:
验证命令与结果:
commit hash:
push 结果:
是否消耗 buffer:
风险/阻塞:
下一轮目标:
需要架构/PM 决策:
```

## 9. Debug 自检模板

每轮至少回答：

- 当前变更能否用最小测试 fixture 或最小用户工作流解释？
- 失败能否定位到具体层：Domain、Application、Infrastructure、Desktop、tooling？
- 成功、失败、空数据、缺失数据、无效数据是否覆盖？
- 如果 UI 改了，是否有可重复的 smoke 验证？
- 如果状态或配置改了，load、save、validate、migration 边界是否覆盖？

## 10. 架构自检模板

每轮至少回答：

- source-of-truth 层是否仍是 source of truth？
- UI 是否避免复制 Domain/Application 语义？
- JSON、Registry、file IO 是否只在 Infrastructure？
- 是否避免把未来 scope 拉进当前 phase？
- 是否保留无关文件、生成输出和用户改动？

## 11. 架构/PM 升级规则

出现以下情况必须暂停请求决策：

- 想引入第三方包。
- 发现现有 phase 切分无法保持依赖方向。
- 需要改变假期数据来源或数据可信度不足。
- 需要使用 Win32 API 替代 `NotifyIcon`。
- 需要改变 MVP 非目标。
- UI 为满足 Windows 行为需要牺牲架构边界。
- 完整验证连续两轮失败且原因不明。
- buffer 预计超过 4 轮。

## 12. 分轮安排

### R1 P0 接收与基线确认

目标：

- 确认 `main...origin/main`。
- 运行完整 `Validate.cmd`。
- 确认 P0 产物存在：solution、四层项目、测试项目、workflow、docs。

PASS：

- 工作区干净。
- 验证通过。
- P1 可开始。

### R2 P1.1 Domain 模型

- 实现 `CalendarMonth`、`CalendarDay`、`DayMarker`、`MonthGrid`。
- 覆盖 year/month validation。
- 删除或替换 marker-only 占位测试。

### R3 P1.2 MonthGridBuilder

- 生成固定 42 cells。
- 支持 Monday 默认 first day。
- 支持自定义 first day。
- 覆盖周一开头、周日开头、月中开头、闰年二月。

### R4 P1.3 Domain classification final

- 加入 `IsToday`、`IsWeekend`、`IsInCurrentMonth`。
- 确认 Domain 无项目引用、无真实系统时间。
- 运行完整验证并推送。

P1 PASS：

- Domain 日历引擎 deterministic。
- Domain 不知道 UI、IO、JSON、Registry。

### R5 P2.1 Holiday model and docs

- 定义假期模型和类型。
- 更新 `docs/HOLIDAY_DATA.md` schema。
- 确定 dayOff、adjustedWorkday、festivalOnly 预留语义。

### R6 P2.2 JSON parser validator

- 使用 `System.Text.Json`。
- 拒绝 invalid date、duplicate date、unknown type、mismatched year、unsupported jurisdiction。
- 加 parser tests。

### R7 P2.3 JsonHolidayRepository

- 实现 repository 和 per-year cache。
- 确定缺失年份策略。
- 测试 missing file、valid file、invalid file。

### R8 P2.4 Official data import

- 查验官方来源。
- 导入 2025、2026 数据。
- 保留 source metadata 和 source URL/说明。
- 不推断 2027 调休。

### R9 P2.5 Holiday classification final

- 将假期 marker 集成到月历分类。
- adjusted workday 覆盖 weekend。
- 运行完整验证并推送。

P2 PASS：

- 无效假期数据被拒绝。
- 缺失 future year 不虚构数据。
- IO/JSON 仍在 Infrastructure。

### R10 P3.1 Ports and DTOs

- 定义 `IClock`、`IHolidayRepository`、`ISettingsStore`、`IAutoStartService`。
- 定义 `AppSettings` 和 calendar DTO。
- 所有可能 IO 的 async port 带 `CancellationToken`。

### R11 P3.2 GetMonthCalendarUseCase

- 计算 visible years。
- 组合 Domain month grid 和 holiday data。
- 覆盖跨年 filler、fake clock、缺失假期数据测试。

### R12 P3.3 Settings/startup use cases

- 实现 load/save settings 用例。
- 实现 startup toggle 用例。
- 覆盖默认值和无效设置。

### R13 P3.4 Application final

- 检查 Application 不引用 WPF/WinForms/Registry/file system。
- 清理命名和 DTO 边界。
- 运行完整验证并推送。

P3 PASS：

- UI 可通过 use case 拿到完整日历 DTO。
- wall-clock 只通过 `IClock`。

### R14 P4.1 CalendarViewModel

- 实现 view model 状态和命令。
- previous/next/today/close 命令可测试。
- ViewModel 不引用 WPF 控件。

### R15 P4.2 Calendar popup layout

- 实现 6 x 7 月历布局。
- today、weekend、holiday、workday 有非纯色标识。
- 中文默认文案集中放置或为后续资源化留口。

### R16 P4.3 Navigation and states

- 连接 use case。
- 支持 month navigation、today、loading、error。
- 避免 UI 线程 IO/parse。

### R17 P4.4 Windows 11 visual baseline

- Segoe UI Variable。
- 8-12px 圆角、弱阴影、清爽主题。
- 不做营销式/装饰性 UI。

### R18 P4.5 Popup polish buffer

- 修复 binding、layout、warning、test。
- code-behind 只保留允许的视觉桥接。

### R19 P4.6 Popup final validation

- 完整验证。
- 手动 smoke：打开窗口、切换月份、回今天、错误状态。
- 推送。

P4 PASS：

- UI 不计算日历业务。
- Popup 可独立手动验证。

### R20 P5.1 Tray icon lifecycle

- 实现 tray icon service。
- 创建、显示、dispose 清晰。
- 右键菜单基本项存在。

### R21 P5.2 Popup toggle

- 左键打开/关闭 popup。
- App 启动不显示普通 main window。
- popup 无任务栏按钮。

### R22 P5.3 Single instance

- 实现单实例守卫。
- 第二次启动安全退出或唤起已有实例。
- shutdown 清理 tray icon。

### R23 P5.4 Placement, DPI, animation

- 基于点击点确定屏幕。
- 贴近任务栏边缘，不居中。
- fade + translate 动画 120-180ms。
- PerMonitorV2 DPI 策略。

### R24 P5.5 Tray/UX final validation

- 手动 smoke：托盘显示、左键切换、右键菜单、ESC、外部点击、退出。
- 完整验证并推送。

P5 PASS：

- 不出现在 Alt+Tab。
- 不显示任务栏窗口按钮。
- 不 hook Shell，不需要管理员权限。

### R25 P6.1 JsonSettingsStore

- `%APPDATA%/ChinaTrayCalendar/settings.json`。
- missing file default。
- corrupt JSON safe error。
- atomic save。

### R26 P6.2 WindowsAutoStartService

- HKCU startup。
- executable path 安全引用。
- Registry 逻辑只在 Infrastructure。
- 不写 HKLM。

### R27 P6.3 Settings UI binding

- settings window 或 popup entry。
- Start with Windows toggle。
- first day of week。
- theme placeholder 不扩 scope。

### R28 P6.4 Settings final validation

- 手动 smoke：保存、重启读取、自启动开关读写。
- 完整验证并推送。

P6 PASS：

- 设置跨重启持久化。
- UI 不直接访问 Registry/file system。

### R29 P7.1 Icon and publish profile

- 添加 icon assets。
- 配置 desktop publish。
- 验证 release output。

### R30 P7.2 README and troubleshooting

- README：功能、运行、构建、测试、隐私、权限、假期数据更新。
- troubleshooting：托盘图标隐藏、DPI、多屏、settings path、startup。

### R31 P7.3 Release dry-run

- 完整验证。
- publish/package。
- fresh output smoke。
- 推送。

P7 PASS：

- 可生成 Release artifact。
- 用户能按文档构建和运行。

### R32-R35 跨阶段 Buffer

只能用于：

- 修复完整验证失败。
- 修复 Windows 托盘/DPI/多屏行为。
- 修正官方假期数据问题。
- 补缺失测试或文档。
- 处理架构负责人确认的 MVP 必需问题。

每个 buffer round 必须说明：

- 为什么需要 buffer。
- 原本 phase/round 是哪一轮。
- 是否影响总验收。

### R36 Final Goal 验收

必须完成：

- `git status --short --branch` 干净。
- 完整 `Validate.cmd` 通过。
- package/publish 通过。
- fresh output 手动 smoke 通过。
- README 和 troubleshooting 覆盖 fresh install/use path。
- 汇总 P0-P7 commit 范围和最终 release artifact 路径。

## 13. 验证矩阵

| 范围 | 必跑验证 | 额外验证 |
| --- | --- | --- |
| Domain | Domain tests + full Validate before push | 无真实时间/IO/JSON 检查 |
| Application | Application tests + full Validate before push | 依赖方向检查 |
| Infrastructure | Infrastructure tests + full Validate before push | settings temp path/Registry adapter smoke |
| Desktop UI | full Validate before push | 手动 WPF smoke |
| Tray/DPI | full Validate before push | 真实 Windows tray smoke |
| Docs only | `git diff --check` | 若影响行为或命令，跑 full Validate |
| Release | full Validate + package/publish | fresh output smoke |

## 14. 整体 PASS 标准

P0-P7 Goal 完成必须满足：

- 所有轮次 commit 已推送。
- `main...origin/main` 干净。
- 完整 `Validate.cmd` 通过。
- Release publish 可生成。
- app 可普通用户权限启动。
- tray icon 可见或在 Windows hidden tray 中可找到。
- 左键可打开/关闭日历 popup。
- 右键菜单可退出。
- 今天、周末、节假日、调休展示正确。
- settings 持久化。
- Start with Windows 使用 HKCU。
- 不 hook Shell，不修改 Explorer，不需要管理员。
- README 能指导 fresh clone/build/run。

## 15. 最终报告模板

```text
Goal: Dateview P0-P7 MVP
总预算轮数:
实际轮数:
完成 phase:
未完成 phase:
主要 commit:
最终验证:
Release artifact:
手动 smoke:
架构偏差:
已消耗 buffer:
遗留风险:
建议下一 Goal:
```

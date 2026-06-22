# Dateview P8 Desktop UX QA Goal 模式执行指南

日期：2026-06-22

状态：P0-P7 已完成并验收通过。本文是下一阶段 P8 的执行指南，用于把当前 MVP 从“工程验收通过”推进到“真实 Windows 桌面交互验收通过”的 Release Candidate 状态。

## 0. 直接给执行者的 Goal Prompt

```text
你是 Dateview P8 的执行程序员/QA 执行者。请在 D:\ToolProjects\Dateview 中执行 P8 Desktop UX QA & Release Candidate Hardening。

背景：P0-P7 已完成，docs/FINAL_ACCEPTANCE.md 记录了 Validate、Package 和 fresh output smoke 已通过。当前唯一明确保留项是尚未人工点击真实托盘图标、右键菜单、ESC、外部点击、多屏/DPI 视觉位置。

目标：在不扩大 MVP scope 的前提下，完成真实 Windows 桌面交互验收，修复过程中发现的 MVP 必需缺陷，并产出 Release Candidate 验收记录。

必读：
- AGENTS.md
- docs/FINAL_ACCEPTANCE.md
- docs/P7_RELEASE_DRY_RUN.md
- docs/P5_TRAY_UX_VALIDATION.md
- docs/P6_SETTINGS_VALIDATION.md
- docs/UX_FUSION_ADDENDUM.md
- docs/TROUBLESHOOTING.md
- docs/P8_DESKTOP_UX_QA_GOAL_MODE_EXECUTION_GUIDE.md

执行规则：
- 每轮先运行 git status，确认没有无关改动。
- 每轮必须包含 Debug 自检、架构自检、验证命令与结果。
- 验证通过后提交并推送；推送成功后才能进入下一轮。
- 不新增功能，不做 lunar calendar、在线数据、安装器、自动更新或任务栏注入。
- 发现需要改变架构边界、使用 Win32 替代 NotifyIcon、引入第三方包或扩大 MVP scope 时，暂停并请求架构/PM 决策。
```

## 1. 必读上下文

上一阶段 PASS 证据：

- `docs/FINAL_ACCEPTANCE.md`：记录 R36 最终验收通过。
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`：已通过。
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`：已通过。
- fresh output smoke：发布 exe 可启动，第二实例退出码 `0`。
- 当前发布产物包含 `assets\holidays\cn\2025.json` 和 `assets\holidays\cn\2026.json`。

P8 的来源：

- 最终验收只做了进程级 fresh smoke。
- 还需要在真实桌面上手动验证托盘、弹窗、右键菜单、ESC、外部点击、设置、自启动、多屏/DPI 和任务栏贴边体验。

## 2. 本阶段要完成什么

P8 要完成：

- 建立 Release Candidate 手动 QA checklist。
- 在真实 Windows 桌面运行发布版 exe。
- 验证 tray icon 可见或可在隐藏托盘中找到。
- 验证左键打开/关闭 popup。
- 验证右键菜单 Today、Settings、Start with Windows、Exit。
- 验证 ESC、外部点击、焦点丢失收起。
- 验证 popup 不出现在 Alt+Tab，不显示任务栏按钮。
- 验证 popup 贴近任务栏边缘，不居中。
- 验证主要 DPI/多屏场景；若当前机器只有单屏，应记录限制并保留可复现几何测试证据。
- 验证 settings 持久化和 HKCU startup toggle，不遗留危险注册表状态。
- 修复 P8 中发现的 MVP 必需缺陷。
- 产出 `docs/P8_DESKTOP_UX_QA_VALIDATION.md`。

## 3. 本阶段不做什么

- 不新增产品功能。
- 不做安装器。
- 不做自动更新。
- 不接入在线节假日 API。
- 不做 lunar calendar。
- 不 hook Shell，不修改 Explorer，不注入任务栏 UI。
- 不要求管理员权限。
- 不引入第三方包，除非先写 ADR 并取得架构/PM 决策。

## 4. 架构边界

P8 允许改动：

- Desktop 层的 WPF popup、tray lifecycle、placement、animation、view model、settings UI。
- Infrastructure 层的 settings/startup 适配器缺陷修复。
- Application/Domain 层只允许修复 P8 暴露的真实业务缺陷；不得因为 UI 方便而迁移职责。
- docs 和 tests。

P8 禁止：

- 在 Desktop code-behind 中加入业务逻辑。
- 在 UI 层解析 holiday JSON 或直接访问 settings file/Registry。
- 在 Domain/Application 中加入 WPF、WinForms、Win32、file IO、Registry。
- 绕过 `IClock` 使用真实当前日期。

## 5. 每轮固定工作流

每轮开始：

```powershell
git status --short --branch
```

每轮验证：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

涉及发布包时额外运行：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
```

每轮提交推送：

```powershell
git status --short --branch
git diff --stat
git add <phase-relevant files>
git commit -m "qa: <round summary>"
git push
git status --short --branch
```

不要 stage 无关 untracked files。验证失败不得提交；提交成功但推送失败不得进入下一轮。

## 6. 每轮回复模板

```text
Round:
本轮目标:
完成内容:
Debug 自检:
架构自检:
手动 QA 证据:
验证命令与结果:
commit hash:
push 结果:
是否消耗 buffer:
风险/阻塞:
下一轮目标:
需要架构/PM 决策:
```

## 7. Debug 自检

每轮至少回答：

- 当前问题能否用最小用户工作流复现？
- 失败能否定位到 Desktop、Application、Infrastructure、Domain 或 tooling？
- 是否覆盖成功、失败、缺失数据、设置损坏、自启动失败、第二实例等状态？
- UI 改动是否有手动 smoke 或可复现测试？
- 注册表或 settings 改动是否能清理和恢复？

## 8. 架构自检

每轮至少回答：

- Desktop 是否仍只负责 UI、tray、placement 和 composition？
- UI 是否避免复制 Domain/Application 业务语义？
- JSON、file IO、Registry 是否仍只在 Infrastructure？
- 是否避免把 P9/P10 功能拉入 P8？
- 是否保留无关文件、发布输出和用户改动？

## 9. 轮次预算

总预算：8 轮。

- R1-R5：主 QA 和必要硬化。
- R6-R7：buffer 修复。
- R8：最终 Release Candidate 验收。

| Round | 类型 | 目标 |
| --- | --- | --- |
| R1 | 主线 | 建立 P8 QA checklist 和当前发布包基线 |
| R2 | 主线 | 托盘入口、右键菜单、退出、单实例真实桌面 QA |
| R3 | 主线 | popup 打开/关闭、ESC、外部点击、焦点、Alt+Tab/任务栏按钮 QA |
| R4 | 主线 | 任务栏贴边、多屏/DPI、动画和 placement QA |
| R5 | 主线 | settings、自启动、holiday data、README/troubleshooting QA |
| R6 | Buffer | 修复 P8 发现的 MVP 必需缺陷 |
| R7 | Buffer | 修复剩余 P8 缺陷或补测试/文档 |
| R8 | Final | Release Candidate 验收、Package、fresh manual smoke、最终报告 |

## 10. 分轮安排

### R1 QA checklist 和发布包基线

目标：

- 读取 P7 和 final acceptance。
- 运行 `Validate.cmd` 和 `Package.cmd`。
- 创建或更新 `docs/P8_DESKTOP_UX_QA_VALIDATION.md`，列出手动 QA checklist。
- 记录发布 exe 路径、假期数据路径、当前 commit。

PASS：

- checklist 完整。
- 发布包存在且可启动。
- 工作区提交推送成功。

### R2 Tray shell QA

目标：

- 从发布目录启动 exe。
- 验证 tray icon 可见或在隐藏托盘中可找到。
- 验证右键菜单项：Today、Settings、Start with Windows、Exit。
- 验证 Exit 后进程退出且 tray icon 清理。
- 验证第二实例退出码 `0`。

PASS：

- tray lifecycle 可手动证明。
- 发现缺陷则修复并补测试或记录限制。

### R3 Popup interaction QA

目标：

- 验证左键打开/关闭 popup。
- 验证 ESC 收起。
- 验证外部点击/焦点丢失收起。
- 验证 popup 不出现在 Alt+Tab，不显示任务栏按钮。
- 验证 Today、Previous、Next 基本交互。

PASS：

- 核心 popup 交互符合 `UX_FUSION_ADDENDUM.md`。
- UI 不新增业务逻辑泄漏。

### R4 Placement, DPI, animation QA

目标：

- 验证 popup 贴近任务栏边缘，不居中。
- 验证当前屏幕弹出策略。
- 如果有多屏，至少验证主屏和副屏点击。
- 如果有不同 DPI，记录缩放组合；如果没有，确认 `PopupPlacementCalculatorTests` 覆盖纯几何路径。
- 验证 opening/closing 动画方向和时长感知。

PASS：

- 实机结果或测试证据足够支持 release candidate。
- 不使用 Shell hook 或 Explorer 修改。

### R5 Settings, startup, data QA

目标：

- 验证 settings 保存和重启读取。
- 验证 Start with Windows toggle 只写 HKCU。
- 验证关闭自启动后注册表状态恢复。
- 验证 2025/2026 holiday data 在发布包中存在且可读取。
- 更新 README/TROUBLESHOOTING 中与实际 QA 不一致的内容。

PASS：

- settings/startup 可证明且可清理。
- 不遗留危险本机状态。

### R6 Buffer fix round

只用于修复 R1-R5 暴露的 MVP 必需缺陷。

必须说明：

- 缺陷复现步骤。
- 修复范围。
- 为什么属于 MVP 必需。
- 验证和回归结果。

### R7 Buffer fix/documentation round

只用于：

- 修复剩余 P8 缺陷。
- 补缺失测试。
- 补 QA 证据或文档。

若 R6-R7 都不需要使用，应在 R8 记录 buffer 未消耗。

### R8 Release Candidate final validation

必须完成：

- `git status --short --branch` 干净。
- `Validate.cmd` 通过。
- `Package.cmd` 通过。
- 从 publish 输出启动 exe。
- 手动 tray/popup/settings/startup smoke 通过。
- `docs/P8_DESKTOP_UX_QA_VALIDATION.md` 记录最终结果。
- 提交并推送。

## 11. PASS 标准

P8 完成必须满足：

- P8 所有实际使用轮次已提交推送。
- `main...origin/main` 干净。
- `Validate.cmd` 通过。
- `Package.cmd` 通过。
- 发布 exe 可启动并保持运行。
- 第二实例安全退出。
- tray icon、右键菜单、Exit 可手动验证。
- 左键 popup、ESC、外部点击、焦点丢失可手动验证。
- popup 不出现在 Alt+Tab，不显示任务栏按钮。
- popup placement 和 DPI/multi-monitor 证据已记录。
- settings 和 startup toggle 可验证且可清理。
- README/TROUBLESHOOTING 与实际行为一致。

## 12. 最终报告模板

```text
Phase: P8 Desktop UX QA & Release Candidate Hardening
预估轮数:
实际轮数:
完成内容:
未完成内容:
手动 QA 环境:
验证:
发布产物:
已推送 commit:
消耗 buffer:
架构偏差:
遗留风险:
建议下一 phase:
```

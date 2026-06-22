# Dateview P10 Compatibility And Trust Goal 模式执行指南

日期：2026-06-22

状态：P9 portable release distribution 已通过。本文用于执行 P10：补齐硬件兼容性证据，并为下一步信任/签名/分发决策准备可审阅材料。

## 0. 直接给执行者的 Goal Prompt

```text
你是 Dateview P10 的执行程序员/QA 执行者。请在 D:\ToolProjects\Dateview 中执行 P10 Compatibility And Trust Readiness。

背景：P9 已通过 portable win-x64 分发验收，docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md 记录了 bundle、hash、manifest、portable smoke 和 startup cleanup。剩余主要风险是：P8/P9 机器只有单显示器 96 DPI，尚未在物理多屏和非 100% DPI 上 spot check；当前分发包仍为 unsigned portable preview，SHA256 只是完整性校验，不是代码签名。

目标：在不新增产品功能的前提下，补齐多屏/DPI/Windows 信任提示相关的验证与决策文档；如发现 MVP 必需兼容性缺陷，则做最小修复并验证。不要购买证书，不要引入 installer，不要实现自动更新。

必读：
- AGENTS.md
- docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md
- docs/P8_DESKTOP_UX_QA_VALIDATION.md
- docs/TROUBLESHOOTING.md
- docs/RELEASE_NOTES.md
- docs/P10_COMPATIBILITY_TRUST_GOAL_MODE_EXECUTION_GUIDE.md

执行规则：
- 每轮先运行 git status，确认没有无关改动。
- 每轮必须包含 Debug 自检、架构自检、验证命令与结果。
- 每轮验证通过后提交并推送；推送成功后才能进入下一轮。
- 不新增日历功能、在线 API、自动更新、installer、Explorer/taskbar injection、Shell hook、HKLM 写入、管理员权限要求或遥测。
- 证书购买、签名服务、installer 技术选型属于架构/PM 决策；P10 只能产出 ADR/建议和验证证据，不能擅自执行。
```

## 1. 必读上下文

上一阶段 PASS 证据：

- `docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md`：P9 release distribution PASS。
- `scripts\package-release.ps1`：已能生成 portable folder、zip、SHA256、release metadata、app-local manifest。
- P9 final artifact shape：`Dateview-0.1.0-preview-win-x64.zip`。
- P9 residual risks：physical multi-monitor/non-100% DPI 未实测；unsigned portable preview。

P10 的定位：

- P10 不是 feature phase。
- P10 是 release readiness phase：兼容性验证、已知风险收敛、信任/签名/分发下一步决策材料。

## 2. 本阶段要完成什么

P10 要完成：

- 建立 `docs/P10_COMPATIBILITY_TRUST_VALIDATION.md`。
- 记录硬件/显示环境矩阵：Windows 版本、显示器数量、缩放、任务栏位置、结果。
- 在可用硬件上执行多屏和非 100% DPI spot checks。
- 若当前机器仍无法提供多屏/DPI，明确记录限制，并用现有 placement tests 和可复现手动步骤支撑 release note 风险。
- 验证 portable bundle 在非 repo 路径启动时 placement、popup、settings、holiday data 基本可用。
- 检查 Windows Defender/SmartScreen/unsigned app 提示的用户文档是否准确；不绕过安全机制。
- 起草或更新 ADR：release trust/signing/installer strategy。ADR 应列出 portable unsigned、code signing、MSIX、WiX/Inno/NSIS 等选项、利弊和推荐下一步。
- 如果发现兼容性缺陷，做最小修复并增加测试或 QA 证据。

## 3. 本阶段不做什么

- 不新增日历功能。
- 不做 installer 实现。
- 不购买或配置代码签名证书。
- 不实现 auto-update。
- 不接入在线服务。
- 不绕过 SmartScreen/Defender。
- 不写 HKLM，不要求管理员权限。
- 不 hook Shell，不修改 Explorer，不注入任务栏。
- 不把 generated release artifacts 提交进 git。

## 4. 架构边界

P10 允许改动：

- Desktop placement/focus/animation 的兼容性修复。
- Desktop tests for placement/shell behavior。
- release docs、troubleshooting、release notes、ADR。
- package script 的非功能性兼容性修复，例如 path robustness。

P10 禁止：

- 在 Domain/Application 中加入 UI/IO/Win32/Registry。
- 在 Desktop code-behind 中加入业务日期/假期逻辑。
- 用“兼容性”为理由引入 Shell hook 或 Explorer 修改。
- 把 installer/signing 变成已实施功能，除非另有架构/PM 决策。

## 5. 每轮固定工作流

每轮开始：

```powershell
git status --short --branch
```

源代码或项目配置改动后运行：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

涉及 release bundle 时运行：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1
```

文档-only 轮次至少运行：

```powershell
git diff --check
```

每轮提交推送：

```powershell
git status --short --branch
git diff --stat
git add <phase-relevant files>
git commit -m "compat: <round summary>"
git push
git status --short --branch
```

不要 stage 无关 untracked files。验证失败不得提交；推送失败不得进入下一轮。

## 6. 每轮回复模板

```text
Round:
本轮目标:
完成内容:
Debug 自检:
架构自检:
硬件/显示证据:
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

- 当前兼容性结论是否能用最小硬件/显示 fixture 或用户工作流解释？
- 失败能否定位到 placement math、WPF shell style、tray lifecycle、package path、docs？
- 是否覆盖单屏、多屏、100% DPI、非 100% DPI、任务栏底部/侧边可测试路径？
- 如果真实硬件不可用，是否明确记录限制和替代证据？
- 如果状态或 startup 被测试，是否恢复原始状态？

## 8. 架构自检

每轮至少回答：

- 兼容性修复是否保持在 Desktop placement/shell/release tooling 边界？
- UI 是否避免复制 Domain/Application 业务语义？
- Registry/file IO 是否仍只在 Infrastructure 或 release script/docs 层？
- 是否避免把 installer/signing/auto-update 拉进当前 phase？
- 是否保留无关文件、generated artifacts 和用户改动？

## 9. 轮次预算

总预算：8 轮。

- R1-R5：主线验证与决策材料。
- R6-R7：buffer 修复。
- R8：最终 P10 验收。

| Round | 类型 | 目标 |
| --- | --- | --- |
| R1 | 主线 | 建立兼容性矩阵和 P10 validation 文档 |
| R2 | 主线 | 多屏/任务栏位置 spot check 或限制记录 |
| R3 | 主线 | 非 100% DPI/缩放 spot check 或限制记录 |
| R4 | 主线 | unsigned app/SmartScreen/Defender 文档与用户路径检查 |
| R5 | 主线 | release trust/signing/installer ADR 草案 |
| R6 | Buffer | 修复发现的 MVP 必需兼容性缺陷 |
| R7 | Buffer | 补验证、文档、ADR 或小修复 |
| R8 | Final | 最终 compatibility/trust readiness 验收 |

## 10. 分轮安排

### R1 兼容性矩阵基线

目标：

- 创建 `docs/P10_COMPATIBILITY_TRUST_VALIDATION.md`。
- 记录当前系统、显示器、DPI、任务栏位置。
- 运行 `Validate.cmd` 和 `package-release.ps1`。
- 明确 P10 不做 installer/signing 实施。

PASS：

- validation 文档存在。
- 当前环境事实清楚。
- bundle 可生成。

### R2 多屏和任务栏位置 spot check

目标：

- 如果有多屏，验证每个屏幕上的 tray/popup placement。
- 如果只能单屏，记录限制，并确认 placement tests 覆盖多边任务栏/工作区几何。
- 如有可安全调整任务栏位置的环境，验证 bottom/left/right/top 中至少两个方向；否则记录限制。

PASS：

- 真实证据或替代测试证据可审阅。
- 发现缺陷则进入最小修复。

### R3 DPI/缩放 spot check

目标：

- 如果可安全更改缩放，验证 125% 或 150% DPI 下 popup placement 和可读性。
- 如果不可更改，记录原因，并确认 DPI-aware placement 代码路径和测试覆盖。
- 验证 text 不溢出关键 controls。

PASS：

- 非 100% DPI 风险被实测或明确保留。
- 文档不虚假承诺未测试硬件。

### R4 Windows trust prompt 文档检查

目标：

- 从用户角度检查 unsigned portable app 可能出现的 Windows/Defender/SmartScreen 文案。
- 更新 README/TROUBLESHOOTING/RELEASE_NOTES，使其说明 SHA256 不是签名。
- 不提供绕过安全机制的危险指引。

PASS：

- 文档准确、克制、不会诱导用户关闭安全功能。

### R5 Release trust/signing ADR

目标：

- 新增 ADR，例如 `docs/adr/0001-release-trust-and-signing.md`。
- 比较 portable unsigned、code signing、MSIX、installer options。
- 给出推荐下一步和需要 PM/预算/证书决策的事项。
- 不实现签名或 installer。

PASS：

- ADR 可供架构/PM 决策。
- 无 scope creep。

### R6 Buffer compatibility fix

只用于修复 R1-R5 暴露的 MVP 必需兼容性缺陷。

### R7 Buffer docs/test fix

只用于补测试、文档、ADR 或 release script 小修复。

### R8 Final P10 validation

必须完成：

- `git status --short --branch` 干净。
- `Validate.cmd` 通过。
- `Package.cmd` 通过。
- `package-release.ps1` 通过。
- P10 validation 文档完整。
- ADR 存在并明确未实施签名/installer。
- 提交并推送。

## 11. PASS 标准

P10 完成必须满足：

- P10 实际使用轮次均已提交推送。
- `main...origin/main` 干净。
- validation/package/bundle 命令通过。
- 多屏/DPI 兼容性证据或限制记录完整。
- unsigned app/trust 文档准确。
- ADR 给出签名/installer 下一步决策材料。
- 未引入 installer、auto-update、online service、telemetry、admin requirement、Shell hook、Explorer injection。

## 12. 最终报告模板

```text
Phase: P10 Compatibility And Trust Readiness
预估轮数:
实际轮数:
完成内容:
未完成内容:
硬件/显示环境:
验证:
兼容性结论:
trust/signing ADR:
已推送 commit:
消耗 buffer:
架构偏差:
遗留风险:
建议下一 phase:
```

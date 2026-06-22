# Dateview P9 Release Distribution Goal 模式执行指南

日期：2026-06-22

状态：P8 Release Candidate 已通过。本指南用于把 Dateview 从“本机发布输出可用”推进到“可交付给普通 Windows 用户试用的分发包与启动体验”。

## 0. 直接给执行者的 Goal Prompt

```text
你是 Dateview P9 的执行程序员。请在 D:\ToolProjects\Dateview 中执行 P9 Release Distribution & Startup Polish。

背景：P8 已通过真实桌面 QA，docs/P8_DESKTOP_UX_QA_VALIDATION.md 记录了托盘、popup、settings、自启动、发布包和 fresh smoke 结果。P9 的目标不是新增日历功能，而是把当前 RC 整理成可交付给普通 Windows 用户试用的分发形态。

目标：产出可重复生成的 release bundle、版本/校验信息、启动体验 polish、发布说明和最终分发验证记录。保持普通用户权限、离线优先、不注入 Explorer、不 hook Shell、不引入遥测。

必读：
- AGENTS.md
- README.md
- docs/P8_DESKTOP_UX_QA_VALIDATION.md
- docs/FINAL_ACCEPTANCE.md
- docs/TROUBLESHOOTING.md
- docs/HOLIDAY_DATA.md
- docs/P9_RELEASE_DISTRIBUTION_GOAL_MODE_EXECUTION_GUIDE.md

执行规则：
- 每轮先运行 git status，确认没有无关改动。
- 每轮必须包含 Debug 自检、架构自检、验证命令与结果。
- 每轮验证通过后提交并推送；推送成功后才能进入下一轮。
- 不引入第三方 installer/build 包，除非先写 ADR 并等待架构/PM 确认。
- 不做自动更新、在线节假日、账号同步、lunar calendar、Explorer 注入、Shell hook、管理员权限安装。
```

## 1. 必读上下文

上一阶段 PASS 证据：

- `docs/P8_DESKTOP_UX_QA_VALIDATION.md`：P8 Release Candidate accepted。
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`：P8 通过。
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`：P8 通过。
- 发布 exe、tray、popup、settings、HKCU startup、holiday data 已在真实桌面 QA 中通过。

P9 的定位：

- P8 证明当前应用可运行。
- P9 让发布产物更适合交付和复验：bundle、hash、version、release notes、clean startup state、distribution smoke。

## 2. 本阶段要完成什么

P9 要完成：

- 定义 Dateview 的首个试用分发形态，默认优先 portable zip/folder bundle。
- 增加可重复的 release bundle 脚本或项目命令。
- 生成版本信息和校验 hash。
- 确认发布包包含 exe、runtime config、dll、assets/holidays/cn/2025.json、assets/holidays/cn/2026.json。
- 确认普通用户解压/运行路径，不需要管理员权限。
- polish 首次启动和自启动说明，避免用户误以为需要安装或修改任务栏。
- 更新 README/TROUBLESHOOTING/release notes。
- 产出 `docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md`。

## 3. 本阶段不做什么

- 不新增日历功能。
- 不新增 lunar calendar。
- 不做在线节假日 API。
- 不做自动更新。
- 不做账号同步。
- 不做遥测。
- 不写 HKLM。
- 不要求管理员权限。
- 不改变任务栏或 Explorer。
- 不选择复杂 installer 技术，除非 ADR 明确比较 portable bundle、MSIX、WiX/Inno/NSIS 等方案并获得架构/PM 决策。

## 4. 架构边界

P9 允许改动：

- release scripts、publish profile、README、TROUBLESHOOTING、release notes。
- Desktop 层中与启动体验、version display、first-run clarity 直接相关的小改动。
- Infrastructure startup/settings 的安全修复，仅限 P9 分发验证暴露的问题。
- tests/docs。

P9 禁止：

- 在 Desktop 层加入业务日期/假期逻辑。
- 在 Domain/Application 层加入 file IO、Registry、WPF、WinForms。
- 把分发脚本耦合到开发者本机绝对路径。
- 把 build output、zip 包、bin/obj 产物提交到 git，除非项目显式决定跟踪 release artifact。

## 5. 每轮固定工作流

每轮开始：

```powershell
git status --short --branch
```

源代码或项目配置改动后运行：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

涉及发布产物时运行：

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
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
git commit -m "release: <round summary>"
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
验证命令与结果:
release artifact/hash:
commit hash:
push 结果:
是否消耗 buffer:
风险/阻塞:
下一轮目标:
需要架构/PM 决策:
```

## 7. Debug 自检

每轮至少回答：

- 当前变更能否用 fresh clone / fresh publish / fresh unzip 用户路径解释？
- 失败能否定位到 publish profile、script、Desktop startup、settings/startup adapter、docs？
- 是否覆盖 missing artifact、stale artifact、wrong version、missing holiday data、second instance、startup toggle cleanup？
- 若改了启动体验，是否做了 published-output smoke？
- 若改了 release script，是否避免硬编码本机绝对路径？

## 8. 架构自检

每轮至少回答：

- release tooling 是否保持在项目/脚本/docs 层，不污染 Domain/Application？
- Desktop 是否仍只负责 UI、tray、placement、composition？
- Registry/file IO 是否仍在 Infrastructure？
- 是否避免把 installer/auto-update/online service 拉进 P9？
- 是否保留无关文件、生成产物和用户改动？

## 9. 轮次预算

总预算：10 轮。

- R1-R7：主线。
- R8-R9：buffer 修复。
- R10：最终分发验收。

| Round | 类型 | 目标 |
| --- | --- | --- |
| R1 | 主线 | 分发策略确认与基线 package 验证 |
| R2 | 主线 | 可重复 release bundle 命令或脚本 |
| R3 | 主线 | 版本信息、hash、artifact manifest |
| R4 | 主线 | README/TROUBLESHOOTING 分发路径更新 |
| R5 | 主线 | startup/settings 清理与 fresh-user smoke |
| R6 | 主线 | portable bundle unzip/run smoke |
| R7 | 主线 | release notes 和 validation 文档 |
| R8 | Buffer | 修复分发脚本、路径、manifest 或 docs 问题 |
| R9 | Buffer | 修复启动体验或 package smoke 问题 |
| R10 | Final | 最终 release distribution 验收 |

## 10. 分轮安排

### R1 分发策略和基线

目标：

- 确认 P8 当前 publish 输出可复现。
- 决定 P9 默认分发形态：portable folder/zip。
- 记录不做 installer 的理由；如果必须 installer，暂停并请求 ADR/架构决策。
- 创建 `docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md` 初版 checklist。

PASS：

- `Validate.cmd` 和 `Package.cmd` 通过。
- 分发策略写入 validation 或 docs。

### R2 Release bundle 命令

目标：

- 增加可重复 bundle 命令或脚本，例如 `scripts/package-release.ps1`。
- 脚本从 repo root 运行，不依赖开发者本机绝对路径。
- 输出到 ignored artifacts/release 目录，避免提交生成包。
- 包含 exe、dll、runtimeconfig、deps、assets。

PASS：

- 脚本可运行。
- 输出结构稳定。
- `.gitignore` 覆盖生成产物。

### R3 Version、hash、manifest

目标：

- 设计版本来源，优先使用项目属性或脚本参数。
- 为 bundle 生成 manifest 或 release metadata。
- 生成 SHA256 hash。
- 文档说明 hash 只作为用户校验，不是签名。

PASS：

- hash 可重复生成。
- manifest 不包含本机私密路径。

### R4 文档分发路径

目标：

- 更新 README：下载/解压/运行/退出/自启动/卸载说明。
- 更新 TROUBLESHOOTING：portable 路径、Windows tray overflow、settings path、startup cleanup。
- 确认 docs 不承诺未实现 installer。

PASS：

- `git diff --check` 通过。
- README 与当前分发形态一致。

### R5 Startup/settings clean-user smoke

目标：

- 在可控临时设置状态下验证 first run。
- 验证 settings file 创建、保存、删除/清理路径。
- 验证 HKCU startup enable/disable 后恢复原状态。
- 不污染用户机器状态。

PASS：

- 记录原始状态和恢复结果。
- 不遗留 HKCU Run 或 settings test residue。

### R6 Portable bundle unzip/run smoke

目标：

- 从 bundle 输出复制/解压到临时目录。
- 从临时目录启动 exe。
- 验证假期 JSON 从相对发布目录读取。
- 验证第二实例退出。

PASS：

- 临时路径 smoke 通过。
- 清理临时目录或记录保留路径。

### R7 Release notes 和 validation

目标：

- 写 release notes：功能、限制、已知风险、校验 hash、使用方法。
- 更新 `docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md`。
- 明确 multi-monitor/DPI 遗留风险来自 P8。

PASS：

- docs 与实际 bundle 一致。
- 没有新增未验证承诺。

### R8 Buffer 修复

仅用于修复 R1-R7 发现的分发脚本、路径、manifest、hash、docs 问题。

### R9 Buffer 启动体验修复

仅用于修复 startup/settings/package smoke 暴露的 MVP 分发必需问题。

### R10 Final release distribution validation

必须完成：

- `git status --short --branch` 干净。
- `Validate.cmd` 通过。
- `Package.cmd` 通过。
- release bundle 脚本通过。
- SHA256 hash 生成。
- 临时目录 unzip/run smoke 通过。
- settings/startup cleanup 通过。
- `docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md` 记录最终结果。
- 提交并推送。

## 11. PASS 标准

P9 完成必须满足：

- P9 所有实际使用轮次已提交推送。
- `main...origin/main` 干净。
- 完整 validation、package、bundle、hash、portable smoke 通过。
- bundle 不依赖本机绝对路径。
- 发布包包含 holiday data。
- 普通用户可解压运行。
- startup toggle 只影响 HKCU，并可恢复。
- README/TROUBLESHOOTING/release notes 与实际行为一致。
- 没有 installer/auto-update/online service scope creep。

## 12. 最终报告模板

```text
Phase: P9 Release Distribution & Startup Polish
预估轮数:
实际轮数:
完成内容:
未完成内容:
分发形态:
artifact path:
SHA256:
验证:
portable smoke:
settings/startup cleanup:
已推送 commit:
消耗 buffer:
架构偏差:
遗留风险:
建议下一 phase:
```

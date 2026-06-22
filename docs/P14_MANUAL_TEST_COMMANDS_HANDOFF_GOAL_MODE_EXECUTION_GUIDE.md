# Dateview P14 Manual Test Commands And Preview Handoff Goal Mode Execution Guide

Date: 2026-06-22

Status: Created after P13 real desktop UX acceptance passed. This phase formalizes the root-level manual testing commands and keeps the local preview handoff easy to run.

## 0. Direct Goal Prompt For The Executor

```text
You are the Dateview P14 executor/programmer. Work in D:\ToolProjects\Dateview.

Goal: formalize the root-level manual testing commands for building the latest preview and starting the current preview app, then document and validate them.

Context:
- The workspace currently has pre-existing untracked helper scripts: BuildLatest.cmd and StartPreview.cmd.
- The user asked for root-level commands that make manual testing easy.
- P13 accepted the P12 tray icon/animation/toggle UX, with remaining limitations documented for tray overflow and subjective animation feel.

Mandatory reading:
- AGENTS.md
- docs/P14_MANUAL_TEST_COMMANDS_HANDOFF_GOAL_MODE_EXECUTION_GUIDE.md
- docs/P13_REAL_DESKTOP_UX_ACCEPTANCE_VALIDATION.md
- docs/PREVIEW_UX_ACCEPTANCE_CHECKLIST.md
- docs/PREVIEW_RELEASE_HANDOFF.md
- README.md
- BuildLatest.cmd, if present
- StartPreview.cmd, if present

Execution rules:
- Start every round with git status and preserve unrelated changes.
- Treat BuildLatest.cmd and StartPreview.cmd as the expected work-in-progress inputs for this phase if they are still untracked.
- Every round must include Debug self-check, architecture self-check, validation commands and results, commit hash, and push result.
- Validation must pass before commit; push must succeed before moving to the next round.
- Do not introduce installer/signing, public release/upload, online dependencies, telemetry, Explorer/taskbar injection, Shell hooks, global hooks, admin requirements, or HKLM writes.
- Keep generated artifacts under artifacts\release ignored and uncommitted.
```

## 1. Phase Intent

P14 makes manual preview testing boring in the good way:

- `BuildLatest.cmd` should build the current portable preview bundle from the repository root.
- `StartPreview.cmd` should start the current generated preview app from the repository root, building first if needed.
- Documentation should tell a tester/developer which command to run.
- The commands should be committed, validated, and safe for normal current-user local testing.

P14 is not a product feature phase and not a public release phase.

## 2. Required Outputs

P14 must produce or update:

- `BuildLatest.cmd`
- `StartPreview.cmd`
- `docs/P14_MANUAL_TEST_COMMANDS_HANDOFF_VALIDATION.md`
- README and/or preview handoff docs with a short root-command pointer where useful.

Optional if useful:

- A short troubleshooting note if command execution policy, missing build output, or stale processes need explanation.

## 3. Explicit Non-Scope

Do not do any of the following:

- No runtime product feature changes.
- No installer, MSIX, signing, certificate setup, public GitHub Release, upload, store publish, or auto-update.
- No online API, telemetry, analytics, or sync.
- No Explorer/taskbar injection, global hooks, Shell hooks, admin requirements, or HKLM writes.
- No generated artifact commit.
- No broad docs rewrite.

If the commands reveal a real product bug, report it and keep fixes minimal and separately justified.

## 4. Command Acceptance Direction

`BuildLatest.cmd` should:

- Run from the repository root by double-click or terminal.
- Call `scripts\package-release.ps1` using PowerShell with `-NoProfile` and `-ExecutionPolicy Bypass`.
- Return nonzero if packaging fails.
- Tell the user where artifacts are written.

`StartPreview.cmd` should:

- Run from the repository root by double-click or terminal.
- Locate the generated `ChinaTrayCalendar.Desktop.exe` under `artifacts\release`.
- Build first by invoking `BuildLatest.cmd` if no generated preview app is found.
- Start the app with the executable's folder as working directory.
- Avoid requiring administrator privileges.
- Avoid changing startup registry or user settings.

## 5. Fixed Workflow Per Round

Start each round:

```powershell
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd
```

For script/docs changes, run:

```powershell
git diff --check
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

For final validation, run:

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
.\BuildLatest.cmd
.\StartPreview.cmd
```

When `StartPreview.cmd` is smoke-tested, stop the launched `ChinaTrayCalendar.Desktop` process afterward and confirm no process remains. Do not leave startup state changed.

## 6. Debug Self-Check

Each round must answer:

- What manual tester workflow did this round cover?
- If it fails, can the issue be localized to root command pathing, package script invocation, artifact discovery, process launch, working directory, or cleanup?
- Did command testing leave any Dateview process running?
- Did command testing create only ignored artifacts under `artifacts\release`?
- Did the scripts work from the repository root without admin privileges?

## 7. Architecture Self-Check

Each round must answer:

- Did this remain a tooling/docs phase?
- Did runtime product code remain unchanged unless a blocking issue required a minimal fix?
- Did the phase avoid installer/signing/public-release/online/telemetry/shell-hook scope?
- Did generated artifacts remain ignored and uncommitted?
- Were unrelated files preserved?

## 8. Round Budget

Estimated total: 4 conversation rounds.

| Round | Type | Goal |
| --- | --- | --- |
| R1 | Main | Baseline and normalize root helper scripts; create P14 validation report. |
| R2 | Main | Document manual command usage in README/handoff docs and validate scripts. |
| R3 | Buffer | Repair command pathing, quoting, process cleanup, or docs clarity if needed. |
| R4 | Final | Full validation, command smoke, package/release smoke, push, and completion report. |

R3 may be unused.

## 9. Round Plan

### R1 - Script Baseline

Goals:

- Review existing `BuildLatest.cmd` and `StartPreview.cmd`.
- Normalize them only if needed.
- Create `docs/P14_MANUAL_TEST_COMMANDS_HANDOFF_VALIDATION.md`.
- Run `git diff --check`.

PASS:

- Scripts are present, readable, root-relative, and documented in the validation report.

### R2 - Documentation And First Smoke

Goals:

- Add a short README/handoff pointer for `BuildLatest.cmd` and `StartPreview.cmd`.
- Run `BuildLatest.cmd`.
- Run `StartPreview.cmd`, then clean up the launched process.
- Record artifact path/hash and process cleanup evidence.

PASS:

- A tester can discover and use the two root commands.
- Command smoke passes.

### R3 - Buffer Repair

Use only for:

- Quoting/path issue.
- Missing artifact discovery issue.
- Process cleanup issue.
- Small docs clarification.

### R4 - Final Validation

Must complete:

- `Validate.cmd` passes.
- `Package.cmd` passes.
- `BuildLatest.cmd` passes.
- `StartPreview.cmd` starts the app.
- Launched process is cleaned up.
- `git diff --check` passes.
- Boundary scan shows no prohibited scope.
- P14 validation report is complete.
- All P14 changes are committed and pushed.

## 10. PASS Criteria

P14 is accepted only if:

- Root `BuildLatest.cmd` and `StartPreview.cmd` are committed.
- README or handoff docs point to the commands.
- `BuildLatest.cmd` generates the portable preview bundle.
- `StartPreview.cmd` starts the generated preview or builds first if missing.
- Validation/package gates pass.
- No generated artifacts are committed.
- No forbidden runtime/distribution scope is introduced.

## 11. Completion Report Template

```text
Phase: P14 Manual Test Commands And Preview Handoff
Estimated rounds:
Actual rounds:
Completed:
Not completed by design:
Validation:
Command smoke:
Package/artifact evidence:
Pushed commits:
Consumed buffer:
Architecture deviations:
Residual risks:
Recommended next phase:
```

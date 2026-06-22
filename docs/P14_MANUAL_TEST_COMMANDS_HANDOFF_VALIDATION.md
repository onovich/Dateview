# P14 Manual Test Commands And Preview Handoff Validation

Date: 2026-06-22
Phase: P14 Manual Test Commands And Preview Handoff
Guide: `docs/P14_MANUAL_TEST_COMMANDS_HANDOFF_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline: P13 Real Desktop UX Acceptance accepted in `docs/P13_REAL_DESKTOP_UX_ACCEPTANCE_VALIDATION.md`

## Scope

P14 formalizes root-level local manual preview commands and a short preview handoff pointer. It is a tooling and documentation phase.

P14 does not add runtime product features, installers, MSIX, signing, certificate setup, public release/upload, auto-update, online APIs, telemetry, Explorer/taskbar injection, Shell hooks, global hooks, admin requirements, or HKLM writes.

Generated artifacts under `artifacts\release` remain ignored and must not be committed.

## Expected Outputs

- `BuildLatest.cmd`
- `StartPreview.cmd`
- `docs\P14_MANUAL_TEST_COMMANDS_HANDOFF_VALIDATION.md`
- Short command pointer in README and/or preview handoff docs
- Validation evidence for `Validate.cmd`, `Package.cmd`, `BuildLatest.cmd`, `StartPreview.cmd`, `git diff --check`, boundary scan, package artifact/hash, and process cleanup

## Script Baseline

`BuildLatest.cmd` status: PASS

- Runs from the repository root by double-click or terminal because it uses `%~dp0` and `pushd`.
- Invokes `powershell -NoProfile -ExecutionPolicy Bypass -File ".\scripts\package-release.ps1"`.
- Captures `%ERRORLEVEL%` and returns nonzero if packaging fails.
- Prints that release files are under `artifacts\release`.
- Does not require administrator privileges.
- Does not write startup registry entries or user settings.

`StartPreview.cmd` status: PASS

- Runs from the repository root by double-click or terminal because it derives `REPO_ROOT` from `%~dp0`.
- Searches under `artifacts\release` for `ChinaTrayCalendar.Desktop.exe`.
- If no generated preview app is found, calls `BuildLatest.cmd` first.
- Starts the app with `start /D "%APP_DIR%" "%APP_EXE%"`, so the executable folder is the working directory.
- Does not require administrator privileges.
- Does not change startup registry entries or user settings.

## Acceptance Matrix

| Item | Status | Evidence / Notes |
| --- | --- | --- |
| `BuildLatest.cmd` is present at repository root | PASS | R1 found existing root script and reviewed content. |
| `BuildLatest.cmd` invokes `scripts\package-release.ps1` with `-NoProfile` and `-ExecutionPolicy Bypass` | PASS | R1 script review. |
| `BuildLatest.cmd` returns nonzero on packaging failure | PASS | R1 script review captures `%ERRORLEVEL%` and exits with that code. |
| `BuildLatest.cmd` tells the user where artifacts are written | PASS | R1 script review: `artifacts\release`. |
| `StartPreview.cmd` is present at repository root | PASS | R1 found existing root script and reviewed content. |
| `StartPreview.cmd` locates generated preview app under `artifacts\release` | PASS | R1 script review. |
| `StartPreview.cmd` builds first if preview app is missing | PASS | R1 script review calls `BuildLatest.cmd` when `APP_EXE` is not defined. |
| `StartPreview.cmd` starts with executable folder as working directory | PASS | R1 script review uses `start /D "%APP_DIR%"`. |
| README/handoff docs point to root commands | PENDING | R2 documentation. |
| `BuildLatest.cmd` smoke passes | PENDING | R2/R4 validation. |
| `StartPreview.cmd` smoke starts app and process is cleaned up | PENDING | R2/R4 validation. |
| Final package/hash evidence recorded | PENDING | R2/R4 validation. |
| Boundary scan has no prohibited scope | PENDING | R4 final validation. |

## Round Log

### R1 - Script Baseline

Status: PASS

Scope:

- Reviewed existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- Determined both scripts already match the P14 command acceptance direction.
- Created this P14 validation report.
- Left runtime product code unchanged.

Debug self-check:

- Manual tester workflow covered: root-level build command and root-level start command baseline review.
- Failure localization: command failures can be localized to root command pathing, PowerShell package script invocation, artifact discovery, process launch, working directory, or cleanup.
- Process cleanup: R1 did not launch Dateview.
- Generated artifacts: R1 did not create generated artifacts.
- Admin requirement: scripts use normal current-user commands and no admin elevation.

Architecture self-check:

- R1 stayed in tooling/docs.
- Runtime product code remained unchanged.
- No installer/signing/public-release/online/telemetry/shell-hook scope was introduced.
- Generated artifacts remain ignored and uncommitted.
- No unrelated files were modified.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: R1 start showed `BuildLatest.cmd` and `StartPreview.cmd` as untracked expected P14 inputs.
- `git diff --check`: passed after this R1 document add.

Commit / push:

- R1 baseline commit: pending.

Risk / blocked:

- None for R1.

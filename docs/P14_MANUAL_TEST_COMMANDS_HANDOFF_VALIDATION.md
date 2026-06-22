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
| README/handoff docs point to root commands | PASS | R2 added concise root-command pointers to `README.md` and `docs\PREVIEW_RELEASE_HANDOFF.md`. |
| `BuildLatest.cmd` smoke passes | PASS | R2 command smoke generated `Dateview-0.1.0-preview-win-x64.zip` under `artifacts\release`. |
| `StartPreview.cmd` smoke starts app and process is cleaned up | PASS | R2 command smoke started `ChinaTrayCalendar.Desktop.exe`, confirmed a responding process, then stopped it with no remaining process. |
| Final package/hash evidence recorded | PASS | R4 final `BuildLatest.cmd` generated zip bytes `176355`, SHA256 `ee7ec7fadea164574e9337fe40be29c1401d09c9554b5e9443eca6a34985facf`. |
| Boundary scan has no prohibited scope | PASS | R4 scoped scan over `src`, `tests`, `scripts`, `BuildLatest.cmd`, and `StartPreview.cmd` found no prohibited implementation-scope matches. |

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

### R2 - Documentation And First Smoke

Status: PASS

Scope:

- Added README and preview handoff pointers for `BuildLatest.cmd` and `StartPreview.cmd`.
- Ran `BuildLatest.cmd` from the repository root.
- Ran `StartPreview.cmd` from the repository root.
- Confirmed the launched Dateview process and cleaned it up.
- Left runtime product code unchanged.

Documentation evidence:

- `README.md` now lists the local manual preview commands under `Portable Release Bundle`.
- `docs\PREVIEW_RELEASE_HANDOFF.md` now includes a project-side manual testing note before the external portable zip install flow.

Command smoke evidence:

- `.\BuildLatest.cmd`: passed.
- Generated zip: `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`.
- Zip bytes: `176379`.
- SHA256: `3ed1053d7829f82708144e86a006956afba6b548119f83f4d0c472cd44aeafae`.
- SHA256 file: `3ed1053d7829f82708144e86a006956afba6b548119f83f4d0c472cd44aeafae  Dateview-0.1.0-preview-win-x64.zip`.
- Release metadata `gitCommit`: `16733a2`.
- Release metadata `generatedAtUtc`: `2026-06-22T15:51:16.2962649+00:00`.
- Release manifest files: `13`.
- `.\StartPreview.cmd`: passed.
- Started executable: `artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\ChinaTrayCalendar.Desktop.exe`.
- Started process: PID `41668`, responding, no main window title.
- Cleanup: stopped `ChinaTrayCalendar.Desktop`; no remaining process.

Debug self-check:

- Manual tester workflow covered: discover root commands in docs, build latest preview, start current generated preview, and clean up the launched process.
- Failure localization: command smoke would localize failures to package script invocation, artifact generation, artifact discovery, process launch, working directory, or cleanup.
- Process cleanup: no Dateview process remained after smoke.
- Generated artifacts: only ignored artifacts under `artifacts\release` were created/refreshed.
- Admin requirement: commands ran as normal current-user commands without elevation.

Architecture self-check:

- R2 stayed in tooling/docs.
- Runtime product code remained unchanged.
- No installer/signing/public-release/online/telemetry/shell-hook scope was introduced.
- Generated artifacts remain ignored and uncommitted.
- No unrelated files were modified.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: tracked tree clean at R2 start.
- `.\BuildLatest.cmd`: passed.
- `.\StartPreview.cmd`: passed and launched the generated app.
- Process cleanup: passed.
- `git diff --check`: passed after R2 document updates.

Commit / push:

- R2 docs and smoke evidence commit: pending.

Risk / blocked:

- None for R2.

### R3 - Buffer Repair

Status: NOT USED BY DESIGN

Reason:

- `BuildLatest.cmd` and `StartPreview.cmd` passed R2 command smoke without pathing, quoting, artifact discovery, launch, or cleanup failures.
- No runtime product code changed in P14.
- No small docs repair beyond the planned R2 pointer was needed.

### R4 - Final Validation

Status: PASS

Scope:

- Re-ran final validation gates.
- Re-ran `Package.cmd`.
- Re-ran `BuildLatest.cmd`.
- Re-ran `StartPreview.cmd`, confirmed the launched process, and cleaned it up.
- Re-ran diff and boundary scans.
- Left generated artifacts ignored and uncommitted.

Final validation:

- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- Final test counts: Domain `33`, Application `21`, Infrastructure `37`, Desktop `46`; all passed.
- `dotnet format Dateview.slnx --verify-no-changes`: passed with the existing workspace-load warning only.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.
- `.\BuildLatest.cmd`: passed.
- `.\StartPreview.cmd`: passed.
- `git diff --check`: passed.
- Broad docs-inclusive boundary scan returned expected historical documentation references to prohibited scope.
- Scoped implementation boundary scan over `src`, `tests`, `scripts`, `BuildLatest.cmd`, and `StartPreview.cmd`: no matches.
- Final status before this report update: `main...origin/main` clean.

Final command smoke evidence:

- `StartPreview.cmd` started `artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\ChinaTrayCalendar.Desktop.exe`.
- Started process: PID `31912`, responding, no main window title.
- Cleanup: stopped `ChinaTrayCalendar.Desktop`; no remaining process.
- No startup registry or user settings change was made by either root command.

Final package / artifact evidence:

- Zip: `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`.
- Zip bytes: `176355`.
- SHA256: `ee7ec7fadea164574e9337fe40be29c1401d09c9554b5e9443eca6a34985facf`.
- SHA256 file: `ee7ec7fadea164574e9337fe40be29c1401d09c9554b5e9443eca6a34985facf  Dateview-0.1.0-preview-win-x64.zip`.
- Release metadata `gitCommit`: `2a227d6`.
- Release metadata `generatedAtUtc`: `2026-06-22T15:54:59.0522232+00:00`.
- Release manifest files: `13`.
- Holiday JSON parse, using explicit UTF-8: `2025.json` schema `1` / `CN` / `33` days; `2026.json` schema `1` / `CN` / `39` days.
- Packaged executable associated icon: `32x32`.

Debug self-check:

- Manual tester workflow covered: final root-command discovery, build latest preview, start generated preview, verify process launch, and clean up.
- Failure localization: final evidence covers root command pathing, package script invocation, artifact discovery, process launch, working directory, and cleanup.
- Process cleanup: no Dateview process remained after final smoke.
- Generated artifacts: only ignored artifacts under `artifacts\release` were created/refreshed.
- Admin requirement: commands ran as normal current-user commands without elevation.

Architecture self-check:

- P14 stayed in tooling/docs.
- Runtime product code remained unchanged.
- No installer/signing/public-release/upload/online/telemetry/shell-hook scope was introduced.
- Generated artifacts remain ignored and uncommitted.
- No unrelated files were modified.

Commit / push:

- Final P14 validation commit: pending.

Residual risks:

- None specific to P14 command handoff. Generated release hashes change whenever `BuildLatest.cmd` regenerates metadata for a new git commit/time, so testers should always use the matching `.sha256.txt` from the same generation.

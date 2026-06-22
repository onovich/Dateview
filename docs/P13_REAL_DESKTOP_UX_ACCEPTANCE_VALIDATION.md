# P13 Real Desktop UX Acceptance Validation

Date: 2026-06-22
Phase: P13 Real Desktop UX Acceptance
Guide: `docs/P13_REAL_DESKTOP_UX_ACCEPTANCE_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline: P12 Customer UX Polish accepted in `docs/P12_CUSTOMER_UX_POLISH_VALIDATION.md`

## Scope

P13 is a validation, acceptance, and feedback-triage phase for the P12 tray icon, popup animation, and tray-click toggle changes.

Default expectation: validation and documentation only. Minimal Desktop-only fixes are allowed only if a P12 blocker is found during real desktop acceptance.

P13 does not add calendar features, redesign animation/icon/UI, implement signing or installers, publish or upload artifacts, add auto-update, add online APIs, add telemetry, inject into Explorer/taskbar, add global hooks, add Shell hooks, write HKLM, or require administrator privileges.

Pre-existing untracked root helper scripts are preserved and not staged:

- `BuildLatest.cmd`
- `StartPreview.cmd`

## Carried-Forward P12 Evidence

- P12 final commit: `2cf8fdd`
- P12 validation: `docs\P12_CUSTOMER_UX_POLISH_VALIDATION.md`
- Tray icon uses `DateviewTrayIconProvider` and embedded `DateviewIcon.ico`, not `SystemIcons.Application`.
- Packaged executable associated icon: `32x32`.
- Open animation: `170 ms` opacity plus content scale/translate ease-out.
- Close animation: `140 ms` opacity plus content scale/translate ease-in.
- No `Window.RenderTransform` animation is used.
- Tray visible-click close, view-model close, Escape dismiss, and deactivation dismiss route through animated close.
- Desktop automated tests at P12 final: Domain `33`, Application `21`, Infrastructure `37`, Desktop `46`.
- P12 residual risk: subjective animation feel and tray overflow behavior still need real desktop/human confirmation.

## Acceptance Matrix

| Item | Status | Evidence / Notes |
| --- | --- | --- |
| Tray icon appears and is Dateview-specific in notification area | PASS | R2 UI Automation found `SystemTray.NormalButton` named `Dateview` at `{X=2090,Y=1392,Width=32,Height=48}`. Screenshot `taskbar-after-launch.png` and extracted icon reference `dateview-icon-reference.png` show the blue Dateview calendar icon, not the generic application icon. |
| Tray icon appears acceptably in tray overflow | LIMITED | R2 opened the Windows tray overflow panel and captured `stable-overflow-open.png`, but this user profile currently shows Dateview in the normal notification area rather than inside overflow. P13 did not mutate Windows notification-area preferences to force Dateview into overflow. |
| Right-click menu remains accessible | PASS | R2 real tray right-click exposed menu items `今天`, `设置`, `开机启动`, and `退出`; screenshot `stable-right-click-menu.png`; UIA menu item coordinates captured. |
| Left-click opens popup | PASS | R2 real coordinate click at tray icon center `{X=2106,Y=1416}` opened a `Dateview` WPF window at approximately `{X=1926,Y=972,Width=360,Height=420}`. |
| Left-click again closes popup | PASS | R2 second real coordinate click on the same tray icon hid the popup. |
| Reopen after close works | PASS | R2 reopened after the close; popup became visible again at approximately `{X=1924,Y=972,Width=360,Height=420}`. |
| Escape closes popup | PASS | R2 sent Escape to the focused popup window; UIA no longer found the `Dateview` popup afterward. |
| Outside/deactivation closes popup | PASS | R2 foregrounded the popup and clicked outside; foreground moved to Codex and UIA no longer found the popup. A pure automated outside click while the popup was not foregrounded did not exercise `Deactivated`, so that specific automation path is recorded as a limitation, not a product pass claim. |
| Open animation feels calm/soft with no obvious bounce/shake | LIMITED | R2 exercised the real open path without JIT/crash and captured stable final frames. Subjective motion feel still needs human live observation because still screenshots cannot prove smoothness. Existing P12 code evidence remains `170 ms` opacity plus content scale/translate ease-out. |
| Close animation feels calm/soft | LIMITED | R2 exercised tray toggle, Escape, and deactivation close without JIT/crash and captured hidden final states. Subjective motion feel still needs human live observation because still screenshots cannot prove smoothness. Existing P12 code evidence remains `140 ms` opacity plus content scale/translate ease-in. |
| Second instance exits cleanly | PASS | R3 portable smoke launched a second instance from the extracted package; it exited with code `0` while the first instance remained running. |
| No Dateview process remains after exit/cleanup | PASS | R2 invoked `退出` from the real tray right-click menu with no process remaining. R3/R5 portable smoke force-cleaned the first process after second-instance validation and confirmed no remaining `ChinaTrayCalendar.Desktop` process. |
| Settings/startup state is not changed or is restored | PASS | R2 checked `HKCU:\Software\Microsoft\Windows\CurrentVersion\Run` value `Dateview`; it remained absent before and after smoke. |

## Environment

- OS: Microsoft Windows 11 Professional for Workstations, version `10.0.26200`, build `26200`, 64-bit.
- Display: one primary display, `\\.\DISPLAY1`, bounds `{X=0,Y=0,Width=2560,Height=1440}`, working area `{X=0,Y=0,Width=2560,Height=1392}`.
- DPI / scale: `96` DPI, `100%`.
- Taskbar: bottom edge, visible, approximately `48 px` high, autohide not observed.
- Tray icon location during R2: normal notification area, immediately right of `显示隐藏的图标`, not inside overflow.
- R2 artifact path: `artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\ChinaTrayCalendar.Desktop.exe`.
- R2 artifact hash: `30e29a2e049d182842b10f491f368064a2eafdfebd3cbf2145e9ac2e505f8086  Dateview-0.1.0-preview-win-x64.zip`.
- R2 package manifest source commit: `2cf8fdd` from P12. P13 had only documentation changes at R2, so runtime bits were unchanged.
- Local evidence directory, not committed: `C:\Users\Administrator\AppData\Local\Temp\Dateview-P13-R2-20260622-224524`.

## P13 Checklist

### R1 Checklist And Baseline

- [x] Create this P13 validation document.
- [x] Create `docs/PREVIEW_UX_ACCEPTANCE_CHECKLIST.md`.
- [x] Carry forward P12 automated evidence and residual risks.
- [x] Run `git diff --check`.

### R2 Real Desktop UX Smoke

- [x] Run the app in the real Windows desktop session where feasible.
- [x] Record tray icon, tray overflow, right-click menu, open/close, Escape, outside/deactivation, and animation feel evidence.
- [x] Record honest `NOT AVAILABLE` / `LIMITED` where direct inspection cannot be performed reliably.
- [x] Clean up any Dateview process and leave startup/settings state unchanged.

### R3 Package And Feedback Reconciliation

- [x] Run package/release smoke.
- [x] Confirm zip/hash/manifest/basic process smoke.
- [x] Map findings to `fix-now`, `document`, `defer`, or `needs hardware reproduction`.

### R4 Buffer Repair

- [x] Not used by design; no P12 blocker was found.

### R5 Final Acceptance

- [x] Run final `Validate.cmd`.
- [x] Run final `Package.cmd`.
- [x] Run final `package-release.ps1`.
- [x] Run final `git diff --check`.
- [x] Run final boundary scan.
- [x] Complete the final acceptance matrix.
- [x] Push all P13 commits.

## Round Log

### R1 - Checklist And Baseline

Status: PASS

Scope:

- Created this P13 validation report.
- Created `docs\PREVIEW_UX_ACCEPTANCE_CHECKLIST.md`.
- Carried forward the P12 icon, animation, toggle, automated test, package, and residual-risk evidence.
- Left implementation code unchanged.

Debug self-check:

- Real user workflow covered: a human preview tester can follow a concise checklist for tray icon, tray overflow, open/close toggle, Escape/outside close, subjective animation feel, and cleanup.
- Failure localization: the checklist separates icon visibility, tray menu, popup placement, animation feel, click toggle state, Escape/deactivation, package startup, and cleanup.
- Evidence type: R1 is documentation/checklist evidence only; real desktop observation starts in R2.
- State cleanup: R1 did not launch Dateview, mutate settings, write startup registry values, create temp extraction folders, or change desktop/taskbar state.
- Subjective observations: R1 does not claim subjective pass/fail; it defines how to record them.

Architecture self-check:

- R1 changes documentation only.
- No Desktop runtime code changed in R1.
- Domain/Application/Infrastructure remain unchanged.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: tracked tree clean at R1 start; only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- `git diff --check`: passed.

Commit / push:

- R1 P13 checklist/baseline commit: `ab3c1b0`, pushed to `origin/main`.

Risk / blocked:

- None for R1.

### R2 - Real Desktop UX Smoke

Status: PASS WITH LIMITATIONS

Scope:

- Launched the published preview executable in the real Windows desktop session.
- Captured taskbar/tray screenshots and UI Automation evidence for the Dateview notification-area icon.
- Exercised real tray left-click open, left-click close, reopen, Escape close, deactivation close after foreground activation, right-click menu, and tray-menu exit.
- Opened the Windows tray overflow panel without changing user notification-area preferences.
- Left implementation code unchanged.

Real desktop evidence:

- Launch process: `ChinaTrayCalendar.Desktop.exe`, PID `48820`, path `artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\ChinaTrayCalendar.Desktop.exe`, responding, no taskbar window title.
- Tray icon: UIA found `SystemTray.NormalButton` named `Dateview` at `{X=2090,Y=1392,Width=32,Height=48}`.
- Tray click point: `{X=2106,Y=1416}`.
- Left-click open: popup visible at approximately `{X=1926,Y=972,Width=360,Height=420}`.
- Left-click close: popup hidden.
- Reopen: popup visible again at approximately `{X=1924,Y=972,Width=360,Height=420}`.
- Escape close: focused popup received Escape; popup hidden afterward.
- Deactivation close: popup was foregrounded, an outside click moved foreground to Codex, and popup was hidden afterward.
- Right-click menu: UIA found menu items `今天`, `设置`, `开机启动`, and `退出`.
- Exit cleanup: `退出` menu item invoked; no `ChinaTrayCalendar.Desktop` process remained.
- Startup state: `HKCU:\Software\Microsoft\Windows\CurrentVersion\Run` value `Dateview` remained absent.
- Local screenshots, not committed: `taskbar-after-launch.png`, `taskbar-right-after-launch.png`, `stable-left-click-open.png`, `stable-right-click-menu.png`, `stable-overflow-open.png`, and related step captures under `C:\Users\Administrator\AppData\Local\Temp\Dateview-P13-R2-20260622-224524`.

Limitations / triage:

- Tray overflow Dateview click: `LIMITED`. Dateview was visible in the normal notification area in this Windows user profile. The overflow panel opened successfully, but Dateview was not inside it. P13 did not change Windows notification-area settings to force an overflow placement.
- Subjective animation feel: `LIMITED`. The real open/close paths completed without JIT/crash and screenshots show stable end states, but still images cannot prove motion feel. Human preview should still confirm the animation feels soft.
- Automated outside click without foreground activation: `LIMITED`. A pure automated outside click while the popup was not foregrounded did not exercise the WPF `Deactivated` path. When the popup was foregrounded, outside click closed it as expected.

Debug self-check:

- Real user workflow covered: tray icon visibility, left-click open/close/reopen, Escape close, deactivation close, right-click menu, tray-menu exit, process cleanup, and startup-state preservation.
- Failure localization: remaining limitations localize to Windows notification-area placement/user preference and subjective animation observation, not Domain/Application logic.
- Evidence type: R2 used real desktop UI Automation, screenshots, process checks, registry startup checks, and documented limitations.
- State cleanup: Dateview exited via tray menu, no process remained, and no startup registry value was created.
- Subjective observations: animation is recorded as `LIMITED` because screenshots do not replace live human motion review.

Architecture self-check:

- R2 remained validation/docs-only.
- No Desktop runtime code changed in R2.
- Domain/Application/Infrastructure remain unchanged.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Generated screenshots and release artifacts remain uncommitted.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: tracked tree clean at R2 start; only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- `git diff --check`: passed after the R2 document update.

Commit / push:

- R2 evidence document commit: `06da922`, pushed to `origin/main`.

Risk / blocked:

- No P12 blocker found. Remaining issues are acceptance limitations for human preview / hardware or Windows user-profile reproduction.

### R3 - Package And Feedback Reconciliation

Status: PASS

Scope:

- Regenerated the folder publish output with `Package.cmd`.
- Regenerated the release bundle with `scripts\package-release.ps1`.
- Verified zip hash, release metadata, release manifest, packaged holiday JSON, associated executable icon, portable launch, second-instance behavior, and cleanup.
- Mapped the remaining R2 limitations to feedback triage outcomes.
- Left implementation code unchanged.

Package / artifact evidence:

- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1`: passed.
- Zip: `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`.
- Zip bytes: `176394`.
- SHA256: `c9a69ee1cff8953cd58782f6e96f0308ea04f7ce85e6d99ae31dd40a80c46922`.
- SHA256 file: `c9a69ee1cff8953cd58782f6e96f0308ea04f7ce85e6d99ae31dd40a80c46922  Dateview-0.1.0-preview-win-x64.zip`.
- Release metadata `gitCommit`: `06da922`.
- Release metadata `generatedAtUtc`: `2026-06-22T14:57:43.7378703+00:00`.
- Release manifest: schema version `1`, product `Dateview`, files `13`.
- Holiday JSON parse, using explicit UTF-8:
  - `2025.json`: schema version `1`, jurisdiction `CN`, days `33`, source title `国务院办公厅关于2025年部分节假日安排的通知`.
  - `2026.json`: schema version `1`, jurisdiction `CN`, days `39`, source title `国务院办公厅关于2026年部分节假日安排的通知`.
- Packaged executable associated icon: `32x32`.

Process smoke evidence:

- Portable extraction temp path: `C:\Users\Administrator\AppData\Local\Temp\Dateview-P13-R3-20260622-231113`.
- Portable executable: `Dateview\ChinaTrayCalendar.Desktop.exe` under the extracted temp folder.
- First instance PID: `24344`, responding, no main window title.
- Second instance exit code: `0`.
- After second-instance launch, first instance remained running and responding.
- Cleanup stopped the first process; no `ChinaTrayCalendar.Desktop` process remained.
- Temp extraction directory was removed after path safety verification.

Notes from smoke implementation:

- The first JSON smoke attempt used Windows PowerShell's default text decoding and corrupted UTF-8 holiday JSON before parsing. The successful smoke used `Get-Content -Encoding UTF8`; this is a validation script detail, not a package defect.
- The zip root is `Dateview\...`, not `Dateview-0.1.0-preview-win-x64\Dateview\...`; the successful portable smoke used the correct extracted path.

Feedback triage:

- `fix-now`: none. No P12 blocker or runtime crash was found.
- `document`: record the UTF-8 parsing detail in this P13 validation report for future Windows PowerShell smoke commands.
- `defer`: subjective animation feel remains for live human preview because P13 automated evidence can show the path completes and the final states are stable, but not how the motion feels.
- `needs hardware reproduction`: tray overflow placement/click remains limited because this Windows user profile currently shows Dateview in the normal notification area, not inside overflow. Reproduce on a machine/profile where Dateview is in overflow or after a tester manually moves it there.

Debug self-check:

- Real user workflow covered: package integrity, preview handoff artifact, portable launch, second-instance guard, process cleanup, packaged icon, and bundled holiday data.
- Failure localization: any future package issue can be localized to publish output, release zip/hash/manifest, UTF-8 holiday parsing, icon association, single-instance behavior, or cleanup.
- Evidence type: R3 used generated release artifacts and process smoke from an extracted portable package.
- State cleanup: the extracted temp directory was removed and no Dateview process remained.
- Subjective observations: none added in R3.

Architecture self-check:

- R3 remained validation/docs-only.
- No Desktop runtime code changed in R3.
- Domain/Application/Infrastructure remain unchanged.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Generated release artifacts remain ignored and uncommitted.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: tracked tree clean at R3 start; only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- `Package.cmd`: passed.
- `package-release.ps1`: passed.
- Portable package smoke: passed.
- `git diff --check`: passed after this R3 document update.

Commit / push:

- R3 package smoke document commit: `d473975`, pushed to `origin/main`.

Risk / blocked:

- No blocker. R4 buffer is currently unused.

### R4 - Buffer Repair

Status: NOT USED BY DESIGN

Reason:

- R2 and R3 found no P12 blocker requiring a Desktop-only fix.
- No runtime, Domain, Application, Infrastructure, or test code changed in P13.
- Remaining limitations are acceptance/hardware or live-human-observation items, not implementation blockers.

### R5 - Final Acceptance

Status: PASS

Scope:

- Re-ran the final validation matrix.
- Regenerated the final package and release bundle after the R3 evidence commit.
- Re-ran portable package/process smoke against the final bundle.
- Re-ran `git diff --check`, prohibited-boundary scan, and status checks.
- Left generated artifacts ignored and uncommitted.

Final validation:

- Initial in-sandbox `Validate.cmd` attempt was blocked by sandbox access to `C:\Users\Administrator\AppData\Roaming\NuGet\NuGet.Config`.
- Retried `Validate.cmd` outside the sandbox with approval: passed.
- Test counts from final `Validate.cmd`: Domain `33`, Application `21`, Infrastructure `37`, Desktop `46`; all passed.
- `Package.cmd`: passed.
- `package-release.ps1`: passed.
- `git diff --check`: passed.
- Boundary scan over `src`, `tests`, and `scripts` for prohibited scope keywords (`HKEY_LOCAL_MACHINE`, `HKLM`, `SetWindowsHookEx`, `Shell_NotifyIcon`, telemetry, auto-update/autoupdate, installer/signing/public-release/upload/hook indicators): no matches.
- Final status before this report update: `main...origin/main` clean except preserved untracked `BuildLatest.cmd` and `StartPreview.cmd`.

Final package evidence:

- Zip: `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`.
- Zip bytes: `176417`.
- SHA256: `971e50b95bda6245a0f1ec0275b5b4d232801aab5b0ba4ca01b76521fe75abe6`.
- SHA256 file: `971e50b95bda6245a0f1ec0275b5b4d232801aab5b0ba4ca01b76521fe75abe6  Dateview-0.1.0-preview-win-x64.zip`.
- Release metadata `gitCommit`: `d473975`.
- Release metadata `generatedAtUtc`: `2026-06-22T15:17:30.3068568+00:00`.
- Release manifest files: `13`.
- Holiday JSON parse, using explicit UTF-8: `2025.json` schema `1` / `CN` / `33` days; `2026.json` schema `1` / `CN` / `39` days.
- Packaged executable associated icon: `32x32`.

Final process smoke evidence:

- Portable extraction temp path: `C:\Users\Administrator\AppData\Local\Temp\Dateview-P13-R5-20260622-231821`.
- First instance PID: `2368`, responding, no main window title.
- Second instance exit code: `0`.
- After second-instance launch, first instance remained running and responding.
- Cleanup stopped the first process; no `ChinaTrayCalendar.Desktop` process remained.
- Temp extraction directory was removed after path safety verification.

Final acceptance summary:

- `PASS`: normal tray icon presence, Dateview-specific icon, right-click menu, left-click open, left-click close, reopen, Escape close, foregrounded outside/deactivation close, second-instance exit, process cleanup, startup state preservation, package integrity, portable process smoke, boundary scan.
- `LIMITED`: tray overflow placement/click on this specific user profile because Dateview appears in the normal notification area, and subjective animation feel because screenshots/process checks cannot replace live human observation.
- `FAIL`: none.
- `NOT AVAILABLE`: none beyond the explicitly limited overflow-placement condition.

Debug self-check:

- Real user workflow covered: final handoff artifact, final package integrity, portable run, single-instance behavior, cleanup, and final validation gates.
- Failure localization: no untriaged failure remains; limitations map to Windows notification-area user preference and live human animation review.
- Evidence type: final validation commands, release metadata, portable process smoke, and boundary scan.
- State cleanup: no Dateview process remained and the final temp extraction directory was removed.
- Subjective observations: not overclaimed; animation remains `LIMITED` pending human live preview.

Architecture self-check:

- P13 remained validation/docs-only.
- No code changes were made in Desktop, Domain, Application, Infrastructure, or tests.
- No forbidden shell integration, hook, admin, online, telemetry, installer/signing, public release, upload, or HKLM scope was introduced.
- Generated artifacts remain ignored and uncommitted.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Commit / push:

- Final validation report commit: `a8ff293`, pushed to `origin/main`.

Residual risks:

- Tray overflow behavior still needs reproduction on a Windows profile where Dateview is actually in overflow.
- Animation softness still needs a live human preview verdict; no crash, JIT popup, or obvious automated-state failure was observed.

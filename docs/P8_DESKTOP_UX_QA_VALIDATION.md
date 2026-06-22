# P8 Desktop UX QA Validation

Date: 2026-06-22
Phase: P8 Desktop UX QA and Release Candidate Hardening
Guide: `docs/P8_DESKTOP_UX_QA_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline commit: `8279607`

## Environment

- Machine: `WIN-ONOVICH`
- User: `Administrator`
- OS: Microsoft Windows 11 专业工作站版 `10.0.26200`
- .NET SDK: `10.0.102`
- Execution mode: normal current-user desktop process, no administrator elevation required.

## Release Artifact

Published executable:

```text
D:\ToolProjects\Dateview\src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\ChinaTrayCalendar.Desktop.exe
```

Published holiday data:

```text
D:\ToolProjects\Dateview\src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\assets\holidays\cn\2025.json
D:\ToolProjects\Dateview\src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\assets\holidays\cn\2026.json
```

## Manual QA Checklist

### Tray And Single Instance

- [x] Start the published executable as a normal user.
- [x] Confirm the Dateview tray icon is visible in the notification area or Windows tray overflow.
- [x] Confirm a second launch exits successfully without opening another long-running instance.
- [x] Right-click the tray icon and confirm the context menu contains Today, Settings, Start with Windows, and Exit.
- [ ] Click Today from the context menu and confirm the calendar returns to the current month when the popup is visible.
- [ ] Click Settings from the context menu and confirm the settings window opens.
- [ ] Toggle Start with Windows from the context menu and restore the original state afterward.
- [x] Click Exit and confirm the app process exits and the tray icon clears.

### Popup Interaction

- [x] Left-click the tray icon and confirm the calendar popup opens.
- [x] Left-click the tray icon again and confirm the popup hides.
- [x] Press Escape while the popup is open and confirm it hides.
- [x] Click outside the popup and confirm it hides.
- [x] Move focus away from the popup and confirm it hides when the pointer is outside the popup.
- [ ] Confirm the popup does not appear in Alt+Tab.
- [x] Confirm the popup does not create a normal taskbar button.
- [x] Use Previous Month, Next Month, and Today controls and confirm the displayed month changes correctly.

### Placement, DPI, And Visual Behavior

- [x] Confirm the popup opens near the taskbar edge, not centered on the screen.
- [x] Confirm the popup stays within the current monitor working area.
- [x] Confirm the popup appears on the monitor where the tray interaction occurred, when a multi-monitor setup is available.
- [x] Confirm placement remains correct on available display scale settings, or record the limitation if the environment cannot change DPI.
- [ ] Confirm open and close animations feel like a short taskbar-adjacent panel transition.

### Settings, Startup, And Offline Data

- [ ] Change first day of week in Settings, save, restart the app, and confirm the setting persists.
- [ ] Confirm settings are written under `%APPDATA%\ChinaTrayCalendar\settings.json`.
- [ ] Enable Start with Windows and confirm only the current-user Run key is affected.
- [ ] Disable Start with Windows and confirm the original registry state is restored.
- [ ] Confirm no HKLM startup location is written.
- [ ] Confirm bundled holiday data is loaded offline from the published output.
- [ ] Confirm 2025 and 2026 Chinese holidays and adjusted workdays render with `休` and `班` badges.

### Documentation And RC Closure

- [ ] Confirm `README.md` and `docs/TROUBLESHOOTING.md` match the final behavior.
- [ ] Run `Validate.cmd`.
- [ ] Run `Package.cmd`.
- [ ] Run a fresh published-output smoke test.
- [ ] Record final residual risk and Release Candidate status.

## Round Log

### R1 - Baseline Checklist And Publish Smoke

Status: PASS

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: baseline worktree contained P8 handoff documentation changes only.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.
- Published executable exists at the Release Artifact path above.
- Published `2025.json` and `2026.json` holiday data files exist at the Release Artifact paths above.
- Fresh output smoke passed:
  - First published instance stayed running for the smoke window.
  - Second published instance exited with code `0`.
  - Smoke-started first instance was stopped after verification.

Debug self-check:

- Minimal workflow covered in R1: package can be produced and the published executable can start.
- Failure layer checked: build/test/format/package pipeline plus published-output process startup.
- Success/failure cases covered: first instance stays running, second instance exits successfully, missing artifact checks fail the smoke script.
- Registry/settings cleanup: no registry or user settings writes were performed in R1.

Architecture self-check:

- R1 changed documentation only.
- No Domain, Application, Infrastructure, or Desktop source boundaries changed.
- No new package, startup, shell hook, admin, or online dependency was introduced.
- P8 manual QA scope remains focused on Release Candidate hardening rather than new product features.

### R2 - Tray Entry, Context Menu, Exit, And Single Instance

Status: PASS

Defect found and fixed:

- The tray context menu contained `开机启动`, but it was disabled.
- Fixed `TrayIconService` so `开机启动` is enabled, checkable by app state, and raises `StartWithWindowsToggleRequested`.
- Wired the Desktop composition root to the existing `ToggleStartupUseCase` and `IAutoStartService`; Registry access remains in Infrastructure.
- Added Desktop tests for the enabled menu item, toggle request event, and checked/enabled state refresh.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R2 start.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed after the fix.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed after the fix.
- Desktop test count increased from `33` to `35`.

Manual and smoke evidence:

- Started the published executable as a normal user.
- Confirmed the Dateview tray entry appears in the Windows tray overflow through UI Automation:
  - Name: `Dateview`
  - Class: `SystemTray.NormalButton`
  - Rectangle: `2006,1295,40,40`
- Right-clicked the Dateview tray entry and confirmed all Dateview menu items were visible and enabled:
  - `今天`, enabled, Dateview process.
  - `设置`, enabled, Dateview process.
  - `开机启动`, enabled, Dateview process.
  - `退出`, enabled, Dateview process.
- Clicked `退出` from the real tray context menu:
  - Started process: `2224`
  - Exit code: `0`
  - No remaining `ChinaTrayCalendar.Desktop` process after exit.
- Re-ran single-instance smoke on the republished output:
  - First instance stayed running.
  - Second instance exited with code `0`.
  - Smoke-started first instance was stopped after verification.
- Temporary local screenshots were used for coordinate confirmation under `artifacts\p8-qa\` and were not intended as committed release artifacts.

Deferred to later P8 rounds:

- `今天` command behavior is best verified with the popup open in R3.
- `设置` window behavior and `开机启动` registry state are verified in R5, where registry state capture and restore are required.

Debug self-check:

- Minimal workflow covered in R2: start published app, find tray entry, open real context menu, verify menu shape, exit from menu, verify single-instance behavior.
- Failure layer checked: Desktop tray service, Desktop composition wiring, existing Application startup use case, published executable interaction with Windows tray overflow.
- Success/failure cases covered: enabled menu items, disabled-menu regression test, exit success, second-instance success.
- Registry/settings cleanup: R2 did not click `开机启动`; no startup registry state was changed.

Architecture self-check:

- UI and tray behavior stayed in Desktop.
- Startup state mutation uses existing Application `ToggleStartupUseCase` and Infrastructure `WindowsAutoStartService`.
- No Domain or Application business logic was duplicated in the UI.
- No shell hook, Explorer injection, admin requirement, online dependency, or third-party package was introduced.

### R3 - Popup Open, Hide, Navigation, And Window Shell Behavior

Status: PASS

Defects found and fixed:

- Real tray left-click reached `PopupAnimationService.PlayEntrance` and exposed `System.InvalidOperationException: Transform is not valid for Window`.
- Fixed `PopupAnimationService` so opacity animation remains on the `Window`, while translate animation is applied to `window.Content` only when it is a `UIElement`; non-UIElement content safely falls back to opacity only.
- Added `PopupAnimationServiceTests` to verify `Window.RenderTransform` is not set and content-root transform is used.
- Real tray left-click did not reliably raise the popup through `NotifyIcon.MouseUp`.
- Switched tray primary-click wiring to `NotifyIcon.MouseClick` via `ITrayIcon.MouseClick`, and updated tray tests accordingly.
- After suppressing the tray-overflow immediate `Deactivated`, the popup is explicitly activated only after a user tray click so Escape and in-popup controls receive input.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R3 start.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed after the fixes.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed after the fixes.
- Desktop test count increased from `35` to `37`.

Manual and smoke evidence:

- Started the published executable as a normal user.
- Real tray left-click on `Dateview` opened the popup:
  - Tray button rectangle: `2090,1392,32,48`
  - Popup rectangle: `1926,964,360,420`
  - Month title: `2026年6月`
  - JIT window count: `0`
- Real tray left-click toggle passed:
  - `VisibleAfterOpen`: `true`
  - `VisibleAfterSecondClick`: `false`
- Escape and outside-click hide passed:
  - `VisibleAfterEscape`: `false`
  - `VisibleAfterOutsideClick`: `false`
- Popup shell evidence:
  - `TaskbarDateviewButtonCount`: `0`
  - Popup extended style: `0x80000`
  - `HasAppWindow`: `false`
  - JIT window count: `0`
- Navigation smoke:
  - Initial title: `2026年6月`
  - Next month button changed the title to `2026年7月`.
  - Previous month button moved the displayed month backward.
  - Today button returned the title to `2026年6月`.
  - Exact Previous/Next command semantics remain covered by deterministic Desktop view-model tests; real smoke confirmed the controls are clickable in the published popup.

Deferred to later P8 rounds:

- Visual Alt+Tab switcher inspection remains for R8 final manual RC pass; R3 verified `ShowInTaskbar=False` behavior through taskbar count and no `WS_EX_APPWINDOW`.

Debug self-check:

- Minimal workflow covered in R3: published app tray left-click opens popup, second left-click hides it, Escape hides it, outside click hides it, navigation buttons are clickable, no JIT crash appears.
- Failure layer checked: Desktop tray event adapter, Desktop popup focus/deactivation handling, Desktop animation service, real published WPF/WinForms interaction.
- Success/failure cases covered: visible popup after left click, no popup after second click/Escape/outside click, no taskbar button, no JIT window.
- Registry/settings cleanup: R3 did not change settings or startup registry state.

Architecture self-check:

- All R3 changes stayed in Desktop UI/tray/animation code and Desktop tests.
- No Domain/Application business rules were changed or duplicated.
- No Infrastructure registry/file/JSON behavior changed.
- No shell hook, Explorer injection, admin requirement, online dependency, or third-party package was introduced.

### R4 - Taskbar Edge, Monitor, DPI, And Placement QA

Status: PASS

Defect found and fixed:

- The popup window was placed with an 8px outer placement margin while the root visual Border also had an 8px margin.
- On the bottom taskbar this produced about a 16px visual gap from the taskbar edge, which missed the UX Fusion 2-6px visual edge target.
- Fixed `PopupWindowPlacer` to use `0` outer placement margin and changed the popup root Border margin to `4`, keeping the outer window inside the working area while making the visible panel sit about 4px from the taskbar edge.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R4 start.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.

Manual and smoke evidence:

- Environment display data:
  - Screen count: `1`
  - Screen bounds: `{X=0,Y=0,Width=2560,Height=1440}`
  - Working area: `{X=0,Y=0,Width=2560,Height=1392}`
  - Popup DPI: `96`
- Published executable placement smoke:
  - Tray button rectangle: `2090,1392,32,48`
  - Popup rectangle: `1926,972,360,420`
  - Window bottom gap from working area: `0px`
  - Visible Border bottom gap from working area: `4px`
  - Popup inside working area: `true`
  - Open attempts: `1`
- Existing `PopupPlacementCalculatorTests` cover bottom, top, left, right, and no-taskbar working-area geometry.
- Animation hardening from R3 remains active:
  - Entrance opacity animation still applies to the window.
  - Entrance translate animation applies to the popup content root rather than the WPF top-level Window.
  - R4 published smoke did not produce JIT or animation errors.

Environment limitation:

- Multi-monitor QA could not be performed on this machine because only `DISPLAY1` is available.
- Non-100% DPI scaling could not be changed safely during this automated round; R4 records 96 DPI live evidence and retains geometry coverage for monitor/taskbar edge cases.

Debug self-check:

- Minimal workflow covered in R4: published app opens the popup from the tray, popup appears at the bottom taskbar edge, remains inside the working area, and uses the current display.
- Failure layer checked: Desktop placement calculator, Desktop WPF placement adapter, published executable on the actual Windows taskbar.
- Success/failure cases covered: bottom taskbar placement, single-monitor limitation, 96 DPI evidence, working-area containment.
- Registry/settings cleanup: R4 did not change settings or startup registry state.

Architecture self-check:

- R4 changes stayed in Desktop XAML and Desktop popup placement code.
- Shared placement math remains in the existing Desktop placement helper and tests.
- No Domain/Application/Infrastructure behavior changed.
- No shell hook, Explorer injection, admin requirement, online dependency, or third-party package was introduced.

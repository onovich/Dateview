# P12 Customer UX Polish Validation

Date: 2026-06-22
Phase: P12 Customer UX Polish
Guide: `docs/P12_CUSTOMER_UX_POLISH_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline: P11 limited preview release readiness accepted in `docs/P11_LIMITED_PREVIEW_RELEASE_VALIDATION.md`

## Scope

P12 implements narrow Desktop/UI polish from customer-support feedback:

- Complete the recognizable Dateview tray/application icon experience.
- Replace the popup's shaky/drop-feeling animation with a soft, natural open motion.
- Make left-clicking the tray icon a reliable open/close toggle, with soft close animation.

P12 does not add calendar features, redesign the whole UI, change Domain/Application business behavior, introduce new tray technology, publish or upload artifacts, add installer/signing work, add online dependencies, add telemetry, inject into Explorer/taskbar, add Shell hooks/global hooks, write HKLM, or require administrator privileges.

Generated artifacts remain ignored and uncommitted. Pre-existing untracked root helper scripts are preserved and not staged:

- `BuildLatest.cmd`
- `StartPreview.cmd`

## Starting Evidence

- Current guide commit: `82dc9e7`
- Previous final P11 commit: `0e039dd`
- P12 starts from `82dc9e7 docs: add P12 customer UX polish goal`.
- `assets\icons\dateview.ico` exists, is readable as a `32x32` icon, length `766` bytes, SHA256 `7D2EE64C957BF5849C3669434FA03834496094AC3504D3B793767D4463D25BA7`.
- `src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj` has `<ApplicationIcon>..\..\assets\icons\dateview.ico</ApplicationIcon>`.
- `src\ChinaTrayCalendar.Desktop\Tray\TrayIconService.cs` currently assigns `SystemIcons.Application`, so the tray icon is generic despite the Dateview icon asset.
- `src\ChinaTrayCalendar.Desktop\PopupPlacement\PopupAnimationService.cs` currently supports entrance animation only: opacity plus content `TranslateTransform.Y` from `8` to `0` over `150 ms`.
- `PopupAnimationService` does not set `Window.RenderTransform`, preserving the P9/P10 fix that avoided WPF `Window.RenderTransform` instability.
- `src\ChinaTrayCalendar.Desktop\App.xaml.cs` has visible-state branching in `TogglePopup`, but the close path calls immediate `HidePopup()`, and `HidePopup()` calls `popupWindow?.Hide()` without a close animation.
- `src\ChinaTrayCalendar.Desktop\CalendarPopupWindow.xaml.cs` directly calls `Hide()` for deactivation and Escape.

## P12 Checklist

### R1 Baseline And Validation Doc

- [x] Create this P12 validation document.
- [x] Record current icon, animation, and tray-toggle evidence.
- [x] Run `Validate.cmd`.
- [x] Preserve unrelated untracked root helper scripts.

### R2 Icon Completion

- [x] Make the tray icon use the Dateview icon asset or embedded application icon instead of `SystemIcons.Application`.
- [x] Keep icon ownership/disposal correct.
- [x] Add/update Desktop tests so the tray icon is not the generic system application icon.
- [x] Verify package/publish still has coherent application icon evidence.

### R3 Soft Open/Close Animation

- [x] Add deterministic soft entrance and exit animation behavior.
- [x] Keep transforms on popup content, never the WPF `Window`.
- [x] Add/update Desktop animation tests.

### R4 Tray Toggle Integration

- [x] Wire tray left-click visible state to animated close.
- [x] Keep Escape, close command, and deactivation paths working.
- [x] Cover repeated click/open/close behavior with tests or recorded smoke evidence.

### R5 Buffer Repair

- [ ] Use only if icon asset, animation timing, toggle race, tests, or docs need repair.

### R6 Final Validation

- [ ] Run final `Validate.cmd`.
- [ ] Run final `Package.cmd`.
- [ ] Run final `package-release.ps1`.
- [ ] Run final `git diff --check`.
- [ ] Run final boundary scan.
- [ ] Record manual smoke or environment limitation.
- [ ] Push all P12 commits.

## Round Log

### R1 - Baseline And Validation Doc

Status: PASS

Scope:

- Created this P12 validation report.
- Recorded the customer-support UX feedback and starting implementation state.
- Confirmed the Dateview icon asset and application icon project setting already exist.
- Confirmed the tray icon currently uses the generic system application icon.
- Confirmed the popup has only entrance animation and immediate hide paths.
- Ran the full repository validation gate.

Debug self-check:

- Smallest user-visible workflow covered: current launch/tray/popup baseline before any UX polish.
- Failure localization: starting gaps localize to tray icon assignment, popup animation service, App popup orchestration, and CalendarPopupWindow direct hide paths.
- Open/close coverage: current tests cover tray primary click raising and entrance content transform; close animation and repeated-click behavior are not yet implemented.
- `Window.RenderTransform`: current entrance animation already avoids setting it and tests verify the local value remains unset.
- State cleanup: R1 did not launch Dateview, mutate settings, write startup registry values, change display settings, or alter desktop/taskbar state.

Architecture self-check:

- R1 changes documentation only.
- Domain/Application/Infrastructure runtime behavior remains unchanged.
- Desktop remains the expected layer for tray icon, popup animation, and popup orchestration.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Unrelated untracked root helper scripts are preserved and not staged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: P12 start had only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
  - Domain tests: `33` passed.
  - Application tests: `21` passed.
  - Infrastructure tests: `37` passed.
  - Desktop tests: `38` passed.
  - `dotnet format --verify-no-changes`: passed.
- Icon asset read check: passed after explicitly loading `System.Drawing`; `dateview.ico` is `32x32`, `766` bytes.

Commit / push:

- This R1 section is committed by the R1 P12 baseline commit.

Risk / blocked:

- None for R1.

### R2 - Icon Completion

Status: PASS

Scope:

- Added `DateviewIcon.ico` as an embedded Desktop assembly resource sourced from `assets\icons\dateview.ico`.
- Added `ITrayIconProvider` and `DateviewTrayIconProvider`.
- Updated `TrayIconService` to obtain its tray icon from the provider instead of assigning `SystemIcons.Application`.
- Updated `TrayIconService` disposal to clear the NotifyIcon adapter icon and dispose the owned tray icon.
- Added Desktop tests proving `Show()` replaces the fake generic icon and that an injected icon provider supplies the tray icon.
- Verified the published executable still exposes a `32x32` associated application icon.

Debug self-check:

- Smallest user-visible workflow covered: app startup creates a tray icon using Dateview-specific icon data instead of the generic system application icon.
- Failure localization: icon failures localize to embedded icon resource inclusion, `DateviewTrayIconProvider`, `TrayIconService` assignment, or icon disposal.
- Open/close paths were not changed in R2 and remain for R3/R4.
- `Window.RenderTransform` is unaffected by R2.
- State cleanup: R2 did not launch Dateview, mutate settings, write startup registry values, change display settings, or alter desktop/taskbar state.

Architecture self-check:

- R2 changes stay in Desktop tray code, Desktop project resource configuration, and Desktop tests.
- Domain/Application/Infrastructure remain free of WPF, WinForms tray icon, icon resource, and animation state.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean tracked tree at R2 start, with only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- `dotnet test tests\ChinaTrayCalendar.Desktop.Tests\ChinaTrayCalendar.Desktop.Tests.csproj --configuration Release --filter FullyQualifiedName~TrayIconServiceTests`: passed.
  - TrayIconService tests: `15` passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed after formatting line endings.
  - Domain tests: `33` passed.
  - Application tests: `21` passed.
  - Infrastructure tests: `37` passed.
  - Desktop tests: `40` passed.
  - `dotnet format --verify-no-changes`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.
- Published icon evidence:
  - Published exe: `src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\ChinaTrayCalendar.Desktop.exe`
  - Associated icon: `32x32`
  - Desktop assembly embedded resource `DateviewIcon.ico`: present.

Commit / push:

- This R2 section is committed by the R2 P12 icon completion commit.

Risk / blocked:

- Manual tray visual inspection is deferred to R4/R6 smoke after animation/toggle integration.

### R3 - Soft Open/Close Animation

Status: PASS

Scope:

- Updated `PopupAnimationService` entrance animation from opacity plus translate-only to a restrained `170 ms` opacity plus content scale/translate ease-out.
- Added `PlayExitAsync(Window)` with a `140 ms` soft opacity plus content scale/translate ease-in and task completion.
- Kept all motion on popup content using `TransformGroup`, `ScaleTransform`, and `TranslateTransform`.
- Continued to avoid assigning or animating `Window.RenderTransform`.
- Reset visual state after exit animation completion so the next open starts from a clean opacity/scale/translate state.
- Added Desktop tests covering entrance transform, entrance fallback without UIElement content, exit transform/task completion, exit fallback without UIElement content, and no top-level Window transform.

Debug self-check:

- Smallest user-visible workflow covered: popup opening and closing motion can be animated without bounce/overshoot and without invalid WPF `Window.RenderTransform` usage.
- Failure localization: animation failures now localize to `PopupAnimationService` content transform preparation, entrance animation, exit animation completion, or fallback paths.
- Open/close coverage: entrance, exit, non-UIElement fallback, and visual reset are covered; tray-click orchestration is deferred to R4.
- `Window.RenderTransform`: tests continue to assert the WPF Window has no local `RenderTransform` value after entrance/exit setup.
- State cleanup: R3 tests create and close WPF windows in STA threads; no Dateview app process, settings, startup registry value, display setting, or taskbar state is changed.

Architecture self-check:

- R3 changes stay in Desktop popup animation code and Desktop tests.
- Domain/Application/Infrastructure remain unchanged.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean tracked tree at R3 start, with only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- `dotnet test tests\ChinaTrayCalendar.Desktop.Tests\ChinaTrayCalendar.Desktop.Tests.csproj --configuration Release --filter FullyQualifiedName~PopupAnimationServiceTests`: passed.
  - PopupAnimationService tests: `4` passed.
- `dotnet format Dateview.slnx`: ran to normalize line endings after source/test edits.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
  - Domain tests: `33` passed.
  - Application tests: `21` passed.
  - Infrastructure tests: `37` passed.
  - Desktop tests: `42` passed.
  - `dotnet format --verify-no-changes`: passed.

Commit / push:

- This R3 section is committed by the R3 P12 animation commit.

Risk / blocked:

- Manual feel validation is deferred until R4/R6 after tray toggle integration uses the close animation.

### R4 - Tray Toggle Integration

Status: PASS WITH MANUAL VISUAL LIMITATION

Scope:

- Updated `CalendarPopupWindow` so Escape and deactivation raise `DismissRequested` instead of directly calling `Hide()`.
- Added a short delayed deactivation-dismiss timer so a tray click can be handled as a toggle before outside-click dismissal wins.
- Updated `App` to route view-model close, Escape, deactivation, and tray click close through `PopupAnimationService.PlayExitAsync`.
- Added `PopupVisibilityCoordinator` and `PopupToggleAction` to keep Desktop popup shell state deterministic.
- Implemented repeated tray-click behavior during close: queue the latest tray click point and reopen after the close animation completes.
- Added coordinator tests for hidden click open, visible click close, repeated click during close, and duplicate close request handling.
- Updated popup Escape test to verify `DismissRequested`.

Debug self-check:

- Smallest user-visible workflow covered: left-click tray opens when hidden, left-click tray while visible starts close, and another click during close deterministically reopens from the latest point after close completes.
- Failure localization: toggle failures localize to `PopupVisibilityCoordinator`, `App` popup orchestration, `PopupAnimationService`, or `CalendarPopupWindow` dismiss events.
- Open/close/repeated click/Escape/deactivation: coordinator tests cover click state transitions and repeated click; popup tests cover Escape request; App wiring routes view-model close/Escape/deactivation/tray close through the exit animation.
- `Window.RenderTransform`: no `Window.RenderTransform` animation was added.
- State cleanup: R4 process smoke stopped the test process and left no Dateview process running. It did not mutate settings, startup registry, display settings, or taskbar state.

Architecture self-check:

- R4 changes stay in Desktop window code, Desktop popup orchestration/state, and Desktop tests.
- Domain/Application/Infrastructure remain unchanged.
- Popup visibility/animation state remains in Desktop and is not moved into Domain/Application.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean tracked tree at R4 start, with only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- Focused Desktop tests:
  - Command: `dotnet test tests\ChinaTrayCalendar.Desktop.Tests\ChinaTrayCalendar.Desktop.Tests.csproj --configuration Release --filter "FullyQualifiedName~CalendarViewModelTests|FullyQualifiedName~PopupVisibilityCoordinatorTests|FullyQualifiedName~PopupAnimationServiceTests"`
  - Result: passed, `16` tests.
- `dotnet format Dateview.slnx`: ran to normalize line endings after source/test edits.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
  - Domain tests: `33` passed.
  - Application tests: `21` passed.
  - Infrastructure tests: `37` passed.
  - Desktop tests: `46` passed.
  - `dotnet format --verify-no-changes`: passed.
- Process smoke:
  - A stale Dateview preview process from `artifacts\release` was found before smoke and stopped to avoid single-instance interference.
  - Exe: `src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\ChinaTrayCalendar.Desktop.exe`
  - Primary process id: `22680`
  - Primary remained alive after `3` seconds: `true`
  - Second instance exit code: `0`
  - Running Dateview process count after cleanup: `0`

Manual smoke:

- Automated system-tray icon clicking and visual animation feel inspection are not reliably available in this executor environment.
- Strongest available evidence is focused Desktop state/animation/window tests plus process/single-instance smoke.
- Final R6 will repeat package/release smoke and record the same limitation if interactive tray inspection remains unavailable.

Commit / push:

- This R4 section is committed by the R4 P12 toggle integration commit.

Risk / blocked:

- Human visual confirmation of tray icon appearance and animation feel is still recommended on a real desktop tray session.

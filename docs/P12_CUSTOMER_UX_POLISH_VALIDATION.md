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

- [ ] Make the tray icon use the Dateview icon asset or embedded application icon instead of `SystemIcons.Application`.
- [ ] Keep icon ownership/disposal correct.
- [ ] Add/update Desktop tests so the tray icon is not the generic system application icon.
- [ ] Verify package/publish still has coherent application icon evidence.

### R3 Soft Open/Close Animation

- [ ] Add deterministic soft entrance and exit animation behavior.
- [ ] Keep transforms on popup content, never the WPF `Window`.
- [ ] Add/update Desktop animation tests.

### R4 Tray Toggle Integration

- [ ] Wire tray left-click visible state to animated close.
- [ ] Keep Escape, close command, and deactivation paths working.
- [ ] Cover repeated click/open/close behavior with tests or recorded smoke evidence.

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

# P5 Tray UX Validation

Date: 2026-06-22
Round: R24 P5.5 Tray/UX final validation

## Scope

P5 validates tray ownership, popup toggling, placement, basic context menu shape, single-instance behavior, and shutdown cleanup.

## Quality Gates

- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- Release build: passed with 0 warnings and 0 errors.
- Test suite: passed.
- `dotnet format Dateview.slnx --verify-no-changes`: passed with the known workspace-load warning only.

## Smoke Evidence

- App starts as a tray-owned process and does not show the popup immediately.
- R24 process smoke confirmed the first instance stayed running, the startup popup stayed hidden, and the second instance exited with code 0.
- Tray left-click flow is covered by `TrayIconServiceTests.LeftMouseUpRaisesPrimaryClick`.
- Popup toggle, Escape hide, and reusable hidden popup behavior are covered by Desktop tests.
- Right-click context menu shape is covered by `TrayIconServiceTests.ShowCreatesBasicContextMenuItems`.
- Context menu `Today` and `Exit` commands are covered by `TrayIconServiceTests.TodayMenuClickRaisesTodayRequested` and `TrayIconServiceTests.ExitMenuClickRaisesExitRequested`.
- Single-instance behavior was process-smoked in R22: first instance stayed running and second instance exited with code 0.
- Popup placement near taskbar edges is covered by `PopupPlacementCalculatorTests`.

## Current UX Notes

- `Settings` is enabled after P6 and opens the settings window; `Start with Windows` remains represented in the settings window rather than as a direct tray-menu toggle.
- The app uses normal user-mode tray APIs only. It does not hook Explorer, inject into the taskbar, or require administrator permission.
- The popup has `ShowInTaskbar=False`, `WindowStyle=None`, and tray-owned show/hide behavior, so it does not add a normal taskbar window button.

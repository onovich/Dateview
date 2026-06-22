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

- [ ] Start the published executable as a normal user.
- [ ] Confirm the Dateview tray icon is visible in the notification area or Windows tray overflow.
- [ ] Confirm a second launch exits successfully without opening another long-running instance.
- [ ] Right-click the tray icon and confirm the context menu contains Today, Settings, Start with Windows, and Exit.
- [ ] Click Today from the context menu and confirm the calendar returns to the current month when the popup is visible.
- [ ] Click Settings from the context menu and confirm the settings window opens.
- [ ] Toggle Start with Windows from the context menu and restore the original state afterward.
- [ ] Click Exit and confirm the app process exits and the tray icon clears.

### Popup Interaction

- [ ] Left-click the tray icon and confirm the calendar popup opens.
- [ ] Left-click the tray icon again and confirm the popup hides.
- [ ] Press Escape while the popup is open and confirm it hides.
- [ ] Click outside the popup and confirm it hides.
- [ ] Move focus away from the popup and confirm it hides when the pointer is outside the popup.
- [ ] Confirm the popup does not appear in Alt+Tab.
- [ ] Confirm the popup does not create a normal taskbar button.
- [ ] Use Previous Month, Next Month, and Today controls and confirm the displayed month changes correctly.

### Placement, DPI, And Visual Behavior

- [ ] Confirm the popup opens near the taskbar edge, not centered on the screen.
- [ ] Confirm the popup stays within the current monitor working area.
- [ ] Confirm the popup appears on the monitor where the tray interaction occurred, when a multi-monitor setup is available.
- [ ] Confirm placement remains correct on available display scale settings, or record the limitation if the environment cannot change DPI.
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

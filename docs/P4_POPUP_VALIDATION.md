# P4 Popup Validation

Date: 2026-06-22
Round: R19 P4.6 Popup final validation

## Scope

P4 validates the standalone WPF calendar popup before tray integration begins.

## Quality Gates

- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- Release build: passed with 0 warnings and 0 errors.
- Test suite: passed.
- `dotnet format Dateview.slnx --verify-no-changes`: passed with the known workspace-load warning only.

## Smoke Evidence

- Launched `src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\ChinaTrayCalendar.Desktop.exe`.
- UI Automation found the `Dateview` popup window.
- UI Automation found 4 popup buttons.
- Invoked next month, previous month, and today buttons through `InvokePattern`.
- Verified popup header and weekday text elements were present.
- Error state remains covered by `CalendarViewModelTests.LoadAsyncSurfacesErrorState`.

## Architecture Check

- Calendar business rules stay in Domain/Application.
- The popup view binds to `CalendarViewModel`; it does not calculate calendar days or holiday markers.
- Code-behind is limited to WPF visual event bridging for `InitializeComponent` and Escape close handling.
- Holiday data is loaded offline from bundled JSON through Infrastructure.

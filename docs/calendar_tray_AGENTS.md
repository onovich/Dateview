# AGENTS.md — China Tray Calendar

## Project mission

Build a lightweight Windows tray calendar app that provides a better replacement experience for the Windows taskbar date flyout. The app must show the current month calendar, highlight today, weekends, Chinese public holidays, and adjusted workdays.

This app must not modify Windows Explorer, inject into the taskbar, hook system shell internals, or replace system DLLs. It should run as a normal user-mode desktop app with a notification-area tray icon.

## Product scope

### MVP scope

- Run as a single-instance Windows desktop app.
- Show a notification-area/tray icon.
- Left click tray icon to show or hide a compact calendar popup near the taskbar/current monitor.
- Right click tray icon to show a context menu with: Today, Settings, Start with Windows, Exit.
- Calendar popup defaults to the current month.
- Support previous month, next month, and return to today.
- Show a 6 x 7 month grid, Monday as the default first day of week.
- Mark today, weekends, Chinese day-off holidays, and adjusted workdays.
- Work offline using bundled holiday JSON files.
- Persist user settings under the current user profile.
- Provide a startup toggle using current-user startup registration.

### Explicit non-goals for MVP

- No Explorer/taskbar injection.
- No global hooks.
- No admin privilege requirement.
- No online holiday API dependency.
- No calendar account sync.
- No lunar calendar in MVP unless implemented after core acceptance is complete.
- No telemetry.

## Technology baseline

- Language: C#.
- Runtime/UI: .NET 10 + WPF.
- Tray integration: `System.Windows.Forms.NotifyIcon` for MVP. Use direct Win32 `Shell_NotifyIcon` only if there is a documented limitation that `NotifyIcon` cannot solve.
- Serialization: `System.Text.Json`.
- Tests: xUnit or MSTest. Pick one and keep it consistent.
- No third-party package unless a short ADR is added under `docs/adr/` explaining why it is necessary and why a standard library approach is insufficient.

## Required repository layout

```text
src/
  ChinaTrayCalendar.Domain/
  ChinaTrayCalendar.Application/
  ChinaTrayCalendar.Infrastructure/
  ChinaTrayCalendar.Desktop/
tests/
  ChinaTrayCalendar.Domain.Tests/
  ChinaTrayCalendar.Application.Tests/
  ChinaTrayCalendar.Infrastructure.Tests/
docs/
  adr/
  DEVELOPMENT_PLAN.md
  HOLIDAY_DATA.md
assets/
  icons/
  holidays/cn/
```

## Architecture rules

### Dependency direction

The dependency direction is strict:

```text
Desktop -> Application -> Domain
Desktop -> Infrastructure -> Application -> Domain
Infrastructure -> Application -> Domain
Domain -> no project dependencies
```

Never introduce a reference that violates this direction.

### Domain project rules

`ChinaTrayCalendar.Domain` is pure business logic.

Allowed:

- `DateOnly`, `DayOfWeek`, collections, records, enums, validation.
- Calendar-grid generation.
- Holiday/day classification logic.

Forbidden:

- WPF, WinForms, Win32, registry, file system, network, JSON, logging frameworks.
- `DateTime.Now`, `DateTime.UtcNow`, `TimeZoneInfo.Local`.
- Service locators, dependency injection containers.

### Application project rules

`ChinaTrayCalendar.Application` contains use cases and ports.

Allowed:

- Use cases such as `GetMonthCalendarUseCase`, `GoToTodayUseCase`, `ToggleStartupUseCase`.
- Interfaces such as `IHolidayRepository`, `ISettingsStore`, `IClock`, `IAutoStartService`.
- DTOs/view models that are UI-framework independent.

Forbidden:

- WPF/WinForms types.
- Registry/file-system implementation details.
- JSON parsing implementation details.
- Direct calls to wall-clock time.

### Infrastructure project rules

`ChinaTrayCalendar.Infrastructure` implements external adapters.

Allowed:

- JSON holiday repository.
- Settings store under user profile.
- Current-user startup registration.
- Windows-specific adapters.
- File IO, registry IO, optional logging sinks.

Forbidden:

- Business rules that belong in Domain/Application.
- Direct UI behavior.
- Network calls during startup.
- Writes outside the app's allowed paths or HKCU startup location.

### Desktop project rules

`ChinaTrayCalendar.Desktop` is the WPF executable and composition root.

Allowed:

- WPF windows, controls, styles, resources.
- Tray icon lifecycle.
- View model binding.
- App startup/shutdown.
- Dependency injection composition.

Forbidden:

- Business logic in code-behind.
- Calendar calculation in XAML converters.
- Holiday parsing in UI classes.
- Registry/file-system logic except composition wiring.

Code-behind must contain only `InitializeComponent`, visual event bridging that cannot be expressed through binding, and window placement plumbing. Any non-trivial behavior must move to a view model, service, or use case.

## Code quality constraints

- Enable nullable reference types in every project.
- Treat warnings as errors, except generated WPF files if a specific exemption is required.
- One public type per file.
- Class length target: under 300 lines. Method length target: under 50 lines.
- Use constructor injection. Do not use service locator patterns.
- No mutable global state.
- No static singletons except pure constants.
- No hard-coded absolute paths.
- Use `DateOnly` for calendar dates. Use `DateTimeOffset` only for timestamps/logging.
- All date-sensitive code must depend on `IClock`.
- All async methods must accept `CancellationToken` where cancellation could matter.
- Public methods must validate arguments and fail with clear exceptions.
- Do not swallow exceptions. Handle expected failures explicitly and surface safe user-facing messages.
- Keep core features offline-first.
- Prefer simple, readable code over clever abstractions.

## UI rules

- Default language: Simplified Chinese.
- All user-facing strings must go through resources or a central string provider after MVP foundation is created.
- Calendar cell must not rely on color alone; use badge text such as `休` and `班`.
- Popup must close on Escape.
- Popup must not steal focus aggressively.
- Popup placement must respect the current monitor working area.
- The app must support light/dark theme detection or at least offer a clean fixed theme in MVP.
- Do not block the UI thread with file IO or expensive parsing.

## Holiday data rules

- Holiday data files live under `assets/holidays/cn/{year}.json`.
- Holiday data must be versioned using `schemaVersion`.
- Every holiday/workday entry must be represented by an ISO date string: `yyyy-MM-dd`.
- Duplicate entries for the same date are invalid unless explicitly resolved by documented precedence.
- Adjusted workday has higher visual priority than weekend.
- Day-off holiday has higher visual priority than normal weekday.
- Source metadata is required for each year.
- Do not infer official adjusted workdays from formulas; use official yearly data.

Recommended model:

```json
{
  "schemaVersion": 1,
  "jurisdiction": "CN",
  "year": 2026,
  "source": {
    "title": "国务院办公厅关于2026年部分节假日安排的通知",
    "publishedDate": "2025-11-04"
  },
  "days": [
    { "date": "2026-01-01", "type": "dayOff", "name": "元旦" },
    { "date": "2026-01-04", "type": "adjustedWorkday", "name": "元旦调休上班" }
  ]
}
```

## Test rules

Minimum required tests:

- Month grid generation for months that start on Monday, Sunday, and midweek.
- Leap year February.
- Today marker with fake clock.
- Weekend classification.
- Holiday classification.
- Adjusted workday overrides weekend.
- JSON holiday parser rejects invalid date, duplicate date, unknown type, and mismatched year.
- Settings store round trip.

Use deterministic fake clocks in tests. Never use the real current date in assertions.

## Commands

Use these commands as quality gates:

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet format --verify-no-changes
```

A change is not complete unless build and tests pass. If formatting tooling is not configured yet, add it early or document why it is unavailable.

## Definition of Done

A task is done only when:

- It fits the architecture boundaries.
- It has tests for domain/application behavior.
- It does not introduce forbidden dependencies.
- It builds in Release mode.
- It does not degrade offline behavior.
- It does not require administrator permission.
- It has clear user-facing behavior for failure cases.
- It updates documentation when public behavior or architecture changes.

## Codex working instructions

Before implementing a feature:

1. Inspect existing project structure and tests.
2. Identify the layer where the change belongs.
3. Make the smallest coherent change.
4. Add or update tests before or alongside implementation.
5. Run build/test/format gates.
6. Summarize changed files, behavior, and any remaining risk.

Do not perform broad refactors unless the task requires them. Do not move code across layers without explaining the architectural reason.

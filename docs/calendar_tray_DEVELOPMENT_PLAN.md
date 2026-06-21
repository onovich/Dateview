# DEVELOPMENT_PLAN.md — China Tray Calendar

## 1. Product vision

Create a small Windows tray calendar app for users who need quick access to the current month and Chinese public holiday information from the bottom-right taskbar area.

The app should feel native, fast, private, and reliable. It should replace the user's workflow, not Windows system components.

## 2. MVP user stories

1. As a user, I can start the app and see a tray icon.
2. As a user, I can left-click the tray icon to open a month calendar.
3. As a user, I can see today's date clearly.
4. As a user, I can see Chinese holidays and adjusted workdays.
5. As a user, I can move between months and return to today.
6. As a user, I can enable or disable start with Windows.
7. As a user, I can exit the app from a tray menu.

## 3. Non-functional goals

- Normal user permission only.
- Offline-first.
- Fast startup.
- Small memory footprint.
- No telemetry.
- No system shell injection.
- Maintainable architecture with strict layer boundaries.
- Deterministic date tests.

## 4. Architecture overview

```text
+-------------------------------------------------------------+
| ChinaTrayCalendar.Desktop                                   |
| WPF App, Views, ViewModels, Tray lifecycle, composition root |
+-----------------------+-------------------------------------+
                        |
                        v
+-------------------------------------------------------------+
| ChinaTrayCalendar.Application                               |
| Use cases, ports, app-level DTOs                            |
+-----------------------+-------------------------------------+
                        |
                        v
+-------------------------------------------------------------+
| ChinaTrayCalendar.Domain                                    |
| Date model, month-grid generation, day classification        |
+-------------------------------------------------------------+

+-------------------------------------------------------------+
| ChinaTrayCalendar.Infrastructure                             |
| JSON holiday repo, settings store, startup registration      |
+-------------------------------------------------------------+
```

## 5. Key domain concepts

### CalendarDate

Use `DateOnly` directly unless a stronger domain type becomes necessary.

### CalendarMonth

Represents a year and month.

Properties:

- `int Year`
- `int Month`

Validation:

- Year: 1900–2100 for MVP.
- Month: 1–12.

### CalendarDay

Represents one cell in the month grid.

Properties:

- `DateOnly Date`
- `bool IsInCurrentMonth`
- `bool IsToday`
- `bool IsWeekend`
- `DayMarker Marker`

### DayMarker

Represents a day annotation.

Types:

- `None`
- `DayOff`
- `AdjustedWorkday`
- `FestivalOnly` reserved for future.

### MonthGrid

A 6 x 7 immutable collection of `CalendarDay`.

Rules:

- Default first day: Monday.
- Always 42 cells.
- Includes previous/next month filler days.
- All day classification must happen before view binding.

## 6. Application ports

```csharp
public interface IClock
{
    DateOnly Today { get; }
}

public interface IHolidayRepository
{
    Task<IReadOnlyList<HolidayDay>> GetHolidayDaysAsync(int year, CancellationToken cancellationToken);
}

public interface ISettingsStore
{
    Task<AppSettings> LoadAsync(CancellationToken cancellationToken);
    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken);
}

public interface IAutoStartService
{
    bool IsEnabled();
    void SetEnabled(bool enabled);
}
```

## 7. Use cases

### GetMonthCalendarUseCase

Input:

- `CalendarMonth month`
- `DateOnly today`
- `DayOfWeek firstDayOfWeek`

Output:

- `MonthGrid`

Behavior:

- Loads holiday data for all years visible in the 42-cell grid.
- Builds the month grid.
- Applies today/weekend/holiday/adjusted-workday classification.

### ToggleAutoStartUseCase

Input:

- `bool enabled`

Output:

- New state.

Behavior:

- Uses `IAutoStartService`.
- Does not require admin permission.

### LoadSettingsUseCase / SaveSettingsUseCase

Behavior:

- Load defaults when no settings exist.
- Validate loaded settings.
- Ignore unknown JSON properties for forward compatibility.

## 8. Infrastructure design

### JsonHolidayRepository

Responsibilities:

- Read bundled files from `assets/holidays/cn/{year}.json` or copied content output.
- Parse and validate schema.
- Cache parsed data per year for process lifetime.
- Return empty list for missing future year only when explicitly allowed; otherwise surface a clear error.

### JsonSettingsStore

Storage path:

```text
%APPDATA%/ChinaTrayCalendar/settings.json
```

Responsibilities:

- Create directory when missing.
- Use atomic save: write temp file, then replace.
- Fall back to default settings when file is missing.
- Return a safe error when JSON is corrupt.

### WindowsAutoStartService

Responsibilities:

- Register or remove current-user startup entry.
- Do not write to machine-wide registry.
- Do not require admin privileges.
- Quote executable path safely.

### TrayIconService

Responsibilities:

- Own the NotifyIcon lifecycle.
- Dispose icon on app shutdown.
- Expose left-click/right-click events to Desktop layer.
- Do not contain calendar logic.

## 9. Desktop/WPF design

### Main startup flow

1. Ensure single-instance app lock.
2. Compose services.
3. Load settings.
4. Create tray icon.
5. Do not show a normal main window.
6. Open popup only on tray click.

### CalendarPopupWindow

Behavior:

- Opens near the tray/current cursor monitor.
- Has no taskbar button.
- Closes on Escape.
- Hides on deactivation unless focus moves to an owned settings window.
- Uses view model commands.

### CalendarViewModel

State:

- `DisplayedMonth`
- `MonthTitle`
- `WeekdayHeaders`
- `ObservableCollection<CalendarDayViewModel>` or immutable replacement list
- `IsLoading`
- `ErrorMessage`

Commands:

- `PreviousMonthCommand`
- `NextMonthCommand`
- `TodayCommand`
- `CloseCommand`

View model must not reference WPF controls.

## 10. Holiday data plan

### MVP bundled years

Bundle the current year and adjacent years:

- 2025
- 2026
- 2027 placeholder only after official data exists

Do not invent future adjusted workdays. For unknown years, show weekends and regular dates, and display a subtle message such as “暂无该年份假期数据”.

### Validation rules

- `schemaVersion` must be supported.
- `jurisdiction` must be `CN`.
- `year` must match all date years, unless cross-year entries are explicitly supported later.
- `type` must be known.
- No duplicate date entries.
- All dates must use ISO format.

## 11. Milestones

### Milestone 0 — Repo bootstrap

Deliverables:

- Solution and projects.
- `AGENTS.md`.
- `docs/DEVELOPMENT_PLAN.md`.
- `.editorconfig`.
- Build/test commands working.

Acceptance:

- `dotnet build --configuration Release` passes.
- Test project exists and runs.

### Milestone 1 — Domain calendar engine

Deliverables:

- Calendar month/grid model.
- 42-cell month generation.
- Weekend/today classification.
- Unit tests.

Acceptance:

- Leap year and first-day-of-week tests pass.
- No UI dependencies in Domain.

### Milestone 2 — Holiday data pipeline

Deliverables:

- Holiday JSON schema.
- 2026 CN data file.
- JSON parser and validator.
- Holiday classification.
- Unit tests for parser and classification.

Acceptance:

- Adjusted workday overrides weekend.
- Invalid JSON data is rejected with clear error.

### Milestone 3 — WPF calendar popup

Deliverables:

- Calendar popup window.
- Month navigation.
- Today marker.
- Holiday/workday badges.
- Loading/error states.

Acceptance:

- Popup displays current month.
- Navigation works.
- UI code does not calculate calendar data directly.

### Milestone 4 — Tray integration

Deliverables:

- Tray icon.
- Left-click popup toggle.
- Right-click menu.
- Clean shutdown.
- Single-instance guard.

Acceptance:

- App starts with no main window.
- Tray icon is disposed on exit.
- Second launch focuses/toggles existing instance or exits safely.

### Milestone 5 — Settings and startup

Deliverables:

- Settings store.
- Start-with-Windows toggle.
- First day of week setting.
- Theme setting placeholder.

Acceptance:

- Settings persist across restarts.
- Startup toggle reads/writes current-user setting only.

### Milestone 6 — Polish and packaging

Deliverables:

- Icon assets.
- Release publish profile.
- README.
- Basic troubleshooting docs.

Acceptance:

- Release build/package produced.
- Fresh install/use path documented.

## 12. Suggested first tasks for Codex

1. Create solution and project structure exactly as defined.
2. Add `.editorconfig` with nullable and warnings-as-errors defaults.
3. Implement Domain models and `MonthGridBuilder`.
4. Add deterministic Domain tests.
5. Implement holiday schema and parser.
6. Add 2026 CN holiday data from official source.
7. Implement WPF popup UI after Domain/Application tests pass.
8. Implement tray lifecycle.
9. Implement settings/startup.
10. Polish and package.

## 13. Risk register

### Risk: Windows tray icon hidden by system settings

Mitigation: Document that Windows may place app icons in the hidden tray menu. Provide onboarding tip.

### Risk: Holiday data changes yearly

Mitigation: Keep data files versioned and source-attributed. Add docs for updating next-year data.

### Risk: Popup placement across monitors/DPI

Mitigation: Add a small placement service and test pure geometry calculations separately from WPF.

### Risk: UI logic leaking into code-behind

Mitigation: Enforce code-behind restrictions and review every UI PR against AGENTS.md.

### Risk: Overengineering

Mitigation: Keep MVP small. No online update service, calendar sync, or lunar calendar until core feature quality is proven.

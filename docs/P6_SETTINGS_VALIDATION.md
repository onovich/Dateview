# P6 Settings Validation

Date: 2026-06-22
Round: R28 P6.4 Settings final validation

## Scope

P6 validates JSON-backed settings, current-user startup registration, and the Desktop settings entry point.

## Quality Gates

- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- Release build: passed with 0 warnings and 0 errors.
- Test suite: passed.
- `dotnet format Dateview.slnx --verify-no-changes`: passed with the known workspace-load warning only.

## Smoke Evidence

- `JsonSettingsStoreTests` cover missing-file defaults, round trip save/load, containing directory creation, existing-file replacement, corrupt JSON, invalid enum values, and null-save rejection.
- `WindowsAutoStartServiceTests` cover HKCU Run value naming, quoted executable paths, enable/disable behavior, observed startup state, path validation, and registry error wrapping without touching the real user registry.
- `SettingsViewModelTests` cover loading settings, using observed startup state, saving first-day/startup values, close notification, and save-failure messaging.
- `CalendarViewModelTests.ApplyFirstDayOfWeekAsyncRefreshesHeadersAndGrid` covers applying first-day changes to the visible popup model.
- R28 process smoke confirmed the settings-enabled app stayed running and a second instance exited with code 0.

## Notes

- The settings file path is `%APPDATA%\ChinaTrayCalendar\settings.json`.
- The startup service writes only `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.
- The validation suite avoids changing the real user's startup registry value; registry behavior is covered through the Infrastructure adapter boundary.

# P7 Release Dry-Run

Date: 2026-06-22
Round: R31 P7.3 Release dry-run

## Commands

- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.

## Publish Output

Output folder:

```text
src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\
```

Confirmed files:

- `ChinaTrayCalendar.Desktop.exe`
- `assets\holidays\cn\2025.json`
- `assets\holidays\cn\2026.json`

## Fresh Output Smoke

- Launched the published executable as a normal user process.
- Confirmed the first instance stayed running.
- Confirmed the published holiday data was present.
- Launched a second published instance and confirmed it exited with code 0.

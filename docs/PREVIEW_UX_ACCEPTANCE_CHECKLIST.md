# Dateview Preview UX Acceptance Checklist

Use this checklist to accept or reject the Dateview `0.1.0-preview` tray icon, popup animation, and tray-click toggle changes from P12.

## Setup

- Use a trusted preview artifact and verify the zip SHA256 before running it.
- Extract the zip to a normal user-writable folder.
- Run `Dateview\ChinaTrayCalendar.Desktop.exe`.
- Keep Windows Security, Microsoft Defender, and SmartScreen enabled.
- Do not enable `Start with Windows` unless startup behavior is explicitly being tested.

Record the environment:

```text
Windows edition/version/build:
Display count and resolutions:
Scale percentage / DPI:
Taskbar edge:
Taskbar autohide: yes | no
Tray icon location: normal tray | overflow | not visible
Artifact path:
Artifact SHA256:
Tester:
Date/time:
```

## Acceptance Matrix

Mark each item `PASS`, `FAIL`, `LIMITED`, or `NOT AVAILABLE`.

| Item | Result | Notes |
| --- | --- | --- |
| Dateview starts and leaves one primary process running |  |  |
| Tray icon is visible in the notification area or overflow |  |  |
| Tray icon looks Dateview-specific, not generic |  |  |
| Tray icon looks acceptable in tray overflow |  |  |
| Right-click menu opens |  |  |
| Right-click menu includes Today, Settings, Start with Windows, Exit |  |  |
| Left-click tray icon opens the popup |  |  |
| Popup appears near the taskbar/current monitor working area |  |  |
| Open animation feels calm/soft |  |  |
| Open animation has no obvious bounce/shake |  |  |
| Left-click tray icon again closes the popup |  |  |
| Close animation feels calm/soft |  |  |
| Reopen after close works |  |  |
| Escape closes the popup |  |  |
| Outside click/deactivation closes the popup |  |  |
| A second app launch exits cleanly |  |  |
| Exit menu closes Dateview |  |  |
| No Dateview process remains after exit |  |  |
| Settings/startup state was not changed, or was restored |  |  |

## Pass / Fail Rules

Accept the P12 UX changes only if:

- The tray icon is recognizable enough for a tester to identify Dateview.
- Left-click open and left-click close both work.
- Open and close motion does not feel shaky or like a bounce-back.
- Escape and outside/deactivation close paths still work when testable.
- No Dateview process remains after exit.

Classify findings:

- `fix-now`: blocks preview acceptance, such as no tray icon, generic icon, crash, stuck popup, or no close path.
- `document`: expected behavior that needs clearer tester instructions.
- `defer`: non-blocking polish outside the P13 scope.
- `needs hardware reproduction`: display/DPI/taskbar behavior that needs a different physical environment.

Report issues using `docs\PREVIEW_FEEDBACK_GUIDE.md`.

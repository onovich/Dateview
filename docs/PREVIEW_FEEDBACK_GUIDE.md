# Dateview Preview Feedback Guide

Use this guide for Dateview `0.1.0-preview` limited trusted preview feedback.

The goal is to make each report reproducible, triageable, and honest about hardware coverage. Dateview does not collect telemetry or upload logs automatically.

## Triage Outcomes

Each report should end in one of these outcomes:

- `fix-now`: blocks or materially harms the limited preview and should be repaired before more testers use the build.
- `document`: expected preview behavior or environment limitation that needs clearer handoff/troubleshooting text.
- `defer`: valid issue, but not required for the limited preview scope.
- `needs hardware reproduction`: likely depends on hardware or desktop state the current executor machine cannot reproduce, such as physical multi-monitor or non-100% DPI.

## Severity

- `S0`: Security, trust, data-loss, or system-modification risk. Examples: unexpected admin prompt, HKLM write, Defender/SmartScreen bypass instruction, untrusted artifact/hash mismatch, Explorer/taskbar injection behavior, or any public-release/upload accident.
- `S1`: Preview blocker. Examples: app will not start, app crashes, app cannot exit, tray icon never appears, second instance hangs, startup toggle leaves the app unable to start normally, or portable zip cannot be verified/extracted.
- `S2`: Important functional issue. Examples: popup appears off-screen, Escape/outside click does not hide it, month navigation breaks, holiday/workday badges are wrong, settings do not persist, or startup toggle state is incorrect.
- `S3`: Usability, documentation, or cosmetic issue that does not block preview usage.

## Issue Categories

Use one primary category:

- `package-hash`: zip, SHA256, sidecar, manifest, metadata, extraction, or file layout.
- `startup-single-instance`: first launch, second launch, process lifetime, startup with Windows, or exit cleanup.
- `tray-visibility-menu`: tray icon visibility, overflow menu behavior, left-click popup toggle, right-click context menu, Today/Settings/Exit entries.
- `popup-placement`: popup monitor selection, taskbar edge, off-screen placement, hiding behavior, focus/deactivation, Escape key.
- `dpi-display`: scaling, non-100% DPI, text fit, multi-monitor, display resolution, taskbar autohide.
- `calendar-holiday-data`: month grid, today marker, weekends, day-off holiday badges, adjusted workday badges, 2025/2026 data.
- `settings-startup`: settings persistence, settings file, HKCU Run value, moved-folder cleanup.
- `trust-warning`: unknown publisher, SmartScreen, Microsoft Defender, source verification, hash mismatch.
- `crash-hang-performance`: crash dialog, unhandled exception, hang, high CPU/memory, slow startup.
- `docs-confusion`: handoff, release notes, troubleshooting, feedback guide, or unclear wording.

## Required Report Fields

Copy this template for every report:

```text
Title:
Severity: S0 | S1 | S2 | S3
Category:
Triage suggestion: fix-now | document | defer | needs hardware reproduction

Dateview version:
Zip filename:
Zip SHA256 shown by Get-FileHash:
SHA256 sidecar matched: yes | no | not checked
Zip/source received from:
Extracted path:

Windows edition:
Windows version/build:
.NET runtime prompt or error, if any:
Security prompt details, if any:

Display count:
Display resolutions:
Scale percentages:
Taskbar edge:
Taskbar autohide enabled: yes | no | unknown
Tray icon was in overflow: yes | no | unknown

Startup enabled in Dateview: yes | no | unknown
Settings folder existed: yes | no | unknown
Settings path, if relevant:

Steps to reproduce:
1.
2.
3.

Expected result:
Actual result:
Frequency: always | sometimes | once
Workaround, if any:
Screenshot/video attached: yes | no | not applicable
Crash dialog or exception text:
```

## Evidence Guidance

- For UI placement, include a screenshot or short screen recording when possible.
- For trust warnings, include the warning text and whether the zip hash matched. Do not include sensitive local paths or account details unless needed.
- For crashes, copy the exception type, message, and top stack frames if a dialog shows them.
- For startup issues, state whether `Start with Windows` was enabled and whether the app folder was moved after enabling it.
- For package/hash issues, attach the exact SHA256 from `Get-FileHash` and the content of the `.sha256.txt` file.

## Environment Focus Areas

The current executor machine has live coverage for:

- Windows 11
- One display
- Bottom taskbar
- `100%` scale / `96 DPI`

Reports are especially valuable for:

- Two or more physical displays.
- Non-100% scaling, such as `125%`, `150%`, or mixed-DPI displays.
- Taskbar on top, left, or right.
- Taskbar autohide enabled.
- Tray icon inside the overflow menu.
- Non-default install paths, such as a folder under `%LOCALAPPDATA%`, Desktop, OneDrive, or a path with spaces.

Do not claim an environment is fully validated unless it was actually tested.

## Privacy And Logs

Dateview preview does not send telemetry, diagnostics, analytics, or logs to the project automatically.

Share only the minimum local information needed to reproduce the issue. Redact usernames, private folder names, and screenshots if they show unrelated personal data.

## Out Of Scope For P11

The following requests should be captured but triaged as `defer` or routed to a future planning phase:

- Code signing or certificate setup.
- Installer/MSIX/WiX/Inno/NSIS implementation.
- Auto-update.
- Online holiday API or account sync.
- Calendar feature expansion beyond the existing preview behavior.
- Explorer/taskbar injection, Shell hooks, global hooks, HKLM writes, or administrator-required behavior.
- Public GitHub Release, store publishing, or public announcement.

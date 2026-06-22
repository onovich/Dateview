# Dateview Limited Preview Handoff

Audience: trusted limited-preview testers for Dateview `0.1.0-preview`.

This handoff is for private preview distribution only. Do not publish the zip publicly, upload it to a store, create a public GitHub Release, or forward it outside the preview group unless the project owner explicitly approves it.

## Files To Receive

The preview coordinator should provide these files from the same release generation:

```text
Dateview-0.1.0-preview-win-x64.zip
Dateview-0.1.0-preview-win-x64.sha256.txt
Dateview-0.1.0-preview-win-x64.release.json
```

The zip contains the portable app folder:

```text
Dateview\ChinaTrayCalendar.Desktop.exe
Dateview\release-manifest.json
Dateview\assets\holidays\cn\2025.json
Dateview\assets\holidays\cn\2026.json
```

## Verify The Zip

Use PowerShell in the folder that contains the zip:

```powershell
Get-FileHash -Algorithm SHA256 .\Dateview-0.1.0-preview-win-x64.zip
Get-Content .\Dateview-0.1.0-preview-win-x64.sha256.txt
```

The SHA256 value printed by `Get-FileHash` must match the value in the `.sha256.txt` file exactly. SHA256 confirms the zip's integrity for this preview handoff; it is not a code signature and does not identify a verified software publisher.

Do not run the zip if:

- The hash does not match.
- The zip came from an unexpected source.
- The filenames or folder layout look different from this handoff.

## Install And Run

Dateview is a portable preview. It does not have an installer.

For local project-side manual testing before handoff, run these commands from the repository root:

```cmd
BuildLatest.cmd
StartPreview.cmd
```

`BuildLatest.cmd` refreshes the preview bundle under `artifacts\release`. `StartPreview.cmd` starts the generated preview app and builds first if the generated app is missing.

1. Extract the zip to a normal user-writable folder, for example:

```text
%LOCALAPPDATA%\Programs\Dateview
```

2. Open the extracted `Dateview` folder.
3. Run:

```text
ChinaTrayCalendar.Desktop.exe
```

4. Look for the Dateview notification-area icon. It may be inside the tray overflow menu.
5. Left-click the tray icon to show or hide the calendar popup.
6. Right-click the tray icon for Today, Settings, Start with Windows, and Exit.

Do not run Dateview directly inside the zip. Extract it first.

## Exit

To close Dateview:

1. Right-click the Dateview tray icon.
2. Select `Exit`.

If the icon is in the tray overflow menu, open the overflow menu first and then right-click Dateview.

## Start With Windows

The `Start with Windows` menu item writes a current-user startup entry only. It does not require administrator privileges and does not write HKLM.

Before moving or deleting the extracted Dateview folder:

1. Right-click the tray icon.
2. Turn off `Start with Windows`.
3. Exit Dateview.

If Dateview was already moved or deleted while startup was enabled, remove the stale current-user startup entry from:

```text
HKCU\Software\Microsoft\Windows\CurrentVersion\Run
```

Value name:

```text
ChinaTrayCalendar
```

## Uninstall The Preview

1. Turn off `Start with Windows` if it is enabled.
2. Exit Dateview.
3. Delete the extracted Dateview folder.
4. Optional: delete the current-user settings folder if you want to remove local preferences:

```text
%APPDATA%\ChinaTrayCalendar
```

## Windows Trust Notes

This preview is unsigned. Windows may show unknown-publisher, SmartScreen, or Microsoft Defender warnings depending on local policy and reputation.

Expected tester behavior:

- Keep Windows Security, Microsoft Defender, and SmartScreen enabled.
- Verify the zip source and SHA256 before running it.
- Stop and report the warning if it looks different from the expected unsigned-preview warning.
- Do not add broad exclusions for Dateview or your download folder.

## Known Preview Limits

- Validated live environment: Windows 11, one display, bottom taskbar, `100%` scale / `96 DPI`.
- Multi-monitor and non-100% DPI coverage still need suitable physical hardware/session feedback.
- Holiday data is bundled offline for `2025` and `2026`.
- There is no installer, auto-update, online holiday API, telemetry, account sync, code signing, or public release channel in this preview.
- Dateview runs as a normal current-user desktop app with a tray icon; it does not replace Explorer, inject into the taskbar, install Shell hooks, or require administrator privileges.

Report feedback using `docs/PREVIEW_FEEDBACK_GUIDE.md`.

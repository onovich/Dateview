# P9 Release Distribution Validation

Date: 2026-06-22
Phase: P9 Release Distribution & Startup Polish
Guide: `docs/P9_RELEASE_DISTRIBUTION_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline: P8 Release Candidate accepted in `docs/P8_DESKTOP_UX_QA_VALIDATION.md`

## Distribution Strategy

Dateview's first trial distribution shape is a portable Windows folder/zip bundle produced from the existing `win-x64-folder` publish output.

- The user downloads or receives a zip, extracts it to a normal user-writable folder, and runs `ChinaTrayCalendar.Desktop.exe`.
- The app remains a normal current-user desktop app with a notification-area tray icon.
- No installer is required for P9.
- No admin rights, HKLM writes, Explorer/taskbar injection, Shell hooks, online holiday API, auto-update, telemetry, account sync, or calendar feature expansion are in scope.
- Hashes generated in P9 are for user-side integrity checks only; they are not code signing.
- Generated bundles, hashes, manifests, and temporary smoke output stay under ignored local artifact folders and are not committed.

Installer decision:

- A complex installer is intentionally deferred. Portable zip/folder is sufficient for the first trial distribution because Dateview already runs without admin rights and stores mutable state under the current user's profile/HKCU.
- Introducing MSIX, WiX, Inno, NSIS, or another third-party packaging tool would require a separate ADR and architect/PM decision.

## Release Artifact Targets

Publish output:

```text
src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\
```

Required portable bundle contents:

- `ChinaTrayCalendar.Desktop.exe`
- `.dll`, `.deps.json`, `.runtimeconfig.json`, and related .NET publish files
- `assets\holidays\cn\2025.json`
- `assets\holidays\cn\2026.json`

Generated release workspace:

```text
artifacts\release\
```

## P9 Checklist

### Strategy And Baseline

- [x] Confirm portable folder/zip is the P9 default distribution shape.
- [x] Document why no installer is required for the first trial distribution.
- [x] Run `Validate.cmd`.
- [x] Run `Package.cmd`.
- [x] Confirm publish output contains executable and bundled holiday data.

### Bundle Tooling

- [x] Add a repeatable release bundle command/script.
- [x] Ensure the script runs from the repository root without developer-machine absolute paths.
- [x] Output generated files under ignored local artifact folders.
- [x] Confirm generated bundle structure is stable.

### Version, Manifest, And Hash

- [x] Define the version source.
- [x] Generate a manifest or release metadata file.
- [x] Generate SHA256 hash data.
- [x] Confirm metadata does not include private local paths.

### Documentation

- [x] Update README for download/extract/run/exit/startup/uninstall.
- [x] Update troubleshooting for portable path and cleanup.
- [x] Add release notes.
- [x] Avoid promises for unimplemented installer/update/sync behavior.

### Startup And Portable Smoke

- [x] Verify clean-user first run and settings path behavior.
- [x] Verify HKCU startup enable/disable and restore original state.
- [ ] Verify portable bundle unzip/run from a temporary folder.
- [ ] Verify second instance exits successfully from the portable folder.
- [ ] Verify holiday data loads from the portable relative `assets` directory.

### Final Distribution Acceptance

- [ ] Run final `Validate.cmd`.
- [ ] Run final `Package.cmd`.
- [ ] Run final release bundle script.
- [ ] Record final artifact path and SHA256.
- [ ] Record residual risk and final release distribution status.

## Round Log

### R1 - Distribution Strategy And Baseline Package

Status: PASS

Scope:

- Confirm P9 distribution strategy.
- Establish baseline validation/package evidence.
- Create this validation report.

Debug self-check:

- Fresh clone/fresh publish path: portable distribution starts from the existing repo-root publish profile and does not require an installed app.
- Failure localization: R1 checks only docs, publish profile output, validation, and package baseline.
- Coverage: missing executable and missing holiday data are explicit checks for R1.
- Startup and release scripts are deferred to later P9 rounds.

Architecture self-check:

- R1 changes documentation only.
- No Domain/Application/Infrastructure/Desktop behavior changes.
- No installer, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, or admin requirement added.
- Generated release artifacts remain out of git.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R1 start.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.
- Publish output checked at:

```text
D:\ToolProjects\Dateview\src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish
```

Required files present:

- `ChinaTrayCalendar.Desktop.exe` (`163328` bytes)
- `ChinaTrayCalendar.Desktop.dll` (`52224` bytes)
- `ChinaTrayCalendar.Desktop.deps.json` (`1981` bytes)
- `ChinaTrayCalendar.Desktop.runtimeconfig.json` (`519` bytes)
- `assets\holidays\cn\2025.json` (`2802` bytes)
- `assets\holidays\cn\2026.json` (`3111` bytes)

Holiday JSON parse evidence:

- `2025.json`: `schemaVersion = 1`, jurisdiction `CN`, `33` days.
- `2026.json`: `schemaVersion = 1`, jurisdiction `CN`, `39` days.

Release artifact/hash:

- Not generated in R1; planned for R2/R3.

Commit / push:

- `ad7798c release: document P9 distribution baseline` pushed.

Risk / blocked:

- None for R1.

### R2 - Repeatable Release Bundle Command

Status: PASS

Scope:

- Added `scripts\package-release.ps1`.
- Added `artifacts/` to `.gitignore`.
- Generated a portable folder plus zip under `artifacts\release`.

Script behavior:

- Runs from the repository root or any script invocation location by resolving paths relative to the script directory.
- Uses the existing `src\ChinaTrayCalendar.Desktop\Properties\PublishProfiles\win-x64-folder.pubxml` publish profile through `dotnet publish`.
- Copies the publish output into:

```text
artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\
```

- Creates:

```text
artifacts\release\Dateview-0.1.0-preview-win-x64.zip
```

- Refuses release cleanup paths outside the repository root.
- Verifies required executable/runtime/holiday files before zip creation.

Debug self-check:

- Fresh publish path: the bundle script starts from the checked-in project and publish profile, not from an existing local artifact.
- Failure localization: script failures are localized to publish, copy/staging, required-file checks, or zip creation.
- Coverage: first run exposed an empty copy bug caused by `Copy-Item -LiteralPath ...\*`; the script now enumerates source children before copying.
- Missing artifact coverage: the script checks exe, dll, deps, runtimeconfig, and 2025/2026 holiday data.

Architecture self-check:

- Release tooling stays under `scripts/` and local ignored `artifacts/`.
- No Domain/Application/Infrastructure/Desktop runtime behavior changed.
- No installer, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, or admin requirement added.
- Generated release artifacts are ignored and not staged.

Validation:

- `powershell -ExecutionPolicy Bypass -File .\scripts\package-release.ps1 -Version 0.1.0-preview`: passed.
- Script output:
  - Bundle name: `Dateview-0.1.0-preview-win-x64`
  - Folder: `D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64`
  - App folder: `D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview`
  - Zip: `D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64.zip`
  - Zip bytes: `171284`
- Zip structure check: `15` entries; required entries present after normalizing separators:
  - `Dateview/ChinaTrayCalendar.Desktop.exe`
  - `Dateview/ChinaTrayCalendar.Desktop.dll`
  - `Dateview/ChinaTrayCalendar.Desktop.deps.json`
  - `Dateview/ChinaTrayCalendar.Desktop.runtimeconfig.json`
  - `Dateview/assets/holidays/cn/2025.json`
  - `Dateview/assets/holidays/cn/2026.json`
- `git status --short --ignored -- artifacts scripts .gitignore docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md`: confirmed `artifacts/` is ignored.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.

Release artifact/hash:

- Bundle zip generated; SHA256 and manifest are planned for R3.

Commit / push:

- `22ed5d3 release: add portable bundle script` pushed.

Risk / blocked:

- None for R2.

### R3 - Version, Manifest, And Hash Metadata

Status: PASS

Scope:

- Defined the release version source as `src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj` property `<Version>0.1.0-preview</Version>`.
- Updated `scripts\package-release.ps1` so `-Version` remains an explicit override, while the default version comes from the Desktop project file.
- Generated app-local `release-manifest.json` inside the portable `Dateview` folder before zipping.
- Generated zip-side release metadata and SHA256 files under ignored `artifacts\release`.

Generated files:

- App manifest:

```text
artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\release-manifest.json
```

- Release metadata:

```text
artifacts\release\Dateview-0.1.0-preview-win-x64.release.json
```

- Zip hash:

```text
artifacts\release\Dateview-0.1.0-preview-win-x64.sha256.txt
```

Debug self-check:

- Fresh publish path: the script reads the version from a checked-in project file and then runs the existing publish profile.
- Failure localization: R3 failures are localized to project version lookup, publish/staging, manifest creation, zip creation, or SHA256 verification.
- Coverage: the manifest includes every app file in the portable folder with relative paths, byte counts, and SHA256 hashes.
- Real tray popup crash regression: `PopupAnimationService` now animates the popup content `UIElement` instead of assigning `RenderTransform` to the top-level WPF `Window`; if content is unavailable, the entrance animation falls back to opacity only. This prevents the `InvalidOperationException` JIT dialog from the real tray left-click popup path.

Architecture self-check:

- Version metadata is in the Desktop executable project, which owns packaging identity for the WPF app.
- Release manifest/hash generation stays in `scripts\package-release.ps1` and ignored local artifacts.
- No Domain/Application/Infrastructure business behavior changed.
- No installer, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, or admin requirement added.
- Generated release artifacts are ignored and not staged.

Validation:

- `powershell -ExecutionPolicy Bypass -File .\scripts\package-release.ps1`: passed.
- Bundle name: `Dateview-0.1.0-preview-win-x64`.
- Zip: `D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64.zip`.
- Zip bytes: `172432`.
- Zip SHA256: `cf7cd17079cebb4d0f5b5d1e9952bab430d914caec5a85298b452e9143ba3c91`.
- Hash file value matches a recalculated SHA256 of the zip.
- Metadata and manifest checked for private path leakage; neither contains `D:\ToolProjects`.
- Manifest file count: `13`.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.

Release artifact/hash:

- `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`
- `cf7cd17079cebb4d0f5b5d1e9952bab430d914caec5a85298b452e9143ba3c91`

Commit / push:

- `f922f24 release: add manifest and hash metadata` pushed.

Risk / blocked:

- None for R3.

### R4 - Distribution Documentation And Release Notes

Status: PASS

Scope:

- Updated `README.md` with the portable release bundle command.
- Added README user workflow for download/copy, optional SHA256 check, extraction, running, exiting, Start with Windows, and portable uninstall.
- Updated `docs\TROUBLESHOOTING.md` with portable path, relative holiday-data, protected-folder, file-lock, moved-folder startup, and SHA256 integrity notes.
- Added `docs\RELEASE_NOTES.md` for `0.1.0-preview`.

Debug self-check:

- Fresh user path: README now starts from receiving a zip and hash file, then extracting and running from a normal user folder.
- Failure localization: troubleshooting separates zip extraction, relative `assets` folder, file locks, startup path staleness, and hash interpretation.
- Coverage: docs explicitly cover exit, startup toggle, uninstall cleanup, and optional settings removal.

Architecture self-check:

- R4 changes documentation only.
- No runtime behavior, dependencies, installer tooling, auto-update, online sync, telemetry, shell hooks, HKLM writes, or admin requirement added.
- Release notes state that SHA256 is an integrity check and not code signing.
- Docs avoid promising unimplemented installer/update/sync behavior.

Validation:

- `git diff --check`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.

Release artifact/hash:

- No new artifact generated in R4; R3 artifact remains the latest script-generated release evidence.

Commit / push:

- `c3e498f docs: add portable release instructions` pushed.

Risk / blocked:

- None for R4.

### R5 - Startup And Settings Clean-User Smoke

Status: PASS

Scope:

- Backed up the current user's `%APPDATA%\ChinaTrayCalendar` directory state.
- Backed up the current user's `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` value named `ChinaTrayCalendar`.
- Cleared those two Dateview-specific states for a controlled first-run smoke.
- Launched the published executable from the `win-x64` publish output.
- Verified test settings creation/parsing/deletion and HKCU startup enable/disable cleanup.
- Restored the original settings directory and startup value state.

Debug self-check:

- Fresh user path: with settings and startup state cleared, the published app started and remained running.
- First-run behavior: the app did not create `settings.json` just by loading default settings.
- Startup behavior: the app did not create the HKCU Run value just by starting.
- Settings cleanup: a test `settings.json` was created, parsed, deleted, and left no test file residue.
- Startup cleanup: a test quoted executable Run value was written, removed, and left no Run value residue.

Architecture self-check:

- R5 changes validation documentation only.
- Runtime settings behavior remains in Infrastructure/Application and tray/UI behavior remains in Desktop.
- No HKLM write, admin requirement, installer, auto-update, telemetry, online service, shell hook, Explorer injection, or taskbar modification added.
- Existing automated tests still cover `JsonSettingsStore`, `WindowsAutoStartService`, and `SettingsViewModel` persistence/toggle behavior.

Validation:

- Published exe:

```text
D:\ToolProjects\Dateview\src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\ChinaTrayCalendar.Desktop.exe
```

- Original settings directory existed: `true`.
- Original `ChinaTrayCalendar` HKCU Run value existed: `false`.
- First-run process id: `6248`.
- First-run `settings.json` created: `false`.
- First-run HKCU Run value created: `false`.
- Test settings file created and parsed: `true`.
- Test settings file deleted: `true`.
- Startup enable value:

```text
"D:\ToolProjects\Dateview\src\ChinaTrayCalendar.Desktop\bin\Release\net10.0-windows\win-x64\publish\ChinaTrayCalendar.Desktop.exe"
```

- Startup disable removed value: `true`.
- Restore verification:
  - Running Dateview process count: `0`.
  - Settings directory exists after restore: `true`.
  - Settings file exists after restore: `false`.
  - HKCU Run value after restore: `null`.

Release artifact/hash:

- No new artifact generated in R5; R3 artifact remains the latest script-generated release evidence.

Commit / push:

- Pending R5 commit.

Risk / blocked:

- None for R5.

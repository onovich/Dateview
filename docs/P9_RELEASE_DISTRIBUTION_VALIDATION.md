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

- [ ] Define the version source.
- [ ] Generate a manifest or release metadata file.
- [ ] Generate SHA256 hash data.
- [ ] Confirm metadata does not include private local paths.

### Documentation

- [ ] Update README for download/extract/run/exit/startup/uninstall.
- [ ] Update troubleshooting for portable path and cleanup.
- [ ] Add release notes.
- [ ] Avoid promises for unimplemented installer/update/sync behavior.

### Startup And Portable Smoke

- [ ] Verify clean-user first run and settings path behavior.
- [ ] Verify HKCU startup enable/disable and restore original state.
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

- Pending R1 commit.

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

- Pending R2 commit.

Risk / blocked:

- None for R2.

# P10 Compatibility And Trust Validation

Date: 2026-06-22
Phase: P10 Compatibility And Trust Readiness
Guide: `docs/P10_COMPATIBILITY_TRUST_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline: P9 portable release distribution accepted in `docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md`

## Scope

P10 is a release-readiness validation and decision-material phase. It does not add calendar features, installer implementation, auto-update, online services, telemetry, Explorer/taskbar injection, Shell hooks, HKLM writes, administrator requirements, or generated artifacts in git.

Allowed P10 outputs:

- Compatibility validation evidence for display, taskbar, DPI, portable startup, and release bundle behavior.
- Documentation updates for unsigned portable distribution and Windows trust expectations.
- ADR material for future release trust, signing, and installer decisions.
- Minimal compatibility fixes only if MVP-required defects are found.

## Hardware And Display Matrix

Current executor environment:

- Machine: `WIN-ONOVICH`
- User: `Administrator`
- OS: `Microsoft Windows 11 专业工作站版`
- OS version: `10.0.26200`
- OS build: `26200`
- .NET SDK: `10.0.102`
- Screen count: `1`
- Primary screen: `\\.\DISPLAY1`
- Screen bounds: `0,0 2560x1440`
- Working area: `0,0 2560x1392`
- Effective DPI: `96x96`
- Scale: `100%`
- Taskbar edge: `Bottom`
- Taskbar rectangle: `0,1392,2560,1440`
- Taskbar auto-hide: `false`
- Taskbar always-on-top state flag: `false`
- Primary GPU: `NVIDIA GeForce RTX 5070 Ti`, driver `32.0.15.9636`
- Secondary GPU listed by Windows: `AMD Radeon(TM) Graphics`, driver `32.0.21043.5001`, no current attached resolution reported.

Compatibility matrix:

| Area | Current evidence | Status |
| --- | --- | --- |
| Single monitor, bottom taskbar, 100% DPI | Physically available on this machine | PASS baseline |
| Physical multi-monitor | Not available; Windows reports only `\\.\DISPLAY1` | Limitation retained |
| Non-100% DPI | Not currently active; effective DPI is `96` | Limitation retained |
| Alternate taskbar edges | Not changed in R1; geometry tests cover placement math | Deferred to R2 |
| Portable bundle generation | `package-release.ps1` passed | PASS baseline |
| Unsigned trust/signing readiness | Documentation/ADR work planned | Deferred to R4/R5 |

## P10 Checklist

### R1 Compatibility Matrix Baseline

- [x] Create this P10 validation document.
- [x] Record current Windows/display/DPI/taskbar facts.
- [x] Run `Validate.cmd`.
- [x] Run `package-release.ps1`.
- [x] Confirm P10 does not implement installer/signing.

### R2 Multi-Monitor And Taskbar Position Spot Check

- [x] Record physical multi-monitor result or limitation.
- [x] Record taskbar-position result or limitation.
- [x] Confirm placement tests provide geometry fallback evidence.

### R3 DPI And Scaling Spot Check

- [x] Record non-100% DPI result or limitation.
- [x] Confirm DPI-aware placement path and tests.
- [x] Verify key text/control fit evidence.

### R4 Windows Trust Prompt Documentation

- [x] Review unsigned portable app documentation.
- [x] Update README/TROUBLESHOOTING/RELEASE_NOTES if needed.
- [x] Avoid instructions that bypass Windows security features.

### R5 Release Trust And Signing ADR

- [x] Add release trust/signing ADR.
- [x] Compare unsigned portable, code signing, MSIX, and installer options.
- [x] Clearly identify decisions requiring PM/budget/certificate approval.

### R8 Final P10 Acceptance

- [x] Run final `Validate.cmd`.
- [x] Run final `Package.cmd`.
- [x] Run final `package-release.ps1`.
- [x] Confirm P10 validation document is complete.
- [x] Confirm ADR exists and does not implement signing/installer.
- [x] Record final compatibility/trust readiness status.

## Round Log

### R1 - Compatibility Matrix Baseline

Status: PASS

Scope:

- Created this P10 validation report.
- Recorded current Windows, display, DPI, taskbar, and GPU facts.
- Ran baseline validation.
- Regenerated the portable release bundle.
- Confirmed P10 remains documentation/validation/decision-material work, not installer or signing implementation.

Debug self-check:

- Minimal fixture: current machine has one physical display, bottom taskbar, and 100% effective DPI.
- Failure localization: R1 only covers environment facts, validation pipeline, and release bundle generation.
- Coverage: single-screen 100% DPI baseline is covered; physical multi-monitor and non-100% DPI remain explicit limitations for later P10 rounds.
- State cleanup: R1 did not alter settings, startup registry, taskbar settings, display scale, or generated artifact tracking.

Architecture self-check:

- R1 changes documentation only.
- No Domain/Application/Infrastructure/Desktop runtime behavior changed.
- No installer, signing, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, or admin requirement added.
- Generated release artifacts remain under ignored `artifacts\release`.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R1 start.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
  - Domain tests: `33` passed.
  - Application tests: `21` passed.
  - Infrastructure tests: `37` passed.
  - Desktop tests: `38` passed.
  - `dotnet format --verify-no-changes`: passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1`: passed.
  - Bundle name: `Dateview-0.1.0-preview-win-x64`
  - Zip: `D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64.zip`
  - Zip bytes: `172391`
  - Zip SHA256: `53d7d7eb1036b04583f3aef79c02e6d9ff26663bf54353a10880ecea6e5b3ca0`

Release artifact/hash:

- `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`
- `53d7d7eb1036b04583f3aef79c02e6d9ff26663bf54353a10880ecea6e5b3ca0`

Commit / push:

- `abec3c6 compat: document P10 baseline matrix` pushed.

Risk / blocked:

- Physical multi-monitor hardware is not currently available.
- Non-100% DPI is not currently active and has not been changed in R1.

### R2 - Multi-Monitor And Taskbar Position Spot Check

Status: PASS WITH LIMITATIONS

Scope:

- Reconfirmed the current environment exposes only one Windows Forms screen: `\\.\DISPLAY1`.
- Reconfirmed the current taskbar edge is `Bottom`, with working area `0,0 2560x1392`.
- Reviewed Desktop placement implementation and tests.
- Ran focused Desktop placement tests for taskbar edge geometry.
- Chose not to programmatically move the Windows taskbar because doing so would mutate the user's desktop state and is not required for a safe automated executor smoke.

Physical multi-monitor result:

- Not available on this machine.
- Windows reports screen count `1`; therefore P10 cannot claim physical multi-monitor coverage.
- The release risk remains a coverage limitation rather than a known placement failure.

Taskbar position result:

- Current physical taskbar position: bottom.
- Alternate physical taskbar edges were not changed in this automated run.
- Geometry fallback evidence covers bottom, top, left, right, and no-taskbar-edge working-area cases.

Relevant implementation evidence:

- `src\ChinaTrayCalendar.Desktop\PopupPlacement\PopupWindowPlacer.cs`
  - Uses `Screen.FromPoint(clickPoint)` to select the monitor for the tray click point.
  - Converts WPF popup size to physical pixels using `VisualTreeHelper.GetDpi`.
  - Converts the final pixel location back to WPF DIPs.
- `src\ChinaTrayCalendar.Desktop\PopupPlacement\PopupPlacementCalculator.cs`
  - Detects taskbar edge by comparing screen bounds with working area.
  - Clamps placement inside the working area.
- `src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj`
  - Declares `ApplicationHighDpiMode` as `PerMonitorV2`.

Debug self-check:

- Minimal fixture: one physical monitor with bottom taskbar is the only live hardware fixture currently available.
- Failure localization: placement failures would localize to monitor selection, DPI conversion, taskbar-edge detection, or clamping math.
- Coverage: bottom taskbar has live environment evidence; top/left/right/no-taskbar cases have deterministic geometry test evidence.
- Limitation honesty: physical multi-monitor and alternate-edge taskbar behavior are not claimed as fully live-tested.

Architecture self-check:

- R2 changes documentation only.
- Placement evidence stays in Desktop placement code/tests.
- No Domain/Application/Infrastructure behavior changed.
- No taskbar mutation, shell hook, Explorer injection, admin requirement, online service, telemetry, installer, signing, or HKLM write added.
- Generated artifacts remain ignored.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R2 start.
- `dotnet test tests\ChinaTrayCalendar.Desktop.Tests\ChinaTrayCalendar.Desktop.Tests.csproj --configuration Release --filter FullyQualifiedName~PopupPlacementCalculatorTests`: passed.
  - `6` tests passed.
  - Covered bottom, top, left, right, no-taskbar-edge, and invalid popup size cases.

Release artifact/hash:

- No new artifact generated in R2; R1 bundle remains the latest P10 script-generated baseline.

Commit / push:

- `70c4a1f compat: record display placement limits` pushed.

Risk / blocked:

- Physical multi-monitor spot check remains pending for hardware that exposes more than one display.
- Physical alternate taskbar-edge spot checks remain pending for a safe manual desktop QA pass.

### R3 - DPI And Scaling Spot Check

Status: PASS WITH LIMITATIONS

Scope:

- Reconfirmed current effective DPI is `96x96` and current display scale is `100%`.
- Reviewed DPI-aware placement code path.
- Reviewed popup/settings XAML layout constraints.
- Ran focused Desktop tests that construct/show popup and settings windows and exercise associated view models.
- Chose not to programmatically change display scale because it affects the user's desktop session and is not a safe automated executor mutation.

Non-100% DPI result:

- Not physically tested in this environment during R3.
- Current live environment remains `100%` scale.
- The risk remains a coverage limitation, not a known defect.

DPI-aware implementation evidence:

- `src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj` sets `ApplicationHighDpiMode` to `PerMonitorV2`.
- `PopupWindowPlacer` obtains WPF DPI through `VisualTreeHelper.GetDpi(window)`.
- `PopupWindowPlacer` converts fixed WPF popup dimensions from DIPs to physical pixels before placement and converts final pixel coordinates back to DIPs.
- `PopupPlacementCalculator` uses pixel-space screen bounds and working area, then clamps the popup within the working area.

Text/control fit evidence:

- `CalendarPopupWindow.xaml`
  - Fixed window size: `360x420`.
  - Header buttons: fixed `36x32`.
  - Day grid: `UniformGrid` with `7` columns and `6` rows.
  - Badges use one-character labels: `末`, `休`, `班`, `节`.
  - Navigation buttons use compact symbols/text: `‹`, `今`, `›`, `×`.
- `SettingsWindow.xaml`
  - Fixed window size: `360x260`.
  - Two-column layout with a `116` DIP label column and flexible input column.
  - Action buttons use `MinWidth=76`.
- Automated window tests construct or show these WPF windows under the current 100% DPI session.

Debug self-check:

- Minimal fixture: current machine gives 96 DPI live evidence only.
- Failure localization: DPI placement issues would localize to `ApplicationHighDpiMode`, `VisualTreeHelper.GetDpi`, pixel/DIP conversion, or working-area clamping.
- Coverage: live 100% DPI window construction and view-model tests pass; non-100% DPI remains explicitly not claimed.
- Text fit: key visible labels are short Chinese strings or compact symbols; window tests confirm current-DPI construction/show paths do not fail.

Architecture self-check:

- R3 changes documentation only.
- DPI evidence stays in Desktop WPF placement/window layer.
- No Domain/Application/Infrastructure behavior changed.
- No display-setting mutation, installer, signing, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, or admin requirement added.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R3 start.
- `dotnet test tests\ChinaTrayCalendar.Desktop.Tests\ChinaTrayCalendar.Desktop.Tests.csproj --configuration Release --filter "FullyQualifiedName~CalendarViewModelTests|FullyQualifiedName~SettingsViewModelTests"`: passed.
  - `12` tests passed.
  - Covered popup construction, popup shell properties, Escape hide path, month/title/header behavior, Settings window show path, settings save/error behavior, and startup checkbox view-model flow.

Release artifact/hash:

- No new artifact generated in R3; R1 bundle remains the latest P10 script-generated baseline.

Commit / push:

- `3c6d00e compat: record DPI scaling limits` pushed.

Risk / blocked:

- Physical non-100% DPI spot check remains pending for a safe manual desktop QA pass or alternate hardware/session.

### R4 - Windows Trust Prompt Documentation

Status: PASS

Scope:

- Reviewed README, troubleshooting, and release notes for unsigned portable distribution wording.
- Added user-facing notes about unknown-publisher, SmartScreen, and Microsoft Defender prompts.
- Clarified that SHA256 verifies zip integrity only and is not a code signature.
- Explicitly told users not to disable Windows Security, Microsoft Defender, or SmartScreen to run Dateview.
- Kept P10 within documentation and decision-material scope; no signing or installer implementation was added.

Documentation updates:

- `README.md`
  - Added `Windows Trust Notes`.
  - Advises trusted source, SHA256 comparison, keeping Windows security enabled, and not running unexpected files.
- `docs\TROUBLESHOOTING.md`
  - Added `Windows Security Prompts`.
  - Treats warnings as a trust decision and points back to source/hash verification.
- `docs\RELEASE_NOTES.md`
  - Added unsigned preview warning expectations and trusted-source/hash guidance.

Debug self-check:

- Minimal user workflow: receive unsigned portable zip, verify source/hash, see possible Windows trust prompt, decide whether to run.
- Failure localization: trust issues are documentation/distribution policy issues, not runtime calendar behavior.
- Coverage: unknown publisher, SmartScreen, Defender, SHA256 integrity vs signature, and unsafe security-disable guidance are addressed.
- Security posture: docs do not tell users to bypass, disable, or suppress Windows security features.

Architecture self-check:

- R4 changes documentation only.
- No Desktop/Application/Infrastructure/Domain runtime behavior changed.
- No certificate configuration, signing, installer, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, or admin requirement added.
- Generated artifacts remain ignored.

Validation:

- `git diff --check`: passed.
- Security prompt wording scan: only negative guidance was found for disabling/bypassing security features; no bypass instructions were added.

Release artifact/hash:

- No new artifact generated in R4; R1 bundle remains the latest P10 script-generated baseline.

Commit / push:

- `68bfbae compat: document unsigned trust prompts` pushed.

Risk / blocked:

- Dateview remains unsigned until a future PM/architecture signing decision is made.

### R5 - Release Trust And Signing ADR

Status: PASS

Scope:

- Added `docs\adr\0001-release-trust-and-signing.md`.
- Compared unsigned portable zip, code signing, MSIX, and traditional installer options.
- Recommended keeping unsigned portable zip for `0.1.0-preview` limited trusted distribution.
- Recommended code-signing the portable executable/artifacts as the likely next trust step before broader public distribution.
- Identified follow-up PM/budget/key ownership/release-process/distribution decisions.
- Explicitly avoided implementing signing, installer packaging, certificate configuration, or auto-update.

ADR summary:

- Option A: Continue unsigned portable zip.
- Option B: Code-sign portable executable and zip.
- Option C: MSIX package.
- Option D: Traditional installer such as WiX, Inno Setup, or NSIS.
- Recommendation: keep Option A for this preview; consider Option B before broader public release; defer installer technology until there is a clear need and a separate implementation phase.

Debug self-check:

- Minimal workflow: current users receive a portable zip; future trust work may add signing before any installer decision.
- Failure localization: release trust issues are distribution policy/release operations concerns, not calendar runtime behavior.
- Coverage: ADR covers unsigned, signed portable, MSIX, and installer options, plus decision ownership.
- Scope control: ADR does not select or implement a third-party installer/build package.

Architecture self-check:

- R5 changes documentation only.
- No runtime code, project references, signing configuration, installer scripts, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, or admin requirement added.
- ADR keeps release trust decisions separate from Desktop/Application/Infrastructure/Domain behavior.

Validation:

- `git diff --check`: passed.
- `git status --short --branch`: only `docs\P10_COMPATIBILITY_TRUST_VALIDATION.md` and `docs\adr\0001-release-trust-and-signing.md` changed.

Release artifact/hash:

- No new artifact generated in R5; R1 bundle remains the latest P10 script-generated baseline.

Commit / push:

- `e9ede8f compat: add release trust ADR` pushed.

Risk / blocked:

- Code signing and installer decisions remain pending for PM/architecture/budget approval.

### R6 - Buffer Compatibility Fix

Status: NOT USED

No MVP-required compatibility defect was found in R1-R5 that required a code fix.

### R7 - Buffer Docs/Test Fix

Status: NOT USED

No additional buffer documentation, test, ADR, or release script repair was required before final validation.

### R8 - Final P10 Validation

Status: PASS

Scope:

- Ran final validation.
- Ran final package publish.
- Regenerated the release bundle.
- Re-ran a source/test/script boundary scan for prohibited scope.
- Confirmed P10 validation and ADR are complete.
- Recorded final compatibility and trust readiness status.

Final compatibility conclusion:

- Current hardware live coverage: single monitor, bottom taskbar, 100% scale / 96 DPI.
- Physical multi-monitor and non-100% DPI remain explicit coverage limitations and are not claimed as fully tested.
- Deterministic placement tests cover bottom, top, left, right, no-taskbar-edge, and invalid-size geometry.
- Desktop window tests cover current-DPI popup/settings construction and settings show paths.
- No known compatibility defect remains from P10.

Final trust conclusion:

- Dateview remains an unsigned portable preview.
- User docs now explain unknown-publisher, SmartScreen, and Microsoft Defender warning expectations without telling users to disable or bypass Windows security.
- ADR `docs\adr\0001-release-trust-and-signing.md` recommends keeping unsigned portable zip for limited `0.1.0-preview` distribution and considering code signing before broader public release.
- P10 did not buy/configure certificates, add signing, implement an installer, or add auto-update.

Final artifact:

```text
D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64.zip
```

Final bundle evidence:

- Version: `0.1.0-preview`
- Zip bytes: `172421`
- Zip SHA256: `c74b1406755b94343ad7940e2bfd2666342fd540b3b8382e27b3eb8fb9d30dff`
- Release metadata git commit: `e9ede8f`

Debug self-check:

- Minimal workflow: compatibility status is explained by one available Windows 11 machine with one display, bottom taskbar, and 100% DPI, plus deterministic geometry/window tests.
- Failure localization: remaining gaps are physical hardware coverage gaps, not known placement, package path, tray lifecycle, or documentation failures.
- Coverage: single-screen bottom taskbar and 100% DPI have live evidence; multi-monitor, alternate physical taskbar edges, and non-100% DPI are explicitly retained as manual spot-check risks.
- State cleanup: R8 did not mutate user settings, startup registry, taskbar position, display scale, or Windows security settings.

Architecture self-check:

- R8 changes validation documentation only.
- P10 runtime code was not changed.
- No Domain/Application/Infrastructure/Desktop business behavior changed.
- No installer, signing implementation, auto-update, online service, telemetry, shell hook, Explorer injection, HKLM write, admin requirement, or generated artifact tracking added.
- ADR stays as decision material and does not introduce a third-party package.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R8 start.
- Boundary scan over `src`, `tests`, and `scripts` for prohibited scope keywords (`HKLM`, `SetWindowsHookEx`, `Shell_NotifyIcon`, telemetry, auto-update, installer/signing tools): no matches.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
  - Domain tests: `33` passed.
  - Application tests: `21` passed.
  - Infrastructure tests: `37` passed.
  - Desktop tests: `38` passed.
  - `dotnet format --verify-no-changes`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1`: passed.
- `git diff --check`: passed.

Release artifact/hash:

- `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`
- `c74b1406755b94343ad7940e2bfd2666342fd540b3b8382e27b3eb8fb9d30dff`

Commit / push:

- This R8 final validation section is committed by the final P10 completion commit and reported in the executor completion response.

Risk / blocked:

- Physical multi-monitor spot check remains pending for suitable hardware.
- Physical non-100% DPI spot check remains pending for a safe manual desktop QA pass or alternate hardware/session.
- The preview remains unsigned until PM/architecture/budget approval for code signing.

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

- [ ] Record physical multi-monitor result or limitation.
- [ ] Record taskbar-position result or limitation.
- [ ] Confirm placement tests provide geometry fallback evidence.

### R3 DPI And Scaling Spot Check

- [ ] Record non-100% DPI result or limitation.
- [ ] Confirm DPI-aware placement path and tests.
- [ ] Verify key text/control fit evidence.

### R4 Windows Trust Prompt Documentation

- [ ] Review unsigned portable app documentation.
- [ ] Update README/TROUBLESHOOTING/RELEASE_NOTES if needed.
- [ ] Avoid instructions that bypass Windows security features.

### R5 Release Trust And Signing ADR

- [ ] Add release trust/signing ADR.
- [ ] Compare unsigned portable, code signing, MSIX, and installer options.
- [ ] Clearly identify decisions requiring PM/budget/certificate approval.

### R8 Final P10 Acceptance

- [ ] Run final `Validate.cmd`.
- [ ] Run final `Package.cmd`.
- [ ] Run final `package-release.ps1`.
- [ ] Confirm P10 validation document is complete.
- [ ] Confirm ADR exists and does not implement signing/installer.
- [ ] Record final compatibility/trust readiness status.

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

- Pending R1 commit.

Risk / blocked:

- Physical multi-monitor hardware is not currently available.
- Non-100% DPI is not currently active and has not been changed in R1.

# P11 Limited Preview Release Validation

Date: 2026-06-22
Phase: P11 Limited Preview Release Readiness
Guide: `docs/P11_LIMITED_PREVIEW_RELEASE_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline: P10 compatibility and trust readiness accepted in `docs/P10_COMPATIBILITY_TRUST_VALIDATION.md`

## Scope

P11 prepares Dateview `0.1.0-preview` for a limited trusted preview handoff and feedback loop. It does not publish a public GitHub Release, upload artifacts, announce publicly, add code signing, configure certificates, implement an installer, add auto-update, add telemetry, add online APIs, inject into Explorer/taskbar, add Shell hooks, write HKLM, or require administrator privileges.

Expected P11 outputs:

- `docs/PREVIEW_RELEASE_HANDOFF.md`
- `docs/PREVIEW_FEEDBACK_GUIDE.md`
- This validation report
- Small links from existing user/release docs only where helpful

Generated release artifacts remain under ignored `artifacts\release` and are not committed.

## Prior Phase Evidence

P9 release distribution:

- Portable zip/folder is the accepted `0.1.0-preview` distribution shape.
- `scripts\package-release.ps1` generates a portable folder, zip, SHA256 sidecar, release metadata, and app-local manifest.
- P9 final portable smoke extracted the zip to `%TEMP%`, ran the app, verified second-instance exit code `0`, parsed bundled `2025.json` and `2026.json`, and cleaned up the temp folder/process.
- P9 residual risks: physical multi-monitor and non-100% DPI spot checks require suitable hardware; the bundle is unsigned.

P10 compatibility and trust:

- Live hardware baseline: Windows 11, one `DISPLAY1` monitor, bottom taskbar, `96x96` effective DPI, `100%` scale.
- Deterministic placement tests cover bottom, top, left, right, no-taskbar-edge, and invalid-size geometry.
- User docs explain unknown-publisher, SmartScreen, and Microsoft Defender warning expectations without telling users to disable Windows security.
- `docs\adr\0001-release-trust-and-signing.md` recommends unsigned portable zip for limited `0.1.0-preview`, with code signing to be considered before broader public release.

## Current Candidate

- Version: `0.1.0-preview`
- Bundle name: `Dateview-0.1.0-preview-win-x64`
- Initial P11 git commit: `ef81e3b`
- Release metadata git commit: `ef81e3b`
- Manifest git commit: `ef81e3b`
- Manifest file count: `13`

Candidate artifact:

```text
D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64.zip
```

Candidate SHA256:

```text
51afc3a300c7539917e80ace0c6ea2b2b09f7be4f47c8098614c63bed7bb6a73
```

Candidate sidecars:

```text
D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64.sha256.txt
D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64.release.json
D:\ToolProjects\Dateview\artifacts\release\Dateview-0.1.0-preview-win-x64\Dateview\release-manifest.json
```

## P11 Checklist

### R1 Release Candidate Baseline

- [x] Create this P11 validation document.
- [x] Record P9/P10 evidence.
- [x] Record current candidate artifact and hash.
- [x] Run `Validate.cmd`.
- [x] Run `Package.cmd`.
- [x] Run `package-release.ps1`.
- [x] Confirm generated artifacts remain ignored.

### R2 Preview Handoff Document

- [x] Create `docs/PREVIEW_RELEASE_HANDOFF.md`.
- [x] Include artifact, hash verification, unzip/run, exit, startup cleanup, uninstall, trust notes, and known limits.
- [x] Confirm no Windows security bypass instruction is present.

### R3 Feedback Intake And Triage

- [ ] Create `docs/PREVIEW_FEEDBACK_GUIDE.md`.
- [ ] Define issue categories, severities, required reproduction fields, and environment fields.
- [ ] Keep multi-monitor and non-100% DPI gaps explicit.

### R4 Existing Docs Linkage

- [ ] Link the handoff and feedback docs from existing docs where useful.
- [ ] Avoid duplicating large instruction blocks.
- [ ] Run `git diff --check`.

### R5 Final Portable Bundle Smoke

- [ ] Regenerate the portable bundle.
- [ ] Verify zip and SHA256 sidecar match.
- [ ] Extract and run from a temp non-repo path.
- [ ] Confirm first instance remains alive and second instance exits `0`.
- [ ] Confirm manifest and bundled holiday JSON parse.
- [ ] Clean up temp extraction and Dateview processes.

### R6 Buffer Repair

- [ ] Use only if a small docs/script/MVP-blocking release fix is needed.

### R7 Final P11 Validation

- [ ] Confirm `main...origin/main` is clean before final report.
- [ ] Run final `Validate.cmd`.
- [ ] Run final `Package.cmd`.
- [ ] Run final `package-release.ps1`.
- [ ] Run final `git diff --check`.
- [ ] Confirm P11 docs are complete.
- [ ] Confirm no generated artifacts are committed.
- [ ] Push all P11 commits.

## Round Log

### R1 - Release Candidate Baseline

Status: PASS

Scope:

- Created this P11 validation report.
- Recorded prior P9/P10 release distribution, compatibility, and trust evidence.
- Ran baseline validation, publish, and release-bundle generation.
- Recorded the current local candidate artifact and SHA256.
- Confirmed generated release artifacts remain ignored.

Debug self-check:

- Smallest preview workflow covered: generate the current portable candidate and identify the exact zip/hash testers would verify.
- Failure localization: R1 failures localize to repository validation, publish output, release-bundle generation, metadata/manifest creation, or ignored-artifact policy.
- Hardware coverage: R1 carries forward P10's honest single-monitor/100% DPI live baseline and does not claim multi-monitor or non-100% DPI coverage.
- State cleanup: R1 did not launch the app, mutate settings, write startup registry values, change display settings, or alter the user desktop.
- Generated artifacts: `git status --short --ignored=matching artifacts\release` reports ignored `artifacts/`; no generated artifact is staged.

Architecture self-check:

- R1 changes documentation only.
- No runtime code, layer dependency, installer, signing, certificate, auto-update, telemetry, online API, Explorer/taskbar injection, Shell hook, HKLM write, or admin requirement was added.
- Release artifacts stay under ignored `artifacts\release`.
- Existing unrelated changes were not present at R1 start.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R1 start.
- Initial HEAD: `ef81e3b docs: add P11 limited preview goal`.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd`: passed.
  - Domain tests: `33` passed.
  - Application tests: `21` passed.
  - Infrastructure tests: `37` passed.
  - Desktop tests: `38` passed.
  - `dotnet format --verify-no-changes`: passed.
- `C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd`: passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1`: passed.
  - Bundle name: `Dateview-0.1.0-preview-win-x64`
  - Zip bytes: `172415`
  - Zip SHA256: `51afc3a300c7539917e80ace0c6ea2b2b09f7be4f47c8098614c63bed7bb6a73`
  - Manifest file count: `13`
  - Metadata git commit: `ef81e3b`

Release artifact/hash:

- `artifacts\release\Dateview-0.1.0-preview-win-x64.zip`
- `51afc3a300c7539917e80ace0c6ea2b2b09f7be4f47c8098614c63bed7bb6a73`

Commit / push:

- This R1 section is committed by the R1 P11 baseline commit.

Risk / blocked:

- Physical multi-monitor and non-100% DPI live coverage remain preview validation gaps until suitable hardware/session coverage is available.
- The preview remains unsigned by design for limited trusted distribution.

### R2 - Preview Handoff Document

Status: PASS

Scope:

- Added `docs/PREVIEW_RELEASE_HANDOFF.md`.
- Documented the limited-preview file set: zip, `.sha256.txt`, and `.release.json`.
- Documented SHA256 verification, unzip/run, tray usage, exit, startup cleanup, portable uninstall, Windows trust notes, and known preview limits.
- Kept the handoff private-preview focused and avoided public release, store upload, or broad announcement instructions.

Debug self-check:

- Smallest preview workflow covered: a trusted tester receives the zip plus sidecars, verifies the hash, extracts the portable folder, runs Dateview, exits, and can uninstall/clean startup state.
- Failure localization: tester failures can be localized to source/hash mismatch, extraction, tray visibility, run/exit behavior, startup cleanup, Windows trust prompts, or known hardware coverage gaps.
- Hardware coverage: the handoff states the live validated environment is single-display, bottom-taskbar, `100%` scale / `96 DPI`, and keeps multi-monitor/non-100% DPI as preview feedback targets.
- State cleanup: the handoff tells testers to turn off `Start with Windows` before moving/deleting the app and identifies the current-user Run value for stale-entry cleanup.
- Generated artifacts: R2 did not generate or stage release artifacts.

Architecture self-check:

- R2 changes documentation only.
- No runtime code, installer, signing, certificate, auto-update, telemetry, online API, Explorer/taskbar injection, Shell hook, HKLM write, or admin requirement was added.
- The handoff preserves current-user portable distribution and offline bundled holiday data.
- Windows trust guidance tells testers to keep Windows Security, Microsoft Defender, and SmartScreen enabled.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: clean at R2 start.
- `git diff --check`: passed.
- Security-bypass wording review: the handoff does not instruct testers to disable SmartScreen, Defender, or Windows Security; it explicitly says to keep them enabled.

Release artifact/hash:

- No new artifact generated in R2; R1 remains the latest P11 script-generated candidate evidence.

Commit / push:

- This R2 section is committed by the R2 P11 handoff commit.

Risk / blocked:

- None for R2.

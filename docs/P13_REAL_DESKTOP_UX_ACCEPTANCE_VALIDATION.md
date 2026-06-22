# P13 Real Desktop UX Acceptance Validation

Date: 2026-06-22
Phase: P13 Real Desktop UX Acceptance
Guide: `docs/P13_REAL_DESKTOP_UX_ACCEPTANCE_GOAL_MODE_EXECUTION_GUIDE.md`
Baseline: P12 Customer UX Polish accepted in `docs/P12_CUSTOMER_UX_POLISH_VALIDATION.md`

## Scope

P13 is a validation, acceptance, and feedback-triage phase for the P12 tray icon, popup animation, and tray-click toggle changes.

Default expectation: validation and documentation only. Minimal Desktop-only fixes are allowed only if a P12 blocker is found during real desktop acceptance.

P13 does not add calendar features, redesign animation/icon/UI, implement signing or installers, publish or upload artifacts, add auto-update, add online APIs, add telemetry, inject into Explorer/taskbar, add global hooks, add Shell hooks, write HKLM, or require administrator privileges.

Pre-existing untracked root helper scripts are preserved and not staged:

- `BuildLatest.cmd`
- `StartPreview.cmd`

## Carried-Forward P12 Evidence

- P12 final commit: `2cf8fdd`
- P12 validation: `docs\P12_CUSTOMER_UX_POLISH_VALIDATION.md`
- Tray icon uses `DateviewTrayIconProvider` and embedded `DateviewIcon.ico`, not `SystemIcons.Application`.
- Packaged executable associated icon: `32x32`.
- Open animation: `170 ms` opacity plus content scale/translate ease-out.
- Close animation: `140 ms` opacity plus content scale/translate ease-in.
- No `Window.RenderTransform` animation is used.
- Tray visible-click close, view-model close, Escape dismiss, and deactivation dismiss route through animated close.
- Desktop automated tests at P12 final: Domain `33`, Application `21`, Infrastructure `37`, Desktop `46`.
- P12 residual risk: subjective animation feel and tray overflow behavior still need real desktop/human confirmation.

## Acceptance Matrix

| Item | Status | Evidence / Notes |
| --- | --- | --- |
| Tray icon appears and is Dateview-specific in notification area | PENDING | R2 real desktop smoke |
| Tray icon appears acceptably in tray overflow | PENDING | R2 real desktop smoke |
| Right-click menu remains accessible | PENDING | R2 real desktop smoke |
| Left-click opens popup | PENDING | R2 real desktop smoke |
| Left-click again closes popup | PENDING | R2 real desktop smoke |
| Reopen after close works | PENDING | R2 real desktop smoke |
| Escape closes popup | PENDING | R2 real desktop smoke |
| Outside/deactivation closes popup | PENDING | R2 real desktop smoke |
| Open animation feels calm/soft with no obvious bounce/shake | PENDING | R2 real desktop smoke |
| Close animation feels calm/soft | PENDING | R2 real desktop smoke |
| Second instance exits cleanly | PENDING | R3 package/process smoke |
| No Dateview process remains after exit/cleanup | PENDING | R2/R3 smoke |
| Settings/startup state is not changed or is restored | PENDING | R2/R3 smoke |

## Environment

P13 environment facts will be recorded in R2/R3 from the current Windows desktop session and release artifact.

## P13 Checklist

### R1 Checklist And Baseline

- [x] Create this P13 validation document.
- [x] Create `docs/PREVIEW_UX_ACCEPTANCE_CHECKLIST.md`.
- [x] Carry forward P12 automated evidence and residual risks.
- [x] Run `git diff --check`.

### R2 Real Desktop UX Smoke

- [ ] Run the app in the real Windows desktop session where feasible.
- [ ] Record tray icon, tray overflow, right-click menu, open/close, Escape, outside/deactivation, and animation feel evidence.
- [ ] Record honest `NOT AVAILABLE` / `LIMITED` where direct inspection cannot be performed reliably.
- [ ] Clean up any Dateview process and leave startup/settings state unchanged.

### R3 Package And Feedback Reconciliation

- [ ] Run package/release smoke.
- [ ] Confirm zip/hash/manifest/basic process smoke.
- [ ] Map findings to `fix-now`, `document`, `defer`, or `needs hardware reproduction`.

### R4 Buffer Repair

- [ ] Use only if real desktop acceptance exposes a blocker or small checklist/docs clarification.

### R5 Final Acceptance

- [ ] Run final `Validate.cmd`.
- [ ] Run final `Package.cmd`.
- [ ] Run final `package-release.ps1`.
- [ ] Run final `git diff --check`.
- [ ] Run final boundary scan.
- [ ] Complete the final acceptance matrix.
- [ ] Push all P13 commits.

## Round Log

### R1 - Checklist And Baseline

Status: PASS

Scope:

- Created this P13 validation report.
- Created `docs\PREVIEW_UX_ACCEPTANCE_CHECKLIST.md`.
- Carried forward the P12 icon, animation, toggle, automated test, package, and residual-risk evidence.
- Left implementation code unchanged.

Debug self-check:

- Real user workflow covered: a human preview tester can follow a concise checklist for tray icon, tray overflow, open/close toggle, Escape/outside close, subjective animation feel, and cleanup.
- Failure localization: the checklist separates icon visibility, tray menu, popup placement, animation feel, click toggle state, Escape/deactivation, package startup, and cleanup.
- Evidence type: R1 is documentation/checklist evidence only; real desktop observation starts in R2.
- State cleanup: R1 did not launch Dateview, mutate settings, write startup registry values, create temp extraction folders, or change desktop/taskbar state.
- Subjective observations: R1 does not claim subjective pass/fail; it defines how to record them.

Architecture self-check:

- R1 changes documentation only.
- No Desktop runtime code changed in R1.
- Domain/Application/Infrastructure remain unchanged.
- No Explorer/taskbar injection, global hook, Shell hook, admin requirement, HKLM write, online dependency, telemetry, installer/signing work, public release, or upload was added.
- Pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd` remain untouched and unstaged.

Validation:

- `C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd`: tracked tree clean at R1 start; only pre-existing untracked `BuildLatest.cmd` and `StartPreview.cmd`.
- `git diff --check`: passed.

Commit / push:

- This R1 section is committed by the R1 P13 checklist/baseline commit.

Risk / blocked:

- None for R1.

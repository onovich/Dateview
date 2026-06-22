# Dateview P11 Limited Preview Release Readiness Goal Mode Execution Guide

Date: 2026-06-22

Status: P10 compatibility and trust readiness has passed. This guide is for P11: preparing a limited trusted preview release handoff and feedback loop without changing Dateview's distribution architecture.

## 0. Direct Goal Prompt For The Executor

```text
You are the Dateview P11 executor/programmer/QA owner. Work in D:\ToolProjects\Dateview.

Goal: prepare Dateview 0.1.0-preview for a limited trusted preview handoff. The output should make it clear what artifact to share, how testers should verify/run it, what environments are covered or not covered, and how feedback should be reported and triaged.

Mandatory reading:
- AGENTS.md
- docs/P10_COMPATIBILITY_TRUST_VALIDATION.md
- docs/adr/0001-release-trust-and-signing.md
- docs/P9_RELEASE_DISTRIBUTION_VALIDATION.md
- docs/RELEASE_NOTES.md
- docs/TROUBLESHOOTING.md
- README.md
- docs/P11_LIMITED_PREVIEW_RELEASE_GOAL_MODE_EXECUTION_GUIDE.md

Execution rules:
- Start every round with git status and preserve unrelated changes.
- Every round must include Debug self-check, architecture self-check, validation commands and results, commit hash, and push result.
- Validation must pass before commit; push must succeed before moving to the next round.
- Do not create a public GitHub Release, publish to a store, announce publicly, or upload artifacts anywhere unless the planner/user gives a separate explicit instruction.
- Do not implement code signing, certificate purchase/configuration, installer packaging, MSIX, auto-update, telemetry, online API, Explorer/taskbar injection, Shell hooks, HKLM writes, or administrator requirements.
- Generated release artifacts under artifacts\release remain local/ignored and must not be committed.
```

## 1. Phase Intent

P11 is a release-operations and feedback-readiness phase. It turns the P9/P10 portable preview evidence into a small tester handoff package and a repeatable feedback loop.

P11 should answer:

- Which local artifact is the current limited preview candidate?
- Which hash and metadata should testers compare?
- What exact run/uninstall/startup cleanup steps should testers follow?
- Which Windows/display/DPI environments are validated, limited, or still unknown?
- How should testers report problems so the next architecture/planning pass can triage them?

P11 is not a feature phase and not a broad-public-release phase.

## 2. Required Outputs

P11 must produce or update:

- `docs/P11_LIMITED_PREVIEW_RELEASE_VALIDATION.md`
  - Final P11 validation report, round log, artifact evidence, and residual risks.
- `docs/PREVIEW_RELEASE_HANDOFF.md`
  - Short tester-facing handoff: artifact name, hash check, run path, exit/uninstall, startup toggle cleanup, trust notes, known limits.
- `docs/PREVIEW_FEEDBACK_GUIDE.md`
  - Feedback intake guide: what information to collect, severity triage, reproduction details, environment matrix, screenshot/log expectations if any.
- Existing docs only if needed:
  - `README.md`
  - `docs/RELEASE_NOTES.md`
  - `docs/TROUBLESHOOTING.md`

Optional if useful and kept lightweight:

- `.github/ISSUE_TEMPLATE/preview-feedback.md`

Only add the optional issue template if it clearly helps the preview feedback workflow and does not conflict with the repo's current structure.

## 3. Explicit Non-Scope

Do not do any of the following in P11:

- No new calendar features.
- No UI redesign.
- No code signing implementation.
- No certificate purchase, key storage setup, or signing service configuration.
- No installer, MSIX, WiX, Inno Setup, NSIS, or auto-update.
- No public GitHub Release or external upload.
- No online holiday API, account sync, telemetry, or analytics.
- No Explorer injection, Shell hook, taskbar injection, global hook, HKLM write, or admin requirement.
- No generated artifact commit.

If preview distribution cannot proceed without one of these items, report BLOCKED to the planner instead of implementing it.

## 4. Architecture Boundaries

Expected P11 changes are documentation, release checklist, and optional repository issue-template material.

Runtime code changes are allowed only for a small MVP-blocking release defect found during smoke validation. If a runtime fix is needed:

- Keep it in the correct layer.
- Add or update tests.
- Run full validation.
- Document why the fix belongs in P11.

Default expectation: P11 is docs plus local validation only.

## 5. Fixed Workflow Per Round

Start each round:

```powershell
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd
```

For documentation-only rounds, at minimum run:

```powershell
git diff --check
```

For final validation and any runtime/script change, run:

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1
```

If validating the portable bundle, smoke the generated zip from a temporary non-repo extraction path:

- Verify zip exists.
- Verify `.sha256.txt` matches `Get-FileHash`.
- Extract to a temp folder.
- Run `Dateview\ChinaTrayCalendar.Desktop.exe`.
- Confirm first instance remains alive.
- Confirm second instance exits with code 0.
- Confirm `release-manifest.json` exists and has the expected file count.
- Confirm bundled `assets\holidays\cn\2025.json` and `2026.json` can be parsed as UTF-8 JSON.
- Stop the process and remove the temp extraction folder.

Every round must commit and push only phase-relevant files after validation passes.

## 6. Debug Self-Check

Each round must answer:

- What is the smallest preview user workflow covered by this round?
- If it fails, can the failure be localized to artifact generation, hash verification, unzip/run, startup cleanup, Windows trust docs, feedback intake, or triage process?
- Did the round avoid claiming hardware coverage that is not physically available?
- Did any smoke test mutate settings, startup registry, or user desktop state, and was it restored?
- Are generated artifacts still ignored and uncommitted?

## 7. Architecture Self-Check

Each round must answer:

- Did this round stay within documentation/release-ops boundaries?
- If code changed, does it preserve the strict layer dependency direction?
- Did it avoid installer/signing/auto-update/public-release scope creep?
- Did it preserve the no-admin, HKCU-only, offline-first, no-telemetry posture?
- Did it preserve unrelated user or executor changes?

## 8. Round Budget

Estimated total: 7 conversation rounds.

| Round | Type | Goal |
| --- | --- | --- |
| R1 | Main | Establish P11 validation document and current release candidate baseline. |
| R2 | Main | Create preview handoff document for trusted testers. |
| R3 | Main | Create feedback intake and triage guide. |
| R4 | Main | Review/update README, release notes, and troubleshooting links only where needed. |
| R5 | Main | Perform final portable bundle smoke from a temp extraction path and record evidence. |
| R6 | Buffer | Repair docs, tests, script, or a small MVP-blocking release defect if found. |
| R7 | Final | Final P11 acceptance validation, clean status, push, and completion report. |

R6 may be unused. Do not spend it on new scope.

## 9. Round Plan

### R1 - Release Candidate Baseline

Goals:

- Create `docs/P11_LIMITED_PREVIEW_RELEASE_VALIDATION.md`.
- Record previous phase evidence from P9/P10.
- Record current git commit, version, package shape, and local artifact candidate if generated.
- Run `Validate.cmd`, `Package.cmd`, and `package-release.ps1`.
- Confirm no generated artifacts are staged.

PASS:

- P11 validation document exists.
- Candidate artifact path and hash are recorded.
- Validation and package generation pass.

### R2 - Preview Handoff Document

Goals:

- Create `docs/PREVIEW_RELEASE_HANDOFF.md`.
- Keep it tester-facing and concise.
- Include artifact name, hash verification, unzip/run, exit, startup toggle cleanup, uninstall, trust notes, and known limits.
- Make clear SHA256 is integrity only, not code signing.
- Do not tell testers to disable Windows security.

PASS:

- A tester can follow the handoff without reading the whole repo.
- No security-bypass instruction is present.

### R3 - Feedback Intake And Triage

Goals:

- Create `docs/PREVIEW_FEEDBACK_GUIDE.md`.
- Define issue categories such as startup, tray visibility, popup placement, DPI/display, holiday data, settings/startup, trust prompt, package/hash, crash/hang, docs confusion.
- Define severity levels and required reproduction fields.
- Include environment fields: Windows version, display count, scaling, taskbar edge, install path, zip hash, Dateview version, startup enabled or not.

PASS:

- Feedback can be triaged into fix-now, document, defer, or needs hardware reproduction.
- Multi-monitor and non-100% DPI gaps remain explicit and honest.

### R4 - Existing Docs Linkage

Goals:

- Update `README.md`, `docs/RELEASE_NOTES.md`, or `docs/TROUBLESHOOTING.md` only where needed to point to the preview handoff/feedback guide.
- Avoid duplicating large blocks of instructions.
- Keep release notes truthful about unsigned preview and hardware coverage limits.

PASS:

- Existing docs point users/testers to the new handoff and feedback material.
- `git diff --check` passes.

### R5 - Final Portable Bundle Smoke

Goals:

- Run final package generation.
- Verify zip/hash sidecar match.
- Extract and run from a temp non-repo path.
- Confirm single-instance behavior.
- Confirm manifest and holiday JSON load.
- Clean up temp extraction and Dateview processes.

PASS:

- Smoke evidence is recorded in P11 validation.
- No leftover Dateview process remains.

### R6 - Buffer Repair

Use only for:

- Small documentation fix.
- Small release script robustness fix.
- Small MVP-blocking release defect discovered in R5.
- Test coverage for a needed fix.

Do not use for new feature work or broad release strategy changes.

### R7 - Final P11 Validation

Must complete:

- `main...origin/main` clean before final report.
- `Validate.cmd` passes.
- `Package.cmd` passes.
- `package-release.ps1` passes.
- `git diff --check` passes.
- P11 validation document complete.
- Handoff and feedback guides complete.
- No generated artifacts committed.
- All P11 commits pushed.

## 10. PASS Criteria

P11 is accepted only if:

- `docs/P11_LIMITED_PREVIEW_RELEASE_VALIDATION.md` records all used rounds and final evidence.
- `docs/PREVIEW_RELEASE_HANDOFF.md` is clear enough for limited trusted testers.
- `docs/PREVIEW_FEEDBACK_GUIDE.md` is clear enough for triage.
- Current release artifact can be generated and smoke-tested locally.
- Hash sidecar matches the generated zip.
- Source/test/script scans or review show no installer/signing/auto-update/telemetry/shell-integration scope creep.
- Existing docs link to the new preview material where appropriate.
- Validation/package commands pass.
- `main...origin/main` is clean after final push.

## 11. Completion Report Template

```text
Phase: P11 Limited Preview Release Readiness
Estimated rounds:
Actual rounds:
Completed:
Not completed by design:
Validation:
Final artifact:
Final artifact SHA256:
Handoff docs:
Feedback docs:
Pushed commits:
Consumed buffer:
Architecture deviations:
Residual risks:
Recommended next phase:
```

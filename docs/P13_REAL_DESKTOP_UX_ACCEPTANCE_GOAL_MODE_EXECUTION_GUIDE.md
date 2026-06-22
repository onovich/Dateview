# Dateview P13 Real Desktop UX Acceptance Goal Mode Execution Guide

Date: 2026-06-22

Status: Created after P12 Customer UX Polish passed automated validation. This phase closes the remaining human/real-desktop acceptance gap for tray icon, tray overflow, and subjective popup motion.

## 0. Direct Goal Prompt For The Executor

```text
You are the Dateview P13 executor/QA owner. Work in D:\ToolProjects\Dateview.

Goal: perform or prepare real-desktop UX acceptance for the P12 tray icon and popup animation/toggle changes. This is a validation and feedback-triage phase, not a feature phase.

Mandatory reading:
- AGENTS.md
- docs/P13_REAL_DESKTOP_UX_ACCEPTANCE_GOAL_MODE_EXECUTION_GUIDE.md
- docs/P12_CUSTOMER_UX_POLISH_VALIDATION.md
- docs/P12_CUSTOMER_UX_POLISH_GOAL_MODE_EXECUTION_GUIDE.md
- docs/PREVIEW_RELEASE_HANDOFF.md
- docs/PREVIEW_FEEDBACK_GUIDE.md

Execution rules:
- Start every round with git status and preserve unrelated changes. Do not stage unrelated root helper scripts unless explicitly assigned.
- Every round must include Debug self-check, architecture self-check, validation commands and results, commit hash, and push result.
- Validation must pass before commit; push must succeed before moving to the next round.
- Prefer real Windows desktop/manual tray smoke where available.
- If interactive tray inspection is unavailable, record the limitation honestly and produce the strongest reproducible checklist/handoff evidence instead.
- Do not implement new product features unless a P12 regression blocks acceptance and a minimal Desktop-only fix is clearly required.
- Do not introduce Explorer/taskbar injection, global hooks, Shell hooks, admin requirements, HKLM writes, online dependencies, telemetry, installer/signing work, public release, or upload.
```

## 1. Phase Intent

P12 fixed the customer feedback in code and automated tests. Its residual risks are human-observed:

- Does the tray icon actually look recognizable in the normal tray and tray overflow?
- Does the open animation feel soft rather than shaky?
- Does left-click open/left-click close feel obvious and responsive?
- Do Escape and outside/deactivation close paths still feel natural?

P13 turns those subjective and environment-dependent checks into an auditable real-desktop acceptance record and feedback triage outcome.

## 2. Required Outputs

P13 must produce or update:

- `docs/P13_REAL_DESKTOP_UX_ACCEPTANCE_VALIDATION.md`
  - Round log, real desktop evidence or limitation, final acceptance matrix, residual risks.
- `docs/PREVIEW_UX_ACCEPTANCE_CHECKLIST.md`
  - A concise checklist a human tester can use to accept or reject the P12 UX changes.
- Existing feedback/handoff docs only if a small link or clarification is needed.

Optional if a real desktop tray session is available:

- Local screenshots or short recordings may be captured for evidence, but do not commit large media unless explicitly approved. Prefer documenting paths and observations in the validation report.

## 3. Explicit Non-Scope

Do not do any of the following in P13:

- No new calendar features.
- No animation redesign beyond a minimal P12 regression fix.
- No new icon design unless the current icon is proven unusable for acceptance.
- No code signing, installer, MSIX, auto-update, online API, telemetry, public release, store upload, or announcement.
- No Explorer/taskbar injection, global hooks, Shell hooks, admin requirements, or HKLM writes.
- No Domain/Application business logic changes.
- No generated artifact commit.

If acceptance requires subjective product taste decisions beyond "passes/fails the P12 feedback", report the decision point to the planner instead of inventing a new design direction.

## 4. Architecture Boundaries

Default expectation: P13 is validation and documentation only.

If a blocking P12 regression is found and a fix is necessary:

- Keep it in `ChinaTrayCalendar.Desktop` and Desktop tests.
- Preserve Domain/Application/Infrastructure boundaries.
- Add focused tests.
- Run full validation/package gates before commit.

## 5. Acceptance Matrix

P13 should classify each item as `PASS`, `FAIL`, `LIMITED`, or `NOT AVAILABLE`:

- Tray icon appears and is Dateview-specific in the notification area.
- Tray icon appears acceptably in tray overflow.
- Right-click menu remains accessible.
- Left-click opens popup.
- Left-click again closes popup.
- Reopen after close works.
- Escape closes popup.
- Outside/deactivation closes popup if the path is testable.
- Open animation feels calm/soft, with no obvious bounce/shake.
- Close animation feels calm/soft.
- Second instance exits cleanly.
- No Dateview process remains after exit.
- Settings/startup state is not changed unless explicitly tested and restored.

Environment fields:

- Windows edition/version/build.
- Display count and resolutions.
- Scale percentage/DPI.
- Taskbar edge and autohide state.
- Tray icon normal area vs overflow.
- Artifact path/hash tested.

## 6. Fixed Workflow Per Round

Start each round:

```powershell
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd
```

For docs-only rounds:

```powershell
git diff --check
```

For final validation:

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1
git diff --check
```

Manual smoke options:

- If root helper scripts are intentionally present and approved for local use, `BuildLatest.cmd` and `StartPreview.cmd` may be used for manual testing, but do not stage them unless assigned.
- Otherwise use `Package.cmd`, `package-release.ps1`, and the generated `artifacts\release\...\Dateview\ChinaTrayCalendar.Desktop.exe`.

Cleanup:

- Exit Dateview through the tray menu when possible.
- Stop any remaining `ChinaTrayCalendar.Desktop` process after smoke.
- Do not leave startup registry entries changed.

## 7. Debug Self-Check

Each round must answer:

- What real user workflow or checklist item did this round cover?
- If it fails, can the issue be localized to icon visibility, tray menu, popup placement, animation feel, click toggle state, Escape/deactivation, package startup, or documentation?
- Is the evidence real desktop observation, automated smoke, or documented limitation?
- Did testing leave any process, startup entry, temp directory, or settings mutation behind?
- Are subjective observations written as observations, not overclaimed facts?

## 8. Architecture Self-Check

Each round must answer:

- Did the round stay in validation/docs unless a blocking P12 regression required a minimal fix?
- If code changed, did it stay in Desktop and keep tests focused?
- Did Domain/Application/Infrastructure remain unchanged unless explicitly justified?
- Did the round avoid shell hooks, injection, admin, online, telemetry, installer/signing, and public release scope?
- Were unrelated files and generated artifacts left alone?

## 9. Round Budget

Estimated total: 5 conversation rounds.

| Round | Type | Goal |
| --- | --- | --- |
| R1 | Main | Create P13 validation report and UX acceptance checklist. |
| R2 | Main | Run or document real desktop tray/icon/open-close UX smoke. |
| R3 | Main | Run package/release smoke and reconcile findings with feedback guide. |
| R4 | Buffer | Minimal Desktop-only repair if real desktop acceptance exposes a blocker. |
| R5 | Final | Final validation, acceptance matrix, clean status, push, and completion report. |

R4 may be unused. Do not spend it on new feature work.

## 10. Round Plan

### R1 - Checklist And Baseline

Goals:

- Create `docs/P13_REAL_DESKTOP_UX_ACCEPTANCE_VALIDATION.md`.
- Create `docs/PREVIEW_UX_ACCEPTANCE_CHECKLIST.md`.
- Carry forward P12 automated evidence and residual risks.
- Run `git diff --check`.

PASS:

- A human tester can follow the checklist without reading implementation code.

### R2 - Real Desktop UX Smoke

Goals:

- Build or use the current preview artifact.
- Run the app in a real Windows desktop session if available.
- Fill the acceptance matrix for tray icon, tray overflow, click-open, click-close, Escape, outside/deactivation, and subjective animation feel.
- If real tray inspection is unavailable, record `NOT AVAILABLE` and why.

PASS:

- Real observation or limitation is recorded honestly.

### R3 - Package And Feedback Reconciliation

Goals:

- Run final package generation or use freshly generated artifact.
- Confirm zip/hash/manifest/basic process smoke still pass.
- Map findings to `fix-now`, `document`, `defer`, or `needs hardware reproduction`.

PASS:

- No untriaged acceptance findings remain.

### R4 - Buffer Repair

Use only for:

- A blocker found during real desktop UX acceptance.
- Small Desktop-only fix with focused tests.
- Small checklist/documentation clarification.

### R5 - Final Acceptance

Must complete:

- `Validate.cmd` passes.
- `Package.cmd` passes.
- `package-release.ps1` passes.
- `git diff --check` passes.
- Boundary scan shows no prohibited scope.
- P13 validation report has final matrix.
- All P13 commits pushed.
- `main...origin/main` is clean except explicitly preserved unrelated untracked helper scripts if still present.

## 11. PASS Criteria

P13 is accepted only if:

- UX acceptance checklist exists and is usable.
- P13 validation report records the real desktop evidence or honest limitation.
- No P12 blocker remains untriaged.
- Package/release smoke and full validation pass.
- No forbidden shell integration, hook, admin, online, telemetry, installer/signing, public release, or upload scope was introduced.
- Generated artifacts are not committed.

## 12. Completion Report Template

```text
Phase: P13 Real Desktop UX Acceptance
Estimated rounds:
Actual rounds:
Completed:
Not completed by design:
Validation:
Real desktop UX evidence:
Acceptance matrix:
Package/artifact evidence:
Pushed commits:
Consumed buffer:
Architecture deviations:
Residual risks:
Recommended next phase:
```

# Dateview P12 Customer UX Polish Goal Mode Execution Guide

Date: 2026-06-22

Status: Created from customer-support user-experience feedback after P11 limited preview readiness. This phase is a narrow Desktop/UI polish and interaction fix phase.

## 0. Direct Goal Prompt For The Executor

```text
You are the Dateview P12 executor/programmer. Work in D:\ToolProjects\Dateview.

Goal: implement the customer-support UX feedback for Dateview's tray/app icon and popup interaction polish.

Customer feedback:
1. The current product appears to have no recognizable icon. Complete the tray/application icon experience.
2. The calendar popup animation currently feels like it drops down and bounces back upward, which users describe as "shaking". Replace it with a natural, soft open animation inspired by Apple-style popover motion.
3. After the calendar popup opens from the tray icon, clicking the tray icon again should naturally collapse it. The close/collapse animation should also feel soft and natural.

Mandatory reading:
- AGENTS.md
- docs/P12_CUSTOMER_UX_POLISH_GOAL_MODE_EXECUTION_GUIDE.md
- docs/P11_LIMITED_PREVIEW_RELEASE_VALIDATION.md
- docs/PREVIEW_FEEDBACK_GUIDE.md
- src/ChinaTrayCalendar.Desktop/App.xaml.cs
- src/ChinaTrayCalendar.Desktop/Tray/TrayIconService.cs
- src/ChinaTrayCalendar.Desktop/PopupPlacement/PopupAnimationService.cs
- tests/ChinaTrayCalendar.Desktop.Tests/TrayIconServiceTests.cs
- tests/ChinaTrayCalendar.Desktop.Tests/PopupAnimationServiceTests.cs

Execution rules:
- Start every round with git status and preserve unrelated changes. In particular, do not accidentally stage unrelated root helper scripts unless they are part of your own assigned change.
- Every round must include Debug self-check, architecture self-check, validation commands and results, commit hash, and push result.
- Validation must pass before commit; push must succeed before moving to the next round.
- Keep the work primarily in ChinaTrayCalendar.Desktop and Desktop tests.
- Do not introduce Explorer/taskbar injection, global hooks, Shell hooks, admin requirements, online dependencies, telemetry, installer/signing work, or HKLM writes.
- Do not move popup visibility state into Domain/Application. This is Desktop shell interaction state.
```

## 1. Phase Intent

P12 fixes concrete preview feedback about first impression and tray popup feel:

- Dateview should have a recognizable product icon in the executable/window/tray experience.
- The popup should open with a calm, soft motion rather than a "drop and bounce" or "shake" feel.
- The same tray click should behave as a clear toggle: click to open, click again to close with a soft closing animation.

This phase should produce a better manual preview feel without changing the product scope or distribution model.

## 2. Required Outputs

P12 must produce or update:

- Desktop runtime changes for tray/application icon use.
- Desktop runtime changes for popup open and close animation.
- Desktop runtime changes, if needed, so left-click tray toggling is reliable while opening/closing animations are in progress.
- Focused Desktop tests for icon use, animation behavior, and toggle/close flow where feasible.
- `docs/P12_CUSTOMER_UX_POLISH_VALIDATION.md` with round log, manual smoke notes, validation evidence, residual risks, and final PASS evidence.
- Existing preview docs only if user-facing behavior changes need a short note.

## 3. Explicit Non-Scope

Do not do any of the following:

- No new calendar features.
- No redesign of the whole calendar UI.
- No new tray technology beyond the existing `System.Windows.Forms.NotifyIcon` unless a blocking limitation is documented first.
- No Explorer injection, taskbar injection, global hooks, Shell hooks, admin requirements, HKLM writes, online APIs, telemetry, installer, signing, MSIX, or auto-update.
- No Domain/Application holiday/calendar/settings logic changes unless a test proves they are necessary, which is not expected.
- No public release/upload.

## 4. Architecture Boundaries

Expected source-of-truth:

- Icon loading and tray presentation: `ChinaTrayCalendar.Desktop.Tray`.
- Popup animation: `ChinaTrayCalendar.Desktop.PopupPlacement`.
- Popup open/close orchestration: `ChinaTrayCalendar.Desktop.App`.
- Calendar content and date logic: unchanged in Application/Domain.

Prefer dependency injection or test seams already present in Desktop tests. Keep code-behind minimal and do not move UI shell state into Domain/Application.

## 5. UX Acceptance Direction

Icon:

- The tray icon must use the Dateview icon asset, not `SystemIcons.Application`.
- The WPF executable/window application icon path must remain valid.
- Published output should include or embed the icon as appropriate for the current project setup.
- If the existing `assets/icons/dateview.ico` is unsuitable or corrupt, repair/replace it with a simple Dateview-appropriate `.ico` asset and document the choice.

Open animation:

- Avoid overshoot, bounce, or reverse-direction effects that can read as vibration.
- Prefer a short fade plus subtle scale/translate easing.
- Keep duration restrained, roughly 140-220 ms.
- Do not set `Window.RenderTransform`; previous WPF instability was avoided by animating content.
- The animation should start near the final position and settle gently.

Close animation:

- Left-clicking the tray icon while popup is visible should start a soft close animation and then hide the window.
- Escape, close command, and deactivation/outside-click hide paths should also use the same close behavior when feasible.
- During closing animation, repeated tray clicks should not leave the popup stuck, double-hidden, or visually inconsistent.
- If a quick click arrives while closing, choose a deterministic behavior and test it. Recommended behavior: finish/cancel the close and reopen from the latest tray click point.

Manual feel target:

- The popup should feel like a lightweight popover attached to the tray area: calm, responsive, and not jarring.

## 6. Fixed Workflow Per Round

Start each round:

```powershell
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd
```

For source changes, run at minimum:

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

For final validation, also run:

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\package-release.ps1
git diff --check
```

Manual smoke should be recorded in the validation document:

- Build/start the app.
- Confirm tray icon is Dateview-specific.
- Left-click tray icon opens popup.
- Left-click tray icon again closes popup.
- Reopen after close.
- Escape closes popup.
- Outside/deactivation closes popup if that path existed before.
- Observe open/close motion and confirm no obvious bounce/shake.

If manual UI inspection is impossible in the executor environment, record the limitation and provide the strongest automated evidence instead.

## 7. Debug Self-Check

Each round must answer:

- What is the smallest user-visible workflow covered by this round?
- If it fails, can the failure be localized to icon loading, tray adapter/service, popup animation service, App popup orchestration, or WPF window behavior?
- Are open, close, repeated click, Escape, and deactivation paths covered where relevant?
- Does the fix avoid reintroducing `Window.RenderTransform` animation?
- Did the smoke test leave any Dateview process running or startup/settings state changed?

## 8. Architecture Self-Check

Each round must answer:

- Did the change stay in Desktop/UI unless a clear boundary reason exists?
- Did Domain/Application remain free of WPF/WinForms/icon/animation/tray state?
- Did the change preserve current-user, offline-first, no-admin, no-hook behavior?
- Did the change avoid installer/signing/public-release scope creep?
- Were unrelated files and generated artifacts left alone?

## 9. Round Budget

Estimated total: 6 conversation rounds.

| Round | Type | Goal |
| --- | --- | --- |
| R1 | Main | Baseline current icon/animation/toggle behavior and create P12 validation doc. |
| R2 | Main | Implement tray/app icon completion and tests. |
| R3 | Main | Implement soft open and close animation service behavior and tests. |
| R4 | Main | Integrate tray click toggle with animated close/reopen behavior and tests/manual smoke. |
| R5 | Buffer | Repair UX edge cases, test flakiness, icon asset issues, or docs notes. |
| R6 | Final | Full validation, package, release bundle generation, final smoke, push, and completion report. |

R5 may be unused. Do not spend it on unrelated UI redesign.

## 10. Round Plan

### R1 - Baseline And Validation Doc

Goals:

- Create `docs/P12_CUSTOMER_UX_POLISH_VALIDATION.md`.
- Record current evidence:
  - `TrayIconService` currently uses `SystemIcons.Application`.
  - `.csproj` has `ApplicationIcon` pointing at `assets\icons\dateview.ico`.
  - `PopupAnimationService` currently does entrance-only opacity plus translate.
  - `App.TogglePopup` already has visible-state branching but close path is immediate `Hide()`.
- Run focused Desktop tests or full `Validate.cmd`.
- Commit/push P12 baseline docs.

PASS:

- Validation doc exists and accurately captures the customer feedback and starting state.

### R2 - Icon Completion

Goals:

- Make tray icon use Dateview icon asset or an embedded app icon resource instead of `SystemIcons.Application`.
- Keep icon ownership/disposal correct; avoid leaking `Icon` handles.
- Add or update tests so the tray icon is not the generic system application icon.
- Verify publish/package still includes or embeds the icon correctly.

PASS:

- Tray icon is Dateview-specific.
- Application icon configuration remains valid.
- Tests pass.

### R3 - Soft Open/Close Animation

Goals:

- Extend `PopupAnimationService` to support a close animation and completion callback/task.
- Replace any motion that reads as bounce/shake with a restrained fade plus subtle content transform.
- Preserve content-only transform animation; do not animate `Window.RenderTransform`.
- Add tests covering entrance and exit behavior/fallback.

PASS:

- Animation service has deterministic open/close behavior.
- Tests verify content transform use and no `Window.RenderTransform` local value.

### R4 - Tray Toggle Integration

Goals:

- Wire left-click tray toggle to animated close when visible.
- Ensure Escape and view-model close request use the close animation when feasible.
- Ensure repeated clicks during opening/closing produce deterministic behavior.
- Add tests if existing seams allow; otherwise record manual smoke evidence and consider a small Desktop-shell test seam.

PASS:

- Manual smoke confirms click-open/click-close/reopen feels natural.
- Automated tests cover the most important state transitions feasible in the current architecture.

### R5 - Buffer Repair

Use only for:

- Icon asset issue.
- Animation timing/easing adjustment.
- Toggle race/state bug.
- Test reliability or small documentation note.

### R6 - Final Validation

Must complete:

- `Validate.cmd` passes.
- `Package.cmd` passes.
- `package-release.ps1` passes.
- `git diff --check` passes.
- Boundary scan confirms no injection/hook/admin/online/telemetry scope creep.
- P12 validation doc has final evidence.
- All P12 commits pushed.
- `main...origin/main` clean except unrelated pre-existing untracked files explicitly preserved.

## 11. PASS Criteria

P12 is accepted only if:

- Tray icon no longer uses the generic system application icon.
- Dateview application/tray icon experience is coherent in dev and packaged output.
- Popup opening animation is soft and does not bounce/shake.
- Popup can be closed by clicking the tray icon again.
- Closing animation is soft and natural.
- Escape and previous close paths still work.
- No forbidden shell integration, hook, admin, online, telemetry, installer/signing scope was added.
- Desktop tests and full validation pass.
- Final package/release bundle generation passes.

## 12. Completion Report Template

```text
Phase: P12 Customer UX Polish
Estimated rounds:
Actual rounds:
Completed:
Not completed by design:
Validation:
Manual smoke:
Icon evidence:
Animation/toggle evidence:
Pushed commits:
Consumed buffer:
Architecture deviations:
Residual risks:
Recommended next phase:
```

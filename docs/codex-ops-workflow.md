<!-- codex-project-ops-workflow: initialized -->
<!-- initialized-at: 2026-06-21 19:19:48 +08:00 -->

# Codex Ops Workflow

Initialization status: initialized
Project: Dateview
Repository root: D:/ToolProjects/Dateview
Machine config: `.codex/project-ops-workflow.json`
Skill: project-ops-workflow

Treat this document and `.codex/project-ops-workflow.json` as the source of truth for mechanical project operations.

## Global Wrappers

Run these from the repository root:

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\EnvCheck.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\RestoreDeps.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Build.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Test.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Format.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Package.cmd
```

## Validate Sequence

Run these before commits that change source, tests, or project configuration:

```powershell
dotnet restore Dateview.slnx
dotnet build Dateview.slnx --configuration Release --no-restore
dotnet test Dateview.slnx --configuration Release --no-build
dotnet format Dateview.slnx --verify-no-changes
```

## Dev Server

No dev server is configured. This is a Windows desktop app.

## Package

```powershell
dotnet publish src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj --configuration Release
```

## Safety Policy

Do not run destructive clean/reset/deploy commands unless the user explicitly asks.

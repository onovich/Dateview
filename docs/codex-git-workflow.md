<!-- codex-project-git-workflow: initialized -->
<!-- initialized-at: 2026-06-21 19:19:48 +08:00 -->

# Codex Git Workflow

Initialization status: initialized
Project: Dateview
Repository root: D:/ToolProjects/Dateview
Machine config: `.codex/project-git-workflow.json`
Skill: project-git-workflow

Treat this document and `.codex/project-git-workflow.json` as the source of truth for this repository's Codex git workflow. Do not replace them with generic defaults unless the user explicitly asks to reinitialize or update the policy.

## Global Wrappers

Run these from the repository root:

```powershell
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Status.cmd
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Validate.cmd
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Commit.cmd -Message "commit message" -Paths path\to\file,other\file
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\CommitAndPush.cmd -Message "commit message" -Paths path\to\file,other\file
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Push.cmd
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Stash.cmd -StashMessage "reason"
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\StashPop.cmd
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\Ignore.cmd -Pattern build-output/
C:\Users\Administrator\.codex\skills\project-git-workflow\scripts\git\DiscardPaths.cmd -ConfirmDangerous -Paths path\to\file
```

## Status

```powershell
git -c safe.directory=D:/ToolProjects/Dateview status --short --branch
```

## Validation

Use the ops validation workflow before commits that change source, tests, or project configuration:

```powershell
C:\Users\Administrator\.codex\skills\project-ops-workflow\scripts\ops\Validate.cmd
```

## Staging Policy

Ask each time. Inspect status before staging and preserve unrelated user changes unless the user explicitly asks to include them.

## Commit

Use the global wrapper's built-in git commit after staging according to policy. Prefer concise conventional commit messages unless the user specifies another message.

## Push

```powershell
git -c safe.directory=D:/ToolProjects/Dateview push -u origin HEAD
```

## Docs And TODO

Keep docs updated when architecture, workflow, or public behavior changes.

## Safety And Branch Policy

No force-push. Do not run destructive git commands unless explicitly requested by the user.

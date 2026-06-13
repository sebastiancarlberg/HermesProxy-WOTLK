# Codex Workflow

This repository uses a branch workflow for Hermes fixes. Follow it unless the user explicitly asks for a different flow.

## Branch Roles

- `master` is stable release code only.
- `develop` is the integration branch for verified fixes and features.
- Work on each new fix or feature in its own branch, created from `develop`.
- Do not commit unverified experiments directly to `develop` or `master`.

## Fix Workflow

1. Create a focused branch from `develop`, using a name like `fix/<short-name>` or `feature/<short-name>`.
2. Keep each branch scoped to one issue or feature.
3. Build and let the user test the branch.
4. Commit only the code that was actually verified by testing.
5. Merge verified branches into `develop`.
6. Merge `develop` into `master` only when the user says the combined set is stable enough for release.

If work has already happened as mixed local changes, first preserve it on a clearly named `test/...` or `wip/...` branch from `develop`. Then split verified pieces into focused `fix/...` or `feature/...` branches before merging to `develop`.

## Local Files

- Do not commit local launchers, local build outputs, machine-specific configuration, logs, or temporary diagnostic files unless the user explicitly asks.
- Keep experimental diagnostics separate from final fixes. Remove or isolate broad debug gates before making final commits.

## Current Hermes Testing Pattern

- The fork repo is `C:\Users\carlb\Documents\WoW\HermesProxy-WOTLK-fork`.
- Build with:
  `& 'C:\Users\carlb\Documents\WoW\TBC\.dotnet-sdk\dotnet.exe' publish 'C:\Users\carlb\Documents\WoW\HermesProxy-WOTLK-fork\HermesProxy\HermesProxy.csproj' -c Release -r win-x64 --self-contained true -o 'C:\Users\carlb\Documents\WoW\HermesProxy-WOTLK-fork\build'`
- The user tests with:
  `C:\Users\carlb\Documents\WoW\HermesProxy-WOTLK-fork\launchers\Start Hermes Fork + Client.cmd`
- Treat runtime test reports from the user as the source of truth for whether a fix is verified.

## Fresh Session Checklist

At the start of a new Codex session in this repo:

1. Run `git status --short --branch` and `git branch --list -vv`.
2. Read this file before making branch or commit decisions.
3. Check recent commits with `git log --oneline --decorate --graph --max-count=20 --all`.
4. Ask whether the user wants testing work, branch cleanup, or final verified commits if the current branch has mixed WIP changes.
5. Prefer local repo evidence and logs over memory from previous chats.

## Project Notes

- `VERIFIED_FIXES.md`, `CORE-ISSUE-PLAN.md`, and similar status files may contain useful history, but code and current test results are authoritative.
- `Hermes-Experiment` can contain older experimental work. Port only deliberate, verified changes into this fork.
- The Wrathion source at `C:\Users\carlb\Documents\WoW\Wrathion-3.4.3_Source` is a useful 3.4.3 reference when Hermes packet serialization is uncertain.
- Broad gates such as skipping all non-self player or pet Values updates are diagnostic stabilizers unless they have been converted into a narrow serialization fix and verified.

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
3. Before building or testing an existing branch, check whether `develop` has moved. Rebase the branch onto current `develop` unless there is a specific reason not to.
4. Build and let the user test the branch.
5. Commit only the code that was actually verified by testing.
6. Merge verified branches into `develop`.
7. Merge `develop` into `master` only when the user says the combined set is stable enough for release.

If work has already happened as mixed local changes, first preserve it on a clearly named `test/...` or `wip/...` branch from `develop`. Then split verified pieces into focused `fix/...` or `feature/...` branches before merging to `develop`.

## Local Files

- Do not commit local launchers, local build outputs, machine-specific configuration, logs, or temporary diagnostic files unless the user explicitly asks.
- Keep experimental diagnostics separate from final fixes. Remove or isolate broad debug gates before making final commits.

## Current Hermes Testing Pattern

- The repo root is the current checkout directory.
- Build from the repo root with:
  `dotnet publish HermesProxy/HermesProxy.csproj -c Release -r win-x64 --self-contained true -o build`
- If the user has a local pinned .NET SDK or wrapper script, use it only for local execution and do not commit that machine-specific path.
- For local game testing, look for repo-local or sibling `launchers/` scripts. Treat launchers as local helpers unless they are already tracked and portable.
- Treat runtime test reports from the user as the source of truth for whether a fix is verified.

## Fresh Session Checklist

At the start of a new Codex session in this repo:

1. Run `git status --short --branch` and `git branch --list -vv`.
2. Read this file before making branch or commit decisions.
3. Check recent commits with `git log --oneline --decorate --graph --max-count=20 --all`.
4. Run `git fetch --prune --all` when network access is available, then check whether the current branch is behind its upstream or behind `develop`.
5. Ask whether the user wants testing work, branch cleanup, or final verified commits if the current branch has mixed WIP changes.
6. Prefer local repo evidence and logs over memory from previous chats.

## Project Notes

- `VERIFIED_FIXES.md`, `CORE-ISSUE-PLAN.md`, and similar status files may contain useful history, but code and current test results are authoritative.
- A local `Hermes-Experiment` checkout may contain older experimental work. Port only deliberate, verified changes into this fork.
- A local Wrathion 3.4.3 source checkout is a useful reference when Hermes packet serialization is uncertain. If present, it is often a sibling directory of this checkout; otherwise ask the user where it is. Do not commit absolute paths to it.
- Broad gates such as skipping all non-self player or pet Values updates are diagnostic stabilizers unless they have been converted into a narrow serialization fix and verified.

## Debugging References

- Prefer field-by-field comparison against a known-good 3.4.3 implementation when modern update-field serialization is suspect.
- Useful reference areas in Wrathion/Trinity-style sources include `Entities/Object/Updates/UpdateFields.*`, object update builders, and packet structures for the affected object type.
- Use Hermes logs, client crash dumps, packet captures/sniffs, and deterministic reproduction reports together. A crash that always faults at the same client instruction usually points to packet layout or field-width/alignment, not game content by itself.
- When a broad diagnostic gate stabilizes the client, keep it on a diagnostic branch and use it to isolate the exact packet/field. Final fixes should repair the specific serialization or translation path.

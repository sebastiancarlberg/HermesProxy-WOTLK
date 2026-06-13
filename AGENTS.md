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

## Local Files

- Do not commit local launchers, local build outputs, machine-specific configuration, logs, or temporary diagnostic files unless the user explicitly asks.
- Keep experimental diagnostics separate from final fixes. Remove or isolate broad debug gates before making final commits.

## Current Hermes Testing Pattern

- The fork repo is `C:\Users\carlb\Documents\WoW\HermesProxy-WOTLK-fork`.
- Build with the repo's known publish command and let the user test with their launcher.
- Treat runtime test reports from the user as the source of truth for whether a fix is verified.

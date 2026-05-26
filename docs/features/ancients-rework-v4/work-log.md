# Ancients Rework v4 Work Log

Status: active summary only. The full chronological log was archived to `docs/archive/feature-work-logs/ancients-rework-v4/work-log-20260518-pre-slim.md`.

## Current Summary

- The active implementation is `Spire Plus` under `EZMicroBalanceCode/` and `EZMicroBalance/`; the stable manifest id remains `EZMicroBalance`.
- Urda, Morvi, and Lotha are source-complete test slices with marker relics for selected rewards.
- Vakuu has a dedicated source enemy/scene and remains opt-in until live victory return, save/load, death/failure, and co-op proof exists.
- Ancient reward text, marker relic visibility, Root Sight, Seed Bank, Morvi debt/page/proofread flows, Lotha powers, and Vakuu entry are covered by source guards.
- Live clicked UI, gameplay, save/load, death/failure path, and co-op evidence remain manual gates.

## Latest Refactor Notes

- Urda hook logic has been split into small `UrdaBlessingService.*.cs` partial files by reward responsibility.
- Morvi Forbidden Loan selection lives in `MorviBlessingService.ForbiddenLoan.cs`, borrowed-card play cost / Act 2 settlement / cleanup live in `MorviBlessingService.ForbiddenLoanBorrowedCards.cs`, Open-Book turn flow lives in `MorviBlessingService.OpenBook.cs`, Open-Book sealed-card restore/cleanup lives in `MorviBlessingService.OpenBookState.cs`, Red Ink Overdraft lives in `MorviBlessingService.RedInkOverdraft.cs`, and Debt Settlement lives in `MorviBlessingService.DebtSettlement.cs`.
- Current docs and tests should point readers to `docs/test-ready-development-goal.md`, `docs/issues.md`, `docs/review.md`, and `docs/toreview.md` for active work.

## Validation Baseline

Recent no-game cleanup validation:

- `dotnet build EZMicroBalance.sln`
- `dotnet test EZMicroBalance.sln --no-build`
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `git diff --check`

Package refresh, artifact tests, and live game evidence are separate gates.

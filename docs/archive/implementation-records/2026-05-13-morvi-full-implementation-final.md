Implemented and verified the Morvi v2.2 source-ready test slice. Manifest id remains `EZMicroBalance`; Morvi is default-on with disable/force gates.

**Files Changed**
- Morvi source: `EZMicroBalanceCode/Ancients/Expansion/Morvi/*`, `EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs`, `EZMicroBalanceCode/Ancients/Common/AncientPlayerState.cs`, `EZMicroBalanceCode/MainFile.cs`
- Morvi resources/localization: `EZMicroBalance/localization/eng|zhs/{ancients,cards,relics,powers}.json`, Morvi event/icon/option assets under `EZMicroBalance/images/`, Morvi scene under `EZMicroBalance/scenes/`, `export_presets.cfg`
- Tests: `tests/EZMicroBalance.Tests/MorviV22GuardTests.cs`, plus updated Ancient/source/release guard tests
- Docs: `PROJECT_STATE.md`, `README.md`, `docs/issues.md`, `docs/issues/ancient-expansion-v2.2.md`, `docs/features/ancient-expansion-v2.2/*`, `docs/mod-changelog.md`, `docs/test-ready-completion-audit.md`, related current checklist/index docs

**Morvi Blessing Status**
- Forbidden Loan: source-complete; uses class Ancient card pool and Borrowed Ancient marker. Deviation: after Act 2 boss it auto-settles by paying 180 Gold if possible, otherwise removes the card; no post-boss choice UI claimed.
- Misprint Press: source-complete; uses play-count modification, no copied hand card, no Power trigger, no recursion.
- Red Ink Overdraft: source-complete with UI deviation; implemented as a temporary 0-cost active card, not a native combat button.
- Overdue Library: source-complete; all six Archive Pages implemented with temporary cleanup.
- Open-Book Exam: source-complete with holding deviation; sealed cards use exhaust-pile holding and return on turn 3 only when hand space allows.
- Paperstorm: source-complete; Status draw conversion is draw-pile based with a 2-per-turn cap.
- Blueprint Proof: source-complete; three Proofread stacks, Power-safe, no extra-play/copy. Temporary upgrade uses source command paths where safe.
- Debt Settlement: source-complete; 220 Gold, 320 Debt, 40 due, 3 HP per 10 Gold short, debt decrements by due and persists through mirrored state.

**Validation**
- `dotnet build EZMicroBalance.sln`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 98 passed / 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed; CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln`: first failed due BOM in `export_presets.cfg`; fixed, rerun passed with the known nested `source code/project.godot` warning.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 98 passed / 18 skipped.
- Final no-build test after doc updates: passed, 98 passed / 18 skipped.

No live game, save-load, or co-op testing was run.
Implemented the corrective Lotha pass.

**Mechanic status**
- Mirror Rebuttal: exact v2.2. It now selects/marks one real non-Curse, non-Status deck card, pulls the matching combat card to hand at combat start when needed, and resolves on first play of that marked card.
- Deferred Verdict: exact v2.2. Turn 4 now draws 4, grants 4 Energy, grants 3 player-owned Verdict stacks, consumes one stack per next non-Status card, and heals 4 HP if combat ends before turn 4.
- Single Sentence: exact v2.2 source-safe behavior. First Attack/Skill each turn gets +2 plays, then the turn is capped at four more cards; a first Power uses the honest Energy/draw replacement and does not consume the sentence.
- Public Evidence: exact v2.2. Debuff applications are doubled via source power hooks, Enlightenment is gained/lost correctly, and turn-start Enlightenment consumes for draw + Block.

Also fixed English/zhs localization, added Enlightenment power text, removed Lotha mojibake, and replaced the Lotha guard tests so they fail on the previous wrong behavior. Docs now explicitly admit the previous deviation in `docs/external-codex-progress.md`.

Validation run:
- `dotnet build EZMicroBalance.sln`: passed, 0 warnings/errors
- `dotnet test EZMicroBalance.sln --no-build`: passed, 89 passed / 18 skipped
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed
- `git diff --check`: passed, with existing CRLF normalization warnings only
- `dotnet publish EZMicroBalance.sln`: passed, with the known nested `source code/project.godot` Godot warning
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 89 passed / 18 skipped
- Final no-build test after doc update: passed, 89 passed / 18 skipped

Live game load, save/load, and co-op testing were not run.
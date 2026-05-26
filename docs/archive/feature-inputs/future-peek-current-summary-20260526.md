# Future Peek Goal

Future Peek is folded into the single `Spire Plus` mod. The active source lives in `EZMicroBalanceCode/Preview/`, and the technical manifest id remains `EZMicroBalance`.

The long design intake is archived at `docs/archive/feature-inputs/future-peek-goal-20260526.md`. Current implementation status:

- Crystal Sphere peek is a local UI-only preview. It changes only `%ScryMask` alpha and now runs in co-op while logging evidence.
- Transform preview is a local UI-only preview. It uses a forked RNG snapshot, does not advance real RNG, does not create choices or rewards, and falls back to vanilla cycling if display fails.
- Map foresight and reward foresight are not implemented here. They would affect future rooms or reward results and need a separate deterministic or host-authoritative precommit plan.

Do not recreate a standalone `EZFuturePeek` project. Preview tools stay inside `Spire Plus`.

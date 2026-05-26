# Ancient Expansion v2.2 Issues

Status: Urda is default-on with an eleven-blessing source-complete/live-pending v2.2 test slice, Morvi is default-on with a source-complete v2.2 test slice, Lotha is default-on with a source-complete v2.2 test slice, and Vakuu fight is hidden by default with a source-dedicated monster/scene pending live proof.

Current-state constraints:

- Urda is already default-on for private-beta testing with eleven source-backed blessing ids, including Elite Root.
- Urda's new v2.2 rows use documented source-safe deviations where local Core source did not prove a richer UI path.
- Urda live gameplay/save-load verification is still pending; encoded Urda/Morvi/Lotha player state now has focused source guards proving `AncientPlayerState` mirror usage and rejecting direct encoded field bypasses, but those guards are not live save/load proof.
- Morvi is default-on for private-beta testing with all eight v2.2 blessing ids, preferred `SPIREPLUS_DISABLE_MORVI` plus legacy `EZMB_DISABLE_MORVI`, force-Ancient gates, forced-blessing gates, custom event art, option art, English/zhs localization, and source guards.
- Morvi source-safe deviations are documented: Forbidden Loan auto-settles after the Act 2 boss instead of opening a post-boss choice, Red Ink Overdraft uses a temporary active card instead of a native combat button, only adds it when hand space allows, and settles unpaid debt through nonlethal HP loss, Open-Book Exam uses exhaust-pile holding with hand-space limits, and Blueprint Proof uses reversible upgrade/downgrade command paths where possible.
- Morvi live gameplay, save/load, and co-op verification are still pending.
- Lotha is default-on for private-beta testing with preferred `SPIREPLUS_DISABLE_LOTHA` plus legacy `EZMB_DISABLE_LOTHA`, force-Ancient gates, forced-blessing gates, custom event art, option art, and all eight v2.2 blessing ids.
- Lotha no longer uses the geometric placeholder event art. It uses the recovered mirror-tribunal background, and Urda/Morvi/Lotha option/icon art now uses browser GPTimage2 rebuilt transparent PNGs recorded in the art manifest. Live UI preview remains pending.
- Lotha live gameplay, save/load, lethal-path, and co-op verification are still pending. Death Reprieve has one documented source-safe deviation: enemy-turn lethal starts the reprieve on the next player turn because local source did not prove safe immediate enemy-turn interruption.
- Vakuu fight is hidden by default while runtime proof is pending. It can be enabled with preferred `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` plus legacy `EZMB_ENABLE_VAKUU_FIGHT=1`, can be focused with `SPIREPLUS_FORCE_ANCIENT=VAKUU`, and can be forced to the fight option with `SPIREPLUS_FORCE_VAKUU_FIGHT=1`. Legacy `EZMB_FORCE_*` aliases remain accepted.
- Vakuu fight now includes a dedicated Vakuu monster, a custom encounter scene, and source-backed Contract/Stolen Vault/Blood Debt pressure after the normal hand draw on turns 1, 3, and 5. Source also clears the parent event `Node` before child combat to address the reported post-victory black screen risk, and the prefinished restore path skips the duplicate Ancient heal when Core reconstructs the parent event below the finished combat. Live UI/gameplay, victory return, save/load, failure/death, and co-op verification are still pending. The current source gate requires single-player (`runState.Players.Count == 1`) and does not claim multiplayer safety.
- Morvi active event art uses the recovered user-uploaded blue-eye court/scribe background; option/icon art uses browser ChatGPT/GPTimage2 oil-repaint transparent PNGs recorded in the art manifest. Live UI/gameplay evidence remains pending.
- v2.2 must not be represented as release-ready until source/live/save-load checks pass.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES

Priority: P1
Status: source-fixed / live-pending
Area: Ancient expansion v2.2 card and power safety rules

Source closure notes:

- Card-copy, replay, cost-change, and generated-card paths now have source guards for power-card exclusions, generated-copy recursion, and combat-end cleanup.
- Remaining closure evidence is live gameplay, save-load, and co-op verification for Urda, Morvi, Lotha, and Vakuu.

## Historical v2.2 Planning Anchors

- `ISSUE-2026-05-12-MORVI-V22-PLANNING`: archived/source-trace anchor preserved for release coverage guards; current implementation status remains source-fixed or live-pending in the active sections above.
- `ISSUE-2026-05-12-LOTHA-V22-PLANNING`: archived/source-trace anchor preserved for release coverage guards; current implementation status remains source-fixed or live-pending in the active sections above.
- `ISSUE-2026-05-12-VAKUU-FIGHT-V22-PLANNING`: archived/source-trace anchor preserved for release coverage guards; current implementation status remains source-fixed or live-pending in the active sections above.
- `ISSUE-2026-05-12-URDA-V22-ALIGNMENT`: archived/source-trace anchor preserved for release coverage guards; current implementation status remains source-fixed or live-pending in the active sections above.
- `ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MILESTONE-GATES`: archived/source-trace anchor preserved for release coverage guards; current implementation status remains source-fixed or live-pending in the active sections above.

## ISSUE-2026-05-13-ANCIENT-EVENT-UI-ART-REPAIR

Priority: P0
Status: source-mitigated / live-pending
Area: Ancient event UI / art / dialogue

Implemented source mitigation:

- Local `NAncientEventLayout` source was used to make active custom Ancient background scenes instantiate safely as `Control`.
- Urda and Lotha use large event/background art only inside the clicked Ancient screen.
- Urda and Lotha have separate map/run-history icons and outline resources.
- Urda and Lotha options use non-droppable marker relics so vanilla option buttons have art.
- English and zhs dialogue keys exist for active custom Ancients.
- Force-Ancient env vars support focused Urda/Lotha/Vakuu testing without random Ancient selection.
- Urda/Morvi/Lotha option generation now logs a clear fallback if a forced blessing id does not match or if the source-backed option pool is unexpectedly empty/undersized; Urda's initial screen uses four options, while Morvi and Lotha use three.
- 2026-05-14 resource-routing hardening confirmed active Urda, Morvi, and Lotha clicked backgrounds route through `CustomScenePath` to Control-root scenes with event art, while map/run-history icons and option marker relic art stay on separate `images/ancients/**` paths. Guard tests now reject generic `images/relics/relic.png` option-marker fallback and check manifest resource targets against `export_presets.cfg`; `scripts/audit-ancient-art-assets.ps1 -FailOnMissingExport` reports 0 missing exports for the current manifest.

Remaining closure evidence:

- Live UI verification must show each active custom Ancient renders the event art, the expected option count, option art, dialogue, and no black screen.

## ISSUE-2026-05-13-LOTHA-FULL-TEST-IMPLEMENTATION

Priority: P1
Status: source-risk reduced / live-pending
Area: Act 3 Ancient / Lotha

Required blessing ids:

- `lotha_mirror_rebuttal` / Mirror Rebuttal / 反证之镜
- `lotha_mirror_hall_echo` / Mirror Hall Echo / 镜厅回声
- `lotha_presumption` / Presumption of Innocence / 无罪推定
- `lotha_closed_court` / Closed Court / 终审封庭
- `lotha_deferred_verdict` / Deferred Verdict / 延期判决
- `lotha_death_reprieve` / Death Reprieve / 死刑缓期
- `lotha_single_sentence` / Single Sentence / 单牌宣判
- `lotha_public_evidence` / Public Evidence / 公开罪证

Safety:

- Power cards must not be copied, replayed, or extra-played.
- Lotha Power-card fallbacks do not copy, replay, or extra-play Power cards. Mirror Rebuttal Power fallback makes the marked Power cost 0 for that play. Mirror Hall Echo, Deferred Verdict, and Single Sentence Power fallbacks make the current eligible Power cost 0 for that play and draw 1 with no Energy gain.
- Lotha extra-play rulings use `ModifyCardPlayCount` on the original player-driven Attack/Skill instead of generated replay copies.
- Generated copies must not recurse.
- `lotha_death_reprieve` uses the source-backed `ShouldDieLate` / `AfterPreventingDeath` path, modeled after local death-prevention source. Runtime lethal-path verification remains pending.

Corrective source-polish completed 2026-05-13:

- `lotha_mirror_rebuttal`: selecting the blessing opens a source-safe deck-card picker for one non-Curse, non-Status card and marks that real deck card. On the first player turn after normal draw, if the matching combat card is not already in hand, it is moved to hand through `CardPileCmd.Add(..., PileType.Hand)`. The first time that marked card is played each combat, Attack/Skill cards play one additional time; Power cards cost 0 for that play.
- `lotha_mirror_hall_echo`: player-turn end records the last player-played non-Status Attack/Skill/Power; the next player turn's first player-played matching type consumes the echo. Attack/Skill adds one play; Power costs 0 for that play and draws 1 with no Energy gain; autoplay/generated clone cards are excluded from setting or consuming the echo.
- `lotha_presumption`: combat start applies visible Innocent state; each player-turn start while Innocent draws 2, grants 1 Energy, and grants 8 Block. Conservative Core-backed break detection uses unblocked enemy `ValueProp.Move` damage with no card source; when it breaks, Innocent is removed and the player loses 8 HP.
- `lotha_closed_court`: combat reward mutation removes only `CardReward` from combat rewards so gold, potion, and relic rewards remain. Turn 1 draws 4 and grants 2 Energy; turn 4 draws 2 and grants 2 Energy. It no longer discounts the first three cards.
- `lotha_deferred_verdict`: on turn 4, the player draws 4, gains 4 Energy, and gains 3 player-owned Verdict stacks. This turn, each next non-Status card consumes 1 player-owned Verdict; Attacks/Skills play one additional time and Powers cost 0 for that play and draw 1 with no Energy gain. If combat ends before turn 4, `AfterCombatEnd` heals 4 HP when the player is alive.
- `lotha_death_reprieve`: once per run prevents death, sets HP to 1, starts a reprieve player turn with draw 10, Energy 10, cost 0 cards, and temporary death prevention, then force-kills the player at turn end if enemies remain. Source-safe deviation: enemy-turn lethal starts on the next player turn rather than interrupting immediately. Rehydration from deck-mirrored pending/active phase now logs the restored phase and keeps active-turn save/load continuation explicitly live-pending.
- `lotha_single_sentence`: the first player-driven Attack/Skill each turn plays two additional times, then only four more normal player-played cards can be played that turn; the first Power before that ruling costs 0 for that play, draws 1, and does not consume the sentence.
- `lotha_public_evidence`: source power-amount hooks double player-applied non-damaging negative statuses on enemies and grant 1 Enlightenment; they also double enemy-applied non-damaging negative statuses on the player and remove 1 Enlightenment. Weak, Vulnerable, and Frail count; Poison, damage-over-time, countdown damage, and source-proven damage/kill Debuffs do not. At turn start, up to 3 Enlightenment are consumed; each consumed stack draws 1 and gives 4 Block.
- English and Simplified Chinese Lotha option/relic/power text now use rich-text highlights for the visible mechanics, include Innocent, Death Reprieve, Verdict, and Enlightenment hoverable powers, and are guarded against stale placeholder wording and mojibake.
- 2026-05-14 follow-up closed the source-review residual risks: Mirror Rebuttal's full-hand fallback is guarded, and Single Sentence now has branch-specific guards for the pre-ruling Power fallback, Attack/Skill-only ruling, four-card post-ruling cap, autoplay/clone/non-first/ruling-card exclusions, and stale EN/zhs text.

Next target:

- Live-test Lotha gameplay, save/load, lethal-path, and co-op behavior before claiming private beta readiness.
- Replace temporary option/relic crops with bespoke generated relic-style art using `docs/test-ready-development-goal.md`.
- Continue broader v2.2 hover/rich-text parity for non-Lotha mechanics.

## ISSUE-2026-05-13-VAKUU-FIGHT-TEST-IMPLEMENTATION

Priority: P0
Status: hidden-by-default / source-dedicated / live-pending
Area: Act 3 Vakuu fight option

Implemented source evidence:

- Event option creation uses a `Vakuu.GenerateInitialOptions` postfix and a non-droppable marker relic so the option has art, but it only appears when explicitly enabled or forced.
- The current encounter uses a dedicated `EzmbVakuuTrialMonster`, localized monster/move text, and a custom `ezmb_vakuu_trial.tscn` encounter scene with a Vakuu marker slot.
- Event-to-combat transition now awaits `RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(...)`, uses a custom `RoomType.Monster` encounter with normal rewards disabled, sets `ShouldResumeParentEventAfterCombat = true`, and does not call Core's `EnterCombatWithoutExitingEvent(...)` helper because that helper rejects non-shared events.
- Before entering child combat, source clears the parent event `Node` to match Core's own child-combat node cleanup behavior and address the reported post-victory black screen risk.
- Active Vakuu child combat no longer stores `ParentEventId`, avoiding Core's known active-combat serialization exception. A narrow `CombatRoom.ToSerializable()` postfix records the Vakuu parent only after the Vakuu trial combat is prefinished, keeping prefinished parent restore source-shaped while live save/load proof remains pending.
- Victory routing patches `EventModel.Resume(...)` for the Vakuu parent event after `EzmbVakuuTrialEncounter`, then uses the protected `SetEventState(...)` path by reflection to offer 1/2/3 non-Vakuu Act 3 Ancient relic options from Nonupeipe/Tanx plus custom Lotha option relics based on broken Stolen Vault locks. Lotha victory choices use `LothaRewardSelectionService.SelectBlessing(...)`, so the visible marker relic and Lotha blessing state are granted together.
- If no unclaimed non-Vakuu reward options remain, victory uses an explicit fallback page instead of passing zero options and silently finishing. If a restored victory resume has no owner, source now logs that the explicit fallback path was used and keeps live restore proof pending.
- The custom encounter has `ShouldGiveRewards => false` and does not put `LinkedRewardSet` or extra rewards into the combat room, avoiding the nonserializable parent-event combat reward path discovered in source. `CombatRoom.OfferRoomEndRewards()` is patched for the prefinished Vakuu trial restore path so it resumes the parent event instead of generating normal combat rewards. Live save/load proof remains pending.
- The visible option/relic text now says the player fights Vakuu, says Contract choices appear after draw on turns 1/3/5, says Contracts can break Stolen Vault locks or manage Blood Debt, says broken locks create extra blessing choices and loot Gold, says Cash Out appears after a broken lock, says normal combat rewards are disabled, and says death ends the run.
- `EZMB_VAKUU_KNIFE_CONTRACT`, `EZMB_VAKUU_TEMPTATION`, and `EZMB_VAKUU_SHELTER_CONTRACT` are hidden 0-cost Skill token Contracts with Ethereal and Exhaust. Playing one signs the Contract, costs HP, breaks a lock if any remain, adds Blood Debt, and then resolves Knife/Gold/Shelter command effects.
- A dedicated run-state combat hook offers Contracts only while `combatState.Encounter is EzmbVakuuTrialEncounter`, applies Stolen Vault to Vakuu on combat entry, and tracks Act-scaled unblocked player-turn damage lock breaks through `AfterDamageGiven` so lethal hits count. It is gated to single-player by `VakuuFightFeatureGate.IsFightEnabledForRun(...)`.

Next target:

- Live-test the fight, post-victory return/no-black-screen path, Contract hand injection/play behavior, Stolen Vault lock breaks, Blood Debt attack scaling, failure/death path, save/load behavior, and victory reward flow before exposing the fight by default or claiming release-ready behavior.

## ISSUE-2026-05-13-SPIREPLUS-TECHNICAL-IDENTITY-MIGRATION

Priority: P1
Status: planned / staged
Area: naming / package / manifest

Goal:

- Players should download a package named `SpirePlus`, not `EZMicroBalance`.

Boundary:

- Do not mutate the existing `EZMicroBalance` manifest id in place.
- Safe first step: rename only generated archive/package-facing files to `SpirePlus-v...zip` while documenting that the installed manifest id remains `EZMicroBalance`.
- Full technical migration requires a separate `SpirePlus` identity, saved-field/env-var compatibility decision, script/test/package updates, and fresh runtime validation.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-DESIGN-DOC-INGEST

Priority: P0
Status: complete / archived
Area: planning / docs / future Ancient expansion roadmap

Closure:

- Full v2.2 design exists in `docs/features/ancient-expansion-v2.2/source-design.md`.
- Milestones are split and gated.
- `docs/issues.md` remains compact.
- Historical audit matrices moved to `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/`.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES

Priority: P0
Status: open
Area: extra play / copy / ability-card safety

Global rule:

- Extra play, copy, reprint, verdict, echo, and replay effects may target Attacks and Skills.
- Power cards are not copied, extra-played, or replayed by default.
- If such an effect meets a Power card, use replacement rewards: cost becomes 0, draw cards, gain energy, or wait for the next Attack/Skill.
- Extra-played or copied cards must not recursively trigger the same blessing.

## ISSUE-2026-05-12-MORVI-V22-PLANNING

Priority: P1
Status: source-complete / live-pending
Area: Act 2 Ancient / Morvi

Morvi blessing pool:

- `morvi_forbidden_loan` / Forbidden Loan / 禁书借贷
- `morvi_misprint_press` / Misprint Press / 错页印刷机
- `morvi_red_ink_overdraft` / Red Ink Overdraft / 红墨透支
- `morvi_overdue_library` / Overdue Library / 逾期书库
- `morvi_open_book_exam` / Open-Book Exam / 开卷考试
- `morvi_paperstorm` / Paperstorm / 纸页风暴
- `morvi_blueprint_proof` / Blueprint Proofreading / 蓝图校样
- `morvi_debt_settlement` / Debt Settlement / 债务清算

Implemented source slice:

- Morvi is default-on in Act 2 and can be hidden with `SPIREPLUS_DISABLE_MORVI=1`; legacy `EZMB_DISABLE_MORVI=1` still works.
- `SPIREPLUS_FORCE_ANCIENT=MORVI` and `SPIREPLUS_FORCE_MORVI_BLESSING` support focused testing. Legacy `EZMB_FORCE_*` aliases still work.
- `morvi_forbidden_loan`: chooses from three source-discovered class Ancient cards, upgrades and marks the chosen card, charges HP on borrowed-card play, and auto-settles after the Act 2 boss by paying 180 Gold if possible or removing the borrowed card otherwise.
- `morvi_misprint_press`: first player-played Attack/Skill each turn uses `ModifyCardPlayCount` on the original card, draws 1 when the original/base Energy cost is at least 1, and creates no copied card in hand.
- `morvi_red_ink_overdraft`: implemented as a source-safe temporary active card because no native Ancient combat button was proven. It is added only when hand space allows, can be played only at 0 Energy once per turn, draws 2, grants Energy 1, records one red-ink debt, and settles unpaid debts with nonlethal HP loss.
- `morvi_overdue_library`: each combat adds three random temporary Archive Pages from all six page ids; pages clean up after combat and unplayed pages have no extra penalty.
- `morvi_open_book_exam`: first combat turn draws up to five extra cards, gains 2 Energy, seals tracked Open Book cards remaining in hand at turn end, and returns them on turn 3 with cost 0 when hand space allows.
- `morvi_paperstorm`: shuffles four Waste Paper status cards into the draw pile and converts the first two Status draws from the draw pile each turn into draw 1 plus Energy 1.
- `morvi_blueprint_proof`: combat start gives 3 Proofread stacks; the first three non-Status player-played cards temporarily upgrade and draw 1 if unupgraded, or cost 1 less and grant 4 Block if already upgraded. Power cards are never extra-played.
- `morvi_debt_settlement`: immediately grants 220 Gold, removes up to two cards, upgrades two cards, sets Debt to 320, and pays `min(40, Debt)` at each combat end with Gold first and 3 HP per 10 Gold short rounded up while reducing Debt by the full due.

Next target:

- Live-test Morvi UI, gameplay, save/load, and co-op behavior before claiming private beta readiness. Replace source-derived temporary Morvi option/icon crops with bespoke generated relic-style art when Image API access or final user source files are available.

## ISSUE-2026-05-12-LOTHA-V22-PLANNING

Priority: P1
Status: source-complete / live-pending
Area: Act 3 Ancient / Lotha

Implemented v2.2 changes:

- Closed Court removes hand-limit +3 and now uses split turn resources: turn 1 draw 4 and gain 2 Energy; turn 4 draw 2 and gain 2 Energy.
- Deferred Verdict is now a turn-4 player-owned Verdict + draw 4 + Energy 4 + player-driven extra-play ruling; it does not auto-damage and no longer uses enemy Verdict as its main mechanic.
- Single Sentence now gives the first Attack/Skill each turn two additional plays, then caps later cards that turn at four.
- Mirror Rebuttal now chooses and marks one real non-Curse, non-Status deck card, moves the matching combat card to hand at combat start when needed, and resolves on the first play of that marked card.
- Public Evidence now uses source power-amount hooks to double non-damaging negative status stacks in both directions and manage Enlightenment. Weak, Vulnerable, and Frail count; Poison, damage-over-time, countdown damage, and source-proven damage/kill Debuffs are excluded. It no longer uses the old "target already has debuff, draw 1" surrogate.
- Power cards use replacement rewards instead of extra play/copy.
- Death Reprieve remains high-risk and uses the source-backed death-prevention path; live lethal-path testing remains pending.

Implementation source is present: Lotha has custom event art/background resources, option art, a default-on Act 3 insertion, disable/force gates, and all eight blessing ids. Runtime loader, UI, live gameplay, save/load, co-op, and lethal-path verification remain pending.

## ISSUE-2026-05-12-VAKUU-FIGHT-V22-PLANNING

Priority: P2
Status: hidden-by-default / source-dedicated / live-pending for the first single-player opt-in slice
Area: Act 3 special Vakuu option

Implemented first slice:

- Add a Vakuu fight option only when the explicit enable/force gate is set.
- The fight uses a dedicated Vakuu monster and encounter scene in source; live victory and restore behavior are still unproven.
- During the fight, after the normal player-turn hand draw on turns 1, 3, and 5, offer three Contract choices when source-safe.
- Victory offers 1/2/3 non-Vakuu Act 3 Ancient blessings from existing Act 3 Ancient reward pools plus custom Lotha option relics based on broken Stolen Vault locks, and each broken lock grants 50 Gold.
- Failure is still described as lethal by the option text; live failure/death verification is pending.
- Vakuu Contracts are source-backed as hidden 0-cost Skill token cards: Ethereal + Exhaust, hidden from the card library, and not normally generatable. Individual contracts either push lock breaking, reduce Blood Debt, or add higher-risk Blood Debt pressure.

Do not claim multiplayer, save/load, or death/failure readiness until live evidence exists.

## ISSUE-2026-05-12-URDA-V22-ALIGNMENT

Priority: P1
Status: source-complete / live-pending
Area: Urda current implementation vs v2.2 design

Current repo state:

- Urda is already default-on for private-beta testing.
- Active ids: `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`, `urda_trial_branch`, `urda_shallow_root_relic`, `urda_rooted_route`, `urda_after_rain`, `urda_root_sight`, and `urda_seed_bank`.
- These have source gameplay hooks, localization, option relic art, and source guard coverage; manual gameplay/save-load verification is still pending.

Next target:

- Live-test the ten-blessing Urda pool and replace the source-derived temporary option icons with bespoke generated relic-style art when image generation or final source art is available.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MILESTONE-GATES

Priority: P1
Status: open
Area: implementation gating

Current milestone rule:

- Each milestone needs source evidence, tests/guards, localization, player-facing text, art, and a live-verification stance before it can be called test-ready.
- No future milestone should start with another documentation-only audit unless it immediately drives implementation.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MORVI-LOTHA-ART-INTEGRATION

Priority: P1
Status: source-integrated / bespoke-icon pending
Area: Ancient event art

Approved direction:

- Morvi uses lender-scribe contract art at `EZMicroBalance/images/events/ezmb_morvi.png`.
- Lotha should use mirror-tribunal art at `EZMicroBalance/images/events/ezmb_lotha.png`.

Requirements:

- Copy only explicit, visually confirmed local source images.
- Do not promote unverified temp/cache files into active resources.
- Add PNGs to `export_presets.cfg` only after the source files are present.
- Active art must record prompt/source/hash and must not use official game assets.
- Morvi and Lotha option/icon art now uses the later GPTimage2/browser art pass for the current package; old temporary crop notes are historical only.


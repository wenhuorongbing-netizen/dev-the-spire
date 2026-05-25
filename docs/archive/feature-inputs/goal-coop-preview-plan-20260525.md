# Archived Goal Input: Co-op Preview Plan

Archived from docs/goal.md on 2026-05-25 because active goal guard files must stay compact.

- Archive long prompt dumps under `docs/archive/feature-inputs/`; see `goal-md-mojibake-intake-20260523.md`.
鍙互浠庢簮鐮佺骇瀹炵幇澶氫汉妯″紡鐢熸晥锛屼絾**涓嶈兘鍙槸鎶?multiplayer gate 鍒犳帀**銆傜幇鍦ㄧ殑闂€€/鏂嚎椋庨櫓鏉ヨ嚜涓ゅ眰锛?
1. **Transform preview 鏈韩鍦ㄥ浜洪噷娌℃湁鏉冨▉鍚屾绛栫暐**锛氬綋鍓?`TransformPreviewPatch` 浼氭寜鏈湴 RNG snapshot 鐢熸垚棰勬祴骞舵浛鎹㈠彸渚ч瑙堝崱锛屼絾瀹冩病鏈?host/client authority 鍒ゆ柇锛屼篃娌℃湁缃戠粶鍚屾銆?2. **澶氫汉閲岃繕鏈夊叾浠栨湭楠岃瘉鐨?reward/card-choice 鏀瑰姩鍦ㄨ繍琛?*锛氫緥濡?Seed Bank 浼氱粰 card reward 娣诲姞棰濆 alternative 骞舵墦寮€棰濆閫夊崱娴佺▼锛汧ission 浼氱洿鎺ヤ慨鏀?card reward options銆傝繖绫诲唴瀹逛細褰卞搷 vanilla 鐨?`PlayerChoiceSynchronizer` 鍜?reward choice index銆?
鎵€浠ユ纭矾绾挎槸锛?
> **鍏堣鈥滈鐭モ€濆湪澶氫汉閲屽彧鍋氬畨鍏ㄦ樉绀猴紝鍐嶆妸鎵€鏈変細鏀瑰彉濂栧姳/閫夌墝/缁撴灉鐨勫姛鑳芥敼鎴?host-authoritative銆?*

---

## 1. 鏈€灏忓彲琛岋細璁┾€滃彉鍖栭鐭モ€濆湪澶氫汉閲屽畨鍏ㄧ敓鏁?
杩欎釜鐗堟湰鍙В鍐充綘鐨勬牳蹇冭瘔姹傦細

```text
澶氫汉閲屾墦寮€ Aroma of Chaos / Astrolabe / New Leaf 绛夊彉鎹㈢晫闈㈡椂锛?褰撳墠鐜╁鍙互鐪嬪埌鑷繁杩欏紶鐗屼細鍙樻垚浠€涔堛€?```

瀹冧笉瑕佹眰鎵€鏈夌帺瀹堕兘鐪嬪埌瀵规柟鐨勯娴嬶紝涔熶笉鏀瑰彉瀹為檯 transform 缁撴灉銆?
### 鍋氭硶

褰撳墠浠ｇ爜鏄細

```csharp
TransformPreviewCyclePatch.PreparePredictions(...)
TransformPredictionRngContext.TryConsume(...)
TransformPredictionService.PredictReplacementModel(...)
```

瀹冨凡缁忚兘 fork RNG 棰勬祴锛屼笉鍒涘缓鐪熷疄鍗★紝涔熶笉娑堣€楃湡瀹?RNG銆?
浣嗗浜洪噷瑕佸姞涓変釜瀹夊叏鏉′欢锛?
```text
1. 鍙粰褰撳墠鏈湴鐜╁鑷繁鐨勫彉鎹㈢晫闈㈡樉绀恒€?2. 涓嶄负杩滅鐜╁銆佹梺瑙傜姸鎬併€侀潪鏈湴 UI 鐢熸垚 preview銆?3. 棰勬祴澶辫触鎴?source RNG 涓嶅彲淇℃椂锛屽洖閫€鍒?vanilla 杞挱銆?```

浼唬鐮侊細

```csharp
internal static bool CanShowLocalTransformPreview(Player owner)
{
    var runState = owner.RunState;

    if (runState.Players.Count <= 1)
        return true;

    // 杩欓噷瑕佺敤 v0.106 婧愮爜纭鈥滄湰鍦扮帺瀹垛€濈殑鍙潬鍒ゆ柇鏂瑰紡銆?    // 涓嶈鐚溿€傚彲浠ヤ粠 PlayerChoiceSynchronizer / RunManager / NetService 涓煡銆?    if (!SpirePlusLocalPlayerService.IsLocalPlayer(owner))
        return false;

    // 鍙厑璁?UI-only preview锛屼笉鍏佽淇敼 PlayerChoice銆丷eward銆丷NG銆?    return true;
}
```

鐒跺悗鍦?`PreparePredictions()` 鍓嶉潰鍔狅細

```csharp
var owner = transformations[0].Original.Owner;

if (!SpirePlusMultiplayerPreviewPolicy.CanShowLocalTransformPreview(owner))
{
    ReleaseEvidenceLog.Log(
        "PreviewTransform",
        "prediction_skipped_multiplayer_nonlocal",
        owner);

    return;
}
```

鍦?`TransformPredictionRngContext.Register()` 閲屼篃瑕佸姞鍚屾牱鍒ゆ柇锛岄伩鍏嶇粰杩滅鐜╁娉ㄥ唽閿欒 snapshot锛?
```csharp
if (!SpirePlusMultiplayerPreviewPolicy.CanRegisterTransformPreviewContext(player))
{
    ReleaseEvidenceLog.Log(
        "PreviewTransform",
        "rng_context_skipped_multiplayer_nonlocal",
        player,
        new Dictionary<string, object?>
        {
            ["source"] = sourceName
        });

    return;
}
```

杩欎釜鐗堟湰鐨勪紭鐐规槸锛?
```text
涓嶄細鏀瑰彉鐪熷疄缁撴灉
涓嶄細鏂板 PlayerChoice
涓嶄細鏂板 Reward
涓嶄細鍙戣嚜瀹氫箟缃戠粶娑堟伅
涓嶄細褰卞搷 host/client reward index
```

杩欏簲璇ユ槸绗竴闃舵鏈€绋崇殑瀹炵幇銆?
---

## 2. 浣嗚鍏堜慨澶氫汉闂€€锛氱鐢ㄦ湭鍚屾鐨?reward/card-choice 鏀瑰姩

浣犺鈥滃彉鍖栦細棰勮锛屼絾鏄浜洪棯閫€鈥濄€備粠鏃ュ織鐪嬶紝宕╂簝閾炬潯閲屽嚭鐜扮殑鏄?`PlayerChoiceSynchronizer.SyncLocalChoice -> CardReward.OnSelect`锛屽苟涓旀棩蹇椾腑鍚屼竴灞€澶氫汉閲屽凡缁忓彂鐢熶簡 Seed Bank 瀛樺崱鍜?Fission reward mutation銆?
褰撳墠 v0.106 婧愮爜瀹¤宸茬粡鎸囧嚭锛歚CardRewardAlternative` 浼氬弬涓?card reward 閫夋嫨锛屽苟涓?source 閲?card reward 鍚屾鐨勬槸涓€涓?card-or-alternative choice index锛涜嚜瀹氫箟 alternatives 蹇呴』閬靛畧 vanilla 鐢熷懡鍛ㄦ湡锛屽惁鍒欎細 soft-lock 鎴?duplicate reward銆?
鎵€浠ヨ璁╁浜虹ǔ瀹氾紝鍏堝姞杩欎簺 gate銆?
### 2.1 Seed Bank reward storage 澶氫汉 gate

褰撳墠 `TryAddSeedBankAlternative()` 娌℃湁澶氫汉 gate锛屼細鐩存帴鍔?alternative銆?
寤鸿鏀癸細

```csharp
private static bool TryAddSeedBankAlternative(
    Player player,
    CardReward cardReward,
    List<CardRewardAlternative> alternatives)
{
    if (!MultiplayerFeaturePolicy.IsSingleplayer(player.RunState))
    {
        MultiplayerFeaturePolicy.LogCoopEvidence(
            "UrdaSeedBank",
            "store_alternative_gated",
            player.RunState,
            new Dictionary<string, object?>
            {
                ["reason"] = "Seed Bank reward storage adds an extra card reward alternative and is pending multiplayer PlayerChoice sync proof."
            });

        return false;
    }

    ...
}
```

`ChooseSeedBankStore()` 涔熻鍔犻槻寰★細

```csharp
private static async Task ChooseSeedBankStore(Player player, CardReward reward)
{
    if (!MultiplayerFeaturePolicy.IsSingleplayer(player.RunState))
    {
        MultiplayerFeaturePolicy.LogCoopEvidence(
            "UrdaSeedBank",
            "store_choice_gated",
            player.RunState);

        return;
    }

    ...
}
```

娉ㄦ剰锛氱幇鍦ㄥ彧 gate 浜?Seed Bank relic click extraction锛屼絾 reward storage 浠嶇劧鍦ㄥ浜洪噷杩愯銆?
### 2.2 Fission reward mutation 澶氫汉 gate

褰撳墠 `TryApplyFission()` 浼氬湪 card reward 鐢熸垚鏃朵慨鏀?reward options锛屾病鏈夊浜?gate銆?
寤鸿鏀癸細

```csharp
private static bool TryApplyFission(
    Player player,
    List<CardCreationResult> cardRewardOptions,
    CardCreationOptions creationOptions)
{
    if (!MultiplayerFeaturePolicy.IsSingleplayer(player.RunState))
    {
        MultiplayerFeaturePolicy.LogCoopEvidence(
            "AscensionFission",
            "reward_mutation_gated",
            player.RunState,
            new Dictionary<string, object?>
            {
                ["reason"] = "Fission mutates card reward options and is pending multiplayer reward sync proof."
            });

        return false;
    }

    ...
}
```

### 2.3 鍏朵粬 reward mutation 涔熻鍚岀被澶勭悊

鍑℃槸鏀硅繖浜涗笢瑗跨殑鍔熻兘锛岄兘蹇呴』鍏?gate锛?
```text
CardRewardAlternative
CardReward options
CardReward.Populate
RewardSet reward list
extra reward option
extra card selection screen
Prismatic Gem reward reroll
Boss reward extra option
Firemarked Elite reward extra option
Deep Branch reward mutation
```

鍥犱负杩欎簺閮藉彲鑳藉奖鍝?vanilla reward index / PlayerChoice id銆?
褰撳墠椤圭洰宸茬粡鏈?`MultiplayerFeaturePolicy`锛屽彲浠ュ鐢ㄥ畠鍋?gate 鍜?evidence log銆?
---

## 3. 瀹屾暣澶氫汉瀹炵幇锛歨ost-authoritative preview sync

濡傛灉鐩爣涓嶆槸鈥滄湰鍦扮帺瀹惰嚜宸辩湅鈥濓紝鑰屾槸锛?
```text
host 鍜?client 閮借兘绋冲畾鐪嬪埌鍑嗙‘棰勬祴锛?client 鐨勯娴嬪拰 host 鏈€缁堟墽琛岀粨鏋滄案杩滀竴鑷达紱
涓嶄細鍥犱负 RNG drift 鎴?PlayerChoice 椤哄簭涓嶅悓瀵艰嚧閿欑墝/鏂嚎銆?```

閭ｅ氨蹇呴』鍋氬畬鏁寸殑 host-authoritative 鏂规銆?
### 鏍稿績鍘熷垯

```text
host 鏄敮涓€棰勬祴鏉冨▉
client 涓嶇嫭绔嬪喅瀹?gameplay-relevant 缁撴灉
client 鍙樉绀?host 鍙戞潵鐨?prediction
actual transform 浠嶇敱 host/vanilla authoritative flow 鎵ц
preview 鍜?actual 蹇呴』鏈夊悓涓€涓?snapshot id
```

### 鏁版嵁缁撴瀯

鏂板锛?
```text
EZMicroBalanceCode/Multiplayer/PreviewSync/
  TransformPreviewSyncService.cs
  TransformPreviewPredictionRecord.cs
  TransformPreviewPredictionKey.cs
```

`PredictionKey` 鑷冲皯鍖呭惈锛?
```csharp
internal readonly record struct TransformPreviewPredictionKey(
    int ActIndex,
    int Floor,
    int RoomIndex,
    int PlayerSlot,
    string SourceName,
    uint Seed,
    int Counter,
    string OriginalCardId,
    int OriginalCardInstanceId);
```

`PredictionRecord`锛?
```csharp
internal sealed record TransformPreviewPredictionRecord(
    TransformPreviewPredictionKey Key,
    string ReplacementCardId,
    bool UpgradedPreview,
    string SourceName,
    uint Seed,
    int Counter);
```

### Host 娴佺▼

```text
1. 鐜╁杩涘叆 transform selection銆?2. host 鎹曡幏 source RNG seed/counter銆?3. host 鐢?fork 棰勬祴 replacement model銆?4. host 鎶?prediction record 瀛樿繘 cache銆?5. host 鍙?prediction 缁?client銆?6. UI 鏄剧ず prediction銆?7. 鐜╁纭銆?8. host 鎵ц vanilla transform銆?9. host 楠岃瘉 actual result == prediction銆?10. 濡傛灉涓嶄竴鑷达紝鍐?ERROR evidence log锛屽苟绂佺敤鏈眬 preview銆?```

### Client 娴佺▼

```text
1. client 鎵撳紑 transform preview銆?2. client 涓嶇嫭绔嬫秷鑰?鎺ㄦ柇 gameplay RNG銆?3. client 绛?host prediction锛屾垨鐭殏 fallback 鍒?vanilla 杞挱銆?4. 鏀跺埌 prediction 鍚庢樉绀恒€?5. final result 浠?host/vanilla 鍚屾缁撴灉涓哄噯銆?```

### 涓轰粈涔堜笉寤鸿 client 鑷繁绠?
鍥犱负澶氫汉閲屽緢瀹规槗鍑虹幇杩欎簺鎯呭喌锛?
```text
client RNG counter 鍜?host 涓嶅悓
event 鏄?non-shared event
host/client reward alternatives 涓嶅悓
杩滅閫夋嫨鎻愬墠/寤跺悗
UI 閫€鍑哄鑷?choice id 涓嶄竴鑷?```

浣犱笂浼犵殑鏃ュ織閲岋紝Aroma of Chaos 鏄細

```text
shared: False
```

鑰屼笖 host/client 鍒嗗埆閫夋嫨浜嗕笉鍚?event option銆傝繖涓満鏅壒鍒鏄撳嚭鍚屾闂銆?
---

## 4. 濡傛灉瑕佸仛鍒扳€滄渶缁堢粨鏋滀篃琚鎻愪氦鈥濓紝闇€瑕佹洿娣?patch

褰撳墠 preview 鏄€滅湅涓€鐪硷紝涓嶆敼鍙樼粨鏋溾€濄€傚鏋滀綘鎯冲仛鍒板浜洪噷 100% 鍑嗙‘锛屾渶寮烘柟妗堟槸锛?
```text
host 鍦?preview 鏃剁敓鎴?committed replacement
纭 transform 鏃剁洿鎺ヤ娇鐢?committed replacement
```

浣嗚繖鏈変竴涓噸瑕侀棶棰橈細

```text
濡傛灉 preview 鏃跺氨娑堣€楃湡瀹?RNG锛岀帺瀹跺彇娑堥€夋嫨浼氫笉浼氫篃娑堣€?RNG锛?```

鏈変袱绉嶉€夋嫨銆?
### 鏂规 A锛氫笉娑堣€楃湡瀹?RNG锛屽彧楠岃瘉

```text
preview 鐢?fork
confirm 鏃?vanilla 姝ｅ父 roll
roll 鍚庨獙璇?actual == predicted
```

浼樼偣锛?
```text
涓嶆敼鍙?RNG 璇箟
鍙栨秷 preview 涓嶅奖鍝嶆湭鏉?```

缂虹偣锛?
```text
鐞嗚涓婂鏋滀腑闂存湁鍚屼竴 RNG source 琚叾浠栫郴缁熸秷鑰楋紝鍙兘 mismatch
```

杩欎釜鏂规閫傚悎绗竴鐗堛€?
### 鏂规 B锛氶鎻愪氦骞堕攣瀹?
```text
preview 鏃?host 娑堣€楃湡瀹?RNG
瀛?committed replacement
confirm 鏃剁洿鎺ヤ娇鐢?committed replacement
cancel 鏃?replacement 浠嶄繚鐣欐垨浣滃簾浣?RNG 宸叉秷鑰?```

浼樼偣锛?
```text
缁撴灉 100% 涓?preview 涓€鑷?```

缂虹偣锛?
```text
鏀瑰彉 vanilla RNG 娑堣€楁椂鏈?鍙栨秷/閲嶅紑閫昏緫澶嶆潅
save/load 鍜?co-op 瑕侀澶栧悓姝?```

杩欎釜鏂规鏇村儚鈥滃弽 SL 璁捐鈥濓紝涓嶆槸绠€鍗?UI preview銆?
寤鸿鍏堝仛 A锛岀瓑澶氫汉绋冲畾鍚庡啀璁ㄨ B銆?
---

## 5. 褰撳墠鏈€绋崇殑寮€鍙戦『搴?
### Phase 1锛氬厛姝㈣

鐩爣锛氬浜轰笉闂€€銆?
```text
1. 澶氫汉 gate Seed Bank reward storage
2. 澶氫汉 gate Fission reward mutation
3. 澶氫汉 gate 鍏朵粬 card reward alternative / reward option mutation
4. 鍙樻崲 preview 鍙厑璁告湰鍦扮帺瀹?UI-only preview
5. 鏀堕泦 host + client 鍙屾棩蹇?```

杩欎竴姝ュ畬鎴愬悗锛孉roma of Chaos 澶氫汉搴旇涓嶅啀鍥犱负 reward/card choice 鍚屾宕┿€?
### Phase 2锛氭湰鍦板浜?preview

鐩爣锛氬浜洪噷姣忎釜鐜╁鑳界湅鑷繁鐨?transform preview銆?
```text
1. 澧炲姞 SpirePlusLocalPlayerService
2. 鍙鏈湴鐜╁ NTransformPreview 鐢熸晥
3. 闈炴湰鍦?player / remote event 涓嶆敞鍐?RNG context
4. mismatch 鑷姩 fallback
5. evidence log 璁板綍 prediction_prepared / prediction_displayed / actual_verified
```

### Phase 3锛歨ost-authoritative preview sync

鐩爣锛歨ost/client 閮借兘鐪嬪埌鍚屼竴涓噯纭?prediction銆?
```text
1. 瀹炵幇 TransformPreviewPredictionRecord
2. 瀹炵幇 host prediction cache
3. 瀹炵幇 client request / host response
4. 娌℃湁 response 灏?fallback
5. actual transform 鍚庨獙璇?prediction
```

杩欎竴姝ラ渶瑕佹繁鍏ョ爺绌?vanilla multiplayer message system銆侭aseLib 鏈韩涓昏璐熻矗 Mod 妗嗘灦銆侀厤缃€佽祫婧愬拰妯″瀷娉ㄥ唽锛涘浜哄悓姝ヨ璐寸潃 STS2 鐨?`PlayerChoiceSynchronizer`銆乣RunLocationTargetedMessageBuffer`銆乣NetMessageBus`銆乣StartRunLobby` 绛夋簮鐮佽蛋锛屼笉鑳藉彧闈?BaseLib 閰嶇疆灞傝В鍐炽€倂0.106 婧愮爜瀹¤涔熸彁閱掞細澶氫汉 join銆丮odelDb hash銆丼tartRunLobby ascension cap 绛夐兘蹇呴』鍜?vanilla 婧愮爜涓€鑷达紝涓嶈兘鐢ㄦ棫鍋囪銆?
### Phase 4锛氭仮澶?reward mutation 鐨勫浜烘敮鎸?
绛?transform preview 绋冲畾鍚庯紝鍐嶉€愪釜鎭㈠锛?
```text
Seed Bank
Fission
Prismatic Gem
Firemarked Elite reward
Boss reward option
Deep Branch reward
```

姣忎釜閮借鍋氾細

```text
host 鐢熸垚 reward plan
client 鎺ユ敹 reward plan
reward screen option index 涓€鑷?PlayerChoice id 涓€鑷?actual result 鐢?host 楠岃瘉
save/load restore
```

---

## 6. 缁?Codex / 寮€鍙戣€呯殑鍏蜂綋瀹炵幇浠诲姟

鍙互鐩存帴杩欐牱涓嬩换鍔★細

```text
瀹炵幇 Spire Plus multiplayer-safe preview pass銆?
瑕佹眰锛?
1. 涓嶈鐩存帴寮€鏀炬墍鏈?multiplayer preview銆?2. 鍏堜慨澶嶅浜?reward/card choice 鍚屾椋庨櫓銆?3. Seed Bank reward storage 鍦ㄥ浜轰腑 gate銆?4. Fission reward mutation 鍦ㄥ浜轰腑 gate銆?5. 鎵€鏈?CardRewardAlternative / CardReward option mutation 蹇呴』閫氳繃 MultiplayerFeaturePolicy銆?6. Transform preview 鍦ㄥ浜轰腑鍙鏈湴鐜╁ UI-only 鐢熸晥銆?7. Remote player / nonlocal event 涓嶆敞鍐?TransformPredictionRngContext銆?8. 澧炲姞 PreviewMultiplayerPolicy / LocalPlayerService銆?9. 棰勬祴缁撴灉浠呮樉绀猴紝涓嶆柊澧?PlayerChoice锛屼笉鏂板 Reward锛屼笉鍙?vanilla choice銆?10. 澧炲姞 evidence logs锛?   - prediction_skipped_multiplayer_nonlocal
   - prediction_prepared_multiplayer_local
   - prediction_displayed
   - prediction_actual_verified
   - prediction_mismatch_fallback
11. 澧炲姞娴嬭瘯锛?   - MultiplayerTransformPreviewDoesNotRegisterRemotePlayerContext
   - MultiplayerTransformPreviewDoesNotMutateRewardOrPlayerChoice
   - SeedBankRewardStorageIsGatedInMultiplayer
   - FissionRewardMutationIsGatedInMultiplayer
   - CardRewardMutationsRequireMultiplayerPolicy
12. 璺?BaseLib + Spire Plus 鍙屽鎴风 Aroma of Chaos 娴嬭瘯銆?13. 杈撳嚭 host/client 涓や唤 godot.log銆?```

---

## 7. 鐜板湪涓嶈鍋氱殑浜?
涓嶈杩欐牱锛?
```text
鍒犻櫎 multiplayer gate锛岀劧鍚庤鎵€鏈?preview/reward mutation 鍦ㄥ浜洪噷鐩存帴璺戙€?```

涔熶笉瑕佽繖鏍凤細

```text
client 鑷繁绠?prediction锛岀劧鍚庡亣璁?host 缁撴灉涓€瀹氫竴鏍枫€?```

涔熶笉瑕佽繖鏍凤細

```text
璁?Seed Bank / Fission / Prismatic Gem / reward options 鍦ㄥ浜轰腑缁х画鐩存帴淇敼 card reward銆?```

杩欎笁绉嶅仛娉曢兘浼氱户缁Е鍙?PlayerChoice 鍚屾闂銆?
---

## 鏈€缁堝洖绛?
**鑳戒粠婧愮爜绾у疄鐜板浜烘ā寮忕敓鏁堛€?*

浣嗘纭仛娉曚笉鏄€滃紑鍏虫墦寮€鈥濓紝鑰屾槸鍒嗕袱姝ワ細

```text
绗竴姝ワ細澶氫汉閲屽彧鍏佽鏈湴 UI-only transform preview锛屽悓鏃?gate 鎺夋墍鏈夋湭鍚屾 reward/card-choice 鏀瑰姩锛屽厛瑙ｅ喅闂€€銆?绗簩姝ワ細瀹炵幇 host-authoritative preview sync锛岃 host 鐢熸垚 prediction銆乧lient 鎺ユ敹鏄剧ず銆佹渶缁堢粨鏋滈獙璇佷竴鑷淬€?```

鐩墠鏈€搴旇鍏堟敼鐨勬槸锛?
```text
Seed Bank reward storage multiplayer gate
Fission reward mutation multiplayer gate
TransformPredictionRngContext remote/nonlocal player gate
TransformPreviewPatch local-player-only preview
host/client 鍙屾棩蹇楀鐜?```

杩欐牱澶氫汉閲屸€滃彉鍖栭瑙佲€濆彲浠ュ畨鍏ㄧ敓鏁堬紱涔嬪悗鍐嶉€愭鎭㈠瀹屾暣 Spire Plus 澶氫汉鐜╂硶銆?

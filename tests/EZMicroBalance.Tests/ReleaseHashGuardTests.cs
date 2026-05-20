using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseHashGuardTests
{
    [Fact]
    public void CurrentStatusDocsUseLatestPackageHashes()
    {
        const string oldZipHash = "0A3C20E20A1B35D2CD41C00BD92E6D91278303983CC9571660A3540BA9B47342";
        const string stalePostPolishZipHash = "2F29C8AC8D4A03398246E34E5C58DA0E5EEC31EDB8656D21AA898BBE26C64612";
        const string staleRootSightUiZipHash = "933235A60DD3D82B3EC76FCF066B034AA48908B5BCB959F4A117480D4E3B6C5D";
        const string staleRootSightMarkerZipHash = "FF1543DA01A050F4461ED9510EE5E7CC7CD904B9D9910BA57F75E70CB46145F3";
        const string staleRootSightMarkerRestoreZipHash = "A8FC73668CC8754BD2266CDBF56CA212D6E48354251AF92DFE32AF1C15881EDA";
        const string staleMorviSplitZipHash = "149A61EA9E802A8FDFFE895B4625FF200310711DEC1EB9C6E33A5A01453D4B27";
        const string staleLothaEchoVerdictZipHash = "2F4CD24C2A126461AD6DB13D2E13663B3685EE9C64C9674CA918118A6E9187A1";
        const string staleRootSightEncounterZipHash = "FB08B1D016F3264B608AF3C6482C751A8EB7EC8D966280505E45FFB6CD15989D";
        const string staleRootSightEncounterPckHash = "15D7380AD87E5B22759226684BD8A6310CCBE7D8C45F9B7156671A07A53073AD";
        const string staleRootSightHoverZipHash = "415FFA771EB2C1787EEE081AC3F01C14BC9104AAC0733DCDF91E19193AD4C046";
        const string staleLothaPresumptionClosedCourtZipHash = "9AA682B537E5328BB52620427D2185C196F5FB8AC432FE95964EB19CB0F78B9C";
        const string staleMorviOpenBookPaperstormZipHash = "D7A71A30DCB303DD6BF02F7F945868180635CE7780C4550F79AC7D9B38836784";
        const string staleRootSightQueueZipHash = "C29E8D79494B4D94AFDBF804B9F8EB5E1DE77B57F23584628F2FE120842DEB1D";
        const string stalePreStrictReviewZipHash = "952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B";
        const string staleStrictReviewZipHash = "9F0C09FDF1F498567CCDA891D6A7AE2527885E2AD5281B53DF3123E894656E48";
        const string staleVakuuSplitZipHash = "F062EB5CDBA92C12D76C05E639A8B0F7B6404B4010CD5A0568F5A94CAC5C2F59";
        const string staleRootBudHelperSplitZipHash = "7CC56CE1E432BC4E4FA523566AED729ED1232207B4885F76F541A822815FAA20";
        const string staleA11SerializableHelperSplitZipHash = "8D22B85BA8FAD4C44218C3DA51A12AE017A596178B17C1F657BCC68E6E435CD7";
        const string staleRootSightRngAuditZipHash = "E7E7B7244DE0D920D471B13E708738AD485CFFE2C3032B619A3E0D3716AE7D34";
        const string oldDllHash = "4A30ACE329A110703ECC02586940B14D435E31CD9D2AB40D009B2B26A8FC0212";
        const string staleRootSightUiDllHash = "ED3E2A7E4A2FA0129082B424D33B5211583377CF909AFE540B67059FFEAA7663";
        const string staleRootSightMarkerDllHash = "52D69F077EA07F1D28E2671E78AE1C75AF46AE72B56596D1EB1849ED7402E155";
        const string staleRootSightMarkerRestoreDllHash = "146BA5B5AC66CC11F4B1031D0A94EE947E4891973B3674C2B967B01319D12167";
        const string staleMorviSplitDllHash = "9747BE773C08E5D90414EA2CA2A94ADF83C9A2F7854BAAFB7472893F14C1DA28";
        const string staleLothaEchoVerdictDllHash = "A771ACA2B38CF23B3F9DE22993AB2444D79CAAB2867CAD8DC9320FF9764A1585";
        const string staleRootSightHoverDllHash = "6BC80A5C7ABAABB70DC088DDFD18E2692908A293365C67E860780CDD1D9F5652";
        const string staleLothaPresumptionClosedCourtDllHash = "0BFD4E24C8C417A99C76032C16046B2D6F85CB23576623D4158755533DDEA09A";
        const string staleMorviOpenBookPaperstormDllHash = "A032DD7A2E0DFA9015807FB25E9C816CD01DB630D37994577353472C0D84C6D5";
        const string staleRootSightQueueDllHash = "87F6B2168C33E412061A00B640915A6C171C4FF0D84B78C55F26AD747B032BCC";
        const string stalePreStrictReviewDllHash = "6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542";
        const string staleStrictReviewDllHash = "20749CC0C0763BBA00DB4521719556D3C17AB039C94BE30F9DF560C2A2A8D243";
        const string staleVakuuSplitDllHash = "D8BA52CDF946287C9FFA813727C8640829C00338AD224642C27D28759CD693FC";
        const string staleRootBudHelperSplitDllHash = "4BD0058BEA3E3EB785F67DE3DA38B2E7D63FD29C3010998C6053AEAD78BC381F";
        const string staleA11SerializableHelperSplitDllHash = "5A97D0A1B6F811F890E538282B9A2FD341E2BD3B68F36A6F267A1EA07BD11F4F";
        const string staleRootSightRngAuditDllHash = "C83AE141F0F4BF4A8057CC22704A1039EA69522A76FA05764C14BA8F8CBEBD6D";
        const string staleStrictReviewClosureZipHash = "8FCDFEA0618A97CCACBEC236F1CF8E25683C43CAC28AED0D8709ED13B5914644";
        const string staleStrictReviewClosureDllHash = "6CDFCF16F9CAF2AC8EC53B2362422BC2E4ED05668EB61374AE384FFED9A5402E";
        const string staleStrictSourceAuditZipHash = "C2577D304566277D38CF8A1A3FABC324F55F568D998B9DB1EFB47BC9D6A97B85";
        const string staleStrictSourceAuditDllHash = "4CBAD7AA58850BDA89DA9DF5AD0261A42200A0CAEA9704637398E2B747A50A8C";
        const string staleRootDeckNoticeSplitZipHash = "9A57756A2EB9A9911C72F440F531E97B073E12BBBBAB966AC7D289BA7A91AAC1";
        const string staleRootDeckNoticeSplitDllHash = "642186C336B09091287FD948745557B4EE194502EC3F4ACE1CF04A938FFD428D";
        const string staleLothaCombatLifecycleSplitZipHash = "93D33814DACAE626F20E6DAB5B679627FDF7A8888173E016F9CFBF15F6F19106";
        const string staleLothaCombatLifecycleSplitDllHash = "2603226BF6EAB219812A32B3592E795776178D0601F3D5773833FCB8680FB6C2";
        const string staleRootDeckDeckCardsSplitZipHash = "A6993D24314D9EA911748F73FFADADAD3BD39EF4669C0F9A9045D30EBD2E0DE3";
        const string staleRootDeckDeckCardsSplitDllHash = "868CFFFE8971793760C1F379C4C65FC8FFDA91223EF44DD64D331E2E3B7096E5";
        const string staleRootDeckPendingDowngradeSplitZipHash = "DCE2DCCFFAA4B502A7592572CAC07B3ADC3FCC2BC0982F7A72121A159B20EFE8";
        const string staleRootDeckPendingDowngradeSplitDllHash = "FF38B31BB4B9A1B85D245D9A8747319164C42C152306D3C8EE0CD71C895976A8";
        const string staleBannerPressingLineSplitZipHash = "3A17687E248C1F201397DC36E5E5042B3EE22AACD58A713D40AD82536DDE78C7";
        const string staleBannerPressingLineSplitDllHash = "9E16B0EE747FB16BF4AB84BAD6E8460710E470C51BAEFDE17DBD7C97FC1A5472";
        const string staleVakuuCombatStateSplitZipHash = "F9077D752D38C91566C84E9CB78D642E98366AB2ADFAE33C7C05582DFE2B773C";
        const string staleVakuuCombatStateSplitDllHash = "3A70790EE2978FC7FC0BA1C8BEC3CC83F4AE05AA8FB44B2E5325EA624EFC3E10";
        const string stalePrismaticGemHoverSplitZipHash = "084976FDD7338D9939E9D1BF358195E536E89441123D462E00EF609E324015F2";
        const string stalePrismaticGemHoverSplitDllHash = "2A0B5FE6BD194B7BDECD4CB095DCED2166D509FDB7CEF1098650B1F804EA3FDC";
        const string stalePreMorviDebtCounterZipHash = "29F45FADB9187AB774F72F8A28481FB08B4B48B4D00975C0879E51EB6EBE4496";
        const string stalePreMorviDebtCounterDllHash = "522FDA8AE0D43C1F9E92FBCCA6EB1BAFFFEF1D02D179F3FBD063A63B86A1F929";
        const string stalePreSeedBankFailureGuardZipHash = "F4BDA95224E8C88CA5BA79CBFF4BCB008893F29D89B5199F9B46EA1CEC3934F3";
        const string stalePreSeedBankFailureGuardDllHash = "6F517FA2F2CA17FAA75AEB5EE0A22F73D2F96C0ECB8714A81D71685E0FBCA462";
        const string stalePreAncientOptionHoverRootSightGuardZipHash = "2FA029B854BFB62279E697D59A3CACA1328AE31788D3826FFEF1637CE9146F05";
        const string stalePreAncientOptionHoverRootSightGuardDllHash = "D919528913CCE98A0F3C8783227E8519E520B162932B44422C7C77A2578C5592";
        const string stalePreRootSightMapScreenGuardZipHash = "54590E9C41F4F3A5E711D0399CF5741E3558534B8DED043D18306480E48018D6";
        const string stalePreRootSightMapScreenGuardDllHash = "BC5548DA62C76D0B09C37F4881B09D6756510357C6F47F3F4CDF8B861122459F";
        const string stalePreCombatLifecycleStateSourceZipHash = "06926BC7000297930FB11DC299C3D0AAD6AD7C7C6C3A030BAB2525A3B0CB3387";
        const string stalePreCombatLifecycleStateSourceDllHash = "8D3383DBA9957EEC43C764C2199798CA655C5CE422262EE64399B479611D7BE5";
        const string stalePreVakuuSceneSeedStateZipHash = "88B06C76BAC7880DCE5F5C512AFD1451D5A88F3DE21E09DD80882BC8D33E5316";
        const string stalePreVakuuSceneSeedStateDllHash = "D12D7D3D6DBFF9D559B54D0990CF79BC0C25632C52A6C380A422778AE9847542";
        const string stalePrePickupRewardServiceSplitZipHash = "98A7319815D3A373DC352AE0FB63D958195C45673B9AB6E73D5C753BEAE9DF30";
        const string stalePrePickupRewardServiceSplitDllHash = "483B3091EFD21BD9264EE14D998172E685ECFED2BCFACE7606007AF6C50FE200";
        const string stalePreRootblightCardSplitZipHash = "76E3213FE91FD0180EA28FAB5770E12321BF33E70DB667D5B8249B1E13ADB7C8";
        const string stalePreRootblightCardSplitDllHash = "60BEC9E768ABD96E148500A733D844786431FDC4066A7BF5D62B7FB6C132D216";
        const string stalePreAscensionRewardServiceSplitZipHash = "FF066562EFA816E41A75B7A303C5789D0A4560F9709CDF5153BD2518A5521EA9";
        const string stalePreAscensionRewardServiceSplitDllHash = "796902BF93DBAADEDF01550AAB871A3F154D8807F207AF9D674DD1C9D39E0755";
        const string stalePreForgeTokenVisibleRelicSplitZipHash = "619C74BC06CCC2757BD5CEAA1B2125177ED71EDA8C3D370A8BA2BC315760A916";
        const string stalePreForgeTokenVisibleRelicSplitDllHash = "A638AFAB07527377CF41529D54F93126D9FF1458F06B60FAAC3E827EFB67E81E";
        const string stalePreMarkerOrderingSplitZipHash = "3899E386C5F81AED2F2CB0143F71D64642DDD24B6136EDD05864AE2C38864C38";
        const string stalePreMarkerOrderingSplitDllHash = "A9DBC0B7FA71CC6EC0F76CE4BE8159339F9DEAB402C7517C676EEDE45DB30246";
        const string stalePreStrictSourceResourceAuditZipHash = "B30547BB7CF203B5A19217204EA6AAAC2569DE6DDFEFE74A80F696DD1D817441";
        const string stalePreStrictSourceResourceAuditDllHash = "1FDDAE2B580C382B1E23588D608FFF793A84D4707D26836CB4FA28C2733CD00A";
        const string stalePreStrictSourceResourceAuditPckHash = "B3EC1B0ECD6636997B5C50E539EB6098FB08A9899C2DA98C05470029CE1BB7FA";
        const string currentZipHash = "CE417F595E2CCE8435C0575D95A3A866CBDA8FD605DE3F40014639E9301EFF62";
        const string currentDllHash = "940F1FEA66B01CB54A1CCEC388D4F023693C947395C7B7F9922BF596A8586E1E";
        const string currentPckHash = "3CDB72F1225FF2492F536091772979983653865F2902E2B485BBCB16B4FD1392";
        const string currentManifestHash = "A41EBF8ABEDCFC09DBB02CB655D7E50465888065ABA77F8EF087E87206F276CF";
        const string currentReadmeHash = "BA885193452EBA22A78433304F383A87A0830FA5E935A20B63BBAA08ABEBB906";

        var currentStatusDocs = new[]
        {
                ReadRepoText("PROJECT_STATE.md"),
                ReadRepoText("docs", "issues.md"),
                ReadRepoText("docs", "dev-environment.md"),
                ReadRepoText("docs", "private-beta-verification-handoff.md"),
                ReadRepoText("docs", "private-beta-release-completion-audit.md"),
                ReadRepoText("docs", "release-checklist.md"),
                ReadRepoText("docs", "test-ready-completion-audit.md")
            };

        foreach (var doc in currentStatusDocs)
        {
            Assert.DoesNotContain(oldZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePostPolishZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightUiZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightMarkerZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightMarkerRestoreZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleMorviSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleLothaEchoVerdictZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightEncounterZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightEncounterPckHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightHoverZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleLothaPresumptionClosedCourtZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleMorviOpenBookPaperstormZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightQueueZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreStrictReviewZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleStrictReviewZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleVakuuSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootBudHelperSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleA11SerializableHelperSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightRngAuditZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleStrictReviewClosureZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleStrictSourceAuditZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootDeckNoticeSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleLothaCombatLifecycleSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootDeckDeckCardsSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootDeckPendingDowngradeSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleBannerPressingLineSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleVakuuCombatStateSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePrismaticGemHoverSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreMorviDebtCounterZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreSeedBankFailureGuardZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreAncientOptionHoverRootSightGuardZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreRootSightMapScreenGuardZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreCombatLifecycleStateSourceZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreVakuuSceneSeedStateZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePrePickupRewardServiceSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreRootblightCardSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreAscensionRewardServiceSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreForgeTokenVisibleRelicSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreMarkerOrderingSplitZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreStrictSourceResourceAuditZipHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreStrictSourceResourceAuditPckHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(oldDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightUiDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightMarkerDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightMarkerRestoreDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleMorviSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleLothaEchoVerdictDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightHoverDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleLothaPresumptionClosedCourtDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleMorviOpenBookPaperstormDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightQueueDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreStrictReviewDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleStrictReviewDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleVakuuSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootBudHelperSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleA11SerializableHelperSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootSightRngAuditDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleStrictReviewClosureDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleStrictSourceAuditDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootDeckNoticeSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleLothaCombatLifecycleSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootDeckDeckCardsSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleRootDeckPendingDowngradeSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleBannerPressingLineSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(staleVakuuCombatStateSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePrismaticGemHoverSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreMorviDebtCounterDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreSeedBankFailureGuardDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreAncientOptionHoverRootSightGuardDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreRootSightMapScreenGuardDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreCombatLifecycleStateSourceDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreVakuuSceneSeedStateDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePrePickupRewardServiceSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreRootblightCardSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreAscensionRewardServiceSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreForgeTokenVisibleRelicSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreMarkerOrderingSplitDllHash, doc, StringComparison.Ordinal);
            Assert.DoesNotContain(stalePreStrictSourceResourceAuditDllHash, doc, StringComparison.Ordinal);
        }

        AssertSourceContains(
            ReadRepoText("docs", "issues.md"),
            currentZipHash,
            currentDllHash,
            currentPckHash,
            currentManifestHash,
            currentReadmeHash);
    }
}

const sourceRelicIcons = {
  "VELVET_CHOKER.description": "velvet_choker.png",
  "DISTINGUISHED_CAPE.description": "distinguished_cape.png",
  "PRISMATIC_GEM.description": "prismatic_gem.png",
  "JEWELRY_BOX.description": "jewelry_box.png",
  "PRESERVED_FOG.description": "preserved_fog.png",
  "CLAWS.description": "claws.png",
  "CHOICES_PARADOX.description": "choices_paradox.png",
  "JEWELED_MASK.description": "jeweled_mask.png",
  "PAELS_HORN.description": "paels_horn.png",
  "BLACK_STAR.description": "black_star.png",
  "WAR_HAMMER.description": "war_hammer.png",
  "PAELS_TOOTH.description": "paels_tooth.png",
  "SEAL_OF_GOLD.description": "seal_of_gold.png",
  "SOZU.description": "sozu.png",
  "ECTOPLASM.description": "ectoplasm.png",
  "FIDDLE.description": "fiddle.png",
  "IRON_CLUB.description": "iron_club.png",
  "BRILLIANT_SCARF.description": "brilliant_scarf.png",
  "BEAUTIFUL_BRACELET.description": "beautiful_bracelet.png",
  "MUSIC_BOX.description": "music_box.png",
  "CROSSBOW.description": "crossbow.png",
  "TOASTY_MITTENS.description": "toasty_mittens.png",
  "WHISPERING_EARRING.description": "whispering_earring.png",
  "MEAT_CLEAVER.description": "meat_cleaver.png",
  "BLOOD_SOAKED_ROSE.description": "blood_soaked_rose.png",
  "SERE_TALON.description": "sere_talon.png"
};

const ownedRelicIcons = {
};

const currentReadableGuardSnippets = [
  "一个更好的《杀戮尖塔 2》拓展",
  "新增进阶 A20 和先古之民修改",
  "A better Slay the Spire 2 expansion",
  "New A20 Ascension and Ancient changes",
  "若合适房间不足，至少放入2个",
  "可附魔牌",
  "If there are not enough suitable rooms, at least 2 are placed.",
  "Only Common, Uncommon, or Rare Attacks and Skills can receive Fission.",
  "Vakuu's Sere Talon",
  "Tanx Claws",
  "On pickup, choose 1 of 4 Curses. Add it, 2 Wish, and 1 Wish+.",
  "On pickup, transform up to 6 cards into Maul."
];

const vanillaIconPaths = {
  relic: "assets/vanilla-icons/relic.svg",
  curse: "assets/vanilla-icons/curse-card.svg",
  event: "assets/vanilla-icons/event-card.svg",
  cards: {
    brightestFlame: "assets/source-art/card_portraits/event/brightest_flame.png",
    folly: "assets/source-art/card_portraits/curse/folly.png",
    enthralled: "assets/source-art/card_portraits/curse/enthralled.png"
  }
};

const sourceCardOverrides = {
  "BRIGHTEST_FLAME.description": {
    icon: vanillaIconPaths.cards.brightestFlame,
    vanilla: "获得2点能量，抽2张牌，失去1点最大生命；升级后获得3点能量，抽3张牌，失去1点最大生命。",
    current: "新增消耗；获得2点能量，抽3张牌，失去1点最大生命；升级后获得3点能量，抽4张牌，失去1点最大生命。"
  },
  "FOLLY.description": {
    icon: vanillaIconPaths.cards.folly,
    vanilla: "无法打出。固有。永恒。虚无。",
    current: "无法打出。固有。永恒。"
  },
  "ENTHRALLED.description": {
    icon: vanillaIconPaths.cards.enthralled,
    vanilla: "2费诅咒。永恒。在手牌中时，必须先打出执迷；打出后无其他效果。",
    current: "2费诅咒。永恒。在手牌中时，必须先打出执迷；打出后获得10点格挡。"
  }
};

const cardDescOverrides = {
  EZMB_URDA_SEEDLING: "0费技能。消耗。获得4点格挡；升级后获得7点格挡。",
  EZMB_URDA_SEEDBED: "1费技能。消耗。获得4点格挡，设置2格苗床；之后进入手牌的临时状态牌、临时诅咒牌或根芽会优先种下，每种下1张加入1张枯壳。升级后获得6点格挡，设置3格苗床，并立即从抽牌堆或弃牌堆种下1张同类牌。",
  EZMB_URDA_RAIN_BREATH: "0费临时技能。消耗。获得5点格挡，抽1张牌。",
  EZMB_WITHERED_HUSK: "临时诅咒。虚无，消耗。被消耗时获得3点格挡；苗床不能种下这张牌。",
  EZMB_MORVI_ARCHIVE_DRAW_PAGE: "0费临时页。虚无，消耗。抽2张牌。",
  EZMB_MORVI_ARCHIVE_VEIL_PAGE: "0费临时页。虚无，消耗。获得14点格挡。",
  EZMB_MORVI_ARCHIVE_BURN_PAGE: "0费临时页。虚无，消耗。对所有敌人造成10点伤害。",
  EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE: "0费临时页。虚无，消耗。本回合下一张牌费用变为0。",
  EZMB_MORVI_ARCHIVE_BRAVERY_PAGE: "0费临时页。虚无，消耗。获得2点临时力量。",
  EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE: "0费临时页。虚无，消耗。获得2点临时敏捷。",
  EZMB_VAKUU_KNIFE_CONTRACT: "0费契约。临时，虚无，消耗。对瓦库造成24点伤害，失去4点生命；若仍有赃物锁，打破1把并增加1层血债。",
  EZMB_VAKUU_TEMPTATION: "0费契约。临时，虚无，消耗。获得2点能量，抽2张牌，失去5点生命；若仍有赃物锁，打破1把并增加1层血债。",
  EZMB_VAKUU_SHELTER_CONTRACT: "0费契约。临时，虚无，消耗。获得22点格挡，移除1层血债。",
  EZMB_VAKUU_TRICK_CONTRACT: "0费契约。临时，虚无，消耗。打破1把赃物锁，增加2层血债；瓦库行动前，它的攻击额外造成6点伤害。",
  EZMB_VAKUU_CASH_OUT_CONTRACT: "0费契约。临时，虚无，消耗。结束瓦库战斗并拿走破锁赃物；至少打破1把锁后可打出。"
};

const ascDescOverrides = {
  FIREMARK_MIGHT: "火印宿主开局获得1/2/4点力量；未被格挡的攻击伤害会积累热势，2层后下次攻击+1/2/4伤害；溢火给1名正在攻击的副目标1/1/2点临时力量。",
  FIREMARK_GIANT: "火印宿主最大生命提高20%/30%/45%；半血时暴露熔核，窗口内造成原最大生命20%/25%/30%的伤害可打破；打破后削弱宿主，并对1名副目标造成6/12/24点溢火伤害。",
  FIREMARK_FORGE_ARMOR: "你的回合开始时，火印宿主获得8/14/24点熔甲；若回合结束时宿主没有格挡，下一次熔甲跳过；溢火给1名副目标3/6/12点格挡。",
  FIREMARK_CONSTANT_HEAL: "敌方回合结束时，火印宿主回复4/8/16点生命；本轮造成12/24/48点伤害可中断治疗；治疗成功时，溢火为1名受伤副目标回复2/4/8点生命。",
  BANNER_VANGUARD: "敌人开战时获得临时力量。第1/2/3幕分别为1/2/4点；第3回合开始时失去这些力量。",
  BANNER_SHIELDWALL: "多敌人战斗中，一名敌人成为旗手。旗手存活时，敌方回合结束后其他敌人获得格挡；旗手死亡时，其他敌人获得5/10/20点格挡。",
  BANNER_BLOOD_PRIZE: "第3回合结束前击杀标记敌人，战斗后获得15/30/55金币。若它存活，它会获得1/2/4点力量和1/1/2层人工制品。",
  BANNER_PRESSING_LINE: "每回合从第4张牌开始敌阵充能，最多3层。充能给敌人4-6/8-12/16-24点格挡；满层使下次攻击+1/2/4伤害。",
  BANNER_LAST_STAND: "多敌人战斗中，第一个敌人死亡时，剩余敌人获得格挡和临时力量；力量第1/2/3幕分别为1/2/4点。"
};

const ascensionDetails = {
  LEVEL_11: [
    detail("地图", "宽度+1；第一幕+1层，第二幕+1层，第三幕+2层。", "Map", "Width +1; Act 1 +1 row, Act 2 +1 row, Act 3 +2 rows."),
    detail("路线", "插入路线必须可抵达，且保留一条不经过插入列的普通路线。", "Routing", "The inserted route must be reachable while a normal route that avoids the inserted column remains available.")
  ],
  LEVEL_12: [
    detail("数量", "第一幕目标2个火印精英；第二幕以后目标3个。若合适房间不足，至少放入2个。", "Count", "Act 1 targets 2 Firemarked Elites; later acts target 3. If there are not enough suitable rooms, at least 2 are placed."),
    detail("奖励", "击败后获得铸令；若已经持有铸令，改为15金币。火印精英卡牌奖励显示到4选。", "Reward", "Defeat one to gain a Forge Token. If already held, it converts to 15 Gold. Firemarked Elite card rewards show up to 4 cards."),
    detail("铸令", "下个休息处：休息随机升级1张可升级普通/罕见牌；没有目标则回复5 HP。锻造则回复7 HP。", "Forge Token", "At the next rest site: Rest upgrades 1 upgradable Common/Uncommon card at random, or heals 5 HP if none exists. Smith heals 7 HP.")
  ],
  LEVEL_13: [
    detail("出现率", "普通战10%；战旗房15%；火印精英20%；首领5%。", "Rates", "Normal combat 10%; Banner Room 15%; Firemarked Elite 20%; Boss 5%."),
    detail("可附魔牌", "只作用于普通/罕见/稀有攻击或技能；排除X费、0费、已有附魔、消耗牌和不能生成附魔的牌。", "Eligible cards", "Only Common, Uncommon, or Rare Attacks and Skills can receive Fission. X-cost, 0-cost, already enchanted, Exhaust, and non-enchantable cards are excluded."),
    detail("效果", "裂变牌耗能-1，打出后消耗。", "Effect", "Fission reduces cost by 1 and Exhausts the card after play.")
  ],
  LEVEL_14: [
    detail("开局", "开局加入根蚀 I。", "Start", "Start with Rootblight I."),
    detail("恶化", "战后仍在主牌组中的根蚀会升级到下一阶段；最多4张根蚀。", "Worsening", "Rootblights left in the master deck after combat worsen to the next stage. Max 4 Rootblights."),
    detail("休息", "休息处休息会移除最高等级的根蚀。", "Rest", "Resting removes the highest-level Rootblight.")
  ],
  LEVEL_15: [
    detail("首领", "第二幕和第三幕首领战埋入2张根芽。", "Boss fights", "Act 2 and Act 3 Boss fights bury 2 Blight Sprouts."),
    detail("萌发", "根芽分别在第3/4回合进入牌流；看见但不处理会在战后加入根蚀 I。", "Sprout timing", "They surface on rounds 3/4. If seen and left unresolved, they add Rootblight I after combat.")
  ],
  LEVEL_16: [
    detail("地图", "战旗房是公开规则的强化普通战斗。", "Map", "Banner Rooms are visible enhanced normal combats."),
    detail("战旗池", "先锋、盾阵、血赏、压阵、残阵。盾阵和残阵需要多敌人战；单敌人回退为血赏。", "Banner pool", "Vanguard, Shieldwall, Blood Prize, Pressing Line, Last Stand. Shieldwall and Last Stand require multi-enemy fights; single-enemy fights fall back to Blood Prize."),
    detail("奖励", "战旗房卡牌奖励的裂变率为15%。", "Reward", "Banner Room card rewards use the 15% Fission chance.")
  ],
  LEVEL_17: [
    detail("路线", "第二幕和第三幕各插入1条3-4节点深层支线，并在后方接回主路。", "Route", "Act 2 and Act 3 each insert one optional 3-4 node Deep Branch that reconnects later."),
    detail("奖励", "深层支线奖励节点会额外补1个罕见遗物奖励；普通安全路线保留。", "Reward", "A Deep Branch reward node adds an Uncommon relic reward. A normal safe route remains.")
  ],
  LEVEL_18: [
    detail("精英", "第二幕和第三幕中后段精英战埋入1张根芽。", "Elite fights", "Mid/late Act 2 and Act 3 Elite fights bury 1 Blight Sprout."),
    detail("结算", "根芽规则沿用A15：看见但不处理会在战后加入根蚀 I。", "Resolution", "Uses the A15 Blight Sprout rule: if seen and unresolved, it adds Rootblight I after combat.")
  ],
  LEVEL_19: [
    detail("\u9996\u9886", "\u6bcf\u540d\u9996\u9886\u83b7\u5f97\u81ea\u5df1\u7684\u4e13\u5c5e\u80fd\u529b\u3002\u5730\u56fe\u60ac\u505c\u548c\u6218\u6597\u63d0\u793a\u4f1a\u663e\u793a\u5177\u4f53\u80fd\u529b\u3002", "Bosses", "Each Boss gets its own dedicated ability. Map hover and combat indicators show the exact ability."),
    detail("\u5956\u52b1", "\u9996\u9886\u724c\u5956\u52b1\u591a\u663e\u793a1\u5f20\u724c\uff0c\u5e76\u4f18\u5148\u8865\u7a00\u6709\u724c\u3002", "Reward", "Boss card rewards show 1 extra card, preferring Rare cards for the extra option."),
    detail("\u5217\u8868", "\u4e0b\u65b9\u6bcf\u4e2a\u9996\u9886\u6761\u76ee\u5217\u51faA19\u4e13\u5c5e\u80fd\u529b\u548cA20\u70d9\u5370\u5f62\u6001\u5dee\u5f02\u3002", "List", "The Boss entries below list the A19 dedicated ability and A20 Branded Form differences.")
  ],
  LEVEL_20: [
    detail("\u7ec8\u5c40", "A20\u4e0d\u65b0\u589e\u7b2c\u4e09\u4e2a\u5b8c\u6574\u9996\u9886\uff0c\u6cbf\u7528\u539f\u7248\u7b2c\u4e09\u5e55\u53cc\u9996\u9886\u6d41\u7a0b\u3002", "Finale", "A20 does not add a third full Boss. It uses the vanilla Act 3 double-Boss flow."),
    detail("\u9996\u98861", "\u7b2c\u4e09\u5e55\u7b2c\u4e00\u4e2a\u9996\u9886\u4f7f\u7528\u666e\u901a\u4e13\u5c5e\u80fd\u529b\u3002\u6218\u540e\u82e5\u6ca1\u6709\u9996\u9886\u724c\u5956\u52b1\uff0c\u4f1a\u88651\u4e2a\u9996\u9886\u724c\u5956\u52b1\u3002", "Boss 1", "The first Act 3 Boss uses its normal dedicated ability. After Boss 1, a Boss card reward is added if no card reward exists."),
    detail("\u4e2d\u5ead", "\u9996\u98861\u5956\u52b1\u540e\u8fdb\u5165\u4e2d\u5ead\uff0c\u56de\u590d25%\u5df2\u635f\u751f\u547d\uff0c\u7136\u540e\u8fdb\u5165\u7b2c\u4e8c\u4e2a\u9996\u9886\u3002", "Courtyard", "After Boss 1 rewards, the Courtyard heals 25% of missing HP before Boss 2."),
    detail("\u9996\u98862", "\u7b2c\u4e09\u5e55\u7b2c\u4e8c\u4e2a\u9996\u9886\u63d0\u524d\u663e\u793a\uff0c\u5e76\u8fdb\u5165\u70d9\u5370\u5f62\u6001\uff0c\u5f3a\u5316\u81ea\u5df1\u7684\u4e13\u5c5e\u80fd\u529b\u3002", "Boss 2", "The second Act 3 Boss is revealed early and enters Branded Form, strengthening its own dedicated ability."),
    detail("\u8fb9\u754c", "\u5355\u4eba\u6d41\u7a0b\u4e3a\u4e3b\u8981\u76ee\u6807\uff1b\u591a\u4ebaA20\u70d9\u5370\u5f62\u6001\u4ecd\u6309\u5f00\u53d1\u6d4b\u8bd5\u5904\u7406\u3002", "Boundary", "Single-player is the primary target. Multiplayer A20 Branded Form behavior remains development-test scope.")
  ],
  FIREMARK_MIGHT: [
    detail("数值", "力量：第一幕1，第二幕2，第三幕4。", "Values", "Strength: Act 1/2/3 = 1/2/4."),
    detail("热势", "造成未被格挡攻击伤害后获得热势；2层热势使下一次攻击额外+1/+2/+4伤害。", "Heat", "Unblocked attack damage builds Heat. At 2 Heat, the next attack gains +1/+2/+4 damage."),
    detail("溢火", "1名正在攻击的副目标获得1/1/2点临时力量。", "Overflow", "1 attacking secondary enemy gains 1/1/2 temporary Strength.")
  ],
  FIREMARK_GIANT: [
    detail("数值", "最大/当前生命提高：第一幕20%，第二幕30%，第三幕45%。", "Values", "Max/current HP increase: Act 1/2/3 = +20%/+30%/+45%."),
    detail("熔核", "半血后暴露熔核；窗口内造成原最大生命20%/25%/30%的伤害可打破并削弱它，否则获得1层人工制品。", "Molten Core", "At half HP, Molten Core opens. Deal 20%/25%/30% of original Max HP during the window to break and weaken it; otherwise it gains 1 Artifact."),
    detail("溢火", "打破熔核时，对1名副目标造成6/12/24点溢火伤害。", "Overflow", "Breaking the core deals 6/12/24 overflow damage to 1 secondary enemy.")
  ],
  FIREMARK_FORGE_ARMOR: [
    detail("数值", "你的回合开始时，宿主获得8/14/24点熔甲。", "Values", "At the start of your turn, the host gains 8/14/24 Molten Armor."),
    detail("破甲", "回合结束时宿主没有格挡，则跳过下一次熔甲；每场最多触发2次。", "Break", "If the host has no Block at turn end, the next Molten Armor is skipped. Max 2 skips per combat."),
    detail("溢火", "1名副目标获得3/6/12点格挡。", "Overflow", "1 secondary enemy gains 3/6/12 Block.")
  ],
  FIREMARK_CONSTANT_HEAL: [
    detail("数值", "敌方回合结束时回复4/8/16 HP。", "Values", "At enemy turn end, heal 4/8/16 HP."),
    detail("打断", "本轮对宿主造成12/24/48点伤害可中断本次治疗。", "Interrupt", "Deal 12/24/48 damage to the host during the round to interrupt that heal."),
    detail("溢火", "治疗成功时，为1名受伤副目标回复2/4/8 HP。", "Overflow", "If the heal succeeds, 1 damaged secondary enemy heals 2/4/8 HP.")
  ],
  BANNER_VANGUARD: [
    detail("数值", "所有主要敌人开战获得1/2/4点临时力量。", "Values", "All primary enemies start with 1/2/4 temporary Strength."),
    detail("移除", "第3回合开始时移除这些力量。", "Removal", "Removed at the start of round 3.")
  ],
  BANNER_SHIELDWALL: [
    detail("数值", "旗手存活时，其他敌人每个敌方回合后获得3/7/14点格挡。", "Values", "While the bannerbearer lives, other enemies gain 3/7/14 Block after each enemy turn."),
    detail("死亡", "旗手死亡时，其他敌人获得5/10/20点格挡。", "Death", "When the bannerbearer dies, other enemies gain 5/10/20 Block.")
  ],
  BANNER_BLOOD_PRIZE: [
    detail("奖励", "第3回合结束前击杀标记敌人，战后获得15/30/55金币。", "Reward", "Kill the marked enemy before round 3 ends to gain 15/30/55 Gold after combat."),
    detail("失败", "若标记敌人存活，它获得1/2/4力量和1/1/2层人工制品。", "Miss", "If it survives, it gains 1/2/4 Strength and 1/1/2 Artifact.")
  ],
  BANNER_PRESSING_LINE: [
    detail("触发", "每回合第4/5/6张牌给敌阵充能，最多3层。", "Trigger", "Each turn, cards 4/5/6 charge the enemy line, max 3 layers."),
    detail("数值", "充能给敌人4-6/8-12/16-24点格挡；满层使下一次攻击+1/2/4伤害。", "Values", "Charge gives 4-6/8-12/16-24 Block. Full charge adds +1/2/4 damage to the next attack.")
  ],
  BANNER_LAST_STAND: [
    detail("触发", "仅多敌人战。第一个主要敌人死亡时触发一次。", "Trigger", "Multi-enemy fights only. Triggers once when the first primary enemy dies."),
    detail("数值", "剩余主要敌人获得6/12/24点格挡和1/2/4点临时力量。", "Values", "Remaining primary enemies gain 6/12/24 Block and 1/2/4 temporary Strength.")
  ]
};

function detail(label, text, labelEn, textEn) {
  return { label, text, labelEn, textEn };
}

window.SPIRE_PLUS_DATA = {
  labels: {
    brandSub: "Spire Plus，一个更好的《杀戮尖塔 2》拓展 · 温火融冰制作",
    navUpdates: "\u66f4\u65b0\u5185\u5bb9",
    navInstall: "\u4e0b\u8f7d\u4e0e\u5b89\u88c5",
    navForum: "\u8bba\u575b",
    navIssues: "\u5df2\u77e5\u95ee\u9898",
    navAbout: "关于",
    releaseLine: "",
    heroTitle: "Spire Plus",
    heroCopy: "一个更好的《杀戮尖塔 2》拓展。",
    modIntroTitle: "新增进阶 A20 和先古之民修改",
    featAscensionTitle: "新增进阶 20",
    featAscensionDesc: "原版进阶的难度上升不够快，但敌人和精英本身已经很强。Spire Plus 重新设计了更高阶的进阶挑战：一部分进阶在提高难度的同时也提高奖励，有些甚至会直接奖励玩家。奖励更多，面对的压力也更大。",
    featPhilosophyTitle: "科学难度与构筑可玩性",
    featPhilosophyDesc: "设计理念是让游戏更好玩，而不是单纯加数字、堆难度。Spire Plus 想解决一种很死板的玩法：为了活下去，被迫在火堆睡觉，被迫抓特定防御牌、解牌和生存牌。我们希望玩家有更多元、更自由的选择空间。",
    featRewardTitle: "高风险，高回报",
    featRewardDesc: "不必再为了苟活而做无趣的选择。如果你敢于挑战更危险的路线或特殊的先古试炼，你将赢取无与伦比的专属先古遗物与强力祝福。走最强大的路线，拿最丰厚的奖励！",
    aboutTitle: "关于",
    aboutLead: "项目说明、素材来源和发布边界。",
    download: "\u4e0b\u8f7d\u6a21\u7ec4",
    viewIssues: "\u67e5\u770b\u5df2\u77e5\u95ee\u9898",
    all: "\u5168\u90e8",
    search: "\u641c\u7d22",
    searchPlaceholder: "\u9057\u7269\u3001\u5148\u53e4\u3001\u8fdb\u9636\u3001\u5173\u952e\u8bcd",
    vanilla: "\u539f\u7248",
    current: "\u5f53\u524d",
    expandDetails: "\u5c55\u5f00\u5177\u4f53\u6548\u679c",
    sourceArtPlaceholder: "\u539f\u7248",
    installTitle: "\u4e0b\u8f7d\u4e0e\u5b89\u88c5",
    installLead: "\u4e0b\u8f7d\u6700\u65b0\u79c1\u6d4b\u5305\uff0c\u5e76\u6309\u4e0b\u65b9\u8def\u5f84\u5b89\u88c5 BaseLib \u4e0e Spire Plus\u3002",
    currentDownload: "\u5f53\u524d\u4e0b\u8f7d",
    requiredFilesTitle: "第一步：下载必要文件",
    optionalFilesTitle: "发布页与源码",
    directDownload: "直链下载",
    openRelease: "\u6253\u5f00\u53d1\u5e03\u9875",
    openBaseLib: "下载 BaseLib 3.1.4",
    openRepo: "\u6253\u5f00\u4ed3\u5e93",
    steps: "\u5b89\u88c5\u6b65\u9aa4",
    requirements: "\u8fd0\u884c\u8981\u6c42",
    assetPolicy: "图片与素材说明",
    forumTitle: "\u8bba\u575b",
    forumLead: "玩家发帖和回复的地方。",
    forumPublicTitle: "Spire Plus 论坛",
    openForum: "进入论坛",
    forumHealth: "数据库状态",
    forumDeployTitle: "运行方式",
    forumDeployCopy: "论坛页面由 GitHub Pages 托管，帖子和回复写入 Supabase PostgreSQL。未配置 Supabase 时，论坛会显示连接说明。",
    postName: "名字",
    postNamePlaceholder: "名字，可留空",
    anonymous: "匿名玩家",
    postTitle: "\u6807\u9898",
    postBody: "\u5185\u5bb9",
    postTitlePlaceholder: "\u8f93\u5165\u6807\u9898",
    postBodyPlaceholder: "写下你想说的内容。",
    replyPlaceholder: "写回复",
    replySubmit: "回复",
    postSubmit: "发布",
    postClear: "清空",
    noPosts: "还没有帖子。",
    issuesTitle: "\u5df2\u77e5\u95ee\u9898\u4e0e\u66f4\u65b0\u8bb0\u5f55",
    issuesLead: "\u8fd9\u91cc\u8bb0\u5f55\u4ecd\u9700\u9a8c\u8bc1\u7684\u95ee\u9898\u548c\u5df2\u5b8c\u6210\u7684\u7248\u672c\u6539\u52a8\u3002\u72b6\u6001\u4f1a\u968f\u6d4b\u8bd5\u8fdb\u5ea6\u66f4\u65b0\u3002",
    knownIssues: "\u5df2\u77e5\u95ee\u9898",
    changeLog: "\u66f4\u65b0\u8bb0\u5f55",
    noTitle: "\u672a\u547d\u540d"
  },
  summary: [],
  package: {
    localDownload: "../publish/SpirePlus-v0.1.0-private-beta.0.zip",
    releaseDownload:
      "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/latest/download/SpirePlus-v0.1.0-private-beta.0.zip",
    latestReleaseApi: "https://api.github.com/repos/wenhuorongbing-netizen/dev-the-spire/releases/latest",
    releasesPage: "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/latest",
    baseLibRelease: "https://github.com/Alchyr/BaseLib-StS2/releases/download/v3.1.4/BaseLib.3.1.4.zip",
    repository: "https://github.com/wenhuorongbing-netizen/dev-the-spire",
    meta: [
      ["\u6587\u4ef6", "SpirePlus-v0.1.0-private-beta.0.zip"],
      ["\u7248\u672c", "v0.1.0-private-beta.0"],
      ["\u663e\u793a\u540d", "Spire Plus"],
      ["\u4f9d\u8d56", "BaseLib v3.1.4"],
      ["\u6e38\u620f\u7248\u672c", "Slay the Spire 2 v0.106.0"],
      ["\u4f53\u79ef", "18,921,528 \u5b57\u8282"],
      ["\u54c8\u5e0c", "2EAC08531559C7871497741F5827705A3B9DB0EC60AF69A1C485AB6F9B4A3006"]
    ]
  },
  installSteps: [
    "\u4e0b\u8f7d SpirePlus-v0.1.0-private-beta.0.zip\u3002",
    "下载 BaseLib.3.1.4.zip，并解压到游戏的 mods\\BaseLib 目录。",
    "Windows 常见路径：Steam\\steamapps\\common\\Slay the Spire 2。",
    "将压缩包内的 Spire Plus 模组文件夹放入游戏的 mods 目录；不要手动改名。",
    "压缩包内保留技术兼容目录；玩家不需要手动改名。",
    "BaseLib 最终位置：mods\\BaseLib\\BaseLib.json。",
    "\u542f\u52a8\u6e38\u620f\uff0c\u5728\u6a21\u7ec4\u5217\u8868\u91cc\u542f\u7528 Spire Plus\u3002",
    "此版本仍处于私测；如果遇到问题，论坛里直接发帖即可。"
  ],
  requirements: [
    "Slay the Spire 2 public beta v0.106.0\u3002",
    "BaseLib v3.1.4；此版本按该依赖构建。",
    "\u6a21\u7ec4\u5217\u8868\u4e2d\u7684\u663e\u793a\u540d\u5e94\u4e3a Spire Plus\u3002",
    "为兼容现有存档，压缩包内的技术目录名暂不手动修改；玩家看到的模组名应为 Spire Plus。"
  ],
  assetPolicy: [
    "本站会使用 Spire Plus 自有素材、生成素材，以及已确认可以用于本站展示的原版美术。",
    "原版游戏的非美术资产不放进仓库；代码、数据表、文本转储、场景资源和玩法资源都不作为网站素材复制。",
    "当前页面用到的原版遗物图标和卡牌图像已确认可以用于本站发布，文件统一放在 website/assets/source-art/。",
    "以后如果要放更多原版截图或原图，会单独记录来源和用途。"
  ],
  forum: {
    url: "./forum/",
    localUrl: "./forum/",
    notice:
      "这里会是玩家讨论、Bug 反馈、构筑记录和版本建议的主入口。填写名字只是显示用，留空会显示为匿名玩家。数据由 Supabase 保存。",
    points: [
      "发帖：名字可留空，填写标题和正文即可发布。",
      "看帖：所有可见帖子按最后回复时间排序。",
      "回复：进入帖子详情后可直接回复。"
    ],
    links: [
      ["GitHub 仓库", "https://github.com/wenhuorongbing-netizen/dev-the-spire"],
      ["发布页", "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/tag/v0.1.0-private-beta.0"]
    ]
  },
  updateGroups: [
    {
      short: "\u5956\u52b1",
      title: "\u73b0\u6709\u5148\u53e4\u5956\u52b1\u91cd\u6784",
      note: "\u4ee5\u4e0b\u6761\u76ee\u5217\u51fa\u539f\u7248\u6548\u679c\u4e0e Spire Plus \u5f53\u524d\u6548\u679c\u3002",
      icon: "assets/relics/relic.png",
      defaultVanilla: "\u539f\u7248\u9057\u7269\u884c\u4e3a\u3002",
      items: [
        baseRelic("\u5929\u9e45\u7ed2\u9879\u5708", "VELVET_CHOKER.description", ["\u80fd\u91cf", "\u8f6f\u9650\u5236"], "每回合开始获得1点能量；每回合最多打出6张牌。", "每回合开始获得1点能量；第7张及之后从手牌打出的牌费用+1，不再硬性禁止出牌。"),
        baseRelic("\u5353\u8d8a\u6597\u7bf7", "DISTINGUISHED_CAPE.description", ["\u6700\u5927\u751f\u547d", "\u7075\u4f53"], "拾起时失去9点最大生命；将3张灵体加入牌组。", "拾起时失去当前最大生命的30%，至少18点；将3张灵体加入牌组。"),
        baseRelic("\u68f1\u5f69\u5b9d\u77f3", "PRISMATIC_GEM.description", ["\u80fd\u91cf", "\u5f02\u8272\u724c"], "每回合开始获得1点能量；卡牌奖励会包含其他颜色卡牌。", "每回合开始获得1点能量；每第2次标准卡牌奖励只出现异色牌。"),
        baseRelic("\u73e0\u5b9d\u76d2", "JEWELRY_BOX.description", ["\u795e\u5316"], "拾起时将1张神化加入牌组。", "拾起时将1张神化加入牌组；这张神化没有固有。"),
        baseRelic("腌制活雾", "PRESERVED_FOG.description", ["\u5220\u724c"], "拾起时从牌组移除3张牌；将1张愚行加入牌组。", "拾起时从牌组移除4张牌；将1张愚行加入牌组。"),
        cardLoc("愚行", "FOLLY.description", ["\u8bc5\u5492", "\u5361\u724c\u672c\u4f53"], "无法打出。固有。永恒。虚无。"),
        baseRelic("\u74e6\u5e93\u539f\u521d\u4e4b\u722a", "SERE_TALON.description", ["\u8bc5\u5492", "\u8bb8\u613f"], "\u74e6\u5e93\u5956\u52b1\u3002\u62fe\u53d6\u65f6\uff0c\u4ece\u0034\u5f20\u8bc5\u5492\u4e2d\u9009\u62e9\u0031\u5f20\u3002\u52a0\u5165\u5b83\u3001\u0032\u5f20\u8bb8\u613f\u548c\u0031\u5f20\u8bb8\u613f+\u3002", "\u62fe\u53d6\u65f6\uff0c\u4ece\u0034\u5f20\u8bc5\u5492\u4e2d\u9009\u62e9\u0031\u5f20\u3002\u52a0\u5165\u5b83\u3001\u0032\u5f20\u8bb8\u613f\u548c\u0031\u5f20\u8bb8\u613f+\u3002"),
        baseRelic("\u5766\u514b\u65af\u5229\u722a", "CLAWS.description", ["\u6495\u54ac+", "\u53d8\u5316"], "\u62fe\u53d6\u65f6\u9009\u62e9\u81f3\u591a6\u5f20\u724c\uff0c\u5c06\u5b83\u4eec\u53d8\u5316\u4e3a\u201c\u6495\u54ac+\u201d\u3002", "\u9009\u62e9\u81f3\u591a6\u5f20\u724c\uff0c\u5c06\u5b83\u4eec\u53d8\u5316\u4e3a\u201c\u6495\u54ac+\u201d\u3002"),
        baseRelic("\u9009\u62e9\u6096\u8bba", "CHOICES_PARADOX.description", ["\u7a00\u6709\u724c"], "每场战斗第1回合开始时，从5张随机牌中选择1张加入手牌；该牌获得保留。", "每场战斗开始时，从5张可用稀有牌中选择1张加入手牌；获得保留，并在战斗后移除。"),
        baseRelic("\u5b9d\u77f3\u9762\u5177", "JEWELED_MASK.description", ["\u80fd\u529b\u724c"], "每场战斗第1回合抽牌前，将抽牌堆中1张随机能力牌移入手牌；本回合费用为0。", "拾起时选择1张能力牌永久变为0费；每场战斗开始时，将它从抽牌堆移入手牌。"),
        baseRelic("\u5e15\u5c14\u4e4b\u89d2", "PAELS_HORN.description", ["\u653e\u677e"], "拾起时将2张放松加入牌组。", "拾起时将1张放松和1张已升级的放松+加入牌组。"),
        baseRelic("\u9ed1\u661f", "BLACK_STAR.description", ["\u7cbe\u82f1"], "精英额外掉落1件遗物。", "精英额外掉落1件遗物；若在第3幕或之后获得，立即获得1件随机遗物。"),
        baseRelic("\u6218\u9524", "WAR_HAMMER.description", ["\u5347\u7ea7"], "每当击败精英，随机升级4张牌。", "拾起时选择2张牌升级；每当击败精英，随机升级4张牌。"),
        baseRelic("\u5e15\u5c14\u4e4b\u7259", "PAELS_TOOTH.description", ["\u5220\u724c"], "拾起时移除5张可升级牌；每场战斗结束后随机将其中1张升级后放回牌组。", "拾起时移除5张牌；之后每2场非首领战斗后，选择1张已储存牌升级后放回牌组；击败本幕首领后剩余牌永久移除。"),
        baseRelic("\u9ec4\u91d1\u4e4b\u5370", "SEAL_OF_GOLD.description", ["\u80fd\u91cf", "\u503a\u52a1"], "你的回合开始时，若至少有5金币，花费5金币获得1点能量。", "获得1点能量；将2张可打出的债务诅咒加入牌组。债务1费，消耗；被消耗时失去至多5金币。"),
        baseRelic("\u6dfb\u6c34", "SOZU.description", ["\u836f\u6c34"], "每回合开始获得1点能量；不能再获得药水。", "获得1点能量；拾起时填满所有空药水栏；不能再获得药水。"),
        baseRelic("\u5916\u8d28", "ECTOPLASM.description", ["\u91d1\u5e01"], "每回合开始获得1点能量；不能再获得金币。", "获得1点能量；拾起时获得250金币；不能再获得金币。"),
        baseRelic("\u5c0f\u63d0\u7434", "FIDDLE.description", ["\u62bd\u724c"], "每回合开始额外抽2张牌；自己的回合中不能通过其他效果抽牌。", "每回合开始时抽到手牌有7张；自己的回合中，抽牌效果不能使手牌超过7张。"),
        baseRelic("\u94c1\u68d2", "IRON_CLUB.description", ["\u8ba1\u6570"], "每打出4张牌，抽1张牌。", "每打出5张牌，抽1张牌。"),
        baseRelic("\u95ea\u4eae\u56f4\u5dfe", "BRILLIANT_SCARF.description", ["\u8ba1\u6570"], "每回合从手牌打出的第5张牌费用变为0。", "每回合从手牌打出的第6张牌费用变为0。"),
        baseRelic("\u6f02\u4eae\u624b\u94fe", "BEAUTIFUL_BRACELET.description", ["\u8fc5\u901f"], "拾起时从牌组选择3张牌，附魔迅捷3。", "拾起时从牌组选择3张牌，附魔迅捷2。"),
        baseRelic("\u516b\u97f3\u76d2", "MUSIC_BOX.description", ["\u590d\u5236"], "每回合第一次打出攻击牌后，在手牌生成1张该攻击的虚无复制品。", "每回合第一次打出攻击牌后，在手牌生成1张复制；复制本回合费用-1，并具有虚无和消耗。"),
        baseRelic("\u5f29", "CROSSBOW.description", ["\u653b\u51fb"], "你的回合开始时，将1张随机攻击牌加入手牌；本回合费用为0。", "每回合开始时，你可以将1张随机攻击牌加入手牌；它本回合费用-1，并具有虚无和消耗。"),
        baseRelic("\u6696\u70d8\u624b\u5957", "TOASTY_MITTENS.description", ["\u529b\u91cf"], "你的回合抽牌前，消耗抽牌堆顶牌并获得1点力量；第1回合优先避开固有牌。", "每回合抽牌前查看抽牌堆顶牌；可以消耗它以获得1点力量。"),
        baseRelic("\u4f4e\u8bed\u8033\u73af", "WHISPERING_EARRING.description", ["\u81ea\u52a8\u6253\u51fa"], "每回合开始获得1点能量；第1回合由瓦库自动打出至多13张可打出的牌。", "获得1点能量；每场战斗前3回合，抽牌后自动打出费用最高的可打出牌。"),
        baseRelic("\u5207\u8089\u5200", "MEAT_CLEAVER.description", ["\u4f11\u606f\u5904"], "休息处新增烹饪：移除2张牌并获得9点最大生命。", "休息处新增切肉：移除2张牌并失去5点生命。"),
        baseRelic("\u8840\u67d3\u73ab\u7470", "BLOOD_SOAKED_ROSE.description", ["\u8bc5\u5492"], "遗物：拾起时将1张执迷加入牌组；每回合开始获得1点能量。执迷：2费诅咒，永恒；在手牌中时，必须先打出执迷。", "遗物本体仍为获得1点能量并加入1张执迷。"),
        cardLoc("\u6267\u8ff7", "ENTHRALLED.description", ["\u8bc5\u5492", "\u5361\u724c\u672c\u4f53"], "2费诅咒。永恒。在手牌中时，必须先打出执迷；打出后无其他效果。"),
        cardLoc("\u81f3\u4eae\u4e4b\u7130", "BRIGHTEST_FLAME.description", ["\u62bd\u724c", "\u6d88\u8017"], "获得2点能量，抽2张牌，失去1点最大生命；升级后获得3点能量，抽3张牌，失去1点最大生命。")
      ]
    },
    {
      short: "\u5148\u53e4",
      title: "\u65b0\u589e\u5148\u53e4\u4e0e\u74e6\u5e93\u8bd5\u70bc",
      note: "\u539f\u7248\u6ca1\u6709\u8fd9\u4e9b\u5148\u53e4\u8282\u70b9\u3002",
      icon: "assets/ancients/urda/ezmb_urda_map_icon.png",
      defaultVanilla: "\u539f\u7248\u65e0\u6b64\u65b0\u589e\u5185\u5bb9\u3002",
      items: [
        ancient("urda_seedbed", "assets/ancients/urda/options/urda_seedbed.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_humus_pact", "assets/ancients/urda/options/urda_humus_pact.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_molting", "assets/ancients/urda/options/urda_molting.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_moss_map", "assets/ancients/urda/options/urda_moss_map.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_trial_branch", "assets/ancients/urda/options/urda_trial_branch.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_shallow_root_relic", "assets/ancients/urda/options/urda_shallow_root_relic.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_rooted_route", "assets/ancients/urda/options/urda_rooted_route.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_after_rain", "assets/ancients/urda/options/urda_after_rain.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_root_sight", "assets/ancients/urda/options/urda_root_sight.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("urda_seed_bank", "assets/ancients/urda/options/urda_seed_bank.png", ["\u4e4c\u5c14\u59b2"]),
        ancient("morvi_forbidden_loan", "assets/ancients/morvi/options/morvi_forbidden_loan.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("morvi_misprint_press", "assets/ancients/morvi/options/morvi_misprint_press.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("morvi_red_ink_overdraft", "assets/ancients/morvi/options/morvi_red_ink_overdraft.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("morvi_overdue_library", "assets/ancients/morvi/options/morvi_overdue_library.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("morvi_open_book_exam", "assets/ancients/morvi/options/morvi_open_book_exam.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("morvi_paperstorm", "assets/ancients/morvi/options/morvi_paperstorm.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("morvi_blueprint_proof", "assets/ancients/morvi/options/morvi_blueprint_proof.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("morvi_debt_settlement", "assets/ancients/morvi/options/morvi_debt_settlement.png", ["\u83ab\u5c14\u7ef4"]),
        ancient("lotha_mirror_rebuttal", "assets/ancients/lotha/options/lotha_mirror_rebuttal.png", ["\u6d1b\u838e"]),
        ancient("lotha_mirror_hall_echo", "assets/ancients/lotha/options/lotha_mirror_hall_echo.png", ["\u6d1b\u838e"]),
        ancient("lotha_presumption", "assets/ancients/lotha/options/lotha_presumption.png", ["\u6d1b\u838e"]),
        ancient("lotha_closed_court", "assets/ancients/lotha/options/lotha_closed_court.png", ["\u6d1b\u838e"]),
        ancient("lotha_deferred_verdict", "assets/ancients/lotha/options/lotha_deferred_verdict.png", ["\u6d1b\u838e"]),
        ancient("lotha_death_reprieve", "assets/ancients/lotha/options/lotha_death_reprieve.png", ["\u6d1b\u838e"]),
        ancient("lotha_single_sentence", "assets/ancients/lotha/options/lotha_single_sentence.png", ["\u6d1b\u838e"]),
        ancient("lotha_public_evidence", "assets/ancients/lotha/options/lotha_public_evidence.png", ["\u6d1b\u838e"]),
        locItem("ancients", "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.title", "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description", "assets/ancients/vakuu/options/vakuu_fight.png", ["\u74e6\u5e93", "\u9690\u85cf"])
      ]
    },
    {
      short: "\u8fdb\u9636",
      title: "\u8fdb\u9636 11-20",
      note: "\u539f\u7248\u5f53\u524d\u53ea\u5230 A10\uff1b\u672c\u5305\u52a0\u5165 A11-A20 \u5f00\u53d1\u6d4b\u8bd5\u5185\u5bb9\u3002",
      icon: "assets/ascension/firemarked_elite_indicator.png",
      defaultVanilla: "\u539f\u7248\u65e0\u6b64\u8fdb\u9636\u5185\u5bb9\u3002",
      items: [
        asc("LEVEL_11", ["\u5730\u56fe"]),
        asc("LEVEL_12", ["\u7cbe\u82f1"], "assets/ascension/firemarked_elite_indicator.png"),
        asc("LEVEL_13", ["\u5956\u52b1"], "assets/ascension/fission_enchantment_icon.png"),
        asc("LEVEL_14", ["\u6839\u8680"], "assets/card_portraits/rootblight_i.png"),
        asc("LEVEL_15", ["\u9996\u9886", "\u6839\u82bd"], "assets/card_portraits/blight_sprout.png"),
        asc("LEVEL_16", ["\u6218\u65d7"], "assets/ascension/banner_room_indicator.png"),
        asc("LEVEL_17", ["\u5730\u56fe"]),
        asc("LEVEL_18", ["\u7cbe\u82f1", "\u6839\u82bd"], "assets/card_portraits/blight_sprout.png"),
        asc("LEVEL_19", ["\u9996\u9886"], "assets/ascension/boss_seal_indicator.png"),
        asc("LEVEL_20", ["\u9996\u9886"], "assets/ascension/boss_seal_indicator.png"),
        asc("FIREMARK_MIGHT", ["A12"], "assets/ascension/firemark_might_indicator.png"),
        asc("FIREMARK_GIANT", ["A12"], "assets/ascension/firemark_giant_indicator.png"),
        asc("FIREMARK_FORGE_ARMOR", ["A12"], "assets/ascension/firemark_forge_armor_indicator.png"),
        asc("FIREMARK_CONSTANT_HEAL", ["A12"], "assets/ascension/firemark_constant_heal_indicator.png"),
        asc("BANNER_VANGUARD", ["A16"], "assets/ascension/banner_vanguard_indicator.png"),
        asc("BANNER_SHIELDWALL", ["A16"], "assets/ascension/banner_shield_formation_indicator.png"),
        asc("BANNER_BLOOD_PRIZE", ["A16"], "assets/ascension/banner_bounty_indicator.png"),
        asc("BANNER_PRESSING_LINE", ["A16"], "assets/ascension/banner_room_indicator.png"),
        asc("BANNER_LAST_STAND", ["A16"], "assets/ascension/banner_room_indicator.png"),
        ...bossSealItems()
      ]
    },
    {
      short: "\u5361\u724c",
      title: "\u65b0\u589e\u5361\u724c\u3001\u72b6\u6001\u4e0e\u9884\u89c8\u5de5\u5177",
      note: "\u539f\u7248\u6ca1\u6709\u8fd9\u4e9b\u65b0\u589e\u5361\u724c\u3001\u72b6\u6001\u548c\u9884\u89c8\u5de5\u5177\u3002",
      icon: "assets/card_portraits/rootblight_i.png",
      defaultVanilla: "\u539f\u7248\u65e0\u6b64\u65b0\u589e\u5185\u5bb9\u3002",
      items: [
        card("EZMB_URDA_SEEDLING", ["\u4e4c\u5c14\u59b2"], "assets/card_portraits/urda_seedling.png"),
        card("EZMB_URDA_SEEDBED", ["\u4e4c\u5c14\u59b2"], "assets/card_portraits/urda_seedling.png"),
        card("EZMB_URDA_RAIN_BREATH", ["\u4e4c\u5c14\u59b2"], "assets/card_portraits/urda_seedling.png"),
        card("EZMB_WITHERED_HUSK", ["\u8bc5\u5492"], "assets/card_portraits/withered_husk.png"),
        card("EZMB_MORVI_ARCHIVE_DRAW_PAGE", ["\u83ab\u5c14\u7ef4"], "assets/card_portraits/morvi_archive_pages.png"),
        card("EZMB_MORVI_ARCHIVE_VEIL_PAGE", ["\u83ab\u5c14\u7ef4"], "assets/card_portraits/morvi_archive_pages.png"),
        card("EZMB_MORVI_ARCHIVE_BURN_PAGE", ["\u83ab\u5c14\u7ef4"], "assets/card_portraits/morvi_archive_pages.png"),
        card("EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE", ["\u83ab\u5c14\u7ef4"], "assets/card_portraits/morvi_archive_pages.png"),
        card("EZMB_MORVI_ARCHIVE_BRAVERY_PAGE", ["\u83ab\u5c14\u7ef4"], "assets/card_portraits/morvi_archive_pages.png"),
        card("EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE", ["\u83ab\u5c14\u7ef4"], "assets/card_portraits/morvi_archive_pages.png"),
        card("EZMB_MORVI_WASTE_PAPER", ["\u72b6\u6001"], "assets/card_portraits/morvi_waste_paper.png"),
        card("EZMB_MORVI_RED_INK_OVERDRAFT", ["\u83ab\u5c14\u7ef4"], "assets/card_portraits/morvi_red_ink_overdraft.png"),
        card("EZMB_ROOT", ["\u8fdb\u9636"], "assets/card_portraits/rootblight_i.png"),
        card("EZMB_DEEP_ROOT", ["\u8fdb\u9636"], "assets/card_portraits/rootblight_ii.png"),
        card("EZMB_ROOTBLIGHT_III", ["\u8fdb\u9636"], "assets/card_portraits/rootblight_iii.png"),
        card("EZMB_ROOT_BUD", ["\u8fdb\u9636"], "assets/card_portraits/blight_sprout.png"),
        card("EZMB_VAKUU_KNIFE_CONTRACT", ["\u74e6\u5e93"], "assets/card_portraits/vakuu_temptation.png"),
        card("EZMB_VAKUU_TEMPTATION", ["\u74e6\u5e93"], "assets/card_portraits/vakuu_temptation.png"),
        card("EZMB_VAKUU_SHELTER_CONTRACT", ["\u74e6\u5e93"], "assets/card_portraits/vakuu_temptation.png"),
        card("EZMB_VAKUU_TRICK_CONTRACT", ["\u74e6\u5e93"], "assets/card_portraits/vakuu_temptation.png"),
        card("EZMB_VAKUU_CASH_OUT_CONTRACT", ["\u74e6\u5e93"], "assets/card_portraits/vakuu_temptation.png"),
        manual("\u6c34\u6676\u7403\u9884\u77e5", "\u539f\u7248\u65e0\u9884\u77e5\u6309\u94ae\u3002", "\u6c34\u6676\u7403\u5c0f\u6e38\u620f\u4e2d\u663e\u793a\u9884\u77e5\u6309\u94ae\uff1b\u53ea\u6539\u53d8\u906e\u7f69\u53ef\u89c1\u6027\uff0c\u4e0d\u53d1\u653e\u5956\u52b1\u3002", ["\u9884\u89c8\u5de5\u5177"]),
        manual("\u53d8\u6362\u771f\u5b9e\u9884\u89c8", "\u539f\u7248\u4e0d\u663e\u793a\u786e\u5b9a\u7ed3\u679c\u3002", "\u4f7f\u7528\u590d\u5236\u7684\u968f\u673a\u6570\u5feb\u7167\u9884\u6d4b\u53d8\u6362\u7ed3\u679c\uff1b\u4e0d\u521b\u5efa\u5361\u724c\uff0c\u4e0d\u63a8\u8fdb\u771f\u5b9e\u968f\u673a\u6570\u3002", ["\u9884\u89c8\u5de5\u5177"])
      ]
    }
  ],
  knownIssues: [
    ["\u9ad8\u4f18\u5148\u7ea7", "\u5b9e\u673a\u52a0\u8f7d\u8bb0\u5f55\u5f85\u66f4\u65b0", "\u5f53\u524d\u5305\u9700\u8981\u91cd\u65b0\u8dd1\u4e00\u6b21\u5b8c\u6574\u52a0\u8f7d\u6d4b\u8bd5\uff0c\u5e76\u4fdd\u7559\u4e0e\u6700\u65b0\u6784\u5efa\u5bf9\u5e94\u7684 godot.log\u3002"],
    ["\u5f85\u9a8c\u8bc1", "\u5148\u53e4\u8282\u70b9\u754c\u9762", "\u4e4c\u5c14\u59b2\u3001\u83ab\u5c14\u7ef4\u3001\u6d1b\u838e\u3001\u666e\u901a\u74e6\u5e93\u548c\u9690\u85cf\u74e6\u5e93\u6218\u6597\u9700\u8981\u8865\u5145\u622a\u56fe\u3001\u65e5\u5fd7\u548c\u60ac\u505c\u6587\u672c\u53ef\u8bfb\u6027\u8bc1\u660e\u3002"],
    ["\u5f85\u9a8c\u8bc1", "\u74e6\u5e93\u8bd5\u70bc", "\u80dc\u5229\u8fd4\u56de\u4e8b\u4ef6\u3001\u5931\u8d25/\u6b7b\u4ea1\u7ed3\u7b97\u3001\u5b58\u8bfb\u6863\u548c\u8054\u673a\u8fb9\u754c\u9700\u8981\u8fdb\u4e00\u6b65\u5b9e\u673a\u6d4b\u8bd5\u3002"],
    ["\u5f85\u9a8c\u8bc1", "\u8fdb\u9636 11-20", "A11 \u8def\u7ebf\u70b9\u51fb\u3001A12/A16/A19/A20 \u6218\u6597\u89c4\u5219\u3001\u6839\u8680\u6218\u540e\u7ed3\u7b97\u3001\u5b58\u8bfb\u6863\u548c\u53cc\u4eba\u8054\u673a\u8fd8\u9700\u8981\u5b8c\u6574\u8bb0\u5f55\u3002"],
    ["监控中", "公开论坛", "论坛已接入 GitHub Pages 与 Supabase；继续观察免费项目休眠、缓存刷新、匿名刷屏和简单管理后台缺失问题。"],
    ["\u7d20\u6750\u8bf4\u660e", "\u539f\u7248\u6e38\u620f\u56fe\u7247", "\u539f\u7248\u9057\u7269\u56fe\u6807\u4e0e\u5361\u724c\u7acb\u7ed8\u5df2\u83b7\u5f97\u7ad9\u70b9\u53d1\u5e03\u6388\u6743\u786e\u8ba4\uff0c\u5e76\u4ec5\u5206\u53d1\u5f53\u524d\u66f4\u65b0\u9875\u5b9e\u9645\u4f7f\u7528\u7684\u56fe\u50cf\u3002"]
  ],
  changeLog: [
    ["2026-05-23 · 玩法文本同步", "网站重新同步当前 mod localization，并更新苗床、雨息、终审封庭、瓦库试炼契约、A12 火印溢火与 A19/A20 首领专属能力展示。"],
    ["2026-05-22 \u00b7 \u7f51\u7ad9\u91cd\u6784", "\u7ad9\u70b9\u6539\u4e3a\u56db\u4e2a\u4e3b\u8981\u9875\u9762\uff1a\u66f4\u65b0\u5185\u5bb9\u3001\u4e0b\u8f7d\u4e0e\u5b89\u88c5\u3001\u8bba\u575b\u3001\u5df2\u77e5\u95ee\u9898\u4e0e\u66f4\u65b0\u8bb0\u5f55\u3002"],
    ["\u5f53\u524d\u5305", "SpirePlus-v0.1.0-private-beta.0.zip；游戏内显示名为 Spire Plus。"],
    ["\u5148\u53e4\u5185\u5bb9", "\u4e4c\u5c14\u59b2\u3001\u83ab\u5c14\u7ef4\u3001\u6d1b\u838e\u5df2\u4f5c\u4e3a\u65b0\u5148\u53e4\u52a0\u5165\uff1b\u74e6\u5e93\u8bd5\u70bc\u4ecd\u4fdd\u6301\u9690\u85cf\u95e8\u63a7\u3002"],
    ["\u8fdb\u9636\u5185\u5bb9", "A11-A20 \u5df2\u52a0\u5165\u79c1\u6d4b\u5305\u3002\u5355\u4eba\u548c\u623f\u4e3b\u591a\u4eba\u53ef\u9009\uff0c\u5b8c\u6574\u8054\u673a\u73a9\u6cd5\u4ecd\u9700\u540e\u7eed\u9a8c\u8bc1\u3002"],
    ["\u9884\u89c8\u5de5\u5177", "\u6c34\u6676\u7403\u9884\u77e5\u548c\u53d8\u6362\u771f\u5b9e\u9884\u89c8\u5df2\u5408\u5e76\u8fdb Spire Plus\uff0c\u4e0d\u518d\u4f5c\u4e3a\u72ec\u7acb\u6a21\u7ec4\u53d1\u5e03\u3002"]
  ],
  locFiles: {
    relics: "assets/localization/zhs/relics.json",
    ancients: "assets/localization/zhs/ancients.json",
    ascension: "assets/localization/zhs/ascension.json",
    cards: "assets/localization/zhs/cards.json",
    powers: "assets/localization/zhs/powers.json"
  }
};

function sourceRelicIcon(descKey) {
  const ownedIcon = ownedRelicIcons[descKey];
  if (ownedIcon) return ownedIcon;
  const fileName = sourceRelicIcons[descKey];
  return fileName ? `assets/source-art/relics/${fileName}` : undefined;
}

function baseRelic(title, descKey, tags, vanilla, current) {
  return { title, descKey, desc: current, namespace: "relics", tags, vanilla, icon: sourceRelicIcon(descKey) };
}

function cardLoc(title, descKey, tags, vanilla) {
  const override = sourceCardOverrides[descKey] || {};
  return {
    title,
    descKey,
    desc: override.current,
    namespace: "cards",
    tags,
    vanilla: override.vanilla || vanilla,
    icon: override.icon
  };
}

function ancient(id, icon, tags) {
  let prefix = "EZMB_URDA";
  if (id.startsWith("morvi_")) prefix = "EZMB_MORVI";
  if (id.startsWith("lotha_")) prefix = "EZMB_LOTHA";
  return {
    titleKey: `${prefix}.pages.INITIAL.options.${id}.title`,
    descKey: `${prefix}.pages.INITIAL.options.${id}.description`,
    namespace: "ancients",
    icon,
    tags,
    ancientId: id
  };
}

function locItem(namespace, titleKey, descKey, icon, tags) {
  return { namespace, titleKey, descKey, icon, tags };
}

function asc(id, tags, icon) {
  return {
    namespace: "ascension",
    titleKey: `${id}.title`,
    descKey: `${id}.description`,
    desc: ascDescOverrides[id],
    details: ascensionDetails[id],
    icon,
    tags
  };
}

function bossSealItems() {
  return [
    bossSeal("ceremonial_beast_holy_daze", "仪式兽", "Ceremonial Beast", "圣昏", "Holy Daze", "holy_daze", "BOSS_SEAL_HOLY_DAZE"),
    bossSeal("the_kin_martyr_oath", "同族小队", "The Kin", "殉誓", "Martyr Oath", "martyr_oath", "BOSS_SEAL_MARTYR_OATH"),
    bossSeal("vantom_ink_return", "墨影幻灵", "Vantom", "墨返", "Ink Return", "ink_return", "BOSS_SEAL_INK_RETURN"),
    bossSeal("lagavulin_matriarch_plating_wake", "乐加维林族母", "Lagavulin Matriarch", "多重护甲苏醒", "Plating Wake", "startled_shell", "BOSS_SEAL_STARTLED_SHELL"),
    bossSeal("soul_fysh_soul_tide", "灵魂异鱼", "Soul Fysh", "魂潮", "Soul Tide", "soul_tide", "BOSS_SEAL_SOUL_TIDE"),
    bossSeal("waterfall_giant_unweakenable", "瀑布巨兽", "Waterfall Giant", "不可削弱", "Unweakenable", "boiling_critical", "BOSS_SEAL_BOILING_CRITICAL"),
    bossSeal("kaiser_crab_claw_calibration", "帝皇蟹", "Kaiser Crab", "错壳校准", "Claw Calibration", "misaligned_shell", "BOSS_SEAL_MISALIGNED_SHELL"),
    bossSeal("knowledge_demon_marginal_note", "知识恶魔", "Knowledge Demon", "旁注", "Marginal Note", "marginal_note", "BOSS_SEAL_MARGINAL_NOTE"),
    bossSeal("the_insatiable_escape_fatigue", "无厌沙虫", "The Insatiable", "逃亡疲劳", "Escape Fatigue", "struggle_bait", "BOSS_SEAL_STRUGGLE_BAIT"),
    bossSeal("aeonglass_hourglass", "永世沙漏", "Aeonglass", "时砂回流", "Time Sand Reflow", "aeonglass_hourglass", "BOSS_SEAL_AEONGLASS_HOURGLASS"),
    bossSeal("queen_royal_decree", "女王", "Queen", "御令", "Royal Decree", "chosen_decree", "BOSS_SEAL_CHOSEN_DECREE"),
    bossSeal("test_subject_experimental_record", "实验体", "Test Subject", "实验记录", "Experimental Record", "residual_sample", "BOSS_SEAL_RESIDUAL_SAMPLE")
  ];
}

function bossSeal(id, bossName, bossNameEn, abilityName, abilityNameEn, iconName, locPrefix) {
  return {
    namespace: "ascension",
    i18nKey: `boss_seal_${id}`,
    title: `${bossName}：${abilityName}`,
    titleEn: `${bossNameEn}: ${abilityNameEn}`,
    descKey: `${locPrefix}.summary`,
    vanilla: "原版首领没有 A19 专属能力或 A20 烙印形态。",
    vanillaEn: "Vanilla bosses do not have A19 dedicated abilities or A20 Branded Form.",
    icon: `assets/ascension/boss_seals/${iconName}.png`,
    tags: ["A19", "A20", "专属能力", "烙印形态"],
    details: [
      { label: "专属能力", labelEn: "Dedicated Ability", key: `${locPrefix}.summary` },
      { label: "烙印形态", labelEn: "Branded Form", key: `${locPrefix}.brand` }
    ]
  };
}
function card(id, tags, icon) {
  return {
    namespace: "cards",
    titleKey: `${id}.title`,
    descKey: `${id}.description`,
    desc: cardDescOverrides[id],
    icon,
    tags
  };
}

function manual(title, vanilla, current, tags, icon) {
  let i18nKey = title;
  if (title === "\u6c34\u6676\u7403\u9884\u77e5") i18nKey = "crystal_sphere_peek";
  if (title === "\u53d8\u6362\u771f\u5b9e\u9884\u89c8") i18nKey = "transform_preview";
  return { title, vanilla, current, tags, icon, i18nKey };
}

window.SPIRE_PLUS_DATA.i18n = {
  en: {
    labels: {
      brandSub: "Spire Plus, a better Slay the Spire 2 expansion by Wenhuo Rongbing",
      navUpdates: "Updates",
      navInstall: "Download & Install",
      navForum: "Forum",
      navIssues: "Known Issues",
      navAbout: "About",
      releaseLine: "",
      heroCopy: "A better Slay the Spire 2 expansion.",
      modIntroTitle: "New A20 Ascension and Ancient changes",
      featAscensionTitle: "New Ascension 20",
      featAscensionDesc: "Vanilla Ascension does not climb fast enough, while enemies and Elites are already strong. Spire Plus redesigns the higher Ascension challenge: some levels raise rewards while raising danger, and some directly reward the player. More rewards also means harder fights.",
      featPhilosophyTitle: "Science of Fun & Build Freedom",
      featPhilosophyDesc: "The goal is to make the game more fun, not just add numbers and difficulty. Spire Plus targets rigid survival patterns where players feel forced to rest at fires or draft specific defense, answer, and survival cards. The result should be more varied and freer deck choices.",
      featRewardTitle: "High Risk, High Reward",
      featRewardDesc: "No more boring, compromise-filled choices. Challenge yourself with riskier, stronger paths and Boss trials to earn powerful custom marker relics and Ancient blessings. Survive and conquer your own way!",
      aboutTitle: "About",
      aboutLead: "Project notes, asset sources, and release boundaries.",
      download: "Download Mod",
      viewIssues: "Known Issues",
      all: "All",
      search: "Search",
      searchPlaceholder: "Relic, Ancient, Ascension, keyword",
      vanilla: "Vanilla",
      current: "Current",
      expandDetails: "Expand exact effects",
      sourceArtPlaceholder: "Vanilla",
      installTitle: "Download & Install",
      installLead: "Download the latest private test build, then install BaseLib and Spire Plus in the paths below.",
      currentDownload: "Current Download",
      requiredFilesTitle: "Step 1: Required Files",
      optionalFilesTitle: "Release Page & Source",
      directDownload: "Direct Download",
      openRelease: "Open Releases",
      openBaseLib: "Download BaseLib 3.1.4",
      openRepo: "Open Repository",
      steps: "Install Steps",
      requirements: "Requirements",
      assetPolicy: "Images and Assets",
        forumTitle: "Forum",
        forumLead: "A place for player posts and replies.",
        forumPublicTitle: "Spire Plus Forum",
        openForum: "Open Forum",
        forumHealth: "Database Status",
        forumDeployTitle: "Runtime",
        forumDeployCopy: "The forum page is hosted by GitHub Pages. Posts and replies are stored in Supabase PostgreSQL. If Supabase is not configured yet, the forum page shows setup instructions.",
      postName: "Name",
      postNamePlaceholder: "Name, optional",
      anonymous: "Anonymous player",
      postTitle: "Title",
      postBody: "Body",
      postTitlePlaceholder: "Enter title",
      postBodyPlaceholder: "Write what you want to say.",
      replyPlaceholder: "Write a reply",
      replySubmit: "Reply",
        postSubmit: "Post",
        postClear: "Clear",
        noPosts: "No posts yet.",
      issuesTitle: "Known Issues & Changelog",
      issuesLead: "Known issues and completed changes are tracked here. Status will update as testing continues.",
      knownIssues: "Known Issues",
      changeLog: "Changelog",
      noTitle: "Untitled",
      separator: " · ",
      issueSeparator: " · "
    },
    summary: [],
    locFiles: {
      relics: "assets/localization/eng/relics.json",
      ancients: "assets/localization/eng/ancients.json",
      ascension: "assets/localization/eng/ascension.json",
      cards: "assets/localization/eng/cards.json",
      powers: "assets/localization/eng/powers.json"
    },
    package: {
      meta: [
        ["File", "SpirePlus-v0.1.0-private-beta.0.zip"],
        ["Version", "v0.1.0-private-beta.0"],
        ["Display name", "Spire Plus"],
        ["Dependency", "BaseLib v3.1.4"],
        ["Game version", "Slay the Spire 2 v0.106.0"],
        ["Size", "18,921,528 bytes"],
        ["Hash", "2EAC08531559C7871497741F5827705A3B9DB0EC60AF69A1C485AB6F9B4A3006"]
      ]
    },
    installSteps: [
      "Download SpirePlus-v0.1.0-private-beta.0.zip.",
      "Download BaseLib.3.1.4.zip and extract it to the game's mods\\BaseLib folder.",
      "Common Windows path: Steam\\steamapps\\common\\Slay the Spire 2.",
      "Place the Spire Plus mod folder from the zip into the game's mods folder. Do not rename it manually.",
      "The archive keeps its technical compatibility folder; players should not rename it manually.",
      "Final BaseLib path: mods\\BaseLib\\BaseLib.json.",
      "Start the game and enable Spire Plus in the mod list.",
      "This is a private test build. If something breaks, post it in the forum."
    ],
    requirements: [
      "Slay the Spire 2 public beta v0.106.0.",
      "BaseLib v3.1.4. This build targets that dependency version.",
      "The mod list should show Spire Plus.",
      "For save compatibility, do not manually rename the technical folder inside the zip. The in-game mod name should be Spire Plus."
    ],
    assetPolicy: [
      "This site uses Spire Plus-owned assets, generated assets, and approved base-game art needed for the update page.",
      "Original non-art game assets are not copied into the repository: no code, data tables, text dumps, scenes, or gameplay resources.",
      "The vanilla relic icons and card portraits currently shown on the site have been approved for this public page and live under website/assets/source-art/.",
      "If more base-game screenshots or art are added later, the source and usage scope will be documented separately."
    ],
      forum: {
        url: "./forum/",
        localUrl: "./forum/",
        notice: "This is the main place for player discussion, bug reports, run notes, and version feedback. Names are display-only, and empty names are shown as anonymous players. Data is stored in Supabase.",
        points: [
          "Post: name is optional; title and body are enough.",
          "Browse: visible posts are sorted by latest activity.",
          "Reply: open a post and answer directly."
        ],
        links: [
          ["GitHub Repository", "https://github.com/wenhuorongbing-netizen/dev-the-spire"],
          ["Release Page", "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/tag/v0.1.0-private-beta.0"]
        ]
      },
    updateGroups: [
      {
        short: "Rewards",
        title: "Existing Ancient Reward Rework",
        note: "Reworked effects for existing relics and rewards.",
        defaultVanilla: "Vanilla relic behavior."
      },
      {
        short: "Ancients",
        title: "New Ancients and Vakuu Trial",
        note: "These Ancient nodes do not exist in vanilla.",
        defaultVanilla: "No equivalent vanilla content."
      },
      {
        short: "Ascension",
        title: "Ascension 11-20",
        note: "Spire Plus adds a private-test A11-A20 ruleset beyond vanilla A10.",
        defaultVanilla: "No equivalent vanilla content."
      },
      {
        short: "Cards",
        title: "New Cards, Statuses, and Preview Tools",
        note: "New cards, statuses, and preview tools added by the mod.",
        defaultVanilla: "No equivalent vanilla content."
      }
    ],
    tagTranslations: {
      "能量": "Energy",
      "软限制": "Soft cap",
      "最大生命": "Max HP",
      "灵体": "Apparition",
      "异色牌": "Off-color",
      "神化": "Apotheosis",
      "删牌": "Remove",
      "诅咒": "Curse",
      "卡牌本体": "Card change",
      "许愿": "Wish",
      "稀有牌": "Rare card",
      "能力牌": "Power",
      "放松": "Relax",
      "精英": "Elite",
      "升级": "Upgrade",
      "债务": "Debt",
      "药水": "Potion",
      "金币": "Gold",
      "抽牌": "Draw",
      "计数": "Counter",
      "迅速": "Swift",
      "复制": "Copy",
      "攻击": "Attack",
      "力量": "Strength",
      "自动打出": "Auto-play",
      "休息处": "Rest site",
      "乌尔妲": "Urda",
      "莫尔维": "Morvi",
      "洛莎": "Lotha",
      "瓦库": "Vakuu",
      "隐藏": "Hidden",
      "消耗": "Exhaust",
      "地图": "Map",
      "奖励": "Reward",
      "\u8fdb\u9636": "Ascension",
      "根蚀": "Rootblight",
      "首领": "Boss",
      "根芽": "Blight Sprout",
      "战旗": "Banner",
      "专属能力": "Dedicated Ability",
      "烙印形态": "Branded Form",
      "卡牌": "Card",
      "状态": "Status",
      "预览工具": "Preview tool"
    },
    items: {
      "VELVET_CHOKER.description": {
        title: "Velvet Choker",
        vanilla: "Gain 1 Energy at the start of each turn. You cannot play more than 6 cards per turn.",
        desc: "Gain 1 Energy at the start of each turn. The 7th and later cards played from hand cost 1 more instead of being blocked."
      },
      "DISTINGUISHED_CAPE.description": {
        title: "Distinguished Cape",
        vanilla: "On pickup, lose 9 Max HP and add 3 Apparitions to your deck.",
        desc: "On pickup, lose 30% of current Max HP, minimum 18. Add 3 Apparitions to your deck."
      },
      "PRISMATIC_GEM.description": {
        title: "Prismatic Gem",
        vanilla: "Gain 1 Energy at the start of each turn. Card rewards can include cards from other colors.",
        desc: "Gain 1 Energy at the start of each turn. Every 2nd standard card reward contains only off-color cards."
      },
      "JEWELRY_BOX.description": {
        title: "Jewelry Box",
        vanilla: "On pickup, add 1 Apotheosis to your deck.",
        desc: "On pickup, add 1 Apotheosis to your deck. That Apotheosis does not have Innate."
      },
      "PRESERVED_FOG.description": {
        title: "Preserved Fog",
        vanilla: "On pickup, remove 3 cards from your deck and add 1 Folly.",
        desc: "On pickup, remove 4 cards from your deck and add 1 Folly."
      },
      "FOLLY.description": {
        title: "Folly",
        vanilla: "Unplayable. Innate. Eternal. Ethereal.",
        desc: "Unplayable. Innate. Eternal."
      },
      "SERE_TALON.description": {
        title: "Vakuu's Sere Talon",
        vanilla: "Vakuu reward. On pickup, choose 1 of 4 Curses, then add that Curse, 2 Wish, and 1 Wish+.",
        desc: "On pickup, choose 1 of 4 Curses. Add it, 2 Wish, and 1 Wish+."
      },
      "CLAWS.description": {
        title: "Tanx Claws",
        vanilla: "On pickup, choose up to 6 cards and transform them into Maul.",
        desc: "Transforms up to 6 cards into upgraded Maul."
      },
      "CHOICES_PARADOX.description": {
        title: "Choices Paradox",
        vanilla: "At the start of combat turn 1, choose 1 of 5 random cards to add to hand. It gains Retain.",
        desc: "At combat start, choose 1 of 5 available Rare cards to add to hand. It gains Retain and is removed after combat."
      },
      "JEWELED_MASK.description": {
        title: "Jeweled Mask",
        vanilla: "Before the first draw of each combat, move 1 random Power from draw pile to hand. It costs 0 this turn.",
        desc: "On pickup, choose 1 Power. It permanently costs 0. At combat start, move it from draw pile to hand."
      },
      "PAELS_HORN.description": {
        title: "Pael's Horn",
        vanilla: "On pickup, add 2 Relax to your deck.",
        desc: "On pickup, add 1 Relax and 1 upgraded Relax+ to your deck."
      },
      "BLACK_STAR.description": {
        title: "Black Star",
        vanilla: "Elites drop 1 additional relic.",
        desc: "Elites drop 1 additional relic. If obtained in Act 3 or later, immediately gain 1 random relic."
      },
      "WAR_HAMMER.description": {
        title: "War Hammer",
        vanilla: "Whenever you defeat an Elite, upgrade 4 random cards.",
        desc: "On pickup, choose 2 cards to upgrade. Whenever you defeat an Elite, upgrade 4 random cards."
      },
      "PAELS_TOOTH.description": {
        title: "Pael's Tooth",
        vanilla: "On pickup, remove 5 upgradable cards. After each combat, randomly return 1 of them upgraded.",
        desc: "On pickup, remove 5 cards. After every 2 non-boss combats, choose 1 stored card to return upgraded. After the act boss, remaining stored cards are removed permanently."
      },
      "SEAL_OF_GOLD.description": {
        title: "Seal of Gold",
        vanilla: "At the start of your turn, if you have at least 5 Gold, spend 5 Gold to gain 1 Energy.",
        desc: "Gain 1 Energy. Add 2 playable Debt Curses to your deck. Debt costs 1, Exhausts, and loses up to 5 Gold when exhausted."
      },
      "SOZU.description": {
        title: "Sozu",
        vanilla: "Gain 1 Energy at the start of each turn. You can no longer obtain Potions.",
        desc: "Gain 1 Energy. On pickup, fill all empty potion slots. You can no longer obtain Potions."
      },
      "ECTOPLASM.description": {
        title: "Ectoplasm",
        vanilla: "Gain 1 Energy at the start of each turn. You can no longer obtain Gold.",
        desc: "Gain 1 Energy. On pickup, gain 250 Gold. You can no longer obtain Gold."
      },
      "FIDDLE.description": {
        title: "Fiddle",
        vanilla: "At the start of each turn, draw 2 additional cards. Other draw effects during your turn are prevented.",
        desc: "At the start of each turn, draw until you have 7 cards in hand. During your turn, draw effects cannot put your hand above 7 cards."
      },
      "IRON_CLUB.description": {
        title: "Iron Club",
        vanilla: "Every 4 cards you play, draw 1 card.",
        desc: "Every 5 cards you play, draw 1 card."
      },
      "BRILLIANT_SCARF.description": {
        title: "Brilliant Scarf",
        vanilla: "The 5th card you play from hand each turn costs 0.",
        desc: "The 6th card you play from hand each turn costs 0."
      },
      "BEAUTIFUL_BRACELET.description": {
        title: "Beautiful Bracelet",
        vanilla: "On pickup, choose 3 cards in your deck and enchant them with Swift 3.",
        desc: "On pickup, choose 3 cards in your deck and enchant them with Swift 2."
      },
      "MUSIC_BOX.description": {
        title: "Music Box",
        vanilla: "Each turn, after you play your first Attack, create 1 Ethereal copy in hand.",
        desc: "Each turn, after you play your first Attack, create 1 copy in hand. The copy costs 1 less this turn and has Ethereal and Exhaust."
      },
      "CROSSBOW.description": {
        title: "Crossbow",
        vanilla: "At the start of your turn, add 1 random Attack to hand. It costs 0 this turn.",
        desc: "At the start of your turn, you may add 1 random Attack to hand. It costs 1 less this turn and has Ethereal and Exhaust."
      },
      "TOASTY_MITTENS.description": {
        title: "Toasty Mittens",
        vanilla: "Before your turn draw, exhaust the top card of your draw pile and gain 1 Strength. On turn 1 it prefers a non-Innate card.",
        desc: "Before drawing each turn, view the top card of your draw pile. You may exhaust it to gain 1 Strength."
      },
      "WHISPERING_EARRING.description": {
        title: "Whispering Earring",
        vanilla: "Gain 1 Energy at the start of each turn. On turn 1, Vakuu auto-plays up to 13 playable cards.",
        desc: "Gain 1 Energy. During the first 3 turns of each combat, after draw, auto-play your highest-cost playable card."
      },
      "MEAT_CLEAVER.description": {
        title: "Meat Cleaver",
        vanilla: "Adds Cook at Rest Sites: remove 2 cards and gain 9 Max HP.",
        desc: "Adds Butcher at Rest Sites: remove 2 cards and lose 5 HP."
      },
      "BLOOD_SOAKED_ROSE.description": {
        title: "Blood-Soaked Rose",
        vanilla: "Relic: on pickup, add 1 Enthralled to your deck; gain 1 Energy at the start of each turn. Enthralled: 2-cost Curse, Eternal; while in hand, it must be played first.",
        desc: "Relic body remains +1 Energy and 1 Enthralled."
      },
      "ENTHRALLED.description": {
        title: "Enthralled",
        vanilla: "2-cost Curse. Eternal. While in hand, it must be played before other cards; it has no effect after play.",
        desc: "2-cost Curse. Eternal. While in hand, it must be played before other cards; when played, gain 10 Block."
      },
      "BRIGHTEST_FLAME.description": {
        title: "Brightest Flame",
        vanilla: "Gain 2 Energy, draw 2 cards, and lose 1 Max HP; upgraded gains 3 Energy and draws 3 cards.",
        desc: "Adds Exhaust. Gain 2 Energy, draw 3 cards, and lose 1 Max HP; upgraded gains 3 Energy and draws 4 cards."
      },
      "EZMB_URDA_SEEDLING.description": {
        desc: "0-cost Skill. Exhaust. Gain 4 Block; upgraded gains 7 Block."
      },
      "EZMB_URDA_SEEDBED.description": {
        desc: "1-cost Skill. Exhaust. Gain 4 Block and set up a 2-space Seedbed. Later Temporary Status cards, Temporary Curse cards, or Blight Sprouts that enter hand are planted first; each planted card adds 1 Withered Husk. Upgraded: gain 6 Block, capacity becomes 3, and it plants 1 matching card from draw or discard."
      },
      "EZMB_URDA_RAIN_BREATH.description": {
        desc: "0-cost temporary Skill. Exhaust. Gain 5 Block and draw 1 card."
      },
      "EZMB_WITHERED_HUSK.description": {
        desc: "Temporary Curse. Ethereal, Exhaust. When exhausted, gain 3 Block. Seedbed cannot plant this."
      },
      "EZMB_MORVI_ARCHIVE_DRAW_PAGE.description": {
        desc: "0-cost temporary page. Ethereal, Exhaust. Draw 2 cards."
      },
      "EZMB_MORVI_ARCHIVE_VEIL_PAGE.description": {
        desc: "0-cost temporary page. Ethereal, Exhaust. Gain 14 Block."
      },
      "EZMB_MORVI_ARCHIVE_BURN_PAGE.description": {
        desc: "0-cost temporary page. Ethereal, Exhaust. Deal 10 damage to all enemies."
      },
      "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.description": {
        desc: "0-cost temporary page. Ethereal, Exhaust. The next card you play this turn costs 0."
      },
      "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description": {
        desc: "0-cost temporary page. Ethereal, Exhaust. Gain 2 temporary Strength."
      },
      "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description": {
        desc: "0-cost temporary page. Ethereal, Exhaust. Gain 2 temporary Dexterity."
      },
      "EZMB_VAKUU_KNIFE_CONTRACT.description": {
        desc: "0-cost temporary Contract. Ethereal, Exhaust. Deal 24 damage to Vakuu and lose 4 HP. If any Stolen Lock remains, break 1 and add 1 Blood Debt."
      },
      "EZMB_VAKUU_TEMPTATION.description": {
        desc: "0-cost temporary Contract. Ethereal, Exhaust. Gain 2 Energy, draw 2 cards, and lose 5 HP. If any Stolen Lock remains, break 1 and add 1 Blood Debt."
      },
      "EZMB_VAKUU_SHELTER_CONTRACT.description": {
        desc: "0-cost temporary Contract. Ethereal, Exhaust. Gain 22 Block and remove 1 Blood Debt."
      },
      "EZMB_VAKUU_TRICK_CONTRACT.description": {
        desc: "0-cost temporary Contract. Ethereal, Exhaust. Break 1 Stolen Lock and add 2 Blood Debt. Until Vakuu acts, its attacks deal 6 more damage."
      },
      "EZMB_VAKUU_CASH_OUT_CONTRACT.description": {
        desc: "0-cost temporary Contract. Ethereal, Exhaust. End the Vakuu fight and take the loot from broken locks. Can be played after at least 1 lock is broken."
      },
      "FIREMARK_MIGHT.description": {
        desc: "The Firemark host starts with 1/2/4 Strength. Unblocked attack damage builds Heat; at 2 Heat, the next attack gains +1/+2/+4 damage. Overflow gives 1 attacking secondary enemy 1/1/2 temporary Strength."
      },
      "FIREMARK_GIANT.description": {
        desc: "The Firemark host has +20%/+30%/+45% Max HP. At half HP, a Molten Core opens; deal 20%/25%/30% original Max HP during the window to break it. A broken core weakens the host and deals 6/12/24 overflow damage to 1 secondary enemy."
      },
      "FIREMARK_FORGE_ARMOR.description": {
        desc: "At the start of your turn, the Firemark host gains 8/14/24 Molten Armor. If the host has no Block at turn end, the next Molten Armor is skipped. Overflow gives 1 secondary enemy 3/6/12 Block."
      },
      "FIREMARK_CONSTANT_HEAL.description": {
        desc: "At enemy turn end, the Firemark host heals 4/8/16 HP. Deal 12/24/48 damage in the round to interrupt the heal. If it heals, overflow heals 1 damaged secondary enemy for 2/4/8 HP."
      },
      "BANNER_VANGUARD.description": {
        desc: "Enemies start with temporary Strength. Act 1/2/3 values are 1/2/4. It is removed at the start of round 3."
      },
      "BANNER_SHIELDWALL.description": {
        desc: "Multi-enemy fights only. One enemy is the bannerbearer. While it lives, other enemies gain Block after enemy turns; when it dies, they gain 5/10/20 Block."
      },
      "BANNER_BLOOD_PRIZE.description": {
        desc: "Kill the marked enemy before round 3 ends to gain 15/30/55 Gold after combat. If it survives, it gains 1/2/4 Strength and 1/1/2 Artifact."
      },
      "BANNER_PRESSING_LINE.description": {
        desc: "From the 4th card each turn, the enemy line gains charge, max 3. Charge gives 4-6/8-12/16-24 Block; max charge adds +1/2/4 attack damage."
      },
      "BANNER_LAST_STAND.description": {
        desc: "Multi-enemy fights only. When the first enemy dies, the remaining enemies gain Block and temporary Strength. Strength values by act are 1/2/4."
      },
      "crystal_sphere_peek": {
        title: "Crystal Sphere Peek",
        vanilla: "Vanilla has no peek button.",
        current: "Adds a peek button to the Crystal Sphere minigame. It only changes mask visibility and does not grant rewards.",
        tags: ["Preview tool"]
      },
      "transform_preview": {
        title: "Deterministic Transform Preview",
        vanilla: "Vanilla does not show the exact transform result.",
        current: "Uses a copied RNG snapshot to predict transform results. It creates no card and does not advance real RNG.",
        tags: ["Preview tool"]
      }
    },
    knownIssues: [
      ["High priority", "Live-load evidence needs a refresh", "The current package needs a fresh full load test with a godot.log that matches the latest build."],
      ["Needs verification", "Ancient node UI", "Urda, Morvi, Lotha, normal Vakuu, and hidden Vakuu combat need screenshots, logs, and hover text readability proof."],
      ["Needs verification", "Vakuu trial", "Victory return, failure/death resolution, save-load behavior, and multiplayer boundaries need more live testing."],
      ["Needs verification", "Ascension 11-20", "A11 route clicks, A12/A16/A19/A20 combat rules, Rootblight post-combat resolution, save-load behavior, and two-player co-op need full test records."],
      ["Monitoring", "Public forum", "The forum now uses GitHub Pages and Supabase. Remaining operational risks are free-plan sleep, cache refreshes, anonymous spam, and the lack of a public moderation UI."],
      ["Asset note", "Original game images", "Vanilla relic icons and card portraits are approved for this site and are included only for the update entries that use them."]
    ],
    changeLog: [
      ["2026-05-23 · Gameplay text sync", "Resynced website localization and refreshed Seedbed, Rain Breath, Closed Court, Vakuu Trial contracts, A12 Firemark overflow, and A19/A20 Boss dedicated ability display text."],
      ["2026-05-22 · Website rebuild", "The site now has four main pages: updates, download and install, forum, and known issues with changelog."],
      ["Current package", "SpirePlus-v0.1.0-private-beta.0.zip; the in-game display name is Spire Plus."],
      ["Ancient content", "Urda, Morvi, and Lotha are included as new Ancients. The Vakuu trial remains hidden behind test gates."],
      ["Ascension content", "A11-A20 is included in the private test build. Single-player and host multiplayer selection are available; full co-op play still needs verification."],
      ["Preview tools", "Crystal Sphere peek and deterministic transform preview are merged into Spire Plus and are no longer shipped as a separate package."]
    ]
  }
};

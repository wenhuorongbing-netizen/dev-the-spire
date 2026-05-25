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
  "让《杀戮尖塔 2》的奖励更敢拿",
  "Spire Plus 做了什么",
  "Stronger rewards with readable costs",
  "What Spire Plus Changes",
  "若合适房间不足，至少放入2个",
  "可附魔牌",
  "If there are not enough suitable rooms, at least 2 are placed.",
  "Only Common, Uncommon, or Rare Attacks and Skills can receive Fission.",
  "Vakuu's Sere Talon",
  "Tanx Claws",
  "On pickup, choose 1 of 4 Curses. Add it, 2 Wish, and 1 Wish+.",
  "Transforms up to 6 cards into upgraded Maul."
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
  EZMB_URDA_SEEDBED: "1费技能。消耗。获得8点格挡，设置2格苗床，并立即从抽牌堆或弃牌堆种下至多1张可种下的牌。之后临时状态牌、临时诅咒牌、根芽或根蚀进入手牌前，会先被种下：离开本场战斗，不进入手牌，你获得1张枯壳。种下是苗床替你处理这张牌；它不是打出、弃牌或消耗，不触发这些联动。临时负面牌本战不再出现；永久诅咒不能种下，也不会被删除。根芽不算打出，但按已处理结算，战后不会生成根蚀 I。根蚀只在本场停住，仍按同等级留在主牌组，战后不升级、不分裂、不移除、不降级。升级后获得12点格挡，设置3格苗床，并立即种下至多2张可种下的牌。",
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
  FIREMARK_CONSTANT_HEAL: "敌方回合结束时，火印宿主回复4/8/16点生命；本轮造成18/36/72点伤害可中断治疗；治疗成功时，溢火为1名受伤副目标回复2/4/8点生命。",
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
    detail("打断", "本轮对宿主造成18/36/72点伤害可中断本次治疗。", "Interrupt", "Deal 18/36/72 damage to the host during the round to interrupt that heal."),
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

function mechanic(id, title, desc, bullets, terms, relatedItemKeys = [], relatedMechanicIds = [], options = {}) {
  return {
    id,
    title,
    titleEn: options.titleEn,
    desc,
    descEn: options.descEn,
    bullets,
    bulletsEn: options.bulletsEn,
    terms,
    termsEn: options.termsEn,
    relatedItemKeys,
    relatedMechanicIds,
    tags: options.tags || ["机制"],
    keywordClass: options.keywordClass || "sts-keyword-gold",
    icon: options.icon || "assets/card_portraits/morvi_archive_pages.png"
  };
}

const mechanicGlossary = [
  mechanic(
    "archive_pages",
    "档案页",
    "莫尔维的0费临时页。逾期书库会在战斗开始时给3张随机档案页；每张页只在当前战斗存在。",
    ["抽取页抽2张牌。", "遮蔽页获得14点格挡。", "焚页对所有敌人造成10点伤害。", "折价页让本回合下一张牌费用变为0。", "勇武页获得2点临时力量。", "灵巧页获得2点临时敏捷。"],
    ["档案页", "临时页", "抽取页", "遮蔽页", "焚页", "折价页", "勇武页", "灵巧页"],
    [
      "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.description",
      "EZMB_MORVI_ARCHIVE_DRAW_PAGE.description",
      "EZMB_MORVI_ARCHIVE_VEIL_PAGE.description",
      "EZMB_MORVI_ARCHIVE_BURN_PAGE.description",
      "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.description",
      "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description",
      "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description"
    ],
    ["temporary"],
    {
      titleEn: "Archive Pages",
      descEn: "Morvi's 0-cost temporary pages. Overdue Library adds 3 random Archive Pages at combat start; each page exists only for the current combat.",
      bulletsEn: ["Draw Page draws 2 cards.", "Veil Page gains 14 Block.", "Burn Page deals 10 damage to all enemies.", "Discount Page makes the next card played this turn cost 0.", "Bravery Page gains 2 temporary Strength.", "Dexterity Page gains 2 temporary Dexterity."],
      termsEn: ["Archive Page", "Archive Pages", "Draw Page", "Veil Page", "Burn Page", "Discount Page", "Bravery Page", "Dexterity Page", "temporary page", "temporary pages"]
    }
  ),
  mechanic(
    "temporary",
    "临时",
    "临时牌只服务当前战斗或当前生成流程。它们通常不会进入长期牌组，战斗后会被移除。",
    ["临时牌可以正常被打出、消耗或被其它机制处理。", "苗床会优先种下临时状态牌、临时诅咒牌、根芽和根蚀。", "档案页、契约、雨息、幼芽、枯壳都属于这类短期资源。"],
    ["临时", "临时牌"],
    ["EZMB_URDA_SEEDLING.description", "EZMB_URDA_RAIN_BREATH.description", "EZMB_WITHERED_HUSK.description"],
    ["archive_pages", "contract", "seedbed"],
    {
      titleEn: "Temporary",
      descEn: "Temporary cards serve the current combat or generation flow. They usually do not enter the long-term deck and are removed after combat.",
      bulletsEn: ["Temporary cards can still be played, exhausted, or handled by other mechanics.", "Seedbed plants Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight first.", "Archive Pages, Contracts, Rain Breath, Seedling, and Withered Husk are short-term resources."],
      termsEn: ["Temporary", "temporary card", "temporary cards"]
    }
  ),
  mechanic(
    "seedbed",
    "苗床",
    "乌尔妲的负面牌处理轴。它先给一张合格防御牌的格挡，再铺下几个空位，把之后要进手的临时负面牌、根芽或根蚀截住，换成可消耗的枯壳。",
    ["打出苗床：获得8点格挡、设置2格苗床，并立即从抽牌堆或弃牌堆种下至多1张可种下的牌；苗床+获得12点格挡、设置3格，并立即种下至多2张。", "苗床会处理临时状态牌、临时诅咒牌、根芽和根蚀；不会处理永久诅咒、枯壳或正向临时页。", "每种下1张，加入1张枯壳；枯壳是临时诅咒，被消耗时获得3点格挡。", "种下不是消耗掉诅咒，也不会触发消耗收益。你可以把它理解为：苗床把这张负面牌压在本场战斗之外。", "临时状态牌或临时诅咒牌被种下后，本场战斗不会再回来；永久诅咒不能这样删除。", "种下根芽不算打出，但结算上按已处理计算：它不会触发打出效果，也不会在战后生成根蚀 I。", "种下根蚀只让它这场停住：它仍按同等级留在主牌组，战后不升级、不分裂、不移除、不降级。", "强度判断：苗床保底是8/12点格挡；打出时先处理牌堆污染。每成功种下1张，还少处理1张负面牌，并多拿1张可转成3点格挡的枯壳。"],
    ["苗床", "种下", "枯壳", "根芽", "根蚀"],
    ["EZMB_URDA.pages.INITIAL.options.urda_seedbed.description", "EZMB_URDA_SEEDBED.description", "EZMB_WITHERED_HUSK.description", "EZMB_ROOT_BUD.description"],
    ["temporary", "blight_sprout"],
    {
      titleEn: "Seedbed",
      descEn: "Urda's negative-card handling axis. It gives real Block first, then sets slots that catch later temporary negative cards, Blight Sprouts, or Rootblight and turn them into exhaustable Withered Husks.",
      bulletsEn: ["Playing Seedbed gains 8 Block, sets 2 slots, and immediately plants up to 1 eligible draw/discard card; Seedbed+ gains 12 Block, sets 3 slots, and immediately plants up to 2.", "Seedbed handles Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight. It does not handle permanent Curses, Withered Husk, or beneficial temporary pages.", "Each planted card adds 1 Withered Husk. Withered Husk is a Temporary Curse that gains 3 Block when exhausted.", "Planting is not exhausting a Curse and does not trigger Exhaust rewards. Think of it as Seedbed holding that negative card outside this combat.", "Planted temporary Status and Curse cards do not return this combat; this cannot delete permanent Curses.", "Planting a Blight Sprout is handled without playing it: it triggers no play effects and adds no Rootblight I after combat.", "Planting Rootblight holds it still for this combat only: it remains in your master deck at the same level and does not upgrade, split, get removed, or downgrade after combat.", "Power check: Seedbed starts as 8/12 Block. On play, it first handles deck pollution; each planted card also removes one negative card from this combat and gives a Husk that can become 3 Block."],
      termsEn: ["Seedbed", "Seedbeds", "plant", "planted", "Withered Husk", "Blight Sprout", "Rootblight"]
    }
  ),
  mechanic(
    "plant",
    "种下",
    "种下是苗床的处理动作：符合条件的牌在进入手牌前被苗床截住，移出本场战斗，并换成1张枯壳。",
    ["种下不是打出、弃牌或消耗；不会触发这些关键词和对应联动。", "可种下：临时状态牌、临时诅咒牌、根芽、根蚀。不可种下：永久诅咒、枯壳、正向临时页。", "临时状态牌或临时诅咒牌被种下后，本场不会再出现；这更像把本场污染压住，不是永久删牌。", "种下根芽不算打出，但按已处理结算，战后不会生成根蚀 I；它不会触发“打出根芽”的效果。", "种下根蚀只让它这场停住：这张根蚀仍按同等级留在主牌组，战后不升级、不分裂、不移除、不降级。它也不会因为苗床变好。", "每次种下都会占用1格苗床。格数用完后，后续负面牌按原规则进入手牌。"],
    ["种下", "苗床", "根芽", "枯壳", "根蚀"],
    ["EZMB_URDA_SEEDBED.description", "EZMB_WITHERED_HUSK.description", "EZMB_ROOT_BUD.description"],
    ["seedbed", "blight_sprout", "rootblight"],
    {
      titleEn: "Plant",
      descEn: "Plant is Seedbed's handling action: an eligible card is caught before entering hand, removed from combat, and converted into 1 Withered Husk.",
      bulletsEn: ["Planting is not playing, discarding, or exhausting the card, so those triggers do not fire.", "Can plant: Temporary Status cards, Temporary Curse cards, Blight Sprouts, and Rootblight. Cannot plant: permanent Curses, Withered Husk, or beneficial temporary pages.", "For temporary Status or Curse cards, planting handles them for this combat only. This is not permanent card removal.", "Planting a Blight Sprout treats that Sprout as handled, so it will not add Rootblight I after combat. It does not count as playing the Sprout.", "Planting Rootblight only holds it still for this combat: it stays in your master deck at the same level and does not upgrade, split, get removed, or downgrade. Seedbed also does not improve it.", "Each plant consumes 1 Seedbed slot. After slots run out, later negative cards follow their normal rules."],
      termsEn: ["Plant", "plant", "planted", "planting", "Seedbed", "Blight Sprout", "Withered Husk", "Rootblight"]
    }
  ),
  mechanic(
    "rootblight",
    "根蚀",
    "A14开始出现的长期污染。根蚀是主牌组里的诅咒，不是根芽；苗床可以把本战进入手牌的根蚀种下，让它这一场停滞。",
    ["根蚀 I：打出后从主牌组移除；战后仍留在主牌组中则变为根蚀 II。", "根蚀 II：打出后移除，战后加入根蚀 I；战后仍留在主牌组中则变为根蚀 III。", "根蚀 III：打出后移除，战后加入根蚀 II；如果继续不处理，它保持根蚀 III，第一次额外加入根蚀 I。没有第四阶段。", "根蚀被苗床种下时，这场战斗结束不改变阶段。它不升级、不分裂，也不会被移除或降级。", "最多4张根蚀。休息会移除最高等级根蚀。"],
    ["根蚀", "根蚀 I", "根蚀 II", "根蚀 III"],
    ["LEVEL_14.description", "EZMB_ROOT.description", "EZMB_DEEP_ROOT.description", "EZMB_ROOTBLIGHT_III.description", "EZMB_ROOT_BUD.description"],
    ["blight_sprout", "seedbed"],
    {
      titleEn: "Rootblight",
      descEn: "A long-term pollution system starting at A14. Rootblight is a master-deck Curse, not a Blight Sprout. Seedbed can plant Rootblight that enters hand and freeze it for one combat.",
      bulletsEn: ["Rootblight I: playing it removes it from the master deck; if still in the master deck after combat, it becomes Rootblight II.", "Rootblight II: playing it removes it and adds Rootblight I after combat; if still in the master deck after combat, it becomes Rootblight III.", "Rootblight III: playing it removes it and adds Rootblight II after combat. If ignored again, it stays Rootblight III and adds Rootblight I the first time. There is no stage IV.", "When Seedbed plants Rootblight, it does not change stage at combat end. It does not upgrade, split, get removed, or downgrade.", "Max 4 Rootblights. Resting removes the highest-stage Rootblight."],
      termsEn: ["Rootblight", "Rootblights", "Rootblight I", "Rootblight II", "Rootblight III"],
      keywordClass: "sts-keyword-purple"
    }
  ),
  mechanic(
    "blight_sprout",
    "根芽",
    "根芽是短期压力牌。第3或第4回合开始时，如果还没进过手牌，会被放到抽牌堆顶。",
    ["根芽本身是2费临时诅咒。打出它会处理掉本场根芽压力。", "见到后不打出也不种下，战斗后加入根蚀 I。", "从未见到则枯萎，不进入长期牌组。", "苗床可以种下根芽：不算打出，但按已处理结算，会阻止这张根芽战后生成根蚀 I。"],
    ["根芽"],
    ["LEVEL_15.description", "LEVEL_18.description", "EZMB_ROOT_BUD.description", "EZMB_URDA_SEEDBED.description"],
    ["rootblight", "seedbed"],
    {
      titleEn: "Blight Sprout",
      descEn: "Blight Sprout is a short-term pressure card. On round 3 or 4, if it has not entered hand, it is placed on top of the draw pile.",
      bulletsEn: ["Blight Sprout itself is a 2-cost Temporary Curse. Playing it handles the Sprout pressure for that combat.", "If seen and neither played nor planted, it adds Rootblight I after combat.", "If never seen, it withers and does not enter the long-term deck.", "Seedbed can plant Blight Sprout: it is treated as handled without counting as played, preventing that Sprout from adding Rootblight I after combat."],
      termsEn: ["Blight Sprout", "Blight Sprouts", "Sprout", "Sprouts"],
      keywordClass: "sts-keyword-purple"
    }
  ),
  mechanic(
    "blood_debt",
    "血债",
    "瓦库试炼的战斗债务。血债越高，瓦库每段攻击越痛；试炼结算时会先用破锁赃物偿还。",
    ["每层血债使瓦库每段攻击伤害+2。", "试炼结束时，每层先扣15赃物金币。", "赃物不足时，剩余债务以非致命生命支付。"],
    ["血债"],
    ["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description", "EZMB_VAKUU_KNIFE_CONTRACT.description", "EZMB_VAKUU_TEMPTATION.description", "EZMB_VAKUU_SHELTER_CONTRACT.description", "EZMB_VAKUU_TRICK_CONTRACT.description"],
    ["contract", "stolen_lock"],
    {
      titleEn: "Blood Debt",
      descEn: "Vakuu Trial's combat debt. More Blood Debt makes every Vakuu attack hit harder; trial loot pays it off first.",
      bulletsEn: ["Each stack makes each Vakuu attack hit deal 2 more damage.", "At trial end, each stack removes 15 loot Gold first.", "Unpaid debt costs nonlethal HP."],
      termsEn: ["Blood Debt"],
      keywordClass: "sts-keyword-purple",
      icon: "assets/powers/vakuu_blood_debt.png"
    }
  ),
  mechanic(
    "contract",
    "契约",
    "瓦库试炼第1、3、5回合提供契约选择。契约帮你破锁、抽牌、防御或收手，但多数会提高血债。",
    ["刀契：造成伤害，失去生命，破1把锁并增加1层血债。", "金契：获得能量并抽牌，失去生命，破1把锁并增加1层血债。", "避债契：获得格挡并移除血债。", "诈契：破1把锁并增加血债，同时让瓦库下一次攻击更痛。", "兑现：至少破1把锁后结束战斗并拿走破锁收益。"],
    ["契约", "刀契", "金契", "避债契", "诈契", "兑现"],
    ["EZMB_VAKUU_KNIFE_CONTRACT.description", "EZMB_VAKUU_TEMPTATION.description", "EZMB_VAKUU_SHELTER_CONTRACT.description", "EZMB_VAKUU_TRICK_CONTRACT.description", "EZMB_VAKUU_CASH_OUT_CONTRACT.description"],
    ["blood_debt", "stolen_lock"],
    {
      titleEn: "Contracts",
      descEn: "Vakuu Trial offers Contracts on turns 1, 3, and 5. They help break locks, draw, defend, or cash out, but most increase Blood Debt.",
      bulletsEn: ["Knife Contract: deals damage, loses HP, breaks 1 lock, and adds 1 Blood Debt.", "Gold Contract: gains Energy and draws, loses HP, breaks 1 lock, and adds 1 Blood Debt.", "Avoid Debt: gains Block and removes Blood Debt.", "Fraud Contract: breaks 1 lock and adds Blood Debt, while making Vakuu's next attack stronger.", "Cash Out: after breaking at least 1 lock, ends the fight and takes broken-lock loot."],
      termsEn: ["Contract", "Contracts", "Knife Contract", "Gold Contract", "Avoid Debt", "Fraud Contract", "Cash Out"]
    }
  ),
  mechanic(
    "stolen_lock",
    "赃物锁",
    "瓦库试炼的收益门槛。你需要至少打破1把锁才能收手；破得越多，最后的赃物和额外祝福选择越多。",
    ["契约和伤害窗口会打破赃物锁。", "破锁收益会先偿还血债，剩余才归玩家。", "战斗没有普通战斗奖励。"],
    ["赃物锁", "破锁"],
    ["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description", "EZMB_VAKUU_CASH_OUT_CONTRACT.description"],
    ["blood_debt", "contract"],
    {
      titleEn: "Stolen Locks",
      descEn: "Vakuu Trial's loot gate. You need to break at least 1 lock before cashing out; more broken locks mean more loot and blessing choices.",
      bulletsEn: ["Contracts and damage windows break Stolen Locks.", "Lock loot pays Blood Debt first; only the remainder is kept.", "The fight has no normal combat rewards."],
      termsEn: ["Stolen Lock", "Stolen Locks", "lock", "locks"]
    }
  ),
  mechanic(
    "verdict",
    "裁决",
    "洛莎的第4回合爆发资源。裁决层数会强化接下来打出的非状态牌。",
    ["攻击牌和技能牌会额外打出1次。", "能力牌不会额外打出，改为本次费用变为0并抽1张牌。", "延期判决提供3层裁决；若战斗在第4回合前结束，回复4点生命。"],
    ["裁决"],
    ["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"],
    ["replay"],
    {
      titleEn: "Verdict",
      descEn: "Lotha's turn-4 burst resource. Verdict stacks empower the next non-Status cards played.",
      bulletsEn: ["Attacks and Skills play 1 extra time.", "Powers do not replay; they cost 0 for that play and draw 1 card instead.", "Deferred Verdict grants 3 Verdict on turn 4; if combat ends before turn 4, heal 4 HP."],
      termsEn: ["Verdict"],
      keywordClass: "sts-keyword-gold"
    }
  ),
  mechanic(
    "replay",
    "额外打出",
    "当前文本中写作“额外打出X次”。它会再次结算攻击牌或技能牌；能力牌使用安全替代收益。",
    ["攻击牌和技能牌按原效果额外结算。", "能力牌不额外打出，通常改为0费、抽牌等安全收益。", "同一个来源不会递归触发自己。"],
    ["额外打出", "重放"],
    ["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description", "BOSS_SEAL_CHOSEN_DECREE.summary"],
    ["verdict"],
    {
      titleEn: "Extra Play",
      descEn: "Current text says \"play extra times\". It resolves Attacks and Skills again; Powers use safe replacement rewards.",
      bulletsEn: ["Attacks and Skills resolve their effect again.", "Powers are not replayed; they usually become 0-cost and draw cards or similar safe rewards.", "A source cannot recursively trigger itself."],
      termsEn: ["play extra time", "play extra times", "extra play", "Replay"],
      keywordClass: "sts-keyword-gold"
    }
  ),
  mechanic(
    "debt",
    "债务",
    "莫尔维和黄金之印都会使用的资源压力。债务会把当前的强度提前给你，再在之后用金币或生命偿还。",
    ["债务清算立即给220金币、删牌和升级，然后记录320债务。", "每场战斗后偿还40金币；金币不足时每缺10金币失去3点非致命生命。", "黄金之印会加入可打出的债务诅咒，被消耗时失去至多5金币。"],
    ["债务"],
    ["EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description", "SEAL_OF_GOLD.description"],
    ["red_ink_debt"],
    {
      titleEn: "Debt",
      descEn: "A resource pressure used by Morvi and Seal of Gold. Debt gives power now and collects Gold or HP later.",
      bulletsEn: ["Debt Settlement immediately grants 220 Gold, removals, and upgrades, then records 320 Debt.", "After each combat, repay 40 Gold; if short, lose 3 nonlethal HP per missing 10 Gold.", "Seal of Gold adds playable Debt Curses that lose up to 5 Gold when exhausted."],
      termsEn: ["Debt"],
      keywordClass: "sts-keyword-gold"
    }
  ),
  mechanic(
    "red_ink_debt",
    "红墨债",
    "红墨透支的短期账。你在0能量时主动换取抽牌和能量，战斗后再付账。",
    ["每回合限1次。", "抽2张牌并获得1点能量。", "战斗后支付12金币；金币不足则失去3点生命。"],
    ["红墨债", "透支"],
    ["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description", "EZMB_MORVI_RED_INK_OVERDRAFT.description"],
    ["debt"],
    {
      titleEn: "Red-Ink Debt",
      descEn: "Red Ink Overdraft's short-term bill. At 0 Energy, you actively trade later payment for cards and Energy now.",
      bulletsEn: ["Once per turn.", "Draw 2 cards and gain 1 Energy.", "After combat, pay 12 Gold; if short, lose 3 HP."],
      termsEn: ["red-ink debt", "Overdraft", "Red Ink Overdraft"],
      keywordClass: "sts-keyword-gold"
    }
  ),
  mechanic(
    "fission",
    "裂变",
    "A13奖励附魔。裂变只会出现在合格的攻击牌和技能牌上。",
    ["普通战10%，战旗房15%，火印精英20%，首领5%。", "只作用于普通、罕见、稀有攻击或技能。", "X费、0费、已有附魔、消耗牌和不能生成附魔的牌不会获得裂变。", "裂变牌耗能-1，打出后消耗。"],
    ["裂变"],
    ["LEVEL_13.description"],
    [],
    {
      titleEn: "Fission",
      descEn: "An A13 reward enchantment. Fission appears only on eligible Attacks and Skills.",
      bulletsEn: ["Rates: normal combat 10%, Banner Room 15%, Firemarked Elite 20%, Boss 5%.", "Only Common, Uncommon, or Rare Attacks and Skills are eligible.", "X-cost, 0-cost, already enchanted, Exhaust, and non-enchantable cards are excluded.", "Fission reduces cost by 1 and Exhausts the card after play."],
      termsEn: ["Fission"],
      keywordClass: "sts-keyword-gold"
    }
  ),
  mechanic(
    "firemark",
    "火印",
    "A12的强化精英规则。火印会选择1名宿主，给它完整火印，并让战斗带有公开反制窗口。",
    ["火印精英奖励更高，击败后获得铸令；已有铸令时改为15金币。", "火印种类包括烈力、巨体、锻甲、恒愈。", "溢火每次最多影响1名非召唤副目标。"],
    ["火印", "火印精英"],
    ["LEVEL_12.description", "FIREMARK_MIGHT.description", "FIREMARK_GIANT.description", "FIREMARK_FORGE_ARMOR.description", "FIREMARK_CONSTANT_HEAL.description"],
    ["overflow", "forge_token"],
    {
      titleEn: "Firemark",
      descEn: "A12's enhanced Elite rule. One host receives the full Firemark, and the fight exposes a counterplay window.",
      bulletsEn: ["Firemarked Elites have better rewards. Defeat one to gain a Forge Token; if you already have one, gain 15 Gold instead.", "Firemarks include Might, Giant, Forge Armor, and Constant Heal.", "Overflow affects at most 1 non-summoned secondary target each time."],
      termsEn: ["Firemark", "Firemarked Elite", "Firemarked Elites"]
    }
  ),
  mechanic(
    "overflow",
    "溢火",
    "火印的副目标扩散。宿主触发或被打破窗口时，溢火会把一小部分效果给另一名敌人。",
    ["烈力：给1名正在攻击的副目标临时力量。", "巨体：打破熔核时对1名副目标造成溢火伤害。", "锻甲：给1名副目标格挡。", "恒愈：治疗成功时为1名受伤副目标回复生命。"],
    ["溢火"],
    ["FIREMARK_MIGHT.description", "FIREMARK_GIANT.description", "FIREMARK_FORGE_ARMOR.description", "FIREMARK_CONSTANT_HEAL.description"],
    ["firemark"],
    {
      titleEn: "Overflow",
      descEn: "Firemark spillover. When the host triggers or its window breaks, Overflow gives a smaller effect to another enemy.",
      bulletsEn: ["Might: gives temporary Strength to 1 attacking secondary enemy.", "Giant: breaking Molten Core deals overflow damage to 1 secondary enemy.", "Forge Armor: gives Block to 1 secondary enemy.", "Constant Heal: successful healing restores HP to 1 damaged secondary enemy."],
      termsEn: ["Overflow"]
    }
  ),
  mechanic(
    "forge_token",
    "铸令",
    "击败火印精英后获得的下个休息处奖励。",
    ["休息：随机升级1张可升级普通/罕见牌；没有目标则回复5点生命。", "锻造：额外回复7点生命。", "若已经持有铸令，再击败火印精英改为获得15金币。"],
    ["铸令"],
    ["LEVEL_12.description"],
    ["firemark"],
    {
      titleEn: "Forge Token",
      descEn: "The next-rest-site reward from defeating a Firemarked Elite.",
      bulletsEn: ["Rest: randomly upgrades 1 upgradable Common/Uncommon card; if none exists, heals 5 HP.", "Smith: additionally heals 7 HP.", "If you already have a Forge Token, another Firemarked Elite converts to 15 Gold."],
      termsEn: ["Forge Token"]
    }
  ),
  mechanic(
    "banner",
    "战旗",
    "A16的公开规则强化普通战。战旗房会在进房前显示具体规则，奖励里也有更高裂变率。",
    ["战旗池：先锋、盾阵、血赏、压阵、残阵。", "盾阵和残阵需要多敌人战；单敌人战回退为血赏。", "战旗房卡牌奖励裂变率为15%。"],
    ["战旗", "战旗房"],
    ["LEVEL_16.description", "BANNER_VANGUARD.description", "BANNER_SHIELDWALL.description", "BANNER_BLOOD_PRIZE.description", "BANNER_PRESSING_LINE.description", "BANNER_LAST_STAND.description"],
    ["fission"],
    {
      titleEn: "Banner",
      descEn: "A16's visible-rule enhanced normal combat. Banner Rooms show their exact rule before entry and use a higher Fission reward rate.",
      bulletsEn: ["Banner pool: Vanguard, Shieldwall, Blood Prize, Pressing Line, Last Stand.", "Shieldwall and Last Stand require multi-enemy fights; single-enemy fights fall back to Blood Prize.", "Banner Room card rewards use a 15% Fission chance."],
      termsEn: ["Banner", "Banner Room", "Banner Rooms"]
    }
  ),
  mechanic(
    "royal_decree",
    "御令",
    "女王A19专属能力。女王施加束缚时，其中1张束缚牌会被标记为御令牌。",
    ["打出御令牌不会触发额外惩罚。", "打出非御令束缚牌时，女王获得1层威仪。", "没有打出束缚牌时，女王获得1层威仪，火炬头获得1点力量。"],
    ["御令", "御令牌"],
    ["BOSS_SEAL_CHOSEN_DECREE.summary"],
    ["majesty", "bound"],
    {
      titleEn: "Royal Decree",
      descEn: "Queen's A19 dedicated ability. When Queen applies Bound, 1 Bound card is marked as the Decree.",
      bulletsEn: ["Playing the Decree has no extra penalty.", "Playing a non-Decree Bound card gives Queen 1 Majesty.", "Playing no Bound card gives Queen 1 Majesty and Torch Head 1 Strength."],
      termsEn: ["Royal Decree", "Decree"]
    }
  ),
  mechanic(
    "majesty",
    "威仪",
    "女王御令惩罚产生的防御资源。威仪会强化女王下一次防御或屏障动作。",
    ["每层威仪让下一次防御或屏障动作额外获得8点格挡。", "A19最多2层。", "A20烙印形态上限变为3层，且一次防御或屏障最多消耗2层。"],
    ["威仪"],
    ["BOSS_SEAL_CHOSEN_DECREE.summary"],
    ["royal_decree", "bound"],
    {
      titleEn: "Majesty",
      descEn: "Queen's defensive resource from Royal Decree punishment. Majesty strengthens Queen's next defense or barrier action.",
      bulletsEn: ["Each Majesty adds 8 Block to the next defense or barrier action.", "A19 cap is 2 stacks.", "A20 Branded Form raises the cap to 3 and lets one defense/barrier action spend at most 2 stacks."],
      termsEn: ["Majesty"]
    }
  ),
  mechanic(
    "bound",
    "束缚",
    "女王施加到玩家牌上的限制。御令会在束缚牌中标记1张必须优先处理的目标。",
    ["本网站主要记录Spire Plus新增的御令交互。", "打出正确的御令牌可以避免额外惩罚。", "打出其他束缚牌或完全不处理束缚，会加强敌人。"],
    ["束缚"],
    ["BOSS_SEAL_CHOSEN_DECREE.summary"],
    ["royal_decree", "majesty"],
    {
      titleEn: "Bound",
      descEn: "Queen's card restriction. Royal Decree marks one Bound card as the required target.",
      bulletsEn: ["This site mainly records the new Spire Plus Royal Decree interaction.", "Playing the correct Decree avoids the extra penalty.", "Playing another Bound card or ignoring Bound strengthens enemies."],
      termsEn: ["Bound"]
    }
  ),
  mechanic(
    "trial_branch",
    "试炼枝条",
    "乌尔妲的三场战斗小任务。选中的牌升级入库，但必须在接下来三场战斗里证明自己。",
    ["从4张牌中选1张。", "该牌升级后加入牌组并获得试炼枝条标记。", "接下来3场战斗每场都要打出它；漏掉任何一场都会移除。"],
    ["试炼枝条", "试种枝条"],
    ["EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description"],
    [],
    {
      titleEn: "Trial Branch",
      descEn: "Urda's three-combat mini-quest. The chosen card joins upgraded, but must prove itself over the next three combats.",
      bulletsEn: ["Choose 1 of 4 cards.", "It is upgraded, added to the deck, and marked with Trial Branch.", "Play it in each of the next 3 combats; missing any combat removes it."],
      termsEn: ["Trial Branch"]
    }
  ),
  mechanic(
    "sway",
    "婆娑",
    "预留机制词条。当前包的源码和本地化中没有独立的婆娑定义，也没有战斗触发；网站先保留入口，等后续设计落地后直接补正式效果。",
    ["当前状态：未实装。", "不会影响当前私测包战斗。", "后续若进入设计，会在这里记录具体触发、层数和结算。"],
    ["婆娑"],
    [],
    [],
    {
      titleEn: "Sway",
      descEn: "Reserved mechanic entry. Current source and localization do not define a standalone Sway effect or combat trigger; the site keeps this anchor for a later design.",
      bulletsEn: ["Current state: not implemented.", "It does not affect the current private-test package.", "If it enters the design later, trigger, stacks, and resolution will be recorded here."],
      termsEn: ["Sway"],
      keywordClass: "sts-keyword-purple"
    }
  )
];

window.SPIRE_PLUS_DATA = {
  labels: {
    brandSub: "Spire Plus，由温火融冰制作的《杀戮尖塔 2》平衡与高进阶拓展",
    navUpdates: "\u66f4\u65b0\u5185\u5bb9",
    navInstall: "\u4e0b\u8f7d\u4e0e\u5b89\u88c5",
    navForum: "\u8bba\u575b",
    navIssues: "\u5df2\u77e5\u95ee\u9898",
    navAbout: "关于",
    releaseLine: "",
    heroTitle: "Spire Plus",
    heroCopy: "让《杀戮尖塔 2》的奖励更敢拿、代价更看得懂，高进阶也有新的路线与战斗判断。",
    modIntroTitle: "Spire Plus 做了什么",
    featAscensionTitle: "新增进阶 20",
    featAscensionDesc: "在 A11-A20 中加入新的路线、火印精英、裂变奖励、根蚀、战旗房、首领专属能力和 A20 烙印形态。高进阶会给玩家新的路线、出牌和战斗判断。",
    featPhilosophyTitle: "更多选择，更真实的压力",
    featPhilosophyDesc: "Spire Plus 让奖励更有诱惑，也让代价更具体。强奖励会写清楚成本、触发时机和后续处理，玩家可以提前判断路线、战斗和牌组压力。",
    featRewardTitle: "奖励更强，代价更清楚",
    featRewardDesc: "先古之民和部分原版遗物被重做为明确的交换：你可以拿到更强的启动、路线收益、牌组处理或战斗预知，但要承担血量、债务、污染、路线承诺或出牌限制。",
    aboutTitle: "关于",
    aboutLead: "项目说明、素材来源和发布边界。",
    introTitle: "模组说明",
    introCopy: "Spire Plus 是温火融冰制作的《杀戮尖塔 2》玩法拓展。它把先古之民奖励做成可查看的遗物选择，重做一批原版遗物，并加入 A11-A20 测试进阶、根芽/根蚀、火印精英、战旗房、首领专属能力、水晶球预知和变换预览。核心方向很简单：奖励可以更强，代价必须更清楚。玩家需要知道自己为什么要拿、什么时候会亏、后续怎样处理。",
    download: "\u4e0b\u8f7d\u6a21\u7ec4",
    viewIssues: "\u67e5\u770b\u5df2\u77e5\u95ee\u9898",
    all: "\u5168\u90e8",
    search: "\u641c\u7d22",
    searchPlaceholder: "\u9057\u7269\u3001\u5148\u53e4\u3001\u8fdb\u9636\u3001\u5173\u952e\u8bcd",
    vanilla: "\u539f\u7248",
    current: "\u5f53\u524d",
    lockFocus: "锁定关注",
    pinnedFocus: "已锁定",
    unlockFocus: "取消锁定",
    relatedChanges: "相关改动",
    relatedMechanics: "机制解释",
    mechanicCodex: "机制标签",
    mechanicTag: "机制",
    expandDetails: "\u5177\u4f53\u6548\u679c",
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
  mechanics: mechanicGlossary,
  summary: [],
  package: {
    localDownload: "../publish/SpirePlus-v0.1.0-private-beta.17.zip",
    releaseDownload:
      "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/download/v0.1.0-private-beta.17/SpirePlus-v0.1.0-private-beta.17.zip",
    latestReleaseApi: "https://api.github.com/repos/wenhuorongbing-netizen/dev-the-spire/releases/latest",
    releasesPage: "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/tag/v0.1.0-private-beta.17",
    baseLibRelease: "https://github.com/Alchyr/BaseLib-StS2/releases/download/v3.1.4/BaseLib.3.1.4.zip",
    repository: "https://github.com/wenhuorongbing-netizen/dev-the-spire",
    meta: [
      ["\u6587\u4ef6", "SpirePlus-v0.1.0-private-beta.17.zip"],
      ["\u7248\u672c", "v0.1.0-private-beta.17"],
      ["\u663e\u793a\u540d", "Spire Plus"],
      ["\u4f9d\u8d56", "BaseLib v3.1.4"],
      ["\u6e38\u620f\u7248\u672c", "Slay the Spire 2 v0.106.0"],
      ["\u4f53\u79ef", "18,940,143 \u5b57\u8282"],
      ["\u54c8\u5e0c", "44DFFE46847F6EE096EEDAC02303841E0646C1493D00D10DB66A2B46AF885FD6"]
    ]
  },
  installSteps: [
    "\u4e0b\u8f7d SpirePlus-v0.1.0-private-beta.17.zip\u3002",
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
      ["发布页", "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/tag/v0.1.0-private-beta.17"]
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
        baseRelic("\u5766\u514b\u65af\u5229\u722a", "CLAWS.description", ["\u6495\u54ac", "\u6495\u54ac+", "\u53d8\u5316"], "\u62fe\u53d6\u65f6\u9009\u62e9\u81f3\u591a6\u5f20\u724c\uff0c\u5c06\u5b83\u4eec\u53d8\u5316\u4e3a\u201c\u6495\u54ac\u201d\u3002", "\u9009\u62e9\u81f3\u591a6\u5f20\u724c\uff0c\u5c06\u5b83\u4eec\u53d8\u5316\u4e3a\u201c\u6495\u54ac+\u201d\u3002"),
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
        {
          ...ancient("urda_seedbed", "assets/ancients/urda/options/urda_seedbed.png", ["\u4e4c\u5c14\u59b2"]),
          desc: "第1幕普通战斗卡牌奖励可改拿苗床：失去2点最大生命，加入1张苗床。第一次拿到的苗床自动升级。累计收下4次后，获得10点最大生命。苗床本体是1费8/12点格挡；打出时先从抽牌堆或弃牌堆种下1/2张可种下的牌，之后会截住后续临时负面牌、根芽或根蚀，每张换成枯壳。种下是本战处理，不是打出、弃牌或消耗。根芽按已处理结算；根蚀只在本场停住，仍按同等级留在主牌组。",
          details: [
            detail("获取", "一幕普通战斗卡牌奖励中选择苗床，会结束本次奖励选择并加入1张苗床。", "Pickup", "Choose Seedbed from an Act 1 normal combat card reward; it completes that reward and adds 1 Seedbed."),
            detail("成本", "每次收下苗床失去2点最大生命。第一次收下的苗床会升级。", "Cost", "Each taken Seedbed costs 2 Max HP. The first Seedbed taken is upgraded."),
            detail("累计", "累计收下4次苗床后，获得10点最大生命。", "Completion", "After taking Seedbed 4 times, gain 10 Max HP."),
            detail("强度", "苗床是1费防御牌：8点格挡，并立刻种下至多1张抽牌堆或弃牌堆里的可种下牌。第一次拿到的苗床+为12点格挡、3格，并立刻种下至多2张。每次种下还会给1张可转成3点格挡的枯壳。", "Power", "Seedbed is a 1-cost defensive card: 8 Block, and it immediately plants up to 1 eligible draw/discard card. The first upgraded Seedbed gives 12 Block, 3 slots, and immediately plants up to 2. Each planted card also gives a Husk that can become 3 Block."),
            detail("种下", "苗床会截住之后进入手牌的临时状态牌、临时诅咒牌、根芽或根蚀；该牌离开本场战斗，不触发打出、弃牌或消耗，并加入1张枯壳。种下不是消耗永久诅咒，也不会触发消耗收益。", "Plant", "Seedbed catches later Temporary Status cards, Temporary Curse cards, Blight Sprouts, or Rootblight before they enter hand. The card leaves combat, does not trigger play, discard, or Exhaust effects, and gives 1 Withered Husk. Planting does not exhaust permanent Curses."),
            detail("根系结算", "根芽被种下后不算打出，但按已处理结算，战后不生成根蚀 I。根蚀被种下后只在本场停住，仍按同等级留在主牌组，战后不升级、不分裂、不移除、不降级，也不会变好。", "Root resolution", "A planted Blight Sprout is treated as handled without counting as played and adds no Rootblight I after combat. A planted Rootblight only holds still for this combat and remains in the master deck at the same level.")
          ]
        },
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
      short: "机制",
      title: "先古与进阶机制详解",
      note: "把跨条目机制拆开显示。点击机制词时，只锁定对应条目，不重建整页列表。",
      icon: "assets/ascension/forge_token_status.png",
      defaultVanilla: "",
      items: [
        mechanicCard("blood_debt", "assets/powers/vakuu_blood_debt.png", ["瓦库", "血债"]),
        mechanicCard("stolen_lock", "assets/powers/vakuu_stolen_vault.png", ["瓦库", "赃物锁"]),
        mechanicCard("contract", "assets/card_portraits/vakuu_temptation.png", ["瓦库", "契约"]),
        mechanicCard("forge_token", "assets/ascension/forge_token_status.png", ["A12", "铸令"]),
        mechanicCard("firemark", "assets/ascension/firemarked_elite_indicator.png", ["A12", "火印精英"]),
        mechanicCard("overflow", "assets/ascension/firemark_might_indicator.png", ["A12", "溢火"]),
        mechanicCard("fission", "assets/ascension/fission_enchantment_icon.png", ["A13", "裂变"]),
        mechanicCard("seedbed", "assets/ancients/urda/options/urda_seedbed.png", ["乌尔妲", "苗床"]),
        mechanicCard("plant", "assets/ancients/urda/options/urda_seedbed.png", ["乌尔妲", "种下"]),
        mechanicCard("rootblight", "assets/card_portraits/rootblight_i.png", ["A14", "根蚀"]),
        mechanicCard("blight_sprout", "assets/card_portraits/blight_sprout.png", ["A15", "根芽"]),
        mechanicCard("banner", "assets/ascension/banner_room_indicator.png", ["A16", "战旗房"]),
        mechanicCard("trial_branch", "assets/ancients/urda/options/urda_trial_branch.png", ["乌尔妲", "试炼枝条"]),
        mechanicCard("verdict", "assets/powers/lotha_verdict.png", ["洛莎", "裁决"]),
        mechanicCard("majesty", "assets/ascension/boss_seals/chosen_decree.png", ["A19", "威仪"]),
        mechanicCard("royal_decree", "assets/ascension/boss_seals/chosen_decree.png", ["A19", "御令"]),
        manual("\u6c34\u6676\u7403\u9884\u77e5", "", "\u6c34\u6676\u7403\u5c0f\u6e38\u620f\u4e2d\u663e\u793a\u9884\u77e5\u6309\u94ae\uff1b\u53ea\u6539\u53d8\u906e\u7f69\u53ef\u89c1\u6027\uff0c\u4e0d\u53d1\u653e\u5956\u52b1\u3002", ["\u9884\u89c8\u5de5\u5177"], "assets/ancients/urda/options/urda_root_sight.png"),
        manual("\u53d8\u6362\u771f\u5b9e\u9884\u89c8", "", "\u4f7f\u7528\u590d\u5236\u7684\u968f\u673a\u6570\u5feb\u7167\u9884\u6d4b\u53d8\u6362\u7ed3\u679c\uff1b\u4e0d\u521b\u5efa\u5361\u724c\uff0c\u4e0d\u63a8\u8fdb\u771f\u5b9e\u968f\u673a\u6570\u3002", ["\u9884\u89c8\u5de5\u5177"], "assets/ancients/morvi/options/morvi_blueprint_proof.png")
      ]
    },
    {
      short: "\u5361\u724c",
      title: "\u65b0\u589e\u5361\u724c\u4e0e\u72b6\u6001",
      note: "\u539f\u7248\u6ca1\u6709\u8fd9\u4e9b\u65b0\u589e\u5361\u724c\u548c\u72b6\u6001\u3002",
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
        card("EZMB_VAKUU_CASH_OUT_CONTRACT", ["\u74e6\u5e93"], "assets/card_portraits/vakuu_temptation.png")
      ]
    }
  ],
  knownIssues: [
    ["待测试", "先古之民遗物", "需要测试先古之民所有遗物生效。"],
    ["待测试", "联机", "需要测试联机可以用。"],
    ["待修复", "Mac 文本显示", "目前已知 Mac 文本显示错误，需要修复。"]
  ],
  changeLog: [
    ["2026-05-23 · 玩法文本同步", "网站重新同步当前 mod localization，并更新苗床、雨息、终审封庭、瓦库试炼契约、A12 火印溢火与 A19/A20 首领专属能力展示。"],
    ["2026-05-22 \u00b7 \u7f51\u7ad9\u91cd\u6784", "\u7ad9\u70b9\u6539\u4e3a\u56db\u4e2a\u4e3b\u8981\u9875\u9762\uff1a\u66f4\u65b0\u5185\u5bb9\u3001\u4e0b\u8f7d\u4e0e\u5b89\u88c5\u3001\u8bba\u575b\u3001\u5df2\u77e5\u95ee\u9898\u4e0e\u66f4\u65b0\u8bb0\u5f55\u3002"],
      ["\u5f53\u524d\u5305", "SpirePlus-v0.1.0-private-beta.17.zip；游戏内显示名为 Spire Plus。"],
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

function mechanicCard(id, icon, tags = []) {
  const entry = mechanicGlossary.find(item => item.id === id);
  if (!entry) {
    return manual(id, "", id, ["机制"], icon);
  }

  const bullets = entry.bullets || [];
  const bulletsEn = entry.bulletsEn || bullets;
  return {
    namespace: "mechanics",
    i18nKey: `mechanic_${id}`,
    title: entry.title,
    titleEn: entry.titleEn,
    desc: [entry.desc, ...bullets].join(" "),
    descEn: [entry.descEn, ...bulletsEn].filter(Boolean).join(" "),
    vanilla: "",
    vanillaEn: "",
    icon: icon || entry.icon,
    tags: ["机制", ...tags],
    details: bullets.map((line, index) => detail(`规则 ${index + 1}`, line, `Rule ${index + 1}`, bulletsEn[index] || line))
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
      brandSub: "Spire Plus, a Slay the Spire 2 balance and high-Ascension expansion by Wenhuo Rongbing",
      navUpdates: "Updates",
      navInstall: "Download & Install",
      navForum: "Forum",
      navIssues: "Known Issues",
      navAbout: "About",
      releaseLine: "",
      heroCopy: "Stronger rewards with readable costs, plus new route and combat decisions for high Ascension.",
      modIntroTitle: "What Spire Plus Changes",
      featAscensionTitle: "New Ascension 20",
      featAscensionDesc: "A11-A20 add new routes, Firemarked Elites, Fission rewards, Rootblight, Banner Rooms, Boss dedicated abilities, and A20 Branded Forms. High Ascension now asks route, card-play, and combat questions.",
      featPhilosophyTitle: "More Choices, Real Pressure",
      featPhilosophyDesc: "Spire Plus makes rewards more tempting and their costs more concrete. Strong rewards spell out their price, timing, and follow-up rules so players can plan routes, fights, and deck pressure.",
      featRewardTitle: "Stronger Rewards, Clearer Costs",
      featRewardDesc: "Ancients and several vanilla relics are rebuilt as explicit trades: stronger starts, route value, and deck tools in exchange for HP, debt, pollution, route commitments, or play restrictions.",
      aboutTitle: "About",
      aboutLead: "Project notes, asset sources, and release boundaries.",
      introTitle: "Mod Overview",
      introCopy: "Spire Plus is a Slay the Spire 2 gameplay expansion by Wenhuo Rongbing. It turns Ancient rewards into inspectable relic choices, revises selected vanilla relics, and adds the A11-A20 test ruleset: Rootblight and Blight Sprouts, Firemarked Elites, Banner Rooms, boss dedicated abilities, Crystal Sphere peek, and transform preview. The direction is direct: rewards can be stronger, costs must be readable, and players should understand why they take an option, when it can punish them, and how to answer it later.",
      download: "Download Mod",
      viewIssues: "Known Issues",
      all: "All",
      search: "Search",
      searchPlaceholder: "Relic, Ancient, Ascension, keyword",
      vanilla: "Vanilla",
      current: "Current",
      lockFocus: "Lock Focus",
      pinnedFocus: "Locked",
      unlockFocus: "Unlock",
      relatedChanges: "Related Changes",
      relatedMechanics: "Mechanics",
      mechanicCodex: "Mechanic Tags",
      mechanicTag: "Mechanic",
      expandDetails: "Exact effects",
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
        ["File", "SpirePlus-v0.1.0-private-beta.17.zip"],
        ["Version", "v0.1.0-private-beta.17"],
        ["Display name", "Spire Plus"],
        ["Dependency", "BaseLib v3.1.4"],
        ["Game version", "Slay the Spire 2 v0.106.0"],
        ["Size", "18,940,143 bytes"],
        ["Hash", "44DFFE46847F6EE096EEDAC02303841E0646C1493D00D10DB66A2B46AF885FD6"]
      ]
    },
    installSteps: [
      "Download SpirePlus-v0.1.0-private-beta.17.zip.",
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
            ["Release Page", "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/tag/v0.1.0-private-beta.17"]
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
        short: "Mechanics",
        title: "Ancient and Ascension Mechanics",
        note: "Cross-entry mechanics are split into individual reference cards. Clicking a mechanic term locks the matching card instead of rebuilding the whole list.",
        defaultVanilla: ""
      },
      {
        short: "Cards",
        title: "New Cards and Statuses",
        note: "New cards and statuses added by the mod.",
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
      "战旗房": "Banner Room",
      "血债": "Blood Debt",
      "赃物锁": "Stolen Lock",
      "契约": "Contract",
      "铸令": "Forge Token",
      "火印精英": "Firemarked Elite",
      "溢火": "Overflow",
      "裂变": "Fission",
      "苗床": "Seedbed",
      "试炼枝条": "Trial Branch",
      "裁决": "Verdict",
      "威仪": "Majesty",
      "御令": "Royal Decree",
      "专属能力": "Dedicated Ability",
      "烙印形态": "Branded Form",
      "卡牌": "Card",
      "状态": "Status",
      "预览工具": "Preview tool",
      "机制": "Mechanic",
      "资料库": "Mechanic tag"
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
      "EZMB_URDA.pages.INITIAL.options.urda_seedbed.description": {
        title: "Seedbed",
        desc: "Act 1 normal combat card rewards can become Seedbed: lose 2 Max HP and add 1 Seedbed. The first Seedbed taken is upgraded. After taking Seedbed 4 times, gain 10 Max HP. Seedbed is a 1-cost 8/12 Block defense card. On play, it first plants 1/2 eligible draw/discard cards; later it catches temporary negative cards, Blight Sprouts, or Rootblight before they enter hand and gives a Withered Husk for each. Planting handles the card for this combat without playing, discarding, or exhausting it. Sprouts are handled; Rootblight is held still for this combat and stays in the master deck at the same level.",
        details: [
          detail("获取", "一幕普通战斗卡牌奖励中选择苗床，会结束本次奖励选择并加入1张苗床。", "Pickup", "Choose Seedbed from an Act 1 normal combat card reward; it completes that reward and adds 1 Seedbed."),
          detail("成本", "每次收下苗床失去2点最大生命。第一次收下的苗床会升级。", "Cost", "Each taken Seedbed costs 2 Max HP. The first Seedbed taken is upgraded."),
          detail("累计", "累计收下4次苗床后，获得10点最大生命。", "Completion", "After taking Seedbed 4 times, gain 10 Max HP."),
          detail("强度", "苗床是1费防御牌：8点格挡，并立刻种下至多1张抽牌堆或弃牌堆里的可种下牌。第一次拿到的苗床+为12点格挡、3格，并立刻种下至多2张。每次种下还会给1张可转成3点格挡的枯壳。", "Power", "Seedbed is a 1-cost defensive card: 8 Block, and it immediately plants up to 1 eligible draw/discard card. The first upgraded Seedbed gives 12 Block, 3 slots, and immediately plants up to 2. Each planted card also gives a Husk that can become 3 Block."),
          detail("种下", "苗床会截住之后进入手牌的临时状态牌、临时诅咒牌、根芽或根蚀；该牌离开本场战斗，不触发打出、弃牌或消耗，并加入1张枯壳。种下不是消耗永久诅咒，也不会触发消耗收益。", "Plant", "Seedbed catches later Temporary Status cards, Temporary Curse cards, Blight Sprouts, or Rootblight before they enter hand. The card leaves combat, does not trigger play, discard, or Exhaust effects, and gives 1 Withered Husk. Planting does not exhaust permanent Curses."),
          detail("根系结算", "根芽被种下后不算打出，但按已处理结算，战后不生成根蚀 I。根蚀被种下后只在本场停住，仍按同等级留在主牌组，战后不升级、不分裂、不移除、不降级，也不会变好。", "Root resolution", "A planted Blight Sprout is treated as handled without counting as played and adds no Rootblight I after combat. A planted Rootblight only holds still for this combat and remains in the master deck at the same level.")
        ]
      },
      "EZMB_URDA_SEEDLING.description": {
        desc: "0-cost Skill. Exhaust. Gain 4 Block; upgraded gains 7 Block."
      },
      "EZMB_URDA_SEEDBED.description": {
        desc: "1-cost Skill. Exhaust. Gain 8 Block, set up a 2-space Seedbed, and immediately plant up to 1 eligible card from draw or discard. Later Temporary Status cards, Temporary Curse cards, Blight Sprouts, or Rootblight are planted before entering hand: the card leaves combat, does not enter your hand, does not trigger play, discard, or Exhaust synergies, and gives 1 Withered Husk. Planting a Blight Sprout handles it without playing it, so it adds no Rootblight I after combat. Planting Rootblight only holds it still for this combat: it remains in the master deck at the same level and does not upgrade, split, get removed, or downgrade. Upgraded: gain 12 Block, set 3 slots, and immediately plant up to 2 eligible cards."
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
        desc: "At enemy turn end, the Firemark host heals 4/8/16 HP. Deal 18/36/72 damage in the round to interrupt the heal. If it heals, overflow heals 1 damaged secondary enemy for 2/4/8 HP."
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
        vanilla: "",
        current: "Adds a peek button to the Crystal Sphere minigame. It only changes mask visibility and does not grant rewards.",
        tags: ["Preview tool"]
      },
      "transform_preview": {
        title: "Deterministic Transform Preview",
        vanilla: "",
        current: "Uses a copied RNG snapshot to predict transform results. It creates no card and does not advance real RNG.",
        tags: ["Preview tool"]
      }
    },
    knownIssues: [
      ["Needs testing", "Ancient relics", "Test that every Ancient relic works in game."],
      ["Needs testing", "Multiplayer", "Test that multiplayer works with the mod enabled."],
      ["Needs fix", "Mac text rendering", "Known issue: text displays incorrectly on Mac and needs a fix."]
    ],
    changeLog: [
      ["2026-05-23 · Gameplay text sync", "Resynced website localization and refreshed Seedbed, Rain Breath, Closed Court, Vakuu Trial contracts, A12 Firemark overflow, and A19/A20 Boss dedicated ability display text."],
      ["2026-05-22 · Website rebuild", "The site now has four main pages: updates, download and install, forum, and known issues with changelog."],
      ["Current package", "SpirePlus-v0.1.0-private-beta.17.zip; the in-game display name is Spire Plus."],
      ["Ancient content", "Urda, Morvi, and Lotha are included as new Ancients. The Vakuu trial remains hidden behind test gates."],
      ["Ascension content", "A11-A20 is included in the private test build. Single-player and host multiplayer selection are available; full co-op play still needs verification."],
      ["Preview tools", "Crystal Sphere peek and deterministic transform preview are merged into Spire Plus and are no longer shipped as a separate package."]
    ]
  }
};

/* START_EMBEDDED_LOCALIZATION */
window.SPIRE_PLUS_EMBEDDED_LOC = {
  "zh": {
    "relics": {
      "BEAUTIFUL_BRACELET.description": "选择3张牌。它们获得迅速2。",
      "BLACK_STAR.description": "精英额外掉落1件遗物。若在第3幕或之后获得，立即获得1件随机遗物。",
      "BLOOD_SOAKED_ROSE.description": "获得1点能量。将1张执迷诅咒加入你的牌组。",
      "BRILLIANT_SCARF.description": "每回合，你打出的第6张牌费用变为0。",
      "CHOICES_PARADOX.description": "每场战斗开始时，从5张可用的稀有牌中选择1张加入手牌。它获得保留，并在战斗后移除。",
      "CHOICES_PARADOX.selectionScreenPrompt": "选择1张稀有牌加入你的手牌。",
      "CLAWS.description": "拾取时，将至多[blue]{Cards}[/blue]张牌变化为撕咬+。",
      "CLAWS.eventDescription": "将至多[blue]{Cards}[/blue]张牌变化为撕咬+。",
      "CLAWS.selectionScreenPrompt": "选择要变化为撕咬+的牌。",
      "CLAWS.title": "坦克斯利爪",
      "CROSSBOW.description": "每回合开始时，你可以将1张随机攻击牌加入手牌。它本回合费用降低1，并获得虚无和消耗。",
      "DISTINGUISHED_CAPE.description": "拾取时，失去当前最大生命的30%，至少18点。加入3张灵体。",
      "DISTINGUISHED_CAPE.eventDescription": "失去当前最大生命的30%，至少18点。加入3张灵体。",
      "DISTINGUISHED_CAPE.unpayableOption": "最大生命过低，无法支付此代价（{Cost}）。",
      "ECTOPLASM.description": "获得1点能量。拾取时获得250金币。你不能再获得金币。",
      "FIDDLE.description": "每回合开始时，抽牌直到手牌有7张。在你的回合中，抽牌效果不能使你的手牌超过7张。",
      "IRON_CLUB.description": "每当你打出5张牌，抽1张牌。",
      "JEWELED_MASK.description": "拾取时，选择1张能力牌。它永久变为0费。每场战斗开始时，将它从抽牌堆移入你的手牌。",
      "JEWELED_MASK.ezSelectionScreenPrompt": "选择1张能力牌，使其永久变为0费。",
      "JEWELRY_BOX.description": "将1张神化加入你的牌组。它没有固有。",
      "MEAT_CLEAVER.description": "在休息处加入[gold]切肉[/gold]选项：移除[blue]2[/blue]张牌并失去[blue]5[/blue]点生命。",
      "MUSIC_BOX.description": "每回合，你打出的第一张攻击牌会在手牌中生成1张复制。复制本回合费用降低1，并具有虚无和消耗。",
      "PAELS_HORN.description": "将1张放松与1张放松+加入你的牌组。",
      "PAELS_TOOTH.description": "移除5张牌。之后每2场非首领战斗后，选择1张已储存的移除牌，升级后放回你的牌组。击败本幕首领后，剩余已储存牌永久移除。",
      "PRESERVED_FOG.description": "移除4张牌。将1张愚行加入你的牌组。",
      "PRISMATIC_GEM.description": "获得1点能量。每第二次标准卡牌奖励只包含异色牌。",
      "PRISMATIC_GEM.countHint.title": "棱彩计数：{Count}/{Cycle}",
      "PRISMATIC_GEM.countHint.nextNormal": "下一次标准卡牌奖励正常。",
      "PRISMATIC_GEM.countHint.nextOffColor": "下一次标准卡牌奖励只包含异色牌。",
      "PRISMATIC_GEM.rewardScreenHint": "棱彩奖励：本次只出现异色牌。",
      "SEAL_OF_GOLD.description": "获得1点能量。将2张可打出的债务诅咒加入你的牌组。",
      "SOZU.description": "获得1点能量。拾取时填满所有空药水栏。你不能再获得药水。",
      "SERE_TALON.description": "拾取时，从[blue]4[/blue]张诅咒中选择[blue]1[/blue]张。将它、[blue]2[/blue]张[gold]许愿[/gold]和[blue]1[/blue]张[gold]许愿+[/gold]加入你的牌组。",
      "SERE_TALON.eventDescription": "从[blue]4[/blue]张诅咒中选择[blue]1[/blue]张。加入它、[blue]2[/blue]张[gold]许愿[/gold]和[blue]1[/blue]张[gold]许愿+[/gold]。",
      "SERE_TALON.selectionScreenPrompt": "选择1张诅咒。",
      "SERE_TALON.title": "瓦库原初之爪",
      "TOASTY_MITTENS.description": "每回合抽牌前，查看抽牌堆顶的牌。你可以消耗它以获得1点力量。",
      "EZMICROBALANCE-AncientInitialRerollOptionRelic.description": "重置当前第[blue]1[/blue]幕先古奖励。骰子只在第[blue]1[/blue]幕出现。",
      "EZMICROBALANCE-AncientInitialRerollOptionRelic.flavor": "一枚只给这次选择用的小骰子。",
      "EZMICROBALANCE-AncientInitialRerollOptionRelic.title": "重掷",
      "EZMICROBALANCE-ANCIENT_INITIAL_REROLL_OPTION_RELIC.description": "重置当前第[blue]1[/blue]幕先古奖励。骰子只在第[blue]1[/blue]幕出现。",
      "EZMICROBALANCE-ANCIENT_INITIAL_REROLL_OPTION_RELIC.flavor": "一枚只给这次选择用的小骰子。",
      "EZMICROBALANCE-ANCIENT_INITIAL_REROLL_OPTION_RELIC.title": "重掷",
      "EZMICROBALANCE-UrdaHumusPactOptionRelic.description": "第[blue]1[/blue]幕普通战斗卡牌奖励会出现[gold]化为腐殖[/gold]。选择它会跳过本次卡牌，改拿[blue]15[/blue][gold]金币[/gold]。第[blue]3[/blue]次[gold]化为腐殖[/gold]后，移除至多[blue]2[/blue]张牌，并获得[blue]1[/blue]张已升级奖励牌。",
      "EZMICROBALANCE-UrdaHumusPactOptionRelic.flavor": "封存在根须之下的契约记号。",
      "EZMICROBALANCE-UrdaHumusPactOptionRelic.title": "腐殖约定",
      "EZMICROBALANCE-UrdaMoltingOptionRelic.description": "移除[blue]1[/blue]张打击和[blue]1[/blue]张防御，然后加入[blue]2[/blue]张[gold]枯壳[/gold]诅咒牌。第[blue]2[/blue]幕开始时，剩余[gold]枯壳[/gold]会被移除。",
      "EZMICROBALANCE-UrdaMoltingOptionRelic.flavor": "旧皮与新芽之间留下的封存记号。",
      "EZMICROBALANCE-UrdaMoltingOptionRelic.title": "脱壳",
      "EZMICROBALANCE-UrdaMossMapOptionRelic.description": "第[blue]1[/blue]幕首次进入：怪物 +[blue]25[/blue] [gold]金币[/gold]；事件治疗[blue]5[/blue]；商店给[gold]药水[/gold]；[gold]精英[/gold]随机升级[blue]1[/blue]张牌；休息处 +[blue]3[/blue] [gold]最大生命[/gold]。",
      "EZMICROBALANCE-UrdaMossMapOptionRelic.flavor": "苔痕遮住道路时留下的封存记号。",
      "EZMICROBALANCE-UrdaMossMapOptionRelic.title": "苔痕地图",
      "EZMICROBALANCE-UrdaSeedbedOptionRelic.description": "第[blue]1[/blue]幕普通战斗卡牌奖励可改拿[gold]苗床[/gold]：失去[blue]2[/blue]点[gold]最大生命[/gold]并加入[gold]苗床[/gold]。第一次加入的[gold]苗床[/gold]会升级；累计收下[blue]4[/blue]次后，获得[blue]10[/blue]点[gold]最大生命[/gold]。[gold]苗床[/gold]是[blue]1[/blue]费防御牌：[blue]8[/blue]/[blue]12[/blue]点格挡，[blue]2[/blue]/[blue]3[/blue]格苗床，并会立刻从抽牌堆或弃牌堆种下[blue]1[/blue]/[blue]2[/blue]张可种下的牌。之后若[gold]临时[/gold]状态牌、[gold]临时[/gold]诅咒牌、[gold]根芽[/gold]或[gold]根蚀[/gold]将进入手牌，苗床会先种下它，每张换成[gold]枯壳[/gold]。种下表示苗床替你处理这张牌；它不是打出、丢弃或消耗。临时负面牌只在本战消失；永久诅咒不能种下。[gold]根芽[/gold]按已处理结算，战后不生成[gold]根蚀 I[/gold]。[gold]根蚀[/gold]只在本场停住，仍按同等级留在主牌组。",
      "EZMICROBALANCE-UrdaSeedbedOptionRelic.flavor": "一粒耐心种子的封存记号。",
      "EZMICROBALANCE-UrdaSeedbedOptionRelic.title": "苗床",
      "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.description": "第[blue]1[/blue]幕普通战斗卡牌奖励会出现[gold]化为腐殖[/gold]。选择它会跳过本次卡牌，改拿[blue]15[/blue][gold]金币[/gold]。第[blue]3[/blue]次[gold]化为腐殖[/gold]后，移除至多[blue]2[/blue]张牌，并获得[blue]1[/blue]张已升级奖励牌。",
      "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.flavor": "封存在根须之下的契约记号。",
      "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.title": "腐殖约定",
      "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.description": "移除[blue]1[/blue]张打击和[blue]1[/blue]张防御，然后加入[blue]2[/blue]张[gold]枯壳[/gold]诅咒牌。第[blue]2[/blue]幕开始时，剩余[gold]枯壳[/gold]会被移除。",
      "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.flavor": "旧皮与新芽之间留下的封存记号。",
      "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.title": "脱壳",
      "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description": "第[blue]1[/blue]幕首次进入：怪物 +[blue]25[/blue] [gold]金币[/gold]；事件治疗[blue]5[/blue]；商店给[gold]药水[/gold]；[gold]精英[/gold]随机升级[blue]1[/blue]张牌；休息处 +[blue]3[/blue] [gold]最大生命[/gold]。",
      "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.flavor": "苔痕遮住道路时留下的封存记号。",
      "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.title": "苔痕地图",
      "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.description": "第[blue]1[/blue]幕普通战斗卡牌奖励可改拿[gold]苗床[/gold]：失去[blue]2[/blue]点[gold]最大生命[/gold]并加入[gold]苗床[/gold]。第一次加入的[gold]苗床[/gold]会升级；累计收下[blue]4[/blue]次后，获得[blue]10[/blue]点[gold]最大生命[/gold]。[gold]苗床[/gold]是[blue]1[/blue]费防御牌：[blue]8[/blue]/[blue]12[/blue]点格挡，[blue]2[/blue]/[blue]3[/blue]格苗床，并会立刻从抽牌堆或弃牌堆种下[blue]1[/blue]/[blue]2[/blue]张可种下的牌。之后若[gold]临时[/gold]状态牌、[gold]临时[/gold]诅咒牌、[gold]根芽[/gold]或[gold]根蚀[/gold]将进入手牌，苗床会先种下它，每张换成[gold]枯壳[/gold]。种下表示苗床替你处理这张牌；它不是打出、丢弃或消耗。临时负面牌只在本战消失；永久诅咒不能种下。[gold]根芽[/gold]按已处理结算，战后不生成[gold]根蚀 I[/gold]。[gold]根蚀[/gold]只在本场停住，仍按同等级留在主牌组。",
      "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.flavor": "一粒耐心种子的封存记号。",
      "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.title": "苗床",
      "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description": "从[blue]4[/blue]张牌中选择[blue]1[/blue]张。它会升级、加入牌组，并获得[gold]试炼枝条[/gold]。接下来[blue]3[/blue]场战斗每场都必须打出它；漏掉任意一场就会被移除。",
      "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.flavor": "裁决纹路中长出绿芽的封存记号。",
      "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.title": "试炼枝条",
      "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.description": "从[blue]2[/blue]件普通遗物中选择[blue]1[/blue]件，并获得[blue]75[/blue][gold]金币[/gold]。击败第[blue]1[/blue]幕[gold]精英[/gold]后，永久扎根该遗物并获得[blue]35[/blue][gold]金币[/gold]。若第[blue]2[/blue]幕先开始，乌尔妲会收回浅根遗物并返还[blue]75[/blue][gold]金币[/gold]。",
      "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.flavor": "根须尚未触到岩层的封存记号。",
      "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.title": "浅根遗物",
      "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.description": "每场[gold]精英[/gold]战斗结束后，回复[blue]10[/blue]点生命。火印精英也会触发。",
      "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.flavor": "一枚从硬仗里汲取生机的封存记号。",
      "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.title": "精英根须",
      "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.description": "乌尔妲标记第[blue]1[/blue]幕前[blue]7[/blue]层内一场可到达战斗。到达[gold]根印[/gold]，获得[blue]3[/blue]个单卡奖励；第一张已升级，且若有空位获得[blue]1[/blue]瓶[gold]药水[/gold]。错过根印则失去[blue]8[/blue]点生命并获得[blue]25[/blue][gold]金币[/gold]。",
      "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.flavor": "只有根须能读懂路线的封存记号。",
      "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.title": "扎根路线",
      "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description": "第[blue]1[/blue]幕，每场战斗第一次受到未格挡敌方攻击伤害后，获得[blue]1[/blue]张[gold]雨息[/gold]。第[blue]2[/blue]幕开始时，若触发少于[blue]3[/blue]次，获得[blue]75[/blue][gold]金币[/gold]；否则回复[blue]8[/blue]点生命并升级[blue]1[/blue]张牌。",
      "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.flavor": "仍沾着那场救命雨水的封存记号。",
      "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.title": "雨后",
      "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.description": "获得[blue]5[/blue]枚[gold]根眼[/gold]。在地图上点击此遗物，选择一个后续可到达的怪物、随机或精英房间，预见具体敌群或事件。悬停标记房间可查看结果。首次预见若有空位，获得[blue]1[/blue]瓶[gold]药水[/gold]。",
      "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.flavor": "一只眼睛压进活树皮里的封存记号。",
      "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.title": "根眼",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description": "第[blue]1[/blue]幕普通战斗卡牌奖励中，选择[gold]储存种子[/gold]来保存[blue]1[/blue]张展示牌，最多[blue]3[/blue]张。之后点击此遗物，选择至多[blue]2[/blue]张已存牌加入牌组；第一张会升级。之后遗物失效。",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.flavor": "会因未来的叶片而轻响的封存记号。",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.descriptionFooter": "点击此遗物，选择至多[blue]2[/blue]张已存牌加入牌组；第一张会升级。",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.descriptionPrefix": "已保存：",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.title": "保存的种子",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.title": "种子库",
      "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.description": "战斗后的卡牌奖励不再出现。第[blue]1[/blue]回合：抽[blue]4[/blue]张牌，获得[blue]2[/blue]点[gold]能量[/gold]。第[blue]4[/blue]回合：抽[blue]2[/blue]张牌，获得[blue]2[/blue]点[gold]能量[/gold]。",
      "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.flavor": "来自封闭法庭的封存记号。",
      "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.title": "终审封庭",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.description": "每局一次，防止死亡并将生命设为[blue]1[/blue]。你获得一个最后回合：抽[blue]10[/blue]张牌，获得[blue]10[/blue]点[gold]能量[/gold]，所有牌变为[blue]0[/blue]费，且本回合不会死亡。回合结束时，若仍有敌人，你死亡；否则继续本局。",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.flavor": "为最后判决延后一次的封存记号。",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.title": "死刑缓期",
      "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.description": "第[blue]4[/blue]回合开始时，抽[blue]4[/blue]张牌，获得[blue]4[/blue]点[gold]能量[/gold]和[blue]3[/blue]层[gold]裁决[/gold]。本回合每打出[blue]1[/blue]张非状态牌，消耗[blue]1[/blue]层[gold]裁决[/gold]：[gold]攻击牌[/gold]和[gold]技能牌[/gold]额外打出[blue]1[/blue]次；[gold]能力牌[/gold]费用变为[blue]0[/blue]并抽[blue]1[/blue]张牌。若战斗在第[blue]4[/blue]回合前结束，回复[blue]4[/blue]点生命。",
      "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.flavor": "等待第三份证据的封存记号。",
      "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.title": "延期判决",
      "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.description": "回合结束时，记住你最后打出的牌类型。下回合，你第一次打出同类型牌时触发回声：[gold]攻击牌[/gold]和[gold]技能牌[/gold]额外打出[blue]1[/blue]次；[gold]能力牌[/gold]费用变为[blue]0[/blue]并抽[blue]1[/blue]张牌。",
      "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.flavor": "由每面墙反射而来的封存记号。",
      "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.title": "镜厅回声",
      "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description": "从牌组选择[blue]1[/blue]张镜牌。每场战斗你的首回合正常抽牌后，将它移入手牌。第一次打出它：[gold]攻击牌[/gold]和[gold]技能牌[/gold]额外打出[blue]1[/blue]次；[gold]能力牌[/gold]费用变为[blue]0[/blue]。",
      "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.flavor": "回应第一道伤口的封存记号。",
      "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.title": "反证之镜",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.description": "战斗开始时，获得[gold]无罪[/gold]。每个玩家回合开始时，若你仍有[gold]无罪[/gold]，抽[blue]2[/blue]张牌，获得[blue]1[/blue]点[gold]能量[/gold]，并获得[blue]8[/blue]点[gold]格挡[/gold]。当你受到未被格挡的敌人[gold]攻击[/gold]伤害时，失去[gold]无罪[/gold]，立即失去[blue]8[/blue]点生命，且本场战斗不能重新获得[gold]无罪[/gold]。",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.flavor": "证据呈上前的封存记号。",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.title": "无罪推定",
      "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description": "你施加的非伤害类[gold]负面状态[/gold]层数翻倍，并获得[blue]1[/blue]层[gold]开悟[/gold]。敌人施加给你的非伤害类[gold]负面状态[/gold]层数也翻倍，并失去[blue]1[/blue]层[gold]开悟[/gold]。回合开始时，消耗至多[blue]3[/blue]层[gold]开悟[/gold]；每层抽[blue]1[/blue]张牌并获得[blue]4[/blue]点[gold]格挡[/gold]。",
      "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.flavor": "摆在明处的封存记号。",
      "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.title": "公开罪证",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.description": "每回合，你打出的第一张[gold]攻击牌[/gold]或[gold]技能牌[/gold]额外打出[blue]2[/blue]次。之后你本回合最多再打出[blue]4[/blue]张牌。[gold]能力牌[/gold]不计入限制，费用变为[blue]0[/blue]，打出后抽[blue]1[/blue]张牌。",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.flavor": "写成一行的封存记号。",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.title": "单牌宣判",
      "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description": "与瓦库进行赃物试炼。打破[blue]1[/blue]把或更多[gold]赃物锁[/gold]，拿走金币和额外祝福选择。第[blue]1[/blue]、[blue]3[/blue]、[blue]5[/blue]回合，从[blue]3[/blue]张[gold]契约[/gold]中选择[blue]1[/blue]张。契约能帮你破锁，但会增加[gold]血债[/gold]。破锁后可以收手。本场没有普通战斗奖励。死亡会结束本局。",
      "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.flavor": "同一把刀上刻着警告与邀请。",
      "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.title": "瓦库试炼",
      "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description": "从[blue]3[/blue]张已升级的[gold]远古[/gold]牌中选择[blue]1[/blue]张加入牌组。打出该牌会失去生命：[gold]攻击牌[/gold]和[gold]技能牌[/gold]失去[blue]1[/blue]点生命；[gold]能力牌[/gold]失去[blue]8[/blue]点生命。击败第[blue]2[/blue]幕首领后，若能支付[blue]180[/blue][gold]金币[/gold]则保留；否则莫尔维会移除它。",
      "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.flavor": "在页边签下的封存记号。",
      "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.title": "禁忌借贷",
      "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC.description": "每回合一次，你手动打出的第一张牌组[gold]攻击牌[/gold]或[gold]技能牌[/gold]会额外打出[blue]1[/blue]次。若这张牌原本费用为[blue]1[/blue]点或更高[gold]能量[/gold]，抽[blue]1[/blue]张牌。[gold]能力牌[/gold]、状态牌、诅咒牌、自动打出和生成牌不会触发。",
      "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC.flavor": "被误盖两次印章的封存记号。",
      "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC.title": "错页印刷机",
      "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.description": "每回合若手牌有空间，加入[blue]1[/blue]张临时[gold]透支[/gold]牌。每回合一次，在[blue]0[/blue]点[gold]能量[/gold]时打出它：抽[blue]2[/blue]张，获得[blue]1[/blue]点[gold]能量[/gold]，并增加[blue]1[/blue]笔[gold]红墨债[/gold]。每笔[gold]红墨债[/gold]在战斗结束时支付[blue]12[/blue][gold]金币[/gold]，或在无法支付时失去[blue]3[/blue]点非致命生命。",
      "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.flavor": "墨迹迟迟不干的封存记号。",
      "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.title": "红墨透支",
      "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC.description": "战斗开始时，将[blue]3[/blue]张随机临时[gold]档案页[/gold]加入手牌。[gold]档案页[/gold]费用为[blue]0[/blue]，具有[gold]虚无[/gold]和[gold]消耗[/gold]，并会在战斗后移除。",
      "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC.flavor": "藏在最后一层书架后的封存记号。",
      "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC.title": "逾期书库",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC.description": "第[blue]1[/blue]回合，额外抽[blue]5[/blue]张牌并获得[blue]2[/blue]点[gold]能量[/gold]。回合结束时，额外抽到且仍在手牌中的牌会封存在[gold]消耗牌堆[/gold]；第[blue]3[/blue]回合在手牌空间允许时以[blue]0[/blue]费返回。",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC.flavor": "夹进小抄里的封存记号。",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC.title": "开卷考试",
      "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC.description": "战斗开始时，将[blue]4[/blue]张[gold]废纸[/gold]状态牌洗入[gold]抽牌堆[/gold]。每回合，从[gold]抽牌堆[/gold]抽到的前[blue]2[/blue]张状态牌会被消耗；每张都会抽[blue]1[/blue]张并获得[blue]1[/blue]点[gold]能量[/gold]。",
      "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC.flavor": "被不断堆高的表格卷起的封存记号。",
      "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC.title": "纸页风暴",
      "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC.description": "战斗开始时，获得[blue]3[/blue]层[gold]校样[/gold]。你手动打出的前[blue]3[/blue]张牌组牌各获得一次效果：未升级牌临时升级并抽[blue]1[/blue]张；已升级牌[gold]能量[/gold]费用降低[blue]1[/blue]点并获得[blue]4[/blue]点[gold]格挡[/gold]。",
      "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC.flavor": "被蓝色铅笔修订过的封存记号。",
      "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC.title": "蓝图校样",
      "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description": "立即获得[blue]220[/blue][gold]金币[/gold]，移除至多[blue]2[/blue]张牌，并升级至多[blue]2[/blue]张牌。获得[blue]320[/blue]点[gold]债务[/gold]。每场战斗后偿还[blue]40[/blue][gold]金币[/gold]；每短缺[blue]10[/blue][gold]金币[/gold]，失去[blue]3[/blue]点非致命生命。[gold]债务[/gold]照常减少[blue]40[/blue]点。",
      "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.flavor": "盖着预付讫印章的封存记号。",
      "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.title": "债务清算",
      "VELVET_CHOKER.description": "获得1点能量。每回合，从你手牌打出的第7张及之后的牌费用增加1。",
      "WAR_HAMMER.description": "拾取时，选择2张牌升级。每当你击败精英，随机升级4张牌。",
      "WHISPERING_EARRING.description": "获得1点能量。每场战斗的前3回合，抽牌后自动打出你费用最高的可打出牌。"
    },
    "ancients": {
      "EZMICROBALANCE-EZMB_URDA.title": "息壤织母·乌尔妲",
      "EZMICROBALANCE-EZMB_URDA.epithet": "大地之根记住了你的脚步。",
      "EZMICROBALANCE-EZMB_URDA.talk.firstVisitEver.0-0.ancient": "根记得第一枚脚印。选择它将在哪里生长。",
      "EZMICROBALANCE-EZMB_URDA.talk.ANY.0-0r.ancient": "只取你能养活的恩泽。其余都会归于泥土。",
      "EZMB_URDA.title": "息壤织母·乌尔妲",
      "EZMB_URDA.epithet": "大地之根记住了你的脚步。",
      "EZMB_URDA.pages.INITIAL.description": "乌尔妲递来四枚根印。选择一枚，它会成为你的遗物。",
      "EZMB_URDA.pages.INITIAL.options.ezmb_reroll_initial_options.title": "重掷",
      "EZMB_URDA.pages.INITIAL.options.ezmb_reroll_initial_options.description": "重置本次第[blue]1[/blue]幕先古奖励。骰子只在第[blue]1[/blue]幕出现，使用后消失。",
      "NEOW.pages.INITIAL.options.ezmb_reroll_initial_options.title": "重掷",
      "NEOW.pages.INITIAL.options.ezmb_reroll_initial_options.description": "重置本次第[blue]1[/blue]幕先古奖励。骰子只在第[blue]1[/blue]幕出现，使用后消失。",
      "EZMB_URDA.pages.INITIAL.options.urda_seedbed.title": "苗床",
      "EZMB_URDA.pages.INITIAL.options.urda_seedbed.description": "第[blue]1[/blue]幕卡牌奖励可改拿[gold]苗床[/gold]：失去[blue]2[/blue]点[gold]最大生命[/gold]并加入[gold]苗床[/gold]。第一次加入的[gold]苗床[/gold]会升级；累计收下[blue]4[/blue]次后，获得[blue]10[/blue]点[gold]最大生命[/gold]。[gold]苗床[/gold]是[blue]1[/blue]费防御牌：[blue]8[/blue]/[blue]12[/blue]点格挡，[blue]2[/blue]/[blue]3[/blue]格苗床，并会立刻从抽牌堆或弃牌堆种下[blue]1[/blue]/[blue]2[/blue]张可种下的牌。之后若[gold]临时[/gold]状态牌、[gold]临时[/gold]诅咒牌、[gold]根芽[/gold]或[gold]根蚀[/gold]将进入手牌，苗床会先种下它，每张换成[gold]枯壳[/gold]。种下表示苗床替你处理这张牌；它不是打出、丢弃或消耗。临时负面牌只在本战消失；永久诅咒不能种下。[gold]根芽[/gold]按已处理结算，战后不生成[gold]根蚀 I[/gold]。[gold]根蚀[/gold]只在本场停住，仍按同等级留在主牌组。",
      "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.title": "腐殖约定",
      "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description": "第[blue]1[/blue]幕普通战斗的卡牌奖励会出现[gold]化为腐殖[/gold]按钮。选择它会跳过本次卡牌，改拿[blue]15[/blue][gold]金币[/gold]。第[blue]3[/blue]次[gold]化为腐殖[/gold]后，移除至多[blue]2[/blue]张牌，并获得[blue]1[/blue]张已升级奖励牌。",
      "EZMB_URDA.pages.INITIAL.options.urda_molting.title": "脱壳",
      "EZMB_URDA.pages.INITIAL.options.urda_molting.description": "移除[blue]1[/blue]张打击和[blue]1[/blue]张防御，加入[blue]2[/blue]张[gold]枯壳[/gold]诅咒牌。剩余[gold]枯壳[/gold]会在第[blue]2[/blue]幕开始时移除。",
      "EZMB_URDA.pages.INITIAL.options.urda_moss_map.title": "苔痕地图",
      "EZMB_URDA.pages.INITIAL.options.urda_moss_map.description": "第[blue]1[/blue]幕首次进入：怪物 +[blue]25[/blue] [gold]金币[/gold]；事件治疗[blue]5[/blue]；商店给[gold]药水[/gold]；[gold]精英[/gold]随机升级[blue]1[/blue]张牌；休息处 +[blue]3[/blue] [gold]最大生命[/gold]。",
      "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.title": "试炼枝条",
      "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description": "从[blue]4[/blue]张牌中选择[blue]1[/blue]张。它会升级、加入牌组，并获得[gold]试炼枝条[/gold]。接下来[blue]3[/blue]场战斗每场都必须打出它；漏掉任意一场就会被移除。",
      "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt": "为[gold]试炼枝条[/gold]选择[blue]1[/blue]张牌。",
      "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.title": "浅根遗物",
      "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.description": "选择[blue]1[/blue]件普通遗物并获得[blue]75[/blue][gold]金币[/gold]。击败第[blue]1[/blue]幕[gold]精英[/gold]可保留它；否则第[blue]2[/blue]幕开始时退还。",
      "EZMB_URDA.pages.INITIAL.options.urda_elite_root.title": "精英根须",
      "EZMB_URDA.pages.INITIAL.options.urda_elite_root.description": "每场[gold]精英[/gold]战斗结束后，回复[blue]10[/blue]点生命。火印精英也会触发。",
      "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.title": "扎根路线",
      "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.description": "第[blue]1[/blue]幕出现一条[gold]根印[/gold]路线。走到标记战斗可获得更好的奖励；错过则失去[blue]8[/blue]点生命并获得[blue]25[/blue][gold]金币[/gold]。",
      "EZMB_URDA.pages.INITIAL.options.urda_after_rain.title": "雨后",
      "EZMB_URDA.pages.INITIAL.options.urda_after_rain.description": "第[blue]1[/blue]幕，每场战斗第一次受到未格挡敌方攻击伤害后，获得[blue]1[/blue]张[gold]雨息[/gold]。第[blue]2[/blue]幕开始时，若触发少于[blue]3[/blue]次，获得[blue]75[/blue][gold]金币[/gold]；否则回复[blue]8[/blue]点生命并升级[blue]1[/blue]张牌。",
      "EZMB_URDA.pages.INITIAL.options.urda_root_sight.title": "根眼",
      "EZMB_URDA.pages.INITIAL.options.urda_root_sight.description": "获得[blue]5[/blue]枚[gold]根眼[/gold]。在地图上点击此遗物，再选择后续可到达的怪物、随机或精英房间。该房间会显示并保留具体敌群或事件；悬停标记房间可查看。",
      "EZMB_URDA.root_sight.hover.title": "根眼",
      "EZMB_URDA.root_sight.hover.description": "在地图上点击此遗物，可预见一个后续可到达的怪物、随机或精英房间。悬停标记房间可查看结果。不能选择篝火、商店、宝箱和首领。",
      "EZMB_URDA.root_sight.selection_hover.title": "用根眼预见",
      "EZMB_URDA.root_sight.selection_hover.description": "点击后显示这个房间的敌群或事件。",
      "EZMB_URDA.root_sight.map_hover.title": "根眼预见",
      "EZMB_URDA.root_sight.map_hover.description": "根眼已经预见这个房间。",
      "EZMB_URDA.root_sight.map_hover.preview_description": "根眼预见了这个结果。进入该房间后会遇到它。",
      "EZMB_URDA.root_sight.map_hover.event_preview_description": "根眼预见了这个事件。主要选项：{Options}",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.title": "种子库",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description": "第[blue]1[/blue]幕卡牌奖励可存下展示牌，最多[blue]3[/blue]张。之后点击此遗物，选择至多[blue]2[/blue]张加入牌组；第一张会升级。之后遗物失效。",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.storeSelectionPrompt": "选择[blue]1[/blue]张展示牌保存为[gold]种子[/gold]。",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.settlementSelectionPrompt": "选择至多[blue]2[/blue]张已存[gold]种子[/gold]加入牌组。",
      "EZMB_URDA.pages.DONE.description": "息壤的恩泽沉入土里。你已选择了道路。",
      "EZMB_URDA.talk.firstVisitEver.0-0.ancient": "根记得第一枚脚印。选择它将在哪里生长。",
      "EZMB_URDA.talk.ANY.0-0r.ancient": "只取你能养活的恩泽。其余都会归于泥土。",
      "EZMICROBALANCE-EZMB_MORVI.title": "借书人·莫尔维",
      "EZMICROBALANCE-EZMB_MORVI.epithet": "每一份恩惠，页边都写着利息。",
      "EZMICROBALANCE-EZMB_MORVI.talk.firstVisitEver.0-0.ancient": "墨水是有耐心的债主。借力时要谨慎。",
      "EZMICROBALANCE-EZMB_MORVI.talk.ANY.0-0r.ancient": "每一处页边，都还能再记一笔债。",
      "EZMICROBALANCE-EZMB_MORVI.talk.IRONCLAD.0-0r.ancient": "你知道契约的重量。只签你能活着偿还的那份。",
      "EZMICROBALANCE-EZMB_MORVI.talk.SILENT.0-0r.ancient": "安静的签名同样有效。读清页边的小字。",
      "EZMICROBALANCE-EZMB_MORVI.talk.DEFECT.0-0r.ancient": "即使完美的机器，也可能欠下利息。选择有用的错误吧。",
      "EZMICROBALANCE-EZMB_MORVI.talk.NECROBINDER.0-0r.ancient": "复仇代价昂贵。我可以先替你垫上。",
      "EZMICROBALANCE-EZMB_MORVI.talk.REGENT.0-0r.ancient": "王冠本就是延期支付的承诺。你的条款在此。",
      "EZMB_MORVI.title": "借书人·莫尔维",
      "EZMB_MORVI.epithet": "每一份恩惠，页边都写着利息。",
      "EZMB_MORVI.pages.INITIAL.description": "莫尔维递来三枚借据。选择一枚，它会成为你的遗物。",
      "EZMB_MORVI.pages.INITIAL.options.ezmb_reroll_initial_options.title": "重掷",
      "EZMB_MORVI.pages.INITIAL.options.ezmb_reroll_initial_options.description": "重置本次第[blue]1[/blue]幕先古奖励。骰子只在第[blue]1[/blue]幕出现，使用后消失。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.title": "禁忌借贷",
      "EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description": "从[blue]3[/blue]张已升级的[gold]远古[/gold]牌中选择[blue]1[/blue]张加入牌组。打出该牌会失去生命：[gold]攻击牌[/gold]和[gold]技能牌[/gold]失去[blue]1[/blue]点生命；[gold]能力牌[/gold]失去[blue]8[/blue]点生命。第[blue]2[/blue]幕首领后支付[blue]180[/blue][gold]金币[/gold]可保留。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.title": "错页印刷机",
      "EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.description": "每回合一次，你手动打出的第一张牌组[gold]攻击牌[/gold]或[gold]技能牌[/gold]会再打出[blue]1[/blue]次。若这张牌原本费用为[blue]1[/blue]点或更高[gold]能量[/gold]，抽[blue]1[/blue]张牌。[gold]能力牌[/gold]和生成牌不触发。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.title": "红墨透支",
      "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description": "每回合获得一张临时[gold]透支[/gold]。每回合一次，在[blue]0[/blue]点[gold]能量[/gold]时打出它：抽[blue]2[/blue]张，获得[blue]1[/blue]点[gold]能量[/gold]；战斗后支付[gold]红墨债[/gold]，付不起则失去非致命生命。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.title": "逾期书库",
      "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.description": "战斗开始时，将[blue]3[/blue]张临时[gold]档案页[/gold]加入手牌。它们费用为[blue]0[/blue]，战斗后移除。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.title": "开卷考试",
      "EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.description": "第[blue]1[/blue]回合多抽[blue]5[/blue]张并获得[blue]2[/blue]点[gold]能量[/gold]。留下的牌会封存在[gold]消耗牌堆[/gold]，并在第[blue]3[/blue]回合以[blue]0[/blue]费回到手牌。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.title": "纸页风暴",
      "EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.description": "战斗开始时洗入[blue]4[/blue]张[gold]废纸[/gold]到[gold]抽牌堆[/gold]。每回合前[blue]2[/blue]张从中抽到的状态牌会被消耗，并各抽[blue]1[/blue]张、获得[blue]1[/blue]点[gold]能量[/gold]。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.title": "蓝图校样",
      "EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.description": "战斗开始时获得[blue]3[/blue]层[gold]校样[/gold]。你手动打出的前[blue]3[/blue]张牌组牌各获得一次效果：未升级牌临时升级并抽[blue]1[/blue]张；已升级牌本次费用降低[blue]1[/blue]并获得[blue]4[/blue]点[gold]格挡[/gold]。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.title": "债务清算",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description": "立即获得[blue]220[/blue][gold]金币[/gold]，移除至多[blue]2[/blue]张牌，升级至多[blue]2[/blue]张牌。获得[blue]320[/blue]点[gold]债务[/gold]。每场战斗后偿还[blue]40[/blue][gold]金币[/gold]；每短缺[blue]10[/blue][gold]金币[/gold]，失去[blue]3[/blue]点非致命生命。[gold]债务[/gold]都会减少到期数值。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.removeSelectionPrompt": "为[gold]债务清算[/gold]移除至多[blue]2[/blue]张牌。",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.upgradeSelectionPrompt": "为[gold]债务清算[/gold]升级至多[blue]2[/blue]张牌。",
      "EZMB_MORVI.pages.DONE.description": "莫尔维合上账本。债已经记下。",
      "EZMB_MORVI.talk.firstVisitEver.0-0.ancient": "墨水是有耐心的债主。借力时要谨慎。",
      "EZMB_MORVI.talk.ANY.0-0r.ancient": "每一处页边，都还能再记一笔债。",
      "EZMICROBALANCE-EZMB_LOTHA.title": "审判者·洛莎",
      "EZMICROBALANCE-EZMB_LOTHA.epithet": "每一次挥击都是证据。每一回合都将宣判。",
      "EZMICROBALANCE-EZMB_LOTHA.talk.firstVisitEver.0-0.ancient": "法庭开庭。选择你登塔时必须遵守的律令。",
      "EZMICROBALANCE-EZMB_LOTHA.talk.ANY.0-0r.ancient": "只陈述一次。干净出手。把证据留在明处。",
      "EZMICROBALANCE-EZMB_LOTHA.talk.IRONCLAD.0-0r.ancient": "怒火可以作证，但不会被赦免。",
      "EZMICROBALANCE-EZMB_LOTHA.talk.SILENT.0-0r.ancient": "沉默可以呈堂。刀刃也可以。",
      "EZMICROBALANCE-EZMB_LOTHA.talk.DEFECT.0-0r.ancient": "金属会诚实记录冲击。记录成立。",
      "EZMICROBALANCE-EZMB_LOTHA.talk.NECROBINDER.0-0r.ancient": "死者可以借你之口发声。我会听取证据。",
      "EZMICROBALANCE-EZMB_LOTHA.talk.REGENT.0-0r.ancient": "统治者入庭时，也只是众声之一。",
      "EZMB_LOTHA.title": "审判者·洛莎",
      "EZMB_LOTHA.epithet": "每一次挥击都是证据。每一回合都将宣判。",
      "EZMB_LOTHA.pages.INITIAL.description": "洛莎递来三枚裁决。选择一枚，它会成为你的遗物。",
      "EZMB_LOTHA.pages.INITIAL.options.ezmb_reroll_initial_options.title": "重掷",
      "EZMB_LOTHA.pages.INITIAL.options.ezmb_reroll_initial_options.description": "重置本次第[blue]1[/blue]幕先古奖励。骰子只在第[blue]1[/blue]幕出现，使用后消失。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.title": "反证之镜",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description": "从牌组选择[blue]1[/blue]张镜牌。每场战斗你的首回合正常抽牌后，将它移入手牌。第一次打出它：[gold]攻击牌[/gold]和[gold]技能牌[/gold]额外打出[blue]1[/blue]次；[gold]能力牌[/gold]费用变为[blue]0[/blue]。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt": "选择[gold]反证牌[/gold]。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.title": "镜厅回声",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description": "回合结束时，记住你最后打出的牌类型。下回合，你第一次打出同类型牌时触发回声：[gold]攻击牌[/gold]和[gold]技能牌[/gold]额外打出[blue]1[/blue]次；[gold]能力牌[/gold]费用变为[blue]0[/blue]并抽[blue]1[/blue]张牌。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.title": "无罪推定",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description": "战斗开始时，获得[gold]无罪[/gold]。每个玩家回合开始时，若你仍有[gold]无罪[/gold]，抽[blue]2[/blue]张牌，获得[blue]1[/blue]点[gold]能量[/gold]，并获得[blue]8[/blue]点[gold]格挡[/gold]。当你受到未被格挡的敌人[gold]攻击[/gold]伤害时，失去[gold]无罪[/gold]，立即失去[blue]8[/blue]点生命，且本场战斗不能重新获得[gold]无罪[/gold]。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.title": "终审封庭",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description": "战斗后的卡牌奖励不再出现。第[blue]1[/blue]回合：抽[blue]4[/blue]张牌，获得[blue]2[/blue]点[gold]能量[/gold]。第[blue]4[/blue]回合：抽[blue]2[/blue]张牌，获得[blue]2[/blue]点[gold]能量[/gold]。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.title": "延期判决",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description": "第[blue]4[/blue]回合开始时，抽[blue]4[/blue]张牌，获得[blue]4[/blue]点[gold]能量[/gold]和[blue]3[/blue]层[gold]裁决[/gold]。本回合每打出[blue]1[/blue]张非状态牌，消耗[blue]1[/blue]层[gold]裁决[/gold]：[gold]攻击牌[/gold]和[gold]技能牌[/gold]额外打出[blue]1[/blue]次；[gold]能力牌[/gold]费用变为[blue]0[/blue]并抽[blue]1[/blue]张牌。若战斗在第[blue]4[/blue]回合前结束，回复[blue]4[/blue]点生命。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.title": "死刑缓期",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description": "每局一次，防止死亡并将生命设为[blue]1[/blue]。你获得一个最后回合：抽[blue]10[/blue]张牌，获得[blue]10[/blue]点[gold]能量[/gold]，所有牌变为[blue]0[/blue]费，且本回合不会死亡。回合结束时，若仍有敌人，你死亡；否则继续本局。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.title": "单牌宣判",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description": "每回合，你打出的第一张[gold]攻击牌[/gold]或[gold]技能牌[/gold]额外打出[blue]2[/blue]次。之后你本回合最多再打出[blue]4[/blue]张牌。[gold]能力牌[/gold]不计入限制，费用变为[blue]0[/blue]，打出后抽[blue]1[/blue]张牌。",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.title": "公开罪证",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description": "你施加的非伤害类[gold]负面状态[/gold]层数翻倍，并获得[blue]1[/blue]层[gold]开悟[/gold]。敌人施加给你的非伤害类[gold]负面状态[/gold]层数也翻倍，并失去[blue]1[/blue]层[gold]开悟[/gold]。回合开始时，消耗至多[blue]3[/blue]层[gold]开悟[/gold]；每层抽[blue]1[/blue]张牌并获得[blue]4[/blue]点[gold]格挡[/gold]。",
      "EZMB_LOTHA.pages.DONE.description": "洛莎敲下法槌。裁决将随你而行。",
      "EZMB_LOTHA.talk.firstVisitEver.0-0.ancient": "法庭开庭。选择你登塔时必须遵守的律令。",
      "EZMB_LOTHA.talk.ANY.0-0r.ancient": "只陈述一次。干净出手。把证据留在明处。",
      "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.title": "挑战瓦库",
      "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description": "与瓦库进行赃物试炼。打破[blue]1[/blue]把或更多[gold]赃物锁[/gold]，拿走金币和额外祝福选择。第[blue]1[/blue]、[blue]3[/blue]、[blue]5[/blue]回合，从[blue]3[/blue]张[gold]契约[/gold]中选择[blue]1[/blue]张。契约能帮你破锁，但会增加[gold]血债[/gold]。破锁后可以收手。本场没有普通战斗奖励。死亡会结束本局。",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_ONE.description": "瓦库退让。选择[blue]1[/blue]份第[blue]3[/blue]幕远古祝福。",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_TWO.description": "瓦库退让。从[blue]2[/blue]份第[blue]3[/blue]幕远古祝福中选择[blue]1[/blue]份。",
      "EZMB_VAKUU_FIGHT.pages.VICTORY.description": "瓦库退让。从[blue]3[/blue]份第[blue]3[/blue]幕远古祝福中选择[blue]1[/blue]份。",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description": "瓦库退让。已经没有其他第[blue]3[/blue]幕远古祝福可选。破锁金币仍归你。",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.title": "继续",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.description": "没有未获得的祝福可选。",
      "EZMB_VAKUU_FIGHT.pages.DONE.description": "试炼结束。你获得所选祝福。"
    },
    "ascension": {
      "LEVEL_11.description": "地图更宽也更长：宽度+1；第一幕+1层，第二幕+1层，第三幕+2层。",
      "LEVEL_11.title": "宽塔长路",
      "LEVEL_12.description": "地图上会出现[gold]火印精英[/gold]。击败后获得[gold]铸令[/gold]。",
      "LEVEL_12.title": "火印精英",
      "LEVEL_13.description": "部分奖励[gold]攻击牌[/gold]和[gold]技能牌[/gold]带有[gold]裂变[/gold]：[gold]耗能[/gold]降低[blue]1[/blue]，打出后[gold]消耗[/gold]。",
      "LEVEL_13.title": "裂变附魔",
      "LEVEL_14.description": "开局获得[gold]根蚀 I[/gold]。战后仍留在主牌组里的[gold]根蚀[/gold]会恶化。最多[blue]4[/blue]张[gold]根蚀[/gold]。",
      "LEVEL_14.title": "根蚀初生",
      "LEVEL_15.description": "第二幕和第三幕首领战会埋入[blue]2[/blue]张[gold]根芽[/gold]，分别在第[blue]3[/blue]/[blue]4[/blue]回合萌发。",
      "LEVEL_15.title": "首领根芽",
      "LEVEL_16.description": "地图上会出现[gold]战旗房[/gold]。它们是带有可见[gold]战旗[/gold]规则和额外奖励的强化普通战斗。",
      "LEVEL_16.title": "战旗房",
      "LEVEL_17.description": "第二幕和第三幕各有[blue]1[/blue]条特殊路线。它更危险，奖励也更好。",
      "LEVEL_17.title": "深层支线",
      "LEVEL_18.description": "第二幕和第三幕中后段精英也会埋入[blue]1[/blue]张[gold]根芽[/gold]。",
      "LEVEL_18.title": "精英根芽",
      "LEVEL_19.description": "每名首领获得自己的专属能力。首领牌奖励多显示[blue]1[/blue]张牌。",
      "LEVEL_19.title": "首领专属能力",
      "LEVEL_20.description": "只有第[blue]3[/blue]幕第二名首领进入[gold]烙印形态[/gold]，强化它的专属能力。双首领顺序会提前显示。",
      "LEVEL_20.title": "烙印形态",
      "MODIFIER_GUIDE.description": "地图悬停会在进房前显示具体的[gold]火印[/gold]、[gold]战旗[/gold]、[gold]首领专属能力[/gold]或[gold]烙印形态[/gold]。",
      "MODIFIER_GUIDE.title": "地图强化预览",
      "FIREMARK_ELITE.description": "带有[gold]火印[/gold]和更好奖励的可选精英。1名火印宿主获得完整火印。[gold]溢火[/gold]每次最多影响1名非召唤副目标。",
      "FIREMARK_ELITE.title": "火印精英",
      "FIREMARK_MIGHT.description": "宿主开局获得[blue]{Strength}[/blue]点[gold]力量[/gold]。造成未被格挡的攻击伤害后积累[gold]热势[/gold]；[blue]2[/blue]层后，下次攻击更危险。溢火给1名正在攻击的副目标[blue]{OverflowStrength}[/blue]点临时[gold]力量[/gold]。",
      "FIREMARK_MIGHT.title": "火印：烈力",
      "FIREMARK_GIANT.description": "宿主最大生命提高[blue]{MaxHpPercent}%[/blue]。半血时暴露[gold]熔核[/gold]；窗口内打破熔核可削弱它，并对1名副目标造成[blue]{OverflowDamage}[/blue]点溢火伤害。",
      "FIREMARK_GIANT.title": "火印：巨体",
      "FIREMARK_FORGE_ARMOR.description": "你的回合开始时，宿主获得[blue]{Armor}[/blue]点[gold]熔甲[/gold]。若回合结束时宿主没有格挡，下一次熔甲跳过。溢火给1名副目标[blue]{OverflowBlock}[/blue]点[gold]格挡[/gold]。",
      "FIREMARK_FORGE_ARMOR.title": "火印：锻甲",
      "FIREMARK_CONSTANT_HEAL.description": "敌方回合结束时，宿主回复[blue]{Heal}[/blue]点生命。本轮对其造成[blue]{InterruptDamage}[/blue]点伤害可中断治疗。若治疗成功，溢火为1名受伤副目标回复[blue]{OverflowHeal}[/blue]点生命。",
      "FIREMARK_CONSTANT_HEAL.title": "火印：恒愈",
      "BANNER_ROOM.description": "带有公开[gold]战旗[/gold]规则和额外奖励的强化普通战斗。",
      "BANNER_ROOM.title": "战旗房",
      "BANNER_SHIELDWALL.description": "仅在多敌人战斗中生效。一名敌人成为旗手。旗手存活时，敌方回合结束后其他敌人获得[blue]{Block}[/blue]点[gold]格挡[/gold]；旗手死亡时，其他敌人获得[blue]{DeathBlock}[/blue]点[gold]格挡[/gold]。",
      "BANNER_SHIELDWALL.title": "盾阵战旗",
      "BANNER_BLOOD_PRIZE.description": "第[blue]3[/blue]回合结束前击杀标记敌人，战斗后获得[blue]{Gold}[/blue][gold]金币[/gold]。若它存活，它会获得[blue]{Strength}[/blue]点[gold]力量[/gold]和[blue]{Artifact}[/blue]层[gold]人工制品[/gold]。",
      "BANNER_BLOOD_PRIZE.title": "血赏战旗",
      "BANNER_PRESSING_LINE.description": "每回合第[blue]4[/blue]、[blue]5[/blue]、[blue]6[/blue]张牌会为敌阵充能。充能给敌人[blue]{PartialBlock}[/blue]-[blue]{FullBlock}[/blue]点[gold]格挡[/gold]；满层使下次攻击+[blue]{ExtraDamage}[/blue]伤害。",
      "BANNER_PRESSING_LINE.title": "压阵战旗",
      "BANNER_LAST_STAND.description": "仅在多敌人战斗中生效。第一个敌人死亡时，剩余敌人获得[blue]{Block}[/blue]点[gold]格挡[/gold]和[blue]{Strength}[/blue]点临时[gold]力量[/gold]。",
      "BANNER_LAST_STAND.title": "残阵战旗",
      "BANNER_VANGUARD.description": "敌人开战时获得[blue]{Strength}[/blue]点临时[gold]力量[/gold]；第[blue]3[/blue]回合开始时失去这些力量。",
      "ROOTBLIGHT_ADDED": "[gold]根蚀[/gold]已加入。",
      "ROOT_SYSTEM_FULL": "根系已满：最多[blue]4[/blue]张[gold]根蚀[/gold]。",
      "DEEP_BRANCH_ENTRY.description": "可选深层支线的入口。它会在后方接回主路，并保留一条普通路线。",
      "DEEP_BRANCH_ENTRY.title": "深层支线入口",
      "DEEP_BRANCH_REWARD.description": "深层支线奖励节点。因为支线路线更危险，所以奖励更高。",
      "DEEP_BRANCH_REWARD.title": "深层支线奖励",
      "DEEP_BRANCH_RISK.description": "深层支线风险节点。它是可选路线，并会接回主地图。",
      "DEEP_BRANCH_RISK.title": "深层支线风险",
      "BANNER_VANGUARD.title": "先锋战旗",
      "A20_INTERMISSION_HEADER": "前方中庭",
      "A20_INTERMISSION_PROCEED": "进入中庭",
      "BOSS_BRANDED_FORM.description": "第[blue]3[/blue]幕第二名首领进入[gold]烙印形态[/gold]。下方专属能力会在本场战斗中强化。",
      "BOSS_BRANDED_FORM.title": "烙印形态",
      "BOSS_DEDICATED_ABILITY.description": "该首领拥有一个本场战斗生效的[gold]专属能力[/gold]。影响攻击的变化会显示在意图里。",
      "BOSS_DEDICATED_ABILITY.title": "首领专属能力",
      "BOSS_KING_BRAND.description": "第[blue]3[/blue]幕第二名首领进入[gold]烙印形态[/gold]。下方专属能力会在本场战斗中强化。",
      "BOSS_KING_BRAND.title": "烙印形态",
      "BOSS_ROYAL_SEAL.description": "该首领拥有一个本场战斗生效的[gold]专属能力[/gold]。影响攻击的变化会显示在意图里。",
      "BOSS_ROYAL_SEAL.title": "首领专属能力",
      "BOSS_SEAL_HOLY_DAZE.title": "圣昏",
      "BOSS_SEAL_HOLY_DAZE.summary": "首次眩晕期间，每次受击最多受到[blue]1[/blue]点伤害。眩晕结束后，首领获得[blue]1[/blue]点[gold]力量[/gold]。",
      "BOSS_SEAL_HOLY_DAZE.brand": "首次眩晕期间，每次受击最多受到[blue]1[/blue]点伤害。眩晕结束后，首领获得[blue]2[/blue]点[gold]力量[/gold]。",
      "BOSS_SEAL_MARTYR_OATH.title": "殉誓",
      "BOSS_SEAL_MARTYR_OATH.summary": "每名随从死亡，使亲族祭司获得[blue]1[/blue]枚殉誓，最多[blue]2[/blue]枚。下一次施加负面状态时，每枚使持续时间+[blue]1[/blue]；下一次攻击时，每次命中每枚额外造成[blue]3[/blue]点伤害。",
      "BOSS_SEAL_MARTYR_OATH.brand": "殉誓仍最多[blue]2[/blue]枚。负面状态每枚持续时间+[blue]1[/blue]；下一次攻击每次命中每枚额外造成[blue]4[/blue]点伤害。若两名随从同一回合死亡，亲族祭司获得[blue]1[/blue]层[gold]人工制品[/gold]。",
      "BOSS_SEAL_INK_RETURN.title": "墨返",
      "BOSS_SEAL_INK_RETURN.summary": "首次完全移除[gold]滑溜[/gold]后，下个敌方回合返还清除量的[blue]25%[/blue]，至少[blue]3[/blue]层，最多[blue]12[/blue]层。每场触发一次。",
      "BOSS_SEAL_INK_RETURN.brand": "首次完全移除[gold]滑溜[/gold]后，下个敌方回合返还清除量的[blue]35%[/blue]，至少[blue]5[/blue]层，最多[blue]18[/blue]层。每场触发一次。",
      "BOSS_SEAL_STARTLED_SHELL.title": "多重护甲苏醒",
      "BOSS_SEAL_STARTLED_SHELL.summary": "族母醒来时获得[gold]多重护甲[/gold]。被攻击唤醒：[blue]4[/blue]层；自然醒来：[blue]8[/blue]层。多人模式按首领战规则缩放。首次[gold]摄魂[/gold]移除当前多重护甲的一半。",
      "BOSS_SEAL_STARTLED_SHELL.brand": "族母醒来时获得[gold]多重护甲[/gold]。被攻击唤醒：[blue]6[/blue]层；自然醒来：[blue]10[/blue]层。多人模式按首领战规则缩放。首次[gold]摄魂[/gold]移除当前多重护甲的三分之一。",
      "BOSS_SEAL_SOUL_TIDE.title": "魂潮",
      "BOSS_SEAL_SOUL_TIDE.summary": "魂鱼获得[gold]无形[/gold]时，获得[blue]1[/blue]层[gold]人工制品[/gold]。手牌中每张[gold]呼唤[/gold]使它下个敌方回合获得[blue]2[/blue]点格挡。队伍上限：单人[blue]8[/blue]，2人[blue]12[/blue]，3-4人[blue]16[/blue]。",
      "BOSS_SEAL_SOUL_TIDE.brand": "手牌中每张[gold]呼唤[/gold]使魂鱼下个敌方回合获得[blue]3[/blue]点格挡。队伍上限：单人[blue]12[/blue]，2人[blue]16[/blue]，3-4人[blue]20[/blue]。获得[gold]无形[/gold]时仍只获得[blue]1[/blue]层[gold]人工制品[/gold]。",
      "BOSS_SEAL_BOILING_CRITICAL.title": "不可削弱",
      "BOSS_SEAL_BOILING_CRITICAL.summary": "爆发回合，瀑布巨兽清除[gold]虚弱[/gold]和攻击降低，本回合获得足够[gold]人工制品[/gold]，并使受爆发影响的玩家获得[blue]1[/blue]回合[gold]易伤[/gold]。",
      "BOSS_SEAL_BOILING_CRITICAL.brand": "爆发回合，受爆发影响的玩家获得[blue]2[/blue]回合[gold]易伤[/gold]。爆发仍不受[gold]虚弱[/gold]和攻击降低影响，但基础伤害不提高。",
      "BOSS_SEAL_MISALIGNED_SHELL.title": "错壳校准",
      "BOSS_SEAL_MISALIGNED_SHELL.summary": "玩家回合结束时，若两只爪生命百分比相差[blue]35%[/blue]或更多，生命更高的爪获得校准。[blue]2[/blue]层后，它的下一次攻击每次命中额外造成[blue]4[/blue]点伤害。每只爪触发一次。",
      "BOSS_SEAL_MISALIGNED_SHELL.brand": "校准改为生命百分比相差[blue]30%[/blue]时触发。[blue]2[/blue]层后，下一次攻击每次命中额外造成[blue]5[/blue]点伤害。每只爪触发一次。",
      "BOSS_SEAL_MARGINAL_NOTE.title": "旁注",
      "BOSS_SEAL_MARGINAL_NOTE.summary": "[gold]知识诅咒[/gold]后，每名玩家弃牌堆加入[blue]1[/blue]张临时[gold]旁注[/gold]。留在手牌中的旁注会变为[gold]深思[/gold]。深思会给下一次知识诅咒添加附加代价。",
      "BOSS_SEAL_MARGINAL_NOTE.brand": "[gold]深思[/gold]最多[blue]3[/blue]层。每回合最多增加[blue]2[/blue]层。懒惰和枯竭的附加代价每次知识诅咒最多结算一次。",
      "BOSS_SEAL_STRUGGLE_BAIT.title": "逃亡疲劳",
      "BOSS_SEAL_STRUGGLE_BAIT.summary": "首领获得[gold]力量[/gold]或推进[gold]沙坑[/gold]时，将[blue]1[/blue]张由专属能力生成的[gold]狂乱逃离[/gold]加入受影响玩家的弃牌堆。每打出[blue]3[/blue]张这种逃离，无厌沙虫获得[blue]2[/blue]点[gold]活力[/gold]。",
      "BOSS_SEAL_STRUGGLE_BAIT.brand": "每打出[blue]3[/blue]张专属能力生成的[gold]狂乱逃离[/gold]，无厌沙虫获得[blue]3[/blue]点[gold]活力[/gold]。每个玩家回合最多触发一次。",
      "BOSS_SEAL_AEONGLASS_HOURGLASS.title": "时砂回流",
      "BOSS_SEAL_AEONGLASS_HOURGLASS.summary": "永世沙漏使用[gold]消退[/gold]后，生成[blue]2[/blue]枚时砂。下个玩家回合中，每花费[blue]1[/blue]点能量移除[blue]1[/blue]枚。每剩余[blue]1[/blue]枚时砂，使下一次[gold]加大力度[/gold]额外加入[blue]1[/blue]张[gold]枯萎[/gold]。",
      "BOSS_SEAL_AEONGLASS_HOURGLASS.brand": "使用[gold]消退[/gold]后，生成[blue]3[/blue]枚时砂。剩余时砂会加入额外[gold]枯萎[/gold]。若[gold]眼部激光[/gold]开始时仍有时砂，眼部激光额外命中[blue]1[/blue]次，每场最多触发[blue]2[/blue]次。",
      "BOSS_SEAL_CHOSEN_DECREE.title": "御令",
      "BOSS_SEAL_CHOSEN_DECREE.summary": "女王施加[gold]束缚[/gold]时，其中[blue]1[/blue]张束缚牌获得[gold]御令[/gold]。打出御令牌不会触发额外惩罚。打出非御令束缚牌时，女王获得[blue]1[/blue]层[gold]威仪[/gold]；没有打出束缚牌时，女王获得[blue]1[/blue]层威仪，火炬头获得[blue]1[/blue]点[gold]力量[/gold]。威仪使下一次防御或屏障动作每层额外获得[blue]8[/blue]点格挡。",
      "BOSS_SEAL_CHOSEN_DECREE.brand": "威仪上限变为[blue]3[/blue]层。女王一次防御或屏障动作最多消耗[blue]2[/blue]层威仪。",
      "BOSS_SEAL_RESIDUAL_SAMPLE.title": "实验记录",
      "BOSS_SEAL_RESIDUAL_SAMPLE.summary": "测试体进入新阶段时，保留上一阶段的[blue]1[/blue]份样本：力量、技能、攻击、抗体或污染。",
      "BOSS_SEAL_RESIDUAL_SAMPLE.brand": "测试体进入新阶段时，保留上一阶段的[blue]2[/blue]份不同样本。力量残留仍遵守上限。",
      "BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE": "实验记录：{Samples}。{Reason}",
      "BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH": "力量残留",
      "BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH.reason": "它保留了上一阶段的一部分力量。",
      "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL": "技能适应",
      "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL.reason": "上一阶段主要使用技能牌。",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ATTACK": "攻击适应",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ATTACK.reason": "上一阶段使用了攻击牌。",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ANTIBODY": "抗体样本",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ANTIBODY.reason": "它在上一阶段受到过负面状态影响。",
      "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED": "污染样本",
      "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED.reason": "上一阶段没有明显记录。"
    },
    "cards": {
      "BRIGHTEST_FLAME.title": "至亮之焰",
      "BRIGHTEST_FLAME.description": "[gold]消耗[/gold]。\n获得{Energy:energyIcons()}。\n抽{Cards:diff()}{Cards:plural:张牌|张牌}。\n失去{MaxHp:diff()}点最大生命。",
      "DEBT.title": "债务",
      "DEBT.description": "消耗。被消耗时，失去5金币。",
      "ENTHRALLED.title": "执迷",
      "ENTHRALLED.description": "如果这张牌在你的手牌中，你必须先打出这张牌。获得10格挡。永恒。",
      "FOLLY.title": "愚行",
      "FOLLY.description": "无法打出。固有。永恒。",
      "SOVEREIGN_BLADE.description": "{TargetType:choose(AllEnemies):对所有敌人|}造成{Damage:diff()}点伤害{Repeat:choose(1):|{}次}。\n获得[blue]3[/blue]点[gold]力量[/gold]、[blue]3[/blue]点[gold]敏捷[/gold]、[blue]3[/blue]层[gold]覆甲[/gold]、[blue]3[/blue]层[gold]再生[/gold]和[blue]3[/blue]点[gold]活力[/gold]。{GainsBlock:cond:\n获得{CalculatedBlock:diff()}点[gold]格挡[/gold].|}",
      "EZMB_ROOT.title": "根蚀 I",
      "EZMB_ROOT.description": "将本牌从你的主牌组中移除。\n若战斗结束时本牌仍在你的主牌组中，本牌变为[gold]根蚀 II[/gold]。",
      "EZMB_DEEP_ROOT.title": "根蚀 II",
      "EZMB_DEEP_ROOT.description": "打出时将本牌从你的主牌组中移除。\n战斗后，加入1张[gold]根蚀 I[/gold]。\n若战斗结束时本牌仍在你的主牌组中，本牌变为[gold]根蚀 III[/gold]。",
      "EZMB_ROOTBLIGHT_III.title": "根蚀 III",
      "EZMB_ROOTBLIGHT_III.description": "打出时将本牌从你的主牌组中移除。\n战斗后，加入1张[gold]根蚀 II[/gold]。\n若战斗结束时本牌仍在你的主牌组中，它保持为[gold]根蚀 III[/gold]。首次如此时，加入1张[gold]根蚀 I[/gold]。没有第四阶段根蚀。",
      "EZMB_ROOT_BUD.title": "根芽",
      "EZMB_ROOT_BUD.description": "[gold]临时[/gold]。第[blue]3[/blue]或第[blue]4[/blue]回合开始时，若本牌还未进入手牌，将其置于你的[gold]抽牌堆[/gold]顶部。\n若见到后未打出，战斗后加入1张[gold]根蚀 I[/gold]。若从未见到，则枯萎。",
      "EZMB_MARGINAL_NOTE.title": "旁注",
      "EZMB_MARGINAL_NOTE.description": "[gold]临时[/gold]。抽1张牌。\n若回合结束时仍在手牌中，它会变为[gold]深思[/gold]。",
      "EZMB_URDA_SEEDLING.title": "幼芽",
      "EZMB_URDA_SEEDLING.description": "[gold]临时[/gold]。获得{Block:diff()}点[gold]格挡[/gold]。",
      "EZMB_URDA_SEEDBED.title": "苗床",
      "EZMB_URDA_SEEDBED.description": "获得{Block:diff()}点[gold]格挡[/gold]。设置[blue]{Capacity}[/blue]格[gold]苗床[/gold]。{ImmediateLine}\n之后若[gold]临时[/gold]状态牌、[gold]临时[/gold]诅咒牌、[gold]根芽[/gold]或[gold]根蚀[/gold]将进入手牌，苗床会先种下它：那张牌移出本场战斗，不进入手牌，你获得[blue]1[/blue]张[gold]枯壳[/gold]。\n种下是苗床替你处理这张牌。它不是打出、丢弃或消耗，不触发这些联动。临时状态牌和临时诅咒牌被种下后，本战不会再出现；永久诅咒不能种下，也不会被删除。种下[gold]根芽[/gold]不算打出，但按已处理结算，战后不会生成[gold]根蚀 I[/gold]。种下[gold]根蚀[/gold]表示本场停住：它仍按同等级留在主牌组，战后不升级、不分裂、不移除、不降级。\n消耗。",
      "EZMB_URDA_SEEDBED.upgradeLine": "\n立即从抽牌堆或弃牌堆中种下至多[blue]{ImmediatePlantCount}[/blue]张可种下的牌。",
      "EZMB_URDA_SEEDBED.selectionScreenPrompt": "选择要种下的牌。该牌会离开本场战斗，不算打出、丢弃或消耗。根芽按已处理；根蚀保持同等级。",
      "EZMB_URDA_RAIN_BREATH.title": "雨息",
      "EZMB_URDA_RAIN_BREATH.description": "[gold]临时[/gold]。获得{Block:diff()}点[gold]格挡[/gold]。\n抽{Cards:diff()}张牌。\n消耗。",
      "EZMB_WITHERED_HUSK.title": "枯壳",
      "EZMB_WITHERED_HUSK.description": "[gold]临时[/gold]诅咒。\n被消耗时，获得{Block:diff()}点[gold]格挡[/gold]。\n苗床不会种下枯壳。",
      "EZMB_VAKUU_CONTRACT.selectionScreenPrompt": "选择一张契约。",
      "EZMB_VAKUU_KNIFE_CONTRACT.title": "刀契",
      "EZMB_VAKUU_KNIFE_CONTRACT.description": "[gold]临时[/gold]。对瓦库造成{Damage:diff()}点伤害。失去{HpLoss:diff()}点生命。若仍有[gold]赃物锁[/gold]，打破[blue]1[/blue]把。增加[blue]1[/blue]层[gold]血债[/gold]。",
      "EZMB_VAKUU_TEMPTATION.title": "金契",
      "EZMB_VAKUU_TEMPTATION.description": "[gold]临时[/gold]。获得{Energy:energyIcons()}。抽{Cards:diff()}张牌。失去{HpLoss:diff()}点生命。若仍有[gold]赃物锁[/gold]，打破[blue]1[/blue]把。增加[blue]1[/blue]层[gold]血债[/gold]。",
      "EZMB_VAKUU_SHELTER_CONTRACT.title": "避债契",
      "EZMB_VAKUU_SHELTER_CONTRACT.description": "[gold]临时[/gold]。获得{Block:diff()}点[gold]格挡[/gold]。移除{Debt:diff()}层[gold]血债[/gold]。",
      "EZMB_VAKUU_TRICK_CONTRACT.title": "诈契",
      "EZMB_VAKUU_TRICK_CONTRACT.description": "[gold]临时[/gold]。打破[blue]1[/blue]把[gold]赃物锁[/gold]。增加{Debt:diff()}层[gold]血债[/gold]。瓦库行动前，它的攻击额外造成{Backlash:diff()}点伤害。",
      "EZMB_VAKUU_CASH_OUT_CONTRACT.title": "收手契",
      "EZMB_VAKUU_CASH_OUT_CONTRACT.description": "[gold]临时[/gold]。结束瓦库战斗，带走已破锁的赃物。\n至少打破[blue]1[/blue]把锁后才能打出。",
      "EZMB_VAKUU_CASH_OUT.selectionScreenPrompt": "现在收手？",
      "EZMB_MORVI_ARCHIVE_DRAW_PAGE.title": "抽牌页",
      "EZMB_MORVI_ARCHIVE_DRAW_PAGE.description": "[gold]临时[/gold]。抽{Cards:diff()}张牌。",
      "EZMB_MORVI_ARCHIVE_VEIL_PAGE.title": "帷幕页",
      "EZMB_MORVI_ARCHIVE_VEIL_PAGE.description": "[gold]临时[/gold]。获得{Block:diff()}点[gold]格挡[/gold]。",
      "EZMB_MORVI_ARCHIVE_BURN_PAGE.title": "焚页",
      "EZMB_MORVI_ARCHIVE_BURN_PAGE.description": "[gold]临时[/gold]。对所有敌人造成{Damage:diff()}点伤害。",
      "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.title": "折扣页",
      "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.description": "[gold]临时[/gold]。你本回合打出的下一张牌费用变为[blue]0[/blue]点[gold]能量[/gold]。",
      "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.title": "勇气页",
      "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description": "[gold]临时[/gold]。获得{StrengthPower:diff()}点临时[gold]力量[/gold]。",
      "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.title": "敏捷页",
      "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description": "[gold]临时[/gold]。获得{DexterityPower:diff()}点临时[gold]敏捷[/gold]。",
      "EZMB_MORVI_RED_INK_OVERDRAFT.title": "红墨透支",
      "EZMB_MORVI_RED_INK_OVERDRAFT.description": "[gold]临时[/gold]。只能在你拥有[blue]0[/blue]点[gold]能量[/gold]时打出。抽[blue]2[/blue]张牌，获得[blue]1[/blue]点[gold]能量[/gold]，并记录[blue]1[/blue]笔[gold]红墨债[/gold]。",
      "EZMB_MORVI_WASTE_PAPER.title": "废纸",
      "EZMB_MORVI_WASTE_PAPER.description": "[gold]临时[/gold]。纸页风暴可以在它从[gold]抽牌堆[/gold]被抽到时将其消耗。"
    },
    "powers": {
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description": "[gold]死刑缓期[/gold]：缓刑期间，你的生命保持在[blue]1[/blue]点，牌费用变为[blue]0[/blue]，且你不会死亡。该玩家回合结束时，若仍有敌人存活，你死亡。",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.smartDescription": "[gold]死刑缓期[/gold]：本回合你不会死亡。牌费用变为[blue]0[/blue]。在回合结束前击杀所有敌人。",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.title": "死刑缓期",
      "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description": "[gold]开悟[/gold]：你的回合开始时，消耗至多[blue]3[/blue]层。每消耗1层，抽[blue]1[/blue]张牌并获得[blue]4[/blue]点[gold]格挡[/gold]。",
      "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.smartDescription": "[gold]开悟[/gold]：回合开始时，消耗至多[blue]3[/blue]层；每层抽[blue]1[/blue]张牌并获得[blue]4[/blue]点[gold]格挡[/gold]。",
      "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.title": "开悟",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description": "[gold]无罪[/gold]：每个玩家回合开始时，抽[blue]2[/blue]张牌，获得[blue]1[/blue]点[gold]能量[/gold]，并获得[blue]8[/blue]点[gold]格挡[/gold]。未被格挡的敌人[gold]攻击[/gold]伤害会移除此状态，并使你失去[blue]8[/blue]点生命。",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.smartDescription": "[gold]无罪[/gold]：每回合抽[blue]2[/blue]张牌，获得[blue]1[/blue]点[gold]能量[/gold]和[blue]8[/blue]点[gold]格挡[/gold]，直到受到未被格挡的敌人[gold]攻击[/gold]伤害。",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.title": "无罪",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.description": "[gold]单牌宣判[/gold]：计数为[blue]5[/blue]时表示宣判待发。下一张[gold]攻击牌[/gold]或[gold]技能牌[/gold]会额外打出[blue]2[/blue]次，然后计数变为[blue]4[/blue]，显示本回合还能打出几张牌。宣判前打出的[gold]能力牌[/gold]改为费用[blue]0[/blue]并抽[blue]1[/blue]张牌，不会降低计数。",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.smartDescription": "[gold]单牌宣判[/gold]：计数[blue]5[/blue]代表待发。下一张[gold]攻击牌[/gold]/[gold]技能牌[/gold]额外打出，之后计数显示本回合剩余可出牌数。",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.title": "单牌宣判",
      "EZMICROBALANCE-LOTHA_VERDICT_POWER.description": "[gold]裁决[/gold]：本回合，接下来每打出1张非状态牌会消耗[blue]1[/blue]层。[gold]攻击牌[/gold]和[gold]技能牌[/gold]会额外打出[blue]1[/blue]次；[gold]能力牌[/gold]改为本次打出费用变为[blue]0[/blue]并抽[blue]1[/blue]张牌。",
      "EZMICROBALANCE-LOTHA_VERDICT_POWER.smartDescription": "[gold]裁决[/gold]：非状态牌会消耗它。[gold]攻击牌[/gold]/[gold]技能牌[/gold]额外打出[blue]1[/blue]次；[gold]能力牌[/gold]费用变为[blue]0[/blue]并抽牌。",
      "EZMICROBALANCE-LOTHA_VERDICT_POWER.title": "裁决",
      "EZMICROBALANCE-MORVI_DEBT_POWER.description": "[gold]债务[/gold]：战斗结束时，至多[blue]40[/blue]点到期。优先支付[gold]金币[/gold]；每短缺[blue]10[/blue][gold]金币[/gold]（向上取整），失去[blue]3[/blue]点非致命生命。[gold]债务[/gold]都会减少到期数值。",
      "EZMICROBALANCE-MORVI_DEBT_POWER.smartDescription": "[gold]债务[/gold]：战斗结束时优先用[gold]金币[/gold]支付；不足时用非致命生命支付。",
      "EZMICROBALANCE-MORVI_DEBT_POWER.title": "债务",
      "EZMICROBALANCE-MORVI_PROOFREAD_POWER.description": "[gold]校样[/gold]：接下来每张手动打出的非状态、非诅咒牌组牌消耗[blue]1[/blue]层。未升级牌临时升级并抽[blue]1[/blue]张；已升级牌[gold]能量[/gold]费用降低[blue]1[/blue]点并获得[blue]4[/blue]点[gold]格挡[/gold]。",
      "EZMICROBALANCE-MORVI_PROOFREAD_POWER.smartDescription": "[gold]校样[/gold]：接下来的合格牌获得升级/抽牌或降费/[gold]格挡[/gold]效果。",
      "EZMICROBALANCE-MORVI_PROOFREAD_POWER.title": "校样",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_POWER.description": "[gold]开卷[/gold]：追踪开卷考试抽到的牌。第[blue]1[/blue]回合结束时，仍在手牌中的追踪牌会封存在[gold]消耗牌堆[/gold]；第[blue]3[/blue]回合在手牌空间允许时返回，并变为[blue]0[/blue]费。",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_POWER.smartDescription": "[gold]开卷[/gold]：追踪牌在第[blue]1[/blue]回合结束时封存，并在第[blue]3[/blue]回合返回。",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_POWER.title": "开卷",
      "EZMICROBALANCE-MORVI_OVERDRAFT_POWER.description": "[gold]透支[/gold]：每层都是本场战斗的一笔[gold]红墨债[/gold]。战斗结束时，每笔[gold]红墨债[/gold]支付[blue]12[/blue][gold]金币[/gold]；无法支付时改为失去[blue]3[/blue]点非致命生命。",
      "EZMICROBALANCE-MORVI_OVERDRAFT_POWER.smartDescription": "[gold]透支[/gold]：战斗结束时用[gold]金币[/gold]或非致命生命偿还。",
      "EZMICROBALANCE-MORVI_OVERDRAFT_POWER.title": "透支",
      "EZMICROBALANCE-MORVI_PAPERSTORM_POWER.description": "[gold]纸页风暴[/gold]：每回合，从[gold]抽牌堆[/gold]抽到的前[blue]2[/blue]张状态牌会被消耗。每张都会抽[blue]1[/blue]张牌并获得[blue]1[/blue]点[gold]能量[/gold]。",
      "EZMICROBALANCE-MORVI_PAPERSTORM_POWER.smartDescription": "[gold]纸页风暴[/gold]：抽到的状态牌会转化为抽牌和[gold]能量[/gold]。",
      "EZMICROBALANCE-MORVI_PAPERSTORM_POWER.title": "纸页风暴",
      "EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.description": "[gold]赃物库[/gold]：每把被打破的锁都会提高瓦库胜利奖励。用[gold]契约[/gold]破锁，或在单个玩家回合对瓦库造成[blue]40[/blue]点未被格挡伤害破锁。破锁后可以[gold]收手[/gold]。",
      "EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.smartDescription": "[gold]赃物库[/gold]：破锁获得更多奖励，然后选择是否[gold]收手[/gold]。",
      "EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.title": "赃物库",
      "EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.description": "[gold]血债[/gold]：每层使瓦库的每段攻击伤害增加[blue]2[/blue]点。试炼结束时，每层先扣除[blue]15[/blue]赃物金币；不足时失去非致命生命。",
      "EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.smartDescription": "[gold]血债[/gold]：瓦库攻击更痛。结算时先用赃物偿还。",
      "EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.title": "血债",
      "EZMICROBALANCE-VAKUU_BACKLASH_POWER.description": "[gold]反扑[/gold]：瓦库行动前，它的攻击额外造成[blue]{Amount}[/blue]点伤害。",
      "EZMICROBALANCE-VAKUU_BACKLASH_POWER.smartDescription": "[gold]反扑[/gold]：瓦库下次攻击更危险。",
      "EZMICROBALANCE-VAKUU_BACKLASH_POWER.title": "反扑"
    }
  },
  "en": {
    "relics": {
      "BEAUTIFUL_BRACELET.description": "Choose 3 cards. They gain Swift 2.",
      "BLACK_STAR.description": "Elites drop an additional Relic. If obtained in Act 3 or later, immediately obtain 1 random Relic.",
      "BLOOD_SOAKED_ROSE.description": "Gain 1 Energy. Add 1 Enthralled Curse to your deck.",
      "BRILLIANT_SCARF.description": "Every turn, the 6th card you play costs 0.",
      "CHOICES_PARADOX.description": "At the start of each combat, choose 1 of 5 usable Rare cards to add to your hand. It gains Retain and is removed after combat.",
      "CHOICES_PARADOX.selectionScreenPrompt": "Choose 1 Rare card to add to your hand.",
      "CLAWS.description": "On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul.",
      "CLAWS.eventDescription": "Transform up to [blue]{Cards}[/blue] cards into upgraded Maul.",
      "CLAWS.selectionScreenPrompt": "Choose cards to transform into upgraded Maul.",
      "CLAWS.title": "Tanx Claws",
      "CROSSBOW.description": "At the start of each turn, you may add a random Attack to your hand. It costs 1 less this turn and gains Ethereal and Exhaust.",
      "DISTINGUISHED_CAPE.description": "On pickup, lose 30% of current Max HP, at least 18. Add 3 Apparitions.",
      "DISTINGUISHED_CAPE.eventDescription": "Lose 30% of current Max HP, at least 18. Add 3 Apparitions.",
      "DISTINGUISHED_CAPE.unpayableOption": "Max HP too low to pay this cost ({Cost}).",
      "ECTOPLASM.description": "Gain 1 Energy. On pickup, gain 250 Gold. You can no longer gain Gold.",
      "FIDDLE.description": "At the start of each turn, draw until you have 7 cards. During your turn, draw effects cannot make your hand exceed 7 cards.",
      "IRON_CLUB.description": "Whenever you play 5 cards, draw 1 card.",
      "JEWELED_MASK.description": "On pickup, choose a Power. It permanently costs 0. At the start of each combat, move it from your draw pile to your hand.",
      "JEWELED_MASK.ezSelectionScreenPrompt": "Choose a Power to permanently cost 0.",
      "JEWELRY_BOX.description": "Add 1 Apotheosis to your deck. It does not have Innate.",
      "MEAT_CLEAVER.description": "Adds a [gold]Cleaver[/gold] option to rest sites: remove [blue]2[/blue] cards and lose [blue]5[/blue] HP.",
      "MUSIC_BOX.description": "Each turn, the first Attack you play creates a copy in your hand. The copy costs 1 less this turn and has Ethereal and Exhaust.",
      "PAELS_HORN.description": "Add 1 Relax and 1 Relax+ to your deck.",
      "PAELS_TOOTH.description": "Remove 5 cards. After every 2 non-boss combats, choose 1 stored removed card to return to your deck upgraded. After the act boss, remaining stored cards are removed permanently.",
      "PRESERVED_FOG.description": "Remove 4 cards. Add 1 Folly to your deck.",
      "PRISMATIC_GEM.description": "Gain 1 Energy. Every second standard card reward contains only off-color cards.",
      "PRISMATIC_GEM.countHint.title": "Prismatic count: {Count}/{Cycle}",
      "PRISMATIC_GEM.countHint.nextNormal": "Next standard card reward is normal.",
      "PRISMATIC_GEM.countHint.nextOffColor": "Next standard card reward contains only off-color cards.",
      "PRISMATIC_GEM.rewardScreenHint": "Prismatic reward: only off-color cards this time.",
      "SEAL_OF_GOLD.description": "Gain 1 Energy. Add 2 playable Debt Curses to your deck.",
      "SOZU.description": "Gain 1 Energy. On pickup, fill all empty Potion slots. You can no longer obtain Potions.",
      "SERE_TALON.description": "On pickup, choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+ to your deck.",
      "SERE_TALON.eventDescription": "Choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+.",
      "SERE_TALON.selectionScreenPrompt": "Choose 1 Curse.",
      "SERE_TALON.title": "Vakuu's Sere Talon",
      "TOASTY_MITTENS.description": "Before drawing each turn, view the top card of your draw pile. You may Exhaust it to gain 1 Strength.",
      "EZMICROBALANCE-AncientInitialRerollOptionRelic.description": "Reroll the current Act 1 Ancient rewards once. The die appears only in Act 1.",
      "EZMICROBALANCE-AncientInitialRerollOptionRelic.flavor": "A small die for the one choice you get to ask again.",
      "EZMICROBALANCE-AncientInitialRerollOptionRelic.title": "Reroll",
      "EZMICROBALANCE-ANCIENT_INITIAL_REROLL_OPTION_RELIC.description": "Reroll the current Act 1 Ancient rewards once. The die appears only in Act 1.",
      "EZMICROBALANCE-ANCIENT_INITIAL_REROLL_OPTION_RELIC.flavor": "A small die for the one choice you get to ask again.",
      "EZMICROBALANCE-ANCIENT_INITIAL_REROLL_OPTION_RELIC.title": "Reroll",
      "EZMICROBALANCE-UrdaHumusPactOptionRelic.description": "Act [blue]1[/blue] normal combat card rewards gain [gold]Compost Reward[/gold]. Taking it skips the reward cards and gives [blue]15[/blue] [gold]Gold[/gold]. The [blue]3[/blue]rd compost removes up to [blue]2[/blue] deck cards and offers [blue]1[/blue] upgraded reward card.",
      "EZMICROBALANCE-UrdaHumusPactOptionRelic.flavor": "A sealed token for the pact beneath the roots.",
      "EZMICROBALANCE-UrdaHumusPactOptionRelic.title": "Humus Pact",
      "EZMICROBALANCE-UrdaMoltingOptionRelic.description": "Remove [blue]1[/blue] Strike and [blue]1[/blue] Defend, then add [blue]2[/blue] [gold]Withered Husk[/gold] Curse cards. Remaining [gold]Withered Husk[/gold] cards are removed at Act [blue]2[/blue] start.",
      "EZMICROBALANCE-UrdaMoltingOptionRelic.flavor": "A sealed token for the bark left behind.",
      "EZMICROBALANCE-UrdaMoltingOptionRelic.title": "Molting",
      "EZMICROBALANCE-UrdaMossMapOptionRelic.description": "First Act [blue]1[/blue] rooms: Monster +[blue]25[/blue] [gold]Gold[/gold]; Event heal [blue]5[/blue]; Shop [gold]Potion[/gold]; [gold]Elite[/gold] upgrade [blue]1[/blue] random card; Rest Site +[blue]3[/blue] [gold]Max HP[/gold].",
      "EZMICROBALANCE-UrdaMossMapOptionRelic.flavor": "A sealed token for the path hidden by moss.",
      "EZMICROBALANCE-UrdaMossMapOptionRelic.title": "Moss Map",
      "EZMICROBALANCE-UrdaSeedbedOptionRelic.description": "Act [blue]1[/blue] normal combat card rewards can become [gold]Seedbed[/gold]: lose [blue]2[/blue] [gold]Max HP[/gold] and add a [gold]Seedbed[/gold]. The first one is upgraded. After taking [blue]4[/blue], gain [blue]10[/blue] [gold]Max HP[/gold]. [gold]Seedbed[/gold] is a [blue]1[/blue]-cost defense card: [blue]8[/blue]/[blue]12[/blue] Block, [blue]2[/blue]/[blue]3[/blue] spaces, and immediate planting of [blue]1[/blue]/[blue]2[/blue] eligible draw/discard cards. Later it plants [gold]Temporary[/gold] Status cards, [gold]Temporary[/gold] Curse cards, [gold]Blight Sprouts[/gold], or [gold]Rootblight[/gold] before they enter hand, giving [gold]Withered Husk[/gold] for each. Planting means Seedbed handles the card; it is not playing, discarding, or exhausting. Temporary negative cards are gone for this combat only; permanent Curses are not planted. [gold]Blight Sprouts[/gold] count as handled and add no [gold]Rootblight I[/gold]. [gold]Rootblight[/gold] is frozen for this combat and stays in the master deck at the same level.",
      "EZMICROBALANCE-UrdaSeedbedOptionRelic.flavor": "A sealed token for a patient seed.",
      "EZMICROBALANCE-UrdaSeedbedOptionRelic.title": "Seedbed",
      "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.description": "Act [blue]1[/blue] normal combat card rewards gain [gold]Compost Reward[/gold]. Taking it skips the reward cards and gives [blue]15[/blue] [gold]Gold[/gold]. The [blue]3[/blue]rd compost removes up to [blue]2[/blue] deck cards and offers [blue]1[/blue] upgraded reward card.",
      "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.flavor": "A sealed token for the pact beneath the roots.",
      "EZMICROBALANCE-URDA_HUMUS_PACT_OPTION_RELIC.title": "Humus Pact",
      "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.description": "Remove [blue]1[/blue] Strike and [blue]1[/blue] Defend, then add [blue]2[/blue] [gold]Withered Husk[/gold] Curse cards. Remaining [gold]Withered Husk[/gold] cards are removed at Act [blue]2[/blue] start.",
      "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.flavor": "A sealed token for the bark left behind.",
      "EZMICROBALANCE-URDA_MOLTING_OPTION_RELIC.title": "Molting",
      "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description": "First Act [blue]1[/blue] rooms: Monster +[blue]25[/blue] [gold]Gold[/gold]; Event heal [blue]5[/blue]; Shop [gold]Potion[/gold]; [gold]Elite[/gold] upgrade [blue]1[/blue] random card; Rest Site +[blue]3[/blue] [gold]Max HP[/gold].",
      "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.flavor": "A sealed token for the path hidden by moss.",
      "EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.title": "Moss Map",
      "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.description": "Act [blue]1[/blue] normal combat card rewards can become [gold]Seedbed[/gold]: lose [blue]2[/blue] [gold]Max HP[/gold] and add a [gold]Seedbed[/gold]. The first one is upgraded. After taking [blue]4[/blue], gain [blue]10[/blue] [gold]Max HP[/gold]. [gold]Seedbed[/gold] is a [blue]1[/blue]-cost defense card: [blue]8[/blue]/[blue]12[/blue] Block, [blue]2[/blue]/[blue]3[/blue] spaces, and immediate planting of [blue]1[/blue]/[blue]2[/blue] eligible draw/discard cards. Later it plants [gold]Temporary[/gold] Status cards, [gold]Temporary[/gold] Curse cards, [gold]Blight Sprouts[/gold], or [gold]Rootblight[/gold] before they enter hand, giving [gold]Withered Husk[/gold] for each. Planting means Seedbed handles the card; it is not playing, discarding, or exhausting. Temporary negative cards are gone for this combat only; permanent Curses are not planted. [gold]Blight Sprouts[/gold] count as handled and add no [gold]Rootblight I[/gold]. [gold]Rootblight[/gold] is frozen for this combat and stays in the master deck at the same level.",
      "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.flavor": "A sealed token for a patient seed.",
      "EZMICROBALANCE-URDA_SEEDBED_OPTION_RELIC.title": "Seedbed",
      "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.description": "Choose [blue]1[/blue] of [blue]4[/blue] cards. It is upgraded, added to your deck, and gains [gold]Trial Branch[/gold]. Play it in each of the next [blue]3[/blue] combats. Missing any combat removes it.",
      "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.flavor": "A sealed token with a green shoot through the verdict line.",
      "EZMICROBALANCE-URDA_TRIAL_BRANCH_OPTION_RELIC.title": "Trial Branch",
      "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.description": "Choose [blue]1[/blue] of [blue]2[/blue] common relics and gain [blue]75[/blue] [gold]Gold[/gold]. Defeat an Act [blue]1[/blue] [gold]Elite[/gold] to root it permanently and gain [blue]35[/blue] [gold]Gold[/gold]. If Act [blue]2[/blue] starts first, Urda takes the shallow relic back and refunds [blue]75[/blue] [gold]Gold[/gold].",
      "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.flavor": "A sealed token whose roots have not found stone yet.",
      "EZMICROBALANCE-URDA_SHALLOW_ROOT_RELIC_OPTION_RELIC.title": "Shallow-Root Relic",
      "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.description": "After each [gold]Elite[/gold] combat, heal [blue]10[/blue] HP. Firemarked Elites count.",
      "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.flavor": "A sealed token that drinks from hard-won ground.",
      "EZMICROBALANCE-URDA_ELITE_ROOT_OPTION_RELIC.title": "Elite Root",
      "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.description": "Urda marks a reachable Act [blue]1[/blue] combat in the first [blue]7[/blue] floors. Reach the [gold]Root Mark[/gold] to receive [blue]3[/blue] single-card rewards; the first is upgraded, and you gain [blue]1[/blue] [gold]Potion[/gold] if possible. Miss the mark to lose [blue]8[/blue] HP and gain [blue]25[/blue] [gold]Gold[/gold].",
      "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.flavor": "A sealed token threaded with a route only roots can read.",
      "EZMICROBALANCE-URDA_ROOTED_ROUTE_OPTION_RELIC.title": "Rooted Route",
      "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.description": "In Act [blue]1[/blue], after the first unblocked enemy attack damage each combat, gain [blue]1[/blue] [gold]Rain Breath[/gold]. At Act [blue]2[/blue] start, fewer than [blue]3[/blue] triggers grants [blue]75[/blue] [gold]Gold[/gold]; otherwise heal [blue]8[/blue] HP and upgrade [blue]1[/blue] card.",
      "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.flavor": "A sealed token still wet from the storm that spared it.",
      "EZMICROBALANCE-URDA_AFTER_RAIN_OPTION_RELIC.title": "After the Rain",
      "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.description": "Gain [blue]5[/blue] [gold]Root Eyes[/gold]. On the map, click this relic and choose a future reachable Monster, Unknown, or Elite room to preview its enemy group or event. Hover the marked room to see it. The first preview grants [blue]1[/blue] [gold]Potion[/gold] if possible.",
      "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.flavor": "A sealed token with an eye pressed into living bark.",
      "EZMICROBALANCE-URDA_ROOT_SIGHT_OPTION_RELIC.title": "Root Eyes",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.description": "Use [gold]Store Seed[/gold] on Act [blue]1[/blue] normal combat card rewards to save [blue]1[/blue] offered card, max [blue]3[/blue]. Click this relic later to add up to [blue]2[/blue] stored cards. The first chosen card is upgraded, then this relic is used up.",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.flavor": "A sealed token that rattles with future leaves.",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.descriptionFooter": "Click this relic to choose up to [blue]2[/blue] stored cards. The first chosen card is upgraded.",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.descriptionPrefix": "Stored cards:",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.storedSeeds.title": "Stored Seeds",
      "EZMICROBALANCE-URDA_SEED_BANK_OPTION_RELIC.title": "Seed Bank",
      "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.description": "Post-combat card rewards no longer appear. Turn [blue]1[/blue]: draw [blue]4[/blue], gain [blue]2[/blue] [gold]Energy[/gold]. Turn [blue]4[/blue]: draw [blue]2[/blue], gain [blue]2[/blue] [gold]Energy[/gold].",
      "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.flavor": "A sealed token from a locked courtroom.",
      "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.title": "Closed Court",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.description": "Once per run, prevent death and set HP to [blue]1[/blue]. Take one final turn: draw [blue]10[/blue], gain [blue]10[/blue] [gold]Energy[/gold], all cards cost [blue]0[/blue], and you cannot die. At turn end, if any enemies remain, die; otherwise continue the run.",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.flavor": "A sealed token for one delayed final sentence.",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.title": "Death Reprieve",
      "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.description": "At turn [blue]4[/blue] start, draw [blue]4[/blue], gain [blue]4[/blue] [gold]Energy[/gold] and [blue]3[/blue] [gold]Verdict[/gold]. This turn, each non-Status card spends [blue]1[/blue] [gold]Verdict[/gold]: [gold]Attack[/gold] and [gold]Skill[/gold] cards play [blue]1[/blue] extra time; [gold]Power[/gold] cards cost [blue]0[/blue] and draw [blue]1[/blue]. If combat ends before turn [blue]4[/blue], heal [blue]4[/blue] HP.",
      "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.flavor": "A sealed token waiting for the third exhibit.",
      "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.title": "Deferred Verdict",
      "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.description": "At turn end, remember the last card type you played. Next turn, your first matching card echoes: [gold]Attack[/gold] and [gold]Skill[/gold] cards play [blue]1[/blue] extra time; [gold]Power[/gold] cards cost [blue]0[/blue] and draw [blue]1[/blue].",
      "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.flavor": "A sealed token reflected from every wall.",
      "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.title": "Mirror Hall Echo",
      "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description": "Choose [blue]1[/blue] mirror card from your deck. On your first turn each combat, move it to your hand after your normal draw. The first time you play it: [gold]Attack[/gold] and [gold]Skill[/gold] cards play [blue]1[/blue] extra time; [gold]Power[/gold] cards cost [blue]0[/blue].",
      "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.flavor": "A sealed token that answers the first wound.",
      "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.title": "Mirror Rebuttal",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.description": "At combat start, gain [gold]Innocent[/gold]. At the start of each player turn while [gold]Innocent[/gold], draw [blue]2[/blue] cards, gain [blue]1[/blue] [gold]Energy[/gold], and gain [blue]8[/blue] [gold]Block[/gold]. When you take unblocked enemy [gold]Attack[/gold] damage, lose [gold]Innocent[/gold], lose [blue]8[/blue] HP immediately, and cannot regain [gold]Innocent[/gold] this combat.",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.flavor": "A sealed token from before evidence is heard.",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.title": "Presumption of Innocence",
      "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description": "Your non-damaging [gold]negative status[/gold] stacks apply twice and grant [blue]1[/blue] [gold]Enlightenment[/gold]. Enemy non-damaging [gold]negative status[/gold] stacks on you also apply twice and remove [blue]1[/blue] [gold]Enlightenment[/gold]. At turn start, spend up to [blue]3[/blue] [gold]Enlightenment[/gold]; each draws [blue]1[/blue] and gives [blue]4[/blue] [gold]Block[/gold].",
      "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.flavor": "A sealed token laid where all can see it.",
      "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.title": "Public Evidence",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.description": "Each turn, your first [gold]Attack[/gold] or [gold]Skill[/gold] plays [blue]2[/blue] extra times. Then you may play up to [blue]4[/blue] more cards this turn. [gold]Power[/gold] cards do not count, cost [blue]0[/blue], and draw [blue]1[/blue].",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.flavor": "A sealed token written in one line.",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.title": "Single Sentence",
      "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description": "Fight Vakuu in a greed trial. Break [blue]1[/blue]+ [gold]Stolen Locks[/gold] for loot Gold and extra blessing choices. On turns [blue]1[/blue], [blue]3[/blue], and [blue]5[/blue], choose [blue]1[/blue] of [blue]3[/blue] [gold]Contracts[/gold]. They help break locks, but add [gold]Blood Debt[/gold]. After breaking a lock, you may cash out. No normal combat rewards. Death ends the run.",
      "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.flavor": "A warning and an invitation etched on the same blade.",
      "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.title": "Vakuu Trial",
      "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.description": "Choose [blue]1[/blue] of [blue]3[/blue] upgraded [gold]Ancient[/gold] cards and add it to your deck. Playing that card costs [blue]1[/blue] HP for [gold]Attack[/gold] or [gold]Skill[/gold], or [blue]8[/blue] HP for [gold]Power[/gold]. After the Act [blue]2[/blue] boss, pay [blue]180[/blue] [gold]Gold[/gold] to keep it if possible; otherwise Morvi removes it.",
      "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.flavor": "A sealed token signed in the margin.",
      "EZMICROBALANCE-MORVI_FORBIDDEN_LOAN_OPTION_RELIC.title": "Forbidden Loan",
      "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC.description": "Once each turn, your first manually played deck [gold]Attack[/gold] or [gold]Skill[/gold] plays [blue]1[/blue] extra time. If that card's printed cost is [blue]1[/blue] or more [gold]Energy[/gold], draw [blue]1[/blue]. [gold]Power[/gold], Status, Curse, autoplay, and generated cards do not trigger.",
      "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC.flavor": "A sealed token stamped twice by mistake.",
      "EZMICROBALANCE-MORVI_MISPRINT_PRESS_OPTION_RELIC.title": "Misprint Press",
      "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.description": "Each turn, add a temporary [gold]Overdraft[/gold] card if hand space allows. Once per turn at [blue]0[/blue] [gold]Energy[/gold], play it to draw [blue]2[/blue], gain [blue]1[/blue] [gold]Energy[/gold], and add [blue]1[/blue] [gold]red-ink debt[/gold]. Each [gold]red-ink debt[/gold] costs [blue]12[/blue] [gold]Gold[/gold] at combat end, or [blue]3[/blue] nonlethal HP if unpaid.",
      "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.flavor": "A sealed token whose ink refuses to dry.",
      "EZMICROBALANCE-MORVI_RED_INK_OVERDRAFT_OPTION_RELIC.title": "Red Ink Overdraft",
      "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC.description": "At combat start, add [blue]3[/blue] random temporary [gold]Archive Pages[/gold] to hand. Pages cost [blue]0[/blue], are [gold]Ethereal[/gold] and [gold]Exhaust[/gold], and are removed after combat.",
      "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC.flavor": "A sealed token tucked behind the last shelf.",
      "EZMICROBALANCE-MORVI_OVERDUE_LIBRARY_OPTION_RELIC.title": "Overdue Library",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC.description": "On turn [blue]1[/blue], draw [blue]5[/blue] extra cards and gain [blue]2[/blue] [gold]Energy[/gold]. Extra-drawn cards left in hand at turn end are sealed in the [gold]Exhaust Pile[/gold], then return on turn [blue]3[/blue] as [blue]0[/blue]-cost cards if hand space allows.",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC.flavor": "A sealed token folded into a cheat sheet.",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_EXAM_OPTION_RELIC.title": "Open-Book Exam",
      "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC.description": "At combat start, shuffle [blue]4[/blue] [gold]Waste Paper[/gold] Status cards into your [gold]Draw Pile[/gold]. Each turn, the first [blue]2[/blue] Status cards drawn from the [gold]Draw Pile[/gold] are consumed; each draws [blue]1[/blue] and grants [blue]1[/blue] [gold]Energy[/gold].",
      "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC.flavor": "A sealed token caught in a rising stack of forms.",
      "EZMICROBALANCE-MORVI_PAPERSTORM_OPTION_RELIC.title": "Paperstorm",
      "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC.description": "At combat start, gain [blue]3[/blue] [gold]Proofread[/gold]. Your first [blue]3[/blue] manually played deck cards get a one-play benefit: unupgraded cards temporarily upgrade and draw [blue]1[/blue]; upgraded cards cost [blue]1[/blue] less [gold]Energy[/gold] and gain [blue]4[/blue] [gold]Block[/gold].",
      "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC.flavor": "A sealed token corrected in blue pencil.",
      "EZMICROBALANCE-MORVI_BLUEPRINT_PROOF_OPTION_RELIC.title": "Blueprint Proof",
      "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.description": "Gain [blue]220[/blue] [gold]Gold[/gold], remove up to [blue]2[/blue] cards, and upgrade up to [blue]2[/blue] cards. Take [blue]320[/blue] [gold]Debt[/gold]. After each combat, repay [blue]40[/blue] [gold]Gold[/gold]; for each [blue]10[/blue] short, lose [blue]3[/blue] nonlethal HP. [gold]Debt[/gold] drops by [blue]40[/blue] either way.",
      "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.flavor": "A sealed token stamped PAID IN ADVANCE.",
      "EZMICROBALANCE-MORVI_DEBT_SETTLEMENT_OPTION_RELIC.title": "Debt Settlement",
      "VELVET_CHOKER.description": "Gain 1 Energy. Each turn, the 7th and later cards played from your hand cost 1 more.",
      "WAR_HAMMER.description": "On pickup, choose 2 cards to upgrade. Whenever you defeat an Elite, randomly upgrade 4 cards.",
      "WHISPERING_EARRING.description": "Gain 1 Energy. During the first 3 turns of each combat, after drawing, automatically play your highest-cost playable card."
    },
    "ancients": {
      "EZMICROBALANCE-EZMB_URDA.title": "Urda, Loamweaver",
      "EZMICROBALANCE-EZMB_URDA.epithet": "The roots beneath the floor remember your passage.",
      "EZMB_URDA.title": "Urda, Loamweaver",
      "EZMB_URDA.epithet": "The roots beneath the floor remember your passage.",
      "EZMICROBALANCE-EZMB_URDA.talk.firstVisitEver.0-0.ancient": "The root remembers the first footprint. Choose where it will grow.",
      "EZMICROBALANCE-EZMB_URDA.talk.ANY.0-0r.ancient": "Take only what you can keep alive. The rest becomes soil.",
      "EZMB_URDA.pages.INITIAL.description": "Urda offers four root-seals. Choose one; it becomes your relic.",
      "EZMB_URDA.pages.INITIAL.options.ezmb_reroll_initial_options.title": "Reroll",
      "EZMB_URDA.pages.INITIAL.options.ezmb_reroll_initial_options.description": "Reroll these Act [blue]1[/blue] Ancient rewards once. The die appears only in Act [blue]1[/blue] and disappears after use.",
      "NEOW.pages.INITIAL.options.ezmb_reroll_initial_options.title": "Reroll",
      "NEOW.pages.INITIAL.options.ezmb_reroll_initial_options.description": "Reroll these Act [blue]1[/blue] Ancient rewards once. The die appears only in Act [blue]1[/blue] and disappears after use.",
      "EZMB_URDA.pages.INITIAL.options.urda_seedbed.title": "Seedbed",
      "EZMB_URDA.pages.INITIAL.options.urda_seedbed.description": "Act [blue]1[/blue] card rewards can become [gold]Seedbed[/gold]: lose [blue]2[/blue] [gold]Max HP[/gold] and add a [gold]Seedbed[/gold]. The first one is upgraded. After taking [blue]4[/blue], gain [blue]10[/blue] [gold]Max HP[/gold]. [gold]Seedbed[/gold] is a [blue]1[/blue]-cost defense card: [blue]8[/blue]/[blue]12[/blue] Block, [blue]2[/blue]/[blue]3[/blue] spaces, and immediate planting of [blue]1[/blue]/[blue]2[/blue] eligible draw/discard cards. Later it plants [gold]Temporary[/gold] Status cards, [gold]Temporary[/gold] Curse cards, [gold]Blight Sprouts[/gold], or [gold]Rootblight[/gold] before they enter hand, giving [gold]Withered Husk[/gold] for each. Planting means Seedbed handles the card; it is not playing, discarding, or exhausting. Temporary negative cards are gone for this combat only; permanent Curses are not planted. [gold]Blight Sprouts[/gold] count as handled and add no [gold]Rootblight I[/gold]. [gold]Rootblight[/gold] is frozen for this combat and stays in the master deck at the same level.",
      "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.title": "Humus Pact",
      "EZMB_URDA.pages.INITIAL.options.urda_humus_pact.description": "Act [blue]1[/blue] normal combat card rewards gain a [gold]Compost Reward[/gold] button. Taking it skips those cards and gives [blue]15[/blue] [gold]Gold[/gold]. The [blue]3[/blue]rd compost then removes up to [blue]2[/blue] deck cards and offers [blue]1[/blue] upgraded reward card.",
      "EZMB_URDA.pages.INITIAL.options.urda_molting.title": "Molting",
      "EZMB_URDA.pages.INITIAL.options.urda_molting.description": "Remove [blue]1[/blue] Strike and [blue]1[/blue] Defend, then add [blue]2[/blue] [gold]Withered Husk[/gold] Curse cards. Remaining Husks are removed at Act [blue]2[/blue] start.",
      "EZMB_URDA.pages.INITIAL.options.urda_moss_map.title": "Moss Map",
      "EZMB_URDA.pages.INITIAL.options.urda_moss_map.description": "First Act [blue]1[/blue] rooms: Monster +[blue]25[/blue] [gold]Gold[/gold]; Event heal [blue]5[/blue]; Shop [gold]Potion[/gold]; [gold]Elite[/gold] upgrade [blue]1[/blue] random card; Rest Site +[blue]3[/blue] [gold]Max HP[/gold].",
      "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.title": "Trial Branch",
      "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.description": "Choose [blue]1[/blue] of [blue]4[/blue] cards. It is upgraded, added to your deck, and gains [gold]Trial Branch[/gold]. Play it in each of the next [blue]3[/blue] combats. Missing any combat removes it.",
      "EZMB_URDA.pages.INITIAL.options.urda_trial_branch.selectionScreenPrompt": "Choose [blue]1[/blue] card for [gold]Trial Branch[/gold].",
      "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.title": "Shallow-Root Relic",
      "EZMB_URDA.pages.INITIAL.options.urda_shallow_root_relic.description": "Choose [blue]1[/blue] common relic and gain [blue]75[/blue] [gold]Gold[/gold]. Defeat an Act [blue]1[/blue] [gold]Elite[/gold] to keep it; otherwise it is refunded at Act [blue]2[/blue] start.",
      "EZMB_URDA.pages.INITIAL.options.urda_elite_root.title": "Elite Root",
      "EZMB_URDA.pages.INITIAL.options.urda_elite_root.description": "After each [gold]Elite[/gold] combat, heal [blue]10[/blue] HP. Firemarked Elites count.",
      "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.title": "Rooted Route",
      "EZMB_URDA.pages.INITIAL.options.urda_rooted_route.description": "Act [blue]1[/blue] gains a [gold]Root Mark[/gold] route. Reach the marked combat for better rewards; miss it to lose [blue]8[/blue] HP and gain [blue]25[/blue] [gold]Gold[/gold].",
      "EZMB_URDA.pages.INITIAL.options.urda_after_rain.title": "After the Rain",
      "EZMB_URDA.pages.INITIAL.options.urda_after_rain.description": "In Act [blue]1[/blue], after the first unblocked enemy attack damage each combat, gain [blue]1[/blue] [gold]Rain Breath[/gold]. At Act [blue]2[/blue] start, fewer than [blue]3[/blue] triggers grants [blue]75[/blue] [gold]Gold[/gold]; otherwise heal [blue]8[/blue] HP and upgrade [blue]1[/blue] card.",
      "EZMB_URDA.pages.INITIAL.options.urda_root_sight.title": "Root Eyes",
      "EZMB_URDA.pages.INITIAL.options.urda_root_sight.description": "Gain [blue]5[/blue] [gold]Root Eyes[/gold]. Click this relic on the map, then choose a future reachable Monster, Unknown, or Elite room. The chosen room reveals and keeps its enemy group or event. Hover the marked room to see it.",
      "EZMB_URDA.root_sight.hover.title": "Root Eyes",
      "EZMB_URDA.root_sight.hover.description": "On the map, click this relic to preview a future reachable Monster, Unknown, or Elite room. Hover the marked room to see the result. Rest Sites, Shops, Treasure, and Boss rooms cannot be chosen.",
      "EZMB_URDA.root_sight.selection_hover.title": "Preview with Root Eyes",
      "EZMB_URDA.root_sight.selection_hover.description": "Click to reveal this room's enemy group or event.",
      "EZMB_URDA.root_sight.map_hover.title": "Root Eyes Preview",
      "EZMB_URDA.root_sight.map_hover.description": "Root Eyes previewed this room.",
      "EZMB_URDA.root_sight.map_hover.preview_description": "Root Eyes previewed this result. Enter this room to face it.",
      "EZMB_URDA.root_sight.map_hover.event_preview_description": "Root Eyes previewed this event. Main options: {Options}",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.title": "Seed Bank",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.description": "Act [blue]1[/blue] card rewards can store offered cards, max [blue]3[/blue]. Click this relic later to add up to [blue]2[/blue] stored cards; the first chosen card is upgraded. Then the relic is used up.",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.storeSelectionPrompt": "Choose [blue]1[/blue] offered card to store as a [gold]Seed[/gold].",
      "EZMB_URDA.pages.INITIAL.options.urda_seed_bank.settlementSelectionPrompt": "Choose up to [blue]2[/blue] stored [gold]Seeds[/gold] to add to your deck.",
      "EZMB_URDA.pages.DONE.description": "Urda's blessing sinks into the soil. The chosen path is set.",
      "EZMB_URDA.talk.firstVisitEver.0-0.ancient": "The root remembers the first footprint. Choose where it will grow.",
      "EZMB_URDA.talk.ANY.0-0r.ancient": "Take only what you can keep alive. The rest becomes soil.",
      "EZMICROBALANCE-EZMB_MORVI.title": "Morvi, the Lender-Scribe",
      "EZMICROBALANCE-EZMB_MORVI.epithet": "Every favor arrives with a margin note.",
      "EZMICROBALANCE-EZMB_MORVI.talk.firstVisitEver.0-0.ancient": "Ink is a patient creditor. Borrow carefully.",
      "EZMICROBALANCE-EZMB_MORVI.talk.ANY.0-0r.ancient": "Every margin has room for another debt.",
      "EZMICROBALANCE-EZMB_MORVI.talk.IRONCLAD.0-0r.ancient": "You know the weight of a bargain. Sign only what you can survive.",
      "EZMICROBALANCE-EZMB_MORVI.talk.SILENT.0-0r.ancient": "A quiet signature still binds. Read the margin.",
      "EZMICROBALANCE-EZMB_MORVI.talk.DEFECT.0-0r.ancient": "Even a perfect machine can owe interest. Choose the useful error.",
      "EZMICROBALANCE-EZMB_MORVI.talk.NECROBINDER.0-0r.ancient": "Revenge is expensive. I can advance the cost.",
      "EZMICROBALANCE-EZMB_MORVI.talk.REGENT.0-0r.ancient": "A crown is a promise to pay later. Your terms await.",
      "EZMB_MORVI.title": "Morvi, the Lender-Scribe",
      "EZMB_MORVI.epithet": "Every favor arrives with a margin note.",
      "EZMB_MORVI.pages.INITIAL.description": "Morvi offers three signed debts. Choose one; it becomes your relic.",
      "EZMB_MORVI.pages.INITIAL.options.ezmb_reroll_initial_options.title": "Reroll",
      "EZMB_MORVI.pages.INITIAL.options.ezmb_reroll_initial_options.description": "Reroll these Act [blue]1[/blue] Ancient rewards once. The die appears only in Act [blue]1[/blue] and disappears after use.",
      "EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.title": "Forbidden Loan",
      "EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.description": "Choose [blue]1[/blue] of [blue]3[/blue] upgraded [gold]Ancient[/gold] cards and add it to your deck. Playing that card costs HP: [gold]Attack[/gold] and [gold]Skill[/gold] cost [blue]1[/blue]; [gold]Power[/gold] costs [blue]8[/blue]. After the Act [blue]2[/blue] boss, pay [blue]180[/blue] [gold]Gold[/gold] to keep it.",
      "EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.title": "Misprint Press",
      "EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.description": "Once each turn, your first manually played deck [gold]Attack[/gold] or [gold]Skill[/gold] plays [blue]1[/blue] extra time. If that card's printed cost is [blue]1[/blue] or more [gold]Energy[/gold], draw [blue]1[/blue]. [gold]Power[/gold] cards and generated cards do not trigger.",
      "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.title": "Red Ink Overdraft",
      "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description": "Each turn, add a temporary [gold]Overdraft[/gold]. Once per turn at [blue]0[/blue] [gold]Energy[/gold], play it to draw [blue]2[/blue], gain [blue]1[/blue] [gold]Energy[/gold], then pay [gold]red-ink debt[/gold] after combat or lose nonlethal HP.",
      "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.title": "Overdue Library",
      "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.description": "At combat start, add [blue]3[/blue] temporary [gold]Archive Pages[/gold] to hand. They cost [blue]0[/blue] and are removed after combat.",
      "EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.title": "Open-Book Exam",
      "EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.description": "On turn [blue]1[/blue], draw [blue]5[/blue] extra cards and gain [blue]2[/blue] [gold]Energy[/gold]. Cards left in hand are sealed in the [gold]Exhaust Pile[/gold], then return on turn [blue]3[/blue] as [blue]0[/blue]-cost cards.",
      "EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.title": "Paperstorm",
      "EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.description": "At combat start, shuffle [blue]4[/blue] [gold]Waste Paper[/gold] into your [gold]Draw Pile[/gold]. Each turn, the first [blue]2[/blue] Status cards drawn from it exhaust, draw [blue]1[/blue], and gain [blue]1[/blue] [gold]Energy[/gold].",
      "EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.title": "Blueprint Proof",
      "EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.description": "At combat start, gain [blue]3[/blue] [gold]Proofread[/gold]. Your first [blue]3[/blue] manually played deck cards get one benefit: unupgraded cards upgrade and draw [blue]1[/blue]; upgraded cards cost [blue]1[/blue] less and gain [blue]4[/blue] [gold]Block[/gold].",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.title": "Debt Settlement",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description": "Gain [blue]220[/blue] [gold]Gold[/gold], remove up to [blue]2[/blue] cards, and upgrade up to [blue]2[/blue] cards. Take [blue]320[/blue] [gold]Debt[/gold]. After each combat, repay [blue]40[/blue] [gold]Gold[/gold]; for each [blue]10[/blue] short, lose [blue]3[/blue] nonlethal HP. [gold]Debt[/gold] drops by [blue]40[/blue] either way.",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.removeSelectionPrompt": "Remove up to [blue]2[/blue] cards for [gold]Debt Settlement[/gold].",
      "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.upgradeSelectionPrompt": "Upgrade up to [blue]2[/blue] cards for [gold]Debt Settlement[/gold].",
      "EZMB_MORVI.pages.DONE.description": "Morvi closes the ledger. The debt is written.",
      "EZMB_MORVI.talk.firstVisitEver.0-0.ancient": "Ink is a patient creditor. Borrow carefully.",
      "EZMB_MORVI.talk.ANY.0-0r.ancient": "Every margin has room for another debt.",
      "EZMICROBALANCE-EZMB_LOTHA.title": "Lotha, the Judge",
      "EZMICROBALANCE-EZMB_LOTHA.epithet": "Every strike is evidence. Every turn receives a sentence.",
      "EZMICROBALANCE-EZMB_LOTHA.talk.firstVisitEver.0-0.ancient": "The court opens. Choose the law your climb must obey.",
      "EZMICROBALANCE-EZMB_LOTHA.talk.ANY.0-0r.ancient": "Speak once. Strike cleanly. Leave proof where all can see.",
      "EZMICROBALANCE-EZMB_LOTHA.talk.IRONCLAD.0-0r.ancient": "Rage may testify, but it will not be excused.",
      "EZMICROBALANCE-EZMB_LOTHA.talk.SILENT.0-0r.ancient": "Silence is admissible. So is the knife.",
      "EZMICROBALANCE-EZMB_LOTHA.talk.DEFECT.0-0r.ancient": "Metal records impact honestly. Let the record stand.",
      "EZMICROBALANCE-EZMB_LOTHA.talk.NECROBINDER.0-0r.ancient": "The dead may speak through you. I will hear the evidence.",
      "EZMICROBALANCE-EZMB_LOTHA.talk.REGENT.0-0r.ancient": "A ruler enters court as one voice among many.",
      "EZMB_LOTHA.title": "Lotha, the Judge",
      "EZMB_LOTHA.epithet": "Every strike is evidence. Every turn receives a sentence.",
      "EZMB_LOTHA.pages.INITIAL.description": "Lotha offers three rulings. Choose one; it becomes your relic.",
      "EZMB_LOTHA.pages.INITIAL.options.ezmb_reroll_initial_options.title": "Reroll",
      "EZMB_LOTHA.pages.INITIAL.options.ezmb_reroll_initial_options.description": "Reroll these Act [blue]1[/blue] Ancient rewards once. The die appears only in Act [blue]1[/blue] and disappears after use.",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.title": "Mirror Rebuttal",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description": "Choose [blue]1[/blue] mirror card from your deck. On your first turn each combat, move it to your hand after your normal draw. The first time you play it: [gold]Attack[/gold] and [gold]Skill[/gold] cards play [blue]1[/blue] extra time; [gold]Power[/gold] cards cost [blue]0[/blue].",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt": "Choose the [gold]Rebuttal Card[/gold].",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.title": "Mirror Hall Echo",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description": "At turn end, remember the last card type you played. Next turn, your first matching card echoes: [gold]Attack[/gold] and [gold]Skill[/gold] cards play [blue]1[/blue] extra time; [gold]Power[/gold] cards cost [blue]0[/blue] and draw [blue]1[/blue].",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.title": "Presumption of Innocence",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description": "At combat start, gain [gold]Innocent[/gold]. At the start of each player turn while [gold]Innocent[/gold], draw [blue]2[/blue] cards, gain [blue]1[/blue] [gold]Energy[/gold], and gain [blue]8[/blue] [gold]Block[/gold]. When you take unblocked enemy [gold]Attack[/gold] damage, lose [gold]Innocent[/gold], lose [blue]8[/blue] HP immediately, and cannot regain [gold]Innocent[/gold] this combat.",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.title": "Closed Court",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description": "Post-combat card rewards no longer appear. Turn [blue]1[/blue]: draw [blue]4[/blue], gain [blue]2[/blue] [gold]Energy[/gold]. Turn [blue]4[/blue]: draw [blue]2[/blue], gain [blue]2[/blue] [gold]Energy[/gold].",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.title": "Deferred Verdict",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description": "At turn [blue]4[/blue] start, draw [blue]4[/blue], gain [blue]4[/blue] [gold]Energy[/gold] and [blue]3[/blue] [gold]Verdict[/gold]. This turn, each non-Status card spends [blue]1[/blue] [gold]Verdict[/gold]: [gold]Attack[/gold] and [gold]Skill[/gold] cards play [blue]1[/blue] extra time; [gold]Power[/gold] cards cost [blue]0[/blue] and draw [blue]1[/blue]. If combat ends before turn [blue]4[/blue], heal [blue]4[/blue] HP.",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.title": "Death Reprieve",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description": "Once per run, prevent death and set HP to [blue]1[/blue]. Take one final turn: draw [blue]10[/blue], gain [blue]10[/blue] [gold]Energy[/gold], all cards cost [blue]0[/blue], and you cannot die. At turn end, if any enemies remain, die; otherwise continue the run.",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.title": "Single Sentence",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description": "Each turn, your first [gold]Attack[/gold] or [gold]Skill[/gold] plays [blue]2[/blue] extra times. Then you may play up to [blue]4[/blue] more cards this turn. [gold]Power[/gold] cards do not count, cost [blue]0[/blue], and draw [blue]1[/blue].",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.title": "Public Evidence",
      "EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description": "Your non-damaging [gold]negative status[/gold] stacks apply twice and grant [blue]1[/blue] [gold]Enlightenment[/gold]. Enemy non-damaging [gold]negative status[/gold] stacks on you also apply twice and remove [blue]1[/blue] [gold]Enlightenment[/gold]. At turn start, spend up to [blue]3[/blue] [gold]Enlightenment[/gold]; each draws [blue]1[/blue] and gives [blue]4[/blue] [gold]Block[/gold].",
      "EZMB_LOTHA.pages.DONE.description": "Lotha strikes the bench. The ruling follows you.",
      "EZMB_LOTHA.talk.firstVisitEver.0-0.ancient": "The court opens. Choose the law your climb must obey.",
      "EZMB_LOTHA.talk.ANY.0-0r.ancient": "Speak once. Strike cleanly. Leave proof where all can see.",
      "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.title": "Face Vakuu",
      "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description": "Fight Vakuu in a greed trial. Break [blue]1[/blue]+ [gold]Stolen Locks[/gold] for loot Gold and extra blessing choices. On turns [blue]1[/blue], [blue]3[/blue], and [blue]5[/blue], choose [blue]1[/blue] of [blue]3[/blue] [gold]Contracts[/gold]. They help break locks, but add [gold]Blood Debt[/gold]. After breaking a lock, you may cash out. No normal combat rewards. Death ends the run.",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_ONE.description": "Vakuu yields. Choose [blue]1[/blue] Act [blue]3[/blue] Ancient blessing.",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_TWO.description": "Vakuu yields. Choose [blue]1[/blue] of [blue]2[/blue] Act [blue]3[/blue] Ancient blessings.",
      "EZMB_VAKUU_FIGHT.pages.VICTORY.description": "Vakuu yields. Choose [blue]1[/blue] of [blue]3[/blue] Act [blue]3[/blue] Ancient blessings.",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description": "Vakuu yields. No other Act [blue]3[/blue] Ancient blessing remains. Broken-lock Gold is still yours.",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.title": "Continue",
      "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE.description": "No unclaimed blessing remains.",
      "EZMB_VAKUU_FIGHT.pages.DONE.description": "The trial ends. The chosen blessing is yours."
    },
    "ascension": {
      "LEVEL_11.description": "Maps are wider and longer: width +1; Act 1 +1 row, Act 2 +1 row, Act 3 +2 rows.",
      "LEVEL_11.title": "Wide Tower, Long Road",
      "LEVEL_12.description": "[gold]Firemarked Elites[/gold] appear on the map. Defeat one to gain a [gold]Forge Token[/gold].",
      "LEVEL_12.title": "Firemarked Elites",
      "LEVEL_13.description": "Some [gold]Attack[/gold] and [gold]Skill[/gold] rewards have [gold]Fission[/gold]: they cost [blue]1[/blue] less and [gold]Exhaust[/gold] after play.",
      "LEVEL_13.title": "Fission Enchantment",
      "LEVEL_14.description": "Start with [gold]Rootblight I[/gold]. Ignored [gold]Rootblight[/gold] worsens after combat. Max [blue]4[/blue] [gold]Rootblights[/gold].",
      "LEVEL_14.title": "Rootblight Begins",
      "LEVEL_15.description": "Act [blue]2[/blue] and Act [blue]3[/blue] Boss fights bury [blue]2[/blue] [gold]Blight Sprouts[/gold]. They sprout on rounds [blue]3[/blue] and [blue]4[/blue].",
      "LEVEL_15.title": "Boss Blight Sprout",
      "LEVEL_16.description": "[gold]Banner Rooms[/gold] appear on the map. They are tougher normal combats with visible [gold]Banner[/gold] rules and extra rewards.",
      "LEVEL_16.title": "Banner Rooms",
      "LEVEL_17.description": "Act [blue]2[/blue] and Act [blue]3[/blue] each contain [blue]1[/blue] optional special route with higher risk and better rewards.",
      "LEVEL_17.title": "Deep Branches",
      "LEVEL_18.description": "Mid and late Act [blue]2[/blue] and Act [blue]3[/blue] Elites also bury [blue]1[/blue] [gold]Blight Sprout[/gold].",
      "LEVEL_18.title": "Elite Blight Sprout",
      "LEVEL_19.description": "Each Boss gains its own dedicated ability. Boss card rewards show [blue]1[/blue] more card.",
      "LEVEL_19.title": "Boss Dedicated Abilities",
      "LEVEL_20.description": "Only the second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold], strengthening its dedicated ability. The double-Boss order is revealed early.",
      "LEVEL_20.title": "Branded Form",
      "MODIFIER_GUIDE.description": "Map hover previews name the exact [gold]Firemark[/gold], [gold]Banner[/gold], [gold]Boss Dedicated Ability[/gold], or [gold]Branded Form[/gold] before you enter the room.",
      "MODIFIER_GUIDE.title": "Map Modifier Preview",
      "FIREMARK_ELITE.description": "An optional [gold]Firemarked Elite[/gold] with better rewards. One Firemark Host gets the full Firemark. Overflow affects at most 1 other non-summon enemy at a time.",
      "FIREMARK_ELITE.title": "Firemarked Elite",
      "FIREMARK_MIGHT.description": "The host starts with [blue]{Strength}[/blue] [gold]Strength[/gold]. Unblocked attack damage builds [gold]Heat[/gold]; at [blue]2[/blue], its next attack hits harder. Overflow gives [blue]{OverflowStrength}[/blue] temporary [gold]Strength[/gold] to 1 attacking secondary enemy.",
      "FIREMARK_MIGHT.title": "Firemark: Might",
      "FIREMARK_GIANT.description": "The host starts with [blue]{MaxHpPercent}%[/blue] more max HP. At half HP, break its [gold]Molten Core[/gold] in the window to reduce it. A broken core deals [blue]{OverflowDamage}[/blue] overflow damage to 1 secondary enemy.",
      "FIREMARK_GIANT.title": "Firemark: Giant",
      "FIREMARK_FORGE_ARMOR.description": "At the start of your turn, the host gains [blue]{Armor}[/blue] [gold]Molten Armor[/gold]. If the host has no Block at turn end, the next Molten Armor is skipped. Overflow gives [blue]{OverflowBlock}[/blue] [gold]Block[/gold] to 1 secondary enemy.",
      "FIREMARK_FORGE_ARMOR.title": "Firemark: Forge Armor",
      "FIREMARK_CONSTANT_HEAL.description": "At enemy turn end, the host heals [blue]{Heal}[/blue] HP. Deal [blue]{InterruptDamage}[/blue] damage to it in one round to interrupt the heal. If it heals, overflow heals 1 damaged secondary enemy for [blue]{OverflowHeal}[/blue] HP.",
      "FIREMARK_CONSTANT_HEAL.title": "Firemark: Constant Heal",
      "BANNER_ROOM.description": "Enhanced normal combat with a public [gold]Banner[/gold] rule and extra reward.",
      "BANNER_ROOM.title": "Banner Room",
      "BANNER_SHIELDWALL.description": "Multi-enemy fights only. One enemy becomes the bannerbearer. While it lives, other enemies gain [blue]{Block}[/blue] [gold]Block[/gold] after the enemy turn; when it dies, they gain [blue]{DeathBlock}[/blue] [gold]Block[/gold].",
      "BANNER_SHIELDWALL.title": "Shieldwall Banner",
      "BANNER_BLOOD_PRIZE.description": "Kill the marked enemy before round [blue]3[/blue] ends to gain [blue]{Gold}[/blue] [gold]Gold[/gold]. If it survives, it retaliates with [blue]{Strength}[/blue] [gold]Strength[/gold] and [blue]{Artifact}[/blue] [gold]Artifact[/gold].",
      "BANNER_BLOOD_PRIZE.title": "Blood Prize Banner",
      "BANNER_PRESSING_LINE.description": "Each turn, your [blue]4th[/blue], [blue]5th[/blue], and [blue]6th[/blue] cards charge the enemy line. Charge gives enemies [blue]{PartialBlock}[/blue]-[blue]{FullBlock}[/blue] [gold]Block[/gold]; full charge adds +[blue]{ExtraDamage}[/blue] attack damage.",
      "BANNER_PRESSING_LINE.title": "Pressing Line Banner",
      "BANNER_LAST_STAND.description": "Multi-enemy fights only. The first enemy death makes the rest gain [blue]{Block}[/blue] [gold]Block[/gold] and [blue]{Strength}[/blue] temporary [gold]Strength[/gold].",
      "BANNER_LAST_STAND.title": "Last Stand Banner",
      "BANNER_VANGUARD.description": "Enemies start with [blue]{Strength}[/blue] temporary [gold]Strength[/gold]. It is removed at the start of round [blue]3[/blue].",
      "ROOTBLIGHT_ADDED": "[gold]Rootblight[/gold] added.",
      "ROOT_SYSTEM_FULL": "Root system full: max [blue]4[/blue] [gold]Rootblights[/gold].",
      "DEEP_BRANCH_ENTRY.description": "Entrance to an optional Deep Branch. It reconnects later, and a normal route remains available.",
      "DEEP_BRANCH_ENTRY.title": "Deep Branch Entrance",
      "DEEP_BRANCH_REWARD.description": "A Deep Branch reward node. It pays better because the branch path is riskier.",
      "DEEP_BRANCH_REWARD.title": "Deep Branch Reward",
      "DEEP_BRANCH_RISK.description": "A Deep Branch risk node. It is optional and reconnects to the main map.",
      "DEEP_BRANCH_RISK.title": "Deep Branch Risk",
      "BANNER_VANGUARD.title": "Vanguard Banner",
      "A20_INTERMISSION_HEADER": "Courtyard Ahead",
      "A20_INTERMISSION_PROCEED": "Enter the Courtyard",
      "BOSS_BRANDED_FORM.description": "The second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold]. Its listed dedicated ability is stronger in this combat.",
      "BOSS_BRANDED_FORM.title": "Branded Form",
      "BOSS_DEDICATED_ABILITY.description": "This Boss has a [gold]dedicated ability[/gold] active in this combat. Attack changes from this ability are shown in intent.",
      "BOSS_DEDICATED_ABILITY.title": "Boss Dedicated Ability",
      "BOSS_KING_BRAND.description": "The second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold]. Its listed dedicated ability is stronger in this combat.",
      "BOSS_KING_BRAND.title": "Branded Form",
      "BOSS_ROYAL_SEAL.description": "This Boss has a [gold]dedicated ability[/gold] active in this combat. Attack changes from this ability are shown in intent.",
      "BOSS_ROYAL_SEAL.title": "Boss Dedicated Ability",
      "BOSS_SEAL_HOLY_DAZE.title": "Holy Daze",
      "BOSS_SEAL_HOLY_DAZE.summary": "During the first stun, each hit deals at most [blue]1[/blue] damage. When the stun ends, the Boss gains [blue]1[/blue] [gold]Strength[/gold].",
      "BOSS_SEAL_HOLY_DAZE.brand": "During the first stun, each hit deals at most [blue]1[/blue] damage. When the stun ends, the Boss gains [blue]2[/blue] [gold]Strength[/gold].",
      "BOSS_SEAL_MARTYR_OATH.title": "Martyr Oath",
      "BOSS_SEAL_MARTYR_OATH.summary": "Each follower death gives Kin Priest [blue]1[/blue] Martyr Oath, up to [blue]2[/blue]. Its next debuff lasts [blue]1[/blue] longer per Oath, or each hit of its next attack deals +[blue]3[/blue] damage per Oath.",
      "BOSS_SEAL_MARTYR_OATH.brand": "Martyr Oath is still capped at [blue]2[/blue]. Debuffs last [blue]1[/blue] longer per Oath; each hit of the next attack deals +[blue]4[/blue] damage per Oath. If both followers die in one turn, Kin Priest gains [blue]1[/blue] [gold]Artifact[/gold].",
      "BOSS_SEAL_INK_RETURN.title": "Ink Return",
      "BOSS_SEAL_INK_RETURN.summary": "The first time [gold]Slippery[/gold] is fully removed, Vantom restores [blue]25%[/blue] of the cleared amount next enemy turn, min [blue]3[/blue], max [blue]12[/blue]. Triggers once.",
      "BOSS_SEAL_INK_RETURN.brand": "The first time [gold]Slippery[/gold] is fully removed, Vantom restores [blue]35%[/blue] of the cleared amount next enemy turn, min [blue]5[/blue], max [blue]18[/blue]. Triggers once.",
      "BOSS_SEAL_STARTLED_SHELL.title": "Plating Wake",
      "BOSS_SEAL_STARTLED_SHELL.summary": "When the Matriarch wakes, it gains [gold]Plating[/gold]. Hit wake: [blue]4[/blue]. Natural wake: [blue]8[/blue]. Multiplayer uses boss Plating scaling. The first [gold]Soul Siphon[/gold] removes half of its current Plating.",
      "BOSS_SEAL_STARTLED_SHELL.brand": "When the Matriarch wakes, it gains [gold]Plating[/gold]. Hit wake: [blue]6[/blue]. Natural wake: [blue]10[/blue]. Multiplayer uses boss Plating scaling. The first [gold]Soul Siphon[/gold] removes one-third of its current Plating.",
      "BOSS_SEAL_SOUL_TIDE.title": "Soul Tide",
      "BOSS_SEAL_SOUL_TIDE.summary": "When Soul Fysh gains [gold]Intangible[/gold], it gains [blue]1[/blue] [gold]Artifact[/gold]. Each [gold]Beckon[/gold] left in hand gives it [blue]2[/blue] Block next enemy turn. Team cap: solo [blue]8[/blue], 2 players [blue]12[/blue], 3-4 players [blue]16[/blue].",
      "BOSS_SEAL_SOUL_TIDE.brand": "Each [gold]Beckon[/gold] left in hand gives Soul Fysh [blue]3[/blue] Block next enemy turn. Team cap: solo [blue]12[/blue], 2 players [blue]16[/blue], 3-4 players [blue]20[/blue]. [gold]Artifact[/gold] gain remains [blue]1[/blue].",
      "BOSS_SEAL_BOILING_CRITICAL.title": "Unweakenable",
      "BOSS_SEAL_BOILING_CRITICAL.summary": "On the explosion turn, Waterfall Giant clears [gold]Weak[/gold] and attack-down, gains enough [gold]Artifact[/gold] for this turn, and gives affected players [blue]1[/blue] turn of [gold]Vulnerable[/gold].",
      "BOSS_SEAL_BOILING_CRITICAL.brand": "On the explosion turn, affected players gain [blue]2[/blue] turns of [gold]Vulnerable[/gold]. The explosion still ignores [gold]Weak[/gold] and attack-down, but its base damage is not increased.",
      "BOSS_SEAL_MISALIGNED_SHELL.title": "Claw Calibration",
      "BOSS_SEAL_MISALIGNED_SHELL.summary": "At player turn end, if the claws' HP percentages differ by [blue]35%[/blue] or more, the healthier claw gains Calibration. At [blue]2[/blue], each hit of its next attack deals +[blue]4[/blue] damage. Each claw triggers once.",
      "BOSS_SEAL_MISALIGNED_SHELL.brand": "Calibration checks at [blue]30%[/blue] HP difference. At [blue]2[/blue], each hit of the healthier claw's next attack deals +[blue]5[/blue] damage. Each claw triggers once.",
      "BOSS_SEAL_MARGINAL_NOTE.title": "Marginal Note",
      "BOSS_SEAL_MARGINAL_NOTE.summary": "[gold]Curse of Knowledge[/gold] adds [blue]1[/blue] temporary [gold]Marginal Note[/gold] to each player's discard pile. Notes left in hand become [gold]Deep Thought[/gold]. Deep Thought adds side costs to the next Knowledge curse.",
      "BOSS_SEAL_MARGINAL_NOTE.brand": "[gold]Deep Thought[/gold] can reach [blue]3[/blue]. Each turn can add at most [blue]2[/blue]. Sloth and Waste Away side costs still resolve once per Knowledge curse.",
      "BOSS_SEAL_STRUGGLE_BAIT.title": "Escape Fatigue",
      "BOSS_SEAL_STRUGGLE_BAIT.summary": "When the Boss gains [gold]Strength[/gold] or advances [gold]Sandpit[/gold], add [blue]1[/blue] ability-made [gold]Frantic Escape[/gold] to an affected player's discard pile. Every [blue]3[/blue] such Escapes played gives [blue]2[/blue] [gold]Vigor[/gold].",
      "BOSS_SEAL_STRUGGLE_BAIT.brand": "Every [blue]3[/blue] ability-made [gold]Frantic Escapes[/gold] played gives [blue]3[/blue] [gold]Vigor[/gold] instead. Triggers at most once each player turn.",
      "BOSS_SEAL_AEONGLASS_HOURGLASS.title": "Time Sand Reflow",
      "BOSS_SEAL_AEONGLASS_HOURGLASS.summary": "After [gold]Ebb[/gold], create [blue]2[/blue] Time Sand. During the next player turn, each energy spent removes [blue]1[/blue]. Each remaining Time Sand makes the next [gold]Increasing Intensity[/gold] add [blue]1[/blue] extra [gold]Wither[/gold].",
      "BOSS_SEAL_AEONGLASS_HOURGLASS.brand": "After [gold]Ebb[/gold], create [blue]3[/blue] Time Sand. Remaining Time Sand adds extra [gold]Wither[/gold]. If [gold]Eye Lasers[/gold] begins while Time Sand remains, it hits [blue]1[/blue] extra time, up to [blue]2[/blue] times per fight.",
      "BOSS_SEAL_CHOSEN_DECREE.title": "Royal Decree",
      "BOSS_SEAL_CHOSEN_DECREE.summary": "When Queen applies [gold]Bound[/gold], [blue]1[/blue] Bound card gains [gold]Royal Decree[/gold]. Playing the Decree has no extra penalty. Playing a non-Decree Bound card gives Queen [blue]1[/blue] [gold]Majesty[/gold]; playing no Bound card gives [blue]1[/blue] Majesty and gives Torch Head [blue]1[/blue] [gold]Strength[/gold]. Majesty adds [blue]8[/blue] Block per stack to the next defense or barrier action.",
      "BOSS_SEAL_CHOSEN_DECREE.brand": "Majesty cap becomes [blue]3[/blue]. Queen can spend at most [blue]2[/blue] Majesty on one defense or barrier action.",
      "BOSS_SEAL_RESIDUAL_SAMPLE.title": "Experimental Record",
      "BOSS_SEAL_RESIDUAL_SAMPLE.summary": "When Test Subject changes phase, it keeps [blue]1[/blue] sample from the previous phase: Strength, Skill, Attack, Antibody, or Contamination.",
      "BOSS_SEAL_RESIDUAL_SAMPLE.brand": "When Test Subject changes phase, it keeps [blue]2[/blue] different samples from the previous phase. Strength residue still follows its cap.",
      "BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE": "Experimental Record: {Samples}. {Reason}",
      "BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH": "Strength Residue",
      "BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH.reason": "It keeps part of its previous Strength.",
      "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL": "Skill Adaptation",
      "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL.reason": "Players mostly used Skills last phase.",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ATTACK": "Attack Adaptation",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ATTACK.reason": "Players used Attacks last phase.",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ANTIBODY": "Antibody Sample",
      "BOSS_SEAL_RESIDUAL_SAMPLE_ANTIBODY.reason": "It was hit by a debuff last phase.",
      "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED": "Contamination Sample",
      "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED.reason": "No clear pattern was recorded."
    },
    "cards": {
      "BRIGHTEST_FLAME.title": "Quality Flame",
      "BRIGHTEST_FLAME.description": "[gold]Exhaust[/gold].\nGain {Energy:energyIcons()}.\nDraw {Cards:diff()} {Cards:plural:card|cards}.\nLose {MaxHp:diff()} Max HP.",
      "DEBT.title": "Debt",
      "DEBT.description": "Exhaust. When Exhausted, lose 5 Gold.",
      "ENTHRALLED.title": "Enthralled",
      "ENTHRALLED.description": "If this is in your hand, you must play it before other cards. Gain 10 Block. Eternal.",
      "FOLLY.title": "Folly",
      "FOLLY.description": "Unplayable. Innate. Eternal.",
      "SOVEREIGN_BLADE.description": "Deal {Damage:diff()} damage{TargetType:choose(AllEnemies): to ALL enemies|}{Repeat:plural:| {} times}.\nGain [blue]3[/blue] [gold]Strength[/gold], [blue]3[/blue] [gold]Dexterity[/gold], [blue]3[/blue] [gold]Plating[/gold], [blue]3[/blue] [gold]Regen[/gold], and [blue]3[/blue] [gold]Vigor[/gold].{GainsBlock:cond:\nGain {CalculatedBlock:diff()} [gold]Block[/gold].|}",
      "EZMB_ROOT.title": "Rootblight I",
      "EZMB_ROOT.description": "Remove this from your deck.\nIf this is still in your deck after combat, it becomes [gold]Rootblight II[/gold].",
      "EZMB_DEEP_ROOT.title": "Rootblight II",
      "EZMB_DEEP_ROOT.description": "When played, remove this from your deck. After combat, add a [gold]Rootblight I[/gold].\nIf this is still in your deck after combat, it becomes [gold]Rootblight III[/gold].",
      "EZMB_ROOTBLIGHT_III.title": "Rootblight III",
      "EZMB_ROOTBLIGHT_III.description": "When played, remove this from your deck. After combat, add a [gold]Rootblight II[/gold].\nIf this is still in your deck after combat, it stays [gold]Rootblight III[/gold]. The first time, add a [gold]Rootblight I[/gold]. No Rootblight IV.",
      "EZMB_ROOT_BUD.title": "Blight Sprout",
      "EZMB_ROOT_BUD.description": "[gold]Temporary[/gold]. On round [blue]3[/blue] or [blue]4[/blue], if this has not entered your hand, put it on top of your [gold]Draw Pile[/gold].\nIf seen and not played, add a [gold]Rootblight I[/gold] after combat. If never seen, it withers.",
      "EZMB_MARGINAL_NOTE.title": "Marginal Note",
      "EZMB_MARGINAL_NOTE.description": "[gold]Temporary[/gold]. Draw 1 card.\nIf this stays in your hand at end of turn, it becomes [gold]Deep Thought[/gold].",
      "EZMB_URDA_SEEDLING.title": "Seedling",
      "EZMB_URDA_SEEDLING.description": "[gold]Temporary[/gold]. Gain {Block:diff()} [gold]Block[/gold].",
      "EZMB_URDA_SEEDBED.title": "Seedbed",
      "EZMB_URDA_SEEDBED.description": "Gain {Block:diff()} [gold]Block[/gold]. Set up a [blue]{Capacity}[/blue]-space [gold]Seedbed[/gold].{ImmediateLine}\nLater, if a [gold]Temporary[/gold] Status card, [gold]Temporary[/gold] Curse card, [gold]Blight Sprout[/gold], or [gold]Rootblight[/gold] would enter your hand, Seedbed plants it first: the card leaves this combat, does not enter your hand, and you get [blue]1[/blue] [gold]Withered Husk[/gold].\nPlanting is Seedbed handling the card. It is not playing, discarding, or exhausting it, so those triggers do not fire. Temporary Status and Curse cards are gone for this combat. Permanent Curses cannot be planted and are not deleted. A planted [gold]Blight Sprout[/gold] is handled without being played, so it adds no [gold]Rootblight I[/gold] after combat. A planted [gold]Rootblight[/gold] is held still for this combat: it remains in your master deck at the same level and does not upgrade, split, get removed, or downgrade after combat.\nExhaust.",
      "EZMB_URDA_SEEDBED.upgradeLine": "\nImmediately plant up to [blue]{ImmediatePlantCount}[/blue] eligible cards from your draw or discard pile.",
      "EZMB_URDA_SEEDBED.selectionScreenPrompt": "Choose cards to plant. The card leaves this combat without being played, discarded, or exhausted. Sprouts are handled; Rootblight keeps its level.",
      "EZMB_URDA_RAIN_BREATH.title": "Rain Breath",
      "EZMB_URDA_RAIN_BREATH.description": "[gold]Temporary[/gold]. Gain {Block:diff()} [gold]Block[/gold].\nDraw {Cards:diff()} card.\nExhaust.",
      "EZMB_WITHERED_HUSK.title": "Withered Husk",
      "EZMB_WITHERED_HUSK.description": "[gold]Temporary[/gold] Curse.\nWhen exhausted, gain {Block:diff()} [gold]Block[/gold].\nSeedbed cannot plant this.",
      "EZMB_VAKUU_CONTRACT.selectionScreenPrompt": "Choose a contract.",
      "EZMB_VAKUU_KNIFE_CONTRACT.title": "Knife Contract",
      "EZMB_VAKUU_KNIFE_CONTRACT.description": "[gold]Temporary[/gold]. Deal {Damage:diff()} damage to Vakuu. Lose {HpLoss:diff()} HP. If a [gold]Stolen Lock[/gold] remains, break [blue]1[/blue]. Add [blue]1[/blue] [gold]Blood Debt[/gold].",
      "EZMB_VAKUU_TEMPTATION.title": "Gold Contract",
      "EZMB_VAKUU_TEMPTATION.description": "[gold]Temporary[/gold]. Gain {Energy:energyIcons()}. Draw {Cards:diff()} cards. Lose {HpLoss:diff()} HP. If a [gold]Stolen Lock[/gold] remains, break [blue]1[/blue]. Add [blue]1[/blue] [gold]Blood Debt[/gold].",
      "EZMB_VAKUU_SHELTER_CONTRACT.title": "Avoid Debt",
      "EZMB_VAKUU_SHELTER_CONTRACT.description": "[gold]Temporary[/gold]. Gain {Block:diff()} [gold]Block[/gold]. Remove {Debt:diff()} [gold]Blood Debt[/gold].",
      "EZMB_VAKUU_TRICK_CONTRACT.title": "Fraud Contract",
      "EZMB_VAKUU_TRICK_CONTRACT.description": "[gold]Temporary[/gold]. Break [blue]1[/blue] [gold]Stolen Lock[/gold]. Add {Debt:diff()} [gold]Blood Debt[/gold]. Vakuu's attacks deal {Backlash:diff()} more damage until it acts.",
      "EZMB_VAKUU_CASH_OUT_CONTRACT.title": "Cash Out",
      "EZMB_VAKUU_CASH_OUT_CONTRACT.description": "[gold]Temporary[/gold]. End the Vakuu fight and take the loot from broken locks.\nCan be played after breaking at least [blue]1[/blue] lock.",
      "EZMB_VAKUU_CASH_OUT.selectionScreenPrompt": "Cash out now?",
      "EZMB_MORVI_ARCHIVE_DRAW_PAGE.title": "Draw Page",
      "EZMB_MORVI_ARCHIVE_DRAW_PAGE.description": "[gold]Temporary[/gold]. Draw {Cards:diff()} cards.",
      "EZMB_MORVI_ARCHIVE_VEIL_PAGE.title": "Veil Page",
      "EZMB_MORVI_ARCHIVE_VEIL_PAGE.description": "[gold]Temporary[/gold]. Gain {Block:diff()} [gold]Block[/gold].",
      "EZMB_MORVI_ARCHIVE_BURN_PAGE.title": "Burn Page",
      "EZMB_MORVI_ARCHIVE_BURN_PAGE.description": "[gold]Temporary[/gold]. Deal {Damage:diff()} damage to all enemies.",
      "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.title": "Discount Page",
      "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.description": "[gold]Temporary[/gold]. The next card you play this turn costs [blue]0[/blue] [gold]Energy[/gold].",
      "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.title": "Bravery Page",
      "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description": "[gold]Temporary[/gold]. Gain {StrengthPower:diff()} temporary [gold]Strength[/gold].",
      "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.title": "Dexterity Page",
      "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description": "[gold]Temporary[/gold]. Gain {DexterityPower:diff()} temporary [gold]Dexterity[/gold].",
      "EZMB_MORVI_RED_INK_OVERDRAFT.title": "Red Ink Overdraft",
      "EZMB_MORVI_RED_INK_OVERDRAFT.description": "[gold]Temporary[/gold]. Can only be played when you have [blue]0[/blue] [gold]Energy[/gold]. Draw [blue]2[/blue] cards, gain [blue]1[/blue] [gold]Energy[/gold], and record [blue]1[/blue] [gold]red-ink debt[/gold].",
      "EZMB_MORVI_WASTE_PAPER.title": "Waste Paper",
      "EZMB_MORVI_WASTE_PAPER.description": "[gold]Temporary[/gold]. Paperstorm can consume this from the [gold]Draw Pile[/gold]."
    },
    "powers": {
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description": "[gold]Death Reprieve[/gold]: HP is held at [blue]1[/blue], cards cost [blue]0[/blue], and you cannot die during this reprieve. At the end of this player turn, if any enemies remain, you die.",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.smartDescription": "[gold]Death Reprieve[/gold]: you cannot die this turn. Cards cost [blue]0[/blue]. Kill all enemies before turn end.",
      "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.title": "Death Reprieve",
      "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description": "[gold]Enlightenment[/gold]: at the start of your turn, consume up to [blue]3[/blue]. For each, draw [blue]1[/blue] card and gain [blue]4[/blue] [gold]Block[/gold].",
      "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.smartDescription": "[gold]Enlightenment[/gold]: at turn start, consume up to [blue]3[/blue]. Each draws [blue]1[/blue] and gives [blue]4[/blue] [gold]Block[/gold].",
      "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.title": "Enlightenment",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description": "[gold]Innocent[/gold]: at the start of each player turn, draw [blue]2[/blue] cards, gain [blue]1[/blue] [gold]Energy[/gold], and gain [blue]8[/blue] [gold]Block[/gold]. Unblocked enemy [gold]Attack[/gold] damage removes this and makes you lose [blue]8[/blue] HP.",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.smartDescription": "[gold]Innocent[/gold]: each turn draw [blue]2[/blue], gain [blue]1[/blue] [gold]Energy[/gold], and gain [blue]8[/blue] [gold]Block[/gold] until unblocked enemy [gold]Attack[/gold] damage.",
      "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.title": "Innocent",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.description": "[gold]Single Sentence[/gold]: the counter starts at [blue]5[/blue] while the sentence is ready. Your next [gold]Attack[/gold] or [gold]Skill[/gold] plays [blue]2[/blue] extra times, then the counter becomes [blue]4[/blue] and shows remaining card plays this turn. A [gold]Power[/gold] before the sentence costs [blue]0[/blue] and draws [blue]1[/blue] without lowering the counter.",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.smartDescription": "[gold]Single Sentence[/gold]: ready at [blue]5[/blue]. The next [gold]Attack[/gold]/[gold]Skill[/gold] extra-plays, then this counts remaining card plays this turn.",
      "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.title": "Single Sentence",
      "EZMICROBALANCE-LOTHA_VERDICT_POWER.description": "[gold]Verdict[/gold]: this turn, each next non-Status card consumes [blue]1[/blue] stack. [gold]Attack[/gold] and [gold]Skill[/gold] cards play [blue]1[/blue] additional time. [gold]Power[/gold] cards instead cost [blue]0[/blue] for that play and draw [blue]1[/blue] card.",
      "EZMICROBALANCE-LOTHA_VERDICT_POWER.smartDescription": "[gold]Verdict[/gold]: non-Status cards consume it. [gold]Attack[/gold]/[gold]Skill[/gold] cards play [blue]1[/blue] additional time; [gold]Power[/gold] cards cost [blue]0[/blue] and draw.",
      "EZMICROBALANCE-LOTHA_VERDICT_POWER.title": "Verdict",
      "EZMICROBALANCE-MORVI_DEBT_POWER.description": "[gold]Debt[/gold]: at combat end, up to [blue]40[/blue] comes due. Pay [gold]Gold[/gold] first; for each [blue]10[/blue] short, rounded up, lose [blue]3[/blue] nonlethal HP. [gold]Debt[/gold] decreases by the due amount either way.",
      "EZMICROBALANCE-MORVI_DEBT_POWER.smartDescription": "[gold]Debt[/gold]: combat-end payments consume [gold]Gold[/gold] first, then nonlethal HP if short.",
      "EZMICROBALANCE-MORVI_DEBT_POWER.title": "Debt",
      "EZMICROBALANCE-MORVI_PROOFREAD_POWER.description": "[gold]Proofread[/gold]: each next manually played deck card that is not Status or Curse consumes [blue]1[/blue] stack. Unupgraded cards temporarily upgrade and draw [blue]1[/blue]. Upgraded cards cost [blue]1[/blue] less [gold]Energy[/gold] and gain [blue]4[/blue] [gold]Block[/gold].",
      "EZMICROBALANCE-MORVI_PROOFREAD_POWER.smartDescription": "[gold]Proofread[/gold]: the next eligible deck cards get upgrade/draw or discount/[gold]Block[/gold] benefits.",
      "EZMICROBALANCE-MORVI_PROOFREAD_POWER.title": "Proofread",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_POWER.description": "[gold]Open Book[/gold]: cards drawn by Open-Book Exam are tracked. Cards left in hand at turn [blue]1[/blue] end are sealed in the [gold]Exhaust Pile[/gold] and return on turn [blue]3[/blue] as [blue]0[/blue]-cost cards if hand space allows.",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_POWER.smartDescription": "[gold]Open Book[/gold]: tracked cards seal at turn [blue]1[/blue] end and return on turn [blue]3[/blue].",
      "EZMICROBALANCE-MORVI_OPEN_BOOK_POWER.title": "Open Book",
      "EZMICROBALANCE-MORVI_OVERDRAFT_POWER.description": "[gold]Overdraft[/gold]: each stack is a [gold]red-ink debt[/gold] from this combat. At combat end, each [gold]red-ink debt[/gold] pays [blue]12[/blue] [gold]Gold[/gold]; if unpaid, it loses [blue]3[/blue] nonlethal HP instead.",
      "EZMICROBALANCE-MORVI_OVERDRAFT_POWER.smartDescription": "[gold]Overdraft[/gold]: combat-end debts cost [gold]Gold[/gold] or nonlethal HP.",
      "EZMICROBALANCE-MORVI_OVERDRAFT_POWER.title": "Overdraft",
      "EZMICROBALANCE-MORVI_PAPERSTORM_POWER.description": "[gold]Paperstorm[/gold]: each turn, the first [blue]2[/blue] Status cards drawn from the [gold]Draw Pile[/gold] are consumed. Each draws [blue]1[/blue] card and grants [blue]1[/blue] [gold]Energy[/gold].",
      "EZMICROBALANCE-MORVI_PAPERSTORM_POWER.smartDescription": "[gold]Paperstorm[/gold]: drawn Status cards are converted into draw and [gold]Energy[/gold].",
      "EZMICROBALANCE-MORVI_PAPERSTORM_POWER.title": "Paperstorm",
      "EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.description": "[gold]Stolen Vault[/gold]: each broken lock improves Vakuu victory rewards. Break locks with [gold]Contracts[/gold] or by dealing [blue]40[/blue] unblocked damage to Vakuu in one player turn. After breaking a lock, you may [gold]Cash Out[/gold].",
      "EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.smartDescription": "[gold]Stolen Vault[/gold]: break locks for more rewards, then choose whether to [gold]Cash Out[/gold].",
      "EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.title": "Stolen Vault",
      "EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.description": "[gold]Blood Debt[/gold]: each stack makes Vakuu's attack hits deal [blue]2[/blue] more damage. At trial end, each stack removes [blue]15[/blue] loot Gold first; unpaid debt costs nonlethal HP.",
      "EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.smartDescription": "[gold]Blood Debt[/gold]: Vakuu hits harder. Trial loot pays debt before you keep the rest.",
      "EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.title": "Blood Debt",
      "EZMICROBALANCE-VAKUU_BACKLASH_POWER.description": "[gold]Backlash[/gold]: Vakuu's attacks deal [blue]{Amount}[/blue] more damage until it acts.",
      "EZMICROBALANCE-VAKUU_BACKLASH_POWER.smartDescription": "[gold]Backlash[/gold]: Vakuu's next attacks hit harder.",
      "EZMICROBALANCE-VAKUU_BACKLASH_POWER.title": "Backlash"
    }
  }
};
/* END_EMBEDDED_LOCALIZATION */

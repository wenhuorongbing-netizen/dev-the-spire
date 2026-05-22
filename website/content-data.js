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
  "BLOOD_SOAKED_ROSE.description": "blood_soaked_rose.png"
};

const sourceCardOverrides = {
  "BRIGHTEST_FLAME.description": {
    icon: "../source%20code/images/packed/card_portraits/event/brightest_flame.png",
    vanilla: "获得2点能量，抽2张牌，失去1点最大生命；升级后获得3点能量，抽3张牌，失去1点最大生命。",
    current: "新增消耗；获得2点能量，抽3张牌，失去1点最大生命；升级后获得3点能量，抽4张牌，失去1点最大生命。"
  }
};

const cardDescOverrides = {
  EZMB_URDA_SEEDLING: "0费技能。消耗。获得4点格挡；升级后获得7点格挡。",
  EZMB_URDA_SEEDBED: "1费技能。消耗。获得4点格挡，设置2格苗床；之后进入手牌的根芽、根蚀、状态牌或诅咒牌会先种入苗床，每种入1张加入1张枯壳。升级后获得6点格挡，容量变为3格。",
  EZMB_WITHERED_HUSK: "诅咒。虚无，消耗。被消耗时获得3点格挡。",
  EZMB_MORVI_ARCHIVE_DRAW_PAGE: "0费临时页。虚无，消耗。抽2张牌。",
  EZMB_MORVI_ARCHIVE_VEIL_PAGE: "0费临时页。虚无，消耗。获得14点格挡。",
  EZMB_MORVI_ARCHIVE_BURN_PAGE: "0费临时页。虚无，消耗。对所有敌人造成10点伤害。",
  EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE: "0费临时页。虚无，消耗。本回合下一张牌费用变为0。",
  EZMB_MORVI_ARCHIVE_BRAVERY_PAGE: "0费临时页。虚无，消耗。获得2点临时力量。",
  EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE: "0费临时页。虚无，消耗。获得2点临时敏捷。",
  EZMB_VAKUU_KNIFE_CONTRACT: "0费契约。虚无，消耗。对瓦库造成22点伤害，失去4点生命；若仍有赃物锁，打破1把并增加1层血债。",
  EZMB_VAKUU_TEMPTATION: "0费契约。虚无，消耗。获得2点能量，抽2张牌，失去5点生命；若仍有赃物锁，打破1把并增加1层血债。",
  EZMB_VAKUU_SHELTER_CONTRACT: "0费契约。虚无，消耗。获得24点格挡，失去3点生命；若仍有赃物锁，打破1把并增加1层血债。"
};

const ascDescOverrides = {
  FIREMARK_MIGHT: "火印精英开局获得力量。第1/2/3幕分别为1/2/4点；造成未被格挡的攻击伤害后积累热势，2层后下次攻击更危险。",
  FIREMARK_GIANT: "火印精英最大生命提高。第1/2/3幕分别为20%/30%/45%；半血时暴露熔核，窗口内造成足够伤害可削弱它。",
  FIREMARK_FORGE_ARMOR: "火印精英每个敌方回合后获得熔甲。第1/2/3幕分别为8/14/24点；首次打碎本次熔甲后，下次熔甲不会生成。",
  FIREMARK_CONSTANT_HEAL: "火印精英每个敌方回合回复生命。第1/2/3幕分别回复4/8/16点；本轮造成20/40/80点伤害可阻止治疗。",
  BANNER_VANGUARD: "敌人开战时获得临时力量。第1/2/3幕分别为1/2/4点；第3回合开始时失去这些力量。",
  BANNER_SHIELDWALL: "多敌人战斗中，一名敌人成为旗手。旗手存活时，敌方回合结束后其他敌人获得格挡；旗手死亡时，其他敌人获得5/10/20点格挡。",
  BANNER_BLOOD_PRIZE: "第3回合结束前击杀标记敌人，战斗后获得15/30/55金币。若它存活，它会获得1/2/4点力量和1/1/2层人工制品。",
  BANNER_PRESSING_LINE: "每回合从第4张牌开始敌阵充能，最多3层。充能给敌人4-6/8-12/16-24点格挡；满层使下次攻击+1/2/4伤害。",
  BANNER_LAST_STAND: "多敌人战斗中，第一个敌人死亡时，剩余敌人获得格挡和临时力量；力量第1/2/3幕分别为1/2/4点。"
};

window.SPIRE_PLUS_DATA = {
  labels: {
    brandSub: "《杀戮尖塔 2》平衡与内容扩展 · 温火融冰制作",
    navUpdates: "\u66f4\u65b0\u5185\u5bb9",
    navInstall: "\u4e0b\u8f7d\u4e0e\u5b89\u88c5",
    navForum: "\u8bba\u575b",
    navIssues: "\u5df2\u77e5\u95ee\u9898",
    releaseLine: "\u5f53\u524d\u624b\u52a8\u6d4b\u8bd5\u5305 \u00b7 v0.1.0-private-beta.0",
    heroTitle: "Spire Plus",
    heroCopy: "面向玩家的改动记录。第一页只列原版与当前效果对比。",
    introTitle: "模组说明",
    introCopy: "Spire Plus 是《杀戮尖塔 2》的机制扩展与平衡重构包。当前重点是先古奖励、A11-A20、状态令牌和预览工具：让奖励更明确、更强，但同时保留代价、路线压力和战斗决策。",
    download: "\u4e0b\u8f7d\u6a21\u7ec4",
    viewIssues: "\u67e5\u770b\u5df2\u77e5\u95ee\u9898",
    all: "\u5168\u90e8",
    search: "\u641c\u7d22",
    searchPlaceholder: "\u9057\u7269\u3001\u5148\u53e4\u3001\u8fdb\u9636\u3001\u5173\u952e\u8bcd",
    vanilla: "\u539f\u7248",
    current: "\u5f53\u524d",
    installTitle: "\u4e0b\u8f7d\u4e0e\u5b89\u88c5",
    installLead: "\u5f53\u524d\u4e0b\u8f7d\u6307\u5411\u6700\u65b0\u624b\u52a8\u6d4b\u8bd5\u5305\u3002\u516c\u5f00\u7ad9\u9700\u8981 GitHub Release \u4e0a\u4f20\u540c\u540d\u538b\u7f29\u5305\u540e\u624d\u4f1a\u76f4\u8fbe\u4e0b\u8f7d\u3002",
    currentDownload: "\u5f53\u524d\u4e0b\u8f7d",
    directDownload: "直链下载",
    openRelease: "\u6253\u5f00\u53d1\u5e03\u9875",
    openBaseLib: "下载 BaseLib",
    openRepo: "\u6253\u5f00\u4ed3\u5e93",
    steps: "\u5b89\u88c5\u6b65\u9aa4",
    requirements: "\u8fd0\u884c\u8981\u6c42",
    assetPolicy: "\u56fe\u7247\u4e0e\u7248\u6743\u8fb9\u754c",
    forumTitle: "\u8bba\u575b",
    forumLead: "公开发帖、反馈和回复使用 GitHub Issues / Discussions。本机区域只作为反馈模板和草稿，不会同步给其他玩家。",
    forumPublicTitle: "公开讨论入口",
    forumDraftTitle: "本机反馈模板",
    postName: "名字",
    postNamePlaceholder: "名字，可留空",
    anonymous: "匿名玩家",
    postTitle: "\u6807\u9898",
    postBody: "\u5185\u5bb9",
    postTitlePlaceholder: "\u8f93\u5165\u6807\u9898",
    postBodyPlaceholder: "\u672c\u5730\u533f\u540d\u7559\u8a00\uff0c\u53ea\u4fdd\u5b58\u5728\u5f53\u524d\u6d4f\u89c8\u5668\u3002",
    replyPlaceholder: "写回复",
    replySubmit: "回复",
    postSubmit: "保存草稿",
    postClear: "清空草稿",
    noPosts: "暂无本机草稿。",
    issuesTitle: "\u5df2\u77e5\u95ee\u9898\u4e0e\u66f4\u65b0\u8bb0\u5f55",
    issuesLead: "\u5148\u5217\u963b\u585e\u548c\u5f85\u9a8c\u8bc1\u4e8b\u9879\uff0c\u518d\u5217\u5df2\u5b8c\u6210\u7684\u66f4\u65b0\u8bb0\u5f55\u3002\u4e0d\u8981\u628a\u5f85\u9a8c\u8bc1\u5185\u5bb9\u5199\u6210\u5df2\u53d1\u5e03\u627f\u8bfa\u3002",
    knownIssues: "\u5df2\u77e5\u95ee\u9898",
    changeLog: "\u66f4\u65b0\u8bb0\u5f55",
    noTitle: "\u672a\u547d\u540d"
  },
  summary: [
    ["更新内容", "原版与当前效果对比", "updates"],
    ["下载安装", "测试包、依赖和安装路径", "install"],
    ["论坛", "反馈、发帖和回复入口", "forum"],
    ["已知问题", "阻塞项与更新记录", "issues"]
  ],
  package: {
    localDownload: "../publish/SpirePlus-v0.1.0-private-beta.0.zip",
    releaseDownload:
      "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/latest/download/SpirePlus-v0.1.0-private-beta.0.zip",
    releasesPage: "https://github.com/wenhuorongbing-netizen/dev-the-spire/releases",
    baseLibRelease: "https://github.com/Alchyr/BaseLib-StS2/releases/tag/v3.1.4",
    repository: "https://github.com/wenhuorongbing-netizen/dev-the-spire",
    meta: [
      ["\u6587\u4ef6", "SpirePlus-v0.1.0-private-beta.0.zip"],
      ["\u7248\u672c", "v0.1.0-private-beta.0"],
      ["\u663e\u793a\u540d", "Spire Plus"],
      ["\u5b89\u88c5\u76ee\u5f55", "EZMicroBalance"],
      ["\u4f9d\u8d56", "BaseLib v3.1.4"],
      ["\u6e38\u620f\u7248\u672c", "Slay the Spire 2 v0.106.0"],
      ["\u4f53\u79ef", "15,228,395 \u5b57\u8282"],
      ["\u54c8\u5e0c", "B235DEA5219FFAB46905CE076664C0D3F2E7DF80B4B5C289AA9B1985525E942D"]
    ]
  },
  installSteps: [
    "\u4e0b\u8f7d SpirePlus-v0.1.0-private-beta.0.zip\u3002",
    "下载 BaseLib v3.1.4；版本页应显示 BaseLib-StS2 v3.1.4。",
    "Windows 常见路径：Steam\\steamapps\\common\\Slay the Spire 2。",
    "\u89e3\u538b\u540e\u786e\u8ba4\u6839\u76ee\u5f55\u662f EZMicroBalance \u6587\u4ef6\u5939\u3002",
    "模组最终位置：mods\\EZMicroBalance\\EZMicroBalance.json。",
    "BaseLib 最终位置：mods\\BaseLib\\BaseLib.json。",
    "\u542f\u52a8\u6e38\u620f\uff0c\u5728\u6a21\u7ec4\u5217\u8868\u91cc\u542f\u7528 Spire Plus\u3002",
    "\u5f53\u524d\u6d4b\u8bd5\u5305\u4ecd\u9700\u5b9e\u673a\u9a8c\u8bc1\uff1b\u9047\u5230\u95ee\u9898\u8bf7\u4fdd\u7559 godot.log \u548c\u622a\u56fe\u3002"
  ],
  requirements: [
    "Slay the Spire 2 public beta v0.106.0\u3002",
    "BaseLib v3.1.4；当前包按这个版本构建，不写成自动兼容后续 BaseLib。",
    "\u4e0d\u8981\u628a\u65e7 EzDailyContent \u5f53\u6210\u5f53\u524d\u6a21\u7ec4\u542f\u7528\uff1b\u5f53\u524d\u6a21\u7ec4\u663e\u793a\u540d\u662f Spire Plus\u3002",
    "\u6e05\u5355\u7f16\u53f7\u548c\u5b89\u88c5\u76ee\u5f55\u4fdd\u6301 EZMicroBalance\uff0c\u8fd9\u662f\u517c\u5bb9\u6027\u8981\u6c42\u3002"
  ],
  assetPolicy: [
    "\u516c\u5f00\u7ad9\u70b9\u53ea\u5185\u7f6e\u672c\u4ed3\u5e93\u81ea\u6709\u6216\u751f\u6210\u56fe\u7247\uff0c\u4e3b\u8981\u6765\u81ea EZMicroBalance/images/\u3002",
    "\u539f\u7248\u6e38\u620f\u56fe\u7247\u53ef\u4ee5\u5728\u672c\u673a\u53c2\u8003\uff0c\u4f46\u4e0d\u80fd\u590d\u5236\u8fdb\u4ed3\u5e93\u6216 GitHub Pages \u53d1\u5e03\u5305\u3002",
    "本地预览会从 source code/images/ 读取原版遗物图标；公开 GitHub Pages 不内置这些原版图标。",
    "\u5982\u679c\u540e\u7eed\u9700\u8981\u5c55\u793a\u539f\u7248\u5b9e\u673a\u622a\u56fe\uff0c\u9700\u7531\u9879\u76ee\u6240\u6709\u8005\u786e\u8ba4\u6388\u6743\u8fb9\u754c\u540e\u5355\u72ec\u653e\u5165\u5141\u8bb8\u53d1\u5e03\u7684\u7d20\u6750\u76ee\u5f55\u3002"
  ],
  forum: {
    notice:
      "公开帖子请走 GitHub Issues / Discussions。本机草稿只保存在当前浏览器，用于整理反馈文本和截图，不代表已公开提交。",
    links: [
      ["GitHub \u4ed3\u5e93", "https://github.com/wenhuorongbing-netizen/dev-the-spire"],
      ["GitHub Issues", "https://github.com/wenhuorongbing-netizen/dev-the-spire/issues"],
      ["GitHub Discussions\uff08\u542f\u7528\u540e\u4f7f\u7528\uff09", "https://github.com/wenhuorongbing-netizen/dev-the-spire/discussions"]
    ]
  },
  updateGroups: [
    {
      short: "\u5956\u52b1",
      title: "\u73b0\u6709\u5148\u53e4\u5956\u52b1\u91cd\u6784",
      note: "\u5bf9\u5df2\u6709\u9057\u7269\u548c\u5956\u52b1\u505a\u884c\u4e3a\u8986\u76d6\u3002",
      icon: "assets/relics/relic.png",
      defaultVanilla: "\u539f\u7248\u9057\u7269\u884c\u4e3a\u3002",
      items: [
        baseRelic("\u5929\u9e45\u7ed2\u9879\u5708", "VELVET_CHOKER.description", ["\u80fd\u91cf", "\u8f6f\u9650\u5236"], "每回合开始获得1点能量；每回合最多打出6张牌。", "每回合开始获得1点能量；第7张及之后从手牌打出的牌费用+1，不再硬性禁止出牌。"),
        baseRelic("\u5353\u8d8a\u6597\u7bf7", "DISTINGUISHED_CAPE.description", ["\u6700\u5927\u751f\u547d", "\u7075\u4f53"], "拾起时失去9点最大生命；将3张灵体加入牌组。", "拾起时失去当前最大生命的30%，至少18点；将3张灵体加入牌组。"),
        baseRelic("\u68f1\u5f69\u5b9d\u77f3", "PRISMATIC_GEM.description", ["\u80fd\u91cf", "\u5f02\u8272\u724c"], "每回合开始获得1点能量；卡牌奖励会包含其他颜色卡牌。", "每回合开始获得1点能量；每第2次标准卡牌奖励只出现异色牌。"),
        baseRelic("\u73e0\u5b9d\u76d2", "JEWELRY_BOX.description", ["\u795e\u5316"], "拾起时将1张神化加入牌组。", "拾起时将1张神化加入牌组；这张神化没有固有。"),
        baseRelic("\u4fdd\u5b58\u4e4b\u96fe", "PRESERVED_FOG.description", ["\u5220\u724c"], "拾起时从牌组移除3张牌；将1张愚行加入牌组。", "拾起时从牌组移除4张牌；将1张愚行加入牌组。"),
        baseRelic("\u67af\u722a", "CLAWS.description", ["\u8bc5\u5492", "\u8bb8\u613f"], "拾起时将至多6张牌变化为撕咬。", "从4张诅咒中选择1张加入牌组；加入2张许愿和1张已升级的许愿+。"),
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
        baseRelic("\u8840\u67d3\u73ab\u7470", "BLOOD_SOAKED_ROSE.description", ["\u8bc5\u5492"], "遗物：拾起时将1张执迷加入牌组；每回合开始获得1点能量。执迷：2费诅咒，永恒；在手牌中时，必须先打出执迷。", "遗物本体仍为获得1点能量并加入1张执迷。执迷现在打出后获得10点格挡；仍会强制你先打出执迷。"),
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
        asc("BANNER_LAST_STAND", ["A16"], "assets/ascension/banner_room_indicator.png")
      ]
    },
    {
      short: "\u4ee4\u724c",
      title: "\u65b0\u589e\u724c\u3001\u72b6\u6001\u4e0e\u9884\u89c8\u5de5\u5177",
      note: "\u539f\u7248\u6ca1\u6709\u8fd9\u4e9b\u4e13\u7528\u4ee4\u724c\u548c\u9884\u89c8\u5de5\u5177\u3002",
      icon: "assets/card_portraits/rootblight_i.png",
      defaultVanilla: "\u539f\u7248\u65e0\u6b64\u65b0\u589e\u5185\u5bb9\u3002",
      items: [
        card("EZMB_URDA_SEEDLING", ["\u4e4c\u5c14\u59b2"], "assets/card_portraits/urda_seedling.png"),
        card("EZMB_URDA_SEEDBED", ["\u4e4c\u5c14\u59b2"], "assets/card_portraits/urda_seedling.png"),
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
        manual("\u6c34\u6676\u7403\u9884\u77e5", "\u539f\u7248\u65e0\u9884\u77e5\u6309\u94ae\u3002", "\u6c34\u6676\u7403\u5c0f\u6e38\u620f\u4e2d\u663e\u793a\u9884\u77e5\u6309\u94ae\uff1b\u53ea\u6539\u53d8\u906e\u7f69\u53ef\u89c1\u6027\uff0c\u4e0d\u53d1\u653e\u5956\u52b1\u3002", ["\u9884\u89c8\u5de5\u5177"]),
        manual("\u53d8\u6362\u771f\u5b9e\u9884\u89c8", "\u539f\u7248\u4e0d\u663e\u793a\u786e\u5b9a\u7ed3\u679c\u3002", "\u4f7f\u7528\u590d\u5236\u7684\u968f\u673a\u6570\u5feb\u7167\u9884\u6d4b\u53d8\u6362\u7ed3\u679c\uff1b\u4e0d\u521b\u5efa\u5361\u724c\uff0c\u4e0d\u63a8\u8fdb\u771f\u5b9e\u968f\u673a\u6570\u3002", ["\u9884\u89c8\u5de5\u5177"])
      ]
    }
  ],
  knownIssues: [
    ["\u963b\u585e", "\u5f53\u524d\u5305\u5b9e\u673a\u52a0\u8f7d\u4ecd\u5f85\u5237\u65b0", "\u5f53\u524d\u6e90\u7801\u5b9a\u4e49 25 \u4e2a SavedSpireFields\uff1b\u6700\u65b0\u5b9e\u673a\u52a0\u8f7d\u8bc1\u636e\u4ecd\u662f\u8f83\u65e9\u5305\uff0c\u4e0d\u80fd\u58f0\u660e\u5f53\u524d\u5305\u8fd0\u884c\u65f6\u7b49\u4ef7\u3002"],
    ["\u5f85\u9a8c\u8bc1", "\u5148\u53e4\u70b9\u51fb\u754c\u9762", "\u4e4c\u5c14\u59b2\u3001\u83ab\u5c14\u7ef4\u3001\u6d1b\u838e\u3001\u666e\u901a\u74e6\u5e93\u548c\u9690\u85cf\u74e6\u5e93\u6218\u6597\u754c\u9762\u622a\u56fe\u3001\u65e5\u5fd7\u3001\u60ac\u505c\u53ef\u8bfb\u6027\u4ecd\u5f85\u8865\u8bc1\u3002"],
    ["\u5f85\u9a8c\u8bc1", "\u74e6\u5e93\u8bd5\u70bc", "\u80dc\u5229\u8fd4\u56de\u7236\u4e8b\u4ef6\u3001\u65e0\u9ed1\u5c4f\u3001\u5931\u8d25/\u6b7b\u4ea1\u8def\u5f84\u3001\u5b58\u8bfb\u6863\u548c\u8054\u673a\u8fb9\u754c\u5747\u672a\u5b8c\u6210\u5b9e\u673a\u8bc1\u660e\u3002"],
    ["\u5f85\u9a8c\u8bc1", "\u8fdb\u9636 11-20", "A11 \u81ea\u7136\u8def\u7ebf\u70b9\u51fb\u3001A12/A16/A19/A20 \u6218\u6597\u884c\u4e3a\u3001\u6839\u8680\u6218\u540e\u8868\u73b0\u3001\u5b58\u8bfb\u6863\u3001\u53cc\u4eba\u8054\u673a\u4ecd\u5f85\u9a8c\u8bc1\u3002"],
    ["\u5f85\u9a8c\u8bc1", "\u516c\u5f00\u8bba\u575b", "GitHub Pages \u9759\u6001\u7ad9\u4e0d\u80fd\u5355\u72ec\u63d0\u4f9b\u771f\u6b63\u7684\u516c\u5171\u533f\u540d\u8bba\u575b\uff1b\u9700\u8981\u63a5\u5165 Discussions\u3001Giscus \u6216\u72ec\u7acb\u540e\u7aef\u3002"],
    ["\u8fb9\u754c", "\u539f\u7248\u6e38\u620f\u56fe\u7247", "\u516c\u5f00\u4ed3\u5e93\u4e0d\u80fd\u590d\u5236\u539f\u7248\u6e38\u620f\u7d20\u6750\u3002\u5f53\u524d\u7f51\u9875\u4f7f\u7528\u672c\u6a21\u7ec4\u81ea\u6709\u56fe\u7247\uff1b\u539f\u7248\u56fe\u53ea\u80fd\u4f5c\u4e3a\u672c\u5730\u53c2\u8003\u6216\u7ecf\u6388\u6743\u540e\u53e6\u884c\u5904\u7406\u3002"]
  ],
  changeLog: [
    ["2026-05-22 \u00b7 \u7f51\u7ad9\u91cd\u6784", "\u7ad9\u70b9\u6539\u4e3a\u56db\u9875\u73a9\u5bb6\u7ed3\u6784\uff1a\u66f4\u65b0\u5bf9\u6bd4\u3001\u4e0b\u8f7d\u6559\u5b66\u3001\u8bba\u575b\u5165\u53e3\u3001\u5df2\u77e5\u95ee\u9898/\u66f4\u65b0\u8bb0\u5f55\u3002"],
    ["\u5f53\u524d\u5305", "SpirePlus-v0.1.0-private-beta.0.zip\uff0c\u6e05\u5355\u7f16\u53f7 EZMicroBalance\uff0c\u663e\u793a\u540d Spire Plus\u3002"],
    ["\u5148\u53e4\u5185\u5bb9", "\u4e4c\u5c14\u59b2\u3001\u83ab\u5c14\u7ef4\u3001\u6d1b\u838e\u9ed8\u8ba4\u4f5c\u4e3a\u65b0\u589e\u5148\u53e4\u5185\u5bb9\uff1b\u74e6\u5e93\u8bd5\u70bc\u4fdd\u6301\u9690\u85cf\u95e8\u63a7\u6d4b\u8bd5\u3002"],
    ["\u8fdb\u9636\u5185\u5bb9", "A11-A20 \u4e3a\u5f53\u524d\u5f00\u53d1\u6d4b\u8bd5\u5019\u9009\uff0c\u5355\u4eba\u4e0e\u623f\u4e3b\u591a\u4eba\u9009\u62e9\u9762\u5df2\u6253\u5f00\uff0c\u8054\u673a\u5b8c\u6574\u73a9\u6cd5\u672a\u9a8c\u8bc1\u3002"],
    ["\u9884\u89c8\u5de5\u5177", "\u6c34\u6676\u7403\u9884\u77e5\u548c\u53d8\u6362\u771f\u5b9e\u9884\u89c8\u5df2\u5408\u5e76\u8fdb Spire Plus\uff0c\u4e0d\u518d\u4f5c\u4e3a\u72ec\u7acb Future Peek \u53d1\u5e03\u3002"]
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
  const isLocalPreview = ["", "localhost", "127.0.0.1", "::1"].includes(window.location.hostname);
  if (!isLocalPreview) return undefined;
  const fileName = sourceRelicIcons[descKey];
  return fileName ? `../source%20code/images/relics/${fileName}` : undefined;
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
    icon,
    tags
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
      brandSub: "Balance and content expansion for Slay the Spire 2",
      navUpdates: "Updates",
      navInstall: "Download & Install",
      navForum: "Forum",
      navIssues: "Known Issues",
      releaseLine: "Current manual test package · v0.1.0-private-beta.0",
      heroCopy: "Player-facing change log. The first page lists vanilla behavior against current effects.",
      introTitle: "Mod Scope",
      introCopy: "Spire Plus is a balance and content expansion for Slay the Spire 2. The current package focuses on Ancient rewards, Ascension 11-20, token cards, and preview tools: stronger rewards, clearer costs, route pressure, and combat decisions.",
      download: "Download Mod",
      viewIssues: "Known Issues",
      all: "All",
      search: "Search",
      searchPlaceholder: "Relic, Ancient, Ascension, keyword",
      vanilla: "Vanilla",
      current: "Current",
      installTitle: "Download & Install",
      installLead: "The current download target is the latest manual test package. On the public site, direct downloads require the matching ZIP to be uploaded to GitHub Releases.",
      currentDownload: "Current Download",
      directDownload: "Direct Download",
      openRelease: "Open Releases",
      openBaseLib: "Download BaseLib",
      openRepo: "Open Repository",
      steps: "Install Steps",
      requirements: "Requirements",
      assetPolicy: "Image and Asset Policy",
      forumTitle: "Forum",
      forumLead: "Public posts, feedback, and replies should use GitHub Issues / Discussions. The local area is only a feedback template and draft space; it does not sync to other players.",
      forumPublicTitle: "Public Discussion",
      forumDraftTitle: "Local Feedback Template",
      postName: "Name",
      postNamePlaceholder: "Name, optional",
      anonymous: "Anonymous player",
      postTitle: "Title",
      postBody: "Body",
      postTitlePlaceholder: "Enter title",
      postBodyPlaceholder: "Local anonymous draft. It is stored only in this browser.",
      replyPlaceholder: "Write a reply",
      replySubmit: "Reply",
      postSubmit: "Save Draft",
      postClear: "Clear Drafts",
      noPosts: "No local drafts.",
      issuesTitle: "Known Issues & Changelog",
      issuesLead: "Blocking and unverified items come first, followed by completed changes. Pending verification is not a release-ready claim.",
      knownIssues: "Known Issues",
      changeLog: "Changelog",
      noTitle: "Untitled",
      separator: " · ",
      issueSeparator: " · "
    },
    summary: [
      ["Updates", "Vanilla vs current effects", "updates"],
      ["Install", "Package, dependency, install path", "install"],
      ["Forum", "Feedback and discussion links", "forum"],
      ["Known Issues", "Blockers and changelog", "issues"]
    ],
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
        ["Install folder", "EZMicroBalance"],
        ["Dependency", "BaseLib v3.1.4"],
        ["Game version", "Slay the Spire 2 v0.106.0"],
        ["Size", "15,228,395 bytes"],
        ["Hash", "B235DEA5219FFAB46905CE076664C0D3F2E7DF80B4B5C289AA9B1985525E942D"]
      ]
    },
    installSteps: [
      "Download SpirePlus-v0.1.0-private-beta.0.zip.",
      "Download BaseLib v3.1.4; the release page should show BaseLib-StS2 v3.1.4.",
      "Common Windows path: Steam\\steamapps\\common\\Slay the Spire 2.",
      "After extraction, confirm the root folder is EZMicroBalance.",
      "Final mod path: mods\\EZMicroBalance\\EZMicroBalance.json.",
      "Final BaseLib path: mods\\BaseLib\\BaseLib.json.",
      "Start the game and enable Spire Plus in the mod list.",
      "This test package still needs live verification. Keep godot.log and screenshots when reporting issues."
    ],
    requirements: [
      "Slay the Spire 2 public beta v0.106.0.",
      "BaseLib v3.1.4. The package is built for this version; do not present it as automatically compatible with later BaseLib versions.",
      "Do not enable the old EzDailyContent scaffold as the current mod. The player-facing mod name is Spire Plus.",
      "The manifest id and install folder remain EZMicroBalance for compatibility."
    ],
    assetPolicy: [
      "The public site bundles only repo-owned or generated images, mainly from EZMicroBalance/images/.",
      "Original game images may be referenced locally, but they must not be copied into the repository or GitHub Pages package.",
      "Local preview may read original relic icons from source code/images/. Public GitHub Pages does not include those original icons.",
      "If public screenshots from the base game are needed later, asset permission should be confirmed and handled separately."
    ],
    forum: {
      notice: "Use GitHub Issues / Discussions for public posts. Local drafts stay in the current browser and are only for preparing feedback text and screenshots.",
      links: [
        ["GitHub Repository", "https://github.com/wenhuorongbing-netizen/dev-the-spire"],
        ["GitHub Issues", "https://github.com/wenhuorongbing-netizen/dev-the-spire/issues"],
        ["GitHub Discussions (after enabling)", "https://github.com/wenhuorongbing-netizen/dev-the-spire/discussions"]
      ]
    },
    updateGroups: [
      {
        short: "Rewards",
        title: "Existing Ancient Reward Rework",
        note: "Behavior overrides for existing relics and rewards.",
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
        note: "Vanilla currently ends at A10; this package adds A11-A20 test content.",
        defaultVanilla: "No equivalent vanilla content."
      },
      {
        short: "Tokens",
        title: "New Cards, Statuses, and Preview Tools",
        note: "Dedicated token cards and preview tools added by the mod.",
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
      "令牌": "Token",
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
      "CLAWS.description": {
        title: "Claws",
        vanilla: "On pickup, transform up to 6 cards into Mauls.",
        desc: "Choose 1 of 4 Curses to add to your deck. Add 2 Wishes and 1 upgraded Wish+."
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
        desc: "Relic body remains +1 Energy and 1 Enthralled. Enthralled now gains 10 Block when played; it still forces you to play Enthralled first."
      },
      "BRIGHTEST_FLAME.description": {
        title: "Quality Flame",
        vanilla: "Gain 2 Energy, draw 2 cards, and lose 1 Max HP; upgraded gains 3 Energy and draws 3 cards.",
        desc: "Adds Exhaust. Gain 2 Energy, draw 3 cards, and lose 1 Max HP; upgraded gains 3 Energy and draws 4 cards."
      },
      "EZMB_URDA_SEEDLING.description": {
        desc: "0-cost Skill. Exhaust. Gain 4 Block; upgraded gains 7 Block."
      },
      "EZMB_URDA_SEEDBED.description": {
        desc: "1-cost Skill. Exhaust. Gain 4 Block and set up a 2-space Seedbed. Blight Sprout, Rootblight, Status, or Curse cards that enter hand are planted first; each planted card adds 1 Withered Husk. Upgraded: gain 6 Block and capacity becomes 3."
      },
      "EZMB_WITHERED_HUSK.description": {
        desc: "Curse. Ethereal, Exhaust. When exhausted, gain 3 Block."
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
        desc: "0-cost Contract. Ethereal, Exhaust. Deal 22 damage to Vakuu and lose 4 HP. If any Stolen Lock remains, break 1 and add 1 Blood Debt."
      },
      "EZMB_VAKUU_TEMPTATION.description": {
        desc: "0-cost Contract. Ethereal, Exhaust. Gain 2 Energy, draw 2 cards, and lose 5 HP. If any Stolen Lock remains, break 1 and add 1 Blood Debt."
      },
      "EZMB_VAKUU_SHELTER_CONTRACT.description": {
        desc: "0-cost Contract. Ethereal, Exhaust. Gain 24 Block and lose 3 HP. If any Stolen Lock remains, break 1 and add 1 Blood Debt."
      },
      "FIREMARK_MIGHT.description": {
        desc: "Firemarked Elites start with Strength. Act 1/2/3 values are 1/2/4. Unblocked attack damage builds Heat; at 2 Heat, the next attack is more dangerous."
      },
      "FIREMARK_GIANT.description": {
        desc: "Firemarked Elites have increased Max HP. Act 1/2/3 values are +20%/+30%/+45%. At half HP, they expose a Molten Core window."
      },
      "FIREMARK_FORGE_ARMOR.description": {
        desc: "Firemarked Elites gain Molten Armor after each enemy turn. Act 1/2/3 values are 8/14/24. Breaking that armor once skips the next armor gain."
      },
      "FIREMARK_CONSTANT_HEAL.description": {
        desc: "Firemarked Elites heal after each enemy turn. Act 1/2/3 values are 4/8/16 HP. Dealing 20/40/80 damage in the round stops the heal."
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
      ["Blocker", "Current package live-load evidence is stale", "Current source defines 25 SavedSpireFields. The newest live loader evidence still comes from an earlier package, so current-package runtime parity cannot be claimed yet."],
      ["Needs verification", "Ancient click UI", "Urda, Morvi, Lotha, normal Vakuu, and hidden Vakuu fight UI still need screenshots, logs, and hover readability proof."],
      ["Needs verification", "Vakuu trial", "Victory return to parent event, no black screen, failure/death path, save/load, and multiplayer boundaries still need live proof."],
      ["Needs verification", "Ascension 11-20", "A11 natural route clicks, A12/A16/A19/A20 combat behavior, Rootblight post-combat behavior, save/load, and two-player co-op still need verification."],
      ["Needs verification", "Public forum", "A static GitHub Pages site cannot provide a true public anonymous forum by itself. Discussions, Giscus, or a separate backend is required."],
      ["Boundary", "Original game images", "Public repositories must not copy original game assets. The site uses mod-owned images; original game images are local references only unless separately authorized."]
    ],
    changeLog: [
      ["2026-05-22 · Website rebuild", "Site structure changed to four player pages: update comparison, install guide, forum entry, known issues and changelog."],
      ["Current package", "SpirePlus-v0.1.0-private-beta.0.zip, manifest id EZMicroBalance, display name Spire Plus."],
      ["Ancient content", "Urda, Morvi, and Lotha are default-on new Ancient content. Vakuu trial remains hidden behind test gates."],
      ["Ascension content", "A11-A20 is a current development test candidate. Single-player and host multiplayer selection are open; full co-op gameplay is not verified."],
      ["Preview tools", "Crystal Sphere peek and deterministic transform preview are merged into Spire Plus and are no longer shipped as a separate Future Peek package."]
    ]
  }
};

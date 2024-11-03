using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// 定数データベース
/// </summary>
public class GameDatabase
{
    #region パラメータデータベース

    /// <summary>
    /// パラメータ構造体
    /// </summary>
    public struct ParameterData
    {
        public int maxHp;
        public int atk;
        public int mag;
        public int tec;
        public int spd;
        public int luk;
        public int def;
        public int mdf;
        public int move;

        public ParameterData(int mh, int a, int m, int t, int s, int l, int d, int md, int mv)
        {
            maxHp = mh; atk = a; mag = m; tec = t; spd = s; luk = l; def = d; mdf = md; move = mv;
        }
    }

    /// <summary>プレイヤー初期値</summary>
    public static readonly ParameterData[] Prm_PlayerInit = new ParameterData[] {
        new ParameterData(25, 11, 25, 8, 8, 9, 3, 1, 4),
        new ParameterData(26, 7, 1, 5, 3, 2, 10, 5, 3),
        new ParameterData(21, 9, 3, 9, 8, 2, 11, 2, 4),
        new ParameterData(19, 6, 5, 10, 12, 1, 4, 2, 4),
        new ParameterData(16, 1, 10, 6, 5, 7, 2, 9, 4),
        new ParameterData(20, 7, 5, 11, 10, 5, 6, 4, 4),
    };

    /// <summary>プレイヤー成長率％</summary>
    public static readonly ParameterData[] Prm_PlayerGrow = new ParameterData[] {
        new ParameterData(70, 130, 0, 45, 40, 40, 30, 10, 0),
        new ParameterData(100, 45, 5, 35, 20, 30, 60, 40, 0),
        new ParameterData(85, 50, 10, 55, 45, 20, 40, 25, 0),
        new ParameterData(60, 40, 20, 50, 65, 40, 20, 40, 0),
        new ParameterData(50, 15, 70, 40, 30, 60, 15, 55, 0),
        new ParameterData(65, 55, 25, 50, 50, 45, 25, 30, 0),
    };

    /// <summary>
    /// プレイヤー成長判定結果を取得
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public static ParameterData Prm_PlayerGrow_GetCalced(Constant.PlayerID playerID)
    {
        var ret = new ParameterData();
        var rate = Prm_PlayerGrow[(int)playerID];
        ret.maxHp = Prm_CalcGrowUp(rate.maxHp);
        ret.atk = Prm_CalcGrowUp(rate.atk);
        ret.mag = Prm_CalcGrowUp(rate.mag);
        ret.tec = Prm_CalcGrowUp(rate.tec);
        ret.spd = Prm_CalcGrowUp(rate.spd);
        ret.luk = Prm_CalcGrowUp(rate.luk);
        ret.def = Prm_CalcGrowUp(rate.def);
        ret.mdf = Prm_CalcGrowUp(rate.mdf);

        return ret;
    }

    /// <summary>
    /// 上昇判定
    /// </summary>
    /// <param name="rate"></param>
    /// <returns></returns>
    private static int Prm_CalcGrowUp(int rate)
    {
        // 100以上は必ず上がる
        var upBase = rate / 100;
        rate = rate % 100;
        if (rate <= 0) return upBase;

        return upBase + (Util.RandomCheck(rate) ? 1 : 0);
    }

    /// <summary>クラスチェンジ成長値_ドロシー</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Drows = new ParameterData[] {
        new ParameterData(),
        new ParameterData(1, 2, 0, 3, 4, 1, 1, 7, 1),
        new ParameterData(3, 6, 0, 2, 2, 0, 2, 3, 1),
        new ParameterData(2, 4, 0, 2, 7, 0, 2, 2, 1),
        new ParameterData(4, 2, 0, 4, 2, 0, 4, 3, 0),
        new ParameterData(6, 7, 0, 0, 3, 0, 4, 4, 2),
    };
    /// <summary>クラスチェンジ成長値_エラ</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Eraps = new ParameterData[] {
        new ParameterData(),
        new ParameterData(2, 7, 0, 6, 3, 0, 0, 1, 0),
        new ParameterData(0, 8, 0, 6, 7, 0, -3, 0, 2),
        new ParameterData(3, 3, 0, 4, 1, 0, 2, 6, 1),
        new ParameterData(1, 5, 0, 2, 2, 3, 1, 5, 1),
        new ParameterData(0, 4, 0, 4, 5, 0, 1, 4, 2),
    };
    /// <summary>クラスチェンジ成長値_エグザ</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Exa = new ParameterData[] {
        new ParameterData(),
        new ParameterData(2, 1, 0, 2, 2, 4, 1, 6, 1),
        new ParameterData(5, 4, 0, 1, 3, 1, 3, 1, 1),
        new ParameterData(3, 1, 0, 3, 3, 3, 3, 3, 2),
        new ParameterData(6, 2, 0, 4, 1, 0, 2, 5, 1),
        new ParameterData(3, 5, 0, 1, 4, 0, 4, 2, 1),
    };
    /// <summary>クラスチェンジ成長値_ウーラ</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Worra = new ParameterData[] {
        new ParameterData(),
        new ParameterData(3, 3, 0, 5, 1, 0, 5, 1, 1),
        new ParameterData(5, 1, 0, 3, 4, 0, 3, 2, 1),
        new ParameterData(1, 4, 3, 6, 0, 2, 4, 2, 1),
        new ParameterData(3, 5, 1, 4, 2, 0, 1, 3, 1),
        new ParameterData(1, 7, 0, 5, 6, -2, 1, 1, 2),
    };
    /// <summary>クラスチェンジ成長値_クー</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Koob = new ParameterData[] {
        new ParameterData(),
        new ParameterData(5, 0, 2, 3, 2, 0, 2, 4, 1),
        new ParameterData(2, 0, 6, 2, 1, 0, 5, 2, 1),
        new ParameterData(2, 0, 6, 4, 2, 0, 2, 3, 1),
        new ParameterData(3, 0, 3, 5, 4, 0, 2, 2, 1),
        new ParameterData(4, 0, 2, 1, 4, 0, 5, 3, 1),
    };
    /// <summary>クラスチェンジ成長値_悠</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_You = new ParameterData[] {
        new ParameterData(),
        new ParameterData(2, 5, 0, 4, 1, 0, 4, 2, 1),
        new ParameterData(3, 4, 0, 2, 4, 0, 1, 4, 1),
        new ParameterData(0, 3, 0, 3, 5, 0, 3, 5, 1),
        new ParameterData(2, 2, 0, 6, 6, 0, 1, 2, 1),
        new ParameterData(4, 7, 0, 4, 0, 0, 1, 3, 1),
    };

    /// <summary>クラスチェンジ成長値取得</summary>
    public static ParameterData Prm_ClassChangeGrow_Get(Constant.PlayerID playerID, Constant.ClassID classID)
    {
        var lst = playerID switch
        {
            Constant.PlayerID.Drows => Prm_ClassChangeGrow_Drows,
            Constant.PlayerID.Eraps => Prm_ClassChangeGrow_Eraps,
            Constant.PlayerID.Exa => Prm_ClassChangeGrow_Exa,
            Constant.PlayerID.Worra => Prm_ClassChangeGrow_Worra,
            Constant.PlayerID.Koob => Prm_ClassChangeGrow_Koob,
            _ => Prm_ClassChangeGrow_You,
        };

        return lst[(int)classID];
    }

    #endregion

    #region クラスごとの武器倍率

    /// <summary>
    /// 武器の倍率
    /// </summary>
    public struct WeaponRate
    {
        public int sword;
        public int spear;
        public int axe;
        public int arrow;
        public int book;
        public int rod;
        public WeaponRate(int _sword, int _spear, int _axe, int _arrow, int _book, int _rod)
        {
            sword = _sword; spear = _spear; axe = _axe; arrow = _arrow; book = _book; rod = _rod;
        }

        public int Get(ItemType it)
        {
            return it switch
            {
                ItemType.Sword => sword,
                ItemType.Spear => spear,
                ItemType.Axe => axe,
                ItemType.Arrow => arrow,
                ItemType.Book => book,
                ItemType.Rod => rod,
                _ => 100,
            };
        }
    }

    /// <summary>クラス毎武器倍率_ドロシー</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Drows = new WeaponRate[]
    {
        new (100, 30, 30, 10, 10, 10),
        new (120, 30, 30, 10, 10, 10),
        new (100, 50, 100, 30, 10, 10),
        new (150, 80, 80, 10, 10, 10),
        new (120, 120, 120, 50, 10, 10),
        new (200, 0, 0, 0, 0, 0),
    };
    /// <summary>クラス毎武器倍率_エラ</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Eraps = new WeaponRate[]
    {
        new (20, 100, 20, 20, 10, 50),
        new (50, 100, 50, 20, 10, 50),
        new (20, 120, 20, 20, 10, 50),
        new (100, 100, 100, 50, 10, 50),
        new (100, 120, 50, 50, 10, 100),
        new (50, 150, 50, 20, 10, 50),
    };
    /// <summary>クラス毎武器倍率_エグザ</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Exa = new WeaponRate[]
    {
        new (10, 50, 100, 10, 30, 30),
        new (10, 50, 100, 10, 30, 30),
        new (10, 50, 120, 10, 30, 30),
        new (50, 100, 100, 50, 30, 50),
        new (100, 50, 100, 10, 30, 30),
        new (10, 50, 150, 10, 30, 30),
    };
    /// <summary>クラス毎武器倍率_ウーラ</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Worra = new WeaponRate[]
    {
        new (10, 10, 10, 100, 10, 50),
        new (10, 10, 10, 120, 30, 50),
        new (10, 10, 10, 100, 30, 100),
        new (10, 10, 10, 150, 30, 50),
        new (50, 10, 10, 120, 30, 100),
        new (100, 10, 10, 100, 30, 100),
    };
    /// <summary>クラス毎武器倍率_クー</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Koob = new WeaponRate[]
    {
        new (10, 10, 10, 10, 100, 100),
        new (10, 10, 10, 10, 130, 100),
        new (10, 10, 10, 10, 100, 130),
        new (10, 10, 10, 10, 150, 100),
        new (10, 10, 10, 10, 130, 130),
        new (10, 10, 10, 10, 100, 150),
    };
    /// <summary>クラス毎武器倍率_悠</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_You = new WeaponRate[]
    {
        new (110, 10, 10, 10, 50, 50),
        new (130, 10, 10, 10, 50, 50),
        new (120, 10, 10, 10, 50, 50),
        new (150, 10, 10, 10, 50, 50),
        new (120, 120, 10, 10, 50, 50),
        new (130, 10, 100, 10, 50, 50),
    };

    /// <summary>クラスチェンジ成長値取得</summary>
    public static WeaponRate Prm_ClassWeaponRate_Get(Constant.PlayerID playerID, Constant.ClassID classID)
    {
        var lst = playerID switch
        {
            Constant.PlayerID.Drows => Prm_ClassWeaponRates_Drows,
            Constant.PlayerID.Eraps => Prm_ClassWeaponRates_Eraps,
            Constant.PlayerID.Exa => Prm_ClassWeaponRates_Exa,
            Constant.PlayerID.Worra => Prm_ClassWeaponRates_Worra,
            Constant.PlayerID.Koob => Prm_ClassWeaponRates_Koob,
            _ => Prm_ClassWeaponRates_You,
        };

        return lst[(int)classID];
    }

    #endregion

    #region 敵
    /// <summary>敵Lv1</summary>
    public static readonly ParameterData[] Prm_EnemyInit = new ParameterData[]
    {
        new ParameterData(16, 4, 0, 2, 1, 4, 5, 0, 4), //グリーンスライム
    };

    /// <summary>敵成長率％</summary>
    public static readonly ParameterData[] Prm_EnemyGrow = new ParameterData[]
    {
        new ParameterData(50, 30, 1, 45, 35, 40, 25, 10, 0), //グリーンスライム
    };

    #endregion

    #region 名称データベース

    // 他の場所にあるテキスト
    // を手に入れた
    // が壊れた

    /// <summary>プレイヤー名</summary>
    public static readonly string[] Name_Players =
    {
        "ドロシー",
        "エラ",
        "エグザ",
        "ウーラ",
        "クー",
        "悠",
    };

    /// <summary>クラス名_ドロシー</summary>
    public static readonly string[] Name_Classes_Drows = {
        "遊牧民",
        "ソードメイデン",
        "ストライカー",
        "トルネイド",
        "ヴァイキング",
        "バーサーカー",
    };
    /// <summary>クラス名_エラ</summary>
    public static readonly string[] Name_Classes_Eraps = {
        "ナイト",
        "ヘビーナイト",
        "ランサー",
        "ロイヤルガード",
        "パラディン",
        "ドラグーン",
    };
    /// <summary>クラス名_エグザ</summary>
    public static readonly string[] Name_Classes_Exa = {
        "山賊",
        "ワイルドガード",
        "頭領",
        "アークナイト",
        "コマンダー",
        "ヘラクレス",
    };
    /// <summary>クラス名_ウーラ</summary>
    public static readonly string[] Name_Classes_Worra = {
        "ハンター",
        "スナイパー",
        "スカウト",
        "アルテミス",
        "スターゲイザー",
        "アサシン",
    };
    /// <summary>クラス名_クー</summary>
    public static readonly string[] Name_Classes_Koob = {
        "魔道士",
        "プリーステス",
        "ドルイド",
        "プライム",
        "賢者",
        "ファナティック",
    };
    /// <summary>クラス名_悠</summary>
    public static readonly string[] Name_Classes_You = {
        "浪人",
        "侍",
        "辻斬り",
        "夜叉姫",
        "刺客",
        "修羅",
    };

    /// <summary>
    /// クラス名配列取得
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public static string[] Name_Classes_Get(Constant.PlayerID pid)
    {
        return pid switch
        {
            Constant.PlayerID.Drows => Name_Classes_Drows,
            Constant.PlayerID.Eraps => Name_Classes_Eraps,
            Constant.PlayerID.Exa => Name_Classes_Exa,
            Constant.PlayerID.Worra => Name_Classes_Worra,
            Constant.PlayerID.Koob => Name_Classes_Koob,
            _ => Name_Classes_You,
        };
    }

    /// <summary>スキル名</summary>
    public static readonly string[] Name_Skills =
    {

    };

    /// <summary>敵名</summary>
    public static readonly string[] Name_Enemies =
    {
        "グリーンスライム",
        "スケルトン",
    };

    /// <summary>
    /// 敵モデルの色
    /// </summary>
    /// <param name="eid"></param>
    /// <returns></returns>
    public static Color GetEnemyColor(Constant.EnemyID eid)
    {
        return eid switch
        {
            Constant.EnemyID.GreenSlime => new Color(0.3f, 1f, 0.3f), //グリーンスライム
            _ => Color.white, // ほとんどはそのまま
        };
    }

    #endregion

    #region アイテムデータベース

    /// <summary>
    /// アイテム種別
    /// </summary>
    public enum ItemType
    {
        None = 0,
        Sword,
        Spear,
        Axe,
        Arrow,
        Book,
        Rod,
        Item,
    }

    /// <summary>
    /// アイテムパラメータ構造体
    /// </summary>
    public struct ItemData
    {
        public ItemType iType;
        public string name;
        public int maxUse;
        public int atk;
        public int hit;
        public int rangeMin;
        public int rangeMax;
        public int critical;
        public int rarelity;
        public ItemData(ItemType it, string nm, int rare, int use, int a, int hi, int rMin, int rMax, int crt = 0)
        {
            iType = it; name = nm; maxUse = use;
            atk = a; hit = hi; rangeMin = rMin; rangeMax = rMax; critical = crt;
            rarelity = rare;
        }

        /// <summary>
        /// 物理武器判定
        /// </summary>
        /// <returns></returns>
        public bool is_melee()
        {
            return iType != ItemType.Book;
        }
    }

    /// <summary>アイテムID</summary>
    public enum ItemID
    {
        FreeHand = 0,
        FreeMagic,
        Rod1,
        Item1,
        Sword1,
        Spear1,
        Axe1,
        Arrow1,
        Book1,
        Sword2,
        Spear2,
        Axe2,
        Arrow2,
        Book2,
        Sword3,
        Spear3,
        Axe3,
        Arrow3,
        Book3,
        Sword_Ex1,
        Spear_Ex1,
        Axe_Ex1,
        Arrow_Ex1,
        Book_Ex1,
        Sword_Ex2,
        Spear_Ex2,
        Axe_Ex2,
        Arrow_Ex2,
        Book_Ex2,

        ITEM_COUNT,
    }

    /// <summary>
    /// アイテムデータ
    /// </summary>
    public static readonly ItemData[] ItemDataList = new ItemData[]
    {
        new(ItemType.None, "素手", -1, -1, 0, 100, 1, 1),
        new(ItemType.None, "素手（魔）", -1, -1, 0, 100, 1, 1),

        new(ItemType.Rod, "リフの杖",      50, 30, 10, 100, 1, 1),
        new(ItemType.Item, "エリクサー",   50, 3, 100, 100, 0, 0),

        new(ItemType.Sword, "鉄の剣",       1, 40, 5, 85, 1, 1),
        new(ItemType.Spear, "鉄の槍",       1, 40, 7, 70, 1, 1),
        new(ItemType.Axe,   "鉄の斧",       1, 40, 8, 65, 1, 1),
        new(ItemType.Arrow, "鉄の弓",       1, 40, 6, 80, 2, 3),
        new(ItemType.Book,  "ファイアー",   1, 40, 5, 95, 1, 2),

        new(ItemType.Sword, "銀の剣",       20, 20, 13, 75, 1, 1),
        new(ItemType.Spear, "銀の槍",       20, 20, 14, 65, 1, 1),
        new(ItemType.Axe, "銀の斧",         20, 20, 15, 55, 1, 1),
        new(ItemType.Arrow, "銀の弓",       20, 20, 13, 70, 2, 3),
        new(ItemType.Book, "ブリザード",    20, 15, 13, 80, 1, 2),

        new(ItemType.Sword, "キルソード",   30, 20, 9, 80, 1, 1, 30),
        new(ItemType.Spear, "キラーランス", 30, 20, 10, 75, 1, 1, 30),
        new(ItemType.Axe, "キラーアクス",   30, 20, 11, 65, 1, 1, 30),
        new(ItemType.Arrow, "キラーボウ",   30, 20, 9, 80, 2, 3, 30),
        new(ItemType.Book, "キルグリム",    30, 20, 8, 70, 1, 2, 30),

        new(ItemType.Sword, "ルキフグス",       100, 30, 23, 80, 1, 2, 15),
        new(ItemType.Spear, "アガリアレプト",   100, 30, 18, 100, 1, 2, 15),
        new(ItemType.Axe, "サタナキア",         100, 30, 19, 90, 1, 2, 15),
        new(ItemType.Arrow, "フルーレティ",     100, 30, 18, 85, 2, 4, 15),
        new(ItemType.Book, "アッピンの赤い本",  100, 30, 21, 95, 1, 3, 15),

        new(ItemType.Sword, "妙法村正",         200, 30, 16, 120, 1, 1, 60),
        new(ItemType.Spear, "ディスラプター",   200, 30, 27, 65, 1, 3, 0),
        new(ItemType.Axe, "シルヴァームーン",   200, 30, 22, 100, 1, 1, 20),
        new(ItemType.Arrow, "マルドゥーク",     200, 30, 15, 100, 2, 6, 0),
        new(ItemType.Book, "ネクロノミコン",    200, 30, 36, 40, 1, 2, 0),



        new(ItemType.None, "素手", -1, -1, 0, 100, 1, 1),
    };

    /// <summary>
    /// 武器相性チェック
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    /// <returns>1:攻撃側有利　-1:防御側有利　0:相性なし</returns>
    public static int CheckWeaponAdvantage(ItemID item1, ItemID item2)
    {
        var data1 = ItemDataList[(int)item1];
        var data2 = ItemDataList[(int)item2];

        if (data1.iType == ItemType.Sword)
        {
            if (data2.iType == ItemType.Spear) return -1;
            if (data2.iType == ItemType.Axe) return 1;
        }
        else if (data1.iType == ItemType.Spear)
        {
            if (data2.iType == ItemType.Sword) return 1;
            if (data2.iType == ItemType.Axe) return -1;
        }
        else if (data1.iType == ItemType.Axe)
        {
            if (data2.iType == ItemType.Sword) return -1;
            if (data2.iType == ItemType.Spear) return 1;
        }

        return 0;
    }

    /// <summary>
    /// ランダムアイテム生成
    /// </summary>
    /// <param name="lv">持つ敵のレベル</param>
    /// <param name="boss">ボスである</param>
    /// <param name="weaponOnly">true:武器のみ</param>
    /// <param name="type">None以外を指定するとタイプを指定</param>
    /// <param name="outRate">ハズレになる確率0〜100</param>
    /// <returns></returns>
    public static ItemID CalcRandomItem(int lv, bool boss, bool weaponOnly = true, ItemType type = ItemType.None, int outRate = 0)
    {
        var koho = new List<ItemID>();
        var maxRarelity = 0;
        var bossRate = boss ? 3 : 1; // ボスはレアリティ下がる＝レア高いものが出やすい

        for (var i = 0; i < (int)ItemID.ITEM_COUNT; ++i)
        {
            var data = ItemDataList[i];
            if (data.iType == ItemType.None) continue;
            if (type != ItemType.None && data.iType != type) continue;
            if (weaponOnly &&
                (data.iType == ItemType.Rod ||
                data.iType == ItemType.Item ||
                data.iType == ItemType.None)) continue;

            // rarelityレベルまでは出現しない
            var rareLine = lv + (boss ? 20 : 0);
            if (data.rarelity / bossRate > rareLine) continue;

            if (maxRarelity < data.rarelity / bossRate) maxRarelity = data.rarelity / bossRate;
            koho.Add((ItemID)i);
        }

        if (koho.Count == 0) return ItemID.FreeHand;

        // 候補からrarelityによって確率で判定
        // 最大値との差+1＝選ばれやすさ　最大値のものは1になる
        var selectRate = new Func<ItemID, int>(id =>
            maxRarelity - ItemDataList[(int)id].rarelity / bossRate + 1
            );
        // 全部の選ばれやすさを足す
        var totalAdd = koho.Sum(id => selectRate(id));
        var checkNum = Util.RandomInt(0, totalAdd);
        foreach (var id in koho)
        {
            // ランダム値が選ばれやすさより小さければ選択
            var rate = selectRate(id);
            if (checkNum < rate) return id;
            checkNum -= rate;
        }

        return ItemID.FreeHand;
    }

    #endregion
}

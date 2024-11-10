using System.Collections.Generic;
using System.Linq;
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
        new ParameterData(70, 130, 0, 45, 40, 50, 30, 10, 0),
        new ParameterData(100, 45, 25, 40, 20, 30, 60, 45, 0),
        new ParameterData(80, 60, 20, 55, 40, 20, 35, 20, 0),
        new ParameterData(60, 40, 25, 50, 65, 40, 25, 40, 0),
        new ParameterData(50, 15, 60, 40, 30, 60, 15, 50, 0),
        new ParameterData(65, 45, 30, 45, 55, 45, 30, 30, 0),
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
        new ParameterData(1, 3, 0, 2, 4, 3, 1, 5, 1),
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
        new ParameterData(1, 3, 3, 6, 0, 2, 3, 2, 1),
        new ParameterData(3, 5, 1, 4, 2, 0, 1, 3, 1),
        new ParameterData(1, 5, 0, 5, 6, 0, 1, 1, 2),
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
        new (250, 0, 0, 0, 0, 0),
    };
    /// <summary>クラス毎武器倍率_エラ</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Eraps = new WeaponRate[]
    {
        new (20, 100, 20, 20, 10, 50),
        new (50, 100, 50, 50, 10, 50),
        new (20, 120, 20, 20, 10, 50),
        new (100, 100, 100, 50, 10, 50),
        new (100, 120, 50, 50, 10, 100),
        new (50, 150, 50, 20, 10, 50),
    };
    /// <summary>クラス毎武器倍率_エグザ</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Exa = new WeaponRate[]
    {
        new (10, 50, 100, 10, 30, 30),
        new (50, 50, 100, 10, 30, 30),
        new (10, 50, 120, 10, 30, 30),
        new (50, 100, 100, 50, 100, 50),
        new (100, 50, 120, 10, 30, 30),
        new (10, 50, 150, 10, 30, 30),
    };
    /// <summary>クラス毎武器倍率_ウーラ</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Worra = new WeaponRate[]
    {
        new (10, 10, 10, 100, 10, 50),
        new (10, 10, 10, 120, 50, 50),
        new (50, 10, 10, 100, 30, 100),
        new (10, 10, 10, 150, 100, 50),
        new (70, 30, 30, 120, 100, 100),
        new (100, 10, 100, 100, 70, 100),
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
        new (120, 10, 10, 10, 50, 50),
        new (150, 10, 50, 10, 50, 50),
        new (120, 10, 50, 50, 50, 50),
        new (180, 10, 50, 10, 50, 50),
        new (120, 120, 70, 50, 50, 50),
        new (150, 10, 100, 100, 50, 50),
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
        new(15, 7, 0, 4, 3, 6, 8, 1, 4),        // グリーンスライム
        new(18, 5, 0, 7, 5, 3, 5, 3, 4),        // スケルトン（剣）
        new(18, 6, 0, 4, 4, 2, 11, 1, 3),       // スケルトン（槍）
        new(18, 7, 0, 3, 4, 2, 7, 3, 4),        // スケルトン（斧）
        new(15, 5, 0, 8, 7, 2, 5, 3, 4),        // スケルトン（弓）
        new(13, 0, 6, 7, 5, 2, 4, 8, 4),        // スケルトン（魔）
        new(9, 0, 9, 3, 7, 8, 1, 10, 5),        // ペル
        new(12, 14, 0, 9, 6, 5, 6, 1, 4),       // バーンスライム
        new(11, 10, 0, 7, 12, 7, 5, 1, 6),      // エレキスライム
        new(17, 0, 10, 5, 5, 5, 4, 8, 3),       // ディープスライム
        new(24, 12, 0, 10, 2, 8, 20, 14, 1),    // メタルスライム
        new(20, 11, 0, 9, 6, 6, 5, 5, 4),       // エンジェルス
        new(20, 11, 0, 9, 6, 7, 5, 5, 4),       // アークエンジェルス
        new(20, 11, 0, 9, 6, 8, 6, 7, 4),       // プリンシパリティーズ
        new(20, 12, 0, 9, 7, 8, 7, 5, 5),       // パワーズ
        new(20, 12, 0, 10, 7, 9, 7, 5, 5),      // ヴァーチャーズ
        new(20, 12, 0, 10, 7, 9, 8, 6, 5),      // ドミニオンズ
        new(20, 12, 0, 10, 7, 10, 6, 8, 6),     // トロウンズ
        new(20, 0, 13, 10, 8, 10, 8, 9, 6),     // ケルビム
        new(20, 13, 0, 10, 8, 10, 9, 8, 6),     // セラフィム
        new(20, 14, 14, 11, 8, 11, 9, 9, 7),    // 『尊きもの』
    };

    /// <summary>敵成長率％</summary>
    public static readonly ParameterData[] Prm_EnemyGrow = new ParameterData[]
    {
        new(75, 35, 1, 20, 15, 30, 40, 5, 0),     // グリーンスライム
        new(90, 25, 1, 35, 25, 15, 25, 15, 0),    // スケルトン（剣）
        new(90, 30, 1, 20, 20, 10, 55, 5, 0),     // スケルトン（槍）
        new(90, 35, 1, 15, 20, 10, 35, 15, 0),    // スケルトン（斧）
        new(75, 25, 1, 40, 35, 10, 25, 15, 0),    // スケルトン（弓）
        new(65, 1, 30, 35, 25, 10, 20, 40, 0),    // スケルトン（魔）
        new(45, 1, 45, 15, 35, 40, 5, 50, 0),     // ペル
        new(60, 70, 1, 45, 30, 25, 30, 5, 0),     // バーンスライム
        new(55, 50, 1, 35, 60, 35, 25, 5, 0),     // エレキスライム
        new(95, 1, 65, 30, 15, 15, 25, 35, 0),    // ディープスライム
        new(120, 60, 1, 50, 10, 40, 100, 70, 0),  // メタルスライム
        new(100, 55, 1, 45, 30, 30, 25, 25, 0),   // エンジェルス
        new(100, 55, 1, 45, 30, 35, 25, 25, 0),   // アークエンジェルス
        new(100, 55, 1, 45, 30, 40, 30, 35, 0),   // プリンシパリティーズ
        new(100, 60, 1, 45, 35, 40, 35, 25, 0),   // パワーズ
        new(100, 60, 1, 50, 35, 45, 35, 25, 0),   // ヴァーチャーズ
        new(100, 60, 1, 50, 35, 45, 40, 30, 0),   // ドミニオンズ
        new(100, 60, 1, 50, 35, 50, 30, 40, 0),   // トロウンズ
        new(100, 1, 65, 50, 40, 50, 40, 45, 0),   // ケルビム
        new(100, 65, 1, 50, 40, 50, 45, 40, 0),   // セラフィム
        new(100, 70, 70, 55, 40, 55, 45, 45, 0),  // 『尊きもの』
    };

    /// <summary>
    /// 敵専用その他設定
    /// </summary>
    public struct EnemyOtherData
    {
        /// <summary>使用武器タイプ</summary>
        public ItemType defaultWeaponType;
        /// <summary>レアリティ</summary>
        public int rarelity;
        /// <summary>表示色</summary>
        public Color modelColor;

        public EnemyOtherData(ItemType weapon, int rare, Color? col = null)
        {
            defaultWeaponType = weapon;
            rarelity = rare;
            modelColor = col.HasValue ? col.Value : Color.white;
        }
    }

    /// <summary>敵特殊パラメータ</summary>
    public static readonly EnemyOtherData[] Prm_EnemyOther = new EnemyOtherData[]
    {
        new(ItemType.None, 1, new Color(0.4f, 1, 0.4f)),      // グリーンスライム
        new(ItemType.Sword, 3),         // スケルトン（剣）
        new(ItemType.Spear, 4),         // スケルトン（槍）
        new(ItemType.Axe, 2),           // スケルトン（斧）
        new(ItemType.Arrow, 3),         // スケルトン（弓）
        new(ItemType.Book, 5),          // スケルトン（魔）
        new(ItemType.None, 7),          // ペル
        new(ItemType.None, 21, new Color(1, 0.3f, 0.1f)),     // バーンスライム
        new(ItemType.None, 21, new Color(1, 1, 0f)),          // エレキスライム
        new(ItemType.None, 21, new Color(0, 0, 0.8f)),        // ディープスライム
        new(ItemType.None, 31, new Color(0.7f, 0.7f, 0.7f)),  // メタルスライム
        new(ItemType.Arrow, 101),       // エンジェルス
        new(ItemType.Sword, 104),       // アークエンジェルス
        new(ItemType.Spear, 107),       // プリンシパリティーズ
        new(ItemType.Axe, 110),         // パワーズ
        new(ItemType.Arrow, 113),       // ヴァーチャーズ
        new(ItemType.Spear, 116),       // ドミニオンズ
        new(ItemType.Axe, 119),         // トロウンズ
        new(ItemType.Book, 121),        // ケルビム
        new(ItemType.Sword, 124),       // セラフィム
        new(ItemType.Book, 127),        // 『尊きもの』
    };

    /// <summary>
    /// ランダム出現の敵を選択
    /// </summary>
    /// <param name="floor"></param>
    /// <param name="isBoss"></param>
    /// <returns></returns>
    public static Constant.EnemyID CalcRandomEnemy(int floor, bool isBoss)
    {
        var koho = new List<Constant.EnemyID>();
        for (var i = 0; i < (int)Constant.EnemyID.ENEMY_COUNT; ++i)
        {
            var data = Prm_EnemyOther[i];
            if (data.rarelity < 0) continue;

            // rarelityフロアまでは出現しない
            if (data.rarelity > floor) continue;
            // ボスは100以上
            if (isBoss && data.rarelity <= 100) continue;

            koho.Add((Constant.EnemyID)i);
        }
        if (koho.Count == 0) return Constant.EnemyID.SlimeGreen;

        // 候補からrarelityによって確率で判定
        var kohoRarelity = koho.Select(id => floor - Prm_EnemyOther[(int)id].rarelity + 1).ToList();
        var selIdx = SelectRateIndex(kohoRarelity);

        return koho[selIdx];
    }

    #endregion

    #region 名称データベース

    // 他の場所にあるテキスト
    // 行動　待機　撤退
    // 攻撃　命中　必殺
    // 
    // 力　魔力　攻撃　技　速さ　幸運　守備　魔防　移動
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

    /// <summary>敵名</summary>
    public static readonly string[] Name_Enemies =
    {
        "グリーンスライム",
        "スケルトン",
        "スケルトン",
        "スケルトン",
        "スケルトン",
        "スケルトン",
        "ペル",
        "バーンスライム",
        "エレキスライム",
        "ディープスライム",
        "メタルスライム",
        "エンジェルス",
        "アークエンジェルス",
        "プリンシパリティーズ",
        "パワーズ",
        "ヴァーチャーズ",
        "ドミニオンズ",
        "トロウンズ",
        "ケルビム",
        "セラフィム",
        "『尊きもの』",
    };

    #endregion

    #region スキルデータベース

    /// <summary>
    /// スキルID
    /// </summary>
    public enum SkillID
    {
        Drows_FreeHand = 0,
        Drows_WeaponBreak,
        Drows_Tornaid,
        Drows_WeaponSave,
        Drows_Berserk,
        Drows_Critical,
        Eraps_ZOC,
        Eraps_FirstSpeed,
        Eraps_AllCounter,
        Exa_Guard,
        Exa_Command,
        Exa_Lonely,
        Worra_LongShot,
        Worra_FastMove,
        Worra_Avoid,
        Koob_World,
        Koob_Zenius,
        Koob_Archemy,
        You_StrongAttack,
        You_StealthAttack,
        You_CounterPlus,

        SKILL_COUNT,
    }

    /// <summary>
    /// スキルデータ
    /// </summary>
    public struct SkillData
    {
        /// <summary>説明文</summary>
        public string detail;
        /// <summary>習得プレイヤー</summary>
        public Constant.PlayerID getPlayer;
        /// <summary>習得クラス</summary>
        public Constant.ClassID getClass;
        /// <summary>習得レベル</summary>
        public int getLevel;
        /// <summary>転生で保持するフラグ</summary>
        public bool canKeep;

        public SkillData(Constant.PlayerID pid, Constant.ClassID cid, int lv, string det, bool keep = true)
        {
            detail = det; getPlayer = pid; getLevel = lv; getClass = cid; canKeep = keep;
        }
    }

    /// <summary>
    /// スキルデータ
    /// </summary>
    public static readonly SkillData[] SkillDataList = new SkillData[]
    {
        new (Constant.PlayerID.Drows, Constant.ClassID.Base, 1, "素手で攻撃できる"),
        new (Constant.PlayerID.Drows, Constant.ClassID.Base, 1, "武器の消費量が2倍になる"),
        new (Constant.PlayerID.Drows, Constant.ClassID.A2, 10, "一撃で敵を倒した時、再行動する"),
        new (Constant.PlayerID.Drows, Constant.ClassID.AB, 10, "自分から攻撃する時は武器が消耗しない"),
        new (Constant.PlayerID.Drows, Constant.ClassID.B2, 1, "近くに敵が居ると自動的に攻撃する(継承不可)", false),
        new (Constant.PlayerID.Drows, Constant.ClassID.B2, 1, "必殺率+30"),
        new (Constant.PlayerID.Eraps, Constant.ClassID.A2, 10, "敵は隣接マスを通過できない"),
        new (Constant.PlayerID.Eraps, Constant.ClassID.AB, 10, "フロアの1ターン目のみ移動力+4"),
        new (Constant.PlayerID.Eraps, Constant.ClassID.B2, 10, "距離に関係なく反撃できる"),
        new (Constant.PlayerID.Exa, Constant.ClassID.A2, 10, "周囲2マスの味方の防御+30％"),
        new (Constant.PlayerID.Exa, Constant.ClassID.AB, 10, "周囲2マスの味方の命中、回避、必殺+20"),
        new (Constant.PlayerID.Exa, Constant.ClassID.B2, 10, "周囲2マスに味方が居ない時攻撃、防御+10"),
        new (Constant.PlayerID.Worra, Constant.ClassID.A2, 10, "最大射程+1"),
        new (Constant.PlayerID.Worra, Constant.ClassID.AB, 10, "移動+1"),
        new (Constant.PlayerID.Worra, Constant.ClassID.B2, 10, "回避+30"),
        new (Constant.PlayerID.Koob, Constant.ClassID.A2, 10, "必ず武器相性が有利になる"),
        new (Constant.PlayerID.Koob, Constant.ClassID.AB, 10, "取得経験値+50％"),
        new (Constant.PlayerID.Koob, Constant.ClassID.B2, 10, "武器を壊した時、稀に同種の武器を生成する"),
        new (Constant.PlayerID.You, Constant.ClassID.A2, 10, "自分から攻撃する時、攻撃+10"),
        new (Constant.PlayerID.You, Constant.ClassID.AB, 10, "自分から攻撃する時、反撃されない"),
        new (Constant.PlayerID.You, Constant.ClassID.B2, 10, "反撃する時、攻撃回数+1"),
    };

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
        new(ItemType.None,  "素手",               -1, -1, 0, 100, 1, 1, 0),
        new(ItemType.Book,  "素手（魔）",         -1, -1, 0, 100, 1, 2, 0),
        new(ItemType.Item,  "エリクサー",         1, 3, 100, 100, 0, 0, 0),
        new(ItemType.Rod,   "リフの杖",           1, 30, 10, 100, 1, 1, 0),
        new(ItemType.Sword, "鉄の剣",             1, 40, 5, 85, 1, 1, 0),
        new(ItemType.Spear, "鉄の槍",             1, 40, 7, 70, 1, 1, 0),
        new(ItemType.Axe,   "鉄の斧",             1, 40, 8, 65, 1, 1, 0),
        new(ItemType.Arrow, "鉄の弓",             1, 40, 6, 80, 2, 3, 0),
        new(ItemType.Book,  "ファイアー",         1, 40, 5, 95, 1, 2, 0),
        new(ItemType.Sword, "銀の剣",             15, 20, 13, 75, 1, 1, 0),
        new(ItemType.Spear, "銀の槍",             15, 20, 14, 65, 1, 1, 0),
        new(ItemType.Axe,   "銀の斧",             15, 20, 15, 55, 1, 1, 0),
        new(ItemType.Arrow, "銀の弓",             15, 20, 13, 70, 2, 3, 0),
        new(ItemType.Book,  "ブリザード",         15, 20, 13, 80, 1, 2, 0),
        new(ItemType.Sword, "キルソード",         51, 20, 9, 80, 1, 1, 30),
        new(ItemType.Spear, "キラーランス",       51, 20, 10, 75, 1, 1, 30),
        new(ItemType.Axe,   "キラーアクス",       51, 20, 11, 65, 1, 1, 30),
        new(ItemType.Arrow, "キラーボウ",         51, 20, 9, 80, 2, 3, 30),
        new(ItemType.Book,  "キルグリム",         51, 20, 8, 90, 1, 2, 30),
        new(ItemType.Sword, "ルキフグス",         101, 40, 23, 80, 1, 2, 15),
        new(ItemType.Spear, "アガリアレプト",     101, 40, 18, 100, 1, 2, 15),
        new(ItemType.Axe,   "サタナキア",         101, 40, 19, 90, 1, 2, 15),
        new(ItemType.Arrow, "フルーレティ",       101, 40, 18, 85, 2, 4, 15),
        new(ItemType.Book,  "アッピンの赤い本",   101, 40, 21, 95, 1, 3, 15),
        new(ItemType.Sword, "妙法村正",           151, 40, 16, 120, 1, 1, 60),
        new(ItemType.Spear, "ディスラプター",     151, 40, 27, 65, 1, 3, 0),
        new(ItemType.Axe,   "フレースヴェルグ",   151, 40, 22, 100, 1, 1, 20),
        new(ItemType.Arrow, "マルドゥーク",       151, 40, 15, 100, 2, 6, 0),
        new(ItemType.Book,  "ネクロノミコン",     151, 40, 36, 40, 1, 2, 0),

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
    /// <param name="floor">フロア番号</param>
    /// <param name="boss">ボスである</param>
    /// <param name="weaponOnly">true:武器のみ</param>
    /// <param name="type">None以外を指定するとタイプを指定</param>
    /// <param name="outRate">ハズレになる確率0～100</param>
    /// <returns></returns>
    public static ItemID CalcRandomItem(int floor, bool boss, bool weaponOnly = true, ItemType type = ItemType.None, int outRate = 0)
    {
        if (Util.RandomCheck(outRate)) return ItemID.FreeHand;

        var bossRate = boss ? 2 : 1; // ボスはレアリティ下がる＝レア高いものが出やすい

        // 武器なんでもの場合、持ってないもの優先
        if (weaponOnly && type == ItemType.None)
        {
            var emptyType = new List<ItemType>();
            if (!GameParameter.otherData.haveItemList.Any(itm => itm.ItemData.iType == ItemType.Sword)) emptyType.Add(ItemType.Sword);
            if (!GameParameter.otherData.haveItemList.Any(itm => itm.ItemData.iType == ItemType.Sword)) emptyType.Add(ItemType.Spear);
            if (!GameParameter.otherData.haveItemList.Any(itm => itm.ItemData.iType == ItemType.Sword)) emptyType.Add(ItemType.Axe);
            if (!GameParameter.otherData.haveItemList.Any(itm => itm.ItemData.iType == ItemType.Sword)) emptyType.Add(ItemType.Arrow);
            if (!GameParameter.otherData.haveItemList.Any(itm => itm.ItemData.iType == ItemType.Sword)) emptyType.Add(ItemType.Book);

            if (emptyType.Count > 0)
                type = emptyType[Util.RandomInt(0, emptyType.Count - 1)];
        }

        var koho = new List<ItemID>();

        for (var i = 0; i < (int)ItemID.ITEM_COUNT; ++i)
        {
            var data = ItemDataList[i];
            if (data.iType == ItemType.None) continue;
            if (data.rarelity < 0) continue;
            if (type != ItemType.None && data.iType != type) continue;
            if (weaponOnly &&
                (data.iType == ItemType.Rod || data.iType == ItemType.Item)) continue;

            // rarelityフロアまでは出現しない
            if (data.rarelity > floor) continue;
            koho.Add((ItemID)i);
        }
        if (koho.Count == 0) return ItemID.FreeHand;

        // 候補からrarelityによって確率で判定
        var kohoRarelity = koho.Select(id => floor - ItemDataList[(int)id].rarelity / bossRate + 1).ToList();
        var selIdx = SelectRateIndex(kohoRarelity);

        return koho[selIdx];
    }

    #endregion

    #region 計算

    /// <summary>
    /// 選ばれやすさintのリスト → ランダムでどれかを選ぶ
    /// </summary>
    /// <param name="rateList"></param>
    /// <returns>選ぶインデックス</returns>
    private static int SelectRateIndex(List<int> rateList)
    {
        var rand = Util.RandomInt(0, rateList.Sum() - 1);
        for (var i = 0; i < rateList.Count; i++)
        {
            if (rateList[i] > rand) return i;
            rand -= rateList[i];
        }

        return rateList.Count - 1;
    }

    #endregion
}

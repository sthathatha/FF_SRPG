using System.Collections;
using System.Collections.Generic;
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
        new ParameterData(70, 120, 0, 45, 35, 40, 25, 10, 0),
        new ParameterData(100, 50, 5, 30, 25, 35, 70, 50, 0),
        new ParameterData(85, 60, 5, 60, 45, 30, 40, 25, 0),
        new ParameterData(70, 35, 20, 60, 75, 35, 20, 30, 0),
        new ParameterData(45, 15, 80, 35, 30, 60, 15, 70, 0),
        new ParameterData(75, 50, 25, 50, 50, 45, 30, 30, 0),
    };

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

    #endregion
}

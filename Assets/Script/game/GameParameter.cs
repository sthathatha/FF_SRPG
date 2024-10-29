using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム内のパラメータ管理各種機能
/// </summary>
public class GameParameter
{

    #region プレイヤーセーブデータ

    public static PlayerSaveParameter Prm_Drows = new();
    public static PlayerSaveParameter Prm_Eraps = new();
    public static PlayerSaveParameter Prm_Exa = new();
    public static PlayerSaveParameter Prm_Worra = new();
    public static PlayerSaveParameter Prm_Koob = new();
    public static PlayerSaveParameter Prm_You = new();

    /// <summary>
    /// パラメータ取得
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public static PlayerSaveParameter Prm_Get(Constant.PlayerID playerID)
    {
        return playerID switch
        {
            Constant.PlayerID.Drows => Prm_Drows,
            Constant.PlayerID.Eraps => Prm_Eraps,
            Constant.PlayerID.Exa => Prm_Exa,
            Constant.PlayerID.Worra => Prm_Worra,
            Constant.PlayerID.Koob => Prm_Koob,
            _ => Prm_You,
        };
    }

    /// <summary>
    /// セーブデータ内プレイヤーキャラパラメータ
    /// </summary>
    public class PlayerSaveParameter
    {
        public int Lv;
        public int Exp;

        public int MaxHP;
        public int Atk;
        public int Mag;
        public int Tec;
        public int Spd;
        public int Luk;
        public int Def;
        public int Mdf;
        public int Move;

        /// <summary>転生回数</summary>
        public int ReviveCount;
        /// <summary>クラス</summary>
        public Constant.ClassID ClassID;

        /// <summary>回復待ち戦闘回数</summary>
        public int RestBattle;
        /// <summary>位置</summary>
        public Vector2Int Location;

        /// <summary>所持スキル</summary>
        public List<int> Skills = new List<int>();

        /// <summary>
        /// Lv1初期化
        /// </summary>
        /// <param name="pid"></param>
        public void Init(Constant.PlayerID pid)
        {
            var lv1Param = GameDatabase.Prm_PlayerInit[(int)pid];

            Lv = 1;
            Exp = 0;
            MaxHP = lv1Param.maxHp;
            Atk = lv1Param.atk;
            Mag = lv1Param.mag;
            Tec = lv1Param.tec;
            Spd = lv1Param.spd;
            Luk = lv1Param.luk;
            Def = lv1Param.def;
            Mdf = lv1Param.mdf;
            Move = lv1Param.move;

            ReviveCount = 0;
            ClassID = Constant.ClassID.Base;
            RestBattle = 0;
            Location = Vector2Int.zero;
            Skills.Clear();
        }

        /// <summary>
        /// セーブデータ用文字列作成
        /// </summary>
        /// <returns></returns>
        public string ToSaveString()
        {
            return "";
        }

        /// <summary>
        /// セーブデータから読み込み
        /// </summary>
        /// <param name="data"></param>
        public void ReadString(string data)
        {
        }
    }

    #endregion

    #region フィールドパラメータ

    /// <summary>
    /// フィールドキャラクターパラメータ
    /// </summary>
    public class FieldCharacterParameter
    {
        public int Lv;
        public int HP;
        public int MaxHP;
        public int Exp;
        public int Atk;
        public int Mag;
        public int Tec;
        public int Spd;
        public int Luk;
        public int Def;
        public int Mdf;
        public int Move;

        /// <summary>
        /// プレイヤーパラメータ初期化
        /// </summary>
        /// <param name="pid"></param>
        public void InitPlayer(Constant.PlayerID pid)
        {
            // セーブデータから取得
            var prm = pid switch
            {
                Constant.PlayerID.Drows => Prm_Drows,
                Constant.PlayerID.Eraps => Prm_Eraps,
                Constant.PlayerID.Exa => Prm_Exa,
                Constant.PlayerID.Worra => Prm_Worra,
                Constant.PlayerID.Koob => Prm_Koob,
                _ => Prm_You,
            };

            Lv = prm.Lv;
            HP = prm.MaxHP;
            MaxHP = prm.MaxHP;
            Atk = prm.Atk;
            Mag = prm.Mag;
            Tec = prm.Tec;
            Spd = prm.Spd;
            Luk = prm.Luk;
            Def = prm.Def;
            Mdf = prm.Mdf;
            Move = prm.Move;
        }

        /// <summary>
        /// 敵パラメータ作成
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="lv"></param>
        public void InitEnemy(Constant.EnemyID eid, int lv)
        {
            var prm = GameDatabase.Prm_EnemyInit[(int)eid];
            var grow = GameDatabase.Prm_EnemyGrow[(int)eid];
            var growCnt = lv - 1;
            var rand = new Func<int, int>(rate =>
            {
                if (rate == 0) return 0;
                return Mathf.FloorToInt(rate * growCnt * Util.RandomFloatSin(0.9f, 1.1f) / 100f);
            });

            Lv = lv;
            MaxHP = prm.maxHp + rand(grow.maxHp);
            HP = MaxHP;
            Atk = prm.atk + rand(grow.atk);
            Mag = prm.mag + rand(grow.mag);
            Tec = prm.tec + rand(grow.tec);
            Spd = prm.spd + rand(grow.spd);
            Luk = prm.luk + rand(grow.luk);
            Def = prm.def + rand(grow.def);
            Mdf = prm.mdf + rand(grow.mdf);
            Move = prm.move;
        }

        /// <summary>
        /// セーブデータ用文字列作成
        /// </summary>
        /// <returns></returns>
        public string ToSaveString()
        {
            return "";
        }

        /// <summary>
        /// セーブデータから読み込み
        /// </summary>
        /// <param name="data"></param>
        public void ReadString(string data)
        {
        }
    }

    #endregion
}

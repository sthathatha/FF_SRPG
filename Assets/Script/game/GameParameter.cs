using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ゲーム内のパラメータ管理各種機能
/// </summary>
public class GameParameter
{
    #region セーブ処理

    // プレイヤーパラメータ
    private const string SAVE_PARAM_DROWS = "paramDrows";
    private const string SAVE_PARAM_ERAPS = "paramEraps";
    private const string SAVE_PARAM_EXA = "paramExa";
    private const string SAVE_PARAM_WORRA = "paramWorra";
    private const string SAVE_PARAM_KOOB = "paramKoob";
    private const string SAVE_PARAM_YOU = "paramYou";

    // OtherData
    private const string SAVE_ITEM_LIST = "haveItemList";
    private const string SAVE_EQ_DROWS = "eqDrows";
    private const string SAVE_EQ_ERAPS = "eqEraps";
    private const string SAVE_EQ_EXA = "eqExa";
    private const string SAVE_EQ_WORRA = "eqWorra";
    private const string SAVE_EQ_KOOB = "eqKoob";
    private const string SAVE_EQ_YOU = "eqYou";

    /// <summary>
    /// セーブ
    /// </summary>
    public static void Save()
    {
        var save = Global.GetSaveData();
        save.SetGameData(SAVE_PARAM_DROWS, Prm_Drows.ToSaveString());
        save.SetGameData(SAVE_PARAM_ERAPS, Prm_Eraps.ToSaveString());
        save.SetGameData(SAVE_PARAM_EXA, Prm_Exa.ToSaveString());
        save.SetGameData(SAVE_PARAM_WORRA, Prm_Worra.ToSaveString());
        save.SetGameData(SAVE_PARAM_KOOB, Prm_Koob.ToSaveString());
        save.SetGameData(SAVE_PARAM_YOU, Prm_You.ToSaveString());
        otherData.Save();
    }

    /// <summary>
    /// ロード
    /// </summary>
    public static void Load()
    {
        var save = Global.GetSaveData();
        otherData.Load();
        Prm_Drows.ReadString(save.GetGameDataString(SAVE_PARAM_DROWS));
        Prm_Eraps.ReadString(save.GetGameDataString(SAVE_PARAM_ERAPS));
        Prm_Exa.ReadString(save.GetGameDataString(SAVE_PARAM_EXA));
        Prm_Worra.ReadString(save.GetGameDataString(SAVE_PARAM_WORRA));
        Prm_Koob.ReadString(save.GetGameDataString(SAVE_PARAM_KOOB));
        Prm_You.ReadString(save.GetGameDataString(SAVE_PARAM_YOU));
    }

    #endregion

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
    /// フィールドデータから保存
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="prm"></param>
    public static void Prm_SetFieldParam(Constant.PlayerID playerID, FieldCharacterParameter prm)
    {
        (playerID switch
        {
            Constant.PlayerID.Drows => Prm_Drows,
            Constant.PlayerID.Eraps => Prm_Eraps,
            Constant.PlayerID.Exa => Prm_Exa,
            Constant.PlayerID.Worra => Prm_Worra,
            Constant.PlayerID.Koob => Prm_Koob,
            _ => Prm_You,
        }).ReadFromFieldParam(prm);
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
            Skills.Clear();
        }

        /// <summary>
        /// フィールドパラメータからセット
        /// </summary>
        /// <param name="param"></param>
        public void ReadFromFieldParam(FieldCharacterParameter param)
        {
            Lv = param.Lv;
            Exp = param.Exp;
            MaxHP = param.MaxHP;
            Atk = param.Atk;
            Mag = param.Mag;
            Tec = param.Tec;
            Spd = param.Spd;
            Luk = param.Luk;
            Def = param.Def;
            Mdf = param.Mdf;
            Move = param.Move;
        }

        /// <summary>
        /// セーブデータ用文字列作成
        /// </summary>
        /// <returns></returns>
        public string ToSaveString()
        {
            var sklStr = string.Join('_', Skills);
            return $"{Lv},{Exp},{MaxHP},{Atk},{Mag},{Tec},{Spd},{Luk},{Def},{Mdf},{Move},{ReviveCount},{(int)ClassID},{RestBattle},{sklStr}";
        }

        /// <summary>
        /// セーブデータから読み込み
        /// </summary>
        /// <param name="data"></param>
        public void ReadString(string data)
        {
            var spl = data.Split(",");
            Lv = int.Parse(spl[0]);
            Exp = int.Parse(spl[1]);
            MaxHP = int.Parse(spl[2]);
            Atk = int.Parse(spl[3]);
            Mag = int.Parse(spl[4]);
            Tec = int.Parse(spl[5]);
            Spd = int.Parse(spl[6]);
            Luk = int.Parse(spl[7]);
            Def = int.Parse(spl[8]);
            Mdf = int.Parse(spl[9]);
            Move = int.Parse(spl[10]);
            ReviveCount = int.Parse(spl[11]);
            ClassID = (Constant.ClassID)int.Parse(spl[12]);
            RestBattle = int.Parse(spl[13]);
            Skills.Clear();
            if (!string.IsNullOrEmpty(spl[14]))
            {
                var skillSpl = spl[14].Split('_');
                foreach (var skill in skillSpl)
                {
                    Skills.Add(int.Parse(skill));
                }
            }
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
            return $"{Lv},{HP},{MaxHP},{Exp},{Atk},{Mag},{Tec},{Spd},{Luk},{Def},{Mdf},{Move}";
        }

        /// <summary>
        /// セーブデータから読み込み
        /// </summary>
        /// <param name="data"></param>
        public void ReadString(string data)
        {
            var spl = data.Split(',');
            Lv = int.Parse(spl[0]);
            HP = int.Parse(spl[1]);
            MaxHP = int.Parse(spl[2]);
            Exp = int.Parse(spl[3]);
            Atk = int.Parse(spl[4]);
            Mag = int.Parse(spl[5]);
            Tec = int.Parse(spl[6]);
            Spd = int.Parse(spl[7]);
            Luk = int.Parse(spl[8]);
            Def = int.Parse(spl[9]);
            Mdf = int.Parse(spl[10]);
            Move = int.Parse(spl[11]);
        }
    }

    #endregion

    #region その他フィールド外データ

    /// <summary>
    /// その他フィールド外データ
    /// </summary>
    public class OtherData
    {
        #region セーブ・ロード

        /// <summary>
        /// 開始時初期化
        /// </summary>
        public void Init()
        {
            haveItemList.Clear();
            AddItem(GameDatabase.ItemID.Sword1);
            AddItem(GameDatabase.ItemID.Spear1);
            AddItem(GameDatabase.ItemID.Axe1);
            AddItem(GameDatabase.ItemID.Arrow1);
            AddItem(GameDatabase.ItemID.Book1);
            AddItem(GameDatabase.ItemID.Rod1);
            AddItem(GameDatabase.ItemID.Item1);

            equip_Drows = GetUsableEquip(Constant.PlayerID.Drows);
            equip_Eraps = GetUsableEquip(Constant.PlayerID.Eraps);
            equip_Exa = GetUsableEquip(Constant.PlayerID.Exa);
            equip_Worra = GetUsableEquip(Constant.PlayerID.Worra);
            equip_Koob = GetUsableEquip(Constant.PlayerID.Koob);
            equip_You = GetUsableEquip(Constant.PlayerID.You);
        }

        /// <summary>
        /// セーブ
        /// </summary>
        public void Save()
        {
            var save = Global.GetSaveData();
            var itemListString = string.Join('X', haveItemList.Select(i => i.ToSaveString()));
            save.SetGameData(SAVE_ITEM_LIST, itemListString);

            save.SetGameData(SAVE_EQ_DROWS, equip_Drows);
            save.SetGameData(SAVE_EQ_ERAPS, equip_Eraps);
            save.SetGameData(SAVE_EQ_EXA, equip_Exa);
            save.SetGameData(SAVE_EQ_WORRA, equip_Worra);
            save.SetGameData(SAVE_EQ_KOOB, equip_Koob);
            save.SetGameData(SAVE_EQ_YOU, equip_You);
        }

        /// <summary>
        /// ロード
        /// </summary>
        public void Load()
        {
            var save = Global.GetSaveData();
            var itemList = save.GetGameDataString(SAVE_ITEM_LIST).Split('X');

            haveItemList.Clear();
            if (!string.IsNullOrEmpty(itemList[0]))
            {
                foreach (var item in itemList)
                {
                    haveItemList.Add(HaveItemData.FromString(item));
                }
            }

            equip_Drows = save.GetGameDataInt(SAVE_EQ_DROWS);
            equip_Eraps = save.GetGameDataInt(SAVE_EQ_ERAPS);
            equip_Exa = save.GetGameDataInt(SAVE_EQ_EXA);
            equip_Worra = save.GetGameDataInt(SAVE_EQ_WORRA);
            equip_Koob = save.GetGameDataInt(SAVE_EQ_KOOB);
            equip_You = save.GetGameDataInt(SAVE_EQ_YOU);
        }

        #endregion

        #region 所持アイテムデータ

        /// <summary>
        /// 所持アイテムデータ
        /// </summary>
        public class HaveItemData
        {
            /// <summary>ID</summary>
            public GameDatabase.ItemID id;
            /// <summary>残り使用回数</summary>
            public int useCount;

            /// <summary>
            /// アイテム新品を取得
            /// </summary>
            /// <param name="}id"></param>
            public HaveItemData(GameDatabase.ItemID _id)
            {
                id = _id;
                var itm = GameDatabase.ItemDataList[(int)id];
                useCount = itm.maxUse;
            }

            /// <summary>アイテム詳細取得</summary>
            public GameDatabase.ItemData ItemData { get { return GameDatabase.ItemDataList[(int)id]; } }

            /// <summary>
            /// セーブ用string
            /// </summary>
            /// <returns></returns>
            public string ToSaveString()
            {
                return $"{(int)id}-{useCount}";
            }

            /// <summary>
            /// セーブ用stringから生成
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static HaveItemData FromString(string str)
            {
                var values = str.Split("-");
                var data = new HaveItemData((GameDatabase.ItemID)int.Parse(values[0]));
                data.useCount = int.Parse(values[1]);
                return data;
            }
        }

        /// <summary>
        /// 所持アイテムリスト
        /// </summary>
        public List<HaveItemData> haveItemList = new List<HaveItemData>();

        #endregion

        #region 各キャラ装備

        public int equip_Drows;
        public int equip_Eraps;
        public int equip_Exa;
        public int equip_Worra;
        public int equip_Koob;
        public int equip_You;

        /// <summary>
        /// 装備を取得
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int GetEquipIndex(Constant.PlayerID pid)
        {
            return pid switch
            {
                Constant.PlayerID.Drows => equip_Drows,
                Constant.PlayerID.Eraps => equip_Eraps,
                Constant.PlayerID.Exa => equip_Exa,
                Constant.PlayerID.Worra => equip_Worra,
                Constant.PlayerID.Koob => equip_Koob,
                _ => equip_You,
            };
        }

        /// <summary>
        /// 装備を設定
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="idx"></param>
        public void SetEquipIndex(Constant.PlayerID pid, int idx)
        {
            switch (pid)
            {
                case Constant.PlayerID.Drows: equip_Drows = idx; break;
                case Constant.PlayerID.Eraps: equip_Eraps = idx; break;
                case Constant.PlayerID.Exa: equip_Exa = idx; break;
                case Constant.PlayerID.Worra: equip_Worra = idx; break;
                case Constant.PlayerID.Koob: equip_Koob = idx; break;
                case Constant.PlayerID.You: equip_You = idx; break;
            }
        }

        /// <summary>
        /// アイテム削除
        /// </summary>
        /// <param name="idx"></param>
        public void DeleteItem(int idx)
        {
            //削除
            haveItemList.RemoveAt(idx);

            // 装備品対処
            // ドロシー
            if (idx == equip_Drows) equip_Drows = -1;
            else if (idx < equip_Drows) equip_Drows--;

            // 他は上から得意武器検索
            if (idx == equip_Eraps) equip_Eraps = GetUsableEquip(Constant.PlayerID.Eraps);
            else if (idx < equip_Eraps) equip_Eraps--;
            if (idx == equip_Exa) equip_Exa = GetUsableEquip(Constant.PlayerID.Exa);
            else if (idx < equip_Exa) equip_Exa--;
            if (idx == equip_Worra) equip_Worra = GetUsableEquip(Constant.PlayerID.Worra);
            else if (idx < equip_Worra) equip_Worra--;
            if (idx == equip_Koob) equip_Koob = GetUsableEquip(Constant.PlayerID.Koob);
            else if (idx < equip_Koob) equip_Koob--;
            if (idx == equip_You) equip_You = GetUsableEquip(Constant.PlayerID.You);
            else if (idx < equip_You) equip_You--;
        }

        /// <summary>
        /// アイテム入手
        /// </summary>
        /// <param name="itemID"></param>
        public void AddItem(GameDatabase.ItemID itemID)
        {
            haveItemList.Add(new HaveItemData(itemID));
        }

        /// <summary>
        /// 装備できるものを上から探す
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int GetUsableEquip(Constant.PlayerID pid)
        {
            var prm = Prm_Get(pid);
            var rates = GameDatabase.Prm_ClassWeaponRate_Get(pid, prm.ClassID);

            for (var i = 0; i < haveItemList.Count; ++i)
            {
                var itm = haveItemList[i].ItemData;
                if (itm.iType == GameDatabase.ItemType.Rod ||
                    itm.iType == GameDatabase.ItemType.Item) continue;

                if (rates.Get(itm.iType) >= 100)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }

    public static OtherData otherData = new OtherData();

    #endregion

    #region 戦闘計算式

    /// <summary>
    /// 戦闘結果数値の計算
    /// 実際のダメージ等はこれを使って命中やクリティカル等行う
    /// </summary>
    /// <param name="atkChr"></param>
    /// <param name="defChr"></param>
    /// <returns></returns>
    public static BattleParameter GetBattleParameter(CharacterBase atkChr, CharacterBase defChr)
    {
        var ret = new BattleParameter();

        #region 武器データ取得
        // 武器ID
        var a_weaponId = GameDatabase.ItemID.FreeHand;
        if (atkChr.IsPlayer())
        {
            var idx = otherData.GetEquipIndex(((PlayerCharacter)atkChr).playerID);
            if (idx < 0) a_weaponId = GameDatabase.ItemID.FreeHand;
            else a_weaponId = otherData.haveItemList[idx].id;
        }
        else
            a_weaponId = ((EnemyCharacter)atkChr).weaponID;
        var a_weaponData = GameDatabase.ItemDataList[(int)a_weaponId];

        // 防御側
        var d_weaponId = GameDatabase.ItemID.FreeHand;
        if (defChr.IsPlayer())
        {
            var idx = otherData.GetEquipIndex(((PlayerCharacter)defChr).playerID);
            if (idx < 0) d_weaponId = GameDatabase.ItemID.FreeHand;
            else d_weaponId = otherData.haveItemList[idx].id;
        }
        else
            d_weaponId = ((EnemyCharacter)defChr).weaponID;
        var d_weaponData = GameDatabase.ItemDataList[(int)d_weaponId];

        // 武器相性
        var advantage = GameDatabase.CheckWeaponAdvantage(a_weaponId, d_weaponId);
        #endregion
        var a_melee = a_weaponData.is_melee();
        var d_melee = d_weaponData.is_melee();

        #region 得意武器判定
        var a_weaponRate = 100;
        var d_weaponRate = 100;
        if (atkChr.IsPlayer())
        {
            var pid = ((PlayerCharacter)atkChr).playerID;
            var rates = GameDatabase.Prm_ClassWeaponRate_Get(pid, Prm_Get(pid).ClassID);
            a_weaponRate = rates.Get(a_weaponData.iType);
        }
        if (defChr.IsPlayer())
        {
            var pid = ((PlayerCharacter)defChr).playerID;
            var rates = GameDatabase.Prm_ClassWeaponRate_Get(pid, Prm_Get(pid).ClassID);
            d_weaponRate = rates.Get(d_weaponData.iType);
        }
        #endregion

        #region スキル・相性による加算値 武器性能にプラスする計算
        // クリティカル率
        var a_dmgPlus = 0;
        var d_dmgPlus = 0;
        var a_hitPlus = 0;
        var d_hitPlus = 0;
        var a_crtPlus = 0;
        var d_crtPlus = 0;
        var d_rangePlus = 0;
        // 武器相性
        if (advantage != 0)
        {
            // 攻撃側有利なら攻撃側＋、反撃側－
            a_dmgPlus += advantage;
            a_hitPlus += advantage * 10;
            d_dmgPlus -= advantage;
            d_hitPlus -= advantage * 10;
        }
        if (atkChr.IsPlayer())
        {
            //todo:スキルによって命中、回避、必殺等増加
        }
        if (defChr.IsPlayer())
        {
        }
        #endregion

        #region 反撃の射程判定
        var distV = atkChr.GetLocation() - defChr.GetLocation();
        var distance = Math.Abs(distV.x) + Math.Abs(distV.y);
        var canCounter = false;
        // ドロシー以外の素手は反撃不可
        if (defChr.IsPlayer() &&
            d_weaponId == GameDatabase.ItemID.FreeHand &&
            ((PlayerCharacter)defChr).playerID != Constant.PlayerID.Drows)
            canCounter = false;
        else
            canCounter = distance >= d_weaponData.rangeMin && distance <= d_weaponData.rangeMax + d_rangePlus;
        #endregion

        #region 攻撃側パラメータ
        var a_atk = a_melee ? atkChr.param.Atk : atkChr.param.Mag;
        var a_def = d_melee ? atkChr.param.Def : atkChr.param.Mdf;
        var a_spd = atkChr.param.Spd;
        var a_hit = atkChr.param.Tec * 2 + atkChr.param.Luk / 2;
        var a_crt = atkChr.param.Tec / 2;
        var a_avd = atkChr.param.Spd * 2;
        var a_acrt = atkChr.param.Luk;
        #endregion

        #region 防御側パラメータ
        var d_atk = d_melee ? defChr.param.Atk : defChr.param.Mag;
        var d_def = a_melee ? defChr.param.Def : defChr.param.Mdf;
        var d_spd = defChr.param.Spd;
        var d_hit = defChr.param.Tec * 2 + defChr.param.Luk / 2;
        var d_crt = defChr.param.Tec / 2;
        var d_avd = defChr.param.Spd * 2;
        var d_acrt = defChr.param.Luk;
        #endregion

        #region 武器性能加算
        // 力が100を超えたら武器威力は％で増加
        var aWpnAtk = (a_weaponData.atk + a_dmgPlus) * a_weaponRate / 100;
        var dWpnAtk = (d_weaponData.atk + d_dmgPlus) * d_weaponRate / 100;
        a_atk = CalcWeaponValue(a_atk, aWpnAtk);
        d_atk = CalcWeaponValue(d_atk, dWpnAtk);

        // 命中
        a_hit = CalcWeaponValue(a_hit, a_weaponData.hit + a_hitPlus);
        d_hit = CalcWeaponValue(d_hit, d_weaponData.hit + d_hitPlus);

        // 必殺
        a_crt = CalcWeaponValue(a_crt, a_weaponData.critical + a_crtPlus);
        d_crt = CalcWeaponValue(d_crt, d_weaponData.critical + d_crtPlus);
        #endregion

        #region 計算
        ret.a_dmg = a_atk - d_def;
        ret.a_hit = CalcHitRate(a_hit, d_avd);
        ret.a_critical = CalcHitRate(a_crt, d_acrt);
        ret.a_atkCount = CalcAttackCount(a_spd, d_spd);

        if (canCounter)
        {
            ret.d_dmg = d_atk - a_def;
            ret.d_hit = CalcHitRate(d_hit, a_avd);
            ret.d_critical = CalcHitRate(d_crt, a_acrt);
            ret.d_atkCount = CalcAttackCount(d_spd, a_spd);
        }
        else
        {
            ret.d_dmg = 0;
            ret.d_hit = 0;
            ret.d_critical = 0;
            ret.d_atkCount = 0;
        }

        if (ret.a_dmg < 0) ret.a_dmg = 0;
        if (ret.d_dmg < 0) ret.d_dmg = 0;

        #endregion

        return ret;
    }

    /// <summary>
    /// 武器威力の加算
    /// 命中・必殺率も同じ式
    /// </summary>
    /// <param name="strength">素の力</param>
    /// <param name="wpn">武器威力</param>
    /// <returns>実際の攻撃力</returns>
    public static int CalcWeaponValue(int strength, int wpn)
    {
        if (strength < 100) return strength + wpn;

        return strength + strength * wpn / 100;
    }

    /// <summary>
    /// 命中率判定　必殺率も同様
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="avoid"></param>
    /// <returns></returns>
    private static int CalcHitRate(int hit, int avoid)
    {
        // 回避が高ければ0
        if (hit <= avoid) return 0;

        // 命中200までは差分そのまま
        if (hit < 200) return hit - avoid > 100 ? 100 : hit - avoid;

        // 200以上は2倍で100、1倍で0まで線形
        var rate = (hit - avoid) * 100 / avoid;
        return rate > 100 ? 100 : rate;
    }

    /// <summary>
    /// 攻撃回数決定
    /// </summary>
    /// <param name="atkSpd"></param>
    /// <param name="defSpd"></param>
    /// <returns></returns>
    private static int CalcAttackCount(int atkSpd, int defSpd)
    {
        // 遅ければ１回
        if (atkSpd < defSpd) return 1;

        // 20までは＋４で２回
        if (atkSpd <= 20) return atkSpd - defSpd >= 4 ? 2 : 1;

        // 以降は防御側の何％プラスかで判定
        var atkRate = atkSpd * 100 / defSpd - 100;

        // 20%あれば２回、以降120％、220%と100%ずつ追加
        if (atkRate < 20) return 1;
        atkRate -= 20;
        return 2 + atkRate / 100;
    }

    /// <summary>
    /// 戦闘結果パラメータ
    /// </summary>
    public class BattleParameter
    {
        /// <summary>攻撃側１発のダメージ</summary>
        public int a_dmg;
        /// <summary>攻撃側命中率</summary>
        public int a_hit;
        /// <summary>攻撃側クリティカル率</summary>
        public int a_critical;
        /// <summary>攻撃側攻撃回数</summary>
        public int a_atkCount;

        /// <summary>反撃側１発のダメージ</summary>
        public int d_dmg;
        /// <summary>反撃側命中率</summary>
        public int d_hit;
        /// <summary>反撃側クリティカル率</summary>
        public int d_critical;
        /// <summary>反撃側攻撃回数</summary>
        public int d_atkCount;
    }

    #endregion
}

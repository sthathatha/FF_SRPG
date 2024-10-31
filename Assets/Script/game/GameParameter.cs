using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[�����̃p�����[�^�Ǘ��e��@�\
/// </summary>
public class GameParameter
{

    #region �v���C���[�Z�[�u�f�[�^

    public static PlayerSaveParameter Prm_Drows = new();
    public static PlayerSaveParameter Prm_Eraps = new();
    public static PlayerSaveParameter Prm_Exa = new();
    public static PlayerSaveParameter Prm_Worra = new();
    public static PlayerSaveParameter Prm_Koob = new();
    public static PlayerSaveParameter Prm_You = new();

    /// <summary>
    /// �p�����[�^�擾
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
    /// �Z�[�u�f�[�^���v���C���[�L�����p�����[�^
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

        /// <summary>�]����</summary>
        public int ReviveCount;
        /// <summary>�N���X</summary>
        public Constant.ClassID ClassID;

        /// <summary>�񕜑҂��퓬��</summary>
        public int RestBattle;

        /// <summary>�����X�L��</summary>
        public List<int> Skills = new List<int>();

        /// <summary>
        /// Lv1������
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
        /// �Z�[�u�f�[�^�p������쐬
        /// </summary>
        /// <returns></returns>
        public string ToSaveString()
        {
            return "";
        }

        /// <summary>
        /// �Z�[�u�f�[�^����ǂݍ���
        /// </summary>
        /// <param name="data"></param>
        public void ReadString(string data)
        {
        }
    }

    #endregion

    #region �t�B�[���h�p�����[�^

    /// <summary>
    /// �t�B�[���h�L�����N�^�[�p�����[�^
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
        /// �v���C���[�p�����[�^������
        /// </summary>
        /// <param name="pid"></param>
        public void InitPlayer(Constant.PlayerID pid)
        {
            // �Z�[�u�f�[�^����擾
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
        /// �G�p�����[�^�쐬
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
        /// �Z�[�u�f�[�^�p������쐬
        /// </summary>
        /// <returns></returns>
        public string ToSaveString()
        {
            return "";
        }

        /// <summary>
        /// �Z�[�u�f�[�^����ǂݍ���
        /// </summary>
        /// <param name="data"></param>
        public void ReadString(string data)
        {
        }
    }

    #endregion

    #region ���̑��t�B�[���h�O�f�[�^

    /// <summary>
    /// ���̑��t�B�[���h�O�f�[�^
    /// </summary>
    public class OtherData
    {
        #region �Z�[�u�E���[�h

        /// <summary>
        /// �J�n��������
        /// </summary>
        public void Init()
        {
            haveItemList.Clear();
            haveItemList.Add(new HaveItemData(GameDatabase.ItemID.Sword1));
            haveItemList.Add(new HaveItemData(GameDatabase.ItemID.Spear1));
            haveItemList.Add(new HaveItemData(GameDatabase.ItemID.Axe1));
            haveItemList.Add(new HaveItemData(GameDatabase.ItemID.Arrow1));
            haveItemList.Add(new HaveItemData(GameDatabase.ItemID.Book1));
            haveItemList.Add(new HaveItemData(GameDatabase.ItemID.Rod1));
            haveItemList.Add(new HaveItemData(GameDatabase.ItemID.Item1));

            equip_Drows = 0;
            equip_Eraps = 1;
            equip_Exa = 2;
            equip_Worra = 3;
            equip_Koob = 4;
            equip_You = 0;
        }

        /// <summary>
        /// �Z�[�u
        /// </summary>
        public void Save()
        {

        }

        /// <summary>
        /// ���[�h
        /// </summary>
        public void Load()
        {

        }

        #endregion

        #region �����A�C�e���f�[�^

        /// <summary>
        /// �����A�C�e���f�[�^
        /// </summary>
        public class HaveItemData
        {
            /// <summary>ID</summary>
            public GameDatabase.ItemID id;
            /// <summary>�c��g�p��</summary>
            public int useCount;

            /// <summary>
            /// �A�C�e���V�i���擾
            /// </summary>
            /// <param name="}id"></param>
            public HaveItemData(GameDatabase.ItemID _id)
            {
                id = _id;
                var itm = GameDatabase.ItemDataList[(int)id];
                useCount = itm.maxUse;
            }

            /// <summary>�A�C�e���ڍ׎擾</summary>
            public GameDatabase.ItemData ItemData { get { return GameDatabase.ItemDataList[(int)id]; } }
        }

        /// <summary>
        /// �����A�C�e�����X�g
        /// </summary>
        public List<HaveItemData> haveItemList = new List<HaveItemData>();

        #endregion

        #region �e�L��������

        public int equip_Drows;
        public int equip_Eraps;
        public int equip_Exa;
        public int equip_Worra;
        public int equip_Koob;
        public int equip_You;

        /// <summary>
        /// �������擾
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
        /// ������ݒ�
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
        /// �A�C�e���폜
        /// </summary>
        /// <param name="idx"></param>
        public void DeleteItem(int idx)
        {
            //�폜
            haveItemList.RemoveAt(idx);

            // �����i�Ώ�
            // �h���V�[
            if (idx == equip_Drows) equip_Drows = -1;
            else if (idx < equip_Drows) equip_Drows--;

            // ���͏ォ�瓾�ӕ��팟��
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
        /// �����ł�����̂��ォ��T��
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

    #region �퓬�v�Z��

    /// <summary>
    /// �퓬���ʐ��l�̌v�Z
    /// ���ۂ̃_���[�W���͂�����g���Ė�����N���e�B�J�����s��
    /// </summary>
    /// <param name="atkChr"></param>
    /// <param name="defChr"></param>
    /// <returns></returns>
    public static BattleParameter GetBattleParameter(CharacterBase atkChr, CharacterBase defChr)
    {
        var ret = new BattleParameter();

        #region ����f�[�^�擾
        // ����ID
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

        // �h�䑤
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

        // ���푊��
        var advantage = GameDatabase.CheckWeaponAdvantage(a_weaponId, d_weaponId);
        #endregion
        var a_melee = a_weaponData.is_melee();
        var d_melee = d_weaponData.is_melee();

        #region ���ӕ��픻��
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

        #region �X�L���E�����ɂ����Z�l ���퐫�\�Ƀv���X����v�Z
        // �N���e�B�J����
        var a_dmgPlus = 0;
        var d_dmgPlus = 0;
        var a_hitPlus = 0;
        var d_hitPlus = 0;
        var a_crtPlus = 0;
        var d_crtPlus = 0;
        var d_rangePlus = 0;
        // ���푊��
        if (advantage != 0)
        {
            // �U�����L���Ȃ�U�����{�A�������|
            a_dmgPlus += advantage;
            a_hitPlus += advantage * 10;
            d_dmgPlus -= advantage;
            d_hitPlus -= advantage * 10;
        }
        if (atkChr.IsPlayer())
        {
            //todo:�X�L���ɂ���Ė����A����A�K�E������
        }
        if (defChr.IsPlayer())
        {
        }
        #endregion

        #region �����̎˒�����
        var distV = atkChr.GetLocation() - defChr.GetLocation();
        var distance = Math.Abs(distV.x) + Math.Abs(distV.y);
        var canCounter = distance >= d_weaponData.rangeMin && distance <= d_weaponData.rangeMax + d_rangePlus;
        #endregion

        #region �U�����p�����[�^
        var a_atk = a_melee ? atkChr.param.Atk : atkChr.param.Mag;
        var a_def = d_melee ? atkChr.param.Def : atkChr.param.Mdf;
        var a_spd = atkChr.param.Spd;
        var a_hit = atkChr.param.Tec * 2 + atkChr.param.Luk / 2;
        var a_crt = atkChr.param.Tec / 2;
        var a_avd = atkChr.param.Spd * 2;
        var a_acrt = atkChr.param.Luk;
        #endregion

        #region �h�䑤�p�����[�^
        var d_atk = d_melee ? defChr.param.Atk : defChr.param.Mag;
        var d_def = a_melee ? defChr.param.Def : defChr.param.Mdf;
        var d_spd = defChr.param.Spd;
        var d_hit = defChr.param.Tec * 2 + defChr.param.Luk / 2;
        var d_crt = defChr.param.Tec / 2;
        var d_avd = defChr.param.Spd * 2;
        var d_acrt = defChr.param.Luk;
        #endregion

        #region ���퐫�\���Z
        // �͂�100�𒴂����畐��З͂́��ő���
        var aWpnAtk = (a_weaponData.atk + a_dmgPlus) * a_weaponRate / 100;
        var dWpnAtk = (d_weaponData.atk + d_dmgPlus) * d_weaponRate / 100;
        if (a_atk < 100) a_atk += aWpnAtk;
        else a_atk += a_atk * aWpnAtk / 100;
        if (d_atk < 100) d_atk += dWpnAtk;
        else d_atk += d_atk * dWpnAtk / 100;

        // ����
        if (a_hit < 100) a_hit += a_weaponData.hit + a_hitPlus;
        else a_hit += a_hit * (a_weaponData.hit + a_hitPlus) / 100;
        if (d_hit < 100) d_hit += d_weaponData.hit + d_hitPlus;
        else d_hit += d_hit * (d_weaponData.hit + d_hitPlus) / 100;

        // �K�E
        if (a_crt < 100) a_crt += a_weaponData.critical + a_crtPlus;
        else a_crt += a_crt * (a_weaponData.critical + a_crtPlus) / 100;
        if (d_crt < 100) d_crt += d_weaponData.critical + d_crtPlus;
        else d_crt += d_crt * (d_weaponData.critical + d_crtPlus) / 100;
        #endregion

        #region �v�Z
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
    /// ����������@�K�E�������l
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="avoid"></param>
    /// <returns></returns>
    private static int CalcHitRate(int hit, int avoid)
    {
        // ������������0
        if (hit <= avoid) return 0;

        // ����200�܂ł͍������̂܂�
        if (hit < 200) return hit - avoid > 100 ? 100 : hit - avoid;

        // 200�ȏ��2�{��100�A1�{��0�܂Ő��`
        var rate = (hit - avoid) * 100 / avoid;
        return rate > 100 ? 100 : rate;
    }

    /// <summary>
    /// �U���񐔌���
    /// </summary>
    /// <param name="atkSpd"></param>
    /// <param name="defSpd"></param>
    /// <returns></returns>
    private static int CalcAttackCount(int atkSpd, int defSpd)
    {
        // �x����΂P��
        if (atkSpd < defSpd) return 1;

        // 20�܂ł́{�S�łQ��
        if (atkSpd <= 20) return atkSpd - defSpd >= 4 ? 2 : 1;

        // �ȍ~�͖h�䑤�̉����v���X���Ŕ���
        var atkRate = atkSpd * 100 / defSpd - 100;

        // 20%����΂Q��A�ȍ~80���A140%��60%���ǉ�
        if (atkRate < 20) return 1;
        atkRate -= 20;
        return 2 + atkRate / 60;
    }

    /// <summary>
    /// �퓬���ʃp�����[�^
    /// </summary>
    public class BattleParameter
    {
        /// <summary>�U�����P���̃_���[�W</summary>
        public int a_dmg;
        /// <summary>�U����������</summary>
        public int a_hit;
        /// <summary>�U�����N���e�B�J����</summary>
        public int a_critical;
        /// <summary>�U�����U����</summary>
        public int a_atkCount;

        /// <summary>�������P���̃_���[�W</summary>
        public int d_dmg;
        /// <summary>������������</summary>
        public int d_hit;
        /// <summary>�������N���e�B�J����</summary>
        public int d_critical;
        /// <summary>�������U����</summary>
        public int d_atkCount;
    }

    #endregion
}

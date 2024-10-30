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
        /// <summary>�ʒu</summary>
        public Vector2Int Location;

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
            Location = Vector2Int.zero;
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
}

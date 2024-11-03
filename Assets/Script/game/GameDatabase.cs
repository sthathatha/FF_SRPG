using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �萔�f�[�^�x�[�X
/// </summary>
public class GameDatabase
{
    #region �p�����[�^�f�[�^�x�[�X

    /// <summary>
    /// �p�����[�^�\����
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

    /// <summary>�v���C���[�����l</summary>
    public static readonly ParameterData[] Prm_PlayerInit = new ParameterData[] {
        new ParameterData(25, 11, 25, 8, 8, 9, 3, 1, 4),
        new ParameterData(26, 7, 1, 5, 3, 2, 10, 5, 3),
        new ParameterData(21, 9, 3, 9, 8, 2, 11, 2, 4),
        new ParameterData(19, 6, 5, 10, 12, 1, 4, 2, 4),
        new ParameterData(16, 1, 10, 6, 5, 7, 2, 9, 4),
        new ParameterData(20, 7, 5, 11, 10, 5, 6, 4, 4),
    };

    /// <summary>�v���C���[��������</summary>
    public static readonly ParameterData[] Prm_PlayerGrow = new ParameterData[] {
        new ParameterData(70, 130, 0, 45, 35, 40, 25, 10, 0),
        new ParameterData(100, 50, 5, 30, 25, 35, 70, 50, 0),
        new ParameterData(85, 60, 5, 60, 45, 30, 40, 25, 0),
        new ParameterData(70, 35, 20, 60, 75, 35, 20, 30, 0),
        new ParameterData(45, 15, 80, 35, 30, 60, 15, 70, 0),
        new ParameterData(75, 50, 25, 50, 50, 45, 30, 30, 0),
    };

    /// <summary>
    /// �v���C���[�������茋�ʂ��擾
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
    /// �㏸����
    /// </summary>
    /// <param name="rate"></param>
    /// <returns></returns>
    private static int Prm_CalcGrowUp(int rate)
    {
        // 100�ȏ�͕K���オ��
        var upBase = rate / 100;
        rate = rate % 100;
        if (rate <= 0) return upBase;

        return upBase + (Util.RandomCheck(rate) ? 1 : 0);
    }

    /// <summary>�N���X�`�F���W�����l_�h���V�[</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Drows = new ParameterData[] {
        new ParameterData(),
        new ParameterData(1, 2, 0, 3, 4, 1, 1, 7, 1),
        new ParameterData(3, 6, 0, 2, 2, 0, 2, 3, 1),
        new ParameterData(2, 4, 0, 2, 7, 0, 2, 2, 1),
        new ParameterData(4, 2, 0, 4, 2, 0, 4, 3, 0),
        new ParameterData(6, 7, 0, 0, 3, 0, 4, 4, 2),
    };
    /// <summary>�N���X�`�F���W�����l_�G��</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Eraps = new ParameterData[] {
        new ParameterData(),
        new ParameterData(2, 7, 0, 6, 3, 0, 0, 1, 0),
        new ParameterData(0, 8, 0, 6, 7, 0, -3, 0, 2),
        new ParameterData(3, 3, 0, 4, 1, 0, 2, 6, 1),
        new ParameterData(1, 5, 0, 2, 2, 3, 1, 5, 1),
        new ParameterData(0, 4, 0, 4, 5, 0, 1, 4, 2),
    };
    /// <summary>�N���X�`�F���W�����l_�G�O�U</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Exa = new ParameterData[] {
        new ParameterData(),
        new ParameterData(2, 1, 0, 2, 2, 4, 1, 6, 1),
        new ParameterData(5, 4, 0, 1, 3, 1, 3, 1, 1),
        new ParameterData(3, 1, 0, 3, 3, 3, 3, 3, 2),
        new ParameterData(6, 2, 0, 4, 1, 0, 2, 5, 1),
        new ParameterData(3, 5, 0, 1, 4, 0, 4, 2, 1),
    };
    /// <summary>�N���X�`�F���W�����l_�E�[��</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Worra = new ParameterData[] {
        new ParameterData(),
        new ParameterData(3, 3, 0, 5, 1, 0, 5, 1, 1),
        new ParameterData(5, 1, 0, 3, 4, 0, 3, 2, 1),
        new ParameterData(1, 4, 3, 6, 0, 2, 4, 2, 1),
        new ParameterData(3, 5, 1, 4, 2, 0, 1, 3, 1),
        new ParameterData(1, 7, 0, 5, 6, -2, 1, 1, 2),
    };
    /// <summary>�N���X�`�F���W�����l_�N�[</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_Koob = new ParameterData[] {
        new ParameterData(),
        new ParameterData(5, 0, 2, 3, 2, 0, 2, 4, 1),
        new ParameterData(2, 0, 6, 2, 1, 0, 5, 2, 1),
        new ParameterData(2, 0, 6, 4, 2, 0, 2, 3, 1),
        new ParameterData(3, 0, 3, 5, 4, 0, 2, 2, 1),
        new ParameterData(4, 0, 2, 1, 4, 0, 5, 3, 1),
    };
    /// <summary>�N���X�`�F���W�����l_�I</summary>
    public static readonly ParameterData[] Prm_ClassChangeGrow_You = new ParameterData[] {
        new ParameterData(),
        new ParameterData(2, 5, 0, 4, 1, 0, 4, 2, 1),
        new ParameterData(3, 4, 0, 2, 4, 0, 1, 4, 1),
        new ParameterData(0, 3, 0, 3, 5, 0, 3, 5, 1),
        new ParameterData(2, 2, 0, 6, 6, 0, 1, 2, 1),
        new ParameterData(4, 7, 0, 4, 0, 0, 1, 3, 1),
    };

    /// <summary>�N���X�`�F���W�����l�擾</summary>
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

    #region �N���X���Ƃ̕���{��

    /// <summary>
    /// ����̔{��
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

    /// <summary>�N���X������{��_�h���V�[</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Drows = new WeaponRate[]
    {
        new (100, 30, 30, 10, 10, 10),
        new (120, 30, 30, 10, 10, 10),
        new (100, 50, 100, 30, 10, 10),
        new (150, 80, 80, 10, 10, 10),
        new (120, 120, 120, 50, 10, 10),
        new (200, 0, 0, 0, 0, 0),
    };
    /// <summary>�N���X������{��_�G��</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Eraps = new WeaponRate[]
    {
        new (20, 100, 20, 20, 10, 50),
        new (50, 100, 50, 20, 10, 50),
        new (20, 120, 20, 20, 10, 50),
        new (100, 100, 100, 50, 10, 50),
        new (100, 120, 50, 50, 10, 100),
        new (50, 150, 50, 20, 10, 50),
    };
    /// <summary>�N���X������{��_�G�O�U</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Exa = new WeaponRate[]
    {
        new (10, 50, 100, 10, 30, 30),
        new (10, 50, 100, 10, 30, 30),
        new (10, 50, 120, 10, 30, 30),
        new (50, 100, 100, 50, 30, 50),
        new (100, 50, 100, 10, 30, 30),
        new (10, 50, 150, 10, 30, 30),
    };
    /// <summary>�N���X������{��_�E�[��</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Worra = new WeaponRate[]
    {
        new (10, 10, 10, 100, 10, 50),
        new (10, 10, 10, 120, 30, 50),
        new (10, 10, 10, 100, 30, 100),
        new (10, 10, 10, 150, 30, 50),
        new (50, 10, 10, 120, 30, 100),
        new (100, 10, 10, 100, 30, 100),
    };
    /// <summary>�N���X������{��_�N�[</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Koob = new WeaponRate[]
    {
        new (10, 10, 10, 10, 100, 100),
        new (10, 10, 10, 10, 130, 100),
        new (10, 10, 10, 10, 100, 130),
        new (10, 10, 10, 10, 150, 100),
        new (10, 10, 10, 10, 130, 130),
        new (10, 10, 10, 10, 100, 150),
    };
    /// <summary>�N���X������{��_�I</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_You = new WeaponRate[]
    {
        new (110, 10, 10, 10, 50, 50),
        new (130, 10, 10, 10, 50, 50),
        new (120, 10, 10, 10, 50, 50),
        new (150, 10, 10, 10, 50, 50),
        new (120, 120, 10, 10, 50, 50),
        new (130, 10, 100, 10, 50, 50),
    };

    /// <summary>�N���X�`�F���W�����l�擾</summary>
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

    #region �G
    /// <summary>�GLv1</summary>
    public static readonly ParameterData[] Prm_EnemyInit = new ParameterData[]
    {
        new ParameterData(16, 4, 0, 2, 1, 4, 5, 0, 4), //�O���[���X���C��
    };

    /// <summary>�G��������</summary>
    public static readonly ParameterData[] Prm_EnemyGrow = new ParameterData[]
    {
        new ParameterData(50, 30, 1, 45, 35, 40, 25, 10, 0), //�O���[���X���C��
    };

    #endregion

    #region ���̃f�[�^�x�[�X

    /// <summary>�v���C���[��</summary>
    public static readonly string[] Name_Players =
    {
        "�h���V�[",
        "�G��",
        "�G�O�U",
        "�E�[��",
        "�N�[",
        "�I",
    };

    /// <summary>�N���X��_�h���V�[</summary>
    public static readonly string[] Name_Classes_Drows = {
        "�V�q��",
        "�\�[�h���C�f��",
        "�X�g���C�J�[",
        "�g���l�C�h",
        "���@�C�L���O",
        "�o�[�T�[�J�[",
    };
    /// <summary>�N���X��_�G��</summary>
    public static readonly string[] Name_Classes_Eraps = {
        "�i�C�g",
        "�w�r�[�i�C�g",
        "�����T�[",
        "���C�����K�[�h",
        "�p���f�B��",
        "�h���O�[��",
    };
    /// <summary>�N���X��_�G�O�U</summary>
    public static readonly string[] Name_Classes_Exa = {
        "�R��",
        "���C���h�K�[�h",
        "����",
        "�A�[�N�i�C�g",
        "�R�}���_�[",
        "�w���N���X",
    };
    /// <summary>�N���X��_�E�[��</summary>
    public static readonly string[] Name_Classes_Worra = {
        "�n���^�[",
        "�X�i�C�p�[",
        "�X�J�E�g",
        "�A���e�~�X",
        "�X�^�[�Q�C�U�[",
        "�A�T�V��",
    };
    /// <summary>�N���X��_�N�[</summary>
    public static readonly string[] Name_Classes_Koob = {
        "�����m",
        "�v���[�X�e�X",
        "�h���C�h",
        "�v���C��",
        "����",
        "�t�@�i�e�B�b�N",
    };
    /// <summary>�N���X��_�I</summary>
    public static readonly string[] Name_Classes_You = {
        "�Q�l",
        "��",
        "�Ҏa��",
        "�鍳�P",
        "�h�q",
        "�C��",
    };

    /// <summary>
    /// �N���X���z��擾
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

    /// <summary>�X�L����</summary>
    public static readonly string[] Name_Skills =
    {

    };

    /// <summary>�G��</summary>
    public static readonly string[] Name_Enemies =
    {
        "�O���[���X���C��",
        "�X�P���g��",
    };

    #endregion

    #region �A�C�e���f�[�^�x�[�X

    /// <summary>
    /// �A�C�e�����
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
    /// �A�C�e���p�����[�^�\����
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
        public ItemData(ItemType it, string nm, int use, int a, int hi, int rMin, int rMax, int crt = 0)
        {
            iType = it; name = nm; maxUse = use;
            atk = a; hit = hi; rangeMin = rMin; rangeMax = rMax; critical = crt;
        }

        /// <summary>
        /// �������픻��
        /// </summary>
        /// <returns></returns>
        public bool is_melee()
        {
            return iType != ItemType.Book;
        }
    }

    /// <summary>�A�C�e��ID</summary>
    public enum ItemID
    {
        FreeHand = 0,
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
    }

    /// <summary>
    /// �A�C�e���f�[�^
    /// </summary>
    public static readonly ItemData[] ItemDataList = new ItemData[]
    {
        new(ItemType.None, "�f��", -1, 0, 100, 1, 1),

        new(ItemType.Rod, "���t�̏�", 30, 10, 100, 1, 1),
        new(ItemType.Item, "�G���N�T�[", 3, 100, 100, 0, 0),

        new(ItemType.Sword, "�S�̌�", 40, 5, 85, 1, 1),
        new(ItemType.Spear, "�S�̑�", 40, 7, 70, 1, 1),
        new(ItemType.Axe, "�S�̕�", 40, 8, 65, 1, 1),
        new(ItemType.Arrow, "�S�̋|", 40, 6, 80, 2, 3),
        new(ItemType.Book, "�t�@�C�A�[", 40, 5, 95, 1, 2),

        new(ItemType.Sword, "��̌�", 20, 13, 75, 1, 1),
        new(ItemType.Spear, "��̑�", 20, 14, 65, 1, 1),
        new(ItemType.Axe, "��̕�", 20, 15, 55, 1, 1),
        new(ItemType.Arrow, "��̋|", 20, 13, 70, 2, 3),
        new(ItemType.Book, "�u���U�[�h", 15, 13, 80, 1, 2),

        new(ItemType.Sword, "�L���\�[�h", 20, 9, 80, 1, 1, 30),
        new(ItemType.Spear, "�L���[�����X", 20, 10, 75, 1, 1, 30),
        new(ItemType.Axe, "�L���[�A�N�X", 20, 11, 65, 1, 1, 30),
        new(ItemType.Arrow, "�L���[�{�E", 20, 9, 80, 2, 3, 30),
        new(ItemType.Book, "�L���O����", 20, 8, 70, 1, 2, 30),
    };

    /// <summary>
    /// ���푊���`�F�b�N
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    /// <returns>1:�U�����L���@-1:�h�䑤�L���@0:�����Ȃ�</returns>
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

    #endregion
}

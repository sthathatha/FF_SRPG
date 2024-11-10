using System.Collections.Generic;
using System.Linq;
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
        new ParameterData(70, 130, 0, 45, 40, 50, 30, 10, 0),
        new ParameterData(100, 45, 25, 40, 20, 30, 60, 45, 0),
        new ParameterData(80, 60, 20, 55, 40, 20, 35, 20, 0),
        new ParameterData(60, 40, 25, 50, 65, 40, 25, 40, 0),
        new ParameterData(50, 15, 60, 40, 30, 60, 15, 50, 0),
        new ParameterData(65, 45, 30, 45, 55, 45, 30, 30, 0),
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
        new ParameterData(1, 3, 0, 2, 4, 3, 1, 5, 1),
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
        new ParameterData(1, 3, 3, 6, 0, 2, 3, 2, 1),
        new ParameterData(3, 5, 1, 4, 2, 0, 1, 3, 1),
        new ParameterData(1, 5, 0, 5, 6, 0, 1, 1, 2),
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
        new (250, 0, 0, 0, 0, 0),
    };
    /// <summary>�N���X������{��_�G��</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Eraps = new WeaponRate[]
    {
        new (20, 100, 20, 20, 10, 50),
        new (50, 100, 50, 50, 10, 50),
        new (20, 120, 20, 20, 10, 50),
        new (100, 100, 100, 50, 10, 50),
        new (100, 120, 50, 50, 10, 100),
        new (50, 150, 50, 20, 10, 50),
    };
    /// <summary>�N���X������{��_�G�O�U</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Exa = new WeaponRate[]
    {
        new (10, 50, 100, 10, 30, 30),
        new (50, 50, 100, 10, 30, 30),
        new (10, 50, 120, 10, 30, 30),
        new (50, 100, 100, 50, 100, 50),
        new (100, 50, 120, 10, 30, 30),
        new (10, 50, 150, 10, 30, 30),
    };
    /// <summary>�N���X������{��_�E�[��</summary>
    public static readonly WeaponRate[] Prm_ClassWeaponRates_Worra = new WeaponRate[]
    {
        new (10, 10, 10, 100, 10, 50),
        new (10, 10, 10, 120, 50, 50),
        new (50, 10, 10, 100, 30, 100),
        new (10, 10, 10, 150, 100, 50),
        new (70, 30, 30, 120, 100, 100),
        new (100, 10, 100, 100, 70, 100),
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
        new (120, 10, 10, 10, 50, 50),
        new (150, 10, 50, 10, 50, 50),
        new (120, 10, 50, 50, 50, 50),
        new (180, 10, 50, 10, 50, 50),
        new (120, 120, 70, 50, 50, 50),
        new (150, 10, 100, 100, 50, 50),
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
        new(15, 7, 0, 4, 3, 6, 8, 1, 4),        // �O���[���X���C��
        new(18, 5, 0, 7, 5, 3, 5, 3, 4),        // �X�P���g���i���j
        new(18, 6, 0, 4, 4, 2, 11, 1, 3),       // �X�P���g���i���j
        new(18, 7, 0, 3, 4, 2, 7, 3, 4),        // �X�P���g���i���j
        new(15, 5, 0, 8, 7, 2, 5, 3, 4),        // �X�P���g���i�|�j
        new(13, 0, 6, 7, 5, 2, 4, 8, 4),        // �X�P���g���i���j
        new(9, 0, 9, 3, 7, 8, 1, 10, 5),        // �y��
        new(12, 14, 0, 9, 6, 5, 6, 1, 4),       // �o�[���X���C��
        new(11, 10, 0, 7, 12, 7, 5, 1, 6),      // �G���L�X���C��
        new(17, 0, 10, 5, 5, 5, 4, 8, 3),       // �f�B�[�v�X���C��
        new(24, 12, 0, 10, 2, 8, 20, 14, 1),    // ���^���X���C��
        new(20, 11, 0, 9, 6, 6, 5, 5, 4),       // �G���W�F���X
        new(20, 11, 0, 9, 6, 7, 5, 5, 4),       // �A�[�N�G���W�F���X
        new(20, 11, 0, 9, 6, 8, 6, 7, 4),       // �v�����V�p���e�B�[�Y
        new(20, 12, 0, 9, 7, 8, 7, 5, 5),       // �p���[�Y
        new(20, 12, 0, 10, 7, 9, 7, 5, 5),      // ���@�[�`���[�Y
        new(20, 12, 0, 10, 7, 9, 8, 6, 5),      // �h�~�j�I���Y
        new(20, 12, 0, 10, 7, 10, 6, 8, 6),     // �g���E���Y
        new(20, 0, 13, 10, 8, 10, 8, 9, 6),     // �P���r��
        new(20, 13, 0, 10, 8, 10, 9, 8, 6),     // �Z���t�B��
        new(20, 14, 14, 11, 8, 11, 9, 9, 7),    // �w�������́x
    };

    /// <summary>�G��������</summary>
    public static readonly ParameterData[] Prm_EnemyGrow = new ParameterData[]
    {
        new(75, 35, 1, 20, 15, 30, 40, 5, 0),     // �O���[���X���C��
        new(90, 25, 1, 35, 25, 15, 25, 15, 0),    // �X�P���g���i���j
        new(90, 30, 1, 20, 20, 10, 55, 5, 0),     // �X�P���g���i���j
        new(90, 35, 1, 15, 20, 10, 35, 15, 0),    // �X�P���g���i���j
        new(75, 25, 1, 40, 35, 10, 25, 15, 0),    // �X�P���g���i�|�j
        new(65, 1, 30, 35, 25, 10, 20, 40, 0),    // �X�P���g���i���j
        new(45, 1, 45, 15, 35, 40, 5, 50, 0),     // �y��
        new(60, 70, 1, 45, 30, 25, 30, 5, 0),     // �o�[���X���C��
        new(55, 50, 1, 35, 60, 35, 25, 5, 0),     // �G���L�X���C��
        new(95, 1, 65, 30, 15, 15, 25, 35, 0),    // �f�B�[�v�X���C��
        new(120, 60, 1, 50, 10, 40, 100, 70, 0),  // ���^���X���C��
        new(100, 55, 1, 45, 30, 30, 25, 25, 0),   // �G���W�F���X
        new(100, 55, 1, 45, 30, 35, 25, 25, 0),   // �A�[�N�G���W�F���X
        new(100, 55, 1, 45, 30, 40, 30, 35, 0),   // �v�����V�p���e�B�[�Y
        new(100, 60, 1, 45, 35, 40, 35, 25, 0),   // �p���[�Y
        new(100, 60, 1, 50, 35, 45, 35, 25, 0),   // ���@�[�`���[�Y
        new(100, 60, 1, 50, 35, 45, 40, 30, 0),   // �h�~�j�I���Y
        new(100, 60, 1, 50, 35, 50, 30, 40, 0),   // �g���E���Y
        new(100, 1, 65, 50, 40, 50, 40, 45, 0),   // �P���r��
        new(100, 65, 1, 50, 40, 50, 45, 40, 0),   // �Z���t�B��
        new(100, 70, 70, 55, 40, 55, 45, 45, 0),  // �w�������́x
    };

    /// <summary>
    /// �G��p���̑��ݒ�
    /// </summary>
    public struct EnemyOtherData
    {
        /// <summary>�g�p����^�C�v</summary>
        public ItemType defaultWeaponType;
        /// <summary>���A���e�B</summary>
        public int rarelity;
        /// <summary>�\���F</summary>
        public Color modelColor;

        public EnemyOtherData(ItemType weapon, int rare, Color? col = null)
        {
            defaultWeaponType = weapon;
            rarelity = rare;
            modelColor = col.HasValue ? col.Value : Color.white;
        }
    }

    /// <summary>�G����p�����[�^</summary>
    public static readonly EnemyOtherData[] Prm_EnemyOther = new EnemyOtherData[]
    {
        new(ItemType.None, 1, new Color(0.4f, 1, 0.4f)),      // �O���[���X���C��
        new(ItemType.Sword, 3),         // �X�P���g���i���j
        new(ItemType.Spear, 4),         // �X�P���g���i���j
        new(ItemType.Axe, 2),           // �X�P���g���i���j
        new(ItemType.Arrow, 3),         // �X�P���g���i�|�j
        new(ItemType.Book, 5),          // �X�P���g���i���j
        new(ItemType.None, 7),          // �y��
        new(ItemType.None, 21, new Color(1, 0.3f, 0.1f)),     // �o�[���X���C��
        new(ItemType.None, 21, new Color(1, 1, 0f)),          // �G���L�X���C��
        new(ItemType.None, 21, new Color(0, 0, 0.8f)),        // �f�B�[�v�X���C��
        new(ItemType.None, 31, new Color(0.7f, 0.7f, 0.7f)),  // ���^���X���C��
        new(ItemType.Arrow, 101),       // �G���W�F���X
        new(ItemType.Sword, 104),       // �A�[�N�G���W�F���X
        new(ItemType.Spear, 107),       // �v�����V�p���e�B�[�Y
        new(ItemType.Axe, 110),         // �p���[�Y
        new(ItemType.Arrow, 113),       // ���@�[�`���[�Y
        new(ItemType.Spear, 116),       // �h�~�j�I���Y
        new(ItemType.Axe, 119),         // �g���E���Y
        new(ItemType.Book, 121),        // �P���r��
        new(ItemType.Sword, 124),       // �Z���t�B��
        new(ItemType.Book, 127),        // �w�������́x
    };

    /// <summary>
    /// �����_���o���̓G��I��
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

            // rarelity�t���A�܂ł͏o�����Ȃ�
            if (data.rarelity > floor) continue;
            // �{�X��100�ȏ�
            if (isBoss && data.rarelity <= 100) continue;

            koho.Add((Constant.EnemyID)i);
        }
        if (koho.Count == 0) return Constant.EnemyID.SlimeGreen;

        // ��₩��rarelity�ɂ���Ċm���Ŕ���
        var kohoRarelity = koho.Select(id => floor - Prm_EnemyOther[(int)id].rarelity + 1).ToList();
        var selIdx = SelectRateIndex(kohoRarelity);

        return koho[selIdx];
    }

    #endregion

    #region ���̃f�[�^�x�[�X

    // ���̏ꏊ�ɂ���e�L�X�g
    // �s���@�ҋ@�@�P��
    // �U���@�����@�K�E
    // 
    // �́@���́@�U���@�Z�@�����@�K�^�@����@���h�@�ړ�
    // ����ɓ��ꂽ
    // ����ꂽ

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

    /// <summary>�G��</summary>
    public static readonly string[] Name_Enemies =
    {
        "�O���[���X���C��",
        "�X�P���g��",
        "�X�P���g��",
        "�X�P���g��",
        "�X�P���g��",
        "�X�P���g��",
        "�y��",
        "�o�[���X���C��",
        "�G���L�X���C��",
        "�f�B�[�v�X���C��",
        "���^���X���C��",
        "�G���W�F���X",
        "�A�[�N�G���W�F���X",
        "�v�����V�p���e�B�[�Y",
        "�p���[�Y",
        "���@�[�`���[�Y",
        "�h�~�j�I���Y",
        "�g���E���Y",
        "�P���r��",
        "�Z���t�B��",
        "�w�������́x",
    };

    #endregion

    #region �X�L���f�[�^�x�[�X

    /// <summary>
    /// �X�L��ID
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
    /// �X�L���f�[�^
    /// </summary>
    public struct SkillData
    {
        /// <summary>������</summary>
        public string detail;
        /// <summary>�K���v���C���[</summary>
        public Constant.PlayerID getPlayer;
        /// <summary>�K���N���X</summary>
        public Constant.ClassID getClass;
        /// <summary>�K�����x��</summary>
        public int getLevel;
        /// <summary>�]���ŕێ�����t���O</summary>
        public bool canKeep;

        public SkillData(Constant.PlayerID pid, Constant.ClassID cid, int lv, string det, bool keep = true)
        {
            detail = det; getPlayer = pid; getLevel = lv; getClass = cid; canKeep = keep;
        }
    }

    /// <summary>
    /// �X�L���f�[�^
    /// </summary>
    public static readonly SkillData[] SkillDataList = new SkillData[]
    {
        new (Constant.PlayerID.Drows, Constant.ClassID.Base, 1, "�f��ōU���ł���"),
        new (Constant.PlayerID.Drows, Constant.ClassID.Base, 1, "����̏���ʂ�2�{�ɂȂ�"),
        new (Constant.PlayerID.Drows, Constant.ClassID.A2, 10, "�ꌂ�œG��|�������A�čs������"),
        new (Constant.PlayerID.Drows, Constant.ClassID.AB, 10, "��������U�����鎞�͕��킪���Ղ��Ȃ�"),
        new (Constant.PlayerID.Drows, Constant.ClassID.B2, 1, "�߂��ɓG������Ǝ����I�ɍU������(�p���s��)", false),
        new (Constant.PlayerID.Drows, Constant.ClassID.B2, 1, "�K�E��+30"),
        new (Constant.PlayerID.Eraps, Constant.ClassID.A2, 10, "�G�͗אڃ}�X��ʉ߂ł��Ȃ�"),
        new (Constant.PlayerID.Eraps, Constant.ClassID.AB, 10, "�t���A��1�^�[���ڂ݈̂ړ���+4"),
        new (Constant.PlayerID.Eraps, Constant.ClassID.B2, 10, "�����Ɋ֌W�Ȃ������ł���"),
        new (Constant.PlayerID.Exa, Constant.ClassID.A2, 10, "����2�}�X�̖����̖h��+30��"),
        new (Constant.PlayerID.Exa, Constant.ClassID.AB, 10, "����2�}�X�̖����̖����A����A�K�E+20"),
        new (Constant.PlayerID.Exa, Constant.ClassID.B2, 10, "����2�}�X�ɖ��������Ȃ����U���A�h��+10"),
        new (Constant.PlayerID.Worra, Constant.ClassID.A2, 10, "�ő�˒�+1"),
        new (Constant.PlayerID.Worra, Constant.ClassID.AB, 10, "�ړ�+1"),
        new (Constant.PlayerID.Worra, Constant.ClassID.B2, 10, "���+30"),
        new (Constant.PlayerID.Koob, Constant.ClassID.A2, 10, "�K�����푊�����L���ɂȂ�"),
        new (Constant.PlayerID.Koob, Constant.ClassID.AB, 10, "�擾�o���l+50��"),
        new (Constant.PlayerID.Koob, Constant.ClassID.B2, 10, "������󂵂����A�H�ɓ���̕���𐶐�����"),
        new (Constant.PlayerID.You, Constant.ClassID.A2, 10, "��������U�����鎞�A�U��+10"),
        new (Constant.PlayerID.You, Constant.ClassID.AB, 10, "��������U�����鎞�A��������Ȃ�"),
        new (Constant.PlayerID.You, Constant.ClassID.B2, 10, "�������鎞�A�U����+1"),
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
        public int rarelity;
        public ItemData(ItemType it, string nm, int rare, int use, int a, int hi, int rMin, int rMax, int crt = 0)
        {
            iType = it; name = nm; maxUse = use;
            atk = a; hit = hi; rangeMin = rMin; rangeMax = rMax; critical = crt;
            rarelity = rare;
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
    /// �A�C�e���f�[�^
    /// </summary>
    public static readonly ItemData[] ItemDataList = new ItemData[]
    {
        new(ItemType.None,  "�f��",               -1, -1, 0, 100, 1, 1, 0),
        new(ItemType.Book,  "�f��i���j",         -1, -1, 0, 100, 1, 2, 0),
        new(ItemType.Item,  "�G���N�T�[",         1, 3, 100, 100, 0, 0, 0),
        new(ItemType.Rod,   "���t�̏�",           1, 30, 10, 100, 1, 1, 0),
        new(ItemType.Sword, "�S�̌�",             1, 40, 5, 85, 1, 1, 0),
        new(ItemType.Spear, "�S�̑�",             1, 40, 7, 70, 1, 1, 0),
        new(ItemType.Axe,   "�S�̕�",             1, 40, 8, 65, 1, 1, 0),
        new(ItemType.Arrow, "�S�̋|",             1, 40, 6, 80, 2, 3, 0),
        new(ItemType.Book,  "�t�@�C�A�[",         1, 40, 5, 95, 1, 2, 0),
        new(ItemType.Sword, "��̌�",             15, 20, 13, 75, 1, 1, 0),
        new(ItemType.Spear, "��̑�",             15, 20, 14, 65, 1, 1, 0),
        new(ItemType.Axe,   "��̕�",             15, 20, 15, 55, 1, 1, 0),
        new(ItemType.Arrow, "��̋|",             15, 20, 13, 70, 2, 3, 0),
        new(ItemType.Book,  "�u���U�[�h",         15, 20, 13, 80, 1, 2, 0),
        new(ItemType.Sword, "�L���\�[�h",         51, 20, 9, 80, 1, 1, 30),
        new(ItemType.Spear, "�L���[�����X",       51, 20, 10, 75, 1, 1, 30),
        new(ItemType.Axe,   "�L���[�A�N�X",       51, 20, 11, 65, 1, 1, 30),
        new(ItemType.Arrow, "�L���[�{�E",         51, 20, 9, 80, 2, 3, 30),
        new(ItemType.Book,  "�L���O����",         51, 20, 8, 90, 1, 2, 30),
        new(ItemType.Sword, "���L�t�O�X",         101, 40, 23, 80, 1, 2, 15),
        new(ItemType.Spear, "�A�K���A���v�g",     101, 40, 18, 100, 1, 2, 15),
        new(ItemType.Axe,   "�T�^�i�L�A",         101, 40, 19, 90, 1, 2, 15),
        new(ItemType.Arrow, "�t���[���e�B",       101, 40, 18, 85, 2, 4, 15),
        new(ItemType.Book,  "�A�b�s���̐Ԃ��{",   101, 40, 21, 95, 1, 3, 15),
        new(ItemType.Sword, "���@����",           151, 40, 16, 120, 1, 1, 60),
        new(ItemType.Spear, "�f�B�X���v�^�[",     151, 40, 27, 65, 1, 3, 0),
        new(ItemType.Axe,   "�t���[�X���F���O",   151, 40, 22, 100, 1, 1, 20),
        new(ItemType.Arrow, "�}���h�D�[�N",       151, 40, 15, 100, 2, 6, 0),
        new(ItemType.Book,  "�l�N���m�~�R��",     151, 40, 36, 40, 1, 2, 0),

        new(ItemType.None, "�f��", -1, -1, 0, 100, 1, 1),
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

    /// <summary>
    /// �����_���A�C�e������
    /// </summary>
    /// <param name="floor">�t���A�ԍ�</param>
    /// <param name="boss">�{�X�ł���</param>
    /// <param name="weaponOnly">true:����̂�</param>
    /// <param name="type">None�ȊO���w�肷��ƃ^�C�v���w��</param>
    /// <param name="outRate">�n�Y���ɂȂ�m��0�`100</param>
    /// <returns></returns>
    public static ItemID CalcRandomItem(int floor, bool boss, bool weaponOnly = true, ItemType type = ItemType.None, int outRate = 0)
    {
        if (Util.RandomCheck(outRate)) return ItemID.FreeHand;

        var bossRate = boss ? 2 : 1; // �{�X�̓��A���e�B�����遁���A�������̂��o�₷��

        // ����Ȃ�ł��̏ꍇ�A�����ĂȂ����̗D��
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

            // rarelity�t���A�܂ł͏o�����Ȃ�
            if (data.rarelity > floor) continue;
            koho.Add((ItemID)i);
        }
        if (koho.Count == 0) return ItemID.FreeHand;

        // ��₩��rarelity�ɂ���Ċm���Ŕ���
        var kohoRarelity = koho.Select(id => floor - ItemDataList[(int)id].rarelity / bossRate + 1).ToList();
        var selIdx = SelectRateIndex(kohoRarelity);

        return koho[selIdx];
    }

    #endregion

    #region �v�Z

    /// <summary>
    /// �I�΂�₷��int�̃��X�g �� �����_���łǂꂩ��I��
    /// </summary>
    /// <param name="rateList"></param>
    /// <returns>�I�ԃC���f�b�N�X</returns>
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

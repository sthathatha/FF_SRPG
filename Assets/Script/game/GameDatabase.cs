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
        new ParameterData(70, 120, 0, 45, 35, 40, 25, 10, 0),
        new ParameterData(100, 50, 5, 30, 25, 35, 70, 50, 0),
        new ParameterData(85, 60, 5, 60, 45, 30, 40, 25, 0),
        new ParameterData(70, 35, 20, 60, 75, 35, 20, 30, 0),
        new ParameterData(45, 15, 80, 35, 30, 60, 15, 70, 0),
        new ParameterData(75, 50, 25, 50, 50, 45, 30, 30, 0),
    };

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
}

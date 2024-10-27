using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �萔
/// </summary>
public class Constant
{
    #region ����

    /// <summary>
    /// �T�C���J�[�u
    /// </summary>
    public enum SinCurveType : int
    {
        /// <summary>����</summary>
        Accel = 0,
        /// <summary>����</summary>
        Decel,
        /// <summary>������</summary>
        Both,
    }

    /// <summary>
    /// ��ʕ�
    /// </summary>
    public const float SCREEN_WIDTH = 960f;
    /// <summary>
    /// ��ʍ���
    /// </summary>
    public const float SCREEN_HEIGHT = 540f;

    /// <summary>
    /// ����
    /// </summary>
    public enum Direction : int
    {
        None = 0,
        Up,
        Down,
        Left,
        Right,
    }

    #endregion

    #region �Q�[���ŗL

    /// <summary>
    /// �v���C���[�L����ID
    /// </summary>
    public enum PlayerID
    {
        Drows = 0,
        Eraps,
        Exa,
        Worra,
        Koob,
        You,
    }

    /// <summary>
    /// �N���XID
    /// </summary>
    public enum ClassID
    {
        Base = 0,
        A,
        B,
        A2,
        AB,
        B2,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SkillID
    {

    }

    /// <summary>
    /// �G�L����ID
    /// </summary>
    public enum EnemyID
    {
        GreenSlime = 0,
    }

    #endregion
}

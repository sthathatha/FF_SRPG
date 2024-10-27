using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �L�����N�^�[���ʋ@�\
/// </summary>
public class CharacterBase : MonoBehaviour
{
    #region �����o�[

    /// <summary>�t�B�[���h</summary>
    public FieldSystem field;

    /// <summary>���f��</summary>
    public Animator anim;

    #endregion

    #region �ϐ�

    protected Vector2Int location;

    public GameParameter.FieldCharacterParameter param { get; protected set; } = new();

    #endregion

    #region �ʒu

    /// <summary>
    /// �ʒu�ݒ�
    /// </summary>
    /// <param name="loc"></param>
    public void SetLocation(Vector2Int loc)
    {
        location = loc;
        transform.localPosition = field.GetCellPosition(loc);
    }

    /// <summary>
    /// �ʒu�擾
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetLocation() { return location; }

    #endregion

    #region �p�����[�^

    /// <summary>�v���C���[���ǂ���</summary>
    /// <returns></returns>
    virtual public bool IsPlayer() { return false; }

    #endregion
}

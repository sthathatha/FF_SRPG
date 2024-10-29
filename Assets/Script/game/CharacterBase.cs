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

    /// <summary>���ݍ��W</summary>
    protected Vector2Int location;

    /// <summary>�p�����[�^</summary>
    public GameParameter.FieldCharacterParameter param { get; protected set; } = new();

    /// <summary>�^�[���s����</summary>
    public bool turnActable { get; protected set; } = true;

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

    /// <summary>
    /// �s�����ݒ�
    /// </summary>
    /// <param name="a"></param>
    public void SetActable(bool a)
    {
        turnActable = a;
        if (IsPlayer())
        {
            anim.GetComponent<SpriteRenderer>().color = a ? Color.white : Color.gray;
        }
    }

    #endregion

    #region �A�j���[�V����

    /// <summary>
    /// �A�j���[�V�����Đ�
    /// </summary>
    /// <param name="dir"></param>
    virtual public void PlayAnim(Constant.Direction dir)
    {
        // �G�̏ꍇ�ҋ@�ƈړ�����
        if (dir == Constant.Direction.None)
            anim.Play("idle");
        else
            anim.Play("move");
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="history"></param>
    /// <returns></returns>
    public IEnumerator Walk(FieldSystem.MoveHistory history)
    {
        var p = new DeltaVector3();
        p.Set(transform.localPosition);
        // �������Ɉړ�
        for (var i = 0; i < history.history.Count - 1; ++i)
        {
            var nextLoc = history.history[i + 1];
            var dist = nextLoc - history.history[i];
            PlayAnim(Util.GetDirectionFromVec(new Vector3(dist.x, dist.y)));
            p.MoveTo(field.GetCellPosition(nextLoc), 0.1f, DeltaFloat.MoveType.LINE);
            while (p.IsActive())
            {
                yield return null;
                transform.localPosition = p.Get();
            }
        }
    }

    #endregion
}

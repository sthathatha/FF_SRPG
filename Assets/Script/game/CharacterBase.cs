using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �L�����N�^�[���ʋ@�\
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    #region �����o�[

    /// <summary>�t�B�[���h</summary>
    public FieldSystem field;

    /// <summary>���f��</summary>
    public Animator anim;

    /// <summary>HP�Q�[�W</summary>
    public HPGauge hpGauge;

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
        if (!a)
        {
            PlayAnim(Constant.Direction.None);
        }
    }

    /// <summary>
    /// �ŏ��˒�
    /// </summary>
    /// <returns></returns>
    abstract public int GetRangeMin();
    /// <summary>
    /// �ő�˒�
    /// </summary>
    /// <returns></returns>
    abstract public int GetRangeMax();

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

    /// <summary>
    /// HP�Q�[�W�X�V
    /// </summary>
    /// <param name="immediate">true:�����X�V�@false:�A�j���[�V����</param>
    public void UpdateHP(bool immediate = false)
    {
        if (immediate)
            hpGauge.SetHP(param.HP, param.MaxHP);
        else
            hpGauge.AnimHP(param.HP, param.MaxHP);
    }

    /// <summary>
    /// �U���A�j���[�V����
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator AttackAnim(Vector3 dist, float time = 0.3f)
    {
        var basePos = transform.localPosition;
        var addP = new DeltaVector3();
        addP.Set(Vector3.zero);
        addP.MoveTo(dist.normalized * 20f, time / 2f, DeltaFloat.MoveType.DECEL);
        while (addP.IsActive())
        {
            yield return null;
            addP.Update(Time.deltaTime);
            transform.localPosition = basePos + addP.Get();
        }
        addP.MoveTo(Vector3.zero, time / 2f, DeltaFloat.MoveType.ACCEL);
        while (addP.IsActive())
        {
            yield return null;
            addP.Update(Time.deltaTime);
            transform.localPosition = basePos + addP.Get();
        }
    }

    /// <summary>
    /// ���S�A�j���[�V����
    /// </summary>
    /// <returns></returns>
    public IEnumerator DeathAnim()
    {
        var model = anim.GetComponent<SpriteRenderer>();
        var col = model.color;
        var a = new DeltaFloat();
        a.Set(1f);
        a.MoveTo(0f, 0.5f, DeltaFloat.MoveType.LINE);
        while (a.IsActive())
        {
            yield return null;
            a.Update(Time.deltaTime);
            model.color = new Color(col.r, col.g, col.b, a.Get());
        }
    }

    #endregion
}

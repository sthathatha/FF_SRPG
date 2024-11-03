using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター共通機能
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    #region メンバー

    /// <summary>フィールド</summary>
    public FieldSystem field;

    /// <summary>モデル</summary>
    public Animator anim;

    /// <summary>HPゲージ</summary>
    public HPGauge hpGauge;

    #endregion

    #region 変数

    /// <summary>現在座標</summary>
    protected Vector2Int location;

    /// <summary>パラメータ</summary>
    public GameParameter.FieldCharacterParameter param { get; protected set; } = new();

    /// <summary>ターン行動権</summary>
    public bool turnActable { get; protected set; } = true;

    #endregion

    #region 位置

    /// <summary>
    /// 位置設定
    /// </summary>
    /// <param name="loc"></param>
    public void SetLocation(Vector2Int loc)
    {
        location = loc;
        transform.localPosition = field.GetCellPosition(loc);
    }

    /// <summary>
    /// 位置取得
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetLocation() { return location; }

    #endregion

    #region パラメータ

    /// <summary>プレイヤーかどうか</summary>
    /// <returns></returns>
    virtual public bool IsPlayer() { return false; }

    /// <summary>
    /// 行動権設定
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
    /// 最小射程
    /// </summary>
    /// <returns></returns>
    abstract public int GetRangeMin();
    /// <summary>
    /// 最大射程
    /// </summary>
    /// <returns></returns>
    abstract public int GetRangeMax();

    #endregion

    #region アニメーション

    /// <summary>
    /// アニメーション再生
    /// </summary>
    /// <param name="dir"></param>
    virtual public void PlayAnim(Constant.Direction dir)
    {
        // 敵の場合待機と移動だけ
        if (dir == Constant.Direction.None)
            anim.Play("idle");
        else
            anim.Play("move");
    }

    /// <summary>
    /// 歩く
    /// </summary>
    /// <param name="history"></param>
    /// <returns></returns>
    public IEnumerator Walk(FieldSystem.MoveHistory history)
    {
        var p = new DeltaVector3();
        p.Set(transform.localPosition);
        // 履歴順に移動
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
    /// HPゲージ更新
    /// </summary>
    /// <param name="immediate">true:即時更新　false:アニメーション</param>
    public void UpdateHP(bool immediate = false)
    {
        if (immediate)
            hpGauge.SetHP(param.HP, param.MaxHP);
        else
            hpGauge.AnimHP(param.HP, param.MaxHP);
    }

    /// <summary>
    /// 攻撃アニメーション
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
    /// 死亡アニメーション
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

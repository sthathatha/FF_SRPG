using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター共通機能
/// </summary>
public class CharacterBase : MonoBehaviour
{
    #region メンバー

    /// <summary>フィールド</summary>
    public FieldSystem field;

    /// <summary>モデル</summary>
    public Animator anim;

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
    }

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

    #endregion
}

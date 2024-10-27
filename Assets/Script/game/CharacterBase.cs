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

    protected Vector2Int location;

    public GameParameter.FieldCharacterParameter param { get; protected set; } = new();

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

    #endregion
}

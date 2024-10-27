using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定数
/// </summary>
public class Constant
{
    #region 共通

    /// <summary>
    /// サインカーブ
    /// </summary>
    public enum SinCurveType : int
    {
        /// <summary>加速</summary>
        Accel = 0,
        /// <summary>減速</summary>
        Decel,
        /// <summary>加減速</summary>
        Both,
    }

    /// <summary>
    /// 画面幅
    /// </summary>
    public const float SCREEN_WIDTH = 960f;
    /// <summary>
    /// 画面高さ
    /// </summary>
    public const float SCREEN_HEIGHT = 540f;

    /// <summary>
    /// 向き
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

    #region ゲーム固有

    /// <summary>
    /// プレイヤーキャラID
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
    /// クラスID
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
    /// 敵キャラID
    /// </summary>
    public enum EnemyID
    {
        GreenSlime = 0,
    }

    #endregion
}

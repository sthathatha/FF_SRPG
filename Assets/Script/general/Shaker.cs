using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シェイク管理クラス
/// </summary>
public class Shaker
{
    /// <summary>
    /// サイズ指定
    /// </summary>
    public enum ShakeSize : int
    {
        /// <summary>弱い</summary>
        Weak = 0,
        /// <summary>普通</summary>
        Middle,
        /// <summary>強い</summary>
        Strong,
    }

    /// <summary>
    /// 状態
    /// </summary>
    private enum ShakeState : int
    {
        /// <summary>揺れてない</summary>
        Idle = 0,
        /// <summary>揺れている</summary>
        Active,
        /// <summary>止まりつつある</summary>
        Fadeout,
    }

    /// <summary>角速度</summary>
    private const float SHAKE_SPEED = Mathf.PI * 12f;

    /// <summary>角度</summary>
    private float shakeRot;
    /// <summary>振幅</summary>
    private DeltaFloat shakeWidth;
    /// <summary>止まるまで時間</summary>
    private float shakeTime;

    /// <summary>現在状態</summary>
    private ShakeState state;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Shaker()
    {
        shakeTime = -1f;
        shakeRot = 0f;
        shakeWidth = new DeltaFloat();
        shakeWidth.Set(0);
    }

    /// <summary>
    /// １回シェイク
    /// </summary>
    /// <param name="size"></param>
    /// <param name="time"></param>
    public void PlayOne(ShakeSize size, float time)
    {
        Play(size);
        shakeTime = time;
    }

    /// <summary>
    /// 永久シェイク
    /// </summary>
    /// <param name="size"></param>
    public void Play(ShakeSize size)
    {
        if (state == ShakeState.Idle)
        {
            shakeRot = 0f;
        }

        state = ShakeState.Active;
        shakeTime = -1f;
        shakeWidth.Set(CalcShakeWidth(size));
    }

    /// <summary>
    /// 止める
    /// </summary>
    public void Stop()
    {
        if (state != ShakeState.Active) return;

        state = ShakeState.Fadeout;
        shakeTime = -1f;
        shakeWidth.MoveTo(0f, 0.5f, DeltaFloat.MoveType.LINE);
    }

    /// <summary>
    /// フレーム処理
    /// </summary>
    public void Update()
    {
        // 
        shakeRot += Time.deltaTime * SHAKE_SPEED;
        if (shakeRot > Mathf.PI * 2f) shakeRot -= Mathf.PI * 2f;

        if (state == ShakeState.Active)
        {
            if (shakeTime >= 0f)
            {
                shakeTime -= Time.deltaTime;
                if (shakeTime < 0f)
                {
                    Stop();
                }
            }
        }
        else if (state == ShakeState.Fadeout)
        {
            shakeWidth.Update(Time.deltaTime);
            if (!shakeWidth.IsActive())
            {
                state = ShakeState.Idle;
            }
        }
    }

    /// <summary>
    /// シェイク中
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return state != ShakeState.Idle;
    }

    /// <summary>
    /// シェイク値取得
    /// </summary>
    /// <returns></returns>
    public float GetShakeValue()
    {
        return Mathf.Sin(shakeRot) * shakeWidth.Get();
    }

    /// <summary>
    /// サイズから幅決定
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private float CalcShakeWidth(ShakeSize size)
    {
        return size switch
        {
            ShakeSize.Weak => 10f,
            ShakeSize.Middle => 20f,
            _ => 30f
        };
    }
}

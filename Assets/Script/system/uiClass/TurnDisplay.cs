using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ターン表示UI
/// </summary>
public class TurnDisplay : MonoBehaviour
{
    #region メンバー

    public Image bgImage;
    public TMP_Text turnText;

    #endregion

    /// <summary>
    /// ターン表示
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public IEnumerator DisplayTurnStart(bool player)
    {
        SetDisplay(player);
        gameObject.SetActive(true);

        var alpha = new DeltaFloat();
        alpha.Set(0f);
        alpha.MoveTo(1f, 0.3f, DeltaFloat.MoveType.ACCEL);
        var x = new DeltaFloat();
        x.Set(160f);
        x.MoveTo(0f, 0.3f, DeltaFloat.MoveType.ACCEL);
        var height = new DeltaFloat();
        height.Set(0f);
        height.MoveTo(1f, 0.3f, DeltaFloat.MoveType.LINE);

        bgImage.rectTransform.localScale = new Vector3(1f, height.Get(), 1f);
        turnText.rectTransform.localPosition = new Vector3(x.Get(), 0);
        turnText.color = new Color(1f, 1f, 1f, alpha.Get());
        while (alpha.IsActive())
        {
            yield return null;

            alpha.Update(Time.deltaTime);
            x.Update(Time.deltaTime);
            height.Update(Time.deltaTime);

            bgImage.rectTransform.localScale = new Vector3(1f, height.Get(), 1f);
            turnText.rectTransform.localPosition = new Vector3(x.Get(), 0);
            turnText.color = new Color(1f, 1f, 1f, alpha.Get());
        }
        yield return new WaitForSeconds(0.6f);

        height.MoveTo(0f, 0.3f, DeltaFloat.MoveType.LINE);
        x.MoveTo(-160f, 0.3f, DeltaFloat.MoveType.DECEL);
        alpha.MoveTo(0f, 0.3f, DeltaFloat.MoveType.DECEL);
        while (alpha.IsActive())
        {
            yield return null;

            alpha.Update(Time.deltaTime);
            x.Update(Time.deltaTime);
            height.Update(Time.deltaTime);

            bgImage.rectTransform.localScale = new Vector3(1f, height.Get(), 1f);
            turnText.rectTransform.localPosition = new Vector3(x.Get(), 0);
            turnText.color = new Color(1f, 1f, 1f, alpha.Get());
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 表示内容設定
    /// </summary>
    /// <param name="player"></param>
    private void SetDisplay(bool player)
    {
        if (player)
        {
            turnText.SetText("Player Turn");
            bgImage.color = new Color(0.4f, 0.4f, 1f, 1f);
        }
        else
        {
            turnText.SetText("Enemy Turn");
            bgImage.color = new Color(1f, 0.4f, 0.4f, 1f);
        }
    }
}

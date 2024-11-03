using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// セルに対するUI表示
/// </summary>
public class CellUI : MonoBehaviour
{
    #region 定数

    private const float TXT_START_Y = 24f;
    private const float TXT_END_Y = 64f;
    private const float TXT_ANIM_TIME = 0.5f;
    private const float TXT_ANIM_END_TIME = 0.3f;

    #endregion

    #region メンバー

    public FieldSystem field;

    public TMP_Text txt_Exp;
    public TMP_Text txt_Lvup;

    #endregion

    #region 基底

    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        txt_Exp.gameObject.SetActive(false);
        txt_Lvup.gameObject.SetActive(false);
    }

    #endregion

    /// <summary>
    /// 経験値表示
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public IEnumerator ShowExpCoroutine(Vector2Int loc, int exp)
    {
        transform.localPosition = field.GetCellPosition(loc);
        txt_Exp.SetText($"{exp} exp");

        var y = new DeltaFloat();
        y.Set(TXT_START_Y);
        y.MoveTo(TXT_END_Y, TXT_ANIM_TIME, DeltaFloat.MoveType.DECEL);
        txt_Exp.transform.localPosition = new Vector3(0, y.Get());
        txt_Exp.gameObject.SetActive(true);

        while (y.IsActive())
        {
            yield return null;
            y.Update(Time.deltaTime);

            txt_Exp.transform.localPosition = new Vector3(0, y.Get());
        }

        yield return new WaitForSeconds(TXT_ANIM_END_TIME);
        txt_Exp.gameObject.SetActive(false);
    }

    /// <summary>
    /// レベルアップ表示
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public IEnumerator ShowLevelUpCoroutine(Vector2Int loc)
    {
        transform.localPosition = field.GetCellPosition(loc);
        var y = new DeltaFloat();
        y.Set(TXT_START_Y);
        y.MoveTo(TXT_END_Y, TXT_ANIM_TIME, DeltaFloat.MoveType.DECEL);
        txt_Lvup.transform.localPosition = new Vector3(0, y.Get());
        txt_Lvup.gameObject.SetActive(true);

        while (y.IsActive())
        {
            yield return null;
            y.Update(Time.deltaTime);

            txt_Lvup.transform.localPosition = new Vector3(0, y.Get());
        }

        yield return new WaitForSeconds(TXT_ANIM_END_TIME);
        txt_Lvup.gameObject.SetActive(false);
    }
}

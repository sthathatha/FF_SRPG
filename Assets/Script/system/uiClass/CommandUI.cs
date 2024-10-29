using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コマンド選択UI
/// </summary>
public class CommandUI : MonoBehaviour
{
    #region 戻り値

    /// <summary>
    /// コマンド選択戻り値
    /// </summary>
    public enum CommandResult
    {
        Cancel = 0,
        Act,
        Wait,
        ClassChange,
        Escape,
    }
    public CommandResult Result { get; private set; }

    #endregion

    #region メンバー

    public RectTransform window;
    public Button ccText;

    #endregion

    #region 変数

    private bool showing = false;
    private bool ccEnable = false;

    #endregion

    #region 表示
    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    public IEnumerator ShowCoroutine(PlayerCharacter pc)
    {
        showing = true;
        Result = CommandResult.Cancel;
        SetCommands(pc);
        gameObject.SetActive(true);
        // なにかのボタンを押すまで
        yield return new WaitWhile(() => showing);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// キャラの状態によって選択肢の調整
    /// </summary>
    /// <param name="pc"></param>
    private void SetCommands(PlayerCharacter pc)
    {
        // 半分より右に居る場合左に置く
        var pos = window.localPosition;
        if (pc.GetLocation().x >= FieldSystem.COL_COUNT / 2)
            pos.x = -300f;
        else
            pos.x = 300f;
        window.localPosition = pos;

        // 2次職の場合Lv20以上、それ以外の場合Lv10以上の時のみクラスチェンジ可能
        ccEnable = false;
        var saveParam = pc.GetSaveParameter();
        if (saveParam.ClassID == Constant.ClassID.A2 ||
            saveParam.ClassID == Constant.ClassID.B2 ||
            saveParam.ClassID == Constant.ClassID.AB)
            ccEnable = pc.param.Lv >= 20;
        else
            ccEnable = pc.param.Lv >= 10;

        ccText.interactable = ccEnable;
    }

    #endregion

    /// <summary>
    /// 行動ボタン
    /// </summary>
    public void BtnAct()
    {
        Result = CommandResult.Act;
        showing = false;
    }

    /// <summary>
    /// 待機
    /// </summary>
    public void BtnWait()
    {
        Result = CommandResult.Wait;
        showing = false;
    }

    /// <summary>
    /// クラスチェンジ
    /// </summary>
    public void BtnCC()
    {
        Result = CommandResult.ClassChange;
        showing = false;
    }

    /// <summary>
    /// 撤退
    /// </summary>
    public void BtnEscape()
    {
        Result = CommandResult.Escape;
        showing = false;
    }

    /// <summary>
    /// キャンセル
    /// </summary>
    public void BtnCancel()
    {
        Result = CommandResult.Cancel;
        showing = false;
    }
}

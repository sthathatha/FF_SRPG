using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// クラスチェンジUI
/// </summary>
public class ClassChangeUI : MonoBehaviour
{
    #region メンバー

    public TMP_Text name_old;
    public ClassChangeUIDetail det_nextA;
    public ClassChangeUIDetail det_nextB;
    public ClassChangeUIDetail det_nextRebirth;
    public GameObject arrow_A;
    public GameObject arrow_B;
    public GameObject arrow_Rebirth;

    #endregion

    #region 表示

    public enum CCSelect
    {
        Cancel = 0,
        ClassChange,
        Rebirth,
    }
    /// <summary>選択結果</summary>
    public CCSelect Result { get; private set; }
    /// <summary>選択クラス</summary>
    public Constant.ClassID SelectClass { get; private set; }

    /// <summary>
    /// クラスチェンジ選択
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    public IEnumerator ShowCCCoroutine(PlayerCharacter pc)
    {
        ShowCCSelector(pc);
        gameObject.SetActive(true);

        isShowing = true;
        yield return new WaitWhile(() => isShowing);
        gameObject.SetActive(false);
    }

    private Constant.ClassID selectClassA;
    private Constant.ClassID selectClassB;

    /// <summary>
    /// 選択肢表示
    /// </summary>
    /// <param name="pc"></param>
    private void ShowCCSelector(PlayerCharacter pc)
    {
        var names = GameDatabase.Name_Classes_Get(pc.playerID);
        var nowClass = pc.GetSaveParameter().ClassID;
        name_old.SetText(names[(int)nowClass]);

        selectClassA = Constant.ClassID.Base;
        selectClassB = Constant.ClassID.Base;
        if (nowClass == Constant.ClassID.Base)
        {
            selectClassA = Constant.ClassID.A;
            selectClassB = Constant.ClassID.B;
        }
        else if (nowClass == Constant.ClassID.A)
        {
            selectClassA = Constant.ClassID.A2;
            selectClassB = Constant.ClassID.AB;
        }
        else if (nowClass == Constant.ClassID.B)
        {
            selectClassA = Constant.ClassID.AB;
            selectClassB = Constant.ClassID.B2;
        }

        if (selectClassA == Constant.ClassID.Base)
        {
            // 転生選択
            det_nextRebirth.gameObject.SetActive(true);
            det_nextA.gameObject.SetActive(false);
            det_nextB.gameObject.SetActive(false);
            arrow_Rebirth.SetActive(true);
            arrow_A.SetActive(false);
            arrow_B.SetActive(false);

            det_nextRebirth.SetClass(pc.playerID, Constant.ClassID.Base);
        }
        else
        {
            // クラスチェンジ
            det_nextRebirth.gameObject.SetActive(false);
            det_nextA.gameObject.SetActive(true);
            det_nextB.gameObject.SetActive(true);
            arrow_Rebirth.SetActive(false);
            arrow_A.SetActive(true);
            arrow_B.SetActive(true);

            det_nextA.SetClass(pc.playerID, selectClassA);
            det_nextB.SetClass(pc.playerID, selectClassB);
        }
    }

    #endregion

    #region ボタン処理

    private bool isShowing = false;

    /// <summary>
    /// 選択
    /// </summary>
    /// <param name="param">0:クラスA　1:クラスB　2:転生</param>
    public void ClickSelect(int param)
    {
        switch (param)
        {
            case 0:
                Result = CCSelect.ClassChange;
                SelectClass = selectClassA;
                break;
            case 1:
                Result = CCSelect.ClassChange;
                SelectClass = selectClassB;
                break;
            case 2:
                Result = CCSelect.Rebirth;
                SelectClass = selectClassA;
                break;
        }

        isShowing = false;
    }

    /// <summary>
    /// キャンセル
    /// </summary>
    public void ClickCancel()
    {
        Result = CCSelect.Cancel;
        isShowing = false;
    }

    #endregion
}

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
    public TMP_Text name_nextA;
    public TMP_Text name_nextB;
    public TMP_Text name_nextRebirth;
    public GameObject arrow_A;
    public GameObject arrow_B;
    public GameObject arrow_Rebirth;
    public GameObject window_A;
    public GameObject window_B;
    public GameObject window_Rebirth;

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
            window_Rebirth.SetActive(true);
            window_A.SetActive(false);
            window_B.SetActive(false);
            arrow_Rebirth.SetActive(true);
            arrow_A.SetActive(false);
            arrow_B.SetActive(false);

            name_nextRebirth.SetText(names[(int)Constant.ClassID.Base]);
        }
        else
        {
            // クラスチェンジ
            window_Rebirth.SetActive(false);
            window_A.SetActive(true);
            window_B.SetActive(true);
            arrow_Rebirth.SetActive(false);
            arrow_A.SetActive(true);
            arrow_B.SetActive(true);

            name_nextA.SetText(names[(int)selectClassA]);
            name_nextB.SetText(names[(int)selectClassB]);
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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// �N���X�`�F���WUI
/// </summary>
public class ClassChangeUI : MonoBehaviour
{
    #region �����o�[

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

    #region �\��

    public enum CCSelect
    {
        Cancel = 0,
        ClassChange,
        Rebirth,
    }
    /// <summary>�I������</summary>
    public CCSelect Result { get; private set; }
    /// <summary>�I���N���X</summary>
    public Constant.ClassID SelectClass { get; private set; }

    /// <summary>
    /// �N���X�`�F���W�I��
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
    /// �I�����\��
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
            // �]���I��
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
            // �N���X�`�F���W
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

    #region �{�^������

    private bool isShowing = false;

    /// <summary>
    /// �I��
    /// </summary>
    /// <param name="param">0:�N���XA�@1:�N���XB�@2:�]��</param>
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
    /// �L�����Z��
    /// </summary>
    public void ClickCancel()
    {
        Result = CCSelect.Cancel;
        isShowing = false;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �^�C�g�����
/// </summary>
public class TitleSceneScript : MainScriptBase
{
    #region �����o�[

    public Button btnContinue;

    #endregion

    #region ���

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    public override IEnumerator AfterFadeIn(bool init)
    {
        yield return base.AfterFadeIn(init);

        // �Z�[�u�f�[�^�������Continue
        btnContinue.interactable = Global.GetSaveData().IsEnableGameData();
    }

    #endregion

    /// <summary>
    /// NewGame
    /// </summary>
    public void ClickNewGame()
    {
        Global.GetTemporaryData().isLoadGame = false;
        ManagerSceneScript.GetInstance().LoadMainScene("GameScene", 0);
    }

    /// <summary>
    /// Continue
    /// </summary>
    public void ClickContinue()
    {
        Global.GetTemporaryData().isLoadGame = true;
        ManagerSceneScript.GetInstance().LoadMainScene("GameScene", 0);
    }

    /// <summary>
    /// �I�v�V����
    /// </summary>
    public void ClickOption()
    {
        ManagerSceneScript.GetInstance().optionUI.Open();
    }

    /// <summary>
    /// �}��
    /// </summary>
    public void ClickDictionary()
    {

    }
}

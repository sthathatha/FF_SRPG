using System;
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
    /// BGM�w��
    /// </summary>
    /// <returns></returns>
    public override Tuple<SoundManager.FieldBgmType, AudioClip> GetBgm()
    {
        return new Tuple<SoundManager.FieldBgmType, AudioClip>(SoundManager.FieldBgmType.Common1, null);
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BeforeInitFadeIn()
    {
        yield return base.BeforeInitFadeIn();
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
        ManagerSceneScript.GetInstance().LoadMainScene("DictionaryScene", 0);
    }
}

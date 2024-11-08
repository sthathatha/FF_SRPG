using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトル画面
/// </summary>
public class TitleSceneScript : MainScriptBase
{
    #region メンバー

    public Button btnContinue;

    #endregion

    #region 基底

    /// <summary>
    /// BGM指定
    /// </summary>
    /// <returns></returns>
    public override Tuple<SoundManager.FieldBgmType, AudioClip> GetBgm()
    {
        return new Tuple<SoundManager.FieldBgmType, AudioClip>(SoundManager.FieldBgmType.Common1, null);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BeforeInitFadeIn()
    {
        yield return base.BeforeInitFadeIn();
        // セーブデータがあればContinue
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
    /// オプション
    /// </summary>
    public void ClickOption()
    {
        ManagerSceneScript.GetInstance().optionUI.Open();
    }

    /// <summary>
    /// 図鑑
    /// </summary>
    public void ClickDictionary()
    {
        ManagerSceneScript.GetInstance().LoadMainScene("DictionaryScene", 0);
    }
}

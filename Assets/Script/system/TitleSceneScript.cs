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
    /// 初期化
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    public override IEnumerator AfterFadeIn(bool init)
    {
        yield return base.AfterFadeIn(init);

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

    }
}

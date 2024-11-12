using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 図鑑画面
/// </summary>
public class DictionarySceneScript : MainScriptBase
{
    public TMP_Text txt_Name;
    public TMP_Text txt_Atk;
    public TMP_Text txt_Hit;
    public TMP_Text txt_Crt;
    public TMP_Text txt_Range;
    public TMP_Text txt_UseCnt;

    public ItemDictionaryUI dicUI;

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

        // 
        dicUI.CreateItemList();
        txt_Name.SetText("-----");
        txt_Hit.SetText("--");
        txt_Crt.SetText("--");
        txt_Atk.SetText("--");
        txt_Range.SetText("--");
        txt_UseCnt.SetText("--");
    }

    /// <summary>
    /// アイテムステータス表示
    /// </summary>
    /// <param name="itemID"></param>
    public void ShowItemStatus(GameDatabase.ItemID itemID)
    {
        var prm = GameDatabase.ItemDataList[(int)itemID];
        txt_Name.SetText(prm.name);
        txt_Atk.SetText(prm.atk.ToString());
        txt_UseCnt.SetText(prm.maxUse.ToString());
        if (prm.iType == GameDatabase.ItemType.Item || prm.iType == GameDatabase.ItemType.Rod)
        {
            txt_Hit.SetText("--");
            txt_Crt.SetText("--");
        }
        else
        {
            txt_Hit.SetText(prm.hit.ToString());
            txt_Crt.SetText(prm.critical.ToString());
        }
        if (prm.rangeMin == prm.rangeMax)
        {
            txt_Range.SetText(prm.rangeMin.ToString());
        }
        else
        {
            txt_Range.SetText($"{prm.rangeMin}～{prm.rangeMax}");
        }
    }

    /// <summary>
    /// 戻る
    /// </summary>
    public void ClickBack()
    {
        ManagerSceneScript.GetInstance().LoadMainScene("TitleScene", 0);
    }
}

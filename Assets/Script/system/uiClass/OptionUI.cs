using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// オプション
/// </summary>
public class OptionUI : MonoBehaviour
{
    #region メンバー

    public Slider sliderBgm;
    public Slider sliderSe;

    #endregion

    /// <summary>
    /// 開く
    /// </summary>
    public void Open()
    {
        var save = Global.GetSaveData();
        sliderBgm.value = save.system.bgmVolume;
        sliderSe.value = save.system.seVolume;

        gameObject.SetActive(true);
    }

    #region イベント

    /// <summary>
    /// BGM
    /// </summary>
    public void BgmChange()
    {
        var save = Global.GetSaveData();
        save.system.bgmVolume = Mathf.RoundToInt(sliderBgm.value);

        ManagerSceneScript.GetInstance().soundMan.UpdateBgmVolume();
    }

    /// <summary>
    /// SE
    /// </summary>
    public void SeChange()
    {
        var save = Global.GetSaveData();
        save.system.seVolume = Mathf.RoundToInt(sliderSe.value);

        ManagerSceneScript.GetInstance().soundMan.UpdateSeVolume();
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void ClickClose()
    {
        var save = Global.GetSaveData();
        save.SaveSystemData();

        gameObject.SetActive(false);
    }

    #endregion
}

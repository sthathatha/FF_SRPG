using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �I�v�V����
/// </summary>
public class OptionUI : MonoBehaviour
{
    #region �����o�[

    public Slider sliderBgm;
    public Slider sliderSe;

    #endregion

    /// <summary>
    /// �J��
    /// </summary>
    public void Open()
    {
        var save = Global.GetSaveData();
        sliderBgm.value = save.system.bgmVolume;
        sliderSe.value = save.system.seVolume;

        gameObject.SetActive(true);
    }

    #region �C�x���g

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
    /// ����
    /// </summary>
    public void ClickClose()
    {
        var save = Global.GetSaveData();
        save.SaveSystemData();

        gameObject.SetActive(false);
    }

    #endregion
}

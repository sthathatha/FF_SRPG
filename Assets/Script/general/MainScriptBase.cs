using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainScriptBase : MonoBehaviour
{
    #region メンバー

    /// <summary>スリープ時にActive=falseする親オブジェクト</summary>
    public GameObject objectParent = null;

    /// <summary>BGM</summary>
    public AudioClip bgmClip = null;

    #endregion

    #region 変数

    #endregion

    #region 基底

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainScriptBase()
    {
    }

    /// <summary>
    /// 開始時
    /// </summary>
    /// <returns></returns>
    virtual protected IEnumerator Start()
    {
        // 直接起動時にマネージャ起動
        if (!ManagerSceneScript.GetInstance())
        {
            ManagerSceneScript.isDebugLoad = true;
            Global.GetSaveData().LoadGameData();
            SceneManager.LoadScene("_ManagerScene", LoadSceneMode.Additive);
            yield return null;
        }

        ManagerSceneScript.GetInstance().SetMainScript(this);
    }

    virtual protected void Update()
    {
    }

    #endregion

    #region パブリックメソッド

    /// <summary>
    /// ゲーム開始用にスリープ
    /// </summary>
    virtual public void Sleep()
    {
        objectParent?.SetActive(false);
    }

    /// <summary>
    /// ゲーム終了時に再開
    /// </summary>
    virtual public void AwakeFromGame()
    {
        objectParent?.SetActive(true);

        if (!ManagerSceneScript.GetInstance()) return;
    }

    /// <summary>
    /// ロード後最初の１回
    /// </summary>
    /// <returns></returns>
    virtual public IEnumerator BeforeInitFadeIn()
    {
        yield break;
    }

    /// <summary>
    /// フェードイン直前にやること
    /// </summary>
    /// <returns></returns>
    virtual public IEnumerator BeforeFadeIn()
    {
        yield break;
    }

    /// <summary>
    /// フェードイン終わったらやること
    /// </summary>
    /// <returns></returns>
    virtual public IEnumerator AfterFadeIn(bool init)
    {
        yield break;
    }

    /// <summary>
    /// BGM設定　特殊処理の場合はオーバーライド
    /// </summary>
    /// <returns></returns>
    virtual public Tuple<SoundManager.FieldBgmType, AudioClip> GetBgm()
    {
        return new Tuple<SoundManager.FieldBgmType, AudioClip>(SoundManager.FieldBgmType.Clip, bgmClip);
    }

    #endregion
}

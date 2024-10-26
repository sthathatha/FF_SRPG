using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region 定数
    /// <summary>フィールドBGMタイプ</summary>
    public enum FieldBgmType : int
    {
        None = 0,
        Common1,
        Common2,
        Common3,
        Clip,
    }

    /// <summary>BGMフェード時間</summary>
    private const float BGM_FADE_TIME = 0.5f;
    #endregion

    #region メンバー
    /// <summary>SE再生複製用</summary>
    public GameObject seDummy = null;
    /// <summary>フィールドBGM再生用</summary>
    public AudioSource fieldBgmSource = null;
    ///// <summary>ゲームBGM再生用</summary>
    //public AudioSource gameBgmSource = null;
    ///// <summary>ボイス再生用</summary>
    //public AudioSource voiceSource = null;

    /// <summary>汎用選択SE</summary>
    public AudioClip commonSeSelect = null;
    /// <summary>汎用カーソル移動SE</summary>
    public AudioClip commonSeMove = null;
    /// <summary>汎用ウィンドウ開くSE</summary>
    public AudioClip commonSeWindowOpen = null;
    /// <summary>汎用エラー音ブブッ</summary>
    public AudioClip commonSeError = null;
    #endregion

    #region プライベート変数
    /// <summary>再生中のフィールドBGM</summary>
    private FieldBgmType playingFieldBgm;
    #endregion

    #region 既定処理
    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        playingFieldBgm = FieldBgmType.None;
    }
    #endregion

    #region SE管理
    /// <summary>
    /// SEボリューム設定
    /// </summary>
    /// <param name="v"></param>
    public void UpdateSeVolume()
    {
        seDummy.GetComponent<AudioSource>().volume = CalcSeVolume();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="se"></param>
    /// <param name="startTime">再生開始時間</param>
    public void PlaySE(AudioClip se, float startTime = 0f)
    {
        var seObj = GameObject.Instantiate(seDummy);
        seObj.transform.SetParent(this.transform);

        var seSource = seObj.GetComponent<AudioSource>();
        seSource.clip = se;
        seSource.time = startTime;
        seSource.Play();

        StartCoroutine(DestroyWaitCoroutine(seSource));
    }

    /// <summary>
    /// SEループ再生
    /// </summary>
    /// <param name="se"></param>
    /// <param name="startTime">再生開始時間</param>
    /// <returns>呼び出し側で制御する用オブジェクト</returns>
    public AudioSource PlaySELoop(AudioClip se, float startTime = 0f)
    {
        var seObj = GameObject.Instantiate(seDummy);
        seObj.transform.SetParent(this.transform);

        var seSource = seObj.GetComponent<AudioSource>();
        seSource.clip = se;
        seSource.time = startTime;
        seSource.loop = true;
        seSource.Play();

        return seSource;
    }

    /// <summary>
    /// SE再生終了を待って削除
    /// </summary>
    /// <param name="se"></param>
    /// <returns></returns>
    private IEnumerator DestroyWaitCoroutine(AudioSource se)
    {
        yield return new WaitWhile(() => se.isPlaying);
        Destroy(se.gameObject);
    }

    /// <summary>
    /// ループSEを止める
    /// </summary>
    /// <param name="se"></param>
    /// <param name="time">フェード時間</param>
    public void StopLoopSE(AudioSource se, float time = -1f)
    {
        if (time <= 0f)
        {
            se.Stop();
            Destroy(se.gameObject);
            return;
        }

        StartCoroutine(StopLoopSECoroutine(se, time));
    }

    /// <summary>
    /// ループSEフェードアウトコルーチン
    /// </summary>
    /// <param name="se"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator StopLoopSECoroutine(AudioSource se, float time)
    {
        var vol = new DeltaFloat();
        vol.Set(se.volume);
        vol.MoveTo(0f, time, DeltaFloat.MoveType.LINE);
        while (vol.IsActive())
        {
            yield return null;
            se.volume = vol.Get();
        }

        se.Stop();
        Destroy(se);
    }

    #endregion

    #region BGM管理
    /// <summary>
    /// 
    /// </summary>
    public void UpdateBgmVolume()
    {
        var vol = CalcBgmVolume();
        fieldBgmSource.GetComponent<AudioSource>().volume = vol;
        //gameBgmSource.GetComponent<AudioSource>().volume = vol;
    }

    #region フィールドBGM
    /// <summary>
    /// フィールド読み込み時のBGM再生
    /// </summary>
    /// <param name="fieldBgmType"></param>
    /// <param name="source"></param>
    /// <param name="loop"></param>
    public void PlayFieldBgm(FieldBgmType fieldBgmType, AudioClip source = null, bool loop = true)
    {
        if (IsNeedChangeFieldBgm(fieldBgmType))
        {
            //var clip = fieldBgmType switch
            //{
            //    FieldBgmType.Common1 => commonBgm1,
            //    FieldBgmType.Common2 => commonBgm2,
            //    FieldBgmType.Common3 => commonBgm3,
            //    _ => source,
            //};
            var clip = source;
            if (clip != null)
            {
                fieldBgmSource.volume = CalcBgmVolume();
                fieldBgmSource.clip = clip;
                fieldBgmSource.loop = loop;
                fieldBgmSource.Play();
            }
        }

        playingFieldBgm = fieldBgmType;
    }

    /// <summary>
    /// フィールドBGMをフェードアウトする
    /// </summary>
    /// <param name="isPause">true:ポーズするのみ　false:Stop</param>
    /// <returns></returns>
    public IEnumerator FadeOutFieldBgm(bool isPause = false)
    {
        if (!fieldBgmSource.isPlaying)
        {
            yield break;
        }

        var vol = new DeltaFloat();
        vol.Set(fieldBgmSource.volume);
        vol.MoveTo(0, BGM_FADE_TIME, DeltaFloat.MoveType.LINE);
        while (vol.IsActive())
        {
            vol.Update(Time.deltaTime);
            fieldBgmSource.volume = vol.Get();
            yield return null;
        }

        if (isPause)
        {
            fieldBgmSource.Pause();
        }
        else
        {
            fieldBgmSource.Stop();
            playingFieldBgm = FieldBgmType.None;
        }
    }

    /// <summary>
    /// フィールドBGMがまだ鳴っている（Pauseの場合でもfalseになる）
    /// </summary>
    /// <returns></returns>
    public bool IsFieldBgmPlaying()
    {
        return fieldBgmSource.isPlaying;
    }

    /// <summary>
    /// ポーズしたフィールドBGM復帰
    /// </summary>
    /// <returns></returns>
    public IEnumerator ResumeFieldBgm()
    {
        fieldBgmSource.UnPause();
        var newVol = new DeltaFloat();
        newVol.Set(0);
        newVol.MoveTo(CalcBgmVolume(), BGM_FADE_TIME, DeltaFloat.MoveType.LINE);
        while (newVol.IsActive())
        {
            newVol.Update(Time.deltaTime);
            fieldBgmSource.volume = newVol.Get();
            yield return null;
        }
    }

    /// <summary>
    /// BGM変更の必要があるかチェック
    /// CommonBGMはマップ切り替えで変わらない場合に判定する
    /// </summary>
    /// <param name="fieldBgmType">タイプ</param>
    /// <returns>true:変更する　false:継続</returns>
    public bool IsNeedChangeFieldBgm(FieldBgmType fieldBgmType)
    {
        if (fieldBgmType == FieldBgmType.Clip) return true;
        if (fieldBgmType != playingFieldBgm) return true;
        if (!fieldBgmSource.isPlaying) return true;

        return false;
    }
    #endregion

    #region ゲームBGM
    ///// <summary>
    ///// ゲーム開始時のBGM開始
    ///// </summary>
    ///// <param name="source"></param>
    //public void StartGameBgm(AudioClip source = null)
    //{
    //    if (!source) return;

    //    gameBgmSource.volume = CalcBgmVolume();
    //    gameBgmSource.clip = source;
    //    gameBgmSource.Play();
    //}

    ///// <summary>
    ///// ゲームBGMを止める
    ///// </summary>
    ///// <returns></returns>
    //public IEnumerator FadeOutGameBgm()
    //{
    //    if (!gameBgmSource.isPlaying) yield break;

    //    var vol = new DeltaFloat();
    //    vol.Set(gameBgmSource.volume);
    //    vol.MoveTo(0, BGM_FADE_TIME, DeltaFloat.MoveType.LINE);
    //    while (vol.IsActive())
    //    {
    //        vol.Update(Time.deltaTime);
    //        gameBgmSource.volume = vol.Get();
    //        yield return null;
    //    }
    //    gameBgmSource.Stop();
    //}

    ///// <summary>
    ///// ゲームBGMがまだ動いてる
    ///// </summary>
    ///// <returns></returns>
    //public bool IsGameBgmPlaying()
    //{
    //    return gameBgmSource.isPlaying;
    //}

    #endregion

    #endregion

    #region ボイス管理
    ///// <summary>
    ///// ボイスボリューム設定
    ///// </summary>
    //public void UpdateVoiceVolume()
    //{
    //    voiceSource.volume = CalcVoiceVolume();
    //}

    ///// <summary>
    ///// ボイス再生
    ///// </summary>
    ///// <param name="voice"></param>
    //public void PlayVoice(AudioClip voice)
    //{
    //    StopVoice();
    //    if (voice)
    //    {
    //        voiceSource.clip = voice;
    //        voiceSource.Play();
    //    }
    //}

    ///// <summary>
    ///// 再生中のボイスを停止
    ///// </summary>
    //public void StopVoice()
    //{
    //    if (voiceSource.isPlaying) { voiceSource.Stop(); }
    //}
    #endregion

    #region プライベート
    /// <summary>
    /// BGMSourceの再生ボリューム
    /// </summary>
    /// <returns></returns>
    private float CalcBgmVolume()
    {
        var optionVol = Global.GetSaveData().system.bgmVolume;
        return 0.2f * optionVol / 10;
    }
    /// <summary>
    /// SESourceの再生ボリューム
    /// </summary>
    /// <returns></returns>
    private float CalcSeVolume()
    {
        var optionVol = Global.GetSaveData().system.seVolume;
        return 1f * optionVol / 10;
    }
    /// <summary>
    /// VoiceSourceの再生ボリューム
    /// </summary>
    /// <returns></returns>
    private float CalcVoiceVolume()
    {
        //var optionVol = Global.GetSaveData().system.voiceVolume;
        //return 1f * optionVol / 10;
        return 0f;
    }
    #endregion
}

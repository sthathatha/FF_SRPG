using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステータス上昇表示UI
/// </summary>
public class StatusUpUI : MonoBehaviour
{
    private const float DOWN_Y = -390f;
    private const float UP_Y = 0f;
    private const float UP_WAITTIME = 0.3f;
    private readonly Color UP_COLOR = Color.green;
    private readonly Color DOWN_COLOR = Color.red;

    #region メンバー

    public Transform moveParent;
    public AudioClip se_paramUp;

    public Image faceImage;

    public TMP_Text class_value;
    public TMP_Text lv_value;
    public TMP_Text lv_up;

    public TMP_Text hp_value;
    public TMP_Text hp_up;
    public TMP_Text atk_value;
    public TMP_Text atk_up;
    public TMP_Text mag_value;
    public TMP_Text mag_up;
    public TMP_Text tec_value;
    public TMP_Text tec_up;
    public TMP_Text spd_value;
    public TMP_Text spd_up;
    public TMP_Text luk_value;
    public TMP_Text luk_up;
    public TMP_Text def_value;
    public TMP_Text def_up;
    public TMP_Text mdf_value;
    public TMP_Text mdf_up;
    public TMP_Text move_value;
    public TMP_Text move_up;

    #endregion

    #region 変数

    /// <summary>表示ベースパラメーター</summary>
    private GameParameter.PlayerSaveParameter saveParam = null;

    /// <summary>表示中</summary>
    private bool showing = false;

    #endregion

    #region 表示呼び出し

    /// <summary>
    /// レベルアップ表示
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="upParam"></param>
    /// <returns></returns>
    public IEnumerator ShowLvup(PlayerCharacter pc, GameDatabase.ParameterData upParam)
    {
        var sound = ManagerSceneScript.GetInstance().soundMan;
        SetNowParameter(pc);
        gameObject.SetActive(true);
        yield return ShowUp();

        // レベル上昇表示
        sound.PlaySE(se_paramUp);
        lv_up.SetText("+1");
        lv_up.gameObject.SetActive(true);
        lv_value.SetText((saveParam.Lv + 1).ToString());
        yield return new WaitForSeconds(UP_WAITTIME);

        // ステータス上昇表示
        yield return ShowUpParameter(upParam);

        showing = true;
        yield return new WaitWhile(() => showing);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// クラスチェンジ表示
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="nextClass"></param>
    /// <returns></returns>
    public IEnumerator ShowClassChange(PlayerCharacter pc, Constant.ClassID nextClass)
    {
        var sound = ManagerSceneScript.GetInstance().soundMan;
        SetNowParameter(pc);
        gameObject.SetActive(true);
        yield return ShowUp();

        // クラス変更
        sound.PlaySE(se_paramUp);
        class_value.SetText(GameDatabase.Name_Classes_Get(pc.playerID)[(int)nextClass]);
        lv_value.SetText("1");
        yield return new WaitForSeconds(UP_WAITTIME * 2f);

        // アップ表示
        var upParam = GameDatabase.Prm_ClassChangeGrow_Get(pc.playerID, nextClass);
        yield return ShowUpParameter(upParam);

        showing = true;
        yield return new WaitWhile(() => showing);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 転生表示
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="downParam"></param>
    /// <returns></returns>
    public IEnumerator ShowRebirth(PlayerCharacter pc, GameDatabase.ParameterData downParam)
    {
        var sound = ManagerSceneScript.GetInstance().soundMan;
        SetNowParameter(pc);
        gameObject.SetActive(true);
        yield return ShowUp();

        // クラス変更
        sound.PlaySE(se_paramUp);
        class_value.SetText(GameDatabase.Name_Classes_Get(pc.playerID)[(int)Constant.ClassID.Base]);
        lv_value.SetText("1");
        yield return new WaitForSeconds(UP_WAITTIME * 2f);

        // ダウン表示
        yield return ShowUpParameter(downParam);

        showing = true;
        yield return new WaitWhile(() => showing);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void CloseClick()
    {
        showing = false;
    }

    /// <summary>
    /// 下から出てくる
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowUp()
    {
        var y = new DeltaFloat();
        y.Set(DOWN_Y);
        y.MoveTo(UP_Y, 0.4f, DeltaFloat.MoveType.LINE);
        moveParent.localPosition = new Vector3(0f, y.Get());
        while (y.IsActive())
        {
            yield return null;
            y.Update(Time.deltaTime);

            moveParent.localPosition = new Vector3(0f, y.Get());
        }

        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    #region 内容表示

    /// <summary>
    /// 現在のステータス表示
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    private void SetNowParameter(PlayerCharacter pc)
    {
        saveParam = pc.GetSaveParameter();

        faceImage.sprite = ManagerSceneScript.GetInstance().generalResources.GetFaceIconP(pc.playerID);
        class_value.SetText(GameDatabase.Name_Classes_Get(pc.playerID)[(int)saveParam.ClassID]);
        lv_value.SetText(saveParam.Lv.ToString());
        hp_value.SetText(saveParam.MaxHP.ToString());
        atk_value.SetText(saveParam.Atk.ToString());
        mag_value.SetText(saveParam.Mag.ToString());
        tec_value.SetText(saveParam.Tec.ToString());
        spd_value.SetText(saveParam.Spd.ToString());
        luk_value.SetText(saveParam.Luk.ToString());
        def_value.SetText(saveParam.Def.ToString());
        mdf_value.SetText(saveParam.Mdf.ToString());
        move_value.SetText(saveParam.Move.ToString());

        lv_up.gameObject.SetActive(false);
        hp_up.gameObject.SetActive(false);
        atk_up.gameObject.SetActive(false);
        mag_up.gameObject.SetActive(false);
        tec_up.gameObject.SetActive(false);
        spd_up.gameObject.SetActive(false);
        luk_up.gameObject.SetActive(false);
        def_up.gameObject.SetActive(false);
        mdf_up.gameObject.SetActive(false);
        move_up.gameObject.SetActive(false);
    }

    /// <summary>
    /// 上昇演出
    /// </summary>
    /// <param name="upParam"></param>
    /// <returns></returns>
    private IEnumerator ShowUpParameter(GameDatabase.ParameterData upParam)
    {
        var sound = ManagerSceneScript.GetInstance().soundMan;

        var ShowAct = new Action<TMP_Text, TMP_Text, int, int>((upTxt, valTxt, baseNum, upNum) =>
        {
            sound.PlaySE(se_paramUp);
            if (upNum > 0)
            {
                upTxt.SetText($"+{upNum}");
                upTxt.color = UP_COLOR;
            }
            else
            {
                upTxt.SetText($"-{Math.Abs(upNum)}");
                upTxt.color = DOWN_COLOR;
            }
            upTxt.gameObject.SetActive(true);
            valTxt.SetText((baseNum + upNum).ToString());
        });

        if (upParam.maxHp != 0)
        {
            ShowAct(hp_up, hp_value, saveParam.MaxHP, upParam.maxHp);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.atk != 0)
        {
            ShowAct(atk_up, atk_value, saveParam.Atk, upParam.atk);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.mag != 0)
        {
            ShowAct(mag_up, mag_value, saveParam.Mag, upParam.mag);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.tec != 0)
        {
            ShowAct(tec_up, tec_value, saveParam.Tec, upParam.tec);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.spd != 0)
        {
            ShowAct(spd_up, spd_value, saveParam.Spd, upParam.spd);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.luk != 0)
        {
            ShowAct(luk_up, luk_value, saveParam.Luk, upParam.luk);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.def != 0)
        {
            ShowAct(def_up, def_value, saveParam.Def, upParam.def);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.mdf != 0)
        {
            ShowAct(mdf_up, mdf_value, saveParam.Mdf, upParam.mdf);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
        if (upParam.move != 0)
        {
            ShowAct(move_up, move_value, saveParam.Move, upParam.move);
            yield return new WaitForSeconds(UP_WAITTIME);
        }
    }

    #endregion
}

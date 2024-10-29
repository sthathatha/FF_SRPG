using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステータス詳細画面
/// </summary>
public class StatusScreen : MonoBehaviour
{
    #region 定数

    /// <summary>仲間のウィンドウ色</summary>
    private readonly Color PLAYER_COLOR = new Color(0f, 0.4f, 0.4f, 1f);
    /// <summary>敵のウィンドウ色</summary>
    private readonly Color ENEMY_COLOR = new Color(0.4f, 0f, 0f, 1f);

    #endregion

    #region メンバー

    // イラスト
    public Image faceImage;

    // 背景色設定用
    public Image classWindowBG;
    public Image weaponWindowBG;
    public Image dropWindowBG;
    public Image skillWindowBG;
    public Image statusWindowBG;

    // 表示切替用
    public GameObject weaponWindow;
    public GameObject dropWindow;
    public GameObject skillWindow;

    // 値表示用
    public TMP_Text txt_className;
    public TMP_Text txt_lv;
    public TMP_Text txt_exp;
    public TMP_Text txt_hp;
    public TMP_Text txt_atk;
    public TMP_Text txt_mag;
    public TMP_Text txt_tec;
    public TMP_Text txt_spd;
    public TMP_Text txt_luk;
    public TMP_Text txt_def;
    public TMP_Text txt_mdf;
    public TMP_Text txt_move;

    //todo: スキル表示用

    #endregion

    #region 表示制御

    /// <summary>
    /// 表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region 内容セット

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="chr"></param>
    public void DispParameter(CharacterBase chr)
    {
        // 共通部分表示
        txt_hp.SetText($"{chr.param.HP} / {chr.param.MaxHP}");
        txt_atk.SetText(chr.param.Atk.ToString());
        txt_mag.SetText(chr.param.Mag.ToString());
        txt_tec.SetText(chr.param.Tec.ToString());
        txt_spd.SetText(chr.param.Spd.ToString());
        txt_luk.SetText(chr.param.Luk.ToString());
        txt_def.SetText(chr.param.Def.ToString());
        txt_mdf.SetText(chr.param.Mdf.ToString());
        txt_move.SetText(chr.param.Move.ToString());
        txt_lv.SetText(chr.param.Lv.ToString());

        if (chr.IsPlayer())
        {
            DispPlayerParameter((PlayerCharacter)chr);
        }
        else
        {
            DispEnemyParameter((EnemyCharacter)chr);
        }
    }

    /// <summary>
    /// プレイヤー専用表示
    /// </summary>
    /// <param name="chr"></param>
    public void DispPlayerParameter(PlayerCharacter chr)
    {
        var saveParam = chr.GetSaveParameter();
        txt_className.SetText(GameDatabase.Name_Classes_Get(chr.playerID)[(int)saveParam.ClassID]);
        if (saveParam.Lv >= 20)
            txt_exp.SetText("--");
        else
            txt_exp.SetText(saveParam.Exp.ToString());

        classWindowBG.color = PLAYER_COLOR;
        weaponWindowBG.color = PLAYER_COLOR;
        dropWindowBG.color = PLAYER_COLOR;
        skillWindowBG.color = PLAYER_COLOR;
        statusWindowBG.color = PLAYER_COLOR;

        faceImage.sprite = ManagerSceneScript.GetInstance().generalResources.GetFaceIconP(chr.playerID);
    }

    /// <summary>
    /// 敵専用表示
    /// </summary>
    /// <param name="chr"></param>
    public void DispEnemyParameter(EnemyCharacter chr)
    {
        txt_className.SetText(chr.GetEnemyName());
        txt_exp.SetText("--");

        classWindowBG.color = ENEMY_COLOR;
        weaponWindowBG.color = ENEMY_COLOR;
        dropWindowBG.color = ENEMY_COLOR;
        skillWindowBG.color = ENEMY_COLOR;
        statusWindowBG.color = ENEMY_COLOR;

        faceImage.sprite = ManagerSceneScript.GetInstance().generalResources.GetFaceIconE(chr.enemyID);
    }

    #endregion
}

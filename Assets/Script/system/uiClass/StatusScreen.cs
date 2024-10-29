using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �X�e�[�^�X�ڍ׉��
/// </summary>
public class StatusScreen : MonoBehaviour
{
    #region �萔

    /// <summary>���Ԃ̃E�B���h�E�F</summary>
    private readonly Color PLAYER_COLOR = new Color(0f, 0.4f, 0.4f, 1f);
    /// <summary>�G�̃E�B���h�E�F</summary>
    private readonly Color ENEMY_COLOR = new Color(0.4f, 0f, 0f, 1f);

    #endregion

    #region �����o�[

    // �C���X�g
    public Image faceImage;

    // �w�i�F�ݒ�p
    public Image classWindowBG;
    public Image weaponWindowBG;
    public Image dropWindowBG;
    public Image skillWindowBG;
    public Image statusWindowBG;

    // �\���ؑ֗p
    public GameObject weaponWindow;
    public GameObject dropWindow;
    public GameObject skillWindow;

    // �l�\���p
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

    //todo: �X�L���\���p

    #endregion

    #region �\������

    /// <summary>
    /// �\��
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

    #region ���e�Z�b�g

    /// <summary>
    /// �\��
    /// </summary>
    /// <param name="chr"></param>
    public void DispParameter(CharacterBase chr)
    {
        // ���ʕ����\��
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
    /// �v���C���[��p�\��
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
    /// �G��p�\��
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
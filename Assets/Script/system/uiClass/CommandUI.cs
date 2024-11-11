using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �R�}���h�I��UI
/// </summary>
public class CommandUI : MonoBehaviour
{
    private const float COMMAND_X = 200f;

    #region �߂�l

    /// <summary>
    /// �R�}���h�I��߂�l
    /// </summary>
    public enum CommandResult
    {
        Cancel = 0,
        Act,
        Equip,
        ClassChange,
        Wait,
    }
    public CommandResult Result { get; private set; }

    #endregion

    #region �����o�[

    public RectTransform window;
    public Button ccText;
    public Button escText;

    #endregion

    #region �ϐ�

    private bool showing = false;
    private bool ccEnable = false;

    #endregion

    #region �\��
    /// <summary>
    /// �\��
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    public IEnumerator ShowCoroutine(PlayerCharacter pc)
    {
        showing = true;
        Result = CommandResult.Cancel;
        SetCommands(pc);
        gameObject.SetActive(true);
        // �Ȃɂ��̃{�^���������܂�
        yield return new WaitWhile(() => showing);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �L�����̏�Ԃɂ���đI�����̒���
    /// </summary>
    /// <param name="pc"></param>
    private void SetCommands(PlayerCharacter pc)
    {
        // �������E�ɋ���ꍇ���ɒu��
        var pos = window.localPosition;
        if (pc.GetLocation().x >= FieldSystem.COL_COUNT / 2)
            pos.x = -COMMAND_X;
        else
            pos.x = COMMAND_X;
        window.localPosition = pos;

        // 2���E�̏ꍇLv20�ȏ�A����ȊO�̏ꍇLv10�ȏ�̎��̂݃N���X�`�F���W�\
        ccEnable = false;
        var saveParam = pc.GetSaveParameter();
        if (saveParam.ClassID == Constant.ClassID.A2 ||
            saveParam.ClassID == Constant.ClassID.B2 ||
            saveParam.ClassID == Constant.ClassID.AB)
            ccEnable = pc.param.Lv >= 20;
        else
            ccEnable = pc.param.Lv >= 10;

        ccText.interactable = ccEnable;
    }

    #endregion

    /// <summary>
    /// �s���{�^��
    /// </summary>
    public void BtnAct()
    {
        Result = CommandResult.Act;
        showing = false;
    }

    /// <summary>
    /// �����ύX�{�^��
    /// </summary>
    public void BtnEquip()
    {
        Result = CommandResult.Equip;
        showing = false;
    }

    /// <summary>
    /// �ҋ@
    /// </summary>
    public void BtnWait()
    {
        Result = CommandResult.Wait;
        showing = false;
    }

    /// <summary>
    /// �N���X�`�F���W
    /// </summary>
    public void BtnCC()
    {
        Result = CommandResult.ClassChange;
        showing = false;
    }

    /// <summary>
    /// �L�����Z��
    /// </summary>
    public void BtnCancel()
    {
        Result = CommandResult.Cancel;
        showing = false;
    }
}

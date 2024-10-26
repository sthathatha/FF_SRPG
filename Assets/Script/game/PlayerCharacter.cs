using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    #region �����o�[

    /// <summary>�N���X�\��1</summary>
    public GameObject classIcon1;
    /// <summary>�N���X�\��2</summary>
    public GameObject classIcon2;

    #endregion

    #region �ϐ�

    private Constant.PlayerID playerID;

    #endregion

    #region �p�����[�^

    /// <summary>�v���C���[�t���O</summary>
    /// <returns></returns>
    public override bool IsPlayer() { return true; }

    #endregion

    #region ���f��

    /// <summary>
    /// �L�����Z�b�g
    /// </summary>
    /// <param name="pid"></param>
    public void SetCharacter(Constant.PlayerID pid)
    {
        playerID = pid;

        var resources = field.GetComponent<FieldCharacterModels>();
        anim.runtimeAnimatorController = pid switch
        {
            Constant.PlayerID.Drows => resources.anim_drows_base,
            Constant.PlayerID.Eraps => resources.anim_eraps_base,
            Constant.PlayerID.Exa => resources.anim_exa_base,
            Constant.PlayerID.Worra => resources.anim_worra_base,
            Constant.PlayerID.Koob => resources.anim_koob_base,
            _ => resources.anim_you_base,
        };
    }

    #endregion
}

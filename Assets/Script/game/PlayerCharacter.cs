using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    #region メンバー

    /// <summary>クラス表示1</summary>
    public GameObject classIcon1;
    /// <summary>クラス表示2</summary>
    public GameObject classIcon2;

    #endregion

    #region 変数

    private Constant.PlayerID playerID;

    #endregion

    #region パラメータ

    /// <summary>プレイヤーフラグ</summary>
    /// <returns></returns>
    public override bool IsPlayer() { return true; }

    #endregion

    #region モデル

    /// <summary>
    /// キャラセット
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

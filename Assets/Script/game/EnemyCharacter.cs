using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵キャラクター
/// </summary>
public class EnemyCharacter : CharacterBase
{
    #region メンバー

    #endregion

    #region 変数

    private Constant.EnemyID enemyID;

    #endregion

    #region パラメータ

    /// <summary>
    /// 初期パラメータ設定
    /// </summary>
    /// <param name="lv"></param>
    public void InitParameter(int lv)
    {
        param.InitEnemy(enemyID, lv);
    }



    #endregion

    #region モデル

    /// <summary>
    /// キャラセット
    /// </summary>
    /// <param name="eid"></param>
    /// <param name="lv"></param>
    public void SetCharacter(Constant.EnemyID eid)
    {
        enemyID = eid;

        var resources = field.GetComponent<FieldCharacterModels>();
        anim.runtimeAnimatorController = eid switch
        {
            Constant.EnemyID.GreenSlime => resources.anim_slime_base,
            _ => resources.anim_slime_base,
        };
    }

    #endregion
}

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

    /// <summary>敵ID</summary>
    public Constant.EnemyID enemyID { get; private set; }

    /// <summary>使用武器ID</summary>
    public GameDatabase.ItemID weaponID { get; private set; }
    /// <summary>ドロップアイテムID</summary>
    public GameDatabase.ItemID dropID { get; private set; }

    #endregion

    #region パラメータ

    /// <summary>
    /// 初期パラメータ設定
    /// </summary>
    /// <param name="lv"></param>
    public void InitParameter(int lv)
    {
        param.InitEnemy(enemyID, lv);

        UpdateHP(true);
    }

    /// <summary>
    /// 名前取得
    /// </summary>
    /// <returns></returns>
    public string GetEnemyName() { return GameDatabase.Name_Enemies[(int)enemyID]; }

    /// <summary>
    /// 
    /// </summary>
    public void SetWeaponAndDrop()
    {
        weaponID = GameDatabase.ItemID.FreeHand;
        dropID = GameDatabase.ItemID.Sword1;
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

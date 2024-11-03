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
    /// 使用武器とドロップを設定
    /// </summary>
    /// <param name="wpn">未指定の場合は敵IDごとの既定値</param>
    /// <param name="drp">未指定の場合はランダム？</param>
    public void SetWeaponAndDrop(GameDatabase.ItemID wpn = GameDatabase.ItemID.FreeHand, GameDatabase.ItemID drp = GameDatabase.ItemID.FreeHand)
    {
        if (wpn != GameDatabase.ItemID.FreeHand) { weaponID = wpn; }
        else
        {
            weaponID = GameDatabase.ItemID.FreeHand;
        }
        if (drp != GameDatabase.ItemID.FreeHand) { dropID = drp; }
        else
        {
            dropID = GameDatabase.ItemID.FreeHand;
        }
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

        anim.GetComponent<SpriteRenderer>().color = GameDatabase.GetEnemyColor(enemyID);
    }

    #endregion
}

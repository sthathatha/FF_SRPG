using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 常時保持素材
/// </summary>
public class GeneralResources : MonoBehaviour
{
    #region 顔アイコン

    public Sprite face_Drows;
    public Sprite face_Eraps;
    public Sprite face_Exa;
    public Sprite face_Worra;
    public Sprite face_Koob;
    public Sprite face_You;

    public Sprite face_Monster;

    /// <summary>
    /// プレイヤー顔画像
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public Sprite GetFaceIconP(Constant.PlayerID pid)
    {
        return pid switch
        {
            Constant.PlayerID.Drows => face_Drows,
            Constant.PlayerID.Eraps => face_Eraps,
            Constant.PlayerID.Exa => face_Exa,
            Constant.PlayerID.Worra => face_Worra,
            Constant.PlayerID.Koob => face_Koob,
            _ => face_You
        };
    }

    /// <summary>
    /// モンスター顔画像
    /// </summary>
    /// <param name="eid"></param>
    /// <returns></returns>
    public Sprite GetFaceIconE(Constant.EnemyID eid)
    {
        return eid switch
        {
            Constant.EnemyID.GreenSlime => face_Monster,
            _ => face_Monster,
        };
    }

    #endregion

    #region アイテムアイコン

    public Sprite item_hand;
    public Sprite item_sword;
    public Sprite item_spear;
    public Sprite item_axe;
    public Sprite item_arrow;
    public Sprite item_book;
    public Sprite item_rod;
    public Sprite item_item;

    /// <summary>
    /// アイテムアイコン
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Sprite GetItemIcon(GameDatabase.ItemType type)
    {
        return type switch
        {
            GameDatabase.ItemType.Sword => item_sword,
            GameDatabase.ItemType.Spear => item_spear,
            GameDatabase.ItemType.Axe => item_axe,
            GameDatabase.ItemType.Arrow => item_arrow,
            GameDatabase.ItemType.Book => item_book,
            GameDatabase.ItemType.Rod => item_rod,
            GameDatabase.ItemType.Item => item_item,
            _ => item_hand
        };
    }

    #endregion
}

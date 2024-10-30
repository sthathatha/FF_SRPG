using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムリストのUI１個
/// </summary>
public class ItemListUIItem : MonoBehaviour
{
    #region メンバー

    public ItemListUI parentUI;

    public Image icon;
    public TMP_Text txtName;
    public TMP_Text txtCount;

    #endregion

    private int itemIndex;

    /// <summary>
    /// 表示内容セット
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void SetItem(GameDatabase.ItemID item, int count, int index)
    {
        itemIndex = index;
        var prm = GameDatabase.ItemDataList[(int)item];

        icon.sprite = ManagerSceneScript.GetInstance().generalResources.GetItemIcon(prm.iType);
        txtName.SetText(prm.name);
        txtCount.SetText(count < 0 ? "--" : count.ToString());
    }

    /// <summary>
    /// クリック
    /// </summary>
    public void ClickItem()
    {
        parentUI.ClickItem(itemIndex);
    }
}

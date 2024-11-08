using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 図鑑のアイテム項目
/// </summary>
public class ItemDictionaryUIItem : MonoBehaviour
{
    #region メンバー

    public DictionarySceneScript system;

    public Image icon;
    public TMP_Text txtName;

    #endregion

    private GameDatabase.ItemID itemid;

    /// <summary>
    /// 表示内容セット
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(GameDatabase.ItemID item)
    {
        itemid = item;
        var prm = GameDatabase.ItemDataList[(int)item];

        // 入手済みの場合表示・ボタン有効
        if (Global.GetSaveData().system.getItems[(int)item] == 1)
        {
            GetComponent<Button>().interactable = true;
            txtName.SetText(prm.name);
        }
        else
        {
            GetComponent<Button>().interactable = false;
            txtName.SetText("？？？？？");
        }

        icon.sprite = ManagerSceneScript.GetInstance().generalResources.GetItemIcon(prm.iType);
    }

    /// <summary>
    /// クリック
    /// </summary>
    public void ClickItem()
    {
        system.ShowItemStatus(itemid);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 図鑑画面のアイテムリスト
/// </summary>
public class ItemDictionaryUI : MonoBehaviour
{
    /// <summary>アイテム１個の高さ</summary>
    private const float ITEM_HEIGHT = 44f;

    #region メンバー

    public ItemDictionaryUIItem item_dummy;
    public RectTransform item_parent;

    public Toggle tgl_sword;
    public Toggle tgl_spear;
    public Toggle tgl_axe;
    public Toggle tgl_arrow;
    public Toggle tgl_book;
    public Toggle tgl_rod;
    public Toggle tgl_item;

    #endregion

    #region 変数

    private List<ItemDictionaryUIItem> items = new List<ItemDictionaryUIItem>();

    #endregion

    /// <summary>
    /// トグル変更
    /// </summary>
    public void ChangeSwitch()
    {
        CreateItemList();
    }

    #region リスト作成

    /// <summary>
    /// アイテムリスト作成
    /// </summary>
    public void CreateItemList()
    {
        foreach (var itm in items)
        {
            Destroy(itm.gameObject);
        }
        items.Clear();

        for (var i = 0; i < (int)GameDatabase.ItemID.ITEM_COUNT; ++i)
        {
            var id = (GameDatabase.ItemID)i;
            // 素手は非表示
            if (id == GameDatabase.ItemID.FreeHand || id == GameDatabase.ItemID.FreeMagic) continue;

            // トグルスイッチオンの場合のみ
            var itm = GameDatabase.ItemDataList[i];
            if (itm.iType == GameDatabase.ItemType.Sword && !tgl_sword.isOn) continue;
            if (itm.iType == GameDatabase.ItemType.Spear && !tgl_spear.isOn) continue;
            if (itm.iType == GameDatabase.ItemType.Axe && !tgl_axe.isOn) continue;
            if (itm.iType == GameDatabase.ItemType.Arrow && !tgl_arrow.isOn) continue;
            if (itm.iType == GameDatabase.ItemType.Book && !tgl_book.isOn) continue;
            if (itm.iType == GameDatabase.ItemType.Rod && !tgl_rod.isOn) continue;
            if (itm.iType == GameDatabase.ItemType.Item && !tgl_item.isOn) continue;

            var ui = Instantiate(item_dummy, item_parent, false);
            ui.gameObject.SetActive(true);
            ui.SetItem(id);
            ui.transform.localPosition = new Vector3(0, -items.Count * ITEM_HEIGHT);
            items.Add(ui);
        }

        var scl = item_parent.sizeDelta;
        scl.y = items.Count * ITEM_HEIGHT;
        item_parent.sizeDelta = scl;
    }

    #endregion
}

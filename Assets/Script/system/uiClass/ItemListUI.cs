using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムリスト
/// </summary>
public class ItemListUI : MonoBehaviour
{
    /// <summary>アイテム１個の高さ</summary>
    private const float ITEM_HEIGHT = 44f;

    #region メンバー

    public ItemListUIItem item_dummy;
    public RectTransform item_parent;
    public Transform window;

    public Toggle tgl_sword;
    public Toggle tgl_spear;
    public Toggle tgl_axe;
    public Toggle tgl_arrow;
    public Toggle tgl_book;
    public Toggle tgl_rod;
    public Toggle tgl_item;

    #endregion

    #region 変数

    private PlayerCharacter showPc;
    private List<ItemListUIItem> items = new List<ItemListUIItem>();

    #endregion

    #region 使用

    /// <summary>
    /// 選択結果
    /// </summary>
    public enum ItemResult
    {
        Active = 0,
        Cancel,
        Select,
    }
    public ItemResult Result { get; private set; }

    /// <summary>選択アイテム番号</summary>
    public int Result_SelectIndex { get; private set; } = -1;
    /// <summary>選択アイテムのデータ</summary>
    public GameDatabase.ItemData Result_SelectData { get; private set; }

    /// <summary>
    /// アイテム選択開く
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="initFilter">フィルタ初期化</param>
    /// <returns></returns>
    public IEnumerator ShowCoroutine(PlayerCharacter pc, bool initFilter)
    {
        showPc = pc;
        if (initFilter)
        {
            var saveParam = pc.GetSaveParameter();
            var classPrm = GameDatabase.Prm_ClassWeaponRate_Get(pc.playerID, saveParam.ClassID);
            tgl_sword.isOn = classPrm.sword >= 100;
            tgl_spear.isOn = classPrm.spear >= 100;
            tgl_axe.isOn = classPrm.axe >= 100;
            tgl_arrow.isOn = classPrm.arrow >= 100;
            tgl_book.isOn = classPrm.book >= 100;
            tgl_rod.isOn = classPrm.rod >= 100;
            tgl_item.isOn = false;
        }

        CreateItemList(pc);
        // 半分より右に居る場合左に置く
        var pos = window.localPosition;
        if (pc.GetLocation().x >= FieldSystem.COL_COUNT / 2)
            pos.x = -240f;
        else
            pos.x = 240f;
        window.localPosition = pos;

        Result = ItemResult.Active;
        gameObject.SetActive(true);
        yield return new WaitWhile(() => Result == ItemResult.Active);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// キャンセル
    /// </summary>
    public void ClickCancel()
    {
        Result = ItemResult.Cancel;
    }

    /// <summary>
    /// アイテム選択
    /// </summary>
    /// <param name="index"></param>
    public void ClickItem(int index)
    {
        Result_SelectIndex = index;
        Result = ItemResult.Select;
        if (index < 0) Result_SelectData = GameDatabase.ItemDataList[0]; // 素手
        else Result_SelectData = GameParameter.otherData.haveItemList[index].ItemData; // 選択アイテム
    }

    #endregion

    #region トグル

    /// <summary>
    /// トグル変更
    /// </summary>
    public void ChangeSwitch()
    {
        if (Result != ItemResult.Active) return;
        CreateItemList(showPc);
    }

    #endregion

    #region リスト作成

    /// <summary>
    /// アイテムリスト作成
    /// </summary>
    private void CreateItemList(PlayerCharacter pc)
    {
        foreach (var itm in items)
        {
            Destroy(itm.gameObject);
        }
        items.Clear();

        // ドロシーは一番上に素手追加
        if (pc.HasSkill(GameDatabase.SkillID.Drows_FreeHand))
        {
            // 素手
            var ui = Instantiate(item_dummy, item_parent, false);
            ui.gameObject.SetActive(true);

            ui.SetItem(GameDatabase.ItemID.FreeHand, -1, -1);
            ui.transform.localPosition = new Vector3(0, 0);
            items.Add(ui);
        }

        for (var i = 0; i < GameParameter.otherData.haveItemList.Count; ++i)
        {
            // トグルスイッチオンの場合のみ
            var itm = GameParameter.otherData.haveItemList[i];
            if (itm.ItemData.iType == GameDatabase.ItemType.Sword && !tgl_sword.isOn) continue;
            if (itm.ItemData.iType == GameDatabase.ItemType.Spear && !tgl_spear.isOn) continue;
            if (itm.ItemData.iType == GameDatabase.ItemType.Axe && !tgl_axe.isOn) continue;
            if (itm.ItemData.iType == GameDatabase.ItemType.Arrow && !tgl_arrow.isOn) continue;
            if (itm.ItemData.iType == GameDatabase.ItemType.Book && !tgl_book.isOn) continue;
            if (itm.ItemData.iType == GameDatabase.ItemType.Rod && !tgl_rod.isOn) continue;
            if (itm.ItemData.iType == GameDatabase.ItemType.Item && !tgl_item.isOn) continue;

            var ui = Instantiate(item_dummy, item_parent, false);
            ui.gameObject.SetActive(true);

            ui.SetItem(itm.id, itm.useCount, i);
            ui.transform.localPosition = new Vector3(0, -items.Count * ITEM_HEIGHT);
            items.Add(ui);
        }

        var scl = item_parent.sizeDelta;
        scl.y = items.Count * ITEM_HEIGHT;
        item_parent.sizeDelta = scl;
    }

    #endregion
}

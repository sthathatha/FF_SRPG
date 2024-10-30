using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// �A�C�e�����X�g
/// </summary>
public class ItemListUI : MonoBehaviour
{
    /// <summary>�A�C�e���P�̍���</summary>
    private const float ITEM_HEIGHT = 44f;

    #region �����o�[

    public ItemListUIItem item_dummy;
    public RectTransform item_parent;
    public Transform window;

    #endregion

    #region �ϐ�

    private List<ItemListUIItem> items = new List<ItemListUIItem>();

    #endregion

    #region �g�p

    /// <summary>
    /// �I������
    /// </summary>
    public enum ItemResult
    {
        Active = 0,
        Cancel,
        Select,
    }
    public ItemResult Result { get; private set; }

    /// <summary>�I���A�C�e���ԍ�</summary>
    public int Result_SelectIndex { get; private set; } = -1;
    /// <summary>�I���A�C�e���̃f�[�^</summary>
    public GameDatabase.ItemData Result_SelectData { get; private set; }

    /// <summary>
    /// �A�C�e���I���J��
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    public IEnumerator ShowCoroutine(PlayerCharacter pc)
    {
        Result = ItemResult.Active;
        CreateItemList(pc.playerID);
        // �������E�ɋ���ꍇ���ɒu��
        var pos = window.localPosition;
        if (pc.GetLocation().x >= FieldSystem.COL_COUNT / 2)
            pos.x = -240f;
        else
            pos.x = 240f;
        window.localPosition = pos;

        gameObject.SetActive(true);
        yield return new WaitWhile(() => Result == ItemResult.Active);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �L�����Z��
    /// </summary>
    public void ClickCancel()
    {
        Result = ItemResult.Cancel;
    }

    /// <summary>
    /// �A�C�e���I��
    /// </summary>
    /// <param name="index"></param>
    public void ClickItem(int index)
    {
        Result_SelectIndex = index;
        Result = ItemResult.Select;
        if (index < 0) Result_SelectData = GameDatabase.ItemDataList[0]; // �f��
        else Result_SelectData = GameParameter.otherData.haveItemList[index].ItemData; // �I���A�C�e��
    }

    #endregion

    #region ���X�g�쐬

    /// <summary>
    /// �A�C�e�����X�g�쐬
    /// </summary>
    private void CreateItemList(Constant.PlayerID pid)
    {
        foreach (var itm in items)
        {
            Destroy(itm.gameObject);
        }
        items.Clear();

        // �h���V�[�͈�ԏ�ɑf��ǉ�
        if (pid == Constant.PlayerID.Drows)
        {
            // �f��
            var ui = Instantiate(item_dummy, item_parent, false);
            ui.gameObject.SetActive(true);

            ui.SetItem(GameDatabase.ItemID.FreeHand, -1, -1);
            ui.transform.localPosition = new Vector3(0, 0);
            items.Add(ui);
        }

        for (var i = 0; i < GameParameter.otherData.haveItemList.Count; ++i)
        {
            var itm = GameParameter.otherData.haveItemList[i];
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

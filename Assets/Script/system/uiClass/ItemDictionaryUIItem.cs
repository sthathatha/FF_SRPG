using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �}�ӂ̃A�C�e������
/// </summary>
public class ItemDictionaryUIItem : MonoBehaviour
{
    #region �����o�[

    public DictionarySceneScript system;

    public Image icon;
    public TMP_Text txtName;

    #endregion

    private GameDatabase.ItemID itemid;

    /// <summary>
    /// �\�����e�Z�b�g
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(GameDatabase.ItemID item)
    {
        itemid = item;
        var prm = GameDatabase.ItemDataList[(int)item];

        // ����ς݂̏ꍇ�\���E�{�^���L��
        if (Global.GetSaveData().system.getItems[(int)item] == 1)
        {
            GetComponent<Button>().interactable = true;
            txtName.SetText(prm.name);
        }
        else
        {
            GetComponent<Button>().interactable = false;
            txtName.SetText("�H�H�H�H�H");
        }

        icon.sprite = ManagerSceneScript.GetInstance().generalResources.GetItemIcon(prm.iType);
    }

    /// <summary>
    /// �N���b�N
    /// </summary>
    public void ClickItem()
    {
        system.ShowItemStatus(itemid);
    }
}

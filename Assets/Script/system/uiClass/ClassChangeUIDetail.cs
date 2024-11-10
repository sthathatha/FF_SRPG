using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// クラスチェンジUI１個
/// </summary>
public class ClassChangeUIDetail : MonoBehaviour
{
    public TMP_Text txt_name;
    public TMP_Text txt_sword;
    public TMP_Text txt_spear;
    public TMP_Text txt_axe;
    public TMP_Text txt_arrow;
    public TMP_Text txt_book;
    public TMP_Text txt_rod;

    /// <summary>
    /// クラス表示
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="cid"></param>
    public void SetClass(Constant.PlayerID pid, Constant.ClassID cid)
    {
        txt_name.SetText(GameDatabase.Name_Classes_Get(pid)[(int)cid]);
        var cls = GameDatabase.Prm_ClassWeaponRate_Get(pid, cid);

        txt_sword.SetText(cls.sword.ToString());
        txt_spear.SetText(cls.spear.ToString());
        txt_axe.SetText(cls.axe.ToString());
        txt_arrow.SetText(cls.arrow.ToString());
        txt_book.SetText(cls.book.ToString());
        txt_rod.SetText(cls.rod.ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステータス詳細画面
/// </summary>
public class StatusScreen : MonoBehaviour
{
    #region 定数

    /// <summary>仲間のウィンドウ色</summary>
    private readonly Color PLAYER_COLOR = new Color(0f, 0.4f, 0.4f, 1f);
    /// <summary>敵のウィンドウ色</summary>
    private readonly Color ENEMY_COLOR = new Color(0.4f, 0f, 0f, 1f);

    #endregion

    #region メンバー

    // イラスト
    public Image faceImage;

    // 背景色設定用
    public Image classWindowBG;
    public Image weaponWindowBG;
    public Image dropWindowBG;
    public Image skillWindowBG;
    public Image statusWindowBG;
    public Image weaponRateWindowBG;

    // 表示切替用
    public GameObject weaponWindow;
    public GameObject dropWindow;
    public GameObject weaponRateWindow;
    public GameObject skillWindow;
    public GameObject realAttackWindow;

    // 値表示用
    public TMP_Text txt_className;
    public TMP_Text txt_lv;
    public TMP_Text txt_exp;
    public TMP_Text txt_hp;
    public TMP_Text txt_atk;
    public TMP_Text txt_mag;
    public TMP_Text txt_tec;
    public TMP_Text txt_spd;
    public TMP_Text txt_luk;
    public TMP_Text txt_def;
    public TMP_Text txt_mdf;
    public TMP_Text txt_move;

    public TMP_Text txt_rate_sword;
    public TMP_Text txt_rate_spear;
    public TMP_Text txt_rate_axe;
    public TMP_Text txt_rate_arrow;
    public TMP_Text txt_rate_book;
    public TMP_Text txt_rate_rod;

    public Image icon_weapon;
    public TMP_Text txt_weapon;
    public Image icon_dropItem;
    public TMP_Text txt_dropItem;
    public TMP_Text txt_realAttack;

    // スキル表示用
    public Transform skill_parent;
    public TMP_Text skill_dummy;
    private List<TMP_Text> skills = new List<TMP_Text>();

    #endregion

    #region 表示制御

    /// <summary>
    /// 表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region 内容セット

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="chr"></param>
    public void DispParameter(CharacterBase chr)
    {
        // 共通部分表示
        txt_hp.SetText($"{chr.param.HP} / {chr.param.MaxHP}");
        txt_atk.SetText(chr.param.Atk.ToString());
        txt_mag.SetText(chr.param.Mag.ToString());
        txt_tec.SetText(chr.param.Tec.ToString());
        txt_spd.SetText(chr.param.Spd.ToString());
        txt_luk.SetText(chr.param.Luk.ToString());
        txt_def.SetText(chr.param.Def.ToString());
        txt_mdf.SetText(chr.param.Mdf.ToString());
        txt_move.SetText(chr.param.Move.ToString());
        txt_lv.SetText(chr.param.Lv.ToString());

        foreach (var s in skills) Destroy(s.gameObject);
        skills.Clear();

        if (chr.IsPlayer())
        {
            DispPlayerParameter((PlayerCharacter)chr);
        }
        else
        {
            DispEnemyParameter((EnemyCharacter)chr);
        }
    }

    /// <summary>
    /// プレイヤー専用表示
    /// </summary>
    /// <param name="chr"></param>
    public void DispPlayerParameter(PlayerCharacter chr)
    {
        var saveParam = chr.GetSaveParameter();
        txt_className.SetText(GameDatabase.Name_Classes_Get(chr.playerID)[(int)saveParam.ClassID]);
        if (saveParam.Lv >= 20)
            txt_exp.SetText("--");
        else
            txt_exp.SetText(saveParam.Exp.ToString());

        classWindowBG.color = PLAYER_COLOR;
        skillWindowBG.color = PLAYER_COLOR;
        statusWindowBG.color = PLAYER_COLOR;
        weaponRateWindowBG.color = PLAYER_COLOR;
        weaponWindow.SetActive(false);
        dropWindow.SetActive(false);
        weaponRateWindow.SetActive(true);
        realAttackWindow.SetActive(false);

        // 武器熟練度
        var weaponRate = GameDatabase.Prm_ClassWeaponRate_Get(chr.playerID, saveParam.ClassID);
        txt_rate_sword.SetText(weaponRate.sword.ToString());
        txt_rate_spear.SetText(weaponRate.spear.ToString());
        txt_rate_axe.SetText(weaponRate.axe.ToString());
        txt_rate_arrow.SetText(weaponRate.arrow.ToString());
        txt_rate_book.SetText(weaponRate.book.ToString());
        txt_rate_rod.SetText(weaponRate.rod.ToString());

        faceImage.sprite = ManagerSceneScript.GetInstance().generalResources.GetFaceIconP(chr.playerID);

        // スキル
        foreach (var sid in saveParam.Skills)
        {
            var s = Instantiate(skill_dummy, skill_parent, false);
            s.rectTransform.localPosition = new Vector3(0, skills.Count * -40f);
            s.SetText(GameDatabase.SkillDataList[sid].detail);
            s.gameObject.SetActive(true);
            skills.Add(s);
        }
    }

    /// <summary>
    /// 敵専用表示
    /// </summary>
    /// <param name="chr"></param>
    public void DispEnemyParameter(EnemyCharacter chr)
    {
        var manager = ManagerSceneScript.GetInstance();

        txt_className.SetText(chr.GetEnemyName());
        txt_exp.SetText("--");

        classWindowBG.color = ENEMY_COLOR;
        weaponWindowBG.color = ENEMY_COLOR;
        dropWindowBG.color = ENEMY_COLOR;
        skillWindowBG.color = ENEMY_COLOR;
        statusWindowBG.color = ENEMY_COLOR;
        weaponWindow.SetActive(true);
        dropWindow.SetActive(true);
        weaponRateWindow.SetActive(false);
        realAttackWindow.SetActive(true);

        // 所持武器
        var weaponData = GameDatabase.ItemDataList[(int)chr.weaponID];
        var dropData = GameDatabase.ItemDataList[(int)chr.dropID];
        icon_weapon.sprite = manager.generalResources.GetItemIcon(weaponData.iType);
        txt_weapon.SetText(weaponData.name);
        if (chr.dropID == GameDatabase.ItemID.FreeHand)
        {
            icon_dropItem.gameObject.SetActive(false);
            txt_dropItem.SetText("--");
        }
        else
        {
            icon_dropItem.gameObject.SetActive(true);
            icon_dropItem.sprite = manager.generalResources.GetItemIcon(dropData.iType);
            txt_dropItem.SetText(dropData.name);
        }

        // 実攻撃力
        var realAtk = 0;
        if (weaponData.is_melee())
            realAtk = GameParameter.CalcWeaponValue(chr.param.Atk, weaponData.atk);
        else
            realAtk = GameParameter.CalcWeaponValue(chr.param.Mag, weaponData.atk);
        txt_realAttack.SetText(realAtk.ToString());

        faceImage.sprite = manager.generalResources.GetFaceIconE(chr.enemyID);
    }

    #endregion
}

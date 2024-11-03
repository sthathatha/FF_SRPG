using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    #region メンバー

    /// <summary>クラス表示1</summary>
    public GameObject classIcon1;
    /// <summary>クラス表示2</summary>
    public GameObject classIcon2;

    #endregion

    #region 変数

    public Constant.PlayerID playerID { get; private set; }

    #endregion

    #region パラメータ

    /// <summary>プレイヤーフラグ</summary>
    /// <returns></returns>
    public override bool IsPlayer() { return true; }

    /// <summary>
    /// 初期パラメータ読み込み
    /// </summary>
    public void InitParameter()
    {
        param.InitPlayer(playerID);

        UpdateHP(true);
        UpdateClassIcon();
    }

    /// <summary>
    /// セーブパラメーター
    /// </summary>
    /// <returns></returns>
    public GameParameter.PlayerSaveParameter GetSaveParameter() { return GameParameter.Prm_Get(playerID); }

    /// <summary>
    /// クラスチェンジ表示アイコンの更新
    /// </summary>
    public void UpdateClassIcon()
    {
        var chrData = GetSaveParameter();
        if (chrData.ClassID == Constant.ClassID.Base)
        {
            classIcon1.SetActive(false);
            classIcon2.SetActive(false);
        }
        else if (chrData.ClassID == Constant.ClassID.A || chrData.ClassID == Constant.ClassID.B)
        {
            classIcon1.SetActive(true);
            classIcon2.SetActive(false);
        }
        else
        {
            classIcon1.SetActive(true);
            classIcon2.SetActive(true);
        }
    }

    /// <summary>
    /// 装備中の武器
    /// </summary>
    /// <returns></returns>
    public GameDatabase.ItemID GetEquipWeapon()
    {
        var idx = GameParameter.otherData.GetEquipIndex(playerID);
        if (idx < 0) return GameDatabase.ItemID.FreeHand;

        return GameParameter.otherData.haveItemList[idx].id;
    }

    /// <summary>
    /// 射程
    /// </summary>
    /// <returns></returns>
    public override int GetRangeMin()
    {
        var wid = GetEquipWeapon();
        var wpnData = GameDatabase.ItemDataList[(int)wid];
        return wpnData.rangeMin;
    }

    /// <summary>
    /// 射程
    /// </summary>
    /// <returns></returns>
    public override int GetRangeMax()
    {
        var wid = GetEquipWeapon();
        var wpnData = GameDatabase.ItemDataList[(int)wid];

        //todo:スキルで射程伸びる
        return wpnData.rangeMax;
    }

    #endregion

    #region キャラクター設定

    /// <summary>
    /// キャラセット
    /// </summary>
    /// <param name="pid"></param>
    public void SetCharacter(Constant.PlayerID pid)
    {
        playerID = pid;

        var resources = field.GetComponent<FieldCharacterModels>();
        anim.runtimeAnimatorController = pid switch
        {
            Constant.PlayerID.Drows => resources.anim_drows_base,
            Constant.PlayerID.Eraps => resources.anim_eraps_base,
            Constant.PlayerID.Exa => resources.anim_exa_base,
            Constant.PlayerID.Worra => resources.anim_worra_base,
            Constant.PlayerID.Koob => resources.anim_koob_base,
            _ => resources.anim_you_base,
        };

        InitParameter();
    }

    #endregion

    #region 他

    /// <summary>
    /// アニメーション再生
    /// </summary>
    /// <param name="dir"></param>
    public override void PlayAnim(Constant.Direction dir)
    {
        anim.Play(dir switch
        {
            Constant.Direction.Up => "up",
            Constant.Direction.Down => "down",
            Constant.Direction.Right => "left",
            Constant.Direction.Left => "left",
            _ => "idle",
        });

        // 右アニメは左右反転
        anim.GetComponent<SpriteRenderer>().flipX = dir == Constant.Direction.Right;
    }

    #endregion
}

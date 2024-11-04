using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constant;

/// <summary>
/// �G�L�����N�^�[
/// </summary>
public class EnemyCharacter : CharacterBase
{
    #region �����o�[

    #endregion

    #region �ϐ�

    /// <summary>�GID</summary>
    public Constant.EnemyID enemyID { get; private set; }

    /// <summary>�g�p����ID</summary>
    public GameDatabase.ItemID weaponID { get; private set; }
    /// <summary>�h���b�v�A�C�e��ID</summary>
    public GameDatabase.ItemID dropID { get; private set; }

    /// <summary>�{�X�t���O</summary>
    public bool isBoss { get; private set; } = false;

    #endregion

    #region �Z�[�u

    /// <summary>
    /// �Z�[�u
    /// </summary>
    /// <returns></returns>
    public override string ToSaveString()
    {
        return $"{base.ToSaveString()}e{(int)enemyID}e{(int)weaponID}e{(int)dropID}e{(isBoss ? 1 : 0)}";
    }

    /// <summary>
    /// ���[�h
    /// </summary>
    /// <param name="str"></param>
    public override void FromSaveString(string str)
    {
        var spl = str.Split("e");

        SetCharacter((Constant.EnemyID)int.Parse(spl[1]));
        weaponID = (GameDatabase.ItemID)int.Parse(spl[2]);
        dropID = (GameDatabase.ItemID)int.Parse(spl[3]);
        isBoss = spl[4] == "1";

        base.FromSaveString(spl[0]);
    }

    #endregion

    #region �p�����[�^

    /// <summary>
    /// �����p�����[�^�ݒ�
    /// </summary>
    /// <param name="lv"></param>
    public void InitParameter(int lv)
    {
        param.InitEnemy(enemyID, lv);

        UpdateHP(true);
    }

    /// <summary>
    /// ���O�擾
    /// </summary>
    /// <returns></returns>
    public string GetEnemyName() { return GameDatabase.Name_Enemies[(int)enemyID]; }

    /// <summary>
    /// �g�p����ƃh���b�v��ݒ�
    /// </summary>
    /// <param name="wpn">���w��̏ꍇ�͓GID���Ƃ̊���l</param>
    /// <param name="drp">���w��̏ꍇ�̓����_���H</param>
    public void SetWeaponAndDrop(GameDatabase.ItemID wpn = GameDatabase.ItemID.FreeHand, GameDatabase.ItemID drp = GameDatabase.ItemID.FreeHand)
    {
        if (wpn != GameDatabase.ItemID.FreeHand) { weaponID = wpn; }
        else
        {
            var other = GameDatabase.Prm_EnemyOther[(int)enemyID];
            if (other.defaultWeaponType == GameDatabase.ItemType.None)
            {
                weaponID = param.Atk > param.Mag ? GameDatabase.ItemID.FreeHand : GameDatabase.ItemID.FreeMagic;
            }
            else
            {
                weaponID = GameDatabase.CalcRandomItem(param.Lv, isBoss, true, other.defaultWeaponType);
                if (weaponID == GameDatabase.ItemID.FreeHand)
                    weaponID = param.Atk > param.Mag ? GameDatabase.ItemID.FreeHand : GameDatabase.ItemID.FreeMagic;
            }
        }
        if (drp != GameDatabase.ItemID.FreeHand) { dropID = drp; }
        else
        {
            if (isBoss)
                dropID = GameDatabase.CalcRandomItem(param.Lv, true, false);
            else
                dropID = GameDatabase.CalcRandomItem(param.Lv, false, false, outRate: 98);
        }
    }

    /// <summary>
    /// �˒�
    /// </summary>
    /// <returns></returns>
    public override int GetRangeMin()
    {
        var wpnData = GameDatabase.ItemDataList[(int)weaponID];
        return wpnData.rangeMin;
    }

    /// <summary>
    /// �˒�
    /// </summary>
    /// <returns></returns>
    public override int GetRangeMax()
    {
        var wpnData = GameDatabase.ItemDataList[(int)weaponID];
        return wpnData.rangeMax;
    }

    #endregion

    #region ���f��

    /// <summary>
    /// �L�����Z�b�g
    /// </summary>
    /// <param name="eid"></param>
    /// <param name="lv"></param>
    public void SetCharacter(Constant.EnemyID eid, bool boss = false)
    {
        enemyID = eid;
        isBoss = boss;

        var resources = field.GetComponent<FieldCharacterModels>();
        anim.runtimeAnimatorController = eid switch
        {
            Constant.EnemyID.GreenSlime => resources.anim_slime_base,
            _ => resources.anim_slime_base,
        };

        var other = GameDatabase.Prm_EnemyOther[(int)enemyID];
        anim.GetComponent<SpriteRenderer>().color = other.modelColor;
    }

    #endregion
}

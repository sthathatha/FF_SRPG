using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            // ���A100�ȏ�̕���𑕔����Ă�����h���b�v
            var wpnData = GameDatabase.ItemDataList[(int)weaponID];
            if (wpnData.rarelity > 100)
                dropID = weaponID;
            else if (isBoss)
                dropID = GameDatabase.CalcRandomItem(param.Lv, true, false);
            else
                dropID = GameDatabase.CalcRandomItem(param.Lv, false, false, outRate: 96);
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
            Constant.EnemyID.SlimeGreen => resources.anim_slime_base,
            Constant.EnemyID.SkeletonSword => resources.anim_skeleton_sword,
            Constant.EnemyID.SkeletonSpear => resources.anim_skeleton_spear,
            Constant.EnemyID.SkeletonAxe => resources.anim_skeleton_axe,
            Constant.EnemyID.SkeletonArrow => resources.anim_skeleton_arrow,
            Constant.EnemyID.SkeletonBook => resources.anim_skeleton_book,
            Constant.EnemyID.Pel => resources.anim_pell,
            Constant.EnemyID.SlimeBurn => resources.anim_slime_base,
            Constant.EnemyID.SlimeElec => resources.anim_slime_base,
            Constant.EnemyID.SlimeDeep => resources.anim_slime_base,
            Constant.EnemyID.SlimeMetal => resources.anim_slime_base,

            Constant.EnemyID.Angel => resources.anim_b1_angel,
            Constant.EnemyID.ArchAngel => resources.anim_b2_archangel,
            Constant.EnemyID.Principality => resources.anim_b3_princip,
            Constant.EnemyID.Power => resources.anim_b4_power,
            Constant.EnemyID.Virtue => resources.anim_b5_virtue,
            Constant.EnemyID.Dominion => resources.anim_b6_dominion,
            Constant.EnemyID.Throne => resources.anim_b7_throne,
            Constant.EnemyID.Cherubim => resources.anim_b8_cherubim,
            Constant.EnemyID.Seraphim => resources.anim_b9_seraphim,
            Constant.EnemyID.Prime => resources.anim_b10_prime,
            _ => resources.anim_b8_cherubim,
        };

        var other = GameDatabase.Prm_EnemyOther[(int)enemyID];
        anim.GetComponent<SpriteRenderer>().color = other.modelColor;
    }

    #endregion
}

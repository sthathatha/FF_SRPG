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
            weaponID = GameDatabase.ItemID.FreeHand;
        }
        if (drp != GameDatabase.ItemID.FreeHand) { dropID = drp; }
        else
        {
            dropID = GameDatabase.ItemID.FreeHand;
        }
    }

    #endregion

    #region ���f��

    /// <summary>
    /// �L�����Z�b�g
    /// </summary>
    /// <param name="eid"></param>
    /// <param name="lv"></param>
    public void SetCharacter(Constant.EnemyID eid)
    {
        enemyID = eid;

        var resources = field.GetComponent<FieldCharacterModels>();
        anim.runtimeAnimatorController = eid switch
        {
            Constant.EnemyID.GreenSlime => resources.anim_slime_base,
            _ => resources.anim_slime_base,
        };

        anim.GetComponent<SpriteRenderer>().color = GameDatabase.GetEnemyColor(enemyID);
    }

    #endregion
}

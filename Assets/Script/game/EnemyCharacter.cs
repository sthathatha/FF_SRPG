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
    /// 
    /// </summary>
    public void SetWeaponAndDrop()
    {
        weaponID = GameDatabase.ItemID.FreeHand;
        dropID = GameDatabase.ItemID.Sword1;
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
    }

    #endregion
}

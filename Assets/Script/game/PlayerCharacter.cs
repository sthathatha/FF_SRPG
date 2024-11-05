using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    #region �����o�[

    /// <summary>�N���X�\��1</summary>
    public GameObject classIcon1;
    /// <summary>�N���X�\��2</summary>
    public GameObject classIcon2;

    #endregion

    #region �ϐ�

    public Constant.PlayerID playerID { get; private set; }

    #endregion

    #region �Z�[�u

    /// <summary>
    /// �Z�[�u
    /// </summary>
    /// <returns></returns>
    public override string ToSaveString()
    {
        return $"{base.ToSaveString()}p{(int)playerID}";
    }

    /// <summary>
    /// ���[�h
    /// </summary>
    /// <param name="str"></param>
    public override void FromSaveString(string str)
    {
        var spl = str.Split("p");
        SetCharacter((Constant.PlayerID)int.Parse(spl[1]));

        base.FromSaveString(spl[0]);
    }

    #endregion

    #region �p�����[�^

    /// <summary>�v���C���[�t���O</summary>
    /// <returns></returns>
    public override bool IsPlayer() { return true; }

    /// <summary>
    /// �����p�����[�^�ǂݍ���
    /// </summary>
    public void InitParameter()
    {
        param.InitPlayer(playerID);

        UpdateHP(true);
        UpdateClassIcon();
    }

    /// <summary>
    /// �Z�[�u�p�����[�^�[
    /// </summary>
    /// <returns></returns>
    public GameParameter.PlayerSaveParameter GetSaveParameter() { return GameParameter.Prm_Get(playerID); }

    /// <summary>
    /// �N���X�`�F���W�\���A�C�R���̍X�V
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
    /// �������̕���
    /// </summary>
    /// <returns></returns>
    public GameDatabase.ItemID GetEquipWeapon()
    {
        var idx = GameParameter.otherData.GetEquipIndex(playerID);
        if (idx < 0) return GameDatabase.ItemID.FreeHand;

        return GameParameter.otherData.haveItemList[idx].id;
    }

    /// <summary>
    /// �˒�
    /// </summary>
    /// <returns></returns>
    public override int GetRangeMin()
    {
        var wid = GetEquipWeapon();
        var wpnData = GameDatabase.ItemDataList[(int)wid];
        return wpnData.rangeMin;
    }

    /// <summary>
    /// �˒�
    /// </summary>
    /// <returns></returns>
    public override int GetRangeMax()
    {
        var wid = GetEquipWeapon();
        var wpnData = GameDatabase.ItemDataList[(int)wid];

        //todo:�X�L���Ŏ˒��L�т�
        return wpnData.rangeMax;
    }

    #endregion

    #region �L�����N�^�[�ݒ�

    /// <summary>
    /// �L�����Z�b�g
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

    #region ��

    /// <summary>
    /// �A�j���[�V�����Đ�
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

        // �E�A�j���͍��E���]
        anim.GetComponent<SpriteRenderer>().flipX = dir == Constant.Direction.Right;
    }

    /// <summary>
    ///  �X�L�������Ă��邩
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public override bool HasSkill(GameDatabase.SkillID sid)
    {
        var prm = GetSaveParameter();
        return prm.Skills.Contains((int)sid);
    }

    /// <summary>
    /// �X�L���擾����
    /// </summary>
    public override void CheckGetSkill()
    {
        base.CheckGetSkill();

        var prm = GetSaveParameter();
        var getList = new List<int>();
        for (var i = 0; i < (int)GameDatabase.SkillID.SKILL_COUNT; ++i)
        {
            if (HasSkill((GameDatabase.SkillID)i)) continue;

            var s = GameDatabase.SkillDataList[i];
            if (s.getPlayer == playerID &&
                s.getClass == prm.ClassID &&
                s.getLevel <= prm.Lv)
                getList.Add(i);
        }

        prm.Skills.AddRange(getList);
    }

    /// <summary>
    /// �X�L���Y�p����
    /// </summary>
    public override void CheckDeleteSkill()
    {
        base.CheckDeleteSkill();

        var prm = GetSaveParameter();
        for(var i = prm.Skills.Count-1; i >= 0; --i)
        {
            var s = GameDatabase.SkillDataList[prm.Skills[i]];
            if (s.canKeep) continue;

            if (s.getPlayer != playerID ||
                s.getClass != prm.ClassID ||
                s.getLevel > prm.Lv)
                prm.Skills.RemoveAt(i);
        }
    }

    #endregion
}

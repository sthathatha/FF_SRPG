using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �{��
/// </summary>
public class GameSceneScript : MainScriptBase
{
    #region �萔

    private readonly Color COLOR_TILE_ATTACK = new Color(1f, 0.2f, 0.2f, 0.6f);
    private readonly Color COLOR_TILE_MOVE = new Color(0.2f, 0.5f, 1, 0.6f);
    private readonly Color COLOR_TILE_HEAL = new Color(0.2f, 1f, 0.2f, 0.6f);

    #endregion

    #region �����o�[

    public FieldSystem field;

    #endregion

    #region ����

    /// <summary>
    /// �t�F�[�h�C���O�̏���
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BeforeInitFadeIn()
    {
        yield return base.BeforeInitFadeIn();

        field.InitField();
    }

    /// <summary>
    /// �t�F�[�h�C���I����
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    public override IEnumerator AfterFadeIn(bool init)
    {
        yield return base.AfterFadeIn(init);

        StartCoroutine(GameCoroutine());
    }

    #endregion

    /// <summary>
    /// �Q�[������Ǘ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameCoroutine()
    {
        while (true)
        {
            yield return PlayerTurn();
            field.ResetAllActable();

            yield return EnemyTurn();
            field.ResetAllActable();
        }
    }

    /// <summary>
    /// �I�v�V�����{�^���N���b�N
    /// </summary>
    public void OptionClick()
    {

    }

    #region �v���C���[�^�[��

    /// <summary>
    /// �v���C���[�^�[������
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerTurn()
    {
        var manager = ManagerSceneScript.GetInstance();

        // �^�[���\��
        yield return manager.turnDisplay.DisplayTurnStart(true);

        while (true)
        {
            // �N���b�N�҂�
            yield return field.WaitInput(true);

            if (field.InputResult == FieldSystem.InputResultEnum.TurnEnd)
            {
                break;
            }

            var chr = field.GetCellCharacter(field.InputLoc);
            if (chr == null)
            {
                // �L�����̋��Ȃ��ꏊ���N���b�N�̏ꍇ
                field.ClearTiles();
                continue;
            }
            else if (!chr.IsPlayer())
            {
                // �G���N���b�N�����ꍇ
                var moveList = field.GetMovableLocations(chr);
                // �ړ��\�ꏊ��\��
                field.ClearTiles();
                field.ShowTile(moveList.Select(h => h.current).ToList(), COLOR_TILE_MOVE);
                continue;
            }
            else if (chr.IsPlayer() && chr.turnActable)
            {
                // �v���C���[�L�������N���b�N
                var moveResult = 0;
                yield return PTurnPlayerChrMove(chr as PlayerCharacter, x => moveResult = x);
                // �L�����Z��������߂�
                if (moveResult == 0) continue;

                // �S���s���I�����Ă���^�[���I��
                if (field.GetActableChara(true).Count == 0)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// �v���C���L�������N���b�N���čs��
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="callback">�߂�l���p</param>
    /// <returns></returns>
    private IEnumerator PTurnPlayerChrMove(PlayerCharacter pc, Action<int> callback)
    {
        var baseLoc = pc.GetLocation();
        var moveList = field.GetMovableLocations(pc);
        var moveListCur = moveList.Select(h => h.current).ToList();
        // �ړ��\�ꏊ��\��
        field.ClearTiles();
        field.ShowTile(moveListCur, COLOR_TILE_MOVE);

        // �ړ�����N���b�N
        yield return field.WaitInput();
        var moveCellChr = field.GetCellCharacter(field.InputLoc);

        // �ړ��s���N���b�N�����ꍇ�߂�
        if (moveCellChr != null && field.InputLoc != baseLoc ||
            !moveListCur.Any(m => m == field.InputLoc))
        {
            field.ClearTiles();
            callback?.Invoke(0);
            yield break;
        }
        var moveTargetHistory = moveList.Find(h => h.current == field.InputLoc);

        // �ړ�����
        yield return pc.Walk(moveTargetHistory);
        pc.SetLocation(field.InputLoc);

        // �ړ���R�}���h����
        var commandResult = 0;
        yield return PTurnCommand(pc, x => commandResult = x);
        if (commandResult == 0)
        {
            // �L�����Z��������baseLoc�ɖ߂�
            pc.PlayAnim(Constant.Direction.None);
            pc.SetLocation(baseLoc);
        }

        callback?.Invoke(commandResult);
    }

    /// <summary>
    /// �v���C���[�^�[���s���I������
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="callback">�߂�l���p�@0:�L�����Z���@1:�s���I��</param>
    /// <returns></returns>
    private IEnumerator PTurnCommand(PlayerCharacter pc, Action<int> callback)
    {
        var manager = ManagerSceneScript.GetInstance();
        var command = manager.commandUI;

        while (true)
        {
            // �R�}���h�\��
            yield return command.ShowCoroutine(pc);
            field.ClearTiles();
            if (command.Result == CommandUI.CommandResult.Cancel)
            {
                // �L�����Z��
                callback?.Invoke(0);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.Wait)
            {
                // �ҋ@
                pc.PlayAnim(Constant.Direction.None);
                pc.SetActable(false);
                callback?.Invoke(1);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.Escape)
            {
                //todo: �P��
                pc.PlayAnim(Constant.Direction.None);
                pc.SetActable(false);
                pc.param.HP -= 3;
                if (pc.param.HP < 0) pc.param.HP = 0;
                pc.UpdateHP();
                callback?.Invoke(1);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.ClassChange)
            {
                //todo: �N���X�`�F���W
            }
            else
            {
                // �s���A�C�e���I��
                var actSelectResult = 0;
                yield return PTurnActSelect(pc, x => actSelectResult = x);
                if (actSelectResult != 0)
                {
                    callback?.Invoke(actSelectResult);
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// �s�������j���[����I��
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator PTurnActSelect(PlayerCharacter pc, Action<int> callback)
    {
        var manager = ManagerSceneScript.GetInstance();
        var itemui = manager.itemListUI;
        var estUI = manager.battleEstimateUI;

        while (true)
        {
            // �s���A�C�e���I��UI
            yield return itemui.ShowCoroutine(pc);
            // �L�����Z��
            if (itemui.Result == ItemListUI.ItemResult.Cancel)
            {
                callback?.Invoke(0);
                yield break;
            }

            // �I�񂾑I����
            var selItem = itemui.Result_SelectData;
            var selAtkCells = field.GetRangeLocations(pc.GetLocation(), selItem.rangeMin, selItem.rangeMax);
            if (selItem.iType == GameDatabase.ItemType.Item ||
                selItem.iType == GameDatabase.ItemType.Rod)
                field.ShowTile(selAtkCells, COLOR_TILE_HEAL);
            else
                field.ShowTile(selAtkCells, COLOR_TILE_ATTACK);
            yield return field.WaitInput();
            field.ClearTiles();
            var selAtkChr = field.GetCellCharacter(field.InputLoc);
            // �U���ΏۈȊO��I�񂾂�L�����Z��
            if (selAtkChr == null) continue;

            if (selItem.iType == GameDatabase.ItemType.Item ||
                selItem.iType == GameDatabase.ItemType.Rod)
            {
                // �A�C�e���E��̓L���������ԂłȂ���΃L�����Z��
                if (!selAtkChr.IsPlayer()) continue;
                yield return PTurnHealCoroutine(pc, selAtkChr as PlayerCharacter, itemui.Result_SelectIndex);
                break;
            }
            else
            {
                // ����͓G�łȂ���΃L�����Z��
                if (selAtkChr.IsPlayer()) continue;

                // �I�񂾕���𑕔������ɂ���
                GameParameter.otherData.SetEquipIndex(pc.playerID, itemui.Result_SelectIndex);

                // �퓬���ʗ\���\��
                var battleParam = GameParameter.GetBattleParameter(pc, selAtkChr);
                yield return estUI.ShowCoroutine(battleParam, pc, selAtkChr as EnemyCharacter);
                // �L�����Z��
                if (estUI.Result == BattleEstimateUI.BattleSelectResult.Cancel) continue;

                yield return TurnBattleCoroutine(battleParam, pc, selAtkChr, null);
                break;
            }
        }

        // �s���I��
        callback?.Invoke(1);
    }

    /// <summary>
    /// �񕜃R���[�`��
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="target"></param>
    /// <param name="itemIndex">�g���A�C�e���̑܂̏ꏊ</param>
    /// <returns></returns>
    private IEnumerator PTurnHealCoroutine(PlayerCharacter pc, PlayerCharacter target, int itemIndex)
    {
        yield return null;
    }

    /// <summary>
    /// �퓬�R���[�`��
    /// </summary>
    /// <param name="param">�퓬�v�Z</param>
    /// <param name="atkChr"></param>
    /// <param name="defChr"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator TurnBattleCoroutine(GameParameter.BattleParameter param, CharacterBase atkChr, CharacterBase defChr, Action<int> callback)
    {
        var manager = ManagerSceneScript.GetInstance();

        yield return null;
        callback?.Invoke(1);
    }

    #endregion

    #region �G�^�[��

    /// <summary>
    /// �G�^�[���Ǘ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyTurn()
    {
        var manager = ManagerSceneScript.GetInstance();
        // �^�[���\��
        yield return manager.turnDisplay.DisplayTurnStart(false);
    }

    #endregion
}

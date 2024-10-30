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
        var command = manager.commandUI;
        var itemui = manager.itemListUI;

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
                var baseLoc = chr.GetLocation();
                // �v���C���[�L�������N���b�N
                var pc = chr as PlayerCharacter;
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
                    continue;
                }
                var moveTargetHistory = moveList.Find(h => h.current == field.InputLoc);

                // �ړ�����
                yield return pc.Walk(moveTargetHistory);
                pc.SetLocation(field.InputLoc);

                // �R�}���h�\��
                var commandCancel = 0; // 0:�s�������@1:�L�����Z��
                while (true)
                {
                    yield return command.ShowCoroutine(pc);
                    field.ClearTiles();
                    if (command.Result == CommandUI.CommandResult.Cancel)
                    {
                        // �L�����Z��
                        pc.PlayAnim(Constant.Direction.None);
                        pc.SetLocation(baseLoc);
                        commandCancel = 1;
                        break;
                    }
                    else if (command.Result == CommandUI.CommandResult.Wait)
                    {
                        // �ҋ@
                        pc.PlayAnim(Constant.Direction.None);
                        pc.SetActable(false);
                        break;
                    }
                    else if (command.Result == CommandUI.CommandResult.Escape)
                    {
                        // �P��
                    }
                    else if (command.Result == CommandUI.CommandResult.ClassChange)
                    {
                        // �N���X�`�F���W
                    }

                    // �s���A�C�e���I��UI
                    yield return itemui.ShowCoroutine(pc);
                    // �L�����Z��������R�}���h
                    if (itemui.Result == ItemListUI.ItemResult.Cancel) continue;

                    // �I�񂾑I����
                    var selItem = itemui.Result_SelectData;
                    if (selItem.iType == GameDatabase.ItemType.Item)
                    {
                        // �A�C�e��
                    }
                    else if (selItem.iType == GameDatabase.ItemType.Rod)
                    {
                        // ��
                    }
                    else
                    {
                        // ���͕���
                    }
                }
                // �L�����Z��������߂�
                if (commandCancel == 1) continue;


                // �S���s���I�����Ă���^�[���I��
                if (field.GetActableChara(true).Count == 0)
                {
                    break;
                }
            }
        }
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

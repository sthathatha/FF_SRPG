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

            yield return EnemyTurn();
        }
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

        // �^�[���\��
        yield return manager.turnDisplay.DisplayTurnStart(true);

        while (true)
        {
            // �N���b�N�҂�
            yield return field.WaitInput();

            var chr = field.GetCellCharacter(field.InputPos);
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
            else if (chr.IsPlayer())
            {
                // �v���C���[�L�������N���b�N
                var pc = chr as PlayerCharacter;
                var moveList = field.GetMovableLocations(pc);
                var moveListCur = moveList.Select(h => h.current).ToList();
                // �ړ��\�ꏊ��\��
                field.ClearTiles();
                field.ShowTile(moveListCur, COLOR_TILE_MOVE);

                // �ړ�����N���b�N
                yield return field.WaitInput();

                var moveCellChr = field.GetCellCharacter(field.InputPos);
                // �ړ��s���N���b�N�����ꍇ�߂�
                if (moveCellChr != null || !moveListCur.Any(m => m == field.InputPos))
                {
                    field.ClearTiles();
                    continue;
                }

                // �ړ�����
                //todo:
                pc.SetLocation(field.InputPos);
                // �R�}���h�\��
                yield return command.ShowCoroutine(pc);
                field.ClearTiles();
                if (command.Result == CommandUI.CommandResult.Cancel)
                {
                    // �L�����Z��
                    continue;
                }
                else if (command.Result == CommandUI.CommandResult.Wait)
                {
                    // �ҋ@
                    //todo:
                    continue;
                }
                else if (command.Result == CommandUI.CommandResult.Escape)
                {
                    // �P��
                    continue;
                }
                else if (command.Result == CommandUI.CommandResult.ClassChange)
                {
                    // �N���X�`�F���W
                    continue;
                }

                // �s��
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

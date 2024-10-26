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

        while (true)
        {
            yield return PlayerTurn();
        }
    }

    #endregion

    #region �v���C���[�^�[��

    /// <summary>
    /// �v���C���[�^�[������
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerTurn()
    {
        //
        yield return field.WaitInput();

        var chr = field.GetCellCharacter(field.InputPos);
        if (chr == null)
        {

        }
        else
        {
            if (field.InputLong)
            {

            }
            else
            {
                var moveList = field.GetMovableLocations(chr);
                //
                field.ShowTile(moveList.Select(h => h.current).ToList(), COLOR_TILE_MOVE);

                //
                yield return field.WaitInput();
                field.ClearTiles();
            }
        }
    }

    #endregion

    #region �G�^�[��
    #endregion
}

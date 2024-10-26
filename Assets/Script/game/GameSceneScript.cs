using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 本編
/// </summary>
public class GameSceneScript : MainScriptBase
{
    #region 定数

    private readonly Color COLOR_TILE_ATTACK = new Color(1f, 0.2f, 0.2f, 0.6f);
    private readonly Color COLOR_TILE_MOVE = new Color(0.2f, 0.5f, 1, 0.6f);

    #endregion

    #region メンバー

    public FieldSystem field;

    #endregion

    #region 既定

    /// <summary>
    /// フェードイン前の処理
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BeforeInitFadeIn()
    {
        yield return base.BeforeInitFadeIn();

        field.InitField();
    }

    /// <summary>
    /// フェードイン終了後
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

    #region プレイヤーターン

    /// <summary>
    /// プレイヤーターン処理
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

    #region 敵ターン
    #endregion
}

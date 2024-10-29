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

        StartCoroutine(GameCoroutine());
    }

    #endregion

    /// <summary>
    /// ゲーム流れ管理
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

    #region プレイヤーターン

    /// <summary>
    /// プレイヤーターン処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerTurn()
    {
        var manager = ManagerSceneScript.GetInstance();
        var command = manager.commandUI;

        // ターン表示
        yield return manager.turnDisplay.DisplayTurnStart(true);

        while (true)
        {
            // クリック待ち
            yield return field.WaitInput();

            var chr = field.GetCellCharacter(field.InputPos);
            if (chr == null)
            {
                // キャラの居ない場所をクリックの場合
                field.ClearTiles();
                continue;
            }
            else if (!chr.IsPlayer())
            {
                // 敵をクリックした場合
                var moveList = field.GetMovableLocations(chr);
                // 移動可能場所を表示
                field.ClearTiles();
                field.ShowTile(moveList.Select(h => h.current).ToList(), COLOR_TILE_MOVE);
                continue;
            }
            else if (chr.IsPlayer())
            {
                // プレイヤーキャラをクリック
                var pc = chr as PlayerCharacter;
                var moveList = field.GetMovableLocations(pc);
                var moveListCur = moveList.Select(h => h.current).ToList();
                // 移動可能場所を表示
                field.ClearTiles();
                field.ShowTile(moveListCur, COLOR_TILE_MOVE);

                // 移動先をクリック
                yield return field.WaitInput();

                var moveCellChr = field.GetCellCharacter(field.InputPos);
                // 移動不可をクリックした場合戻る
                if (moveCellChr != null || !moveListCur.Any(m => m == field.InputPos))
                {
                    field.ClearTiles();
                    continue;
                }

                // 移動する
                //todo:
                pc.SetLocation(field.InputPos);
                // コマンド表示
                yield return command.ShowCoroutine(pc);
                field.ClearTiles();
                if (command.Result == CommandUI.CommandResult.Cancel)
                {
                    // キャンセル
                    continue;
                }
                else if (command.Result == CommandUI.CommandResult.Wait)
                {
                    // 待機
                    //todo:
                    continue;
                }
                else if (command.Result == CommandUI.CommandResult.Escape)
                {
                    // 撤退
                    continue;
                }
                else if (command.Result == CommandUI.CommandResult.ClassChange)
                {
                    // クラスチェンジ
                    continue;
                }

                // 行動
            }
        }
    }

    #endregion

    #region 敵ターン

    /// <summary>
    /// 敵ターン管理
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyTurn()
    {
        var manager = ManagerSceneScript.GetInstance();
        // ターン表示
        yield return manager.turnDisplay.DisplayTurnStart(false);
    }

    #endregion
}

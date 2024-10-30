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
            field.ResetAllActable();

            yield return EnemyTurn();
            field.ResetAllActable();
        }
    }

    /// <summary>
    /// オプションボタンクリック
    /// </summary>
    public void OptionClick()
    {

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
        var itemui = manager.itemListUI;

        // ターン表示
        yield return manager.turnDisplay.DisplayTurnStart(true);

        while (true)
        {
            // クリック待ち
            yield return field.WaitInput(true);

            if (field.InputResult == FieldSystem.InputResultEnum.TurnEnd)
            {
                break;
            }

            var chr = field.GetCellCharacter(field.InputLoc);
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
            else if (chr.IsPlayer() && chr.turnActable)
            {
                var baseLoc = chr.GetLocation();
                // プレイヤーキャラをクリック
                var pc = chr as PlayerCharacter;
                var moveList = field.GetMovableLocations(pc);
                var moveListCur = moveList.Select(h => h.current).ToList();
                // 移動可能場所を表示
                field.ClearTiles();
                field.ShowTile(moveListCur, COLOR_TILE_MOVE);

                // 移動先をクリック
                yield return field.WaitInput();
                var moveCellChr = field.GetCellCharacter(field.InputLoc);
                // 移動不可をクリックした場合戻る
                if (moveCellChr != null && field.InputLoc != baseLoc ||
                    !moveListCur.Any(m => m == field.InputLoc))
                {
                    field.ClearTiles();
                    continue;
                }
                var moveTargetHistory = moveList.Find(h => h.current == field.InputLoc);

                // 移動する
                yield return pc.Walk(moveTargetHistory);
                pc.SetLocation(field.InputLoc);

                // コマンド表示
                var commandCancel = 0; // 0:行動完了　1:キャンセル
                while (true)
                {
                    yield return command.ShowCoroutine(pc);
                    field.ClearTiles();
                    if (command.Result == CommandUI.CommandResult.Cancel)
                    {
                        // キャンセル
                        pc.PlayAnim(Constant.Direction.None);
                        pc.SetLocation(baseLoc);
                        commandCancel = 1;
                        break;
                    }
                    else if (command.Result == CommandUI.CommandResult.Wait)
                    {
                        // 待機
                        pc.PlayAnim(Constant.Direction.None);
                        pc.SetActable(false);
                        break;
                    }
                    else if (command.Result == CommandUI.CommandResult.Escape)
                    {
                        // 撤退
                    }
                    else if (command.Result == CommandUI.CommandResult.ClassChange)
                    {
                        // クラスチェンジ
                    }

                    // 行動アイテム選択UI
                    yield return itemui.ShowCoroutine(pc);
                    // キャンセルしたらコマンド
                    if (itemui.Result == ItemListUI.ItemResult.Cancel) continue;

                    // 選んだ選択肢
                    var selItem = itemui.Result_SelectData;
                    if (selItem.iType == GameDatabase.ItemType.Item)
                    {
                        // アイテム
                    }
                    else if (selItem.iType == GameDatabase.ItemType.Rod)
                    {
                        // 杖
                    }
                    else
                    {
                        // 他は武器
                    }
                }
                // キャンセルしたら戻る
                if (commandCancel == 1) continue;


                // 全員行動終了してたらターン終了
                if (field.GetActableChara(true).Count == 0)
                {
                    break;
                }
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

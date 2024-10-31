using System;
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
    private readonly Color COLOR_TILE_HEAL = new Color(0.2f, 1f, 0.2f, 0.6f);

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
                // プレイヤーキャラをクリック
                var moveResult = 0;
                yield return PTurnPlayerChrMove(chr as PlayerCharacter, x => moveResult = x);
                // キャンセルしたら戻る
                if (moveResult == 0) continue;

                // 全員行動終了してたらターン終了
                if (field.GetActableChara(true).Count == 0)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// プレイヤキャラをクリックして行動
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="callback">戻り値受取用</param>
    /// <returns></returns>
    private IEnumerator PTurnPlayerChrMove(PlayerCharacter pc, Action<int> callback)
    {
        var baseLoc = pc.GetLocation();
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
            callback?.Invoke(0);
            yield break;
        }
        var moveTargetHistory = moveList.Find(h => h.current == field.InputLoc);

        // 移動する
        yield return pc.Walk(moveTargetHistory);
        pc.SetLocation(field.InputLoc);

        // 移動後コマンド処理
        var commandResult = 0;
        yield return PTurnCommand(pc, x => commandResult = x);
        if (commandResult == 0)
        {
            // キャンセルしたらbaseLocに戻る
            pc.PlayAnim(Constant.Direction.None);
            pc.SetLocation(baseLoc);
        }

        callback?.Invoke(commandResult);
    }

    /// <summary>
    /// プレイヤーターン行動選択処理
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="callback">戻り値受取用　0:キャンセル　1:行動終了</param>
    /// <returns></returns>
    private IEnumerator PTurnCommand(PlayerCharacter pc, Action<int> callback)
    {
        var manager = ManagerSceneScript.GetInstance();
        var command = manager.commandUI;

        while (true)
        {
            // コマンド表示
            yield return command.ShowCoroutine(pc);
            field.ClearTiles();
            if (command.Result == CommandUI.CommandResult.Cancel)
            {
                // キャンセル
                callback?.Invoke(0);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.Wait)
            {
                // 待機
                pc.PlayAnim(Constant.Direction.None);
                pc.SetActable(false);
                callback?.Invoke(1);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.Escape)
            {
                //todo: 撤退
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
                //todo: クラスチェンジ
            }
            else
            {
                // 行動アイテム選択
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
    /// 行動をメニューから選択
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
            // 行動アイテム選択UI
            yield return itemui.ShowCoroutine(pc);
            // キャンセル
            if (itemui.Result == ItemListUI.ItemResult.Cancel)
            {
                callback?.Invoke(0);
                yield break;
            }

            // 選んだ選択肢
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
            // 攻撃対象以外を選んだらキャンセル
            if (selAtkChr == null) continue;

            if (selItem.iType == GameDatabase.ItemType.Item ||
                selItem.iType == GameDatabase.ItemType.Rod)
            {
                // アイテム・杖はキャラが仲間でなければキャンセル
                if (!selAtkChr.IsPlayer()) continue;
                yield return PTurnHealCoroutine(pc, selAtkChr as PlayerCharacter, itemui.Result_SelectIndex);
                break;
            }
            else
            {
                // 武器は敵でなければキャンセル
                if (selAtkChr.IsPlayer()) continue;

                // 選んだ武器を装備扱いにする
                GameParameter.otherData.SetEquipIndex(pc.playerID, itemui.Result_SelectIndex);

                // 戦闘結果予測表示
                var battleParam = GameParameter.GetBattleParameter(pc, selAtkChr);
                yield return estUI.ShowCoroutine(battleParam, pc, selAtkChr as EnemyCharacter);
                // キャンセル
                if (estUI.Result == BattleEstimateUI.BattleSelectResult.Cancel) continue;

                yield return TurnBattleCoroutine(battleParam, pc, selAtkChr, null);
                break;
            }
        }

        // 行動終了
        callback?.Invoke(1);
    }

    /// <summary>
    /// 回復コルーチン
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="target"></param>
    /// <param name="itemIndex">使うアイテムの袋の場所</param>
    /// <returns></returns>
    private IEnumerator PTurnHealCoroutine(PlayerCharacter pc, PlayerCharacter target, int itemIndex)
    {
        yield return null;
    }

    /// <summary>
    /// 戦闘コルーチン
    /// </summary>
    /// <param name="param">戦闘計算</param>
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

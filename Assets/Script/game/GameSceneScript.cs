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

    public CellUI cellUI;

    public AudioClip se_attack_normal;
    public AudioClip se_attack_dead;
    public AudioClip se_attack_critical;
    public AudioClip se_attack_miss;
    public AudioClip se_attack_zero;
    public AudioClip se_heal;
    public AudioClip se_death;
    public AudioClip se_levelup;
    public AudioClip se_class_change;

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
                pc.UpdateHP(true);
                callback?.Invoke(1);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.ClassChange)
            {
                // クラスチェンジ
                var ccResult = false;
                yield return ClassChangeCoroutine(pc, (r) => ccResult = r);
                if (ccResult)
                {
                    pc.PlayAnim(Constant.Direction.None);
                    pc.SetActable(false);
                    pc.UpdateHP(true);
                    callback?.Invoke(1);
                    yield break;
                }
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
            if (!selAtkCells.Contains(field.InputLoc)) continue;
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

                field.ShowTile(new List<Vector2Int> { field.InputLoc }, COLOR_TILE_ATTACK);
                yield return estUI.ShowCoroutine(battleParam, pc, selAtkChr as EnemyCharacter);
                field.ClearTiles();

                // キャンセル
                if (estUI.Result == BattleEstimateUI.BattleSelectResult.Cancel) continue;

                yield return TurnBattleCoroutine(battleParam, pc, selAtkChr, null);
                break;
            }
        }

        // 行動終了
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

    #region 戦闘・回復アニメーション

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
        var sound = manager.soundMan;
        var aLoc = field.GetCellPosition(atkChr.GetLocation());
        var dLoc = field.GetCellPosition(defChr.GetLocation());
        var aDist = dLoc - aLoc;
        var dDist = aLoc - dLoc;

        // 経験値計算用変数
        PlayerCharacter expTmp_chr = (atkChr.IsPlayer() ? atkChr : defChr) as PlayerCharacter;
        var expTmp_defeat = false;
        var expTmp_dmg = 0;
        var expTmp_enemyLv = (atkChr.IsPlayer() ? defChr : atkChr).param.Lv;

        // アニメーション設定
        atkChr.PlayAnim(Util.GetDirectionFromVec(aDist));
        defChr.PlayAnim(Util.GetDirectionFromVec(dDist));

        var weaponBreak = -1;
        var weaponDrop = GameDatabase.ItemID.FreeHand;
        var phase = 0;
        while (param.a_atkCount > 0 || param.d_atkCount > 0)
        {
            // どっちが行動するか
            var atkTurn = true;
            // どっちかが0の場合はもう一方確定
            if (param.a_atkCount == 0) atkTurn = false;
            else if (param.d_atkCount == 0) atkTurn = true;
            else
            {
                // 最初は攻撃側
                if (phase == 0)
                {
                    atkTurn = true;
                }
                else
                {
                    // 2回目は防御側、3回目以降は普通どっちかが0になっているはず
                    atkTurn = false;
                }
            }
            var chrA = atkTurn ? atkChr : defChr;
            var chrD = atkTurn ? defChr : atkChr;

            var isHit = false;
            var isCrt = false;
            var dmg = 0;
            #region ダメージ判定してSE再生アクション
            var CalcAtk = new Action<int, int, int, int>((hit, crt, atk, hp) =>
            {
                isHit = Util.RandomCheck(hit);
                if (!isHit)
                {
                    sound.PlaySE(se_attack_miss);
                    return;
                }
                if (atk == 0)
                {
                    sound.PlaySE(se_attack_zero);
                    return;
                }
                isCrt = Util.RandomCheck(crt);
                dmg = isCrt ? atk * 3 : atk;
                if (isCrt)
                {
                    sound.PlaySE(se_attack_critical);
                }
                else if (dmg >= hp)
                {
                    sound.PlaySE(se_attack_dead);
                }
                else
                {
                    sound.PlaySE(se_attack_normal);
                }
            }
            );
            #endregion

            // 武器消耗
            if (chrA.IsPlayer())
            {
                var breakIdx = BattleWeaponDecrease(chrA as PlayerCharacter, true);
                if (breakIdx >= 0)
                {
                    weaponBreak = breakIdx;
                    // 壊れたら最後の攻撃にする
                    if (atkTurn) param.a_atkCount = 1;
                    else param.d_atkCount = 1;
                }
            }

            // 攻撃回数を１ずつ減らす
            if (atkTurn) param.a_atkCount--;
            else param.d_atkCount--;

            // 攻撃側
            StartCoroutine(chrA.AttackAnim(atkTurn ? aDist : dDist));
            if (atkTurn) CalcAtk(param.a_hit, param.a_critical, param.a_dmg, chrD.param.HP);
            else CalcAtk(param.d_hit, param.d_critical, param.d_dmg, chrA.param.HP);
            yield return new WaitForSeconds(0.15f);
            // HP減らしてゲージ更新
            if (isHit)
            {
                if (chrA.IsPlayer()) expTmp_dmg += dmg;

                chrD.param.HP -= dmg;
                if (chrD.param.HP < 0) chrD.param.HP = 0;
                chrD.UpdateHP();

                if (chrD.param.HP <= 0)
                {
                    if (chrD.IsPlayer())
                    {
                        // プレイヤーが死んだら経験値計算無し
                        expTmp_chr = null;
                        expTmp_dmg = 0;
                    }
                    else
                    {
                        expTmp_defeat = true;
                    }
                    if (atkTurn) defChr = null;
                    else atkChr = null;

                    // ドロップ判定
                    if (!chrD.IsPlayer()) weaponDrop = (chrD as EnemyCharacter).dropID;

                    // 死んだら消して戦闘終了
                    yield return new WaitForSeconds(0.2f);
                    sound.PlaySE(se_death);
                    yield return chrD.DeathAnim();
                    field.DeleteCharacter(chrD);
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
            }
            yield return new WaitForSeconds(0.6f);

            phase++;
        }

        atkChr?.PlayAnim(Constant.Direction.None);
        atkChr?.SetActable(false);
        defChr?.PlayAnim(Constant.Direction.None);

        // 経験値取得・レベルアップ処理
        if (expTmp_chr != null)
        {
            var expGet = ExpCalcBattle(expTmp_chr, expTmp_enemyLv, expTmp_dmg, expTmp_defeat);
            if (expGet > 0)
            {
                yield return ExpGetCoroutine(expTmp_chr, expGet);
            }
        }

        // 武器破壊処理
        if (weaponBreak >= 0)
        {
            yield return BattleWeaponBreak(weaponBreak);
        }
        // アイテムドロップ処理
        if (weaponDrop != GameDatabase.ItemID.FreeHand)
        {
            yield return BattleWeaponDrop(weaponDrop);
        }

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

        // 経験値取得・レベルアップ
    }

    /// <summary>
    /// 武器消耗
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="atk">true:攻撃時　false:反撃時</param>
    /// <returns>壊れた場合アイテム番号</returns>
    private int BattleWeaponDecrease(PlayerCharacter pc, bool atk)
    {
        var decCount = 1;
        if (atk)
        {
            //todo:スキルにより武器消耗回数変更
        }
        if (decCount == 0) return -1; //消耗しない

        var equipIndex = GameParameter.otherData.GetEquipIndex(pc.playerID);
        if (equipIndex < 0) return -1; //素手は無視

        if (GameParameter.otherData.haveItemList[equipIndex].useCount <= decCount)
        {
            GameParameter.otherData.haveItemList[equipIndex].useCount = 0;
            return equipIndex;
        }
        else
        {
            GameParameter.otherData.haveItemList[equipIndex].useCount -= decCount;
            return -1;
        }
    }

    /// <summary>
    /// 壊れた武器を削除する演出
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    private IEnumerator BattleWeaponBreak(int idx)
    {
        var msg = ManagerSceneScript.GetInstance().lineMessageUI;
        var itm = GameParameter.otherData.haveItemList[idx];
        yield return msg.ShowCoroutine($"{itm.ItemData.name} が壊れた");

        GameParameter.otherData.DeleteItem(idx);
    }

    /// <summary>
    /// アイテム取得演出
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private IEnumerator BattleWeaponDrop(GameDatabase.ItemID id)
    {
        var msg = ManagerSceneScript.GetInstance().lineMessageUI;
        var itm = GameDatabase.ItemDataList[(int)id];
        yield return msg.ShowCoroutine($"{itm.name} を手に入れた");

        GameParameter.otherData.AddItem(id);
    }

    #endregion

    #region 経験値取得・レベルアップ処理

    /// <summary>
    /// 戦闘の取得経験値計算
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="enemyLv"></param>
    /// <param name="atkDmg"></param>
    /// <param name="isDefeat"></param>
    /// <returns></returns>
    private int ExpCalcBattle(PlayerCharacter pc, int enemyLv, int atkDmg, bool isDefeat)
    {
        return 100;
    }

    /// <summary>
    /// 回復の取得経験値計算
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    private int ExpCalcHeal(PlayerCharacter pc)
    {
        return 19;
    }

    /// <summary>
    /// 経験値取得処理
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="exp"></param>
    /// <returns></returns>
    private IEnumerator ExpGetCoroutine(PlayerCharacter pc, int exp)
    {
        var manager = ManagerSceneScript.GetInstance();

        if (exp == 0 || pc.param.Lv >= 20) yield break;

        // UI表示
        yield return cellUI.ShowExpCoroutine(pc.GetLocation(), exp);

        // 加算
        pc.param.Exp += exp;
        if (pc.param.Exp >= 100)
        {
            manager.soundMan.PlaySE(se_levelup);
            yield return cellUI.ShowLevelUpCoroutine(pc.GetLocation());
            // レベルアップ
            pc.param.Lv++;
            pc.param.Exp = pc.param.Lv >= 20 ? 0 : pc.param.Exp - 100;

            // 上昇値判定
            var upParam = GameDatabase.Prm_PlayerGrow_GetCalced(pc.playerID);

            // 表示
            yield return manager.statusUpUI.ShowLvup(pc, upParam);

            // パラメータ更新
            pc.param.MaxHP += upParam.maxHp;
            pc.param.Atk += upParam.atk;
            pc.param.Mag += upParam.mag;
            pc.param.Tec += upParam.tec;
            pc.param.Spd += upParam.spd;
            pc.param.Luk += upParam.luk;
            pc.param.Def += upParam.def;
            pc.param.Mdf += upParam.mdf;
            pc.UpdateHP(true);
        }

        // セーブデータにも反映
        GameParameter.Prm_SetFieldParam(pc.playerID, pc.param);
    }

    #endregion

    #region クラスチェンジ処理

    /// <summary>
    /// クラスチェンジ処理
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="callback">true:クラスチェンジ実行</param>
    /// <returns></returns>
    private IEnumerator ClassChangeCoroutine(PlayerCharacter pc, Action<bool> callback)
    {
        var manager = ManagerSceneScript.GetInstance();
        var ccui = manager.classChangeUI;
        var growui = manager.statusUpUI;
        yield return ccui.ShowCCCoroutine(pc);
        if (ccui.Result == ClassChangeUI.CCSelect.Cancel)
        {
            callback?.Invoke(false);
            yield break;
        }

        callback?.Invoke(true);
        manager.soundMan.PlaySE(se_class_change);
        yield return manager.FadeOut(1f, Color.white);
        yield return manager.FadeIn(1f);
        yield return new WaitForSeconds(0.5f);

        // クラスチェンジ実行
        var saveParam = pc.GetSaveParameter();
        if (ccui.Result == ClassChangeUI.CCSelect.Rebirth)
        {
            // 転生
            var downParam = ClassChangeRebirthCalc(pc);

            yield return growui.ShowRebirth(pc, downParam);
            pc.param.Lv = 1;
            pc.param.MaxHP += downParam.maxHp;
            pc.param.Atk += downParam.atk;
            pc.param.Mag += downParam.mag;
            pc.param.Tec += downParam.tec;
            pc.param.Spd += downParam.spd;
            pc.param.Luk += downParam.luk;
            pc.param.Def += downParam.def;
            pc.param.Mdf += downParam.mdf;
            pc.param.Move += downParam.move;
            saveParam.ClassID = Constant.ClassID.Base;
        }
        else
        {
            // クラスチェンジ
            yield return growui.ShowClassChange(pc, ccui.SelectClass);
            var upParam = GameDatabase.Prm_ClassChangeGrow_Get(pc.playerID, ccui.SelectClass);
            pc.param.Lv = 1;
            pc.param.MaxHP += upParam.maxHp;
            pc.param.Atk += upParam.atk;
            pc.param.Mag += upParam.mag;
            pc.param.Tec += upParam.tec;
            pc.param.Spd += upParam.spd;
            pc.param.Luk += upParam.luk;
            pc.param.Def += upParam.def;
            pc.param.Mdf += upParam.mdf;
            pc.param.Move += upParam.move;
            saveParam.ClassID = ccui.SelectClass;
        }
        GameParameter.Prm_SetFieldParam(pc.playerID, pc.param);

        pc.UpdateClassIcon();
    }

    /// <summary>
    /// 転生時のダウンパラメータを計算
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    private GameDatabase.ParameterData ClassChangeRebirthCalc(PlayerCharacter pc)
    {
        var downParam = new GameDatabase.ParameterData();
        var nowParam = pc.GetSaveParameter();
        var baseParam = GameDatabase.Prm_PlayerInit[(int)pc.playerID];

        // 残る割合
        var savePow = Mathf.Pow(nowParam.ReviveCount, 1.2f);
        var saveRate = (savePow + 2f) / (savePow + 3f);
        var calcAct = new Func<int, int, int>((nowNum, baseNum) =>
        {
            var diff = nowNum - baseNum;
            var save = Mathf.FloorToInt(diff * saveRate);
            return save - diff;
        });
        downParam.maxHp = calcAct(nowParam.MaxHP, baseParam.maxHp);
        downParam.atk = calcAct(nowParam.Atk, baseParam.atk);
        downParam.mag = calcAct(nowParam.Mag, baseParam.mag);
        downParam.tec = calcAct(nowParam.Tec, baseParam.tec);
        downParam.spd = calcAct(nowParam.Spd, baseParam.spd);
        downParam.luk = calcAct(nowParam.Luk, baseParam.luk);
        downParam.def = calcAct(nowParam.Def, baseParam.def);
        downParam.mdf = calcAct(nowParam.Mdf, baseParam.mdf);
        downParam.move = baseParam.move - nowParam.Move;

        return downParam;
    }

    #endregion
}

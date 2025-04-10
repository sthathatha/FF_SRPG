using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameDatabase;

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
    public CanvasGroup gameoverScreen;

    public AudioClip se_attack_normal;
    public AudioClip se_attack_dead;
    public AudioClip se_attack_critical;
    public AudioClip se_attack_miss;
    public AudioClip se_attack_zero;
    public AudioClip se_heal;
    public AudioClip se_death;
    public AudioClip se_levelup;
    public AudioClip se_class_change;
    public AudioClip se_escape;
    public AudioClip se_equip;

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
        bool startGame = true;
        while (true)
        {
            if (startGame)
            {
                //最初のみ行うこと
            }
            else
            {
            }

            // 行動のたびにセーブするようにする、ロード直後はターン加算しないようにする、行動決定操作時にランキング不可フラグ操作
            // ターン加算
            if (!startGame || !Global.GetTemporaryData().isLoadGame)
                field.Prm_BattleTurn++;

            startGame = false;

            yield return PlayerTurn();
            field.ResetAllActable();
            field.ClearTiles();

            if (Gameover_Check()) break;
            if (NextFloor_Check())
            {
                yield return NextFloorCoroutine();
                continue;
            }

            yield return EnemyTurn();
            field.ResetAllActable();
            field.ClearTiles();

            if (Gameover_Check()) break;
            if (NextFloor_Check())
            {
                yield return NextFloorCoroutine();
            }
        }

        yield return GameoverCoroutine();
        // タイトル画面に戻る
        ManagerSceneScript.GetInstance().LoadMainScene("TitleScene", 0);
    }

    /// <summary>
    /// オプションボタンクリック
    /// </summary>
    public void OptionClick()
    {
        // いつでも
        ManagerSceneScript.GetInstance().optionUI.Open();
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
            // バーサーカーキャラが居る場合自動行動
            var berserkChrs = field.GetPlayers().Where(pc => pc.HasSkill(SkillID.Drows_Berserk)).ToList();
            var berserkAct = false;
            foreach (var ber in berserkChrs)
            {
                // 行動終了している
                if (!ber.turnActable) continue;

                // 移動可能リスト
                var bMoves = field.GetMovableLocations(ber);

                // 射程1に攻撃できないのを持っていたら検索して剣か素手にする
                var bNowEq = GameDatabase.ItemDataList[(int)ber.GetEquipWeapon()];
                if (bNowEq.rangeMin != 1)
                {
                    GameParameter.otherData.SetEquipIndex(ber.playerID,
                        GameParameter.otherData.GetUsableEquip(ber.playerID));
                }

                // 攻撃可能リスト
                var bAtks = field.GetAttackableCharacters(ber, bMoves, 1, 1);
                if (bAtks.Count == 0) continue;
                berserkAct = true;

                // 攻撃対象選択
                var bAtkSel = field.SelectAIAttack(ber, bAtks);

                // 攻撃
                yield return AIAttackCoroutine(ber, bAtkSel);
            }
            if (berserkAct) continue;

            // 
            field.SaveField();

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
                var moveList = field.GetMovableLocations(chr).Select(h => h.current).ToList();
                // 移動可能場所を表示
                field.ClearTiles();
                field.ShowTile(moveList, COLOR_TILE_MOVE);
                // 攻撃可能場所
                var atkList = field.GetAttackableLocations(chr, moveList).Where(l => !moveList.Contains(l)).ToList();
                field.ShowTile(atkList, COLOR_TILE_ATTACK);

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
                if (field.GetActableChara(true).Count == 0) break;

                // 次のフロアに行く状態になっても終了
                if (NextFloor_Check()) break;
            }
        }

        // 敵ターンはロード不可
        field.LoadDisableSet();
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
                PTurnSetActEnd(pc);
                callback?.Invoke(1);
                yield break;
            }
            //else if (command.Result == CommandUI.CommandResult.Escape)
            //{
            //    // 撤退
            //    var se = manager.soundMan.PlaySELoop(se_escape);
            //    manager.soundMan.StopLoopSE(se, 1f);
            //    field.DeleteCharacter(pc, false);
            //    callback?.Invoke(1);
            //    yield break;
            //}
            else if (command.Result == CommandUI.CommandResult.Equip)
            {
                // 装備変更
                var itemui = manager.itemListUI;
                yield return itemui.ShowCoroutine(pc, true);
                // キャンセル
                if (itemui.Result == ItemListUI.ItemResult.Cancel) continue;

                // 選んだ選択肢
                var selItem = itemui.Result_SelectData;
                // 薬と杖はキャンセル扱い
                if (selItem.iType == ItemType.Item || selItem.iType == ItemType.Rod) continue;
                // 選んだアイテムを装備してコマンド選択に戻る
                GameParameter.otherData.SetEquipIndex(pc.playerID, itemui.Result_SelectIndex);
                manager.soundMan.PlaySE(se_equip);
            }
            else if (command.Result == CommandUI.CommandResult.ClassChange)
            {
                // クラスチェンジ
                var ccResult = false;
                yield return ClassChangeCoroutine(pc, (r) => ccResult = r);
                if (ccResult)
                {
                    PTurnSetActEnd(pc);
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
        var initItemFilter = true;

        while (true)
        {
            // 行動アイテム選択UI
            yield return itemui.ShowCoroutine(pc, initItemFilter);
            initItemFilter = false;
            // キャンセル
            if (itemui.Result == ItemListUI.ItemResult.Cancel)
            {
                callback?.Invoke(0);
                yield break;
            }

            // 選んだ選択肢
            var selItem = itemui.Result_SelectData;
            var rangePlus = 0;
            if (pc.HasSkill(SkillID.Worra_LongShot)) rangePlus += 1;
            var selAtkCells = field.GetRangeLocations(pc.GetLocation(), selItem.rangeMin, selItem.rangeMax + rangePlus);
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
                // すでにHP最大ならキャンセル
                if (selAtkChr.param.HP == selAtkChr.param.MaxHP) continue;

                // 行動中ロード不可
                field.LoadDisableSet();

                yield return PTurnHealCoroutine(pc, selAtkChr as PlayerCharacter, itemui.Result_SelectIndex);
                PTurnSetActEnd(pc);
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

                // 戦闘開始からロード不可
                field.LoadDisableSet();

                yield return TurnBattleCoroutine(battleParam, pc, selAtkChr, null);
                break;
            }
        }

        // 行動終了
        callback?.Invoke(1);
    }

    /// <summary>
    /// 行動終了セット
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="skillContinue">スキルによって行動終了しない</param>
    private void PTurnSetActEnd(PlayerCharacter pc, bool skillContinue = false)
    {
        pc.PlayAnim(Constant.Direction.None);

        // 敵がもう居なかったら終了しない
        if (field.GetEnemies().Count == 0)
        {
            field.ResetAllActable();
            return;
        }
        else if (skillContinue)
        {
            // スキルにより自分は終了しない
            return;
        }

        pc.SetActable(false);
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

        // 敵が居なかったらターンなし
        if (field.GetActableChara(false).Count == 0) yield break;

        // ターン表示
        yield return manager.turnDisplay.DisplayTurnStart(false);

        // 動けるのが居なくなるまたはプレイヤーが居なくなるまで
        while (field.GetActableChara(false).Count > 0 &&
            field.GetActableChara(true).Count > 0)
        {
            var enm = field.GetActableChara(false)[0] as EnemyCharacter;
            var moveList = field.GetMovableLocations(enm)
                .Where(l => l.current == enm.GetLocation() || field.GetCellCharacter(l.current) == null)
                .ToList();
            var attackableList = field.GetAttackableCharacters(enm, moveList);
            var atkAI = field.SelectAIAttack(enm, attackableList);
            if (atkAI == null)
            {
                // 攻撃できる相手が居ない
                if (enm.isBoss)
                {
                    // ボスは動かない
                    enm.SetActable(false);
                    continue;
                }

                // 近づくプレイヤーを選択
                var player = field.GetPlayers().OrderBy(p =>
                {
                    var dist = p.GetLocation() - enm.GetLocation();
                    return Math.Abs(dist.x) + Math.Abs(dist.y);
                }).First(); // 一番近いキャラ
                // 一番近い移動先を選択
                var moveSel = moveList.OrderBy(m =>
                {
                    var dist = player.GetLocation() - m.current;
                    return Math.Abs(dist.x) + Math.Abs(dist.y);
                }).First();
                // 移動
                yield return enm.Walk(moveSel);
                enm.SetLocation(moveSel.current);
                enm.SetActable(false);
            }
            else
            {
                yield return AIAttackCoroutine(enm, atkAI);
            }
        }
    }

    /// <summary>
    /// AI攻撃演出コルーチン
    /// </summary>
    /// <param name="atkChr">攻撃するキャラ</param>
    /// <param name="ai">攻撃情報</param>
    /// <returns></returns>
    private IEnumerator AIAttackCoroutine(CharacterBase atkChr, FieldSystem.AITargetResult ai)
    {
        yield return atkChr.Walk(ai.tgt.moveHist);
        atkChr.SetLocation(ai.tgt.moveHist.current);

        // 攻撃表示
        field.ShowTile(new List<Vector2Int>() { ai.tgt.target.GetLocation() }, COLOR_TILE_ATTACK);
        yield return new WaitForSeconds(0.5f);
        field.ClearTiles();

        // 攻撃する
        yield return TurnBattleCoroutine(ai.btl, atkChr, ai.tgt.target, null);

        yield return new WaitForSeconds(0.3f);
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
        var expEnemy = (atkChr.IsPlayer() ? defChr : atkChr) as EnemyCharacter;
        var expTmp_enemyLv = expEnemy.param.Lv;
        // 天使系・メタルスライムは経験値増加
        if (Prm_EnemyOther[(int)expEnemy.enemyID].rarelity > 40)
        {
            expTmp_enemyLv += Prm_EnemyOther[(int)expEnemy.enemyID].rarelity / 20;
        }

        // アニメーション設定
        atkChr.PlayAnim(Util.GetDirectionFromVec(aDist));
        defChr.PlayAnim(Util.GetDirectionFromVec(dDist));

        var breakChr = (PlayerCharacter)null;
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

            // 攻撃回数を１ずつ減らす
            if (atkTurn) param.a_atkCount--;
            else param.d_atkCount--;

            // 攻撃側アニメーション
            StartCoroutine(chrA.AttackAnim(atkTurn ? aDist : dDist));
            if (atkTurn) CalcAtk(param.a_hit, param.a_critical, param.a_dmg, chrD.param.HP);
            else CalcAtk(param.d_hit, param.d_critical, param.d_dmg, chrD.param.HP);
            yield return new WaitForSeconds(0.15f);
            // HP減らしてゲージ更新
            if (isHit)
            {
                if (chrA.IsPlayer())
                {
                    expTmp_dmg += dmg;

                    // 武器消耗
                    var breakIdx = BattleWeaponDecrease(chrA as PlayerCharacter, atkTurn);
                    if (breakIdx >= 0)
                    {
                        breakChr = chrA as PlayerCharacter;
                        weaponBreak = breakIdx;
                        // 壊れたら最後の攻撃にする
                        if (atkTurn) param.a_atkCount = 0;
                        else param.d_atkCount = 0;
                    }
                }

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

        if (atkChr != null)
        {
            if (atkChr.IsPlayer())
            {
                if (atkChr.HasSkill(SkillID.Drows_Tornaid) && phase == 0 && expTmp_defeat)
                    PTurnSetActEnd(atkChr as PlayerCharacter, true);
                else
                    PTurnSetActEnd(atkChr as PlayerCharacter);
            }
            else atkChr.SetActable(false);
        }
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
            yield return BattleWeaponBreak(breakChr, weaponBreak);
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
        var manager = ManagerSceneScript.GetInstance();
        pc.PlayAnim(Constant.Direction.None);

        // 回復量決定
        var healNum = 0;
        var itm = GameParameter.otherData.haveItemList[itemIndex];
        if (itm.ItemData.iType == GameDatabase.ItemType.Item)
        {
            // 薬は自分の最大HP割合
            healNum = target.param.MaxHP * itm.ItemData.atk / 100;
        }
        else
        {
            // 杖は自分の魔力＋武器威力
            var rates = GameDatabase.Prm_ClassWeaponRate_Get(pc.playerID, pc.GetSaveParameter().ClassID);
            var rate = rates.Get(GameDatabase.ItemType.Rod);
            var wpnAtk = itm.ItemData.atk * rate / 100;
            if (pc.param.Mag < 100)
                healNum = pc.param.Mag + wpnAtk;
            else
                healNum = pc.param.Mag + pc.param.Mag * wpnAtk / 100;
        }

        target.param.HP += healNum;
        if (target.param.HP > target.param.MaxHP) target.param.HP = target.param.MaxHP;

        manager.soundMan.PlaySE(se_heal);
        target.UpdateHP();
        yield return new WaitForSeconds(1f);

        // 経験値取得・レベルアップ処理
        if (itm.ItemData.iType == ItemType.Rod)
        {
            var expGet = ExpCalcHeal(pc);
            if (expGet > 0)
            {
                yield return ExpGetCoroutine(pc, expGet);
            }
        }

        // 武器破壊処理
        itm.useCount--;
        if (itm.useCount <= 0)
        {
            yield return BattleWeaponBreak(pc, itemIndex);
        }
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
        // スキルにより武器消耗回数変更
        if (atk && pc.HasSkill(SkillID.Drows_WeaponSave)) decCount = 0;
        else if (pc.HasSkill(SkillID.Drows_WeaponBreak)) decCount = 2;

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
    /// <param name="useChr">使ってたキャラ</param>
    /// <param name="idx"></param>
    /// <returns></returns>
    private IEnumerator BattleWeaponBreak(PlayerCharacter useChr, int idx)
    {
        var msg = ManagerSceneScript.GetInstance().lineMessageUI;
        var itm = GameParameter.otherData.haveItemList[idx];
        var data = itm.ItemData;
        yield return msg.ShowCoroutine($"{itm.ItemData.name} が壊れた");

        GameParameter.otherData.DeleteItem(idx);

        // 生成
        if (useChr.HasSkill(SkillID.Koob_Archemy) && Util.RandomCheck(30))
        {
            var createId = GameDatabase.CalcRandomItem(field.Prm_BattleFloor, true, false, data.iType);
            if (createId != ItemID.FreeHand)
            {
                yield return BattleWeaponDrop(createId);
            }
        }
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
        if (atkDmg == 0) return 1;

        var checkLv = pc.param.Lv;
        var savePrm = pc.GetSaveParameter();
        if (savePrm.ClassID == Constant.ClassID.A || savePrm.ClassID == Constant.ClassID.B) checkLv += 20;
        else if (savePrm.ClassID != Constant.ClassID.Base) checkLv += 40;
        checkLv += savePrm.ReviveCount * 50;

        var exp = 40 + (enemyLv - checkLv) * 4;

        if (!isDefeat) exp /= 3;
        if (exp < 1) return 1;
        if (exp > 100) return 100;
        return exp;
    }

    /// <summary>
    /// 回復の取得経験値計算
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    private int ExpCalcHeal(PlayerCharacter pc)
    {
        return 15;
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
        if (pc.HasSkill(SkillID.Koob_Zenius))
        {
            exp = exp * 150 / 100;
            if (exp > 100) exp = 100;
        }

        // UI表示
        yield return cellUI.ShowExpCoroutine(pc.GetLocation(), exp);

        // 加算
        var lvup = false;
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

            lvup = true;
        }

        // セーブデータにも反映
        GameParameter.Prm_SetFieldParam(pc.playerID, pc.param);

        if (lvup) pc.CheckGetSkill();
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
            if (pc.param.HP > pc.param.MaxHP) pc.param.HP = pc.param.MaxHP;
            saveParam.ClassID = Constant.ClassID.Base;
            saveParam.ReviveCount += 1;

            pc.CheckDeleteSkill();
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

            pc.CheckGetSkill();
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
        var savePow = Mathf.Pow(nowParam.ReviveCount, 1.5f);
        var saveRate = (savePow + 4f) / (savePow + 5f);
        var calcAct = new Func<int, int, int>((nowNum, baseNum) =>
        {
            var diff = nowNum - baseNum;
            var save = Mathf.CeilToInt(diff * saveRate);
            return save - diff;
        });
        //downParam.maxHp = calcAct(nowParam.MaxHP, baseParam.maxHp);
        downParam.maxHp = 0;
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

    #region ゲームオーバー

    /// <summary>
    /// ゲームオーバー判定
    /// </summary>
    /// <returns></returns>
    private bool Gameover_Check()
    {
        // プレイヤーが誰も居なかったら終わり
        var players = field.GetPlayers();
        return players.Count == 0;
    }

    private bool gameover_shown = false;
    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameoverCoroutine()
    {
        // UI表示
        gameover_shown = false;
        var alpha = new DeltaFloat();
        alpha.Set(0f);
        alpha.MoveTo(1f, 1.5f, DeltaFloat.MoveType.LINE);
        gameoverScreen.gameObject.SetActive(true);
        gameoverScreen.alpha = 0f;
        while (alpha.IsActive())
        {
            yield return null;
            gameoverScreen.alpha = alpha.Get();
        }

        gameover_shown = true;
        yield return new WaitWhile(() => gameover_shown);
    }

    /// <summary>
    /// ゲームオーバー後クリック
    /// </summary>
    public void Gameover_Click()
    {
        gameover_shown = false;
    }

    #endregion

    #region 次のフロアに進む

    /// <summary>
    /// 次に進む判定
    /// </summary>
    /// <returns></returns>
    private bool NextFloor_Check()
    {
        var players = field.GetPlayers();

        // ゲームオーバーは次行かない
        if (players.Count == 0) return false;

        // 左端じゃないキャラが居たら次行かない
        if (players.Any(p => p.GetLocation().x != 0)) return false;

        // 全員左端
        return true;
    }

    /// <summary>
    /// 次のフロアに進むコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextFloorCoroutine()
    {
        if (field.GetEnemies().Count == 0)
        {
            // 全滅させていたらHP回復
            var healed = false;
            foreach (var p in field.GetPlayers())
            {
                if (p.param.HP < p.param.MaxHP)
                {
                    healed = true;
                    p.param.HP += p.param.MaxHP / 10;
                    if (p.param.HP > p.param.MaxHP) p.param.HP = p.param.MaxHP;

                    p.UpdateHP();
                }
            }
            if (healed)
            {
                ManagerSceneScript.GetInstance().soundMan.PlaySE(se_heal);
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return field.NextFloor();
    }

    #endregion
}

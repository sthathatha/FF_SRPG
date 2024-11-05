using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static GameDatabase;

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
        bool startGame = true;
        while (true)
        {
            if (startGame)
            {
                //�ŏ��̂ݍs�����Ɓ@���ɖ����H
            }
            else
            {
                // �ŏ��ȊO�@�Z�[�u
                field.SaveField();
            }
            startGame = false;

            // �^�[�����Z
            field.Prm_BattleTurn++;

            yield return PlayerTurn();
            field.ResetAllActable();

            if (Gameover_Check()) break;
            if (NextFloor_Check())
            {
                yield return NextFloorCoroutine();
                continue;
            }

            yield return EnemyTurn();
            field.ResetAllActable();

            if (Gameover_Check()) break;
            if (NextFloor_Check())
            {
                yield return NextFloorCoroutine();
            }
        }

        yield return GameoverCoroutine();
        // �^�C�g����ʂɖ߂�
        ManagerSceneScript.GetInstance().LoadMainScene("TitleScene", 0);
    }

    /// <summary>
    /// �I�v�V�����{�^���N���b�N
    /// </summary>
    public void OptionClick()
    {
        // ���ł�
        ManagerSceneScript.GetInstance().optionUI.Open();
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
                if (field.GetActableChara(true).Count == 0) break;

                // ���̃t���A�ɍs����ԂɂȂ��Ă��I��
                if (NextFloor_Check()) break;
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
                PTurnSetActEnd(pc);
                callback?.Invoke(1);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.Escape)
            {
                // �P��
                var se = manager.soundMan.PlaySELoop(se_escape);
                manager.soundMan.StopLoopSE(se, 1f);
                field.DeleteCharacter(pc, false);
                callback?.Invoke(1);
                yield break;
            }
            else if (command.Result == CommandUI.CommandResult.ClassChange)
            {
                // �N���X�`�F���W
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
        var initItemFilter = true;

        while (true)
        {
            // �s���A�C�e���I��UI
            yield return itemui.ShowCoroutine(pc, initItemFilter);
            initItemFilter = false;
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
            if (!selAtkCells.Contains(field.InputLoc)) continue;
            var selAtkChr = field.GetCellCharacter(field.InputLoc);
            // �U���ΏۈȊO��I�񂾂�L�����Z��
            if (selAtkChr == null) continue;

            if (selItem.iType == GameDatabase.ItemType.Item ||
                selItem.iType == GameDatabase.ItemType.Rod)
            {
                // �A�C�e���E��̓L���������ԂłȂ���΃L�����Z��
                if (!selAtkChr.IsPlayer()) continue;
                // ���ł�HP�ő�Ȃ�L�����Z��
                if (selAtkChr.param.HP == selAtkChr.param.MaxHP) continue;
                yield return PTurnHealCoroutine(pc, selAtkChr as PlayerCharacter, itemui.Result_SelectIndex);
                PTurnSetActEnd(pc);
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

                field.ShowTile(new List<Vector2Int> { field.InputLoc }, COLOR_TILE_ATTACK);
                yield return estUI.ShowCoroutine(battleParam, pc, selAtkChr as EnemyCharacter);
                field.ClearTiles();

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
    /// �s���I���Z�b�g
    /// </summary>
    /// <param name="pc"></param>
    private void PTurnSetActEnd(PlayerCharacter pc)
    {
        // ���[�h�֎~�t���O�Z�b�g
        field.LoadDisableSet();

        // �G���������Ȃ�������I�����Ȃ�
        if (field.GetEnemies().Count == 0)
        {
            pc.PlayAnim(Constant.Direction.None);
            field.ResetAllActable();
            return;
        }

        pc.SetActable(false);
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

        // �G�����Ȃ�������^�[���Ȃ�
        if (field.GetActableChara(false).Count == 0) yield break;

        // �^�[���\��
        yield return manager.turnDisplay.DisplayTurnStart(false);

        // ������̂����Ȃ��Ȃ�܂��̓v���C���[�����Ȃ��Ȃ�܂�
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
                // �U���ł��鑊�肪���Ȃ�
                if (enm.isBoss)
                {
                    // �{�X�͓����Ȃ�
                    enm.SetActable(false);
                    continue;
                }

                // �߂Â��v���C���[��I��
                var player = field.GetPlayers().OrderBy(p =>
                {
                    var dist = p.GetLocation() - enm.GetLocation();
                    return Math.Abs(dist.x) + Math.Abs(dist.y);
                }).First(); // ��ԋ߂��L����
                // ��ԋ߂��ړ����I��
                var moveSel = moveList.OrderBy(m =>
                {
                    var dist = player.GetLocation() - m.current;
                    return Math.Abs(dist.x) + Math.Abs(dist.y);
                }).First();
                // �ړ�
                yield return enm.Walk(moveSel);
                enm.SetLocation(moveSel.current);
                enm.SetActable(false);
            }
            else
            {
                // �ړ�
                yield return enm.Walk(atkAI.moveHist);
                enm.SetLocation(atkAI.moveHist.current);

                // �U���\��
                field.ShowTile(new List<Vector2Int>() { atkAI.target.GetLocation() }, COLOR_TILE_ATTACK);
                yield return new WaitForSeconds(0.5f);
                field.ClearTiles();

                // �U������
                var battleParam = GameParameter.GetBattleParameter(enm, atkAI.target);
                yield return TurnBattleCoroutine(battleParam, enm, atkAI.target, null);

                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    #endregion

    #region �퓬�E�񕜃A�j���[�V����

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
        var sound = manager.soundMan;
        var aLoc = field.GetCellPosition(atkChr.GetLocation());
        var dLoc = field.GetCellPosition(defChr.GetLocation());
        var aDist = dLoc - aLoc;
        var dDist = aLoc - dLoc;

        // �o���l�v�Z�p�ϐ�
        PlayerCharacter expTmp_chr = (atkChr.IsPlayer() ? atkChr : defChr) as PlayerCharacter;
        var expTmp_defeat = false;
        var expTmp_dmg = 0;
        var expTmp_enemyLv = (atkChr.IsPlayer() ? defChr : atkChr).param.Lv;

        // �A�j���[�V�����ݒ�
        atkChr.PlayAnim(Util.GetDirectionFromVec(aDist));
        defChr.PlayAnim(Util.GetDirectionFromVec(dDist));

        var weaponBreak = -1;
        var weaponDrop = GameDatabase.ItemID.FreeHand;
        var phase = 0;
        while (param.a_atkCount > 0 || param.d_atkCount > 0)
        {
            // �ǂ������s�����邩
            var atkTurn = true;
            // �ǂ�������0�̏ꍇ�͂�������m��
            if (param.a_atkCount == 0) atkTurn = false;
            else if (param.d_atkCount == 0) atkTurn = true;
            else
            {
                // �ŏ��͍U����
                if (phase == 0)
                {
                    atkTurn = true;
                }
                else
                {
                    // 2��ڂ͖h�䑤�A3��ڈȍ~�͕��ʂǂ�������0�ɂȂ��Ă���͂�
                    atkTurn = false;
                }
            }
            var chrA = atkTurn ? atkChr : defChr;
            var chrD = atkTurn ? defChr : atkChr;

            var isHit = false;
            var isCrt = false;
            var dmg = 0;
            #region �_���[�W���肵��SE�Đ��A�N�V����
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

            // �������
            if (chrA.IsPlayer())
            {
                var breakIdx = BattleWeaponDecrease(chrA as PlayerCharacter, true);
                if (breakIdx >= 0)
                {
                    weaponBreak = breakIdx;
                    // ��ꂽ��Ō�̍U���ɂ���
                    if (atkTurn) param.a_atkCount = 1;
                    else param.d_atkCount = 1;
                }
            }

            // �U���񐔂��P�����炷
            if (atkTurn) param.a_atkCount--;
            else param.d_atkCount--;

            // �U����
            StartCoroutine(chrA.AttackAnim(atkTurn ? aDist : dDist));
            if (atkTurn) CalcAtk(param.a_hit, param.a_critical, param.a_dmg, chrD.param.HP);
            else CalcAtk(param.d_hit, param.d_critical, param.d_dmg, chrD.param.HP);
            yield return new WaitForSeconds(0.15f);
            // HP���炵�ăQ�[�W�X�V
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
                        // �v���C���[�����񂾂�o���l�v�Z����
                        expTmp_chr = null;
                        expTmp_dmg = 0;
                    }
                    else
                    {
                        expTmp_defeat = true;
                    }
                    if (atkTurn) defChr = null;
                    else atkChr = null;

                    // �h���b�v����
                    if (!chrD.IsPlayer()) weaponDrop = (chrD as EnemyCharacter).dropID;

                    // ���񂾂�����Đ퓬�I��
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
            if (atkChr.IsPlayer()) PTurnSetActEnd(atkChr as PlayerCharacter);
            else atkChr.SetActable(false);
        }
        defChr?.PlayAnim(Constant.Direction.None);

        // �o���l�擾�E���x���A�b�v����
        if (expTmp_chr != null)
        {
            var expGet = ExpCalcBattle(expTmp_chr, expTmp_enemyLv, expTmp_dmg, expTmp_defeat);
            if (expGet > 0)
            {
                yield return ExpGetCoroutine(expTmp_chr, expGet);
            }
        }

        // ����j�󏈗�
        if (weaponBreak >= 0)
        {
            yield return BattleWeaponBreak(weaponBreak);
        }
        // �A�C�e���h���b�v����
        if (weaponDrop != GameDatabase.ItemID.FreeHand)
        {
            yield return BattleWeaponDrop(weaponDrop);
        }

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
        var manager = ManagerSceneScript.GetInstance();
        pc.PlayAnim(Constant.Direction.None);

        // �񕜗ʌ���
        var healNum = 0;
        var itm = GameParameter.otherData.haveItemList[itemIndex];
        if (itm.ItemData.iType == GameDatabase.ItemType.Item)
        {
            // ��͎����̍ő�HP����
            healNum = target.param.MaxHP * itm.ItemData.atk / 100;
        }
        else
        {
            // ��͎����̖��́{����З�
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

        // �o���l�擾�E���x���A�b�v����
        if (itm.ItemData.iType == ItemType.Rod)
        {
            var expGet = ExpCalcHeal(pc);
            if (expGet > 0)
            {
                yield return ExpGetCoroutine(pc, expGet);
            }
        }

        // ����j�󏈗�
        itm.useCount--;
        if (itm.useCount <= 0)
        {
            yield return BattleWeaponBreak(itemIndex);
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="atk">true:�U�����@false:������</param>
    /// <returns>��ꂽ�ꍇ�A�C�e���ԍ�</returns>
    private int BattleWeaponDecrease(PlayerCharacter pc, bool atk)
    {
        var decCount = 1;
        // �X�L���ɂ�蕐����Չ񐔕ύX
        if (atk && pc.HasSkill(SkillID.Drows_WeaponSave)) decCount = 0;
        else if (pc.HasSkill(SkillID.Drows_WeaponBreak)) decCount = 2;

        if (decCount == 0) return -1; //���Ղ��Ȃ�

        var equipIndex = GameParameter.otherData.GetEquipIndex(pc.playerID);
        if (equipIndex < 0) return -1; //�f��͖���

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
    /// ��ꂽ������폜���鉉�o
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    private IEnumerator BattleWeaponBreak(int idx)
    {
        var msg = ManagerSceneScript.GetInstance().lineMessageUI;
        var itm = GameParameter.otherData.haveItemList[idx];
        yield return msg.ShowCoroutine($"{itm.ItemData.name} ����ꂽ");

        GameParameter.otherData.DeleteItem(idx);
    }

    /// <summary>
    /// �A�C�e���擾���o
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private IEnumerator BattleWeaponDrop(GameDatabase.ItemID id)
    {
        var msg = ManagerSceneScript.GetInstance().lineMessageUI;
        var itm = GameDatabase.ItemDataList[(int)id];
        yield return msg.ShowCoroutine($"{itm.name} ����ɓ��ꂽ");

        GameParameter.otherData.AddItem(id);
    }

    #endregion

    #region �o���l�擾�E���x���A�b�v����

    /// <summary>
    /// �퓬�̎擾�o���l�v�Z
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
    /// �񕜂̎擾�o���l�v�Z
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    private int ExpCalcHeal(PlayerCharacter pc)
    {
        return 15;
    }

    /// <summary>
    /// �o���l�擾����
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="exp"></param>
    /// <returns></returns>
    private IEnumerator ExpGetCoroutine(PlayerCharacter pc, int exp)
    {
        var manager = ManagerSceneScript.GetInstance();

        if (exp == 0 || pc.param.Lv >= 20) yield break;

        // UI�\��
        yield return cellUI.ShowExpCoroutine(pc.GetLocation(), exp);

        // ���Z
        pc.param.Exp += exp;
        if (pc.param.Exp >= 100)
        {
            manager.soundMan.PlaySE(se_levelup);
            yield return cellUI.ShowLevelUpCoroutine(pc.GetLocation());
            // ���x���A�b�v
            pc.param.Lv++;
            pc.param.Exp = pc.param.Lv >= 20 ? 0 : pc.param.Exp - 100;

            // �㏸�l����
            var upParam = GameDatabase.Prm_PlayerGrow_GetCalced(pc.playerID);

            // �\��
            yield return manager.statusUpUI.ShowLvup(pc, upParam);

            // �p�����[�^�X�V
            pc.param.MaxHP += upParam.maxHp;
            pc.param.Atk += upParam.atk;
            pc.param.Mag += upParam.mag;
            pc.param.Tec += upParam.tec;
            pc.param.Spd += upParam.spd;
            pc.param.Luk += upParam.luk;
            pc.param.Def += upParam.def;
            pc.param.Mdf += upParam.mdf;
            pc.UpdateHP(true);

            pc.CheckGetSkill();
        }

        // �Z�[�u�f�[�^�ɂ����f
        GameParameter.Prm_SetFieldParam(pc.playerID, pc.param);
    }

    #endregion

    #region �N���X�`�F���W����

    /// <summary>
    /// �N���X�`�F���W����
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="callback">true:�N���X�`�F���W���s</param>
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

        // �N���X�`�F���W���s
        var saveParam = pc.GetSaveParameter();
        if (ccui.Result == ClassChangeUI.CCSelect.Rebirth)
        {
            // �]��
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

            pc.CheckDeleteSkill();
        }
        else
        {
            // �N���X�`�F���W
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
    /// �]�����̃_�E���p�����[�^���v�Z
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    private GameDatabase.ParameterData ClassChangeRebirthCalc(PlayerCharacter pc)
    {
        var downParam = new GameDatabase.ParameterData();
        var nowParam = pc.GetSaveParameter();
        var baseParam = GameDatabase.Prm_PlayerInit[(int)pc.playerID];

        // �c�銄��
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

    #region �Q�[���I�[�o�[

    /// <summary>
    /// �Q�[���I�[�o�[����
    /// </summary>
    /// <returns></returns>
    private bool Gameover_Check()
    {
        // �v���C���[���N�����Ȃ�������I���
        var players = field.GetPlayers();
        return players.Count == 0;
    }

    private bool gameover_shown = false;
    /// <summary>
    /// �Q�[���I�[�o�[����
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameoverCoroutine()
    {
        //todo:�����L���O�o�^

        // UI�\��
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
    /// �Q�[���I�[�o�[��N���b�N
    /// </summary>
    public void Gameover_Click()
    {
        gameover_shown = false;
    }

    #endregion

    #region ���̃t���A�ɐi��

    /// <summary>
    /// ���ɐi�ޔ���
    /// </summary>
    /// <returns></returns>
    private bool NextFloor_Check()
    {
        var players = field.GetPlayers();

        // �Q�[���I�[�o�[�͎��s���Ȃ�
        if (players.Count == 0) return false;

        // ���[����Ȃ��L�����������玟�s���Ȃ�
        if (players.Any(p => p.GetLocation().x != 0)) return false;

        // �S�����[
        return true;
    }

    /// <summary>
    /// ���̃t���A�ɐi�ރR���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextFloorCoroutine()
    {
        yield return field.NextFloor();
    }

    #endregion
}

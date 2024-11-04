using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// �t�B�[���h�Ǘ�
/// </summary>
public class FieldSystem : MonoBehaviour
{
    #region �萔

    public const int ROW_COUNT = 7;
    public const int COL_COUNT = 15;

    private const float CELL_SIZE = 64f;
    private const float ZERO_X = -(COL_COUNT - 1) * CELL_SIZE / 2f;
    private const float ZERO_Y = -(ROW_COUNT - 1) * CELL_SIZE / 2f;

    #endregion

    #region �����o�[

    public SpriteRenderer tile_dummy;
    /// <summary>�v���C���[�_�~�[</summary>
    public PlayerCharacter player_dummy;
    /// <summary>�G�_�~�[</summary>
    public EnemyCharacter enemy_dummy;

    /// <summary>�c���_�~�[</summary>
    public Transform colLine_dummy;
    /// <summary>�g���e�I�u�W�F�N�g</summary>
    public Transform Line_parent;
    public Transform Character_parent;
    public Transform Tile_parent;

    public Button btn_TurnEnd;

    public TMP_Text floorDisplay;

    #endregion

    #region �t�B�[���h���e

    /// <summary>�c�����X�g</summary>
    private List<Transform> colLines = new List<Transform>();

    /// <summary>�^�C���\�����X�g</summary>
    private List<SpriteRenderer> tiles = new List<SpriteRenderer>();

    /// <summary>�v���C���[���X�g</summary>
    private List<PlayerCharacter> players = new List<PlayerCharacter>();
    /// <summary>�G���X�g</summary>
    private List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    private int _battleFloor = 0;
    /// <summary>�퓬��</summary>
    public int Prm_BattleFloor
    {
        get
        {
            return _battleFloor;
        }
        private set
        {
            _battleFloor = value;
            floorDisplay.SetText(value.ToString());
        }
    }
    /// <summary>�퓬���̃^�[����</summary>
    public int Prm_BattleTurn { get; set; } = 0;

    public bool Prm_EnableRanking { get; set; } = true;

    #endregion

    #region ����

    /// <summary>
    /// ������
    /// </summary>
    void Start()
    {
        tile_dummy.gameObject.SetActive(false);
        player_dummy.gameObject.SetActive(false);
        enemy_dummy.gameObject.SetActive(false);
        colLine_dummy.gameObject.SetActive(false);

        InitLines();
    }

    #endregion

    #region �Z�[�u����

    private const string SAVE_PLAYERDATA = "pChrData";
    private const string SAVE_ENEMYDATA = "eChrData";
    private const string SAVE_TURN = "turnNum";
    private const string SAVE_FLOOR = "floorNum";

    private const string SAVE_ENABLERANKING = "enbRank";
    private const string SAVE_LOADCOUNT = "loadCount";

    /// <summary>
    /// �Z�[�u
    /// </summary>
    public void SaveField()
    {
        // ���ڋN�����Z�[�u���Ȃ�
        if (ManagerSceneScript.isDebugLoad) return;

        var save = Global.GetSaveData();

        save.SetGameData(SAVE_PLAYERDATA, string.Join('P', players.Select(p => p.ToSaveString())));
        save.SetGameData(SAVE_ENEMYDATA, string.Join('E', enemies.Select(e => e.ToSaveString())));
        save.SetGameData(SAVE_FLOOR, Prm_BattleFloor);
        save.SetGameData(SAVE_TURN, Prm_BattleTurn);
        save.SetGameData(SAVE_ENABLERANKING, Prm_EnableRanking ? 1 : 0);
        save.SetGameData(SAVE_LOADCOUNT, 0);

        GameParameter.Save();
        save.SaveGameData();
    }

    /// <summary>
    /// ���[�h
    /// </summary>
    public void LoadField()
    {
        var save = Global.GetSaveData();
        save.LoadGameData();

        GameParameter.Load();
        Prm_EnableRanking = save.GetGameDataInt(SAVE_ENABLERANKING) == 1;
        var ldCnt = save.GetGameDataInt(SAVE_LOADCOUNT);
        if (ldCnt > 0) Prm_EnableRanking = false;
        save.SetGameData(SAVE_LOADCOUNT, ldCnt + 1);
        save.SaveGameData();

        Prm_BattleFloor = save.GetGameDataInt(SAVE_FLOOR);
        Prm_BattleTurn = save.GetGameDataInt(SAVE_TURN);
        var pdata = save.GetGameDataString(SAVE_PLAYERDATA);
        players.Clear();
        if (!string.IsNullOrEmpty(pdata))
        {
            foreach (var p in pdata.Split(new char[] { 'P' }))
            {
                CreatePlayer(p);
            }
        }
        var edata = save.GetGameDataString(SAVE_ENEMYDATA);
        enemies.Clear();
        if (!string.IsNullOrEmpty(edata))
        {
            foreach (var e in edata.Split(new char[] { 'E' }))
            {
                CreateEnemy(e);
            }
        }
    }

    #endregion

    #region �N���b�N����

    #region �C�x���g����
    /// <summary>����������</summary>
    private bool longClicked = false;

    /// <summary>����������R���[�`��</summary>
    private IEnumerator longClickCoroutine = null;
    /// <summary>
    /// ����������R���[�`��
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private IEnumerator LongClickCheck(Vector2 pos)
    {
        yield return new WaitForSeconds(0.5f);
        longClicked = true;
        LongClick(pos);
    }
    /// <summary>
    /// �}�E�X�������݃C�x���g
    /// </summary>
    /// <param name="ev"></param>
    public void EventMouseDown(BaseEventData ev)
    {
        if (longClickCoroutine != null)
        {
            StopCoroutine(longClickCoroutine);
        }

        longClicked = false;
        var pev = ev as PointerEventData;
        longClickCoroutine = LongClickCheck(pev.position);
        StartCoroutine(longClickCoroutine);
    }
    /// <summary>
    /// �}�E�X�����C�x���g
    /// </summary>
    public void EventMouseUp()
    {
        if (longClickCoroutine != null)
        {
            StopCoroutine(longClickCoroutine);
            longClickCoroutine = null;
        }
    }
    /// <summary>
    /// �}�E�X�������C�x���g
    /// </summary>
    public void EventMouseMove()
    {
        if (longClickCoroutine != null)
        {
            StopCoroutine(longClickCoroutine);
            longClickCoroutine = null;
            longClicked = true;
        }
    }
    #endregion

    /// <summary>
    /// �N���b�N�C�x���g
    /// </summary>
    /// <param name="ev"></param>
    public void EventClick(BaseEventData ev)
    {
        // ������������̓N���b�N�������Ȃ�
        if (longClicked) return;

        var pev = ev as PointerEventData;
        var loc = GetLocationFromPos(pev.position);
        if (loc.x < 0 || loc.y < 0) InputResult = InputResultEnum.OutCell;
        else InputResult = InputResultEnum.Cell;

        InputLoc = loc;
        InputLong = false;
        waitingInput = false;
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="pos"></param>
    private void LongClick(Vector2 pos)
    {
        if (!waitingInput) return;

        var loc = GetLocationFromPos(pos);
        //InputPos = loc;
        //InputLong = true;
        //waitingInput = false;

        var locChr = GetCellCharacter(loc);
        if (locChr != null)
        {
            var statusWindow = ManagerSceneScript.GetInstance().statusScreen;
            statusWindow.DispParameter(locChr);
            statusWindow.Show();
        }
    }

    /// <summary>
    ///  ���͌���
    /// </summary>
    public enum InputResultEnum
    {
        OutCell = 0,
        Cell,
        TurnEnd,
    }
    public InputResultEnum InputResult { get; private set; } = InputResultEnum.OutCell;

    private bool waitingInput = false;
    private bool inputEnableTurnEnd = false;
    public Vector2Int InputLoc { get; set; }
    public bool InputLong { get; set; }
    public IEnumerator WaitInput(bool enableTurnEnd = false)
    {
        waitingInput = true;
        inputEnableTurnEnd = enableTurnEnd;
        btn_TurnEnd.interactable = enableTurnEnd;
        yield return new WaitWhile(() => waitingInput);
        btn_TurnEnd.interactable = false;
    }

    /// <summary>
    /// �^�[���I���{�^�����N���b�N
    /// </summary>
    public void TurnEndClick()
    {
        InputResult = InputResultEnum.TurnEnd;
        waitingInput = false;
    }

    #endregion

    #region ���W�Ǘ�

    /// <summary>
    /// �Z���̕`��ʒu
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    public Vector3 GetCellPosition(Vector2Int loc)
    {
        var v = Vector3.zero;
        v.x = ZERO_X + (loc.x * CELL_SIZE);
        v.y = ZERO_Y + (loc.y * CELL_SIZE);
        return v;
    }

    /// <summary>
    /// ���W����Z���ʒu���擾
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private Vector2Int GetLocationFromPos(Vector2 p)
    {
        var loc = Vector2Int.zero;
        loc.x = Mathf.FloorToInt(p.x / CELL_SIZE);
        loc.y = Mathf.FloorToInt(p.y / CELL_SIZE);

        if (loc.x < 0 || loc.x >= COL_COUNT || loc.y < 0 || loc.y >= ROW_COUNT) return new Vector2Int(-1, -1);

        return loc;
    }

    #endregion

    #region �g���Ǘ�

    /// <summary>
    /// �g���̈ʒu
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    private Vector3 LinePos(int col)
    {
        var v = Vector3.zero;
        v.x = ZERO_X + CELL_SIZE * (col + 0.5f);
        return v;
    }

    /// <summary>
    /// ��̘g�����쐬
    /// </summary>
    private void InitLines()
    {
        for (var c = 0; c < COL_COUNT; c++)
        {
            var line = Instantiate(colLine_dummy, Line_parent, false);
            line.gameObject.SetActive(true);
            line.localPosition = LinePos(c);
            colLines.Add(line);
        }
    }

    /// <summary>
    /// ���̃t���A�̐��𐶐�
    /// </summary>
    private void CreateNextLine()
    {
        for (var c = -1; c > -COL_COUNT; c--)
        {
            var line = Instantiate(colLine_dummy, Line_parent, false);
            line.gameObject.SetActive(true);
            line.localPosition = LinePos(c - COL_COUNT);
            colLines.Insert(0, line);
        }
    }

    /// <summary>
    /// �S���̏c���̍��W���w��
    /// </summary>
    /// <param name="headLineIdx">�퓬�̗�</param>
    /// <param name="addX">���ZX</param>
    private void SetAllLinePos(int headLineIdx, float addX)
    {
        for (var i = 0; i < colLines.Count; i++)
        {
            var p = LinePos(i + headLineIdx);
            p.x += addX;
            colLines[i].localPosition = p;
        }
    }

    /// <summary>
    /// �Â��g�����폜
    /// </summary>
    private void DeleteOldLine()
    {
        var limitX = LinePos(COL_COUNT - 1).x + 1f;

        for (var idx = colLines.Count - 1; idx >= 0; idx--)
        {
            // �E�[��荶�̂�����������I��
            if (colLines[idx].localPosition.x < limitX) break;

            Destroy(colLines[idx].gameObject);
            colLines.RemoveAt(idx);
        }
    }

    #endregion

    #region �^�C������

    /// <summary>
    /// �^�C���N���A
    /// </summary>
    public void ClearTiles()
    {
        foreach (var t in tiles)
        {
            Destroy(t.gameObject);
        }
        tiles.Clear();
    }

    /// <summary>
    /// �^�C���\���ǉ�
    /// </summary>
    /// <param name="list"></param>
    /// <param name="col"></param>
    public void ShowTile(List<Vector2Int> list, Color col)
    {
        foreach (var l in list)
        {
            var tile = Instantiate(tile_dummy);
            tile.color = col;
            tile.transform.SetParent(Tile_parent, false);
            tile.transform.localPosition = GetCellPosition(l);
            tile.gameObject.SetActive(true);

            tiles.Add(tile);
        }
    }

    #endregion

    #region �L�����s���Ǘ�

    /// <summary>
    /// �S�L�����s���t���O���Z�b�g
    /// </summary>
    public void ResetAllActable()
    {
        foreach (var pc in players)
        {
            pc.SetActable(true);
        }
        foreach (var en in enemies)
        {
            en.SetActable(true);
        }
    }

    /// <summary>
    /// �s���\�L�������X�g�擾
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public List<CharacterBase> GetActableChara(bool player)
    {
        var ret = new List<CharacterBase>();
        if (player) ret.AddRange(players.Where(p => p.turnActable));
        else ret.AddRange(enemies.Where(e => e.turnActable));

        return ret;
    }

    #endregion

    #region �t�B�[���h�f�[�^�Ǘ�

    /// <summary>
    /// �t�B�[���h�̏�Ԃ��쐬
    /// </summary>
    public void InitField()
    {
        if (Global.GetTemporaryData().isLoadGame)
        {
            // �Z�[�u�f�[�^����쐬
            LoadField();
        }
        else
        {
            Prm_BattleFloor = 1;
            Prm_BattleTurn = 0;
            Prm_EnableRanking = true;

            // 6�l������
            GameParameter.Prm_Drows.Init(Constant.PlayerID.Drows);
            GameParameter.Prm_Eraps.Init(Constant.PlayerID.Eraps);
            GameParameter.Prm_Exa.Init(Constant.PlayerID.Exa);
            GameParameter.Prm_Worra.Init(Constant.PlayerID.Worra);
            GameParameter.Prm_Koob.Init(Constant.PlayerID.Koob);
            GameParameter.Prm_You.Init(Constant.PlayerID.You);

            // �h���V�[
            CreatePlayer(Constant.PlayerID.Drows, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 1));
            // �G��
            CreatePlayer(Constant.PlayerID.Eraps, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 2));
            // �G�O�U
            CreatePlayer(Constant.PlayerID.Exa, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 3));
            // �E�[��
            CreatePlayer(Constant.PlayerID.Worra, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 4));
            // �N�[
            CreatePlayer(Constant.PlayerID.Koob, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 5));
            // �I
            CreatePlayer(Constant.PlayerID.You, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 6));

            // �����i��������
            GameParameter.otherData.Init();

            CreateRandomEnemy(false);
        }
    }

    /// <summary>
    /// �G�������_������
    /// </summary>
    /// <param name="next">true:���̃t���A�������iX���W�}�C�i�X�j false:���̍��W�Ő���</param>
    private void CreateRandomEnemy(bool next = true)
    {
        var addX = next ? -COL_COUNT + 1 : 0;

        // 3�t���A���ƂɓG���x���P�A�b�v
        var lv = Prm_BattleFloor / 3 + 1;

        // �h���b�v�A�C�e�����m���Ɏ����Ă�����
        var dropIndex = -1;
        var weaponTotalRest = GameParameter.otherData.haveItemList.Sum(itm =>
        {
            if (itm.ItemData.iType == GameDatabase.ItemType.None ||
            itm.ItemData.iType == GameDatabase.ItemType.Rod ||
            itm.ItemData.iType == GameDatabase.ItemType.Item) return 0;
            else return itm.useCount;
        });
        // �܂̕���̍��v��10��ȉ��Ȃ�m���ɕ���h���b�v
        if (weaponTotalRest < 10) dropIndex = Util.RandomInt(0, 4);

        // 4�`6�̂̉����G�𐶐�
        var weakCnt = Util.RandomInt(4, 6);
        for (var i = 0; i < weakCnt; i++)
        {
            //todo:�G���GID��I��
            var eid = (Constant.EnemyID)Util.RandomInt(0, 0);

            var e = Instantiate(enemy_dummy, Character_parent, false);
            e.gameObject.SetActive(true);
            e.SetLocation(new Vector2Int(Util.RandomInt(1, 5) + addX, i));
            e.SetCharacter(eid);
            e.InitParameter(lv);
            if (dropIndex == i)
                e.SetWeaponAndDrop(drp: GameDatabase.CalcRandomItem(lv, false));
            else
                e.SetWeaponAndDrop();
            enemies.Add(e);
        }

        // ���G�o��
        if (Prm_BattleFloor % 10 == 0)
        {
            // ���G��Lv
            var strongLv = lv >= 100 ? Mathf.FloorToInt(lv * 1.1f) : lv + 10;

            //todo:���GID��I��
            var eid = (Constant.EnemyID)Util.RandomInt(0, 0);

            var e = Instantiate(enemy_dummy, Character_parent, false);
            e.gameObject.SetActive(true);
            e.SetLocation(new Vector2Int(1 + addX, ROW_COUNT - 1));
            e.SetCharacter(eid, true);
            e.InitParameter(strongLv);
            e.SetWeaponAndDrop(drp: GameDatabase.CalcRandomItem(strongLv, true, false));
            enemies.Add(e);
        }
    }

    /// <summary>
    /// �Z���ɋ���L�����N�^�[�擾
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    public CharacterBase GetCellCharacter(Vector2Int loc)
    {
        foreach (var chr in players)
        {
            if (chr.GetLocation() == loc) return chr;
        }
        foreach (var chr in enemies)
        {
            if (chr.GetLocation() == loc) return chr;
        }

        return null;
    }

    /// <summary>
    /// �L�������S����
    /// </summary>
    /// <param name="chr"></param>
    /// <param name="death">true:���S�ɂ��폜�@false:�P�ނɂ��폜</param>
    public void DeleteCharacter(CharacterBase chr, bool death = true)
    {
        if (chr.IsPlayer())
        {
            var pc = chr as PlayerCharacter;
            players.Remove(pc);

            // �������Ԃ�ݒ�
            GameParameter.Prm_Get(pc.playerID).RestBattle = death ? 4 : 2;
        }
        else
        {
            enemies.Remove(chr as EnemyCharacter);
        }

        Destroy(chr.gameObject);
    }

    /// <summary>
    /// �v���C���[�L��������
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="loc"></param>
    /// <returns></returns>
    private PlayerCharacter CreatePlayer(Constant.PlayerID pid, Vector2Int loc)
    {
        var p = Instantiate(player_dummy, Character_parent, false);
        p.gameObject.SetActive(true);
        p.SetLocation(loc);
        p.SetCharacter(pid);
        players.Add(p);
        return p;
    }

    /// <summary>
    /// �v���C���[�L�����쐬�i�Z�[�u�f�[�^����j
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private PlayerCharacter CreatePlayer(string str)
    {
        var p = Instantiate(player_dummy, Character_parent, false);
        p.gameObject.SetActive(true);
        p.FromSaveString(str);
        players.Add(p);
        return p;
    }

    /// <summary>
    /// �G�L�����N�^�[�쐬�i�Z�[�u�f�[�^����j
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private EnemyCharacter CreateEnemy(string str)
    {
        var e = Instantiate(enemy_dummy, Character_parent, false);
        e.gameObject.SetActive(true);
        e.FromSaveString(str);
        enemies.Add(e);
        return e;
    }

    /// <summary>
    /// �v���C���[���X�g�擾
    /// </summary>
    /// <returns></returns>
    public List<PlayerCharacter> GetPlayers() { return players; }

    /// <summary>
    /// �G���X�g�擾
    /// </summary>
    /// <returns></returns>
    public List<EnemyCharacter> GetEnemies() { return enemies; }

    /// <summary>
    /// ���̃t���A�ɐi��
    /// </summary>
    /// <returns></returns>
    public IEnumerator NextFloor()
    {
        const float MOVE_TIME = 1f;
        const float MOVE_X = (COL_COUNT - 1) * CELL_SIZE;
        // ���̃t���A�̓G�𐶐�
        Prm_BattleFloor++;
        Prm_BattleTurn = 0;
        CreateRandomEnemy(true);

        // �V�����c���𐶐�
        CreateNextLine();

        // �L�����Əc����COLCOUNT-1��������
        var addX = new DeltaFloat();
        addX.Set(0f);
        addX.MoveTo(MOVE_X, MOVE_TIME, DeltaFloat.MoveType.LINE);
        while (addX.IsActive())
        {
            yield return null;
            addX.Update(Time.deltaTime);
            var addV = new Vector3(addX.Get(), 0);

            SetAllLinePos(-COL_COUNT + 1, addX.Get());
            foreach (CharacterBase c in players)
                c.transform.localPosition = GetCellPosition(c.GetLocation()) + addV;
            foreach (CharacterBase c in enemies)
                c.transform.localPosition = GetCellPosition(c.GetLocation()) + addV;
        }

        // �O�̃t���A�̓G�Əc��������
        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].GetLocation().x > 0) DeleteCharacter(enemies[i], false);
        }
        for (var i = players.Count - 1; i >= 0; i--)
        {
            // �{���E�ɂ͎c���ĂȂ��͂������ꉞ�v���C���[��
            if (players[i].GetLocation().x > 0) DeleteCharacter(players[i], false);
        }

        // ���W���C��
        foreach (CharacterBase c in players)
        {
            var loc = c.GetLocation();
            loc.x += COL_COUNT - 1;
            c.SetLocation(loc);
        }
        foreach (CharacterBase c in enemies)
        {
            var loc = c.GetLocation();
            loc.x += COL_COUNT - 1;
            c.SetLocation(loc);
        }

        yield return ReturnPlayerCoroutine();

        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// �P�ރv���C���[�̕��A����
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnPlayerCoroutine()
    {
        var getEmptyCell = new Func<Vector2Int>(() =>
        {
            for (var r = ROW_COUNT - 1; r >= 0; --r)
            {
                if (GetCellCharacter(new Vector2Int(COL_COUNT - 1, r)) == null &&
                    GetCellCharacter(new Vector2Int(COL_COUNT, r)) == null)
                    return new Vector2Int(COL_COUNT - 1, r);
            }
            return new Vector2Int(COL_COUNT - 2, ROW_COUNT - 1);
        });

        bool created = false;
        var restCheck = new Action<GameParameter.PlayerSaveParameter, Constant.PlayerID>((prm, id) =>
        {
            if (prm.RestBattle > 0)
            {
                prm.RestBattle--;
                if (prm.RestBattle == 0)
                {
                    // ���A
                    var l = getEmptyCell();
                    var l2 = l + new Vector2Int(1, 0);
                    var p = CreatePlayer(id, l2);
                    StartCoroutine(returnCharacterAnim(p, l));
                    created = true;
                }
            }
        });

        // �U�l�`�F�b�N
        restCheck(GameParameter.Prm_Drows, Constant.PlayerID.Drows);
        restCheck(GameParameter.Prm_Eraps, Constant.PlayerID.Eraps);
        restCheck(GameParameter.Prm_Exa, Constant.PlayerID.Exa);
        restCheck(GameParameter.Prm_Worra, Constant.PlayerID.Worra);
        restCheck(GameParameter.Prm_Koob, Constant.PlayerID.Koob);
        restCheck(GameParameter.Prm_You, Constant.PlayerID.You);

        if (created)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// �v���C���[���A�A�j���[�V����
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="l">�߂������Ƃ̈ʒu</param>
    /// <returns></returns>
    private IEnumerator returnCharacterAnim(PlayerCharacter pc, Vector2Int l)
    {
        var hist = new MoveHistory();
        hist.history.Add(pc.GetLocation());
        hist.history.Add(l);

        yield return pc.Walk(hist);
        pc.PlayAnim(Constant.Direction.None);
        pc.SetLocation(l);
    }

    #endregion

    #region �ʒu����

    /// <summary>
    /// �t�B�[���h�͈͓�����
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    public bool IsInField(Vector2Int loc)
    {
        return loc.x >= 0 && loc.y >= 0 && loc.x < COL_COUNT && loc.y < ROW_COUNT;
    }

    /// <summary>
    /// �ړ��\�ꏊ�����X�g�A�b�v
    /// </summary>
    /// <param name="chr">�L�����N�^�[</param>
    /// <returns></returns>
    public List<MoveHistory> GetMovableLocations(CharacterBase chr)
    {
        var ret = new List<MoveHistory>();

        var checkedList = new List<Vector2Int>();
        var beforeList = new List<MoveHistory>();

        // 0���̈ʒu
        var tmpHist = new MoveHistory();
        tmpHist.history.Add(chr.GetLocation());
        ret.Add(tmpHist);
        beforeList.Add(tmpHist);
        checkedList.Add(chr.GetLocation());

        // �ړ������Ŏ擾
        var range = chr.param.Move;
        ret.AddRange(GetMovableLocations(chr, checkedList, beforeList, range));

        return ret;
    }

    /// <summary>
    /// �ړ��\�ꏊ�̃��X�g�擾
    /// </summary>
    /// <param name="chr">�L�����N�^�[</param>
    /// <param name="checkedList">�ړ��ς݃��X�g</param>
    /// <param name="beforeList">���O�̃��X�g</param>
    /// <param name="range">�c�����</param>
    /// <returns></returns>
    private List<MoveHistory> GetMovableLocations(CharacterBase chr, List<Vector2Int> checkedList, List<MoveHistory> beforeList, int range)
    {
        var ret = new List<MoveHistory>();
        var nextList = new List<MoveHistory>();

        foreach (var b in beforeList)
        {
            var tmpList = new List<Vector2Int>();

            //
            var up = b.history.Last() + new Vector2Int(0, 1);
            var down = b.history.Last() + new Vector2Int(0, -1);
            var right = b.history.Last() + new Vector2Int(1, 0);
            var left = b.history.Last() + new Vector2Int(-1, 0);

            if (!checkedList.Contains(up) && CanWalkCell(chr, up)) tmpList.Add(up);
            if (!checkedList.Contains(down) && CanWalkCell(chr, down)) tmpList.Add(down);
            if (!checkedList.Contains(right) && CanWalkCell(chr, right)) tmpList.Add(right);
            if (!checkedList.Contains(left) && CanWalkCell(chr, left)) tmpList.Add(left);

            foreach (var tmp in tmpList)
            {
                //
                var h = new MoveHistory();
                h.history.AddRange(b.history);
                h.history.Add(tmp);

                checkedList.Add(tmp);
                nextList.Add(h);
                ret.Add(h);
            }
        }

        if (range > 1) ret.AddRange(GetMovableLocations(chr, checkedList, nextList, range - 1));

        return ret;
    }

    /// <summary>
    /// �i���\������
    /// </summary>
    /// <param name="chr"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool CanWalkCell(CharacterBase chr, Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= COL_COUNT || cell.y < 0 || cell.y >= ROW_COUNT) return false;

        var cellChara = GetCellCharacter(cell);
        if (cellChara == null) return true;
        if (cellChara.IsPlayer() == chr.IsPlayer()) return true;

        return false;
    }

    /// <summary>
    /// �ړ������N���X
    /// </summary>
    public class MoveHistory
    {
        public List<Vector2Int> history;

        public MoveHistory()
        {
            history = new List<Vector2Int>();
        }

        public Vector2Int current { get { return history.Last(); } }
    }

    /// <summary>
    /// �ėp�@�����}�X�擾
    /// </summary>
    /// <param name="center"></param>
    /// <param name="rangeMin"></param>
    /// <param name="rangeMax"></param>
    /// <returns></returns>
    public List<Vector2Int> GetRangeLocations(Vector2Int center, int rangeMin, int rangeMax)
    {
        var ret = new List<Vector2Int>();

        for (var r = rangeMin; r <= rangeMax; ++r)
        {
            for (var y = r; y >= -r; --y)
            {
                var x = r - Math.Abs(y);
                var l = center + new Vector2Int(x, y);
                if (IsInField(l)) ret.Add(l);

                if (x == 0) continue;
                l.x = center.x - x;
                if (IsInField(l)) ret.Add(l);
            }
        }

        return ret;
    }

    #endregion

    #region AI�p

    /// <summary>
    /// �ړ����ĒN���ɍU������
    /// </summary>
    public class AI_AttackTarget
    {
        public CharacterBase target;
        public MoveHistory moveHist;
    }

    /// <summary>
    /// �U���ł���L�����N�^�[����
    /// </summary>
    /// <param name="own"></param>
    /// <param name="moves"></param>
    /// <returns></returns>
    public List<AI_AttackTarget> GetAttackableCharacters(CharacterBase own, List<MoveHistory> moves, int rangeMin = -1, int rangeMax = -1)
    {
        if (rangeMin < 0) rangeMin = own.GetRangeMin();
        if (rangeMax < 0) rangeMax = own.GetRangeMax();

        var ret = new List<AI_AttackTarget>();
        foreach (var move in moves)
        {
            var atkCells = GetRangeLocations(move.current, rangeMin, rangeMax);
            foreach (var cell in atkCells)
            {
                var chr = GetCellCharacter(cell);
                if (chr == null) continue;
                if (own.IsPlayer() == chr.IsPlayer()) continue;
                var oldCheck = ret.Find(a => a.target == chr);
                if (oldCheck != null)
                {
                    // ���łɂ���ꍇ�A���肪�����ł��Ȃ��ʒu��D��
                    if (!AI_CheckInRange(chr, cell) && AI_CheckInRange(chr, oldCheck.moveHist.current))
                        ret.Remove(oldCheck);
                    else
                        continue;
                }

                var ai = new AI_AttackTarget();
                ai.target = chr;
                ai.moveHist = move;
                ret.Add(ai);
            }
        }

        return ret;
    }

    /// <summary>
    /// �˒��͈͓����ǂ���
    /// </summary>
    /// <param name="chr"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool AI_CheckInRange(CharacterBase chr, Vector2Int cell)
    {
        var dist = chr.GetLocation() - cell;
        var d = Math.Abs(dist.x) + Math.Abs(dist.y);
        return d >= chr.GetRangeMin() && d <= chr.GetRangeMax();
    }

    /// <summary>
    /// �^�[�Q�b�g�Ɛ퓬���ʂ̕ێ�
    /// </summary>
    private class AITargetResult
    {
        public AI_AttackTarget tgt;
        public GameParameter.BattleParameter btl;
    }

    /// <summary>
    /// �ǂ�ɍU�����邩�I��
    /// </summary>
    /// <param name="attackable"></param>
    /// <returns></returns>
    public AI_AttackTarget SelectAIAttack(CharacterBase own, List<AI_AttackTarget> attackable)
    {
        if (attackable.Count == 0) return null;

        var niceList = new List<AITargetResult>();   // �D�惊�X�g
        var safeList = new List<AITargetResult>();   // �������_���[�W�����Ȃ����X�g
        var normalList = new List<AITargetResult>(); // ���ʃ��X�g
        var zeroList = new List<AITargetResult>();   // �_���[�W�^�����Ȃ����X�g
        var dangerList = new List<AITargetResult>(); // ����������郊�X�g

        foreach (var ai in attackable)
        {
            // �퓬���ʗ\��
            var result = GameParameter.GetBattleParameter(own, ai.target);
            var aib = new AITargetResult() { tgt = ai, btl = result };

            // �ꔭ�œ|����Ȃ�nice
            if (result.a_dmg >= ai.target.param.HP && result.a_hit > 0) niceList.Add(aib);
            // �����Ŏ����������Ȃ�danger
            else if (result.d_dmg * result.d_atkCount >= own.param.HP && result.d_hit > 0) dangerList.Add(aib);
            // �|�������Ȃ�nice
            else if (result.a_dmg * result.a_atkCount >= ai.target.param.HP && result.a_hit > 0) niceList.Add(aib);
            // �^����_���[�W�[��
            else if (result.a_dmg == 0 || result.a_hit == 0) zeroList.Add(aib);
            // �������󂯂�_���[�W�[��
            else if (result.d_dmg == 0 || result.d_hit == 0) safeList.Add(aib);
            // ����ȊO
            else normalList.Add(aib);
        }

        // �|����̂�����ΑI��
        if (niceList.Count > 0) return niceList[0].tgt;

        // �����������Ȃ����X�g�����ԗ^�_�����傫���̂�I��
        if (safeList.Count > 0)
            return safeList.OrderByDescending(ai => ai.btl.a_dmg).First().tgt;

        // �ʏ탊�X�g�����ԗ^�_�����傫���̂�I��
        if (normalList.Count > 0)
            return normalList.OrderByDescending(ai => ai.btl.a_dmg).First().tgt;

        // �[�����玩���̃_���[�W���������̂�I��
        if (zeroList.Count > 0)
            return zeroList.OrderBy(ai => ai.btl.d_dmg).First().tgt;

        // �����������̂����ԗ^�_�����傫���̂�I��
        if (dangerList.Count > 0)
            return dangerList.OrderByDescending(ai => ai.btl.a_dmg).First().tgt;

        return null;
    }

    #endregion
}

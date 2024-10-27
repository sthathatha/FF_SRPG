using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// �t�B�[���h�Ǘ�
/// </summary>
public class FieldSystem : MonoBehaviour
{
    #region �萔

    public static int ROW_COUNT = 7;
    public static int COL_COUNT = 15;

    private static float CELL_SIZE = 64f;
    private static float ZERO_X = -(COL_COUNT - 1) * CELL_SIZE / 2f;
    private static float ZERO_Y = -(ROW_COUNT - 1) * CELL_SIZE / 2f;

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

    /// <summary>�퓬��</summary>
    private int Prm_BattleCount = 0;
    /// <summary>�퓬���̃^�[����</summary>
    private int Prm_BattleTurn = 0;

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
        InputPos = loc;
        InputLong = false;
        waitingInput = false;
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="pos"></param>
    private void LongClick(Vector2 pos)
    {
        var loc = GetLocationFromPos(pos);
        InputPos = loc;
        InputLong = true;
        waitingInput = false;
    }

    private bool waitingInput = false;
    public Vector2Int InputPos { get; set; }
    public bool InputLong { get; set; }
    public IEnumerator WaitInput()
    {
        waitingInput = true;
        yield return new WaitWhile(() => waitingInput);
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
            var line = Instantiate(colLine_dummy);
            line.SetParent(Line_parent, false);
            line.gameObject.SetActive(true);
            line.localPosition = LinePos(c);
            colLines.Add(line);
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

    #region �t�B�[���h�f�[�^�Ǘ�

    /// <summary>
    /// �t�B�[���h�̏�Ԃ��쐬
    /// </summary>
    public void InitField()
    {
        if (Global.GetTemporaryData().isLoadGame)
        {
            //todo:�Z�[�u�f�[�^����쐬

        }
        else
        {
            Prm_BattleCount = 1;
            Prm_BattleTurn = 0;

            // 6�l������
            GameParameter.Prm_Drows.Init(Constant.PlayerID.Drows);
            GameParameter.Prm_Eraps.Init(Constant.PlayerID.Eraps);
            GameParameter.Prm_Exa.Init(Constant.PlayerID.Exa);
            GameParameter.Prm_Worra.Init(Constant.PlayerID.Worra);
            GameParameter.Prm_Koob.Init(Constant.PlayerID.Koob);
            GameParameter.Prm_You.Init(Constant.PlayerID.You);

            // �h���V�[
            var p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 1));
            p.SetCharacter(Constant.PlayerID.Drows);
            players.Add(p);
            // �G��
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 2));
            p.SetCharacter(Constant.PlayerID.Eraps);
            players.Add(p);
            // �G�O�U
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 3));
            p.SetCharacter(Constant.PlayerID.Exa);
            players.Add(p);
            // �E�[��
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 4));
            p.SetCharacter(Constant.PlayerID.Worra);
            players.Add(p);
            // �N�[
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 5));
            p.SetCharacter(Constant.PlayerID.Koob);
            players.Add(p);
            // �I
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 6));
            p.SetCharacter(Constant.PlayerID.You);
            players.Add(p);

            CreateRandomEnemy();
        }
    }

    /// <summary>
    /// �G�������_������
    /// </summary>
    private void CreateRandomEnemy()
    {
        //todo:�K�w�ɂ���ă��x��
        var lv = Prm_BattleCount + 4;

        // 4�`6�̂̉����G�𐶐�
        var weakCnt = Util.RandomInt(4, 6);
        for (var i = 0; i < weakCnt; i++)
        {
            //todo:�G���GID��I��
            var eid = (Constant.EnemyID)Util.RandomInt(0, 0);

            var e = Instantiate(enemy_dummy, Character_parent, false);
            e.gameObject.SetActive(true);
            e.SetLocation(new Vector2Int(Util.RandomInt(1, 5), i));
            e.SetCharacter(eid);
            e.InitParameter(lv);
            enemies.Add(e);
        }

        // ���G�o��
        if (Prm_BattleCount % 10 == 0)
        {
            // ���G��Lv
            var strongLv = lv >= 100 ? Mathf.FloorToInt(lv * 1.1f) : lv + 10;

            //todo:���GID��I��
            var eid = (Constant.EnemyID)Util.RandomInt(0, 0);

            var e = Instantiate(enemy_dummy, Character_parent, false);
            e.gameObject.SetActive(true);
            e.SetLocation(new Vector2Int(1, ROW_COUNT - 1));
            e.SetCharacter(eid);
            e.InitParameter(strongLv);
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

    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// フィールド管理
/// </summary>
public class FieldSystem : MonoBehaviour
{
    #region 定数

    public static int ROW_COUNT = 7;
    public static int COL_COUNT = 15;

    private static float CELL_SIZE = 64f;
    private static float ZERO_X = -(COL_COUNT - 1) * CELL_SIZE / 2f;
    private static float ZERO_Y = -(ROW_COUNT - 1) * CELL_SIZE / 2f;

    #endregion

    #region メンバー

    public SpriteRenderer tile_dummy;
    /// <summary>プレイヤーダミー</summary>
    public PlayerCharacter player_dummy;
    /// <summary>敵ダミー</summary>
    public EnemyCharacter enemy_dummy;

    /// <summary>縦線ダミー</summary>
    public Transform colLine_dummy;
    /// <summary>枠線親オブジェクト</summary>
    public Transform Line_parent;
    public Transform Character_parent;
    public Transform Tile_parent;

    #endregion

    #region フィールド

    /// <summary>縦線リスト</summary>
    private List<Transform> colLines = new List<Transform>();

    private List<SpriteRenderer> tiles = new List<SpriteRenderer>();

    /// <summary>プレイヤーリスト</summary>
    private List<PlayerCharacter> players = new List<PlayerCharacter>();
    /// <summary>敵リスト</summary>
    private List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    #endregion

    #region 既定

    /// <summary>
    /// 初期化
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

    #region クリック制御

    #region イベント処理
    /// <summary>長押し判定</summary>
    private bool longClicked = false;

    /// <summary>長押し判定コルーチン</summary>
    private IEnumerator longClickCoroutine = null;
    /// <summary>
    /// 長押し判定コルーチン
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
    /// マウス押し込みイベント
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
    /// マウス放すイベント
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
    /// マウス動かすイベント
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
    /// クリックイベント
    /// </summary>
    /// <param name="ev"></param>
    public void EventClick(BaseEventData ev)
    {
        // 長押しした後はクリック処理しない
        if (longClicked) return;

        var pev = ev as PointerEventData;
        var loc = GetLocationFromPos(pev.position);
        InputPos = loc;
        InputLong = false;
        waitingInput = false;
    }

    /// <summary>
    /// 長押し処理
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

    #region 座標管理

    /// <summary>
    /// セルの描画位置
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
    /// 座標からセル位置を取得
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

    #region 枠線管理

    /// <summary>
    /// 枠線の位置
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
    /// 列の枠線を作成
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

    #region タイル制御

    /// <summary>
    /// タイルクリア
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
    /// タイル表示追加
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

    #region フィールドデータ管理

    /// <summary>
    /// フィールドの状態を作成
    /// </summary>
    public void InitField()
    {
        //todo:セーブデータから作成

        var p = Instantiate(player_dummy);
        p.gameObject.SetActive(true);
        p.transform.SetParent(Character_parent, false);
        p.SetLocation(new Vector2Int(10, 2));
        players.Add(p);

        var e = Instantiate(enemy_dummy);
        e.gameObject.SetActive(true);
        e.transform.SetParent(Character_parent, false);
        //e.SetLocation(new Vector2Int(2, 5));
        e.SetLocation(new Vector2Int(8, 2));
        enemies.Add(e);
    }

    /// <summary>
    /// セルに居るキャラクター取得
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
    /// 移動可能場所をリストアップ
    /// </summary>
    /// <param name="chr">キャラクター</param>
    /// <returns></returns>
    public List<MoveHistory> GetMovableLocations(CharacterBase chr)
    {
        var ret = new List<MoveHistory>();

        var checkedList = new List<Vector2Int>();
        var beforeList = new List<MoveHistory>();

        // 0歩の位置
        var tmpHist = new MoveHistory();
        tmpHist.history.Add(chr.GetLocation());
        ret.Add(tmpHist);
        beforeList.Add(tmpHist);
        checkedList.Add(chr.GetLocation());

        //
        var range = 4; //chr.
        ret.AddRange(GetMovableLocations(chr, checkedList, beforeList, range));

        return ret;
    }

    /// <summary>
    /// 移動可能場所のリスト取得
    /// </summary>
    /// <param name="chr">キャラクター</param>
    /// <param name="checkedList">移動済みリスト</param>
    /// <param name="beforeList">直前のリスト</param>
    /// <param name="range">残り歩数</param>
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

    private bool CanWalkCell(CharacterBase chr, Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= COL_COUNT || cell.y < 0 || cell.y >= ROW_COUNT) return false;

        var cellChara = GetCellCharacter(cell);
        if (cellChara == null) return true;
        if (cellChara.IsPlayer() == chr.IsPlayer()) return true;

        return false;
    }

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

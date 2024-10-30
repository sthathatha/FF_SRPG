using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    public Button btn_TurnEnd;

    #endregion

    #region フィールド内容

    /// <summary>縦線リスト</summary>
    private List<Transform> colLines = new List<Transform>();

    /// <summary>タイル表示リスト</summary>
    private List<SpriteRenderer> tiles = new List<SpriteRenderer>();

    /// <summary>プレイヤーリスト</summary>
    private List<PlayerCharacter> players = new List<PlayerCharacter>();
    /// <summary>敵リスト</summary>
    private List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    /// <summary>戦闘回数</summary>
    private int Prm_BattleCount = 0;
    /// <summary>戦闘内のターン数</summary>
    private int Prm_BattleTurn = 0;

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
        if (loc.x < 0 || loc.y < 0) InputResult = InputResultEnum.OutCell;
        else InputResult = InputResultEnum.Cell;

        InputLoc = loc;
        InputLong = false;
        waitingInput = false;
    }

    /// <summary>
    /// 長押し処理
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
    ///  入力結果
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
    /// ターン終了ボタンをクリック
    /// </summary>
    public void TurnEndClick()
    {
        InputResult = InputResultEnum.TurnEnd;
        waitingInput = false;
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

    #region キャラ行動管理

    /// <summary>
    /// 全キャラ行動フラグリセット
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
    /// 行動可能キャラリスト取得
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

    #region フィールドデータ管理

    /// <summary>
    /// フィールドの状態を作成
    /// </summary>
    public void InitField()
    {
        if (Global.GetTemporaryData().isLoadGame)
        {
            //todo:セーブデータから作成

        }
        else
        {
            Prm_BattleCount = 1;
            Prm_BattleTurn = 0;

            // 6人初期化
            GameParameter.Prm_Drows.Init(Constant.PlayerID.Drows);
            GameParameter.Prm_Eraps.Init(Constant.PlayerID.Eraps);
            GameParameter.Prm_Exa.Init(Constant.PlayerID.Exa);
            GameParameter.Prm_Worra.Init(Constant.PlayerID.Worra);
            GameParameter.Prm_Koob.Init(Constant.PlayerID.Koob);
            GameParameter.Prm_You.Init(Constant.PlayerID.You);

            // ドロシー
            var p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 1));
            p.SetCharacter(Constant.PlayerID.Drows);
            players.Add(p);
            // エラ
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 2));
            p.SetCharacter(Constant.PlayerID.Eraps);
            players.Add(p);
            // エグザ
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 3));
            p.SetCharacter(Constant.PlayerID.Exa);
            players.Add(p);
            // ウーラ
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 4));
            p.SetCharacter(Constant.PlayerID.Worra);
            players.Add(p);
            // クー
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 5));
            p.SetCharacter(Constant.PlayerID.Koob);
            players.Add(p);
            // 悠
            p = Instantiate(player_dummy, Character_parent, false);
            p.gameObject.SetActive(true);
            p.SetLocation(new Vector2Int(COL_COUNT - 1, ROW_COUNT - 6));
            p.SetCharacter(Constant.PlayerID.You);
            players.Add(p);

            // 所持品等初期化
            GameParameter.otherData.Init();

            CreateRandomEnemy();
        }
    }

    /// <summary>
    /// 敵をランダム生成
    /// </summary>
    private void CreateRandomEnemy()
    {
        //todo:階層によってレベル
        var lv = Prm_BattleCount + 4;

        // 4〜6体の下級敵を生成
        var weakCnt = Util.RandomInt(4, 6);
        for (var i = 0; i < weakCnt; i++)
        {
            //todo:雑魚敵IDを選択
            var eid = (Constant.EnemyID)Util.RandomInt(0, 0);

            var e = Instantiate(enemy_dummy, Character_parent, false);
            e.gameObject.SetActive(true);
            e.SetLocation(new Vector2Int(Util.RandomInt(1, 5), i));
            e.SetCharacter(eid);
            e.InitParameter(lv);
            enemies.Add(e);
        }

        // 強敵出現
        if (Prm_BattleCount % 10 == 0)
        {
            // 強敵のLv
            var strongLv = lv >= 100 ? Mathf.FloorToInt(lv * 1.1f) : lv + 10;

            //todo:強敵IDを選択
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

    #endregion

    #region 位置検索

    /// <summary>
    /// フィールド範囲内判定
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    public bool IsInField(Vector2Int loc)
    {
        return loc.x >= 0 && loc.y >= 0 && loc.x < COL_COUNT && loc.y < ROW_COUNT;
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

        // 移動歩数で取得
        var range = chr.param.Move;
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

    /// <summary>
    /// 進入可能か判定
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
    /// 移動履歴クラス
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
    /// 汎用　距離マス取得
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
}

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
/// フィールド管理
/// </summary>
public class FieldSystem : MonoBehaviour
{
    #region 定数

    public const int ROW_COUNT = 7;
    public const int COL_COUNT = 15;

    private const float CELL_SIZE = 64f;
    private const float ZERO_X = -(COL_COUNT - 1) * CELL_SIZE / 2f;
    private const float ZERO_Y = -(ROW_COUNT - 1) * CELL_SIZE / 2f;

    private const float MOVE_X = (COL_COUNT - 1) * CELL_SIZE;

    /// <summary>BG画像右側のX</summary>
    private const float BG_RIGHT_X = CELL_SIZE / 2f;
    /// <summary>BG画像左側のX</summary>
    private const float BG_LEFT_X = BG_RIGHT_X - MOVE_X;

    /// <summary>撤退した時復帰するフロア数</summary>
    private const int RETURN_FLOOR_ESCAPE = 4;
    /// <summary>HP0の時復帰するフロア数</summary>
    private const int RETURN_FLOOR_DEATH = 5;

    #endregion

    #region メンバー

    public SpriteRenderer tile_dummy;
    /// <summary>プレイヤーダミー</summary>
    public PlayerCharacter player_dummy;
    /// <summary>敵ダミー</summary>
    public EnemyCharacter enemy_dummy;

    /// <summary>BG生成親</summary>
    public Transform bg_parent;
    /// <summary>BGダミー</summary>
    public SpriteRenderer bg_dummy;

    /// <summary>通常背景Sprite</summary>
    public Sprite bg_normal;
    /// <summary>ボス背景Sprite</summary>
    public Sprite bg_boss;

    /// <summary>縦線ダミー</summary>
    public Transform colLine_dummy;
    /// <summary>枠線親オブジェクト</summary>
    public Transform Line_parent;
    public Transform Character_parent;
    public Transform Tile_parent;

    /// <summary>ターン終了ボタン</summary>
    public Button btn_TurnEnd;

    /// <summary>フロア表示</summary>
    public TMP_Text floorDisplay;

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

    private int _battleFloor = 0;
    /// <summary>戦闘回数</summary>
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
    /// <summary>戦闘内のターン数</summary>
    public int Prm_BattleTurn { get; set; } = 0;

    /// <summary>ランキング登録可能フラグ</summary>
    public bool Prm_EnableRanking { get; set; } = true;

    /// <summary>ボスフェーズ　0～9まで固定</summary>
    private int bossPhase = 0;

    /// <summary>BG右側</summary>
    private SpriteRenderer bg_R = null;
    /// <summary>BG左側</summary>
    private SpriteRenderer bg_L = null;

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
        bg_dummy.gameObject.SetActive(false);

        InitLines();
    }

    #endregion

    #region セーブ処理

    private const string SAVE_PLAYERDATA = "pChrData";
    private const string SAVE_ENEMYDATA = "eChrData";
    private const string SAVE_TURN = "turnNum";
    private const string SAVE_FLOOR = "floorNum";

    private const string SAVE_ENABLERANKING = "enbRank";
    private const string SAVE_LOADDISABLE = "loadDisable";
    private const string SAVE_BOSSPHASE = "bossPhase";

    /// <summary>
    /// セーブ
    /// </summary>
    public void SaveField()
    {
        // 直接起動時セーブしない
        if (ManagerSceneScript.isDebugLoad) return;

        // 1Fではセーブしない
        if (Prm_BattleFloor == 1) return;

        var save = Global.GetSaveData();

        save.SetGameData(SAVE_PLAYERDATA, string.Join('P', players.Select(p => p.ToSaveString())));
        save.SetGameData(SAVE_ENEMYDATA, string.Join('E', enemies.Select(e => e.ToSaveString())));
        save.SetGameData(SAVE_FLOOR, Prm_BattleFloor);
        save.SetGameData(SAVE_TURN, Prm_BattleTurn);
        save.SetGameData(SAVE_ENABLERANKING, Prm_EnableRanking ? 1 : 0);
        save.SetGameData(SAVE_LOADDISABLE, 0);
        save.SetGameData(SAVE_BOSSPHASE, bossPhase);

        GameParameter.Save();

        save.SaveGameData();
    }

    /// <summary>
    /// ロード禁止フラグ
    /// SaveFieldにてフラグを下ろし、プレイヤー行動選択時に立てる
    /// たった状態でロードするとランキング登録不可になる
    /// </summary>
    public void LoadDisableSet()
    {
        // すでに登録不可になっていたら負荷軽減のため無視
        if (!Prm_EnableRanking) return;

        // 1Fではセーブなし
        if (Prm_BattleFloor == 1) return;

        var save = Global.GetSaveData();
        save.SetGameData(SAVE_LOADDISABLE, 1);
        save.SaveGameData();
    }

    /// <summary>
    /// ロード
    /// </summary>
    public void LoadField()
    {
        var save = Global.GetSaveData();
        save.LoadGameData();

        GameParameter.Load();
        Prm_EnableRanking = save.GetGameDataInt(SAVE_ENABLERANKING) == 1;
        var ldCnt = save.GetGameDataInt(SAVE_LOADDISABLE);
        if (ldCnt > 0) Prm_EnableRanking = false;
        save.SaveGameData();

        Prm_BattleFloor = save.GetGameDataInt(SAVE_FLOOR);
        Prm_BattleTurn = save.GetGameDataInt(SAVE_TURN);
        bossPhase = save.GetGameDataInt(SAVE_BOSSPHASE);
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
            var line = Instantiate(colLine_dummy, Line_parent, false);
            line.gameObject.SetActive(true);
            line.localPosition = LinePos(c);
            colLines.Add(line);
        }
    }

    /// <summary>
    /// 次のフロアの線を生成
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
    /// 全部の縦線の座標を指定
    /// </summary>
    /// <param name="headLineIdx">戦闘の列</param>
    /// <param name="addX">加算X</param>
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
    /// 古い枠線を削除
    /// </summary>
    private void DeleteOldLine()
    {
        var limitX = LinePos(COL_COUNT - 1).x + 1f;

        for (var idx = colLines.Count - 1; idx >= 0; idx--)
        {
            // 右端より左のが見つかったら終了
            if (colLines[idx].localPosition.x < limitX) break;

            Destroy(colLines[idx].gameObject);
            colLines.RemoveAt(idx);
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
            // セーブデータから作成
            LoadField();
        }
        else
        {
            Prm_BattleFloor = 1;
            Prm_BattleTurn = 0;
            Prm_EnableRanking = true;
            bossPhase = 0;

            // 6人初期化
            GameParameter.Prm_Drows.Init(Constant.PlayerID.Drows);
            GameParameter.Prm_Eraps.Init(Constant.PlayerID.Eraps);
            GameParameter.Prm_Exa.Init(Constant.PlayerID.Exa);
            GameParameter.Prm_Worra.Init(Constant.PlayerID.Worra);
            GameParameter.Prm_Koob.Init(Constant.PlayerID.Koob);
            GameParameter.Prm_You.Init(Constant.PlayerID.You);

            // ドロシー
            CreatePlayer(Constant.PlayerID.Drows, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 1));
            // エラ
            CreatePlayer(Constant.PlayerID.Eraps, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 2));
            // エグザ
            CreatePlayer(Constant.PlayerID.Exa, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 3));
            // ウーラ
            CreatePlayer(Constant.PlayerID.Worra, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 4));
            // クー
            CreatePlayer(Constant.PlayerID.Koob, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 5));
            // 悠
            CreatePlayer(Constant.PlayerID.You, new Vector2Int(COL_COUNT - 1, ROW_COUNT - 6));

            // 初期スキル
            foreach (var p in players) p.CheckGetSkill();

            // 所持品等初期化
            GameParameter.otherData.Init();

            CreateRandomEnemy(false);
        }

        InitBG();
    }

    /// <summary>
    /// 敵をランダム生成
    /// </summary>
    /// <param name="next">true:次のフロア分生成（X座標マイナス） false:今の座標で生成</param>
    private void CreateRandomEnemy(bool next = true)
    {
        var addX = next ? -COL_COUNT + 1 : 0;

        // 2フロアごとに敵レベル１アップ
        var lv = Prm_BattleFloor / 2 + 1;
        var randMax = Mathf.CeilToInt(lv / 15);

        // ドロップアイテムを確実に持っているやつ
        var dropIndex = -1;
        var haveWeaponList = GameParameter.otherData.haveItemList.Where(itm =>
                                    itm.ItemData.iType != GameDatabase.ItemType.None &&
                                    itm.ItemData.iType != GameDatabase.ItemType.Item &&
                                    itm.ItemData.iType != GameDatabase.ItemType.Rod).ToList();
        var weaponTotalRest = haveWeaponList.Sum(itm => itm.useCount);
        var weaponTypeCount = haveWeaponList.Select(itm => itm.ItemData.iType).Distinct().Count();
        // 袋の武器の合計がx回未満、または武器がy種類なら確実に武器ドロップ
        if (weaponTotalRest < 10 || weaponTypeCount <= 1) dropIndex = Util.RandomInt(0, 3);

        // 4～6体の下級敵を生成
        var weakCnt = Util.RandomInt(4, 6);
        for (var i = 0; i < weakCnt; i++)
        {
            // 雑魚敵IDを選択
            var eid = GameDatabase.CalcRandomEnemy(Prm_BattleFloor, false);

            var e = Instantiate(enemy_dummy, Character_parent, false);
            e.gameObject.SetActive(true);
            e.SetLocation(new Vector2Int(Util.RandomInt(1, 5) + addX, i));
            e.SetCharacter(eid);
            e.InitParameter(lv + Util.RandomInt(0, randMax));
            if (dropIndex == i)
                e.SetWeaponAndDrop(drp: GameDatabase.CalcRandomItem(Prm_BattleFloor, true));
            else
                e.SetWeaponAndDrop();
            enemies.Add(e);
        }

        // 強敵出現
        if (Prm_BattleFloor % 10 == 0)
        {
            // 強敵のLv
            var strongLv = lv >= 100 ? Mathf.FloorToInt(lv * 1.1f) : (lv + 10);

            // ボスID
            var bossID = Constant.EnemyID.ENEMY_COUNT;
            var bossWpn = GameDatabase.ItemID.FreeHand;
            var bossDrop = GameDatabase.ItemID.FreeHand;
            #region 最初10体は固定、倒したら進む
            switch (bossPhase)
            {
                case 0:
                    bossID = Constant.EnemyID.Angel;
                    bossWpn = GameDatabase.ItemID.Arrow2;
                    bossDrop = GameDatabase.ItemID.Arrow3;
                    break;
                case 1:
                    bossID = Constant.EnemyID.ArchAngel;
                    bossWpn = GameDatabase.ItemID.Sword2;
                    bossDrop = GameDatabase.ItemID.Sword3;
                    break;
                case 2:
                    bossID = Constant.EnemyID.Principality;
                    bossWpn = GameDatabase.ItemID.Spear2;
                    bossDrop = GameDatabase.ItemID.Spear3;
                    break;
                case 3:
                    bossID = Constant.EnemyID.Power;
                    bossWpn = GameDatabase.ItemID.Axe2;
                    bossDrop = GameDatabase.ItemID.Axe3;
                    break;
                case 4:
                    bossID = Constant.EnemyID.Virtue;
                    bossWpn = GameDatabase.ItemID.Arrow_Ex1;
                    bossDrop = GameDatabase.ItemID.Arrow_Ex1;
                    break;
                case 5:
                    bossID = Constant.EnemyID.Dominion;
                    bossWpn = GameDatabase.ItemID.Spear_Ex1;
                    bossDrop = GameDatabase.ItemID.Spear_Ex1;
                    break;
                case 6:
                    bossID = Constant.EnemyID.Throne;
                    bossWpn = GameDatabase.ItemID.Axe_Ex1;
                    bossDrop = GameDatabase.ItemID.Axe_Ex1;
                    break;
                case 7:
                    bossID = Constant.EnemyID.Cherubim;
                    bossWpn = GameDatabase.ItemID.Book_Ex1;
                    bossDrop = GameDatabase.ItemID.Book_Ex1;
                    break;
                case 8:
                    bossID = Constant.EnemyID.Seraphim;
                    bossWpn = GameDatabase.ItemID.Sword_Ex1;
                    bossDrop = GameDatabase.ItemID.Sword_Ex1;
                    break;
                case 9:
                    bossID = Constant.EnemyID.Prime;
                    bossWpn = GameDatabase.ItemID.Book_Ex2;
                    bossDrop = GameDatabase.ItemID.Book_Ex2;
                    break;
                default:
                    bossID = GameDatabase.CalcRandomEnemy(Prm_BattleFloor, true);
                    bossWpn = GameDatabase.ItemID.FreeHand;
                    bossDrop = GameDatabase.CalcRandomItem(strongLv, true, false);
                    break;
            }
            #endregion

            var e = Instantiate(enemy_dummy, Character_parent, false);
            e.gameObject.SetActive(true);
            e.SetLocation(new Vector2Int(1 + addX, ROW_COUNT - 1));
            e.SetCharacter(bossID, true);
            e.InitParameter(strongLv);
            e.SetWeaponAndDrop(bossWpn, bossDrop);
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

    /// <summary>
    /// キャラ死亡処理
    /// </summary>
    /// <param name="chr"></param>
    /// <param name="death">true:死亡による削除　false:撤退による削除</param>
    public void DeleteCharacter(CharacterBase chr, bool death = true)
    {
        if (chr.IsPlayer())
        {
            var pc = chr as PlayerCharacter;
            players.Remove(pc);

            // 復活時間を設定
            GameParameter.Prm_Get(pc.playerID).RestBattle = death ? RETURN_FLOOR_DEATH : RETURN_FLOOR_ESCAPE;
        }
        else
        {
            var ec = chr as EnemyCharacter;
            if (death && ec.isBoss)
            {
                // ボスを倒したらフェーズ進む
                bossPhase++;
            }

            enemies.Remove(chr as EnemyCharacter);
        }

        Destroy(chr.gameObject);
    }

    /// <summary>
    /// プレイヤーキャラ生成
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
    /// プレイヤーキャラ作成（セーブデータから）
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
    /// 敵キャラクター作成（セーブデータから）
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
    /// プレイヤーリスト取得
    /// </summary>
    /// <returns></returns>
    public List<PlayerCharacter> GetPlayers() { return players; }

    /// <summary>
    /// 敵リスト取得
    /// </summary>
    /// <returns></returns>
    public List<EnemyCharacter> GetEnemies() { return enemies; }

    /// <summary>
    /// 次のフロアに進む
    /// </summary>
    /// <returns></returns>
    public IEnumerator NextFloor()
    {
        const float MOVE_TIME = 1f;
        // 次のフロアの敵を生成
        Prm_BattleFloor++;
        Prm_BattleTurn = 0;
        CreateRandomEnemy(true);

        // 新しい縦線を生成
        CreateNextLine();

        // 新しいBGを生成
        var bgX_Next = BG_LEFT_X - MOVE_X;
        var nextBG = Instantiate(bg_dummy, bg_parent, false);
        nextBG.transform.localPosition = new Vector3(bgX_Next, 0f);
        nextBG.sprite = Prm_BattleFloor % 10 == 9 ? bg_boss : bg_normal;
        nextBG.gameObject.SetActive(true);

        // キャラと縦線をCOLCOUNT-1個分動かす
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

            nextBG.transform.localPosition = new Vector3(bgX_Next + addX.Get(), 0f);
            bg_R.transform.localPosition = new Vector3(BG_RIGHT_X + addX.Get(), 0f);
            bg_L.transform.localPosition = new Vector3(BG_LEFT_X + addX.Get(), 0f);
        }

        // 前のフロアの敵と縦線を消す
        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].GetLocation().x > 0) DeleteCharacter(enemies[i], false);
        }
        for (var i = players.Count - 1; i >= 0; i--)
        {
            // 本来右には残ってないはずだが一応プレイヤーも
            if (players[i].GetLocation().x > 0) DeleteCharacter(players[i], false);
        }

        // 座標を修正
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

        // 前のBGを消して変数つけかえ
        Destroy(bg_R);
        bg_R = bg_L;
        bg_L = nextBG;

        yield return ReturnPlayerCoroutine();

        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 現在のフロアのBG表示
    /// </summary>
    private void InitBG()
    {
        if (bg_R != null) Destroy(bg_R);
        if (bg_L != null) Destroy(bg_L);

        bg_R = Instantiate(bg_dummy, bg_parent, false);
        bg_L = Instantiate(bg_dummy, bg_parent, false);
        bg_R.transform.localPosition = new Vector3(BG_RIGHT_X, 0f);
        bg_L.transform.localPosition = new Vector3(BG_LEFT_X, 0f);
        bg_R.sprite = Prm_BattleFloor % 10 == 0 ? bg_boss : bg_normal;
        bg_L.sprite = Prm_BattleFloor % 10 == 9 ? bg_boss : bg_normal;
        bg_R.gameObject.SetActive(true);
        bg_L.gameObject.SetActive(true);
    }

    /// <summary>
    /// 撤退プレイヤーの復帰処理
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
                    // 復帰
                    var l = getEmptyCell();
                    var l2 = l + new Vector2Int(1, 0);
                    var p = CreatePlayer(id, l2);
                    StartCoroutine(returnCharacterAnim(p, l));
                    created = true;
                }
            }
        });

        // ６人チェック
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
    /// プレイヤー復帰アニメーション
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="l">戻ったあとの位置</param>
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
        if (chr.HasSkill(GameDatabase.SkillID.Eraps_FirstSpeed) && Prm_BattleTurn == 1)
            range += 4;
        if (chr.HasSkill(GameDatabase.SkillID.Worra_FastMove))
            range += 1;

        // 敵がもう居ない場合は歩数大増量
        if (enemies.Count == 0)
            range += 10;

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
    /// <param name="isFirst">最初の１歩</param>
    /// <returns></returns>
    private List<MoveHistory> GetMovableLocations(CharacterBase chr, List<Vector2Int> checkedList, List<MoveHistory> beforeList, int range, bool isFirst = true)
    {
        var ret = new List<MoveHistory>();
        var nextList = new List<MoveHistory>();

        foreach (var b in beforeList)
        {
            if (!isFirst)
            {
                // ZOCスキル持ちが居たらここからは広がらない
                var searchZoc = GetRangeLocations(b.current, 1, 1);
                if (searchZoc.Any(s =>
                {
                    var c = GetCellCharacter(s);
                    if (c == null) return false;
                    if (c.IsPlayer() && chr.IsPlayer()) return false;
                    return c.HasSkill(GameDatabase.SkillID.Eraps_ZOC);
                })) continue;
            }

            var tmpList = new List<Vector2Int>();

            // 上下左右に
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

        if (range > 1)
        {
            ret.AddRange(GetMovableLocations(chr, checkedList, nextList, range - 1, false));
        }

        return ret;
    }

    /// <summary>
    /// 攻撃可能場所のリスト
    /// </summary>
    /// <param name="chr"></param>
    /// <param name="moveList"></param>
    /// <returns></returns>
    public List<Vector2Int> GetAttackableLocations(CharacterBase chr, List<Vector2Int> moveList)
    {
        var ret = new List<Vector2Int>();
        var rangeMin = chr.GetRangeMin();
        var rangeMax = chr.GetRangeMax();

        foreach (var move in moveList)
        {
            var checkChr = GetCellCharacter(move);
            if (checkChr != null && checkChr != chr) continue;

            ret.AddRange(GetRangeLocations(move, rangeMin, rangeMax));
        }
        return ret.Distinct().ToList();
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

    /// <summary>
    /// 汎用　距離内のキャラ取得
    /// </summary>
    /// <param name="center"></param>
    /// <param name="rangeMin"></param>
    /// <param name="rangeMax"></param>
    /// <returns></returns>
    public List<CharacterBase> GetRangeCharacters(Vector2Int center, int rangeMin, int rangeMax)
    {
        var ret = new List<CharacterBase>();
        var locs = GetRangeLocations(center, rangeMin, rangeMax);
        foreach (var loc in locs)
        {
            var chr = GetCellCharacter(loc);
            if (chr != null) ret.Add(chr);
        }
        return ret;
    }

    #endregion

    #region AI用

    /// <summary>
    /// 移動して誰かに攻撃する
    /// </summary>
    public class AI_AttackTarget
    {
        public CharacterBase target;
        public MoveHistory moveHist;
    }

    /// <summary>
    /// 攻撃できるキャラクター検索
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
            if (move.current != own.GetLocation())
            {
                // 自分以外の誰かが居たらそこからの攻撃はできない
                if (GetCellCharacter(move.current) != null) continue;
            }

            var atkCells = GetRangeLocations(move.current, rangeMin, rangeMax);
            foreach (var cell in atkCells)
            {
                var chr = GetCellCharacter(cell);
                if (chr == null) continue;
                if (own.IsPlayer() == chr.IsPlayer()) continue;
                var oldCheck = ret.Find(a => a.target == chr);
                if (oldCheck != null)
                {
                    // すでにある場合、相手が反撃できない位置を優先
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
    /// 射程範囲内かどうか
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
    /// ターゲットと戦闘結果の保持
    /// </summary>
    public class AITargetResult
    {
        public AI_AttackTarget tgt;
        public GameParameter.BattleParameter btl;
    }

    /// <summary>
    /// どれに攻撃するか選択
    /// </summary>
    /// <param name="attackable"></param>
    /// <returns></returns>
    public AITargetResult SelectAIAttack(CharacterBase own, List<AI_AttackTarget> attackable)
    {
        if (attackable.Count == 0) return null;

        var niceList = new List<AITargetResult>();   // 優先リスト
        var safeList = new List<AITargetResult>();   // 自分がダメージうけないリスト
        var normalList = new List<AITargetResult>(); // 普通リスト
        var zeroList = new List<AITargetResult>();   // ダメージ与えられないリスト
        var dangerList = new List<AITargetResult>(); // 自分がやられるリスト

        foreach (var ai in attackable)
        {
            // 戦闘結果予測
            var result = GameParameter.GetBattleParameter(own, ai.target, ai.moveHist.current);
            var aib = new AITargetResult() { tgt = ai, btl = result };

            // 一発で倒せるならnice
            if (result.a_dmg >= ai.target.param.HP && result.a_hit > 0) niceList.Add(aib);
            // 反撃で自分がやられるならdanger
            else if (result.d_dmg * result.d_atkCount >= own.param.HP && result.d_hit > 0) dangerList.Add(aib);
            // 倒しきれるならnice
            else if (result.a_dmg * result.a_atkCount >= ai.target.param.HP && result.a_hit > 0) niceList.Add(aib);
            // 与えるダメージゼロ
            else if (result.a_dmg == 0 || result.a_hit == 0) zeroList.Add(aib);
            // 自分が受けるダメージゼロ
            else if (result.d_dmg == 0 || result.d_hit == 0) safeList.Add(aib);
            // それ以外
            else normalList.Add(aib);
        }

        // 倒せるのがあれば選択
        if (niceList.Count > 0) return niceList[0];

        // 自分がくらわないリストから一番与ダメが大きいのを選択
        if (safeList.Count > 0)
            return safeList.OrderByDescending(ai => ai.btl.a_dmg).First();

        // 通常リストから一番与ダメが大きいのを選択
        if (normalList.Count > 0)
            return normalList.OrderByDescending(ai => ai.btl.a_dmg).First();

        // ゼロから自分のダメージが小さいのを選択
        if (zeroList.Count > 0)
            return zeroList.OrderBy(ai => ai.btl.d_dmg).First();

        // 自分がやられるのから一番与ダメが大きいのを選択
        if (dangerList.Count > 0)
            return dangerList.OrderByDescending(ai => ai.btl.a_dmg).First();

        return null;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Global
{
    #region 定数

    /// <summary>項目ごとの区切り</summary>
    public const string SEP_GAMEDATA_ITEM = "&";
    /// <summary>タイトルと内容の区切り</summary>
    public const string SEP_GAMEDATA_TITLE = "=";

    #endregion

    #region インスタンス取得
    private static SaveData _saveData = null;
    /// <summary>
    /// セーブデータ
    /// </summary>
    /// <returns></returns>
    public static SaveData GetSaveData()
    {
        if (_saveData == null)
        {
            _saveData = new SaveData();
        }
        return _saveData;
    }

    private static TemporaryData _temporaryData = null;
    /// <summary>
    /// 一時データ
    /// </summary>
    /// <returns></returns>
    public static TemporaryData GetTemporaryData()
    {
        if (_temporaryData == null)
        {
            _temporaryData = new TemporaryData();
        }
        return _temporaryData;
    }
    #endregion

    /// <summary>
    /// セーブデータ
    /// </summary>
    public class SaveData
    {
        /// <summary>ゲーム内のデータ</summary>
        public Dictionary<string, string> gameData;

        /// <summary>システムデータ</summary>
        public SystemData system;

        /// <summary>
        /// システムデータ
        /// </summary>
        public struct SystemData
        {
            /// <summary>BGM</summary>
            public int bgmVolume;
            /// <summary>SE</summary>
            public int seVolume;
            ///// <summary>ボイス</summary>
            //public int voiceVolume;

            ///// <summary>クリアフラグ</summary>
            //public int clearFlag;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SaveData()
        {
            gameData = new Dictionary<string, string>();

            system.bgmVolume = 3;
            system.seVolume = 3;
            //system.voiceVolume = 3;
            //system.clearFlag = 0;
        }

        #region ゲームデータ

        /// <summary>
        /// ゲームデータをセーブ
        /// </summary>
        public void SaveGameData()
        {
            var serial = ToSaveString(gameData);
            PlayerPrefs.SetString("gameData", serial);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// ゲームデータをロード
        /// </summary>
        public void LoadGameData()
        {
            ReadSaveString(PlayerPrefs.GetString("gameData"));
        }

        /// <summary>
        /// ゲームデータ初期化
        /// </summary>
        public void InitGameData()
        {
            gameData.Clear();
        }

        /// <summary>
        /// データ削除
        /// </summary>
        public void DeleteGameData()
        {
            PlayerPrefs.DeleteKey("gameData");
            PlayerPrefs.Save();
            InitGameData();
        }

        /// <summary>
        /// セーブがあるかどうか
        /// </summary>
        /// <returns></returns>
        public bool IsEnableGameData()
        {
            return PlayerPrefs.HasKey("gameData");
        }

        /// <summary>
        /// ゲームデータセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetGameData(string key, string value)
        {
            gameData[key] = value;
        }

        /// <summary>
        /// ゲームデータセット整数版
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetGameData(string key, int value)
        {
            SetGameData(key, value.ToString());
        }

        /// <summary>
        /// ゲームデータ文字列取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetGameDataString(string key)
        {
            return gameData.ContainsKey(key) ? gameData[key] : "";
        }

        /// <summary>
        /// ゲームデータを整数で取得
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public int GetGameDataInt(string key, int def = 0)
        {
            int ret;
            if (int.TryParse(GetGameDataString(key), out ret))
            {
                return ret;
            }
            else
            {
                return def;
            }
        }

        #endregion

        #region システムデータ

        /// <summary>
        /// システムデータをセーブ
        /// </summary>
        public void SaveSystemData()
        {
            PlayerPrefs.SetInt("optionBgmVolume", system.bgmVolume);
            PlayerPrefs.SetInt("optionSeVolume", system.seVolume);
            //PlayerPrefs.SetInt("optionVoiceVolume", system.voiceVolume);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// システムデータをロード
        /// </summary>
        public void LoadSystemData()
        {
            system.bgmVolume = PlayerPrefs.GetInt("optionBgmVolume", 3);
            system.seVolume = PlayerPrefs.GetInt("optionSeVolume", 3);
            //system.voiceVolume = PlayerPrefs.GetInt("optionVoiceVolume", 3);
        }

        /// <summary>
        /// セーブ用文字列に変換
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ToSaveString(Dictionary<string, string> data)
        {
            var strList = data.Select((pair, idx) => pair.Key + SEP_GAMEDATA_TITLE + pair.Value);

            return string.Join(SEP_GAMEDATA_ITEM, strList);
        }

        /// <summary>
        /// セーブ用文字列を読み込み
        /// </summary>
        /// <param name="data"></param>
        private void ReadSaveString(string data)
        {
            gameData.Clear();
            if (string.IsNullOrEmpty(data)) return;

            foreach (var str in data.Split(SEP_GAMEDATA_ITEM))
            {
                var pair = str.Split(SEP_GAMEDATA_TITLE);
                gameData[pair[0]] = pair[1];
            }
        }

        #endregion
    }

    /// <summary>
    /// 保存しないデータ
    /// </summary>
    public class TemporaryData
    {
        public int dummy;

        public bool isLoadGame;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TemporaryData()
        {
            dummy = 0;
            isLoadGame = false;
        }
    }
}

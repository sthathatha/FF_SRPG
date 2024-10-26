using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Global
{
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

            //todo:テスト用
            //gameData[F101System.PLANT_FLG] = "3";
            //gameData[F111System.BRIDGE_FLG] = "2";
            //gameData[F121System.KEY_FLG] = "3";
            //gameData[F131System.ICE_BLOCK_FLG] = "1";
            //gameData[F131System.ICE_YOU_FLG] = "3";
            //gameData[F141System.CLEAR_FLG] = "1";
            //gameData[F151BoardSource.BOARD_USE_FLG] = "1";
            //gameData[F122System.F122_PIERRE_PHASE] = "2";
            //gameData[F143System.MENDERU_WIN_FLG] = "1";
            //gameData[F153System.AMI_WIN_FLG] = "1";
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
            var strList = data.Select((pair, idx) => pair.Key + ":" + pair.Value);

            return string.Join(',', strList);
        }

        /// <summary>
        /// セーブ用文字列を読み込み
        /// </summary>
        /// <param name="data"></param>
        private void ReadSaveString(string data)
        {
            gameData.Clear();
            if (string.IsNullOrEmpty(data)) return;

            foreach (var str in data.Split(','))
            {
                var pair = str.Split(':');
                gameData[pair[0]] = pair[1];
            }
        }
    }

    /// <summary>
    /// 保存しないデータ
    /// </summary>
    public class TemporaryData
    {
        public int dummy;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TemporaryData()
        {
            dummy = 0;
        }
    }
}

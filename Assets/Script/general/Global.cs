using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Global
{
    #region �萔

    /// <summary>���ڂ��Ƃ̋�؂�</summary>
    public const string SEP_GAMEDATA_ITEM = "&";
    /// <summary>�^�C�g���Ɠ��e�̋�؂�</summary>
    public const string SEP_GAMEDATA_TITLE = "=";

    #endregion

    #region �C���X�^���X�擾
    private static SaveData _saveData = null;
    /// <summary>
    /// �Z�[�u�f�[�^
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
    /// �ꎞ�f�[�^
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
    /// �Z�[�u�f�[�^
    /// </summary>
    public class SaveData
    {
        /// <summary>�Q�[�����̃f�[�^</summary>
        public Dictionary<string, string> gameData;

        /// <summary>�V�X�e���f�[�^</summary>
        public SystemData system;

        /// <summary>
        /// �V�X�e���f�[�^
        /// </summary>
        public struct SystemData
        {
            /// <summary>BGM</summary>
            public int bgmVolume;
            /// <summary>SE</summary>
            public int seVolume;
            ///// <summary>�{�C�X</summary>
            //public int voiceVolume;

            ///// <summary>�N���A�t���O</summary>
            //public int clearFlag;
        }

        /// <summary>
        /// �R���X�g���N�^
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
        /// �Q�[���f�[�^���Z�[�u
        /// </summary>
        public void SaveGameData()
        {
            var serial = ToSaveString(gameData);
            PlayerPrefs.SetString("gameData", serial);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// �Q�[���f�[�^�����[�h
        /// </summary>
        public void LoadGameData()
        {
            ReadSaveString(PlayerPrefs.GetString("gameData"));
        }

        /// <summary>
        /// �Q�[���f�[�^������
        /// </summary>
        public void InitGameData()
        {
            gameData.Clear();

            //todo:�e�X�g�p
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
        /// �Z�[�u�����邩�ǂ���
        /// </summary>
        /// <returns></returns>
        public bool IsEnableGameData()
        {
            return PlayerPrefs.HasKey("gameData");
        }

        /// <summary>
        /// �Q�[���f�[�^�Z�b�g
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetGameData(string key, string value)
        {
            gameData[key] = value;
        }

        /// <summary>
        /// �Q�[���f�[�^�Z�b�g������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetGameData(string key, int value)
        {
            SetGameData(key, value.ToString());
        }

        /// <summary>
        /// �Q�[���f�[�^������擾
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetGameDataString(string key)
        {
            return gameData.ContainsKey(key) ? gameData[key] : "";
        }

        /// <summary>
        /// �Q�[���f�[�^�𐮐��Ŏ擾
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
        /// �V�X�e���f�[�^���Z�[�u
        /// </summary>
        public void SaveSystemData()
        {
            PlayerPrefs.SetInt("optionBgmVolume", system.bgmVolume);
            PlayerPrefs.SetInt("optionSeVolume", system.seVolume);
            //PlayerPrefs.SetInt("optionVoiceVolume", system.voiceVolume);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// �V�X�e���f�[�^�����[�h
        /// </summary>
        public void LoadSystemData()
        {
            system.bgmVolume = PlayerPrefs.GetInt("optionBgmVolume", 3);
            system.seVolume = PlayerPrefs.GetInt("optionSeVolume", 3);
            //system.voiceVolume = PlayerPrefs.GetInt("optionVoiceVolume", 3);
        }

        /// <summary>
        /// �Z�[�u�p������ɕϊ�
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ToSaveString(Dictionary<string, string> data)
        {
            var strList = data.Select((pair, idx) => pair.Key + SEP_GAMEDATA_TITLE + pair.Value);

            return string.Join(SEP_GAMEDATA_ITEM, strList);
        }

        /// <summary>
        /// �Z�[�u�p�������ǂݍ���
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
    }

    /// <summary>
    /// �ۑ����Ȃ��f�[�^
    /// </summary>
    public class TemporaryData
    {
        public int dummy;

        public bool isLoadGame;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public TemporaryData()
        {
            dummy = 0;
            isLoadGame = false;
        }
    }
}
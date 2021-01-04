using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class DataManager
    {

        public const string DataSaveKey = "DataSaveKey";

        public UserData UserData { private set; get; }

        public DataManager()
        {
            Load();
        }

        public void Save()
        {
            PlayerPrefs.SetString(DataSaveKey, JsonUtility.ToJson(UserData));
        }

        private void Load()
        {
            var json = PlayerPrefs.GetString(DataSaveKey);
            if (string.IsNullOrEmpty(json))
            {
                UserData = new UserData();
            }
            else
            {
                UserData = JsonUtility.FromJson<UserData>(json);
            }
        }
    }

    public class UserData
    {
        public bool IsMuted = false;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    public const string DataSaveKey = "DataSaveKey";

    public UserData UserData { private set; get; }

    public void Init()
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

[Serializable]
public class UserData
{
    public int UserHitShipsCount;
    public int ComputerHitShipsCount;
}

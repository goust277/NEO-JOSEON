using UnityEngine;

public static class GameData
{
    public static int Chapter
    {
        get { return PlayerPrefs.GetInt("Chapter", 1); }
        set { PlayerPrefs.SetInt("Chapter", value); }
    }

    public static int Stage
    {
        get { return PlayerPrefs.GetInt("Stage", 1); }
        set { PlayerPrefs.SetInt("Stage", value); }
    }

    public static bool clearTuto
    {
        get { return PlayerPrefs.GetInt("clearTuto", 0) == 1; }
        set { PlayerPrefs.SetInt("clearTuto", value ? 1 : 0); }
    }

    public static float BGMsound
    {
        get { return PlayerPrefs.GetFloat("BGMsound", -20f); }
        set { PlayerPrefs.SetFloat("BGMsound", value); }
    }

    public static float SFXSound
    {
        get { return PlayerPrefs.GetFloat("SFXSound", -20f); }
        set { PlayerPrefs.SetFloat("SFXSound", value); }
    }

    public static void TutoClear()
    {
        clearTuto = true;
    }
    public static void SaveData()
    {
        Debug.Log($"Data Saved: SFXSound = {SFXSound}");
    }
    public static void LoadData()
    {
        Debug.Log($"LoadData: SFXSound = {SFXSound}");
        Debug.Log($"LoadData: BGMSound = {BGMsound}");
    }
}
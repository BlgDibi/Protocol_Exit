using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement; // Ez kell a pálya váltáshoz!

public class SaveSystem
{
    private static SaveData _savedata = new SaveData();
    public static int CurrentSceneIndex; // Itt tároljuk ideiglenesen a pálya számát

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerdata;
        public int sceneIndex; // ÚJ: Ebbe mentjük a pálya indexét
    }

    public static string SaveFileName() => Application.persistentDataPath + "/save.json";

    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_savedata, true));
        Debug.Log("Mentve: " + SaveFileName());
    }

    private static void HandleSaveData()
    {
        _savedata.sceneIndex = CurrentSceneIndex; // Elmentjük az aktuális pálya számát

        PlayerSave playerScript = GameObject.FindObjectOfType<PlayerSave>();
        if (playerScript != null)
        {
            playerScript.Save(ref _savedata.playerdata);
        }
    }

    public static void Load()
    {
        if (File.Exists(SaveFileName()))
        {
            string saveContent = File.ReadAllText(SaveFileName());
            _savedata = JsonUtility.FromJson<SaveData>(saveContent);

            // 1. Betöltjük a mentett jelenetet
            SceneManager.LoadScene(_savedata.sceneIndex);

            // Megjegyzés: A pozíciót a pálya betöltése után a PlayerSave fogja kezelni!
        }
    }

    // Ezt fogja hívni a Player betöltéskor
    public static PlayerSaveData GetPlayerData() => _savedata.playerdata;
}
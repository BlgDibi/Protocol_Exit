using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    private static SaveData _savedata = new SaveData();

    // Ez a "post-it", ami jelzi a Playernek, hogy tölteni kell
    public static bool IsGameJustLoaded = false;

    public static int CurrentSceneIndex;

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerdata;
        public int sceneIndex;
        public SceneSaveData scenedata;
    }

    public static string SaveFileName(int slot = 1)
    {
        return Application.persistentDataPath + "/save_" + slot + ".json";
    }

    public static void Save(int slot = 1)
    {
        HandleSaveData(); // Adatok begyûjtése
        string json = JsonUtility.ToJson(_savedata, true);
        File.WriteAllText(SaveFileName(slot), json);
        Debug.Log("Fájl mentve: " + SaveFileName(slot));
    }

    private static void HandleSaveData()
    {
        // Pálya index mentése
        _savedata.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Játékos adatok mentése
        PlayerSave playerScript = GameObject.FindObjectOfType<PlayerSave>();
        if (playerScript != null)
        {
            playerScript.Save(ref _savedata.playerdata);
        }
        else
        {
            Debug.LogError("HIBA: Nem található PlayerSave script a mentésnél!");
        }

        // SceneData mentése
        SceneData sceneScript = GameObject.FindObjectOfType<SceneData>();
        if (sceneScript != null)
        {
            sceneScript.Save(ref _savedata.scenedata);
        }
    }

    public static void Load(int slot = 1)
    {
        string fileName = SaveFileName(slot);

        if (File.Exists(fileName))
        {
            string saveContent = File.ReadAllText(fileName);
            _savedata = JsonUtility.FromJson<SaveData>(saveContent);

            // JELZÕ BEÁLLÍTÁSA - Ez a legfontosabb sor!
            IsGameJustLoaded = true;

            // Pálya betöltése
            // A PlayerSave Start() függvénye fogja majd érzékelni az IsGameJustLoaded-et
            // amikor a pálya betöltõdött.
            SceneManager.LoadScene(_savedata.sceneIndex);
        }
        else
        {
            Debug.LogWarning("Nincs mentés ezen a sloton: " + slot);
        }
    }

    // Ez adja át az adatot a Playernek
    public static PlayerSaveData GetPlayerData()
    {
        return _savedata.playerdata;
    }
}
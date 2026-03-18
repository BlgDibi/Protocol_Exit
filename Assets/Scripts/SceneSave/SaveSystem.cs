using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    private static SaveData _savedata = new SaveData();
    public static bool IsGameJustLoaded = false;
    public static int CurrentSceneIndex;

    // Statikus listák a jelenetváltások közötti adatmegõrzéshez
    private static List<string> _tempUnlockedDoors = new List<string>();
    private static List<string> _tempMetNPCs = new List<string>();

    static SaveSystem()
    {
        InitializeData();
    }

    private static void InitializeData()
    {
        if (_savedata.unlockedDoors == null) _savedata.unlockedDoors = new List<string>();
        if (_savedata.metNPCs == null) _savedata.metNPCs = new List<string>();
        if (_savedata.activatedSecrets == null) _savedata.activatedSecrets = new List<string>();
        if (_savedata.InventoryBoxnames == null) _savedata.InventoryBoxnames = new List<string>();
        if (_savedata.PlacedBoxes == null) _savedata.PlacedBoxes = new List<PlacedBoxData>();
        if (_savedata.InventoryKeyIds == null) _savedata.InventoryKeyIds = new List<int>();
        if (_savedata.historicalKeyIds == null) _savedata.historicalKeyIds = new List<int>();
        if (_savedata.inkMemoryKeys == null) _savedata.inkMemoryKeys = new List<string>();
        if (_savedata.inkMemoryValues == null) _savedata.inkMemoryValues = new List<string>();
    }

    [System.Serializable]
    public struct PlacedBoxData
    {
        public string boxname;
        public Vector3 position;
    }

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerdata;
        public int sceneIndex;
        public List<string> InventoryBoxnames;
        public List<PlacedBoxData> PlacedBoxes;
        public List<int> InventoryKeyIds;
        public List<int> historicalKeyIds;
        public float savedElapsedTime;
        public List<string> activatedSecrets;
        public List<string> inkMemoryKeys;
        public List<string> inkMemoryValues;
        public bool hasFlashlight;
        public List<string> unlockedDoors;
        public List<string> metNPCs;
    }

    public static string SaveFileName(int slot = 1) => Application.persistentDataPath + "/save_" + slot + ".json";

    public static void Save(int slot = 1)
    {
        HandleSaveData();

        // Statikus listák szinkronizálása a mentés elõtt
        _savedata.unlockedDoors = new List<string>(_tempUnlockedDoors);
        _savedata.metNPCs = new List<string>(_tempMetNPCs);

        string json = JsonUtility.ToJson(_savedata, true);
        File.WriteAllText(SaveFileName(slot), json);
        Debug.Log("<color=green>MENTÉS SIKERES!</color> Ajtók: " + _savedata.unlockedDoors.Count + " | NPC-k: " + _savedata.metNPCs.Count);
    }

    private static void HandleSaveData()
    {
        _savedata.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        CurrentSceneIndex = _savedata.sceneIndex;

        PlayerSave playerScript = GameObject.FindFirstObjectByType<PlayerSave>();
        if (playerScript != null) playerScript.Save(ref _savedata.playerdata);

        if (BoxInventory.Instance != null) _savedata.InventoryBoxnames = BoxInventory.Instance.GetSavedBoxNames();

        // Dobozok mentése a világban
        _savedata.PlacedBoxes = new List<PlacedBoxData>();
        GameObject[] boxesInWorld = GameObject.FindGameObjectsWithTag("Boxes");
        foreach (GameObject box in boxesInWorld)
        {
            Boxpickup bp = box.GetComponent<Boxpickup>();
            if (bp != null && bp.isPlaced)
            {
                string cleanName = box.name.Replace("(Clone)", "").Replace("_Placed", "").Trim();
                _savedata.PlacedBoxes.Add(new PlacedBoxData { boxname = cleanName, position = box.transform.position });
            }
        }

        if (KeyInventory.Instance != null)
        {
            _savedata.InventoryKeyIds = KeyInventory.Instance.GetSavedKeyIds();
            _savedata.historicalKeyIds = KeyInventory.Instance.GetHistoricalKeyIds();
        }

        Stopwatch stopwatch = GameObject.FindFirstObjectByType<Stopwatch>();
        if (stopwatch != null) _savedata.savedElapsedTime = stopwatch.GetElapsedTime();

        // Titkok mentése
        _savedata.activatedSecrets = new List<string>();
        SecretRevealer[] secrets = GameObject.FindObjectsByType<SecretRevealer>(FindObjectsSortMode.None);
        foreach (var secret in secrets)
        {
            if (secret.isMoved && !string.IsNullOrEmpty(secret.secretID))
                _savedata.activatedSecrets.Add(secret.secretID);
        }

        // Ink Memória mentése
        _savedata.inkMemoryKeys = new List<string>();
        _savedata.inkMemoryValues = new List<string>();
        var mem = DialogueManager.GetInkMemory();
        if (mem != null)
        {
            foreach (var kvp in mem)
            {
                _savedata.inkMemoryKeys.Add(kvp.Key);
                _savedata.inkMemoryValues.Add(kvp.Value);
            }
        }
        _savedata.hasFlashlight = Flashlightpickup.HasFlashlight;
    }

    public static void Load(int slot = 1)
    {
        string path = SaveFileName(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _savedata = JsonUtility.FromJson<SaveData>(json);

            InitializeData();

            // Statikus listák visszaállítása a betöltött adatokból
            _tempUnlockedDoors = new List<string>(_savedata.unlockedDoors);
            _tempMetNPCs = new List<string>(_savedata.metNPCs);

            if (_savedata.inkMemoryKeys != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 0; i < _savedata.inkMemoryKeys.Count; i++)
                    dict[_savedata.inkMemoryKeys[i]] = _savedata.inkMemoryValues[i];
                DialogueManager.SetInkMemory(dict);
            }

            Flashlightpickup.HasFlashlight = _savedata.hasFlashlight;
            IsGameJustLoaded = true;
            SceneManager.LoadScene(_savedata.sceneIndex);
        }
    }

    // --- NPC KEZELÉS ---
    public static void SaveNPCMet(string npcID)
    {
        if (string.IsNullOrEmpty(npcID)) return;
        if (!_tempMetNPCs.Contains(npcID))
        {
            _tempMetNPCs.Add(npcID);
            Debug.Log("<color=magenta>NPC rögzítve a listába: </color>" + npcID);
        }
    }

    public static bool WasNPCMet(string npcID)
    {
        if (string.IsNullOrEmpty(npcID)) return false;
        return _tempMetNPCs.Contains(npcID);
    }

    // --- AJTÓK KEZELÉSE ---
    public static void SaveDoorUnlocked(string doorID)
    {
        if (string.IsNullOrEmpty(doorID)) return;
        if (!_tempUnlockedDoors.Contains(doorID))
        {
            _tempUnlockedDoors.Add(doorID);
            Debug.Log("<color=blue>Ajtó felvéve a listába: </color>" + doorID);
        }
    }

    public static void ResetAllData()
    {
        _savedata = new SaveData();
        _tempUnlockedDoors.Clear();
        _tempMetNPCs.Clear();
        InitializeData();
        IsGameJustLoaded = false;
        Debug.Log("<color=red>Minden statikus mentési adat törölve!</color>");
    }

    public static bool IsDoorUnlocked(string doorID) => _tempUnlockedDoors.Contains(doorID);

    // --- GETTEREK ---
    public static PlayerSaveData GetPlayerData() => _savedata.playerdata;
    public static List<string> GetInventoryBoxes() => _savedata.InventoryBoxnames ?? new List<string>();
    public static List<PlacedBoxData> GetSavedPlacedBoxes() => _savedata.PlacedBoxes ?? new List<PlacedBoxData>();
    public static List<int> GetInventoryKeys() => _savedata.InventoryKeyIds ?? new List<int>();
    public static List<int> GetHistoricalKeys() => _savedata.historicalKeyIds ?? new List<int>();
    public static float GetSavedTime() => _savedata.savedElapsedTime;
    public static bool IsSecretActivated(string secretID) => _savedata.activatedSecrets != null && _savedata.activatedSecrets.Contains(secretID);
}
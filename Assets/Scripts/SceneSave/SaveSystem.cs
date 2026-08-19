using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveSystem
{
    public static int SelectedSkinIndex = 0;

    public static int SlotToSaveAfterLoad = -1;

    private static SaveData _savedata = new SaveData();
    public static bool IsGameJustLoaded = false;
    public static int CurrentSceneIndex;

    private static List<string> _tempUnlockedDoors = new List<string>();
    private static List<string> _tempMetNPCs = new List<string>();
    private static List<string> _tempPriedPlanks = new List<string>();

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
        if (_savedata.priedPlanks == null) _savedata.priedPlanks = new List<string>();
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
        public int savedSkinIndex;
        public string savedKeypadFullCode;
        public string savedKeypadClue;
        public string savedDocumentText;
        public string savedSceneId;
        public bool hasCrowbar;
        public List<string> priedPlanks;
    }

    public static string KeypadFullCode = "";
    public static string KeypadClue = "";
    public static string CurrentDocumentText = "";
    public static string CurrentSceneId = "";

    public static string SaveFileName(int slot = 1) => Application.persistentDataPath + "/save_" + slot + ".json";

    public static void Save(int slot = 1)
    {
        HandleSaveData();
        _savedata.unlockedDoors = new List<string>(_tempUnlockedDoors);
        _savedata.metNPCs = new List<string>(_tempMetNPCs);
        _savedata.priedPlanks = new List<string>(_tempPriedPlanks);
        string json = JsonUtility.ToJson(_savedata, true);
        File.WriteAllText(SaveFileName(slot), json);
        Debug.Log("<color=green>Succesfull save!</color>");
    }

    private static void HandleSaveData()
    {
        _savedata.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        CurrentSceneIndex = _savedata.sceneIndex;
        PlayerSave playerScript = GameObject.FindFirstObjectByType<PlayerSave>();
        if (playerScript != null) playerScript.Save(ref _savedata.playerdata);
        if (BoxInventory.Instance != null) _savedata.InventoryBoxnames = BoxInventory.Instance.GetSavedBoxNames();
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
        _savedata.activatedSecrets = new List<string>();
        SecretRevealer[] secrets = GameObject.FindObjectsByType<SecretRevealer>(FindObjectsSortMode.None);
        foreach (var secret in secrets)
        {
            if (secret.isMoved && !string.IsNullOrEmpty(secret.secretID))
                _savedata.activatedSecrets.Add(secret.secretID);
        }
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
        _savedata.hasCrowbar = CrowbarPickup.HasCrowbar;
        _savedata.savedSkinIndex = SelectedSkinIndex;

        _savedata.savedKeypadFullCode = KeypadFullCode;
        _savedata.savedKeypadClue = KeypadClue;
        _savedata.savedDocumentText = CurrentDocumentText;
        _savedata.savedSceneId = CurrentSceneId;
    }

    public static void Load(int slot = 1)
    {
        string path = SaveFileName(slot);
        if (!File.Exists(path)) return;
        try
        {
            string json = File.ReadAllText(path);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);
            if (string.IsNullOrEmpty(json)) throw new Exception("Empty save!");
            ResetAllData();
            if (KeyInventory.Instance != null) KeyInventory.Instance.ResetKeys();
            _savedata = loadedData;
            InitializeData();
            _tempUnlockedDoors = new List<string>(_savedata.unlockedDoors);
            _tempMetNPCs = new List<string>(_savedata.metNPCs);
            if (_savedata.inkMemoryKeys != null && _savedata.inkMemoryKeys.Count > 0)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 0; i < _savedata.inkMemoryKeys.Count; i++)
                    dict[_savedata.inkMemoryKeys[i]] = _savedata.inkMemoryValues[i];
                DialogueManager.SetInkMemory(dict);
            }
            Flashlightpickup.HasFlashlight = _savedata.hasFlashlight;

            SelectedSkinIndex = _savedata.savedSkinIndex;

            CurrentSceneIndex = _savedata.sceneIndex;

            KeypadFullCode = _savedata.savedKeypadFullCode;
            KeypadClue = _savedata.savedKeypadClue;
            CurrentDocumentText = _savedata.savedDocumentText;
            CurrentSceneId = _savedata.savedSceneId;
            CrowbarPickup.HasCrowbar = _savedata.hasCrowbar;
            _tempPriedPlanks = new List<string>(_savedata.priedPlanks ?? new List<string>());

            UiManager.isMenuOpen = false;
            Time.timeScale = 1f;
        }
        catch (Exception error)
        {
            Debug.LogError("Error while loading: " + error.Message);
            ResetAllData();
            SceneManager.LoadScene(0);
        }
    }

    public static int GetFirstEmptySlot(int maxSlots = 3)
    {
        for (int i = 1; i <= maxSlots; i++)
        {
            if (!File.Exists(SaveFileName(i)))
            {
                return i;
            }
        }
        return -1;
    }
    public static string GetOldestSaveFileName()
    {
        string folder = Application.persistentDataPath;
        var files = Directory.GetFiles(folder, "save_*.json");
        return files.Length > 0 ? Path.GetFileName(files.OrderBy(f => File.GetLastWriteTime(f)).First()) : null;
    }

    public static void DeleteOldestSaveFile()
    {
        string folder = Application.persistentDataPath;
        var files = Directory.GetFiles(folder, "save_*.json");
        if (files.Length > 0)
        {
            string oldestFile = files.OrderBy(f => File.GetLastWriteTime(f)).First();
            File.Delete(oldestFile);
            ResetAllData();
            Debug.Log("Oldest save deleted: " + oldestFile);
        }
    }

    public static void ResetAllData()
    {
        _savedata = new SaveData();
        _tempUnlockedDoors.Clear();
        _tempMetNPCs.Clear();
        InitializeData();
        IsGameJustLoaded = false;
        if (Flashlightpickup.HasFlashlight) Flashlightpickup.HasFlashlight = false;
        KeypadFullCode = "";
        KeypadClue = "";
        CurrentDocumentText = "";
        CurrentSceneId = "";
        _tempPriedPlanks.Clear();
        if (CrowbarPickup.HasCrowbar) CrowbarPickup.HasCrowbar = false;
    }

    public static void SaveNPCMet(string npcID) { if (string.IsNullOrEmpty(npcID)) return; if (!_tempMetNPCs.Contains(npcID)) _tempMetNPCs.Add(npcID); }
    public static bool WasNPCMet(string npcID) => _tempMetNPCs.Contains(npcID);
    public static void SaveDoorUnlocked(string doorID) { if (string.IsNullOrEmpty(doorID)) return; if (!_tempUnlockedDoors.Contains(doorID)) _tempUnlockedDoors.Add(doorID); }
    public static bool IsDoorUnlocked(string doorID) => _tempUnlockedDoors.Contains(doorID);
    public static void SavePlankPried(string plankID) { if (string.IsNullOrEmpty(plankID)) return; if (!_tempPriedPlanks.Contains(plankID)) _tempPriedPlanks.Add(plankID); }
    public static bool IsPlankPried(string plankID) => _tempPriedPlanks.Contains(plankID);
    public static PlayerSaveData GetPlayerData() => _savedata.playerdata;
    public static List<string> GetInventoryBoxes() => _savedata.InventoryBoxnames ?? new List<string>();
    public static List<PlacedBoxData> GetSavedPlacedBoxes() => _savedata.PlacedBoxes ?? new List<PlacedBoxData>();
    public static List<int> GetInventoryKeys() => _savedata.InventoryKeyIds ?? new List<int>();
    public static List<int> GetHistoricalKeys() => _savedata.historicalKeyIds ?? new List<int>();
    public static float GetSavedTime() => _savedata.savedElapsedTime;
    public static bool IsSecretActivated(string secretID) => _savedata.activatedSecrets != null && _savedata.activatedSecrets.Contains(secretID);
}
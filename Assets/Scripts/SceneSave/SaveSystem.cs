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
    public static int CurrentSlot = 1;

    private static List<string> _tempUnlockedDoors = new List<string>();
    private static List<string> _tempMetNPCs = new List<string>();
    private static List<string> _tempPriedPlanks = new List<string>();
    private static List<Savedobjectpos_save> _tempSavedTransforms = new List<Savedobjectpos_save>();
    private static List<string> _tempSolvedPuzzles = new List<string>();

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
        if (_savedata.savedTransforms == null) _savedata.savedTransforms = new List<Savedobjectpos_save>();
        if (_savedata.solvedPuzzles == null) _savedata.solvedPuzzles = new List<string>();
    }

    [System.Serializable]
    public struct PlacedBoxData
    {
        public string boxname;
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    public struct Savedobjectpos_save
    {
        public string objectID;
        public Vector3 position;
        public Quaternion rotation;
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
        public float savedBatteryLevel;
        public bool hasCrowbar;
        public bool hasDetonator;
        public bool hasC4;
        public bool isBarricadeDestroyed; 

        public List<string> unlockedDoors;
        public List<string> metNPCs;
        public int savedSkinIndex;
        public string savedKeypadFullCode;
        public string savedKeypadClue;
        public string savedDocumentText;
        public string savedSceneId;
        public List<string> priedPlanks;
        public List<Savedobjectpos_save> savedTransforms;
        public List<string> solvedPuzzles;
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
        _savedata.savedTransforms = new List<Savedobjectpos_save>(_tempSavedTransforms);
        _savedata.solvedPuzzles = new List<string>(_tempSolvedPuzzles);

        string json = JsonUtility.ToJson(_savedata, true);
        File.WriteAllText(SaveFileName(slot), json);
        Debug.Log("<color=green>Successful save!</color>");
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
            if (bp != null && (bp.isPlaced || box.name.Contains("_Placed")))
            {
                string cleanName = box.name.Replace("(Clone)", "").Replace("_Placed", "").Trim();
                _savedata.PlacedBoxes.Add(new PlacedBoxData { boxname = cleanName, position = box.transform.position, rotation = box.transform.rotation });
            }
        }

        _tempSavedTransforms.Clear();
        ItemSave[] saveableObjects = GameObject.FindObjectsByType<ItemSave>(FindObjectsSortMode.None);
        foreach (var obj in saveableObjects)
        {
            if (!string.IsNullOrEmpty(obj.itemID))
            {
                _tempSavedTransforms.Add(new Savedobjectpos_save
                {
                    objectID = obj.itemID,
                    position = obj.transform.position,
                    rotation = obj.transform.rotation
                });
            }
        }

        if (KeyInventory.Instance != null)
        {
            _savedata.InventoryKeyIds = KeyInventory.Instance.GetSavedKeyIds();
            _savedata.historicalKeyIds = KeyInventory.Instance.GetHistoricalKeyIds();
        }
        Stopwatch stopwatch = GameObject.FindFirstObjectByType<Stopwatch>();
        if (stopwatch != null)
            _savedata.savedElapsedTime = stopwatch.GetElapsedTime();

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
        _savedata.savedBatteryLevel = FlashlightController.currentBattery;

        _savedata.hasCrowbar = CrowbarPickup.HasCrowbar;
        _savedata.hasDetonator = Detonator_Pickup.HasDetonator;
        _savedata.hasC4 = C4_Pickup.HasC4;

        if (ConditionManager.Instance != null)
        {
            _savedata.isBarricadeDestroyed = ConditionManager.Instance.CheckCondition("BarricadeDestroyed");
        }

        _savedata.savedSkinIndex = SelectedSkinIndex;
        _savedata.savedKeypadFullCode = KeypadFullCode;
        _savedata.savedKeypadClue = KeypadClue;
        _savedata.savedDocumentText = CurrentDocumentText;
        _savedata.savedSceneId = CurrentSceneId;
    }

    public static bool GetCupboardTransform(string id, out Vector3 pos, out Quaternion rot)
    {
        pos = Vector3.zero;
        rot = Quaternion.identity;
        if (_savedata.savedTransforms == null) return false;

        foreach (var t in _savedata.savedTransforms)
        {
            if (t.objectID == id)
            {
                pos = t.position;
                rot = t.rotation;
                return true;
            }
        }
        return false;
    }

    public static void Load(int slot = 1)
    {
        string path = SaveFileName(slot);
        if (!File.Exists(path)) return;
        try
        {
            string json = File.ReadAllText(path);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);
            if (string.IsNullOrEmpty(json))
                throw new Exception("Empty save!");
            CurrentSlot = slot;

            ResetAllData();

            IsGameJustLoaded = true;

            if (KeyInventory.Instance != null) KeyInventory.Instance.ResetKeys();
            if (!json.Contains("savedBatteryLevel"))
            {
                loadedData.savedBatteryLevel = 100f;
            }

            _savedata = loadedData;
            InitializeData();

            _tempUnlockedDoors = new List<string>(_savedata.unlockedDoors);
            _tempMetNPCs = new List<string>(_savedata.metNPCs);
            if (_savedata.priedPlanks != null)
            {
                _tempPriedPlanks = new List<string>(_savedata.priedPlanks);
            }
            else
            {
                _tempPriedPlanks = new List<string>();
            }
            _tempSavedTransforms = new List<Savedobjectpos_save>(_savedata.savedTransforms ?? new List<Savedobjectpos_save>());
            if (_savedata.solvedPuzzles != null)
            {
                _tempSolvedPuzzles = new List<string>(_savedata.solvedPuzzles);
            }
            else
            {
                _tempSolvedPuzzles = new List<string>();
            }

            if (_savedata.inkMemoryKeys != null && _savedata.inkMemoryKeys.Count > 0)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 0; i < _savedata.inkMemoryKeys.Count; i++)
                    dict[_savedata.inkMemoryKeys[i]] = _savedata.inkMemoryValues[i];
                DialogueManager.SetInkMemory(dict);
            }

            Flashlightpickup.HasFlashlight = _savedata.hasFlashlight;
            FlashlightController.currentBattery = _savedata.savedBatteryLevel;

            CrowbarPickup.HasCrowbar = _savedata.hasCrowbar;
            Detonator_Pickup.HasDetonator = _savedata.hasDetonator;
            C4_Pickup.HasC4 = _savedata.hasC4;

            if (ConditionManager.Instance != null)
            {
                ConditionManager.Instance.SetCondition("BarricadeDestroyed", _savedata.isBarricadeDestroyed);
            }

            if (_savedata.isBarricadeDestroyed)
            {
                Detonator_Pickup.HasDetonator = false;
            }

            SelectedSkinIndex = _savedata.savedSkinIndex;
            CurrentSceneIndex = _savedata.sceneIndex;

            KeypadFullCode = _savedata.savedKeypadFullCode;
            KeypadClue = _savedata.savedKeypadClue;
            CurrentDocumentText = _savedata.savedDocumentText;
            CurrentSceneId = _savedata.savedSceneId;

            if (UiManager.Instance != null)
            {
                UiManager.Instance.RefreshAllUI();
            }

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
            if (!File.Exists(SaveFileName(i))) return i;
        }
        return -1;
    }

    public static string GetOldestSaveFileName()
    {
        string folder = Application.persistentDataPath;
        string[] files = Directory.GetFiles(folder, "save_*.json");

        if (files.Length == 0) return null;

        string oldestFile = files[0];
        System.DateTime oldestTime = File.GetLastWriteTime(oldestFile);

        for (int i = 1; i < files.Length; i++)
        {
            System.DateTime currentTime = File.GetLastWriteTime(files[i]);
            if (currentTime < oldestTime)
            {
                oldestTime = currentTime;
                oldestFile = files[i];
            }
        }
        return Path.GetFileName(oldestFile);
    }

    public static void DeleteOldestSaveFile()
    {
        string folder = Application.persistentDataPath;
        string[] files = Directory.GetFiles(folder, "save_*.json");

        if (files.Length > 0)
        {
            string oldestFile = files[0];
            System.DateTime oldestTime = File.GetLastWriteTime(oldestFile);

            for (int i = 1; i < files.Length; i++)
            {
                System.DateTime currentTime = File.GetLastWriteTime(files[i]);
                if (currentTime < oldestTime)
                {
                    oldestTime = currentTime;
                    oldestFile = files[i];
                }
            }

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
        _tempPriedPlanks.Clear();
        _tempSavedTransforms.Clear();
        _tempSolvedPuzzles.Clear();

        InitializeData();
        IsGameJustLoaded = false;

        if (Flashlightpickup.HasFlashlight) Flashlightpickup.HasFlashlight = false;

        FlashlightController.currentBattery = 100f;

        if (CrowbarPickup.HasCrowbar) CrowbarPickup.HasCrowbar = false;
        if (Detonator_Pickup.HasDetonator) Detonator_Pickup.HasDetonator = false;
        if (C4_Pickup.HasC4) C4_Pickup.HasC4 = false;

        KeypadFullCode = "";
        KeypadClue = "";
        CurrentDocumentText = "";
        CurrentSceneId = "";
    }

    public static void SaveNPCMet(string npcID)
    {
        if (string.IsNullOrEmpty(npcID)) return;
        if (!_tempMetNPCs.Contains(npcID)) _tempMetNPCs.Add(npcID);
    }
    public static bool WasNPCMet(string npcID) => _tempMetNPCs.Contains(npcID);

    public static void SaveDoorUnlocked(string doorID)
    {
        if (string.IsNullOrEmpty(doorID)) return;
        if (!_tempUnlockedDoors.Contains(doorID)) _tempUnlockedDoors.Add(doorID);
    }
    public static bool IsDoorUnlocked(string doorID) => _tempUnlockedDoors.Contains(doorID);

    public static void SavePlankPried(string plankID)
    {
        if (string.IsNullOrEmpty(plankID)) return;
        if (!_tempPriedPlanks.Contains(plankID)) _tempPriedPlanks.Add(plankID);
    }
    public static bool IsPlankPried(string plankID) => _tempPriedPlanks.Contains(plankID);

    public static PlayerSaveData GetPlayerData() => _savedata.playerdata;
    public static List<string> GetInventoryBoxes() => _savedata.InventoryBoxnames ?? new List<string>();
    public static List<PlacedBoxData> GetSavedPlacedBoxes() => _savedata.PlacedBoxes ?? new List<PlacedBoxData>();
    public static List<int> GetInventoryKeys() => _savedata.InventoryKeyIds ?? new List<int>();
    public static List<int> GetHistoricalKeys() => _savedata.historicalKeyIds ?? new List<int>();
    public static float GetSavedTime() => _savedata.savedElapsedTime;
    public static bool IsSecretActivated(string secretID) => _savedata.activatedSecrets != null && _savedata.activatedSecrets.Contains(secretID);

    public static void SavePuzzleSolved(string puzzleID)
    {
        if (string.IsNullOrEmpty(puzzleID)) return;
        if (!_tempSolvedPuzzles.Contains(puzzleID)) _tempSolvedPuzzles.Add(puzzleID);
    }
    public static bool IsPuzzleSolved(string puzzleID) => _savedata.solvedPuzzles != null && _savedata.solvedPuzzles.Contains(puzzleID);
}
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveSystem
{
    private static SaveData _savedata = new SaveData();
    public static bool IsGameJustLoaded = false;
    public static int CurrentSceneIndex;

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
        public SceneSaveData scenedata;
        public List<string> InventoryBoxnames;
        public List<PlacedBoxData> PlacedBoxes;
        public List<int> InventoryKeyIds;
        public float savedElapsedTime;

        public List<string> activatedSecrets;
        public List<string> inkMemoryKeys;
        public List<string> inkMemoryValues;
    }

    public static string SaveFileName(int slot = 1)
    {
        return Application.persistentDataPath + "/save_" + slot + ".json";
    }

    public static void Save(int slot = 1)
    {
        HandleSaveData();
        string json = JsonUtility.ToJson(_savedata, true);
        File.WriteAllText(SaveFileName(slot), json);
        Debug.Log("Game saved! File: " + SaveFileName(slot));
    }

    private static void HandleSaveData()
    {
        _savedata.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        PlayerSave playerScript = GameObject.FindFirstObjectByType<PlayerSave>();
        if (playerScript != null) playerScript.Save(ref _savedata.playerdata);

        if (BoxInventory.Instance != null) { _savedata.InventoryBoxnames = BoxInventory.Instance.GetSavedBoxNames(); }
        else { _savedata.InventoryBoxnames = new List<string>(); }

        _savedata.PlacedBoxes = new List<PlacedBoxData>();
        GameObject[] boxesInWorld = GameObject.FindGameObjectsWithTag("Boxes");
        foreach (GameObject box in boxesInWorld)
        {
            string cleanName = box.name.Replace("(Clone)", "").Replace("_Placed", "").Trim();
            _savedata.PlacedBoxes.Add(new PlacedBoxData { boxname = cleanName, position = box.transform.position });
        }

        if (KeyInventory.Instance != null) { _savedata.InventoryKeyIds = KeyInventory.Instance.GetSavedKeyIds(); }
        else { _savedata.InventoryKeyIds = new List<int>(); }

        Stopwatch stopwatch = GameObject.FindFirstObjectByType<Stopwatch>();
        if (stopwatch != null) { _savedata.savedElapsedTime = stopwatch.GetElapsedTime(); }

        _savedata.activatedSecrets = new List<string>();
        SecretRevealer[] secrets = GameObject.FindObjectsByType<SecretRevealer>(FindObjectsSortMode.None);
        foreach (var secret in secrets)
        {
            if (secret.isMoved && !string.IsNullOrEmpty(secret.secretID))
            {
                _savedata.activatedSecrets.Add(secret.secretID);
            }
        }

        _savedata.inkMemoryKeys = new List<string>();
        _savedata.inkMemoryValues = new List<string>();

        var currentInkMemory = DialogueManager.GetInkMemory();
        if (currentInkMemory != null)
        {
            foreach (var kvp in currentInkMemory)
            {
                _savedata.inkMemoryKeys.Add(kvp.Key);
                _savedata.inkMemoryValues.Add(kvp.Value);
            }
        }
    }

    public static void Load(int slot = 1)
    {
        string fileName = SaveFileName(slot);
        if (File.Exists(fileName))
        {
            string saveContent = File.ReadAllText(fileName);
            _savedata = JsonUtility.FromJson<SaveData>(saveContent);

            if (_savedata.InventoryBoxnames == null) _savedata.InventoryBoxnames = new List<string>();
            if (_savedata.PlacedBoxes == null) _savedata.PlacedBoxes = new List<PlacedBoxData>();
            if (_savedata.InventoryKeyIds == null) _savedata.InventoryKeyIds = new List<int>();
            if (_savedata.activatedSecrets == null) _savedata.activatedSecrets = new List<string>();

            if (_savedata.inkMemoryKeys != null && _savedata.inkMemoryValues != null)
            {
                Dictionary<string, string> loadedInkMem = new Dictionary<string, string>();
                for (int i = 0; i < _savedata.inkMemoryKeys.Count; i++)
                {
                    loadedInkMem[_savedata.inkMemoryKeys[i]] = _savedata.inkMemoryValues[i];
                }
                DialogueManager.SetInkMemory(loadedInkMem);
            }

            IsGameJustLoaded = true;
            SceneManager.LoadScene(_savedata.sceneIndex);
        }
    }

    public static bool IsSecretActivated(string secretID)
    {
        if (_savedata.activatedSecrets == null || string.IsNullOrEmpty(secretID)) return false;
        return _savedata.activatedSecrets.Contains(secretID);
    }

    public static PlayerSaveData GetPlayerData() => _savedata.playerdata;
    public static List<string> GetInventoryBoxes() => _savedata.InventoryBoxnames ?? new List<string>();
    public static List<PlacedBoxData> GetSavedPlacedBoxes()
    {
        if (_savedata.PlacedBoxes == null) _savedata.PlacedBoxes = new List<PlacedBoxData>();
        return _savedata.PlacedBoxes;
    }
    public static List<int> GetInventoryKeys() => _savedata.InventoryKeyIds ?? new List<int>();
    public static float GetSavedTime() => _savedata.savedElapsedTime;
}
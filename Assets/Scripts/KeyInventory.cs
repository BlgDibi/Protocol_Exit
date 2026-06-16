using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class KeyInventory : MonoBehaviour
{
    [SerializeField] private List<int> keyIds = new List<int>();

    [SerializeField] private List<int> historicalKeyIds = new List<int>();

    [Header("Database for loading")]
    [Tooltip("KeyScriptabelObject")]
    [SerializeField] private List<Key> allAvailableKeys = new List<Key>();

    public static KeyInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(CheckLoadStatus());
    }
    IEnumerator CheckLoadStatus()
    {
        yield return null;

        bool isLoaded = SaveSystem.IsGameJustLoaded;

        if (isLoaded)
        {
            LoadKeysFromSave();
        }
    }
    private void LoadKeysFromSave()
    {
        List<int> loadedIds = SaveSystem.GetInventoryKeys();

        List<int> loadedHistorical = SaveSystem.GetHistoricalKeys();
        if (loadedHistorical != null) historicalKeyIds = new List<int>(loadedHistorical);

        if (loadedIds != null && loadedIds.Count > 0)
        {
            keyIds.Clear();

            foreach (int id in loadedIds)
            {
                Key foundKey = allAvailableKeys.Find(k => k.id == id);
                if (foundKey != null && !keyIds.Contains(foundKey.id))
                {
                    keyIds.Add(foundKey.id);
                    if (UiManager.Instance != null) UiManager.Instance.AddKeyToUI(foundKey);
                }
            }
        }
    }

    public List<int> GetSavedKeyIds() => new List<int>(keyIds);

    public List<int> GetHistoricalKeyIds() => new List<int>(historicalKeyIds);

    public void AddKey(Key key)
    {
        if (!keyIds.Contains(key.id))
        {
            keyIds.Add(key.id);

            if (!historicalKeyIds.Contains(key.id)) historicalKeyIds.Add(key.id);

            if (UiManager.Instance != null) UiManager.Instance.AddKeyToUI(key);
        }
    }

    public bool HasKey(Key key) => keyIds.Contains(key.id);

    public void RemoveKey(Key key)
    {
        if (keyIds.Contains(key.id))
        {
            keyIds.Remove(key.id);
            if (UiManager.Instance != null) UiManager.Instance.RemoveKeyFromUI(key);
        }
    }

    public void ResetKeys()
    {
        keyIds.Clear();
        historicalKeyIds.Clear();
        if (UiManager.Instance != null) UiManager.Instance.ClearAllKeysFromUI();
    }




}
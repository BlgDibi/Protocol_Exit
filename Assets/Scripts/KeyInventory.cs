using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Fontos a Coroutine miatt

public class KeyInventory : MonoBehaviour
{
    [SerializeField] private List<int> keyIds = new List<int>();

    [Header("Database for loading")]
    [SerializeField] private List<Key> allAvailableKeys = new List<Key>();

    public static KeyInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        yield return null;

        if (SaveSystem.IsGameJustLoaded)
        {
            LoadKeysFromSave();
        }
    }

    private void LoadKeysFromSave()
    {
        List<int> loadedIds = SaveSystem.GetInventoryKeys();

        if (loadedIds != null && loadedIds.Count > 0)
        {
            keyIds.Clear();

            foreach (int id in loadedIds)
            {
                Key foundKey = allAvailableKeys.Find(k => k.id == id);
                if (foundKey != null)
                {
                    if (!keyIds.Contains(foundKey.id))
                    {
                        keyIds.Add(foundKey.id);
                        if (UiManager.Instance != null) UiManager.Instance.AddKeyToUI(foundKey);
                    }
                }
            }
            Debug.Log("Betöltve " + keyIds.Count + " kulcs.");
        }
    }

    public List<int> GetSavedKeyIds()
    {
        return new List<int>(keyIds);
    }
    public void AddKey(Key key)
    {
        if (!keyIds.Contains(key.id))
        {
            keyIds.Add(key.id);
            Debug.Log("Key Added");

            UiManager.Instance.AddKeyToUI(key);
        }
    }

    public bool HasKey(Key key)
    {
        return keyIds.Contains(key.id);
    }

    public void RemoveKey(Key key)
    {
        if (keyIds.Contains(key.id))
        {
            keyIds.Remove(key.id);
            Debug.Log("Key Removed");

            UiManager.Instance.RemoveKeyFromUI(key);
        }
    }

    public void ResetKeys()
    {
        keyIds.Clear();
        Debug.Log("KeyInventory Reset");

        if (UiManager.Instance != null)
            UiManager.Instance.ClearAllKeysFromUI(); 
    }
}

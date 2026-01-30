using UnityEngine;
using System.Collections.Generic;

public class KeyInventory : MonoBehaviour
{
    [SerializeField] private List<int> keyIds = new List<int>();

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

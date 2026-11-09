using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BoxInventory : MonoBehaviour
{
    [SerializeField] private List<SulyDobozData> Boxesamount = new List<SulyDobozData>();

    [Header("Database for loading")]
    [SerializeField] private List<SulyDobozData> Allavailableboxes = new List<SulyDobozData>();

    public static BoxInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
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
        StartCoroutine(RefreshAfterLoad());
    }

    private System.Collections.IEnumerator RefreshAfterLoad()
    {
        bool isLoadEvent = SaveSystem.IsGameJustLoaded;

        yield return new WaitForSeconds(0.2f);

        if (isLoadEvent == true)
        {
            LoadInventoryFromSave();
            SaveSystem.IsGameJustLoaded = false;
        }
        else
        {
            RefreshUI();
        }
    }

    private void LoadInventoryFromSave()
    {
        if (UiManager.Instance != null)
        {
            foreach (SulyDobozData regiDoboz in Boxesamount)
            {
                UiManager.Instance.RemoveBoxFromUI(regiDoboz);
            }
        }

        List<string> mentettNevek = SaveSystem.GetInventoryBoxes();
        Boxesamount.Clear();

        if (mentettNevek != null && mentettNevek.Count > 0)
        {
            foreach (string nev in mentettNevek)
            {
                foreach (SulyDobozData adatbazisDoboz in Allavailableboxes)
                {
                    if (adatbazisDoboz.boxname == nev)
                    {
                        Boxesamount.Add(adatbazisDoboz);
                        break; 
                    }
                }
            }
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (UiManager.Instance != null)
        {
            foreach (SulyDobozData doboz in Boxesamount)
            {
                UiManager.Instance.AddBoxToUI(doboz);
            }
        }
    }

    public void AddBox(SulyDobozData doboz)
    {
        if (Boxesamount.Contains(doboz) == false)
        {
            Boxesamount.Add(doboz);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.AddBoxToUI(doboz);
            }
        }
    }

    public void RemoveBox(SulyDobozData doboz)
    {
        if (Boxesamount.Contains(doboz) == true)
        {
            Boxesamount.Remove(doboz);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.RemoveBoxFromUI(doboz);
            }
        }
    }

    public bool HasAnyBox()
    {
        if (Boxesamount.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Egyszerű if-else a kérdőjel (?) helyett
    public SulyDobozData GetFirstBox()
    {
        if (HasAnyBox() == true)
        {
            return Boxesamount[0];
        }
        else
        {
            return null;
        }
    }

    public List<string> GetSavedBoxNames()
    {
        List<string> nevekLista = new List<string>();

        foreach (SulyDobozData doboz in Boxesamount)
        {
            if (doboz != null)
            {
                nevekLista.Add(doboz.boxname);
            }
        }

        return nevekLista;
    }

    public void ClearInventory()
    {
        if (UiManager.Instance != null)
        {
            foreach (SulyDobozData doboz in Boxesamount)
            {
                UiManager.Instance.RemoveBoxFromUI(doboz);
            }
        }

        Boxesamount.Clear();
    }
}
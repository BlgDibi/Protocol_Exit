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

        if (isLoadEvent)
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
            foreach (var regiDoboz in Boxesamount)
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
                SulyDobozData megtalaltDoboz = Allavailableboxes.Find(x => x.boxname == nev);
                if (megtalaltDoboz != null)
                {
                    Boxesamount.Add(megtalaltDoboz);
                }
            }
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (UiManager.Instance != null)
        {
            foreach (var doboz in Boxesamount)
            {
                UiManager.Instance.AddBoxToUI(doboz);
            }
        }
    }

    public void AddBox(SulyDobozData doboz)
    {
        if (!Boxesamount.Contains(doboz))
        {
            Boxesamount.Add(doboz);
            if (UiManager.Instance != null) UiManager.Instance.AddBoxToUI(doboz);
        }
    }

    public void RemoveBox(SulyDobozData doboz)
    {
        if (Boxesamount.Contains(doboz))
        {
            Boxesamount.Remove(doboz);
            if (UiManager.Instance != null) UiManager.Instance.RemoveBoxFromUI(doboz);
        }
    }

    public bool HasAnyBox() => Boxesamount.Count > 0;

    public SulyDobozData GetFirstBox()
    {
        return HasAnyBox() ? Boxesamount[0] : null;
    }

    public List<string> GetSavedBoxNames()
    {
        List<string> name = new List<string>();
        foreach (var box in Boxesamount)
        {
            if (box != null) name.Add(box.boxname);
        }
        return name;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxInventory : MonoBehaviour
{
    [SerializeField] private List<SulyDobozData> Boxesamount = new List<SulyDobozData>();

    [Header("Database for loading")]
    [SerializeField] private List<SulyDobozData> Allavailableboxes = new List<SulyDobozData>();

    public static BoxInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    IEnumerator Start()
    {
        yield return null; 

        if (SaveSystem.IsGameJustLoaded)
        {
            LoadInventoryFromSave(SaveSystem.GetInventoryBoxes());
        }
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

    public void LoadInventoryFromSave(List<string> mentettNevek)
    {
        Boxesamount.Clear(); 

        if (mentettNevek != null)
        {
            foreach (string nev in mentettNevek)
            {
                
                SulyDobozData megtalaltDoboz = Allavailableboxes.Find(x => x.boxname == nev);

                if (megtalaltDoboz != null)
                {
                    
                    AddBox(megtalaltDoboz);
                }
            }
        }
    }

   


    public void AddBox(SulyDobozData doboz)
    {
        if (!Boxesamount.Contains(doboz))
        {
            Boxesamount.Add(doboz);
            UiManager.Instance.AddBoxToUI(doboz);
        }
    }

    public void RemoveBox(SulyDobozData doboz)
    {
        if (Boxesamount.Contains(doboz))
        {
            Boxesamount.Remove(doboz);
            UiManager.Instance.RemoveBoxFromUI(doboz);
        }
    }

    public bool HasAnyBox() => Boxesamount.Count > 0;

    public SulyDobozData GetFirstBox()
    {
        return HasAnyBox() ? Boxesamount[0] : null;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class BoxInventory : MonoBehaviour
{
    [SerializeField] private List<SulyDobozData> Boxesamount = new List<SulyDobozData>();

    public static BoxInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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
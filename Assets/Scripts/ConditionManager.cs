using UnityEngine;
using System.Collections.Generic;

public class ConditionManager : MonoBehaviour
{
    public static ConditionManager Instance;

    // Ez a lista tarolja az allapotokat.
    private Dictionary<string, bool> conditions = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Palyavaltasnal is megmarad
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Ezzel teljesitunk 1-1 feladatot
    public void SetCondition(string conditionName, bool value)
    {
        if (conditions.ContainsKey(conditionName))
            conditions[conditionName] = value;
        else
            conditions.Add(conditionName, value);

        Debug.Log($"LOGIKA FRISSITVE: {conditionName} = {value}");
    }

    //Feladat lekerdezese
    public bool CheckCondition(string conditionName)
    {
        if (conditions.ContainsKey(conditionName))
            return conditions[conditionName];

        return false; // Ha nincs a listaban, akkor meg nem tortent meg
    }
}
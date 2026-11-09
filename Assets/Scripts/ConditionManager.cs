using UnityEngine;
using System.Collections.Generic;

public class ConditionManager : MonoBehaviour
{
    public static ConditionManager Instance;

    private Dictionary<string, bool> conditions = new Dictionary<string, bool>();

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

    public void SetCondition(string conditionName, bool value)
    {
        if (conditions.ContainsKey(conditionName))
            conditions[conditionName] = value;
        else
            conditions.Add(conditionName, value);

    }

    public bool CheckCondition(string conditionName)
    {
        if (conditions.ContainsKey(conditionName))
            return conditions[conditionName];

        return false; 
    }
}
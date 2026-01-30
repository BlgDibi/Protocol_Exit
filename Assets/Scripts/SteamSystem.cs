using UnityEngine;

public class SteamSystem : MonoBehaviour
{
    [Header("Conditions")]
    public string valve1ID = "Valve1";
    public string valve2ID = "Valve2"; 

    [Header("Steam")]
    [Tooltip("Steam item")]
    public GameObject steamObject;

  

    private bool solved = false;

    void Update()
    {
        if (solved) 
            return;

        bool v1 = ConditionManager.Instance.CheckCondition(valve1ID);
        bool v2 = ConditionManager.Instance.CheckCondition(valve2ID);

        
        if (v1 && v2)
        {
            PuzzleSolved();
        }
    }

    void PuzzleSolved()
    {
        Debug.Log("Gõz leállítva!");
        solved = true;

        // KIKAPCSOLJUK A GÕZT
        if (steamObject != null)
        {
            steamObject.SetActive(false);
        }

        
        ConditionManager.Instance.SetCondition("SteamRoomSolved", true);
    }
}
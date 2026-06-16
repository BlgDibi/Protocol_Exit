using UnityEngine;
using System.Collections; // EZ SZÜKSÉGES A KORUTINHOZ

public class GameEvents : MonoBehaviour
{

    void Start()
    {
        if (SaveSystem.SlotToSaveAfterLoad != -1)
        {
            StartCoroutine(DelayedSave());
        }
    }

    IEnumerator DelayedSave()
    {
        // Várunk egy kicsit, hogy minden objektum (Player, Boxok, stb.) biztosan betöltõdjön
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Mentés folyamatban a " + SaveSystem.SlotToSaveAfterLoad + " slotba...");
        SaveSystem.Save(SaveSystem.SlotToSaveAfterLoad);

        // Alaphelyzetbe állítás
        SaveSystem.SlotToSaveAfterLoad = -1;
    }


    public static GameEvents instance;

    public DialogueEvents dialogueEvents;

    private void Awake()
    {

        instance = this;

        dialogueEvents = new DialogueEvents();
    }
}


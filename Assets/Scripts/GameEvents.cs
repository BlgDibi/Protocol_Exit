using UnityEngine;
using System.Collections;

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
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Save in progress " + SaveSystem.SlotToSaveAfterLoad + " slot...");
        SaveSystem.Save(SaveSystem.SlotToSaveAfterLoad);

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


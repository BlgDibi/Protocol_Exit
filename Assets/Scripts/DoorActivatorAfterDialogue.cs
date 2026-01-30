using UnityEngine;
using System.Collections.Generic;

public class DoorActivatorAfterDialogue : MonoBehaviour
{
    [Header("Door to Activate")]
    public GameObject door;   
    public Talk npcTalkScript; 

    private void Start()
    {
        if (door != null)
            door.SetActive(false);

        if (npcTalkScript != null)
            npcTalkScript.OnDialogueFinished += ActivateDoor;
    }

    private void ActivateDoor()
    {
        if (door != null)
            door.SetActive(true);
    }

    private void OnDestroy()
    {
        if (npcTalkScript != null)
            npcTalkScript.OnDialogueFinished -= ActivateDoor;
    }
}

using UnityEngine;

public class GameEvents : MonoBehaviour
{

    public static GameEvents instance;

    public DialogueEvents dialogueEvents;

    private void Awake()
    {

        instance = this;

        dialogueEvents = new DialogueEvents();
    }
}
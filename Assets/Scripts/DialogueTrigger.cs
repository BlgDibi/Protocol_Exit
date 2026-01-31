using UnityEngine;
using UnityEngine.Localization;

public class DialogueTrigger : MonoBehaviour, InterfaceInteractable
{
    [Header("Ink Settings")]
    [Tooltip("knot name")]
    public string inkKnotName = "book_intro";

    [Header("Localization - Interaction")]
    [Tooltip("Text")]
    public LocalizedString interactionPrompt;

    [Header("Settings")]
    [Tooltip("Button to Use")]
    [SerializeField] private KeyCode interactionKey = KeyCode.T;
    public KeyCode GetInteractKey()
    {
        return interactionKey;
    }

    public string GetInteractText()
    {
        return interactionPrompt.GetLocalizedString();
    }

    public void Interact()
    {
        if (GameEvents.instance != null)
        {
            GameEvents.instance.dialogueEvents.EnterDialogue(inkKnotName);
        }
        else
        {
            Debug.LogError("HIBA: GameEvents instance nem található!");
        }
    }
}
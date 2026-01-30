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
    public KeyCode interactKey = KeyCode.F;
    public KeyCode GetInteractKey()
    {
        return interactKey;
    }

    public string GetInteractText()
    {
        if (interactionPrompt.IsEmpty) return "Interact";
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
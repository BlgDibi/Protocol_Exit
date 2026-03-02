using UnityEngine;
using UnityEngine.Localization;

public class DialogueTrigger : MonoBehaviour, InterfaceInteractable
{
    [Header("Ink Settings")]
    [Tooltip(".json file")]
    public TextAsset inkJSON;
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
        if (DialogueManager.Instance != null && inkJSON != null)
        {
            Talk npcScript = GetComponent<Talk>();
            if (npcScript != null)
            {
                npcScript.LookAtPlayer();
            }

            DialogueManager.Instance.EnterDialogueMode(inkJSON, inkKnotName, npcScript);
        }
    }
}
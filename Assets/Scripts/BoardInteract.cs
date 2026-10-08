using UnityEngine;

public class BoardInteract : MonoBehaviour, InterfaceInteractable
{
    [Header("Document Settings")]
    [TextArea(5, 10)]
    public string documentText = "- 08:30 - Surgery preparation\r\n\r\n-10:00 - Management Meeting & Full Lockdown \r\n\r\nInstead of the usual 45-minute coffee break!\r\n\r\n??:?? - Meeting ends, doors unlock";

    [Header("UI Settings")]
    public string promptText = "Read the board";
    public KeyCode interactKey = KeyCode.F;

    public string GetInteractText()
    {
        return promptText;
    }

    public KeyCode GetInteractKey()
    {
        return interactKey;
    }

    public void Interact()
    {
        if (BoardViewer.Instance != null)
        {
            BoardViewer.Instance.OpenDocument(documentText);
        }
        else
        {
            Debug.LogError("There is no BoardViewer script!");
        }
    }
}
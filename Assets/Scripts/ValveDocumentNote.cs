using UnityEngine;
using TMPro;

public class ValveDocumentNote : MonoBehaviour, InterfaceInteractable
{
    [Header("UI Settings")]
    public GameObject documentUIPanel;
    public TextMeshProUGUI documentTextUI;

    [Header("Player movement")]
    public MonoBehaviour playerController;

    private bool isReading = false;

    private string noteText = "<b>Maintenance Note: Spare Valve</b>\n\nI moved the spare gas valve to the metal cabinet in the kitchen.\n\nHalf the drawers are completely rusted shut, especially the bottom row and the left side. Don't even bother trying to open those.\n\n You'll find it.\n\n- Marcus";
    public void Interact()
    {
        if (isReading) return;

        UiManager.isMenuOpen = true;
        isReading = true;

        documentUIPanel.SetActive(true);
        documentTextUI.text = noteText;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    public void CloseDocument()
    {
        UiManager.isMenuOpen = false;
        isReading = false;

        documentUIPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    public KeyCode GetInteractKey() => KeyCode.F;

    public string GetInteractText()
    {
        if (isReading == true)
        {
            return "";
        }
        else
        {
            return "Read the memo";
        }
    }
}
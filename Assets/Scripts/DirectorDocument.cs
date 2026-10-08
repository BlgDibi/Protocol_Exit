using UnityEngine;
using TMPro;

public class DirectorDocument : MonoBehaviour, InterfaceInteractable
{
    [Header("UI Settings")]
    public GameObject documentUIPanel;
    public TextMeshProUGUI documentTextUI;

    [Header("Player movement")]
    public MonoBehaviour playerController;

    private bool isReading = false;

    private string noteText = "<b>Security protocol: Lock mechanism</b>\n\n The doors are connected to the time.\n\nThe system will only unlock them when the management meeting is officially over!\n\n- The director";
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
        return isReading ? "" : "Read the schedule";
    }
}
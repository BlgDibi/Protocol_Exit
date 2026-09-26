using UnityEngine;
using TMPro;

public class IncidentDocument : MonoBehaviour, InterfaceInteractable
{
    [Header("UI Settings")]
    public GameObject documentUIPanel;
    public TextMeshProUGUI documentTextUI;

    [Header("Player movement")]
    public MonoBehaviour playerController;

    private bool isReading = false;

    private string noteText = "<b>Incident Report: Sitting Order </b>\n\nThe Test Subject broke another porcelain plate today.\n\nFrom now on, he is only allowed to drink from the chipped glass.\n\n The Professor is demanding the porcelain plate for his meals, and insists on sitting on the right side of the Director to discuss his findings.\n\n- Elizabeth";
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
        return isReading ? "" : "Read note";
    }
}
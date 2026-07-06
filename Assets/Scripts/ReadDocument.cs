using UnityEngine;
using TMPro;

public class ReadDocument : MonoBehaviour, InterfaceInteractable
{
    [Header("UI Settings")]
    public GameObject documentUIPanel;
    public TextMeshProUGUI documentTextUI;

    [Header("Player movement")]
    public MonoBehaviour playerController;

    private bool isReading = false;

    public void Interact()
    {
        if (isReading) return;

        isReading = true;
        documentUIPanel.SetActive(true);
        documentTextUI.text = SaveSystem.CurrentDocumentText;


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    public void CloseDocument()
    {
        isReading = false;
        documentUIPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    public KeyCode GetInteractKey() => KeyCode.E;
    public string GetInteractText()
    {
        if (isReading)
        {
            return "";
        }
        else
        {
            return "Read note";
        }
    }
}
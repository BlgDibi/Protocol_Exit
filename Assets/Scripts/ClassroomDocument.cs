using UnityEngine;
using TMPro;

public class ClassroomDocument : MonoBehaviour, InterfaceInteractable
{
    [Header("UI Settings")]
    public GameObject documentUIPanel;
    public TextMeshProUGUI documentTextUI;

    [Header("Player movement")]
    public MonoBehaviour playerController;

    [Header("Puzzle Link")]
    public ClassroomPuzzleManager puzzleManager;

    private bool isReading = false;

    private string noteText = "<b>Protocol:Exit - The four acts</b>\n\n Subject: Test Subject #42\r\nSupervisor: The Professor\n\nTo leave this room, you must undo the lessons it taught you. Do not break the chain, or the room will forget your progress.\n\nI. Let the dry metal weep.\r\nII. Turn the green field into a void.\r\nIII. Leave the outside world behind.\r\nIV. Close the master's place.";

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

        if (puzzleManager != null)
        {
            puzzleManager.UnlockPuzzleObjects();
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

    public KeyCode GetInteractKey()
    {
        return KeyCode.F;
    }

    public string GetInteractText()
    {
        if (isReading)
        {
            return "";
        }
        else
        {
            return "Read the riddle";
        }
    }
}
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

    [Header("TV Event Link")]
    public TV_Event tvEvent;

    private bool isReading = false;

    private string noteText = "<b>Protocol:Exit - The four steps</b>\n\nSubject: Test Subject #42\nSupervisor: The Professor\n\nTo leave this room, you must complete four tasks in exact order. Do not break the chain, or you must start again.\n\nI. Make the dry metal cry.\nII. Wipe the green board clean.\nIII. Shut out the outside world.\nIV. Return the master's seat.";
    public void Interact()
    {
        if (isReading) return;

        if (tvEvent != null && !tvEvent.isEventFinished) return;

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
        if (isReading) return "";

        if (tvEvent != null && !tvEvent.isEventFinished) return "";

        return "Read the riddle";
    }
}
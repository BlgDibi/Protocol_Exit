using UnityEngine;

public class Classroom_Board : MonoBehaviour, InterfaceInteractable
{
    public ClassroomPuzzleManager puzzleManager;

    public string GetInteractText()
    {
        if (puzzleManager != null && !puzzleManager.hasReadRiddle) return "";
        return "Erase text";
    }

    public KeyCode GetInteractKey()
    {
        return KeyCode.F;
    }

    public void Interact()
    {
        puzzleManager.InteractWithBoard();
    }
}
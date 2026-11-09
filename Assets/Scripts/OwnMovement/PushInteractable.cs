using UnityEngine;

public class PushInteractable : MonoBehaviour, InterfaceInteractable
{
    [Header("Push Settings")]
    public string promptText = "Push";
    public KeyCode interactKey = KeyCode.V;

    [Header("Classroom Puzzle Link")]
    public ClassroomPuzzleManager puzzleManager;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public string GetInteractText()
    {
        if (puzzleManager != null && !puzzleManager.hasReadRiddle) return "";
        return promptText;
    }

    public KeyCode GetInteractKey()
    {
        return interactKey;
    }

    public void Interact()
    {
        if (puzzleManager != null && !puzzleManager.hasReadRiddle) return;

        OwnMovement player = FindFirstObjectByType<OwnMovement>();

        if (player != null && rb != null)
        {
            player.StartPushing(rb, interactKey);
        }
    }
}
using UnityEngine;

public class PushInteractable : MonoBehaviour, InterfaceInteractable
{
    [Header("Push Settings")]
    public string promptText = "Push";
    public KeyCode interactKey = KeyCode.V;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

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
        OwnMovement player = FindFirstObjectByType<OwnMovement>();

        if (player != null && rb != null)
        {
            player.StartPushing(rb, interactKey);
        }
        else
        {
            if (player == null) Debug.LogError("There is no OwnMovement script found!");
            if (rb == null) Debug.LogError("There is no Rigidbody on the interactable object!");
        }
    }
}
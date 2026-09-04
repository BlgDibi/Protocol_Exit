using UnityEngine;

public class PaintingInteract : MonoBehaviour, InterfaceInteractable
{
    [Header("Painting Settings")]
    [Tooltip("Painting Sprite")]
    public Sprite highResPainting;

    [Header("UI Settings")]
    public string promptText = "Look";
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
        if (PictureViewer.Instance != null)
        {
            PictureViewer.Instance.OpenPicture(highResPainting);
        }
        else
        {
            Debug.LogError("There is no PictureViewer script!");
        }
    }
}
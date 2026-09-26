using UnityEngine;
using UnityEngine.Localization;

public class CabinetDrawer : MonoBehaviour, InterfaceInteractable
{
    public static bool HasCabinetAccess = false;

    [Header("Drawer Settings")]
    public bool isCorrectDrawer = false;
    public bool isStuck = false;
    private bool isOpened = false;

    [Header("Components")]
    public Animator drawerAnimator;
    public AudioSource drawerSound;

    [Tooltip("Item to spawn")]
    public GameObject itemToSpawn;

    [Header("Localization")]
    public LocalizedString interactPrompt;
    public LocalizedString lockedPrompt;
    public LocalizedString closePrompt;

    [SerializeField] private KeyCode interactkey = KeyCode.E;

    public string GetInteractText()
    {
        if (!HasCabinetAccess)
            return lockedPrompt.GetLocalizedString();

        if (isOpened)
        {
            return closePrompt != null && !closePrompt.IsEmpty ? closePrompt.GetLocalizedString() : "Close";
        }
        else
        {
            return interactPrompt.GetLocalizedString();
        }
    }

    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        if (!HasCabinetAccess) return;

        if (isStuck)
        {
            if (drawerSound != null) drawerSound.Play();
            return;
        }

        if (!isOpened)
        {
            isOpened = true;

            if (drawerSound != null) drawerSound.Play();

            if (drawerAnimator != null)
            {
                drawerAnimator.enabled = true;
                drawerAnimator.SetTrigger("Open");
            }

            if (isCorrectDrawer && itemToSpawn != null)
            {
                itemToSpawn.SetActive(true);
            }
        }
        else
        {
            isOpened = false;

            if (drawerSound != null) drawerSound.Play();

            if (drawerAnimator != null)
            {
                drawerAnimator.SetTrigger("Close");
            }
        }
    }
}
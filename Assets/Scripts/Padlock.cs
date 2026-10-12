using System.Collections;
using UnityEngine;

public class Padlock : MonoBehaviour, InterfaceInteractable
{
    [Header("Lock Settings")]
    public Key requiredKey;
    public UniversalDoor protectedDoor;
    public Animator padlockAnimator;

    [Header("Visuals")]
    public GameObject insertedKeyVisual;
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip unlockSound;

    private bool isUnlocked = false;

    private void Start()
    {
        bool alreadyUnlocked = false;

        if (protectedDoor != null && !string.IsNullOrEmpty(protectedDoor.doorID))
        {
            if (SaveSystem.IsDoorUnlocked(protectedDoor.doorID))
            {
                alreadyUnlocked = true;
            }
        }

        if (alreadyUnlocked)
        {
            isUnlocked = true;

            if (insertedKeyVisual != null)
            {
                insertedKeyVisual.SetActive(true);
            }

            if (padlockAnimator != null)
            {
                padlockAnimator.enabled = false;
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        else
        {
            if (insertedKeyVisual != null)
            {
                insertedKeyVisual.SetActive(false);
            }
        }
    }

    public string GetInteractText()
    {
        if (isUnlocked) return "";
        return "Unlock Padlock";
    }

    public KeyCode GetInteractKey()
    {
        return KeyCode.F;
    }

    public void Interact()
    {
        if (isUnlocked) return;

        if (requiredKey != null)
        {
            if (KeyInventory.Instance.HasKey(requiredKey))
            {
                KeyInventory.Instance.RemoveKey(requiredKey);
                StartCoroutine(UnlockSequence());
            }
        }
    }

    private IEnumerator UnlockSequence()
    {
        isUnlocked = true;

        if (insertedKeyVisual != null)
        {
            insertedKeyVisual.SetActive(true);
        }

        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }

        if (padlockAnimator != null)
        {
            padlockAnimator.Play("padlock");
        }

        yield return new WaitForSeconds(0.5f);

        if (protectedDoor != null)
        {
            protectedDoor.UnlockOnly();
        }

        if (padlockAnimator != null)
        {
            padlockAnimator.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(transform.forward * 1.5f, ForceMode.Impulse);
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections;

public class KeyCollectable : MonoBehaviour, InterfaceInteractable
{
    [SerializeField] private Key key;
    [SerializeField] private AudioClip pickupsound;
    [SerializeField] private AudioSource audiosource;

    [Header("Localization")]
    public LocalizedString textPickup;

    [SerializeField] private KeyCode interactionkey = KeyCode.E;

    void Start()
    {
        StartCoroutine(CheckIfAlreadyCollected());
    }

    IEnumerator CheckIfAlreadyCollected()
    {
        yield return new WaitForEndOfFrame();

        if (key == null) yield break;

        List<int> mentettKeys = SaveSystem.GetInventoryKeys();

        if (mentettKeys != null && mentettKeys.Contains(key.id))
        {
            gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        if (key != null)
        {
    
            KeyInventory.Instance.AddKey(key);

            if (pickupsound != null && audiosource != null)
            {
                audiosource.PlayOneShot(pickupsound);
            }

            gameObject.SetActive(false); 
        }
    }

    public string GetInteractText()
    {
        return textPickup.GetLocalizedString();
    }

    public KeyCode GetInteractKey()
    {
        return interactionkey;
    }
}
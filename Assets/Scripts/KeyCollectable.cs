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
        StartCoroutine(CheckStatusAfterLoad());
    }

    IEnumerator CheckStatusAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);
        if (key == null) yield break;

        List<int> Savedkeys = SaveSystem.GetHistoricalKeys();

        if (Savedkeys != null && Savedkeys.Contains(key.id))
        {
            gameObject.SetActive(false);
            yield break;
        }

        if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = true;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = true;
    }

    IEnumerator CheckIfAlreadyCollected()
    {
        yield return new WaitForEndOfFrame();
        if (key == null) yield break;

        List<int> mentettKeys = SaveSystem.GetHistoricalKeys();

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
            SaveSystem.Save(SaveSystem.CurrentSlot);
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
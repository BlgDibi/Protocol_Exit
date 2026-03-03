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
        // Amint aktiválódik, azonnal ellenõrizze magát
        StartCoroutine(CheckStatusAfterLoad());
    }

    IEnumerator CheckStatusAfterLoad()
    {
        // Itt már nem kell sokat várni, mert a Manager már várt
        yield return new WaitForSeconds(0.1f);

        if (key == null) yield break;

        // Dupla ellenõrzés: ha valamiért mégis nálunk lenne, tûnjön el
        List<int> mentettKeys = SaveSystem.GetInventoryKeys();
        if (mentettKeys != null && mentettKeys.Contains(key.id))
        {
            gameObject.SetActive(false);
            yield break;
        }

        // Kényszerítsük a megjelenítést (Renderer és Collider)
        if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = true;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = true;

        Debug.Log("Kulcs: Sikeresen megjelentem a világban!");
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
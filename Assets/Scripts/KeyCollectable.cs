using UnityEngine;
using UnityEngine.Localization;

public class KeyCollectable : MonoBehaviour,InterfaceInteractable
{

    [SerializeField] private Key key;
    [SerializeField] private AudioClip pickupsound;
    [SerializeField] private AudioSource audiosource;
    [Header("Localization")]
    public LocalizedString textPickup;

    public void Interact() 
    {
        if (key != null)
        {
            KeyInventory.Instance.AddKey(key);

            if (pickupsound != null &&audiosource !=null)
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
        return KeyCode.F;
    }

}

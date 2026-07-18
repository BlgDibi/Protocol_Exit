using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Reference")]
    public GameObject flashlightInHand;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.U; 

    void Start()
    {
        if (flashlightInHand == null) return;

        if (Flashlightpickup.HasFlashlight)
        {
            flashlightInHand.SetActive(true);
        }
        else
        {
            flashlightInHand.SetActive(false);
        }
    }

    void Update()
    {
        
        if (!Flashlightpickup.HasFlashlight || flashlightInHand == null) return;

       
        if (Input.GetKeyDown(toggleKey))
        {
         
            bool isCurrentlyOn = flashlightInHand.activeSelf;
            flashlightInHand.SetActive(!isCurrentlyOn);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class KeyPadTrigger : MonoBehaviour
{
 
    public static bool IsPlayerInZone = false;

    [Header("Settings")]
    public KeyPadSystem uiKeypad;
    [SerializeField] private Image crosshair;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyUp(KeyCode.F))
        {
            if (!uiKeypad.keypadCanvas.activeSelf)
            {
                HighlightCrosshair(false);
                uiKeypad.ShowKeypad();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            IsPlayerInZone = true; 
            HighlightCrosshair(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            IsPlayerInZone = false;
            HighlightCrosshair(false);
            uiKeypad.CloseKeypad();
        }
    }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
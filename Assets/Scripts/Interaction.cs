using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interaction : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private Image crosshair;

    [Header("Crosshair Icons")]
    [SerializeField] private Sprite defaultCrosshairSprite;  
    [SerializeField] private Sprite interactCrosshairSprite;

    [SerializeField] private bool changeColorAsWell = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private GameObject interactionPanel;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();

        if (interactionText != null) interactionText.gameObject.SetActive(false);
        if (interactionPanel != null) interactionPanel.SetActive(false);

        if (crosshair != null && defaultCrosshairSprite != null)
            crosshair.sprite = defaultCrosshairSprite;
    }

    void Update()
    {
        PerformRayCast();
    }

    void PerformRayCast()
    {
        Vector3 rayOrigin = _camera.transform.position;
        Vector3 rayDirection = _camera.transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance))
        {
            InterfaceInteractable interactable = hit.collider.GetComponent<InterfaceInteractable>();

            if (interactable != null)
            {
                HighlightCrosshair(true);

                if (interactionText != null)
                {
                    string textToShow = interactable.GetInteractText();
                    KeyCode keyNeeded = interactable.GetInteractKey();

                    interactionText.text = textToShow;

                    if (!string.IsNullOrEmpty(textToShow))
                    {
                        interactionText.gameObject.SetActive(true);
                        if (interactionPanel != null) interactionPanel.SetActive(true);
                    }
                    else
                    {
                        if (interactionPanel != null) interactionPanel.SetActive(false);
                    }

                    if (Input.GetKeyDown(keyNeeded))
                    {
                        interactable.Interact();
                    }
                }
            }
            else
            {
                ClearTarget();
            }
        }
        else
        {
            ClearTarget();
        }
    }

    void ClearTarget()
    {
        if (KeyPadTrigger.IsPlayerInZone) return;

        HighlightCrosshair(false);

        if (interactionText != null) interactionText.gameObject.SetActive(false);
        if (interactionPanel != null) interactionPanel.SetActive(false);
    }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null)
        {
            if (on && interactCrosshairSprite != null)
                crosshair.sprite = interactCrosshairSprite;
            else if (!on && defaultCrosshairSprite != null)
                crosshair.sprite = defaultCrosshairSprite;

            if (changeColorAsWell)
                crosshair.color = on ? Color.red : Color.white;
            else
                crosshair.color = Color.white; 
        }
    }

    private void OnDrawGizmos()
    {
        if (_camera == null) _camera = GetComponent<Camera>();
        if (_camera != null)
        {
            Gizmos.color = Color.orange;
            Vector3 startPos = _camera.transform.position;
            Vector3 direction = _camera.transform.forward * rayDistance;
            Gizmos.DrawRay(startPos, direction);
        }
    }
}
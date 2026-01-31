using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoorInteraction : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float rayDistance = 5;
    [SerializeField] private Image crosshair;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private GameObject interactionPanel;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();

        if (interactionText != null) interactionText.gameObject.SetActive(false);
        if (interactionPanel != null) interactionPanel.SetActive(false);
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

            if (interactionText != null)
            {
                string textToShow = interactable.GetInteractText();
                KeyCode keyNeeded = interactable.GetInteractKey();

                interactionText.text = textToShow;

                if (textToShow != "")
                {
                    interactionText.gameObject.SetActive(true);
                    if (interactionPanel != null) interactionPanel.SetActive(true);
                }

                if (Input.GetKeyDown(keyNeeded))
                {
                    interactable.Interact();
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
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
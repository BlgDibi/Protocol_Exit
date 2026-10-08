using UnityEngine;
using TMPro;

public class BoardViewer : MonoBehaviour
{
    public static BoardViewer Instance { get; private set; }

    [Header("UI References")]
    public GameObject viewerPanel;
    public TextMeshProUGUI displayText;

    [Header("HUD Elements To Hide")]
    public GameObject[] hudElementsToHide;

    [Header("Player Control")]
    public OwnMovement playerController;
    public Interaction playerInteraction;

    private bool isViewing = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        if (viewerPanel != null) viewerPanel.SetActive(false);
    }

    private void Update()
    {
        if (isViewing)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CloseDocument();
            }
        }
    }

    public void OpenDocument(string textContent)
    {
        if (isViewing) return;

        isViewing = true;

        if (displayText != null)
        {
            displayText.text = textContent;
        }

        viewerPanel.SetActive(true);

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(false);
        }

        if (playerInteraction != null) playerInteraction.enabled = false;
        if (playerController != null)
        {
            Rigidbody playerRb = playerController.GetComponent<Rigidbody>();
            if (playerRb != null) playerRb.linearVelocity = Vector3.zero;
            playerController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseDocument()
    {
        if (!isViewing) return;

        isViewing = false;
        viewerPanel.SetActive(false);

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(true);
        }

        if (playerInteraction != null) playerInteraction.enabled = true;
        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
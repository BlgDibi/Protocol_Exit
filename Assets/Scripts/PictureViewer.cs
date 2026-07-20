using UnityEngine;
using UnityEngine.UI;

public class PictureViewer : MonoBehaviour
{
    public static PictureViewer Instance { get; private set; }

    [Header("UI References")]
    public GameObject viewerPanel;
    public Image displayImage;

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
            if (Input.GetKeyDown(KeyCode.E))
            {
                ClosePicture();
            }
        }
    }

    public void OpenPicture(Sprite paintingSprite)
    {
        if (isViewing || paintingSprite == null) return;

        isViewing = true;

        displayImage.sprite = paintingSprite;
        viewerPanel.SetActive(true);

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(false);
        }

        if (playerInteraction != null) playerInteraction.enabled = false;

        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePicture()
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
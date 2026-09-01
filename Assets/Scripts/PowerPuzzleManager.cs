using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerBoxPuzzle : MonoBehaviour, InterfaceInteractable
{
    [Header("UI & Player References")]
    public GameObject puzzleUIPanel;
    public GameObject[] hudElementsToHide;
    public OwnMovement playerController;
    public Interaction playerInteraction;

    [Header("Puzzle Settings")]
    public Toggle[] switches;
    public int basePower = 10;
    public int[] switchValues;

    [Header("Toggle Images")]
    public Sprite switchOffSprite; // 01.png (Lefelé)
    public Sprite switchOnSprite;  // 02.png (Felfelé)

    [Header("Lights (UI Images)")]
    public Image lightGreen;
    public Image lightYellow;
    public Image lightRed;

    [Header("Zones")]
    public int yellowZoneMin = 50;
    public int redZoneMin = 85;

    [Header("Needle")]
    public RectTransform needleTransform;
    public float minZAngle = 90f;
    public float maxZAngle = -90f;

    [HideInInspector]
    public int currentPower = 0;

    private bool isViewing = false;
    public bool isSolved = false;

    private Color offGreen = new Color(0.0f, 0.3f, 0.0f);
    private Color onGreen = Color.green;
    private Color offYellow = new Color(0.4f, 0.4f, 0.0f);
    private Color onYellow = Color.yellow;
    private Color offRed = new Color(0.3f, 0.0f, 0.0f);
    private Color onRed = Color.red;

    private void Start()
    {
        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(false);

        foreach (Toggle sw in switches)
        {
            sw.onValueChanged.AddListener(delegate { CalculatePower(); });
        }
        CalculatePower();
    }

    private void Update()
    {
        if (isViewing && Input.GetKeyDown(KeyCode.E))
        {
            ClosePuzzle();
        }
    }

   
    private void UpdateLights()
    {
        lightGreen.color = offGreen;
        lightYellow.color = offYellow;
        lightRed.color = offRed;

        if (currentPower < yellowZoneMin)
        {
            lightGreen.color = onGreen;
        }
        else if (currentPower >= yellowZoneMin && currentPower < redZoneMin)
        {
            lightYellow.color = onYellow;
        }
        else if (currentPower >= redZoneMin)
        {
            lightRed.color = onRed;
        }
    }

    public void CalculatePower()
    {
        currentPower = basePower;

        for (int i = 0; i < switches.Length; i++)
        {
            Image bgImage = switches[i].targetGraphic as Image;

            if (switches[i].isOn)
            {
                currentPower += switchValues[i];
                if (bgImage != null && switchOnSprite != null) bgImage.sprite = switchOnSprite;
            }
            else
            {
                if (bgImage != null && switchOffSprite != null) bgImage.sprite = switchOffSprite;
            }
        }

        currentPower = Mathf.Clamp(currentPower, 0, 150);

        UpdateLights();
        UpdateNeedle();

        if (currentPower == 100)
        {
            isSolved = true;
            Debug.Log("Rendszer túlterhelve! A mutató kiakadt. NYERTÉL!");
            Invoke("ClosePuzzle", 1.5f);
        }
        else
        {
            isSolved = false;
            CancelInvoke("ClosePuzzle");
        }
    }

    private void UpdateNeedle()
    {
        if (needleTransform != null)
        {
            float percentage = (float)currentPower / 100f;

            float currentZAngle = Mathf.LerpUnclamped(minZAngle, -125f, percentage);

            needleTransform.localRotation = Quaternion.Euler(0f, 0f, currentZAngle);
        }
    }

    public string GetInteractText() => isSolved ? "System Overloaded" : "Open Power Box";
    public KeyCode GetInteractKey() => KeyCode.E;

    public void Interact()
    {
        if (isSolved || isViewing) return;
        OpenPuzzle();
    }

    public void OpenPuzzle()
    {
        if (isViewing || isSolved) return;

        for (int i = 0; i < switches.Length; i++)
        {
            switches[i].isOn = false;
        }
        CalculatePower();
        isViewing = true;
        UiManager.isMenuOpen = true;

        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(true);

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

    public void ClosePuzzle()
    {
        if (!isViewing) return;

        isViewing = false;
        UiManager.isMenuOpen = false;

        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(false);

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
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScalePuzzleController : MonoBehaviour, InterfaceInteractable
{
    public static ScalePuzzleController Instance { get; private set; }

    [Header("UI & Camera References")]
    public GameObject viewerPanel;
    public Camera mainPlayerCamera;
    public Camera puzzleCamera;

    [Header("UI Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("HUD Elements To Hide")]
    public GameObject[] hudElementsToHide;

    [Header("Player Control")]
    public OwnMovement playerController;
    public Interaction playerInteraction;

    [Header("Scale 3D Parts")]
    public Transform leftPan;
    public Transform rightPan;
    public Transform needle;

    [Header("Settings")]
    public float rotationSpeed = 2f;
    public float winningDelta = 0f;

    private bool isViewing = false;
    public bool isSolved = false;

    private float leftWeight = 0f;
    private float rightWeight = 0f;
    private float targetAngle = 0f;

    private List<GameObject> spawnedBoxes = new List<GameObject>();
    private List<SulyDobozData> spawnedBoxesData = new List<SulyDobozData>();

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (viewerPanel != null && viewerPanel != gameObject) viewerPanel.SetActive(false);
        if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("ScaleSolved"))
        {
            isSolved = true;
            targetAngle = 0f;
            if (needle != null) needle.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        GameObject[] savedBoxes = GameObject.FindGameObjectsWithTag("Boxes");
        foreach (GameObject box in savedBoxes)
        {
            if (box.name.Contains("_Placed") && !spawnedBoxes.Contains(box))
            {
                spawnedBoxes.Add(box);

                Rigidbody rb = box.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float distLeft = Vector3.Distance(box.transform.position, leftPan.position);
                    float distRight = Vector3.Distance(box.transform.position, rightPan.position);

                    if (distLeft < distRight) leftWeight += rb.mass;
                    else rightWeight += rb.mass;
                }

                Boxpickup pickupScript = box.GetComponent<Boxpickup>();
                if (pickupScript != null)
                {
                    if (pickupScript.Boxdata != null)
                    {
                        spawnedBoxesData.Add(pickupScript.Boxdata);
                    }

                    if (isSolved)
                    {
                        pickupScript.enabled = false;

                        if (rb != null)
                        {
                            rb.isKinematic = true;
                        }
                    }
                }
            }
        }

        if (!isSolved)
        {
            CalculateScaleState();
        }
    }

    private void Update()
    {
        if (needle != null && (!isSolved || (isSolved && needle.localRotation != Quaternion.Euler(0f, 0f, targetAngle))))
        {
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetAngle);
            needle.localRotation = Quaternion.Lerp(needle.localRotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        if (isViewing)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ResetScale();
                ClosePuzzle();
            }
        }
    }

    public string GetInteractText()
    {
        if (isSolved == true)
        {
            return "The scale is balanced.";
        }
        else
        {
            return "Examine Scale";
        }
    }
    public KeyCode GetInteractKey() => KeyCode.F;

    public void Interact()
    {
        if (isSolved || isViewing) return;
        OpenPuzzle();
    }

    public void OpenPuzzle()
    {
        if (isViewing || isSolved) return;

        isViewing = true;
        UiManager.isMenuOpen = true;

        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(false);
        if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(true);
        if (viewerPanel != null) viewerPanel.SetActive(true);

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(false);
        }

        if (playerInteraction != null) playerInteraction.enabled = false;
        if (playerController != null)
        {
            Rigidbody playerRb = playerController.GetComponent<Rigidbody>();
            if (playerRb != null) playerRb.linearVelocity = Vector3.zero;

            AudioSource playerAudio = playerController.GetComponent<AudioSource>();
            if (playerAudio != null) playerAudio.Stop();

            Animator playerAnim = playerController.GetComponentInChildren<Animator>();
            if (playerAnim != null)
            {
                playerAnim.Rebind();
                playerAnim.Update(0f);
            }

            playerController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

       
        if (leftWeight == 0f && rightWeight == 0f)
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Place boxes on the scale.";
                feedbackText.color = Color.white;
            }
        }
        else
        {
            CalculateScaleState();
        }
    }

    public void ClosePuzzle()
    {
        if (!isViewing) return;

        isViewing = false;
        UiManager.isMenuOpen = false;

        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(true);
        if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(false);
        if (viewerPanel != null) viewerPanel.SetActive(false);

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(true);
        }

        if (playerInteraction != null) playerInteraction.enabled = true;
        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PlaceBox(bool isLeft)
    {
        if (BoxInventory.Instance.HasAnyBox() == false)
        {
            return;
        }

        SulyDobozData data = BoxInventory.Instance.GetFirstBox();

        if (data != null && data.boxprefab != null)
        {
            BoxInventory.Instance.RemoveBox(data);

            Transform targetPan;
            if (isLeft == true)
            {
                targetPan = leftPan;
            }
            else
            {
                targetPan = rightPan;
            }

            float randomX = Random.Range(-0.35f, 0.35f);
            float randomZ = Random.Range(-0.35f, 0.35f);
            Vector3 spawnOffset = new Vector3(randomX, 0.7f, randomZ);

            GameObject box = Instantiate(data.boxprefab, targetPan.position + spawnOffset, Quaternion.identity);
            box.SetActive(true);
            box.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            box.transform.rotation = Quaternion.Euler(Random.Range(10f, 30f), Random.Range(0f, 360f), Random.Range(10f, 30f));

            box.name = data.boxname + "_Placed";
            box.tag = "Boxes";

            Boxpickup pickupScript = box.GetComponent<Boxpickup>();

            if (pickupScript != null)
            {
                pickupScript.isPlaced = true;
            }

            Rigidbody rb = box.GetComponent<Rigidbody>();

            if (rb == null)
            {
                rb = box.AddComponent<Rigidbody>();
            }

            rb.mass = data.weight;

            spawnedBoxes.Add(box);
            spawnedBoxesData.Add(data);

            if (isLeft == true)
            {
                leftWeight += data.weight;
            }
            else
            {
                rightWeight += data.weight;
            }

            CalculateScaleState();
        }
    }

    private void CalculateScaleState()
    {

        if (isSolved) return;

        float deltaWeight = rightWeight - leftWeight;
        targetAngle = Mathf.Clamp(deltaWeight * -10f, -60f, 60f);

        if (feedbackText != null)
        {
            if (Mathf.Approximately(deltaWeight, winningDelta) && spawnedBoxes.Count > 0) 
            {
                feedbackText.text = "Perfectly balanced!";
                feedbackText.color = Color.green;
                StartCoroutine(WinRoutine());
            }
            else
            {
                feedbackText.text = $"Left: {leftWeight} kg  |  Right: {rightWeight} kg";
                feedbackText.color = Color.white;
            }
        }
        else if (Mathf.Approximately(deltaWeight, winningDelta) && spawnedBoxes.Count > 0)
        {
            StartCoroutine(WinRoutine());
        }
    }

    public void ResetScale()
    {
        if (isSolved) return;

        foreach (SulyDobozData data in spawnedBoxesData)
        {
            if (data != null) BoxInventory.Instance.AddBox(data);
        }

        foreach (GameObject box in spawnedBoxes)
        {
            if (box != null) Destroy(box);
        }

        spawnedBoxes.Clear();
        spawnedBoxesData.Clear();
        leftWeight = 0f;
        rightWeight = 0f;
        targetAngle = 0f;

        if (feedbackText != null)
        {
            feedbackText.text = "Scale reset. Boxes returned.";
            feedbackText.color = Color.white;
        }
    }

    private IEnumerator WinRoutine()
    {
        isSolved = true;
        ConditionManager.Instance.SetCondition("ScaleSolved", true);

        yield return new WaitForSeconds(1.5f);

        foreach (GameObject box in spawnedBoxes)
        {
            if (box != null)
            {
                Boxpickup pickupScript = box.GetComponent<Boxpickup>();
                if (pickupScript != null)
                {
                    pickupScript.enabled = false;
                }
            }
        }


        yield return new WaitForSeconds(2.0f);
        ClosePuzzle();
    }
}
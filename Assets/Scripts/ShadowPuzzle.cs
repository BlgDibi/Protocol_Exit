using UnityEngine;
using System.Collections;

public class ShadowPuzzle : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public Vector3 solutionRotation;
    public float marginOfError = 5f;
    public float sensitivity = 100f;

    [Header("Camera and Player")]
    public GameObject puzzleCameraObj;
    public GameObject playerObj;
    public GameObject playerCameraObj;
    public GameObject playerVisuals;

    [Header("Outcome")]
    public Light successLight;
    public Color solvedColor = Color.green;

    [Header("Lever")]
    public Lever_PowerPuzzle powerLever;

    [Header("Quad")]
    public Renderer screenRenderer;
    private Color originalMaterialColor;

    private bool isSolved = false;
    private bool puzzleActive = false;

    [Header("WeakLights")]
    public Light[] weakPowerLights;

    void Start()
    {
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);

        if (screenRenderer != null)
        {
            originalMaterialColor = screenRenderer.material.color;

            screenRenderer.material.color = Color.black;
        }
    }

    public void Interact()
    {
        if (isSolved) return;

        if (powerLever != null && !powerLever.isPulled)
        {
            StartCoroutine(WeakPowerFeedback());
            return;
        }

        if (screenRenderer != null)
        {
            screenRenderer.material.color = originalMaterialColor;
        }

        if (!puzzleActive)
            EnterPuzzle();
        else
            ExitPuzzle();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !puzzleActive)
        {
            float dist = Vector3.Distance(playerObj.transform.position, transform.position);
            if (dist < 3f)
            {
                Interact();
            }
        }

        if (!puzzleActive || isSolved) return;

        if (Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotX, Space.World);

            CheckIfSolved();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPuzzle();
        }
    }

    void EnterPuzzle()
    {
        if (playerVisuals != null) playerVisuals.SetActive(false);
        puzzleActive = true;

        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(true);

        if (playerObj != null)
        {
            var controller = playerObj.GetComponent<MonoBehaviour>();
            if (controller != null) controller.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitPuzzle()
    {
        if (playerVisuals != null) playerVisuals.SetActive(true);
        puzzleActive = false;

        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);
        if (playerCameraObj != null) playerCameraObj.SetActive(true);

        if (playerObj != null)
        {
            var controller = playerObj.GetComponent<MonoBehaviour>();
            if (controller != null) controller.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void CheckIfSolved()
    {
        float currentY = transform.eulerAngles.y;
        float targetY = solutionRotation.y;

        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentY, targetY));

        if (angleDiff < marginOfError && !isSolved)
        {
            StartCoroutine(SolvedSequence());
        }
    }

    IEnumerator SolvedSequence()
    {
        isSolved = true;
        puzzleActive = false;

        if (successLight != null)
        {
            successLight.color = solvedColor;
            successLight.enabled = true;
        }

        Debug.Log("Siker! Várakozás...");

        yield return new WaitForSeconds(2.0f);

        CompleteAndExit();
    }

    void CompleteAndExit()
    {
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);
        if (playerCameraObj != null) playerCameraObj.SetActive(true);

        if (playerObj != null)
        {
            playerObj.GetComponent<MonoBehaviour>().enabled = true;
            if (playerVisuals != null) playerVisuals.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator WeakPowerFeedback()
    {
        Debug.Log("<color=yellow>A fények halványan pislákolnak... Nincs elég áram a fõkapcsoló miatt!</color>");

        if (weakPowerLights != null && weakPowerLights.Length > 0)
        {
            float[] originalIntensities = new float[weakPowerLights.Length];
            for (int i = 0; i < weakPowerLights.Length; i++)
            {
                if (weakPowerLights[i] != null)
                    originalIntensities[i] = weakPowerLights[i].intensity;
            }

            for (int step = 0; step < 3; step++)
            {
                foreach (Light l in weakPowerLights)
                {
                    if (l != null)
                    {
                        l.enabled = true;
                        l.intensity = l.intensity * 0.15f;
                    }
                }
                yield return new WaitForSeconds(0.08f);

                foreach (Light l in weakPowerLights)
                {
                    if (l != null) l.enabled = false;
                }
                yield return new WaitForSeconds(0.12f);
            }

            for (int i = 0; i < weakPowerLights.Length; i++)
            {
                if (weakPowerLights[i] != null)
                {
                    weakPowerLights[i].enabled = true;
                    weakPowerLights[i].intensity = originalIntensities[i] * 0.05f;
                }
            }
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < weakPowerLights.Length; i++)
            {
                if (weakPowerLights[i] != null)
                {
                    weakPowerLights[i].enabled = false;
                    weakPowerLights[i].intensity = originalIntensities[i];
                }
            }
        }
    }
}
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Projector_ShadowPuzzle : MonoBehaviour, InterfaceInteractable
{
    [Header("Lever")]
    public Lever_PowerPuzzle powerLever;

    [Header("Quad")]
    public Renderer screenRenderer;
    private Color originalMaterialColor;

    [Header("Weak Lights")]
    public Light[] weakPowerLights;

    public bool isTurnedOn = false;

    [Header("Player")]
    public GameObject playerObj;

    [Header("Save System")]
    public string saveID = "Projector_ShadowPuzzle_01";

    [Header("ProjectorReels - Rotation")]
    public Transform[] projectorReels;
    public float normalRotationSpeed = 150f;
    public float weakRotationSpeed = 30f;
    public Vector3 rotationAxis = new Vector3(0, 0, 1);

    [Header("Audio Files")]
    public AudioClip noPowerTrySound;
    public AudioClip startupSound;
    public AudioClip loopingSound;

    [Header("Custom Loop Settings")]
    [Tooltip("Startup sound duration")]
    public float startupDuration = 8f;

    [Tooltip("Loop Sound start time")]
    public float customLoopStart = 6f;

    [Tooltip("Loop Sound end time")]
    public float customLoopEnd = 15f;

    private AudioSource audioSource;
    private bool isFlashing = false;
    private bool isStartingUp = false;

    private bool isLoopingPhase = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (screenRenderer != null)
        {
            originalMaterialColor = screenRenderer.material.color;
            screenRenderer.material.color = Color.black;
        }

        if (SaveSystem.IsPuzzleSolved(saveID))
        {
            isTurnedOn = true;
            if (screenRenderer != null)
            {
                screenRenderer.material.color = Color.white;
            }
            if (weakPowerLights != null)
            {
                foreach (Light l in weakPowerLights)
                {
                    if (l != null) l.gameObject.SetActive(true);
                }
            }

            if (loopingSound != null)
            {
                isLoopingPhase = true;
                audioSource.clip = loopingSound;
                audioSource.time = customLoopStart; 
                audioSource.loop = false;
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        float currentSpeed = 0f;

        if (isTurnedOn)
        {
            currentSpeed = normalRotationSpeed;
        }
        else if (isFlashing)
        {
            currentSpeed = weakRotationSpeed;
        }

        if (currentSpeed > 0f && projectorReels != null)
        {
            foreach (Transform reel in projectorReels)
            {
                if (reel != null)
                {
                    reel.Rotate(rotationAxis * currentSpeed * Time.deltaTime, Space.Self);
                }
            }
        }

        if (isLoopingPhase && audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                if (audioSource.time >= customLoopEnd)
                {
                    audioSource.time = customLoopStart;
                }
            }
            else
            {
                audioSource.time = customLoopStart;
                audioSource.Play();
            }
        }
    }

    public string GetInteractText()
    {
        if (isTurnedOn || isStartingUp)
            return "Projector is running";
        else
            return "Power On";
    }

    public KeyCode GetInteractKey() => KeyCode.F;

    public void Interact()
    {
        if (isTurnedOn || isFlashing || isStartingUp) return;

        if (powerLever != null && !powerLever.isPulled)
        {
            StartCoroutine(WeakPowerFeedback());
            return;
        }

        StartCoroutine(PowerOnSequence());
    }

    IEnumerator WeakPowerFeedback()
    {
        isFlashing = true;

        float waitTime = 0.5f;

        if (noPowerTrySound != null)
        {
            audioSource.PlayOneShot(noPowerTrySound);
            waitTime = noPowerTrySound.length;
        }

        if (weakPowerLights != null && weakPowerLights.Length > 0)
        {
            float[] originalIntensities = new float[weakPowerLights.Length];
            for (int i = 0; i < weakPowerLights.Length; i++)
            {
                if (weakPowerLights[i] != null) originalIntensities[i] = weakPowerLights[i].intensity;
            }

            if (screenRenderer != null) screenRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);

            for (int i = 0; i < weakPowerLights.Length; i++)
            {
                if (weakPowerLights[i] != null)
                {
                    weakPowerLights[i].gameObject.SetActive(true);
                    weakPowerLights[i].intensity = originalIntensities[i] * 1.5f;
                }
            }

            yield return new WaitForSeconds(waitTime);

            if (screenRenderer != null) screenRenderer.material.color = Color.black;

            for (int i = 0; i < weakPowerLights.Length; i++)
            {
                if (weakPowerLights[i] != null)
                {
                    weakPowerLights[i].gameObject.SetActive(false);
                    weakPowerLights[i].intensity = originalIntensities[i];
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        isFlashing = false;
    }

    IEnumerator PowerOnSequence()
    {
        isStartingUp = true;
        isTurnedOn = true;

        if (screenRenderer != null)
        {
            screenRenderer.material.color = Color.white;
        }

        if (weakPowerLights != null)
        {
            foreach (Light l in weakPowerLights)
            {
                if (l != null) l.gameObject.SetActive(true);
            }
        }

        SaveSystem.SavePuzzleSolved(saveID);

        if (startupSound != null)
        {
            audioSource.clip = startupSound;
            audioSource.loop = false; 
            audioSource.Play();

            yield return new WaitForSeconds(startupDuration);
        }

        isLoopingPhase = true;

        if (loopingSound != null && startupSound != loopingSound)
        {
            audioSource.clip = loopingSound;
            audioSource.time = customLoopStart;
            audioSource.Play();
        }

        isStartingUp = false;
    }
}
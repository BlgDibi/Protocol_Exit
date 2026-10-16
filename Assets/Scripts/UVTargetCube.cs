using UnityEngine;
using TMPro;

public class UVTargetCube : MonoBehaviour
{
    [Header("Target Text")]
    public TextMeshPro targetText;

    [Header("UV Lamp Settings")]
    public Transform flashlightTransform;
    public float maxDistance = 8f;
    public float lightAngle = 35f;
    public float fadeSpeed = 5f;

    [Header("Uv Glow Color")]
    [ColorUsage(true, true)]
    public Color uvGlowColor = new Color(0f, 1f, 0.2f, 1f);

    [Header("Puzzle Requirement")]
    public Projector_ShadowPuzzle requiredProjector;

    private float currentAlpha = 0f;
    private FlashlightController flashlightController;

    private void Start()
    {
        flashlightController = FindFirstObjectByType<FlashlightController>();

        if (targetText != null)
        {
            targetText.color = new Color(uvGlowColor.r, uvGlowColor.g, uvGlowColor.b, 0f);
        }
    }

    private void Update()
    {
        if (flashlightTransform == null || targetText == null || flashlightController == null) return;

        
        if (requiredProjector != null && !requiredProjector.isTurnedOn)
        {
            FadeOutText();
            return;
        }

        bool isHit = false;
        bool isFlashlightOn = flashlightController.flashlightInHand != null && flashlightController.flashlightInHand.activeSelf;
        bool isUVModeActive = flashlightController.isUVMode;

        if (isFlashlightOn && isUVModeActive)
        {
            float distance = Vector3.Distance(flashlightTransform.position, transform.position);

            if (distance <= maxDistance)
            {
                Vector3 directionToCube = (transform.position - flashlightTransform.position).normalized;
                float angle = Vector3.Angle(flashlightTransform.forward, directionToCube);

                if (angle <= lightAngle)
                {
                    isHit = true;
                }
            }
        }

        float targetAlpha = isHit ? 1f : 0f;
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        targetText.color = new Color(uvGlowColor.r, uvGlowColor.g, uvGlowColor.b, currentAlpha);
    }

    private void FadeOutText()
    {
        currentAlpha = Mathf.Lerp(currentAlpha, 0f, Time.deltaTime * fadeSpeed);
        targetText.color = new Color(uvGlowColor.r, uvGlowColor.g, uvGlowColor.b, currentAlpha);
    }
}
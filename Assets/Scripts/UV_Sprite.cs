using UnityEngine;

public class UV_Sprite : MonoBehaviour
{
    [Header("UV Lamp Settings")]
    public Transform flashlightTransform;
    public float maxDistance = 6f;
    public float lightAngle = 35f;
    public float fadeSpeed = 5f;

    [Header("UV Glow Color")]
    [ColorUsage(true, true)]
    public Color uvGlowColor = new Color(0f, 1f, 0.2f, 1f);

    private float currentAlpha = 0f;
    private FlashlightController flashlightController;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        flashlightController = FindFirstObjectByType<FlashlightController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(uvGlowColor.r, uvGlowColor.g, uvGlowColor.b, 0f);
        }
    }

    private void Update()
    {
        if (flashlightTransform == null || spriteRenderer == null || flashlightController == null) return;

        bool isHit = false;

        bool isFlashlightOn = flashlightController.flashlightInHand != null && flashlightController.flashlightInHand.activeSelf;
        bool isUVModeActive = flashlightController.isUVMode;

        if (isFlashlightOn && isUVModeActive)
        {
            float distance = Vector3.Distance(flashlightTransform.position, transform.position);

            if (distance <= maxDistance)
            {
                Vector3 directionToSprite = (transform.position - flashlightTransform.position).normalized;
                float angle = Vector3.Angle(flashlightTransform.forward, directionToSprite);

                if (angle <= lightAngle)
                {
                    isHit = true;
                }
            }
        }

        float targetAlpha;

        if (isHit == true)
        {
            targetAlpha = 1.0f;
        }
        else
        {
            targetAlpha = 0.0f;
        }

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        spriteRenderer.color = new Color(uvGlowColor.r, uvGlowColor.g, uvGlowColor.b, currentAlpha);
    }
}
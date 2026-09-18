using UnityEngine;
using TMPro;

public class BookClueGenerator : MonoBehaviour
{
    [Header("Text Display")]
    [Tooltip("Text Display")]
    public TextMeshPro clueTextDisplay;

    [Header("UV Light Settings")]
    [Tooltip("Flashlight Light Component")]
    public Transform flashlightTransform; 
    public float maxDistance = 5f;
    public float lightAngle = 30f;
    public float fadeSpeed = 5f;

    [ColorUsage(true, true)]
    public Color uvGlowColor = new Color(0f, 1f, 0.2f, 1f); 

    private float currentAlpha = 0f;
    private FlashlightController flashlightController; 

    private void Start()
    {
        flashlightController = FindFirstObjectByType<FlashlightController>();

        if (string.IsNullOrEmpty(SaveSystem.KeypadFullCode))
        {
            GenerateCode();
        }

        if (clueTextDisplay != null)
        {
            clueTextDisplay.text = SaveSystem.KeypadClue;

            clueTextDisplay.color = new Color(uvGlowColor.r, uvGlowColor.g, uvGlowColor.b, 0f);
        }
    }

    private void Update()
    {
        if (flashlightTransform == null || clueTextDisplay == null || flashlightController == null) return;

        bool isHit = false;

        bool isFlashlightOn = flashlightController.flashlightInHand != null && flashlightController.flashlightInHand.activeSelf;
        bool isUVModeActive = flashlightController.isUVMode;

        if (isFlashlightOn && isUVModeActive)
        {
            float distance = Vector3.Distance(flashlightTransform.position, clueTextDisplay.transform.position);

            if (distance <= maxDistance)
            {
                Vector3 directionToText = (clueTextDisplay.transform.position - flashlightTransform.position).normalized;
                float angle = Vector3.Angle(flashlightTransform.forward, directionToText);

                if (angle <= lightAngle)
                {
                    isHit = true; 
                }
            }
        }

        float targetAlpha = isHit ? 1f : 0f;

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        clueTextDisplay.color = new Color(uvGlowColor.r, uvGlowColor.g, uvGlowColor.b, currentAlpha);
    }

    private void GenerateCode()
    {
        int[] digits = new int[4];
        int pattern = Random.Range(0, 4);
        string documentTitle = "";
        string documentText = "";

        int hiddenIndex = Random.Range(0, 4);

        if (pattern == 0) 
        {
            digits[0] = Random.Range(1, 6);
            for (int i = 1; i < 4; i++) 
                digits[i] = digits[i - 1] + 1;

            documentTitle = "Journal Entry - The Girl";
            documentText = "She is methodical. She never skips a room, always moving to the very next door down the hall. Every step is exactly one higher than the last. If you know where she was, you know where she is going.";
        }
        else if (pattern == 1)
        {
            digits[0] = Random.Range(1, 4);
            for (int i = 1; i < 4; i++) 
                digits[i] = digits[i - 1] + 2;

            documentTitle = "Old Maintenance Note";
            documentText = "The mechanism is jammed. To keep it turning, the dial must jump forward by exactly two clicks every single time. Any other sequence will lock it permanently.";
        }
        else if (pattern == 2)
        {
            digits[0] = Random.Range(1, 9);
            digits[1] = 9 - digits[0];
            digits[2] = digits[0];
            digits[3] = digits[1];

            documentTitle = "Torn Diary Page";
            documentText = "The reflection is flawed but consistent. The first and third marks are identical. The second and fourth match as well. Together, the first pair always totals exactly nine.";
        }
        else
        {
            digits[0] = Random.Range(4, 10);
            for (int i = 1; i < 4; i++) 
                digits[i] = digits[i - 1] - 1;

            documentTitle = "Doctor's Note";
            documentText = "The protocol is a strict countdown. With each step, the value drops by exactly one. Do not break the descending sequence, or the lock will reset forever.";
        }

        SaveSystem.KeypadFullCode = $"{digits[0]}{digits[1]}{digits[2]}{digits[3]}";

        string[] clueParts = new string[4];
        for (int i = 0; i < 4; i++)
        {
            clueParts[i] = (i == hiddenIndex) ? "X" : digits[i].ToString();
        }

        SaveSystem.KeypadClue = $"{clueParts[0]}   {clueParts[1]}   {clueParts[2]}   {clueParts[3]}";

        SaveSystem.CurrentDocumentText = $"<b>{documentTitle}</b>\n\n{documentText}";
    }
}
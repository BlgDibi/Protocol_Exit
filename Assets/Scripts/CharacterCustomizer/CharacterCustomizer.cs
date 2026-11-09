using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CharacterCustomizer : MonoBehaviour
{
    [Header("Character Modell")]
    public Renderer[] characterRenderers;
    public Transform characterPreviewTransform;

    [Header("Clothes")]
    public Texture[] characterSkins;

    [Header("Name Settings")]
    public TMP_InputField nameInputField;

    [Header("Size Settings")]
    public Slider heightSlider;
    public Slider widthSlider;

    public MainMenu mainMenuScript;

    private int currentIndex = 0;
    private Vector3 initialPreviewScale = Vector3.one;

    void Start()
    {
        UpdateSkinDisplay();

        if (nameInputField == null)
        {
            nameInputField = GameObject.FindFirstObjectByType<TMP_InputField>();
        }

        if (mainMenuScript == null)
        {
            mainMenuScript = GameObject.FindFirstObjectByType<MainMenu>();
        }

        if (characterPreviewTransform != null)
        {
            initialPreviewScale = characterPreviewTransform.localScale;
        }
    }

    void Update()
    {
        if (characterPreviewTransform != null && heightSlider != null && widthSlider != null)
        {
            float w = widthSlider.value;
            float h = heightSlider.value;
            characterPreviewTransform.localScale = new Vector3(
                initialPreviewScale.x * w,
                initialPreviewScale.y * h,
                initialPreviewScale.z * w
            );
        }
    }

    public void NextSkin()
    {
        currentIndex++;
        if (currentIndex >= characterSkins.Length)
        {
            currentIndex = 0;
        }
        UpdateSkinDisplay();
    }

    public void PrevSkin()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characterSkins.Length - 1;
        }
        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay()
    {
        if (characterSkins.Length == 0)
        {
            return;
        }

        Texture newTexture = characterSkins[currentIndex];

        foreach (Renderer r in characterRenderers)
        {
            if (r != null)
            {
                r.material.mainTexture = newTexture;
                r.material.SetTexture("_BaseMap", newTexture);
            }
        }
    }

    public void AcceptSelection()
    {
        SaveSystem.SelectedSkinIndex = currentIndex;

        if (heightSlider != null && widthSlider != null)
        {
            SaveSystem.PlayerHeight = heightSlider.value;
            SaveSystem.PlayerWidth = widthSlider.value;
        }

        if (nameInputField == null)
        {
            nameInputField = GameObject.FindFirstObjectByType<TMP_InputField>();
        }

        if (nameInputField != null)
        {
            string typedName = nameInputField.text.Trim();

            string[] forbiddenNames = {
                "péter", "tamás", "gábor", "bence", "andrás", "zoltán", "lászló", "istván", "jános", "attila",
                "john", "michael", "david", "james", "robert", "william", "joseph", "thomas", "charles", "daniel",
                "dániel", "pisti", "pista", "laci", "dávid", "róbert", "zalán", "páter", "peti", "ákos", "tomi"
            };

            bool isForbidden = false;
            foreach (string badName in forbiddenNames)
            {
                if (typedName.ToLower() == badName)
                {
                    isForbidden = true;
                    break;
                }
            }

            if (string.IsNullOrEmpty(typedName) == true || isForbidden == true)
            {
                StartCoroutine(ShowNameErrorRoutine());
                return;
            }

            SaveSystem.PlayerName = typedName;

            if (mainMenuScript != null)
            {
                mainMenuScript.ExecuteFreshStart();
            }
            else
            {
                SceneManager.LoadScene("firstroom");
            }
        }
        else
        {
            SaveSystem.PlayerName = "Player";

            if (mainMenuScript != null)
            {
                mainMenuScript.ExecuteFreshStart();
            }
            else
            {
                SceneManager.LoadScene("firstroom");
            }
        }
    }

    private IEnumerator ShowNameErrorRoutine()
    {
        if (nameInputField == null || nameInputField.placeholder == null)
        {
            yield break;
        }

        nameInputField.text = "";

        TMP_Text placeholderText = nameInputField.placeholder.GetComponent<TMP_Text>();
        if (placeholderText != null)
        {
            placeholderText.text = "Only girl names!";

            Color errorColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
            placeholderText.color = errorColor;

            if (nameInputField.textComponent != null)
            {
                nameInputField.textComponent.color = Color.white;
            }

            yield return new WaitForSeconds(2.0f);

            placeholderText.text = "Enter your name...";

            Color normalPlaceholderColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            placeholderText.color = normalPlaceholderColor;
        }
    }
}
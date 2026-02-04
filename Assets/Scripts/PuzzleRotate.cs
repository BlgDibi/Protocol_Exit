using UnityEngine;
using UnityEngine.UI;

public class PuzzleRotate : MonoBehaviour
{
    [Header("Settings")]
    public float correctRotation; // Mi a helyes szög? (0, 90, 180, 270)
    public Sprite offSprite;      // A sötét kép
    public Sprite onSprite;       // A világító kép

    // Ezt látják mások is (hogy jó-e a csõ)
    [HideInInspector] public bool isCorrect = false;

    private Image myImage;
    private float currentAngle;

    void Awake()
    {
        myImage = GetComponent<Image>();
        // Véletlenszerû forgatás az elején (hogy ne legyen kész rögtön)
        // Ha fix kezdést akarsz, ezt a sort töröld ki:
        currentAngle = 90 * Random.Range(0, 4);
        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

        UpdateVisuals();
    }

    // Ezt hívja meg a Gomb (Button), amikor rákattintasz
    public void OnClickRotate()
    {
        currentAngle -= 90; // Forgatás balra
        if (currentAngle <= -360) currentAngle = 0;

        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

        UpdateVisuals();

        // Szólunk a Fõnöknek, hogy ellenõrizze a gyõzelmet
        FindObjectOfType<PuzzleManager>().CheckWin();
    }

    void UpdateVisuals()
    {
        // Normalizáljuk a szöget (hogy 0 és 360 között legyen)
        float z = transform.localEulerAngles.z;

        // Ellenõrizzük, hogy egyezik-e a helyes szöggel (kis tûréshatárral)
        if (Mathf.Abs(Mathf.DeltaAngle(z, correctRotation)) < 1)
        {
            isCorrect = true;
            myImage.sprite = onSprite; // VÁLTÁS VILÁGÍTÓRA
        }
        else
        {
            isCorrect = false;
            myImage.sprite = offSprite; // VISSZA SÖTÉTRE
        }
    }
}
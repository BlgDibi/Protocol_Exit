using UnityEngine;
using UnityEngine.UI; // Fontos a UI elemek miatt!

public class Pipe : MonoBehaviour
{
    [Header("Kötelezõ elemek")]
    // IDE húzd be a gyerek objektumot (Visual) az Inspectorban!
    public Image pipeImage;

    [Header("Beállítások")]
    public Sprite offSprite; // Sötét kép
    public Sprite onSprite;  // Világító kép

    // Milyen szögekben jó a csõ? (Pl. egyenesnél: 0, 180)
    public float[] correctRotations;

    // Ezt látja a PuzzleManager (hogy jó-e a csõ)
    [HideInInspector] public bool isCorrect = false;

    private float currentRotation = 0;

    void Awake()
    {
        // 1. Ellenõrzés: Behúztad a képet?
        if (pipeImage == null)
        {
            Debug.LogError("HIBA! Nem húztad be a Pipe Image-t az Inspectorban ezen: " + gameObject.name);
            return;
        }

        // 2. Véletlen kezdõ forgatás (0, 90, 180, 270)
        int rand = Random.Range(0, 4);
        currentRotation = rand * 90;

        // A SLOT-ot forgatjuk (magunkat)
        transform.localRotation = Quaternion.Euler(0, 0, currentRotation);

        // 3. Ellenõrizzük az állapotot rögtön az elején
        CheckMyRotation();
    }

    // Ezt a függvényt kell beállítanod a BUTTON OnClick()-jéhez!
    public void RotatePipe()
    {
        // 90 fokot vonunk le (balra forgás)
        currentRotation -= 90;

        // Matek rendrakás, hogy ne legyen -360 alatt
        if (currentRotation <= -360) currentRotation += 360;

        // A fizikai forgatás végrehajtása a Sloton
        transform.localRotation = Quaternion.Euler(0, 0, currentRotation);

        // Ellenõrzés: Jó helyen állunk most?
        CheckMyRotation();

        // Szólunk a Fõnöknek (PuzzleManager), hogy nézze meg, nyertünk-e
        PuzzleManager manager = FindObjectOfType<PuzzleManager>();
        if (manager != null)
        {
            manager.CheckWin();
        }
        else
        {
            Debug.LogWarning("Nem találom a PuzzleManagert a pályán!");
        }
    }

    void CheckMyRotation()
    {
        // Normalizáljuk a szöget 0-360 közé a biztonság kedvéért
        // (A transform.localEulerAngles.z mindig 0..360 értéket ad)
        float z = Mathf.Round(transform.localEulerAngles.z);
        if (z >= 360) z = 0;

        isCorrect = false;

        // Végignézzük az összes elfogadható szöget (correctRotations tömb)
        foreach (float angle in correctRotations)
        {
            // Ha a jelenlegi szög (z) egyezik a helyessel (pici eltérés belefér)
            if (Mathf.Abs(Mathf.DeltaAngle(z, angle)) < 1f)
            {
                isCorrect = true;
                break;
            }
        }

        // Kép frissítése (Világít vagy sem)
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (pipeImage == null) return;

        if (isCorrect)
        {
            pipeImage.sprite = onSprite;
        }
        else
        {
            pipeImage.sprite = offSprite;
        }
    }
}
using UnityEngine;

public class PuzzleInteract : MonoBehaviour
{
    public GameObject puzzleCanvas; // A UI Canvas

    private bool isPlayerInZone = false;
    private bool isOpen = false;

    void Start()
    {
        puzzleCanvas.SetActive(false); // Biztos, ami biztos, induláskor legyen zárva
    }

    void Update()
    {
        // Ha a zónában vagyunk ÉS megnyomjuk az E-t
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (isOpen) ClosePuzzle();
            else OpenPuzzle();
        }

        // Esc gombra bezárás
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePuzzle();
        }
    }

    public void OpenPuzzle()
    {
        isOpen = true;
        puzzleCanvas.SetActive(true);

        // ELÕHOZZUK AZ EGERET (Hogy tudj kattintani)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Opcionális: Megállítjuk a játékost/idõt
        // Time.timeScale = 0; 
    }

    public void ClosePuzzle()
    {
        isOpen = false;
        puzzleCanvas.SetActive(false);

        // ELTÜNTETJÜK AZ EGERET (Vissza FPS módba)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Time.timeScale = 1;
    }

    // Érzékeljük a játékost
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            ClosePuzzle(); // Ha elmegyünk, bezáródik
        }
    }
}
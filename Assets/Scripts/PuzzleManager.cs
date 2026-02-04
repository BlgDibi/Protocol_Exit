using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("UI és Effektek")]
    public GameObject steamEffect; // A Gõz részecske rendszer
    public GameObject puzzleCanvas; // Maga a UI ablak
    public PuzzleInteract interactionScript; // A játékos mozgását kezelõ script

    // Ezt hívja meg minden egyes csõ, amikor forog egyet
    public void CheckWin()
    {
        // 1. LÉPÉS: Megkeressük az összes "Pipe" scriptet a pályán
        // (Figyelj: A nevet "Pipe"-ra javítottam, hogy egyezzen a másik scripttel)
        Pipe[] allPipes = FindObjectsOfType<Pipe>();

        // 2. LÉPÉS: Végigmegyünk mindegyiken
        foreach (var pipe in allPipes)
        {
            // Ha akár csak EGY is rossz, akkor kilépünk, nincs gyõzelem
            if (pipe.isCorrect == false) return;
        }

        // 3. LÉPÉS: Ha a ciklus lefutott, és nem léptünk ki -> MINDEN JÓ!
        WinGame();
    }

    void WinGame()
    {
        Debug.Log("NYERTÉL! A GÕZ ELÁLLT.");

        if (steamEffect != null)
        {
            steamEffect.SetActive(false); // Gõz kikapcsolása
        }

        // Bezárjuk a puzzle-t a játékos scriptjén keresztül
        if (interactionScript != null)
        {
            interactionScript.ClosePuzzle();
        }
        else
        {
            // Biztonsági tartalék, ha nincs interactionScript bekötve
            if (puzzleCanvas != null) puzzleCanvas.SetActive(false);
        }
    }
}
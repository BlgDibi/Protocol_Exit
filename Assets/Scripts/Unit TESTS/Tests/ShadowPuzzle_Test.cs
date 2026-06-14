using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class ShadowPuzzle_Test
{
    [Test]
    public void ShadowPuzzle_CalculatesMarginOfError_Correctly()
    {
        GameObject puzzleObj = new GameObject();
        ShadowPuzzleFinal puzzle = puzzleObj.AddComponent<ShadowPuzzleFinal>();

        // A megoldás Y rotációja legyen 180 fok, a tûréshatár 5 fok
        puzzle.solutionRotation = new Vector3(0, 180f, 0);
        puzzle.marginOfError = 5f;

        // Ráforgatjuk a tárgyat 178 fokra (ami 2 fok eltérés, tehát bõven az 5-ös határon belül van)
        puzzle.transform.eulerAngles = new Vector3(0, 178f, 0);

   
        MethodInfo checkMethod = puzzle.GetType().GetMethod("CheckIfSolved", BindingFlags.NonPublic | BindingFlags.Instance);
        checkMethod.Invoke(puzzle, null);

        // ELLENÕRZÉS: Megnézzük, hogy a privát 'isSolved' változó igaz lett-e
        FieldInfo isSolvedField = puzzle.GetType().GetField("isSolved", BindingFlags.NonPublic | BindingFlags.Instance);
        bool isSolvedValue = (bool)isSolvedField.GetValue(puzzle);

        Assert.IsTrue(isSolvedValue, "A puzzle-nek megoldottnak kell lennie, ha az eltérés kisebb, mint a Margin of Error!");
    }
}
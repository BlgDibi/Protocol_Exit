using UnityEngine;

public class Lever_PowerPuzzle : MonoBehaviour, InterfaceInteractable
{
    [Header("PowerBox")]
    public PowerBoxPuzzle powerBox;

    [Header("Animation")]
    public Animator leverAnimator; 
    public string pullTrigger = "Pull";

    [Header("Target Power")]
    public int targetPower = 100;

    [Header("Objectstoturnon")]
    public GameObject[] objectsToTurnOn;

    public bool isPulled = false;

    public string GetInteractText() => isPulled ? "System succesfully overloaded" : "Pull the Lever";
    public KeyCode GetInteractKey() => KeyCode.E;

    public void Interact()
    {
        if (isPulled) return;

        if (powerBox.currentPower == targetPower)
        {
            isPulled = true;
            powerBox.isSolved = true;

            if (leverAnimator != null)
            {
                leverAnimator.SetTrigger(pullTrigger);
            }

            foreach (GameObject obj in objectsToTurnOn)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }

            Debug.Log("<color=red>BUMM! Rendszer rövidre zárva! A Vetítõ áramot kapott!</color>");
        }
        else
        {
            Debug.Log("<color=yellow>Hiba! Az áramszint nem megfelelõ!</color>");
        }
    }
}
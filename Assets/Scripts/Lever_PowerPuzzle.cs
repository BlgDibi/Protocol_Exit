using UnityEngine;

public class Lever_PowerPuzzle : MonoBehaviour, InterfaceInteractable
{
    [Header("Save System")]
    public string saveID = "Lever_Basement_01";

    [Header("PowerBox")]
    public PowerBoxPuzzle powerBox;

    [Header("Animation")]
    public Animator leverAnimator;
    public string pullTrigger = "Pull";

    [Tooltip("Animation name")]
    public string pulledAnimationState = "LeverDown";

    [Header("Target Power")]
    public int targetPower = 100;

    [Header("Objects to turn On (Disabled lamps)")]
    public GameObject[] objectsToTurnOn;

    [Header("Objects to turn Off (Flickering lights)")]
    public GameObject[] objectsToTurnOff;

    public bool isPulled = false;

    public string GetInteractText()
    {
        if (isPulled == true)
        {
            return "System successfully overloaded";
        }
        else
        {
            return "Pull the Lever";
        }
    }
    public KeyCode GetInteractKey() => KeyCode.F;

    void Start()
    {
        if (SaveSystem.IsPuzzleSolved(saveID))
        {
            isPulled = true;
            if (powerBox != null) powerBox.isSolved = true;

            if (leverAnimator != null)
            {
                leverAnimator.Play(pulledAnimationState, 0, 1f);
            }

            foreach (GameObject obj in objectsToTurnOn)
            {
                if (obj != null) obj.SetActive(true);
            }

            DisableFlickeringScripts();
        }
    }

    public void Interact()
    {
        if (isPulled) return;

        if (powerBox.currentPower == targetPower)
        {
            isPulled = true;
            powerBox.isSolved = true;

            SaveSystem.SavePuzzleSolved(saveID);

            if (leverAnimator != null)
            {
                leverAnimator.SetTrigger(pullTrigger);
            }

            foreach (GameObject obj in objectsToTurnOn)
            {
                if (obj != null) 
                    obj.SetActive(true);
            }

            DisableFlickeringScripts();
        }
    }

    private void DisableFlickeringScripts()
    {
        foreach (GameObject obj in objectsToTurnOff)
        {
            if (obj != null)
            {

            Light_Flicker[] flickerScripts = obj.GetComponentsInChildren<Light_Flicker>();

                foreach (Light_Flicker flickerScript in flickerScripts)
                {
                    flickerScript.enabled = false;

                    Light lightComp = flickerScript.GetComponent<Light>();
                    if (lightComp != null)
                    {
                        lightComp.intensity = 1.5f;
                    }
                }
            }
        }
    }
}
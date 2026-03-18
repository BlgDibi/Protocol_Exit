using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class Boxpickup : MonoBehaviour, InterfaceInteractable
{
    [Header("Box info")]
    [SerializeField] private SulyDobozData Boxdata;

    [Header("Settings")]
    [SerializeField] private KeyCode interactkey = KeyCode.E;
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;

    public bool isPlaced = false;

    void Start()
    {
        StartCoroutine(CheckIfAlreadyCollected());
    }

    IEnumerator CheckIfAlreadyCollected()
    {
        if (isPlaced) yield break;

        if (gameObject.name.Contains("_Placed")) yield break;

        yield return new WaitForSeconds(0.1f);

        if (Boxdata == null) yield break;

        List<string> mentettInventory = SaveSystem.GetInventoryBoxes();
        if (mentettInventory != null && mentettInventory.Contains(Boxdata.boxname))
        {
            gameObject.SetActive(false);
            yield break;
        }

        List<SaveSystem.PlacedBoxData> placedBoxes = SaveSystem.GetSavedPlacedBoxes();
        if (placedBoxes != null)
        {
            foreach (var placed in placedBoxes)
            {
                if (placed.boxname == Boxdata.boxname)
                {
                    gameObject.SetActive(false);

                    GameObject newBox = Instantiate(Boxdata.boxprefab, placed.position, Quaternion.identity);
                    newBox.name = Boxdata.boxname + "_Placed";
                    newBox.tag = "Boxes";

                    Rigidbody rb = newBox.GetComponent<Rigidbody>();
                    if (rb != null) rb.mass = Boxdata.weight;

          
                    Boxpickup spawnedScript = newBox.GetComponent<Boxpickup>();
                    if (spawnedScript != null) spawnedScript.isPlaced = true;

                    yield break;
                }
            }
        }
    }

    public string GetInteractText()
    {
        return interactionPrompt.GetLocalizedString();
    }

    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        if (Boxdata != null)
        {
            BoxInventory.Instance.AddBox(Boxdata);
            HighlightCrosshair(false);

            transform.position = new Vector3(transform.position.x, transform.position.y + 1000f, transform.position.z);

            Invoke(nameof(DisableBox), 0.1f);
        }
    }

    private void DisableBox()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) HighlightCrosshair(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) HighlightCrosshair(false);
    }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
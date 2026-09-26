using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

public class TableSlot : MonoBehaviour, InterfaceInteractable
{
    [Header("Slot Settings")]
    [Tooltip("The correct item ID for this specific slot")]
    public string acceptedItemID;
    public string currentPlacedItemID = "";

    [Header("Visuals")]
    public GameObject silverKnifeVisual;
    public GameObject woodenSpoonVisual;
    public GameObject chippedGlassVisual;
    public GameObject porcelainPlateVisual;

    [Header("Localization")]
    public LocalizedString placePrompt;
    public LocalizedString takePrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    private int selectedItemIndex = 0;

    void Update()
    {
        if (DiningPuzzle.IsTableSet) return;

        if (string.IsNullOrEmpty(currentPlacedItemID) && DiningItemsPickup.CollectedDiningItems.Count > 1)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0f)
            {
                selectedItemIndex++;
            }
            else if (scroll < 0f)
            {
                selectedItemIndex--;
                if (selectedItemIndex < 0) selectedItemIndex = DiningItemsPickup.CollectedDiningItems.Count - 1;
            }
        }
    }

    public string GetInteractText()
    {
        if (DiningPuzzle.IsTableSet) 
            return "";

        if (!string.IsNullOrEmpty(currentPlacedItemID))
        {
            return takePrompt.GetLocalizedString() + " (" + currentPlacedItemID + ")";
        }

        if (DiningItemsPickup.CollectedDiningItems.Count > 0)
        {
            int index = selectedItemIndex % DiningItemsPickup.CollectedDiningItems.Count;
            string itemToPlace = DiningItemsPickup.CollectedDiningItems[index];
            return placePrompt.GetLocalizedString() + " - " + itemToPlace;
        }

        return "You don't have any item to place down here!";
    }

    public KeyCode GetInteractKey()
    {
        if (!string.IsNullOrEmpty(currentPlacedItemID))
        {
            return KeyCode.E;
        }
        else
        {
            return KeyCode.Mouse0;
        }
    }

    public void Interact()
    {
        if (DiningPuzzle.IsTableSet) return;

        if (!string.IsNullOrEmpty(currentPlacedItemID))
        {
            DiningItemsPickup.CollectedDiningItems.Add(currentPlacedItemID);
            if (UiManager.Instance != null) UiManager.Instance.AddDiningItemToUI(currentPlacedItemID);

            currentPlacedItemID = "";
            UpdateVisuals();
            return;
        }

        if (DiningItemsPickup.CollectedDiningItems.Count > 0)
        {
            int index = selectedItemIndex % DiningItemsPickup.CollectedDiningItems.Count;
            string itemToPlace = DiningItemsPickup.CollectedDiningItems[index];

            currentPlacedItemID = itemToPlace;

            DiningItemsPickup.CollectedDiningItems.RemoveAt(index);
            if (UiManager.Instance != null) UiManager.Instance.RemoveDiningItemFromUI(itemToPlace);

            UpdateVisuals();

            if (DiningPuzzle.Instance != null)
            {
                DiningPuzzle.Instance.CheckPuzzle();
            }
        }
    }

    public void UpdateVisuals()
    {
        if (silverKnifeVisual) silverKnifeVisual.SetActive(currentPlacedItemID == "silverknife");
        if (woodenSpoonVisual) woodenSpoonVisual.SetActive(currentPlacedItemID == "woodenspoon");
        if (chippedGlassVisual) chippedGlassVisual.SetActive(currentPlacedItemID == "chippedglass");
        if (porcelainPlateVisual) porcelainPlateVisual.SetActive(currentPlacedItemID == "porcelainplate");
    }
}
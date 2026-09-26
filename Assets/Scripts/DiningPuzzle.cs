using UnityEngine;

public class DiningPuzzle : MonoBehaviour
{
    public static DiningPuzzle Instance;
    public static bool IsTableSet = false;

    [Header("Slots on table")]
    public TableSlot slotNorth;
    public TableSlot slotSouth;
    public TableSlot slotEast;
    public TableSlot slotWest;

    [Header("Audio Feedback")]
    public AudioSource puzzleAudioSource;
    public AudioClip successSound;
    public AudioClip errorSound;

    [Header("Punishment Settings")]
    public int damageOnFail = 10;

    [Tooltip("Player HP")]
    public HP playerHPReference;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        IsTableSet = SaveSystem.IsDiningTableSet;

        if (slotNorth != null) { slotNorth.currentPlacedItemID = SaveSystem.SlotNorthItem; slotNorth.UpdateVisuals(); }
        if (slotSouth != null) { slotSouth.currentPlacedItemID = SaveSystem.SlotSouthItem; slotSouth.UpdateVisuals(); }
        if (slotEast != null) { slotEast.currentPlacedItemID = SaveSystem.SlotEastItem; slotEast.UpdateVisuals(); }
        if (slotWest != null) { slotWest.currentPlacedItemID = SaveSystem.SlotWestItem; slotWest.UpdateVisuals(); }
    }

    public void CheckPuzzle()
    {
        if (IsTableSet) return;

        if (!string.IsNullOrEmpty(slotNorth.currentPlacedItemID) &&
            !string.IsNullOrEmpty(slotSouth.currentPlacedItemID) &&
            !string.IsNullOrEmpty(slotEast.currentPlacedItemID) &&
            !string.IsNullOrEmpty(slotWest.currentPlacedItemID))
        {
            bool allCorrect = true;

            if (slotNorth.currentPlacedItemID != slotNorth.acceptedItemID) allCorrect = false;
            if (slotSouth.currentPlacedItemID != slotSouth.acceptedItemID) allCorrect = false;
            if (slotEast.currentPlacedItemID != slotEast.acceptedItemID) allCorrect = false;
            if (slotWest.currentPlacedItemID != slotWest.acceptedItemID) allCorrect = false;

            if (allCorrect)
            {
                IsTableSet = true;
                if (puzzleAudioSource != null && successSound != null)
                {
                    puzzleAudioSource.PlayOneShot(successSound);
                    if (ConditionManager.Instance != null) ConditionManager.Instance.SetCondition("DiningSolved", true);
                }
            }
            else
            {
                if (puzzleAudioSource != null && errorSound != null)
                {
                    puzzleAudioSource.PlayOneShot(errorSound);
                }

                if (playerHPReference != null)
                {
                    playerHPReference.TakeDamage(damageOnFail);
                    Debug.Log("<color=red>Büntetés: " + damageOnFail + " HP levonva!</color>");
                }
                else
                {
                    Debug.LogError("Nincs behúzva a Player a DiningPuzzle scriptbe!");
                }
            }
        }
    }
}
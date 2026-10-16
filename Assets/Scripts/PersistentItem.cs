using UnityEngine;
using System.Collections;

public class PersistentItem : MonoBehaviour
{
    [SerializeField] private string itemID = "green_key_spawned";
    [SerializeField] private int keyIDInInventory = 1;

    [Header("Extra Security")]
    [Tooltip("DisableCondition")]
    [SerializeField] private string disableCondition = "GreenLockSolved";

    private void Awake()
    {
        SetVisibility(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);

        if (IsKeyAlreadyInHistory() || IsPuzzleAlreadySolved())
        {
            gameObject.SetActive(false);
            yield break;
        }

        if (SaveSystem.WasNPCMet(itemID))
        {
            SetVisibility(true);
        }
        else
        {
            SetVisibility(false);
        }
    }

    public void RevealItem()
    {
        SaveSystem.SaveNPCMet(itemID);
        SetVisibility(true);
        SaveSystem.Save(SaveSystem.CurrentSlot);
    }

    public void SetVisibility(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var rend in renderers)
        {
            rend.enabled = visible;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders)
        {
            col.enabled = visible;
        }
    }

    private bool IsKeyAlreadyInHistory()
    {
        if (KeyInventory.Instance == null) return false;
        return KeyInventory.Instance.GetHistoricalKeyIds().Contains(keyIDInInventory);
    }

    private bool IsPuzzleAlreadySolved()
    {
        if (string.IsNullOrEmpty(disableCondition)) return false;
        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(disableCondition)) return true;
        if (SaveSystem.IsPuzzleSolved(disableCondition)) return true;
        return false;
    }
}
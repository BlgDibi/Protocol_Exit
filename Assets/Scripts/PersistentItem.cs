using UnityEngine;
using System.Collections;

public class PersistentItem : MonoBehaviour
{
    [SerializeField] private string itemID = "green_key_spawned";
    [SerializeField] private int keyIDInInventory = 1;

    private void Awake()
    {
        SetVisibility(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        if (IsKeyAlreadyInInventory())
        {
            Destroy(gameObject);
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

    private bool IsKeyAlreadyInInventory()
    {
        if (KeyInventory.Instance == null) return false;
        return KeyInventory.Instance.GetSavedKeyIds().Contains(keyIDInInventory);
    }
}
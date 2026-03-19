using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentItem : MonoBehaviour
{
    [SerializeField] private string itemID = "green_key_spawned";
    [SerializeField] private int keyIDInInventory = 1;

    // Ezt használd Start helyett!
    private void Awake()
    {
        // Alapból láthatatlanná tesszük, amíg el nem dõl a sorsa
        SetVisibility(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        // 1. Ha nálunk van, töröljük
        if (IsKeyAlreadyInInventory())
        {
            Destroy(gameObject);
            yield break;
        }

        // 2. Ha kispawnolták, mutassuk meg
        if (SaveSystem.WasNPCMet(itemID))
        {
            SetVisibility(true);
            Debug.Log("<color=green>PERSISTENT ITEM: Kulcs sikeresen helyreállítva!</color>");
        }
        else
        {
            // Ha nem spawnolt le, maradjon láthatatlan és inaktív
            SetVisibility(false);
        }
    }

    private void SetVisibility(bool visible)
    {
        // Nem magát az objektumot kapcsoljuk ki (hogy a script fusson), 
        // hanem a kinézetét és az ütközését
        var renderer = GetComponentInChildren<Renderer>();
        var collider = GetComponent<Collider>();

        if (renderer != null) renderer.enabled = visible;
        if (collider != null) collider.enabled = visible;

        // Ha van rajta fény vagy részecske, azt is itt kapcsolhatod
    }

    private bool IsKeyAlreadyInInventory()
    {
        if (KeyInventory.Instance == null) return false;
        return KeyInventory.Instance.GetSavedKeyIds().Contains(keyIDInInventory);
    }
}
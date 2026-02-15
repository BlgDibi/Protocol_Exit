using UnityEngine;
using System.Collections; // Fontos a Coroutine miatt!

public class PlayerSave : MonoBehaviour
{
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // A Start coroutine vár egy picit, majd beállítja a pozíciót
    IEnumerator Start()
    {
        // Várunk egy képkockát, hogy a Unity biztosan betöltse a pályát
        yield return null;

        // Megkérdezzük a rendszert, hogy mentésbõl jöttünk-e
        if (SaveSystem.IsGameJustLoaded)
        {
            Debug.Log("MENTÉS ÉRZÉKELVE! Pozíció beállítása...");

            // Adatok lekérése a SaveSystem-tõl
            PlayerSaveData data = SaveSystem.GetPlayerData();

            // Pozíció alkalmazása
            Load(data);

            // Jelzõ kikapcsolása
            SaveSystem.IsGameJustLoaded = false;
        }
    }

    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;
    }

    public void Load(PlayerSaveData data)
    {
        // 1. CharacterController kikapcsolása (KÖTELEZÕ teleportálásnál!)
        if (controller != null) controller.enabled = false;

        // 2. Pozíció átírása
        transform.position = data.Position;

        // 3. Fizika szinkronizálása
        Physics.SyncTransforms();

        Debug.Log("PlayerSave: Játékos teleportálva ide: " + data.Position);

        // 4. Controller visszakapcsolása
        if (controller != null) controller.enabled = true;
    }
}

// Ennek az osztályon KÍVÜL kell lennie, a fájl legvégén!
[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
}
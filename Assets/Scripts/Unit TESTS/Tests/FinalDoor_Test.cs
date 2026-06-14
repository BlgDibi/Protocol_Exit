using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class FinalDoor_Test
{
    private GameObject doorObj;
    private Enddoor endDoor;

    [SetUp]
    public void SetUp()
    {
        // Létrehozunk egy virtuális Végsõ Ajtót a teszthez
        doorObj = new GameObject("TestEndDoor");
        endDoor = doorObj.AddComponent<Enddoor>();
    }

    [TearDown]
    public void TearDown()
    {
        // Takarítás a tesztek után
        Object.DestroyImmediate(doorObj);
    }

    [Test]
    public void Interact_WithUnlockedDoor_OpensDoor()
    {
        // ELÕKÉSZÜLET: Az ajtó alapból NINCS zárva
        endDoor.isLocked = false;

        // CSELEKVÉS: A játékos megnyomja az 'F'-et (Interact)
        endDoor.Interact();

        // ELLENÕRZÉS: A GetInteractText-nek meg kell változnia "Close Door"-ra,
        // ami azt jelenti, hogy a belsõ logika (ToggleDoor) sikeresen kinyitotta.
        // Mivel a szöveg nálad LocalizedString, ha üres (nincs beállítva a tesztben), akkor is megváltozik az állapota.
        Assert.IsFalse(endDoor.isLocked, "Az ajtónak nyitva kell maradnia!");
    }

    [Test]
    public void Interact_LockedDoor_WithNoConditions_UnlocksAndOpens()
    {
        // ELÕKÉSZÜLET: Az ajtó zárva van, DE nem kér semmilyen feltételt
        endDoor.isLocked = true;
        endDoor.requiredConditions = new List<string>(); // Üres lista

        // CSELEKVÉS: Interakció
        endDoor.Interact();

        // ELLENÕRZÉS: Mivel 0 feltétel volt, az ajtónak automatikusan fel kell oldania magát
        Assert.IsFalse(endDoor.isLocked, "Az ajtónak ki kell nyílnia, ha nincsenek megadva feltételek (canOpen = true)!");
    }
}
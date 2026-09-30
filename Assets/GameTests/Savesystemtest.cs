using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Savesystemtest
{
    [Test]
    public void Savedoorunlocked_remembersunlockeddoor()
    {
        Debug.Log("Test: Save unlocked door");

        SaveSystem.ResetAllData();
        string testDoor = "Maindoor";
        Debug.Log("Save data reset. Testing door ID: " + testDoor);

        SaveSystem.SaveDoorUnlocked(testDoor);
        Debug.Log("Door is marked as unlocked in SaveSystem.");

        Assert.IsTrue(SaveSystem.IsDoorUnlocked(testDoor));
        Debug.Log("SaveSystem remembers the door is unlocked. Test passed!");
    }

    [Test]
    public void ResetAllData_ClearsSavedDoors()
    {
        Debug.Log("Test: Reset save data");

        SaveSystem.ResetAllData();
        SaveSystem.SaveDoorUnlocked("Maindoor");
        Debug.Log("Door saved as unlocked.");

        SaveSystem.ResetAllData();
        Debug.Log("All save data is reset.");

        Assert.IsFalse(SaveSystem.IsDoorUnlocked("Maindoor"));
        Debug.Log("Door is no longer unlocked in memory. Test passed!");
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Doortest
{
    [Test]
    public void UnlockAndOpen_SetsIsLockedToFalse()
    {
        Debug.Log("Test: Unlock door");

        GameObject doorObj = new GameObject();
        UniversalDoor doorScript = doorObj.AddComponent<UniversalDoor>();
        doorScript.isLocked = true;
        Debug.Log("Door created and is locked.");

        doorScript.UnlockAndOpen();
        Debug.Log("UnlockAndOpen function is called.");

        Assert.IsFalse(doorScript.isLocked);
        Debug.Log("Door is no longer locked (isLocked is false). Test passed!");
    }
}

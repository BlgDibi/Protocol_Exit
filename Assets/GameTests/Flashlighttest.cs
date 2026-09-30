using NUnit.Framework;
using UnityEngine;

public class Flashlighttest
{
    [Test]
    public void Interact_hasflashlighttrue()
    {
        Debug.Log("Test: Flashlight pickup");
        
        Flashlightpickup.HasFlashlight = false;
        GameObject itemObj = new GameObject();
        Flashlightpickup flashlightScript = itemObj.AddComponent<Flashlightpickup>();
        Debug.Log("Flashlight is not in inventory.");

        flashlightScript.Interact();
        Debug.Log("Player picks up the flashlight.");

        Assert.IsTrue(Flashlightpickup.HasFlashlight);
        Debug.Log("Flashlight is now in inventory (True). Test passed!");
    }
}
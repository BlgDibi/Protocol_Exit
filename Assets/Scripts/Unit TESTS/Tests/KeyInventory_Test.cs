using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class KeyInventory_Test
{
    private GameObject inventoryObj;
    private KeyInventory inventory;
    private Key testKey;

    [SetUp]
    public void SetUp()
    {
        // 1. Létrehozunk egy tiszta KeyInventory-t egy kamu játéktárgyon
        inventoryObj = new GameObject("TestInventory");
        inventory = inventoryObj.AddComponent<KeyInventory>();

        // 2. Létrehozunk egy memóriába töltött "kamu" kulcsot (ScriptableObject-ként)
        testKey = ScriptableObject.CreateInstance<Key>();
        testKey.name = "Test_Golden_Key";
    }

    [TearDown]
    public void TearDown()
    {
        // Eltakarítjuk a memóriát a teszt után
        Object.DestroyImmediate(inventoryObj);
        Object.DestroyImmediate(testKey);
    }

    [Test]
    public void AddKey_StoresKeyInInventory()
    {
        inventory.AddKey(testKey);

        Assert.IsTrue(inventory.HasKey(testKey), "A kulcsnak benne kellene lennie az inventoryban a felvétel után!");
    }

    [Test]
    public void RemoveKey_DeletesKeyFromInventory()
    {
        inventory.AddKey(testKey);
        Assert.IsTrue(inventory.HasKey(testKey));

        inventory.RemoveKey(testKey);

        Assert.IsFalse(inventory.HasKey(testKey), "A kulcsnak el kellett volna tûnnie a RemoveKey után!");
    }

    [Test]
    public void ResetKeys_ClearsEntireInventory()
    {
        inventory.AddKey(testKey);

        inventory.ResetKeys();

        Assert.IsFalse(inventory.HasKey(testKey), "A ResetKeys után a teljes inventory-nak üresnek kell lennie!");
    }
}
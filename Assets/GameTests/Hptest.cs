using NUnit.Framework;
using UnityEngine;

public class HPTests
{
    [Test]
    public void Takedamage_reduceshealth()
    {
        Debug.Log("Test: Normal Damage");

        GameObject playerObj = new GameObject();
        HP hpScript = playerObj.AddComponent<HP>();
        hpScript.Hp = 100f;
        Debug.Log("Player created. HP = 100.");

        hpScript.TakeDamage(20f);
        Debug.Log("Player takes 20 damage. New HP = " + hpScript.Hp);

        Assert.AreEqual(80f, hpScript.Hp);
        Debug.Log("HP is exactly 80. Test passed!");
    }

    [Test]
    public void Takedamage_doesnotdrophealthbelowzero()
    {
        Debug.Log("Test: Death");

        GameObject playerObj = new GameObject();
        HP hpScript = playerObj.AddComponent<HP>();
        hpScript.Hp = 50f;
        Debug.Log("Player created. HP = 50.");


        hpScript.TakeDamage(100f);
        Debug.Log("Player takes 100 damage.");


        Assert.AreEqual(0f, hpScript.Hp);
        Assert.IsTrue(hpScript.isDead);
        Debug.Log("HP is 0 and player is dead. Test passed!");
    }
}
using NUnit.Framework;
using UnityEngine;

public class HP_Test
{
    [Test]
    public void TakeDamage_ReducesHealth_Correctly()
    {
        GameObject playerObj = new GameObject();
        HP playerHP = playerObj.AddComponent<HP>();
        playerHP.Hp = 100f;

        playerHP.TakeDamage(30f);

        Assert.AreEqual(70f, playerHP.Hp, "A HP-nak pontosan 70-nek kellene lennie a sebzõdés után!");
    }

    [Test]
    public void TakeDamage_DoesNotGoBelowZero_AndSetsDead()
    {
        GameObject playerObj = new GameObject();
        HP playerHP = playerObj.AddComponent<HP>();
        playerHP.Hp = 50f;

        playerHP.TakeDamage(100f);

        Assert.AreEqual(0f, playerHP.Hp, "A HP nem eshet 0 alá!");
        Assert.IsTrue(playerHP.isDead, "A játékosnak halottnak kell lennie (isDead = true), ha a HP eléri a 0-t!");
    }
}
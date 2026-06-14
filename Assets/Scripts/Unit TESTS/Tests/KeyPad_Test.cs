using NUnit.Framework;
using UnityEngine;
using TMPro;

public class KeyPad_Tests
{
    [Test]
    public void EnterCode_WithCorrectCode_GrantsAccess()
    {
        GameObject keypadObj = new GameObject();
        KeyPadSystem keypad = keypadObj.AddComponent<KeyPadSystem>();
        keypad.correctCode = "5821";

        // Kamu kijelzõ létrehozása
        GameObject textObj = new GameObject();
        keypad.display = textObj.AddComponent<TextMeshProUGUI>();

        // Beütjük a jó kódot
        keypad.ButtonPress("5");
        keypad.ButtonPress("8");
        keypad.ButtonPress("2");
        keypad.ButtonPress("1");
        keypad.EnterCode();

        // ELLENÕRZÉS
        Assert.AreEqual("GRANTED", keypad.display.text, "A jó kód megadásakor GRANTED feliratnak kell megjelennie!");
        Assert.AreEqual(Color.green, keypad.display.color, "A szöveg színének zöldnek kell lennie!");
    }

    [Test]
    public void EnterCode_WithWrongCode_DeniesAccess()
    {
        GameObject keypadObj = new GameObject();
        KeyPadSystem keypad = keypadObj.AddComponent<KeyPadSystem>();
        keypad.correctCode = "5821";

        GameObject textObj = new GameObject();
        keypad.display = textObj.AddComponent<TextMeshProUGUI>();

        // Beütünk egy rossz kódot
        keypad.ButtonPress("1");
        keypad.ButtonPress("1");
        keypad.ButtonPress("1");
        keypad.ButtonPress("1");
        keypad.EnterCode();

        // ELLENÕRZÉS
        Assert.AreEqual("DENIED", keypad.display.text, "A rossz kód megadásakor DENIED feliratnak kell megjelennie!");
        Assert.AreEqual(Color.red, keypad.display.color, "A szöveg színének pirosnak kell lennie!");
    }
}
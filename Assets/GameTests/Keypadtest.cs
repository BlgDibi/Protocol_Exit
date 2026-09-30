using NUnit.Framework;
using UnityEngine;

public class Keypadtest
{
    [Test]
    public void Buttonpress_addsnumbertodisplay()
    {
        Debug.Log("Test: Keypad input");

        GameObject keypadObj = new GameObject();
        KeyPadSystem keypadScript = keypadObj.AddComponent<KeyPadSystem>();
        keypadScript.currentInput = "";
        Debug.Log("Keypad display is empty.");

        keypadScript.ButtonPress("7");
        keypadScript.ButtonPress("3");
        Debug.Log("Pressed buttons 7 and 3.");

        Assert.AreEqual("73", keypadScript.currentInput);
        Debug.Log("Display shows '73'. Test passed!");
    }

    [Test]
    public void Clearinput_emptiesdisplay()
    {
        Debug.Log("Test: Keypad clear");

        GameObject keypadObj = new GameObject();
        KeyPadSystem keypadScript = keypadObj.AddComponent<KeyPadSystem>();
        keypadScript.currentInput = "1234";
        Debug.Log("Keypad display shows '1234'.");

        keypadScript.ClearInput();
        Debug.Log("Pressed the Clear button.");

        Assert.AreEqual("", keypadScript.currentInput);
        Debug.Log("Display is completely empty. Test passed!");
    }
}
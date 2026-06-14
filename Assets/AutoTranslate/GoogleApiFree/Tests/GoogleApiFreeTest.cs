using EqualchanceGames.Tools.InterfaceTranslate;
using NUnit.Framework;
using System.Collections.Generic;

public class GoogleApiFreeTest
{
    [Test]
    public void Translate_SimpleText_Hello()
    {
        GoogleApiFree googleApiFree = new GoogleApiFree();

        string result = googleApiFree.Translate("Hello", "en", "ru");

        Assert.IsNotNull(result, "Result is null");
        Assert.True(string.Equals(result, "Привет"), "No true translate");
    }

    [Test]
    public void Translate_BiggerText_Hello()
    {
        GoogleApiFree googleApiFree = new GoogleApiFree();

        string result = googleApiFree.Translate("The technology was developed for different platforms.", "en", "ru");

        Assert.IsNotNull(result, "Result is null");
        Assert.True(string.Equals(result, "Технология разрабатывалась для разных платформ."), "No true translate");
    }


    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    //[UnityTest]
    //public IEnumerator GoogleApiFreeTestWithEnumeratorPasses()
    //{
    //    // Use the Assert class to test conditions.
    //    // Use yield to skip a frame.
    //    yield return null;
    //}
}

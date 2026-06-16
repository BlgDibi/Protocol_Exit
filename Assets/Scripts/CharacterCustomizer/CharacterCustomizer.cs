using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomizer : MonoBehaviour
{
    [Header("Character Modell")]
    public Renderer[] characterRenderers;

    [Header("Clothes")]
    public Texture[] characterSkins;

    private int currentIndex = 0;

    void Start()
    {
        UpdateSkinDisplay();
    }

    public void NextSkin()
    {
        currentIndex++;
        if (currentIndex >= characterSkins.Length) currentIndex = 0;
        UpdateSkinDisplay();
    }

    public void PrevSkin()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = characterSkins.Length - 1;
        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay()
    {
        if (characterSkins.Length == 0) return;

        Texture newTexture = characterSkins[currentIndex];

        foreach (Renderer r in characterRenderers)
        {
            if (r != null)
            {
                r.material.mainTexture = newTexture;
                r.material.SetTexture("_BaseMap", newTexture);
            }
        }
    }

    public void AcceptSelection()
    {
        SaveSystem.SelectedSkinIndex = currentIndex;
        SceneManager.LoadScene("firstroom"); 
    }
}

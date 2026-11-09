using UnityEngine;
using TMPro;

public class LoadCharachterOutfit : MonoBehaviour
{
    [Header("Main Character")]
    public Renderer[] playerRenderers;
    public Transform playerRootTransform;

    [Header("Clothes")]
    public Texture[] playerSkins;

    [Header("Name Display (Optional)")]
    public TMP_Text playerNameText;

    void Start()
    {
        // --- TESZT ÜZENET A CONSOLE-BA ---
        Debug.Log("PÁLYA BETÖLTVE! A SaveSystem-ben lévõ név jelenleg: '" + SaveSystem.PlayerName + "'");

        int savedSkin = SaveSystem.SelectedSkinIndex;
        if (playerSkins.Length > savedSkin)
        {
            Texture savedTexture = playerSkins[savedSkin];

            foreach (Renderer r in playerRenderers)
            {
                if (r != null)
                {
                    r.material.mainTexture = savedTexture;
                    r.material.SetTexture("_BaseMap", savedTexture);
                }
            }
        }

        if (playerNameText != null)
        {
            playerNameText.text = SaveSystem.PlayerName;
            Debug.Log("Név kiírása a UI-ra megtörtént!");
        }
        else
        {
            Debug.LogError("HIBA: A playerNameText NINCS BEHÚZVA a LoadCharachterOutfit Inspectorába ezen a pályán!");
        }

        if (playerRootTransform != null)
        {
            Vector3 baseScale = playerRootTransform.localScale;
            float w = SaveSystem.PlayerWidth;
            float h = SaveSystem.PlayerHeight;

            playerRootTransform.localScale = new Vector3(baseScale.x * w, baseScale.y * h, baseScale.z * w);
        }
    }
}
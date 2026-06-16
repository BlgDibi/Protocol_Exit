using UnityEngine;

public class LoadCharachterOutfit : MonoBehaviour
{
    [Header("Main Character")]
    public Renderer[] playerRenderers;
    [Header("Clothes")]
    public Texture[] playerSkins;

    void Start()
    {
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
    }
}
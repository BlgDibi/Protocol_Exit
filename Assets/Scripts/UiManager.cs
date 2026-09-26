using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UiManager : MonoBehaviour
{
    public static bool isMenuOpen = false;

    [Header("UI References")]
    [SerializeField] private Transform keyPanel;
    [SerializeField] private GameObject keyImagePrefab;
    [SerializeField] private Transform boxPanel;
    [SerializeField] private GameObject boxImagePrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite flashlightSprite;
    [SerializeField] private Sprite crowbarSprite;
    [SerializeField] private Sprite detonatorSprite;
    [SerializeField] private Sprite c4Sprite;
    [SerializeField] private Sprite valveSprite;

    [Header("Dining Puzzle Sprites")]
    [SerializeField] private Sprite silverKnifeSprite;
    [SerializeField] private Sprite woodenSpoonSprite;
    [SerializeField] private Sprite chippedGlassSprite;
    [SerializeField] private Sprite porcelainPlateSprite;

    private Dictionary<SulyDobozData, GameObject> boxImages = new Dictionary<SulyDobozData, GameObject>();
    private Dictionary<Key, GameObject> keyImages = new Dictionary<Key, GameObject>();

    private GameObject flashlightUIInstance;
    private GameObject crowbarUIInstance;
    private GameObject detonatorUIInstance;
    private GameObject c4UIInstance;
    private GameObject valveUIInstance;

    private GameObject silverKnifeUIInstance;
    private GameObject woodenSpoonUIInstance;
    private GameObject chippedGlassUIInstance;
    private GameObject porcelainPlateUIInstance;

    public Transform keysParent;

    public static UiManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshAllUI();
    }

    public void RefreshAllUI()
    {
        if (keyPanel == null) return;

        foreach (Transform child in keyPanel)
        {
            Destroy(child.gameObject);
        }

        flashlightUIInstance = null;
        crowbarUIInstance = null;
        detonatorUIInstance = null;
        c4UIInstance = null;
        valveUIInstance = null;

        silverKnifeUIInstance = null;
        woodenSpoonUIInstance = null;
        chippedGlassUIInstance = null;
        porcelainPlateUIInstance = null;

        boxImages.Clear();
        keyImages.Clear();

        if (Flashlightpickup.HasFlashlight) AddFlashlightToUI();
        if (CrowbarPickup.HasCrowbar) AddCrowbarToUI();
        if (Detonator_Pickup.HasDetonator) AddDetonatorToUI();
        if (C4_Pickup.HasC4) AddC4ToUI();
        if (ValvePickup.HasValve) AddValveToUI();

        if (DiningItemsPickup.CollectedDiningItems != null)
        {
            foreach (string itemID in DiningItemsPickup.CollectedDiningItems)
            {
                AddDiningItemToUI(itemID);
            }
        }

        Canvas.ForceUpdateCanvases();
    }


    public void AddDiningItemToUI(string itemID)
    {
        if (keyPanel == null) return;

        if (itemID == "silverknife" && silverKnifeUIInstance == null && silverKnifeSprite != null)
        {
            silverKnifeUIInstance = Instantiate(keyImagePrefab, keyPanel);
            silverKnifeUIInstance.GetComponent<Image>().sprite = silverKnifeSprite;
        }
        else if (itemID == "woodenspoon" && woodenSpoonUIInstance == null && woodenSpoonSprite != null)
        {
            woodenSpoonUIInstance = Instantiate(keyImagePrefab, keyPanel);
            woodenSpoonUIInstance.GetComponent<Image>().sprite = woodenSpoonSprite;
        }
        else if (itemID == "chippedglass" && chippedGlassUIInstance == null && chippedGlassSprite != null)
        {
            chippedGlassUIInstance = Instantiate(keyImagePrefab, keyPanel);
            chippedGlassUIInstance.GetComponent<Image>().sprite = chippedGlassSprite;
        }
        else if (itemID == "porcelainplate" && porcelainPlateUIInstance == null && porcelainPlateSprite != null)
        {
            porcelainPlateUIInstance = Instantiate(keyImagePrefab, keyPanel);
            porcelainPlateUIInstance.GetComponent<Image>().sprite = porcelainPlateSprite;
        }
    }

    public void RemoveDiningItemFromUI(string itemID)
    {
        if (itemID == "silverknife" && silverKnifeUIInstance != null)
        {
            Destroy(silverKnifeUIInstance);
            silverKnifeUIInstance = null;
        }
        else if (itemID == "woodenspoon" && woodenSpoonUIInstance != null)
        {
            Destroy(woodenSpoonUIInstance);
            woodenSpoonUIInstance = null;
        }
        else if (itemID == "chippedglass" && chippedGlassUIInstance != null)
        {
            Destroy(chippedGlassUIInstance);
            chippedGlassUIInstance = null;
        }
        else if (itemID == "porcelainplate" && porcelainPlateUIInstance != null)
        {
            Destroy(porcelainPlateUIInstance);
            porcelainPlateUIInstance = null;
        }
    }

    public void AddFlashlightToUI()
    {
        if (keyPanel == null || flashlightSprite == null) return;
        if (flashlightUIInstance != null) return;

        flashlightUIInstance = Instantiate(keyImagePrefab, keyPanel);
        flashlightUIInstance.GetComponent<Image>().sprite = flashlightSprite;
    }

    public void AddCrowbarToUI()
    {
        if (keyPanel == null || crowbarSprite == null) return;
        if (crowbarUIInstance != null) return;

        crowbarUIInstance = Instantiate(keyImagePrefab, keyPanel);
        crowbarUIInstance.GetComponent<Image>().sprite = crowbarSprite;
    }

    public void AddDetonatorToUI()
    {
        if (keyPanel == null || detonatorSprite == null) return;
        if (detonatorUIInstance != null) return;

        detonatorUIInstance = Instantiate(keyImagePrefab, keyPanel);
        detonatorUIInstance.GetComponent<Image>().sprite = detonatorSprite;
    }

    public void AddC4ToUI()
    {
        if (keyPanel == null || c4Sprite == null) return;
        if (c4UIInstance != null) return;

        c4UIInstance = Instantiate(keyImagePrefab, keyPanel);
        c4UIInstance.GetComponent<Image>().sprite = c4Sprite;
    }

    public void AddValveToUI()
    {
        if (keyPanel == null || valveSprite == null) return;
        if (valveUIInstance != null) return;

        valveUIInstance = Instantiate(keyImagePrefab, keyPanel);
        valveUIInstance.GetComponent<Image>().sprite = valveSprite;
    }

    public void RemoveValveFromUI()
    {
        if (valveUIInstance != null)
        {
            Destroy(valveUIInstance);
            valveUIInstance = null;
        }
    }

    public void AddBoxToUI(SulyDobozData doboz)
    {
        if (doboz == null || keyPanel == null) return;
        if (boxImages.ContainsKey(doboz)) return;

        GameObject boxImage = Instantiate(keyImagePrefab, keyPanel);
        boxImage.GetComponent<Image>().sprite = doboz.boxpicture;
        boxImages[doboz] = boxImage;
    }

    public void RemoveBoxFromUI(SulyDobozData doboz)
    {
        if (doboz == null || !boxImages.ContainsKey(doboz)) return;

        if (boxImages[doboz] != null) Destroy(boxImages[doboz]);
        boxImages.Remove(doboz);
    }

    public void AddKeyToUI(Key key)
    {
        if (key == null || keyPanel == null) return;
        if (keyImages.ContainsKey(key)) return;

        GameObject keyImage = Instantiate(keyImagePrefab, keyPanel);
        keyImage.GetComponent<Image>().sprite = key.keysprite;
        keyImages[key] = keyImage;
    }

    public void RemoveKeyFromUI(Key key)
    {
        if (key == null || !keyImages.ContainsKey(key)) return;

        if (keyImages[key] != null) Destroy(keyImages[key]);
        keyImages.Remove(key);
    }

    public void ClearAllKeysFromUI()
    {
        if (keysParent != null)
        {
            foreach (Transform child in keysParent)
            {
                if (child != null) Destroy(child.gameObject);
            }
        }
        keyImages.Clear();
    }

    public void RemoveC4FromUI()
    {
        if (c4UIInstance != null)
        {
            Destroy(c4UIInstance);
            c4UIInstance = null;
        }
    }

    public void RemoveDetonatorFromUI()
    {
        if (detonatorUIInstance != null)
        {
            Destroy(detonatorUIInstance);
            detonatorUIInstance = null;
        }
    }

    public void ShowTemporaryMessage(GameObject messagePanel, float duration)
    {
        if (messagePanel == null) return;

        StartCoroutine(TemporaryMessageRoutine(messagePanel, duration));
    }

    private IEnumerator TemporaryMessageRoutine(GameObject panel, float delay)
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
    }
}
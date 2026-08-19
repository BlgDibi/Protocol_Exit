using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class UiManager : MonoBehaviour
{

    public static bool isMenuOpen = false;

    [SerializeField] private Transform keyPanel;
    [SerializeField] private GameObject keyImagePrefab;

    [SerializeField] private Transform boxPanel;
    [SerializeField] private GameObject boxImagePrefab;
    private Dictionary<SulyDobozData, GameObject> boxImages = new Dictionary<SulyDobozData, GameObject>();


    private Dictionary<Key,GameObject> keyImages = new Dictionary<Key,GameObject>();

    [SerializeField] private Sprite flashlightSprite;
    private GameObject flashlightUIInstance;


    [SerializeField] private Sprite crowbarSprite;
    private GameObject crowbarUIInstance;

    public static UiManager Instance { get; private set; }

    public Transform keysParent;

    private void Start()
    {
        RefreshAllUI();
    }

    private void OnEnable()
    {
        RefreshAllUI();
    }
    public void RefreshAllUI()
    {
        if (Flashlightpickup.HasFlashlight)
        {
            AddFlashlightToUI();
        }

        if (CrowbarPickup.HasCrowbar)
        {
            AddCrowbarToUI();
        }

        Canvas.ForceUpdateCanvases();
    }

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddFlashlightToUI()
    {
        if (flashlightUIInstance != null || flashlightSprite == null) return;

        flashlightUIInstance = Instantiate(keyImagePrefab, keyPanel);
        flashlightUIInstance.GetComponent<Image>().sprite = flashlightSprite;

        Debug.Log("Flashlight added to UI!");

        Canvas.ForceUpdateCanvases();
    }

    public void AddCrowbarToUI()
    {
        if (crowbarUIInstance != null || crowbarSprite == null) return;

        crowbarUIInstance = Instantiate(keyImagePrefab, keyPanel);
        crowbarUIInstance.GetComponent<Image>().sprite = crowbarSprite;

        Debug.Log("Crowbar added to UI!");

        Canvas.ForceUpdateCanvases();
    }
    public void AddBoxToUI(SulyDobozData doboz)
    {
        if (boxImages.ContainsKey(doboz)) return;

        GameObject boxImage = Instantiate(keyImagePrefab, keyPanel);
        boxImage.GetComponent<UnityEngine.UI.Image>().sprite = doboz.boxpicture;

        boxImages[doboz] = boxImage;
    }

    public void RemoveBoxFromUI(SulyDobozData doboz)
    {
        if (boxImages.ContainsKey(doboz))
        {
            Destroy(boxImages[doboz]);
            boxImages.Remove(doboz);
        }
    }



    public void AddKeyToUI(Key key)
    {
        if (!keyImages.ContainsKey(key))
        {
            GameObject keyImage = Instantiate(keyImagePrefab, keyPanel);
            keyImage.GetComponent<Image>().sprite = key.keysprite;
            keyImages[key] = keyImage;

            Debug.Log("Key added to UI: " + key.keysprite.name);


        }

    }




    public void RemoveKeyFromUI(Key key)
    {
        if (keyImages.ContainsKey(key))
        {
            Destroy(keyImages[key]);
            keyImages.Remove(key);
            Debug.Log("Key removed from UI: " + key.keysprite.name);
        }
    }


    public void ClearAllKeysFromUI()
    {

        if (keysParent == null) return;

        foreach (Transform child in keysParent)
        {
            Destroy(child.gameObject);
        }
    }



}

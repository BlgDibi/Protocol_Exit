using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class UiManager : MonoBehaviour
{

    [SerializeField] private Transform keyPanel;
    [SerializeField] private GameObject keyImagePrefab;


    private Dictionary<Key,GameObject> keyImages = new Dictionary<Key,GameObject>();

    public static UiManager Instance { get; private set; }

    public Transform keysParent;
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
           
        
        }
        else
        {
            Destroy(gameObject);
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

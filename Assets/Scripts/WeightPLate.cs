using UnityEngine;
using TMPro;

public class WeightPlateInteractor : MonoBehaviour, InterfaceInteractable
{
    [Header("Scale Settings")]
    [SerializeField] private float optimalweight = 25f;
    [SerializeField] private TMP_Text Weighttext;
    private float currentweight = 0f;

    void Start()
    {
        UiReset();
    }

    public void Interact()
    {
        if (BoxInventory.Instance.HasAnyBox())
        {
            SulyDobozData data = BoxInventory.Instance.GetFirstBox();
            if (data != null && data.boxprefab != null)
            {
                BoxInventory.Instance.RemoveBox(data);
                GameObject boxdropped = Instantiate(data.boxprefab, transform.position + (Vector3.up * 1.5f), Quaternion.identity);

                boxdropped.name = data.boxname;
                boxdropped.tag = "Boxes";

                Rigidbody rb = boxdropped.GetComponent<Rigidbody>();
                if (rb != null) rb.mass = data.weight;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boxes"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                currentweight += rb.mass;
                UiReset();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boxes"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                currentweight -= rb.mass;
                if (currentweight < 0.01f) currentweight = 0f;
                UiReset();
            }
        }
    }

    void UiReset()
    {
        if (Weighttext != null)
        {
            if (currentweight < 0.01f)
            {
                Weighttext.gameObject.SetActive(false);
                return; 
            }

            Weighttext.gameObject.SetActive(true);
            Weighttext.text = currentweight + " / " + optimalweight + " kg";

            if (Mathf.Approximately(currentweight, optimalweight))
            {
                Weighttext.color = Color.green;
            }
            else
            {
                Weighttext.color = Color.white;
            }
        }
    }

    public string GetInteractText()
    {
        if (BoxInventory.Instance.HasAnyBox())
        {
            SulyDobozData adat = BoxInventory.Instance.GetFirstBox();
            return "Drop box";
        }
        return "You dont have any boxes";
    }

    public KeyCode GetInteractKey() => KeyCode.E;
}
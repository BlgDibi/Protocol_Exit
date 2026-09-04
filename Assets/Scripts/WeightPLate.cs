using UnityEngine;
using TMPro;
using System.Collections;

public class WeightPlate : MonoBehaviour, InterfaceInteractable
{
    [Header("Scale Settings")]
    [SerializeField] private float optimalweight = 25f;
    [SerializeField] private TMP_Text Weighttext;
    private float currentweight = 0f;

    [Header("UI")]
    public GameObject scaleUIPanel;
    private bool isUiHidden = false;
    private Coroutine hideCoroutine;

    void Start()
    {
        UiReset();
    }

    public void Interact()
    {
        
        if (IsWeightOptimal())
        {
            if (Weighttext != null)
            {
                Weighttext.text = "The scale is already balanced!";
                Weighttext.color = Color.yellow;
            }
            return;
        }

        if (BoxInventory.Instance.HasAnyBox())
        {
            SulyDobozData data = BoxInventory.Instance.GetFirstBox();
            if (data != null && data.boxprefab != null)
            {
                BoxInventory.Instance.RemoveBox(data);
                GameObject boxdropped = Instantiate(data.boxprefab, transform.position + (Vector3.up * 1.5f), Quaternion.identity);

                boxdropped.name = data.boxname + "_Placed";
                boxdropped.tag = "Boxes";

                Boxpickup pickupScript = boxdropped.GetComponent<Boxpickup>();
                if (pickupScript != null)
                {
                    pickupScript.isPlaced = true;
                }

                Rigidbody rb = boxdropped.GetComponent<Rigidbody>();
                if (rb != null) rb.mass = data.weight;
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
                if (scaleUIPanel != null) scaleUIPanel.SetActive(false);
                return;
            }

            if (scaleUIPanel != null && !isUiHidden) scaleUIPanel.SetActive(true);
            Weighttext.gameObject.SetActive(true);
            Weighttext.text = currentweight + " / " + optimalweight + " kg";

            if (Mathf.Approximately(currentweight, optimalweight))
            {
                Weighttext.color = Color.green;

                ConditionManager.Instance.SetCondition("ScaleSolved", true);

                if (!isUiHidden)
                {
                    isUiHidden = true;
                    if (hideCoroutine != null) StopCoroutine(hideCoroutine);
                    hideCoroutine = StartCoroutine(HideUIAfterDelay());
                }
            }
            else
            {
                Weighttext.color = Color.white;

                ConditionManager.Instance.SetCondition("ScaleSolved", false);

                isUiHidden = false;
                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                    hideCoroutine = null;
                }
                if (scaleUIPanel != null) scaleUIPanel.SetActive(true);
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


    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        if (scaleUIPanel != null)
        {
            scaleUIPanel.SetActive(false);
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

    public bool IsWeightOptimal()
    {
        return Mathf.Approximately(currentweight, optimalweight);
    }

    public KeyCode GetInteractKey() => KeyCode.F;
}
using UnityEngine;
using TMPro;

public class WeightPlateInteractor : MonoBehaviour, InterfaceInteractable
{
    [Header("Scale Settings")]
    [SerializeField] private float optimalweight = 25f;
    [SerializeField] private TMP_Text sulySzoveg;
    private float currentweight = 0f;

    void Start()
    {
            UiReset();
    }

    public void Interact()
    {
        if (BoxInventory.Instance.HasAnyBox())
        {
            SulyDobozData adat = BoxInventory.Instance.GetFirstBox();

            if (adat != null && adat.boxprefab != null)
            {
                BoxInventory.Instance.RemoveBox(adat);

                GameObject boxdropped = Instantiate(adat.boxprefab, transform.position + Vector3.up, Quaternion.identity);

                Rigidbody rb = boxdropped.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.mass = adat.weight; 
                }
            }
        }
        else
        {
            Debug.Log("You dont have any boxes!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            currentweight += rb.mass; 
            UiReset();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            currentweight -= rb.mass;
            if (currentweight < 0.01f) currentweight = 0f;
            UiReset();
        }
    }

    void UiReset()
    {
        if (sulySzoveg != null)
        {

            sulySzoveg.text = currentweight + " / " + optimalweight + " kg";

            sulySzoveg.color = (currentweight == optimalweight) ? Color.green : Color.white;
        }
    }

    public string GetInteractText()
    {
        if (BoxInventory.Instance.HasAnyBox())
        {
            SulyDobozData adat = BoxInventory.Instance.GetFirstBox();
            return "Drop box (" + adat.weight + "kg)";
        }
        return "You dont have any boxes";
    }

    public KeyCode GetInteractKey() => KeyCode.E;
}
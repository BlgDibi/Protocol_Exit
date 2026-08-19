using UnityEngine;

public class Plank : MonoBehaviour
{
    [Header("Own ID for Planks")]
    public string plankID = "Plank1";

    void Start()
    {
        if (SaveSystem.IsPlankPried(plankID))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            gameObject.tag = "Untagged";
        }
    }

    public void OnPried()
    {
        SaveSystem.SavePlankPried(plankID);
    }
}
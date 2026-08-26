using UnityEngine;

public class ItemSave : MonoBehaviour
{
    public string itemID = "Cupboard_01";

    void Start()
    {
        LoadCupboard();
    }

    public void LoadCupboard()
    {
        if (SaveSystem.GetCupboardTransform(itemID, out Vector3 pos, out Quaternion rot))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            bool wasKinematic = false;

            if (rb != null)
            {
                wasKinematic = rb.isKinematic;
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            transform.position = pos;
            transform.rotation = rot;

            if (rb != null)
            {
                rb.isKinematic = wasKinematic; 
            }
        }
    }
}
using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [Header("Main camera ")]
    public Camera mainCam;

    [Header("Player Body")]
    public Transform playerBody;

    [Header("Max rotation")]
    public float maxAngle = 80f;

    void Start()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (mainCam == null || playerBody == null) return;

        Quaternion targetRotation = mainCam.transform.rotation;

        transform.rotation = Quaternion.RotateTowards(playerBody.rotation, targetRotation, maxAngle);
    }
}
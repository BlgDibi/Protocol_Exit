using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [Header("References")]
    public Camera mainCam;
    public Transform playerBody;

    [Header("Angle")]
    public float maxUpDown = 50f;    
    public float maxLeftRight = 50f;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (playerBody == null && transform.parent != null) playerBody = transform.parent;
    }

    void LateUpdate()
    {
        if (mainCam == null || playerBody == null) return;

        Quaternion localCamRotation = Quaternion.Inverse(playerBody.rotation) * mainCam.transform.rotation;

        Vector3 euler = localCamRotation.eulerAngles;

        float pitchX = euler.x > 180 ? euler.x - 360 : euler.x;
        float yawY = euler.y > 180 ? euler.y - 360 : euler.y;

        pitchX = Mathf.Clamp(pitchX, -maxUpDown, maxUpDown);
        yawY = Mathf.Clamp(yawY, -maxLeftRight, maxLeftRight);

        Quaternion clampedLocalRotation = Quaternion.Euler(pitchX, yawY, 0f);
        transform.rotation = playerBody.rotation * clampedLocalRotation;
    }
}
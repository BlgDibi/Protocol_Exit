using UnityEngine;

public class SecretRevealer : MonoBehaviour
{
    [Header("Save settings ")]
    [Tooltip("SecretID")]
    public string secretID;

    [Header("Movement settings")]
    public Vector3 moveDirection = new Vector3(0.2f, 0, 0);
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    public bool isMoved { get; private set; } = false;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveDirection;

        if (SaveSystem.IsSecretActivated(secretID))
        {
            isMoved = true;
            transform.position = targetPosition;
        }
    }

    void Update()
    {
        Vector3 destination = isMoved ? targetPosition : startPosition;
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * moveSpeed);
    }

    public void ActivateSecret()
    {
        isMoved = true;
    }

    public void ResetSecret()
    {
        isMoved = false;
    }
}
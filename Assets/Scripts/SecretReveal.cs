using UnityEngine;

public class SecretRevealer : MonoBehaviour
{
    public Vector3 moveDirection = new Vector3(0.2f, 0, 0);
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoved = false;

    void Start()
    {
        startPosition = transform.position; 
        targetPosition = startPosition + moveDirection;
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
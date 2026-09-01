using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("Pivot")]
    public Transform pivot;

    [Header("Rotation Speed")]
    public float rotationSpeed = 10f;

    [Header("Camera")]
    public Camera puzzleCamera;

    private bool isDragging = false;

    void Update()
    {
        if (puzzleCamera == null || !puzzleCamera.gameObject.activeInHierarchy)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("<color=yellow>Az egér ezt találta el: " + hit.collider.gameObject.name + "</color>");

                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;

                ClockPuzzle manager = FindFirstObjectByType<ClockPuzzle>();
                if (manager != null)
                {
                    manager.CheckTime();
                }
            }
        }

        if (isDragging)
        {
            float mouseMove = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
            pivot.Rotate(0, 0, -mouseMove * rotationSpeed);
        }
    }
}
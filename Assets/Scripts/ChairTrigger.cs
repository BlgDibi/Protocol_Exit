using UnityEngine;

public class ChairTrigger : MonoBehaviour
{
    public ClassroomPuzzleManager puzzleManager;
    public Collider chairCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other == chairCollider)
        {
            if (puzzleManager != null)
            {
                puzzleManager.InteractWithDesk();
            }
        }
    }
}
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [Header("More spawn points")]
    public Transform[] spawnPoints;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int totalChoices = spawnPoints.Length + 1;

        int randomIndex = Random.Range(0, totalChoices);

        if (randomIndex == 0)
        {
            return;
        }

        Transform selectedSpawnPoint = spawnPoints[randomIndex - 1];

        transform.position = selectedSpawnPoint.position;
        transform.rotation = selectedSpawnPoint.rotation;
    }
}
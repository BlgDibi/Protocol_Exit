using UnityEngine;

public class Chopping : MonoBehaviour
{
    [Header("Slices")]
    public Transform[] vegetableSlices;
    [Tooltip("Pushdirection (X, Y, Z)")]
    public Vector3 pushDirection = new Vector3(0.05f, 0f, 0f);

    [Tooltip("CarrotParent")]
    public Transform carrotParent;
    [Tooltip("Direction_feed (X, Y, Z) ")]
    public Vector3 feedDirection = new Vector3(-0.02f, 0f, 0f);

    private int currentSliceIndex = 0;

    public void ChopNextSlice()
    {
        if (currentSliceIndex < vegetableSlices.Length)
        {
            for (int i = 0; i <= currentSliceIndex; i++)
            {
                vegetableSlices[i].position += pushDirection;

                if (i == currentSliceIndex)
                {
                    vegetableSlices[i].Rotate(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f));
                }
            }

            if (carrotParent != null)
            {
                carrotParent.position += feedDirection;
            }

            currentSliceIndex++;
        }
    }
}
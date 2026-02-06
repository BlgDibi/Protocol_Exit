using UnityEngine;

public class SceneData : MonoBehaviour
{
    public SceneData_Save Data;

    private void Awake()
    {
        if (Data != null)
        {
            SaveSystem.CurrentSceneIndex = Data.SceneIndex;
        }
    }
}
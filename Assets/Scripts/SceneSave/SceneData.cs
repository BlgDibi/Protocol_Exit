
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



public class SceneData : MonoBehaviour

{

    public SceneData_Save Data;

    public static string TargetSceneName = "";

    private void Awake()

    {

        if (Data != null)

        {

            SaveSystem.CurrentSceneId = Data.UniqueName;
        }
        else
        {
            Debug.LogWarning("SceneData not connected");
        }

    }

 

}


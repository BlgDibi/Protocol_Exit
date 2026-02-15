
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



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



    public void Save(ref SceneSaveData data)

    {

        data.SceneId = Data.UniqueName;

    }

    public void Load(ref SceneSaveData data)

    {

        if (SceneLoader.Instance != null)

        {

            SceneLoader.Instance.LoadSceneByIndex(data.SceneId);

        }

    }

}







[System.Serializable]



public struct SceneSaveData

{

    public string SceneId;



}


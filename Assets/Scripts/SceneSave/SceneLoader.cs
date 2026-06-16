using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



public class SceneLoader : MonoBehaviour

{

    public static SceneLoader Instance;



    [SerializeField] private SceneData_Save[] _sceneDataarray;



    private Dictionary<string, int> _sceneIdToIndexmap = new Dictionary<string, int>();



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PopulateSceneMappings();
    }


    private void PopulateSceneMappings()

    {

        foreach (var sceneMapping in _sceneDataarray)

        {

            if (sceneMapping != null)

            {

                _sceneIdToIndexmap[sceneMapping.UniqueName] = sceneMapping.SceneIndex;

            }

        }

    }

   

    public void LoadSceneByIndex(string saveSceneID)

    {

        if (_sceneIdToIndexmap.TryGetValue(saveSceneID, out int sceneindex))

        {

            SceneManager.LoadScene(sceneindex);

        }

        else

        {

            Debug.Log("No scene found for ID: " + saveSceneID);

        }

    }

}



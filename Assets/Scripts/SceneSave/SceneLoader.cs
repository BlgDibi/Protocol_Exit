using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneData[] _sceneDataarray;

    private Dictionary<string, int> _sceneIdToIndexmap=new Dictionary<string, int>();

    private void Awake()
    {
        PopulateSceneMappings();
    }


    private void PopulateSceneMappings()
    {
        foreach (var sceneMapping in _sceneDataarray)
        {
            _sceneIdToIndexmap[SceneData_Save.UniqueName] = SceneData_Save.SceneIndex;
        
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
            Debug.Log("No scene");
        }

    
    }



}

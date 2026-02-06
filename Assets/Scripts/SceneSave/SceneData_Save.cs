using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName ="Scene Data", fileName ="New scene data")]

public class SceneData_Save : ScriptableObject
{
    public string UniqueName;
    public int SceneIndex;

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSave : MonoBehaviour
{
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveSystem.Save();
            Debug.Log("Gyorsmentés (K) sikeres!");
            //System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveSystem.Load();
            Debug.Log("Gyorsbetöltés (L) sikeres!");
        }
    }

    #region Save and Load
    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;
    }

    public void Load(PlayerSaveData data)
    {
        if (controller != null) controller.enabled = false;

        transform.position = data.Position;

        if (controller != null) controller.enabled = true;
    }
    #endregion
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
}
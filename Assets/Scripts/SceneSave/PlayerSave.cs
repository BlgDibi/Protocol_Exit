using UnityEngine;
using System.Collections; 

public class PlayerSave : MonoBehaviour
{
    private CharacterController controller;
    private HP playerHP;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerHP = GetComponent<HP>();
    }

    IEnumerator Start()
    {
        yield return null;

        if (SaveSystem.IsGameJustLoaded)
        {
            Debug.Log("Save inicialized, HP and position sync");

            PlayerSaveData data = SaveSystem.GetPlayerData();

            Load(data);

            SaveSystem.IsGameJustLoaded = false;
        }
    }

    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;

        if (playerHP != null)
        {
            data.Health = playerHP.Hp;
        }
        else
        {
            data.Health = 100f;
        }
    }

    public void Load(PlayerSaveData data)
    {
        if (controller != null) controller.enabled = false;

        transform.position = data.Position;

        Physics.SyncTransforms();

        if (playerHP != null)
        {
            playerHP.Hp = data.Health;
            if (playerHP.Hp > 0)
            {
                playerHP.isDead = false;
            }
        }

        Debug.Log("PlayerSave: Player TP-d here: " + data.Position + " HP: " + data.Health);

        if (controller != null) controller.enabled = true;
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
    public float Health;
}
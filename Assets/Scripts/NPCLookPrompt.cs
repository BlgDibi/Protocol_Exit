using UnityEngine;
public class NPCLookPrompt : MonoBehaviour 
{ 
    public Camera playerCam; 
    public GameObject prompt; 
    public float maxDistance = 3f; 
    void Update() 
    { 
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward); 
        RaycastHit hit; 
        if (Physics.Raycast(ray, out hit, maxDistance)) 
        { 
            if (hit.collider.CompareTag("NPC")) 
            { prompt.SetActive(true); 
                if (Input.GetKeyDown(KeyCode.T)) 
                { hit.collider.GetComponent<Talk>().StartDialogue(); 
                } 
            } 
            else 
            { 
                prompt.SetActive(false); 
            } 
        } 
        else 
        { 
            prompt.SetActive(false); 
        } 
    } 
}
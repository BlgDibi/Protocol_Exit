using UnityEngine;
using System.Collections;




public class ShadowPuzzleFinal : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public Vector3 solutionRotation;
    public float marginOfError = 5f;
    public float sensitivity = 100f;

    [Header("Camera and Player")]
    public GameObject puzzleCameraObj; 
    public GameObject playerObj;      
    public GameObject playerCameraObj; 

    [Header("Outcome")]
    public Light successLight;
    public Color solvedColor = Color.green;

    private bool isSolved = false;
    private bool puzzleActive = false;
    public GameObject playerVisuals;


    void Start()
    {
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);
    }


    public void Interact()
    {
        if (isSolved) return;

        if (!puzzleActive)
            EnterPuzzle();
        else
            ExitPuzzle();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && !puzzleActive)
        {
           
            float dist = Vector3.Distance(playerObj.transform.position, transform.position);
            if (dist < 3f)
            { 
                Interact();
            }
        }

        if (!puzzleActive || isSolved) return;

        if (Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
       

            transform.Rotate(Vector3.up, -rotX, Space.World);


            CheckIfSolved();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPuzzle();
        }
    }

    void EnterPuzzle()
    {
        playerVisuals.SetActive(false);

        puzzleActive = true;

      
        if (playerCameraObj != null) playerCameraObj.SetActive(false);
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(true);

       
        if (playerObj != null)
        {
   
            var controller = playerObj.GetComponent<MonoBehaviour>();
            if (controller != null) controller.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitPuzzle()
    {

        playerVisuals.SetActive(true);
        puzzleActive = false;

 
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);
        if (playerCameraObj != null) playerCameraObj.SetActive(true);

        if (playerObj != null)
        {
            var controller = playerObj.GetComponent<MonoBehaviour>();
            if (controller != null) controller.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

  
    void CheckIfSolved()
    {
        float angleDiff = Quaternion.Angle(transform.rotation, Quaternion.Euler(solutionRotation));

        if (angleDiff < marginOfError && !isSolved)
        {
            StartCoroutine(SolvedSequence());
        }
    }

    IEnumerator SolvedSequence()
    {
        isSolved = true;
        puzzleActive = false; 

        // 1. VISSZAJELZÉS: Lámpa felgyullad
        if (successLight != null)
        {
            successLight.color = solvedColor;
            successLight.enabled = true;
        }

        Debug.Log("Siker! Várakozás...");

        yield return new WaitForSeconds(2.0f);

        CompleteAndExit();
    }

    void CompleteAndExit()
    {
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);
        if (playerCameraObj != null) playerCameraObj.SetActive(true);

        if (playerObj != null)
        {
            playerObj.GetComponent<MonoBehaviour>().enabled = true;
            if (playerVisuals != null) playerVisuals.SetActive(true); // Modell vissza
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
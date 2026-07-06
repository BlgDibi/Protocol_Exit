using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DoorAnimation : MonoBehaviour
{
    public GameObject doorpivot;
    public AudioSource dooropen;
    public AudioSource doorclose;

    [Header("Transition Settings")]
    public int nextSceneIndex = 4;

    void Start()
    {
        StartCoroutine(dooropening());
    }

    IEnumerator dooropening()
    {
        yield return new WaitForSeconds(3);
        dooropen.Play();
        doorpivot.GetComponent<Animator>().Play("dooranim");
        yield return new WaitForSeconds(4);
        doorclose.Play();
        yield return new WaitForSeconds(1f);

     
        AsyncLoader.TargetSceneIndex = nextSceneIndex;

      
        SceneManager.LoadScene(1);
    }
}
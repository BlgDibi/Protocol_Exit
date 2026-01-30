using Unity.Mathematics;
using UnityEngine;

public class DoorMechanics : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float rotationSpeed = 2f;

    //Lock Settings
    [Header("Door Settings")]
    [SerializeField] private bool isLocked =false;
    [SerializeField] private Key requiredKey;

    //audio

    private AudioSource audiosource;
    [SerializeField] private AudioClip opensound;
    [SerializeField] private AudioClip closesound;
    [SerializeField] private AudioClip lockedsound;

    private bool isdooropen=false;
    private bool isanimating=false;
    private Quaternion targetrotation;


    [SerializeField] private GameObject endingTrigger;

    private void Start()
    {
        targetrotation= Quaternion.Euler(0f,closeAngle,0f);
        audiosource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isanimating) 
        {
            doorPivot.localRotation = Quaternion.Lerp(doorPivot.localRotation, targetrotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(doorPivot.localRotation,targetrotation) <0.1f)
            {
                doorPivot.localRotation = targetrotation;
                isanimating = false;
            }
        }
        

    }

    public void ToggleDoor() 
    {
        if (isLocked)
        {
            if (KeyInventory.Instance.HasKey(requiredKey))
            {
                Debug.Log($"Door '{gameObject.name}' unlocked with key {requiredKey.keyName}");
                isLocked = false;

                if (endingTrigger != null)
                {
                    endingTrigger.SetActive(true);
                }
              
                KeyInventory.Instance.RemoveKey(requiredKey);
            }
            else
            {
                PlaySound(lockedsound);
                Debug.Log($"Door '{gameObject.name}' is locked. You need key {requiredKey.keyName}");
                return;
            }
        }

        if (!isanimating)
        {
            isdooropen = !isdooropen;
            targetrotation = Quaternion.Euler(0f,isdooropen ? openAngle : closeAngle, 0f);
            isanimating = true;
            PlaySound(isdooropen ? opensound : closesound);
        }
    
    }


    void PlaySound(AudioClip clip) 
    {
        if (audiosource != null && clip != null)
        {
            audiosource.PlayOneShot(clip);
        }
    
    
    }


}

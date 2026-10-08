using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections;

public class HiddenKeyCollectable : MonoBehaviour, InterfaceInteractable
{
    [Header("Key Settings")]
    [SerializeField] private Key key;
    [SerializeField] private AudioClip pickupsound;
    [SerializeField] private AudioSource audiosource;

    [Header("Localization")]
    public LocalizedString textPickup;

    [Header("Required Condition")]
    public string requiredCondition = "SawMirrorClue";

    [Header("TV Event")]
    public TV_Event tvEventToTrigger;

    [SerializeField] private KeyCode interactionkey = KeyCode.E;

    void Start()
    {
        SetKeyVisible(false);
        StartCoroutine(CheckStatusAfterLoad());
    }

    void Update()
    {
        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(requiredCondition))
        {
            List<int> savedKeys = SaveSystem.GetHistoricalKeys();
            if (savedKeys == null || (key != null && !savedKeys.Contains(key.id)))
            {
                SetKeyVisible(true);
            }
        }
    }

    IEnumerator CheckStatusAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);
        if (key == null) yield break;

        List<int> savedKeys = SaveSystem.GetHistoricalKeys();

        if (savedKeys != null && savedKeys.Contains(key.id))
        {
            gameObject.SetActive(false);
            yield break;
        }

        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(requiredCondition))
        {
            SetKeyVisible(true);
        }
        else
        {
            SetKeyVisible(false);
        }
    }

    private void SetKeyVisible(bool isVisible)
    {
        if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = isVisible;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = isVisible;
    }

    public void Interact()
    {
        if (ConditionManager.Instance != null && !ConditionManager.Instance.CheckCondition(requiredCondition))
        {
            return;
        }

        if (key != null)
        {
            KeyInventory.Instance.AddKey(key);

            if (pickupsound != null && audiosource != null)
            {
                audiosource.PlayOneShot(pickupsound);
            }

            if (tvEventToTrigger != null)
            {
                tvEventToTrigger.TriggerTVEvent();
            }

            gameObject.SetActive(false);

        }
    }

    public string GetInteractText()
    {
        if (ConditionManager.Instance != null && !ConditionManager.Instance.CheckCondition(requiredCondition))
        {
            return "";
        }

        return textPickup.GetLocalizedString();
    }

    public KeyCode GetInteractKey()
    {
        return interactionkey;
    }
}